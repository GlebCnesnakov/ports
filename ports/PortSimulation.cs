using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace ports
{
    class PortSimulation
    {
        class Channel
        {
            public bool b = true;
            public Ship ship;
            public int workingTime;
            public Channel() { }
        }
        Random rnd = new Random();
        Ship[] ships = new Ship[3000];
        //List<Ship> ships = new List<Ship> ();
        Queue<Ship> queue = new Queue<Ship>();
        int SimulationTime { get; } = 8640;
        int LeftBorder { get; } = 6;
        int RightBorder { get; } = 8;
        int[] arrivalTimesForShips = new int[3000];
        Channel[] channels = new Channel[3] { new(), new(), new()};

        int stormExpectation = 48;//мат ожидание 48 часов
        List<int> stormTimes = new List<int>();
        
        double[] workingTimeForChannels = new double[3];//массив для хранения времени работы каналов
        int[] countOfShips = new int[3];
        List<int> queueTimes = new List<int>(new int[3000]);
        //int shipCount = 0;
        public PortSimulation() //создаются корабли в соответствии с вероятностями
        {
            
            for(int i = 0; i < 3000; i++)
            {
                double random = rnd.NextDouble();
                if (random <= ArrivalChances.First.Chance)
                {
                    ships[i] = (new Ship(rnd.Next(ArrivalChances.First.LeftTime, ArrivalChances.First.RightTime), "Первый"));
                    //Console.WriteLine(ships[i].LoadTime + " 1"); 
                }
                else if (random > ArrivalChances.First.Chance && random <= ArrivalChances.Second.Chance)
                {
                    ships[i] = (new Ship(rnd.Next(ArrivalChances.Second.LeftTime, ArrivalChances.Second.RightTime), "Второй"));
                    //Console.WriteLine(ships[i].LoadTime + " 2");
                }
                else if (random > ArrivalChances.Second.Chance && random <= ArrivalChances.Third.Chance)
                {
                    ships[i] = (new Ship(rnd.Next(ArrivalChances.Third.LeftTime, ArrivalChances.Third.RightTime), "Третий"));
                    //Console.WriteLine(ships[i].LoadTime + " 3");
                }
                //WriteLine(ships.Length + " ships count");


                double timeForStorms = Storm.GetExponentialTime();//определение времён наступлений штормов
                if (stormTimes.Count > 0)
                {
                    int j = (int)Math.Round(timeForStorms) + stormTimes[stormTimes.Count - 1]; //через сколько наступит новый + время начала старого
                    //if (j == stormTimes[stormTimes.Count - 1])//шторм не начинается если еще идёт другой шторм
                    //{
                    //    continue;
                    //}
                    if (j < SimulationTime)
                    {
                        stormTimes.Add(j);
                        int stormDuration = rnd.Next(Storm.LeftBorder, Storm.RightBorder);//добавление времён фактического шторма

                        for(int b = 1; b <= stormDuration;b++)
                        {
                            stormTimes.Add(j + b);
                        }
                    }
                }
                else
                {
                    stormTimes.Add((int)Math.Round(timeForStorms)); //lобавляем первый элемент если список пуст
                    int stormDuration = rnd.Next(Storm.LeftBorder, Storm.RightBorder);//добавление времён фактического шторма
                    
                    for (int b = 1; b <= stormDuration; b++)
                    {
                        stormTimes.Add((int)timeForStorms + b);
                    }
                }
                
            }
            //for (int i = 0; i < stormTimes.Count; i++)
            //{
            //    WriteLine(stormTimes[i]);
            //}
        }
        bool TakeChannel(bool IsThereNewShip, int nextArrivalTimeForShipiterator, int currTime)//попробовать занять канал
        {
            for(int i = 0; i < channels.Length; i++)
            {
                
                if (channels[i].b == true)//свободен
                {
                    if (queue.Count == 0) { // в очереди никого
                        //когда первый встречный канал свободен, помещаем очередной корабль в канал, закрываем канал,
                        //WriteLine(countOfShips[0] + countOfShips[1] + countOfShips[2]);
                        channels[i].ship = ships[countOfShips[0] + countOfShips[1] + countOfShips[2]];
                    }
                    else
                    {
                        channels[i].ship = queue.Dequeue();//берем из очереди
                                                                        //время прибытия
                        queueTimes[nextArrivalTimeForShipiterator] = arrivalTimesForShips[nextArrivalTimeForShipiterator] - currTime;

                        //если есть новый корабль и вышло так, что каналы были заняты всем кораблыми из очереди
                        if (IsThereNewShip && i == channels.Length - 1)
                        {
                            WriteLine("Зашел когда три");
                            queue.Enqueue(ships[countOfShips[0] + countOfShips[1] + countOfShips[2]]);
                        }
                    }
                    channels[i].b = false;
                    //countOfShips[i]++;
                    //shipCount++;
                    return true;
                }
            }
            return false;//если все заняты
        }
        void IncreaseChannelsWorkingTime(int time)
        {
            for(int i = 0; i < 3; i++)
            {
                if (!channels[i].b) channels[i].workingTime += time;
            }
            
        }
        void FreeChannel(int channelNumber)//освободить канал
        {
            channels[channelNumber].b = true;
            
            channels[channelNumber].ship = null!;
        }
        int counter = 0;
        public void Simulate()
        {
            int firstChannelLoadingCounter = 0;
            int secondChannelLoadingCounter = 0;
            int thirdChannelLoadingCounter = 0;
            int nextArrivalTimeForShipiterator = 0;
            for (int i = 0; i < 3000; i++)
            {
                //считаем время прибытия нового корабля
                if (i != 0)
                {
                    arrivalTimesForShips[i] = arrivalTimesForShips[i - 1] + rnd.Next(LeftBorder, RightBorder);// * (RightBorder - LeftBorder) + LeftBorder);
                }
                else
                {
                    arrivalTimesForShips[i] = rnd.Next(LeftBorder, RightBorder);// * (RightBorder - LeftBorder) + LeftBorder);
                }
                //Console.WriteLine(arrivalTimesForShips[i]);
            }
            
            for (int i = 0; i < SimulationTime; i++)//через час
            {
                for (int u = 0; u < channels.Length; u++)
                {
                    //Channel c = channels[u]; // Получаем канал
                    if (channels[u].b == false) // Канал занят
                    {
                        switch (channels[u].ship.ShipType)//проверка на освобождение канала
                        {
                            case "Первый":
                                firstChannelLoadingCounter++;
                                if (channels[u].ship.LoadTime <= firstChannelLoadingCounter) // если время загрузки закончилось
                                {
                                    WriteLine(channels[u].ship.LoadTime);
                                    WriteLine(firstChannelLoadingCounter);
                                    FreeChannel(u);   // освободить канал
                                    countOfShips[0]++;
                                    firstChannelLoadingCounter = 0;
                                }
                                break;
                            case "Второй":
                                secondChannelLoadingCounter++;
                                if (channels[u].ship.LoadTime <= secondChannelLoadingCounter)
                                {
                                    WriteLine(channels[u].ship.LoadTime);
                                    WriteLine(secondChannelLoadingCounter);
                                    FreeChannel(u);
                                    countOfShips[1]++;
                                    secondChannelLoadingCounter = 0;
                                }
                                break;
                            case "Третий":
                                thirdChannelLoadingCounter++;
                                if (channels[u].ship.LoadTime <= thirdChannelLoadingCounter)
                                {
                                    WriteLine(channels[u].ship.LoadTime);
                                    WriteLine(thirdChannelLoadingCounter);
                                    FreeChannel(u);
                                    countOfShips[2]++;
                                    thirdChannelLoadingCounter = 0;
                                }
                                break;
                            
                        }
                    }
                }
                if ((i < arrivalTimesForShips[nextArrivalTimeForShipiterator]) || stormTimes.Contains(i))//новый корабль не прибыл
                {
                    
                    if (queue.Count != 0)//в очереди кто то есть
                    {
                        TakeChannel(false, nextArrivalTimeForShipiterator, i);
                    }
                    //else // в очереди кто то есть
                    //{
                    //    //queue.Enqueue(ships[countOfShips[0] + countOfShips[1] + countOfShips[2]]);
                    //    TakeChannel(false, nextArrivalTimeForShipiterator, i);
                    //}
                }
                else // корабль прибыл
                {
                    if (!TakeChannel(true, nextArrivalTimeForShipiterator, i))//все каналы заняты
                    {
                        queue.Enqueue(ships[countOfShips[0] + countOfShips[1] + countOfShips[2]]);//все каналы заняты - в очередь корабль
                    }
                    nextArrivalTimeForShipiterator++;
                }
                IncreaseChannelsWorkingTime(1);
                counter++;
            }



            Console.WriteLine("Всего кораблей" + (countOfShips[0] + countOfShips[1] + countOfShips[2]));
            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine("Работа канала: " + (i + 1) + " " + channels[i].workingTime);
                Console.WriteLine("Простой канала: " + (i + 1) + " " + (8640 - channels[i].workingTime));
                
            }
            double sr = 0;
            double h = 0;
            Console.WriteLine("Типы: " + countOfShips[0] + " " + countOfShips[1] + " " + countOfShips[2]);
            for (int i = 0; i < queueTimes.Count; i++)
            {
                //Console.WriteLine(queueTimes[i]);
                if (queueTimes[i] > 0)
                {
                    sr += queueTimes[i];
                    h++;
                }
            }
            Console.WriteLine("Среднее " + sr / h);
        }
    }
}
