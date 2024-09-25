using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

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
        //int shipCount = 0;
        public PortSimulation() //создаются корабли в соответствии с вероятностями
        {
            
            for(int i = 0; i < 3000; i++)
            {
                double random = rnd.NextDouble();
                if (random <= ArrivalChances.First.Chance)
                {
                    ships[i] = new Ship(rnd.Next(ArrivalChances.First.LeftTime, ArrivalChances.First.RightTime), "Первый");
                }
                else if (random > ArrivalChances.First.Chance && random <= ArrivalChances.Second.Chance)
                {
                    ships[i] = new Ship(rnd.Next(ArrivalChances.Second.LeftTime, ArrivalChances.Second.RightTime), "Второй");
                }
                else if (random > ArrivalChances.Second.Chance && random <= ArrivalChances.Third.Chance)
                {
                    ships[i] = new Ship(rnd.Next(ArrivalChances.Third.LeftTime, ArrivalChances.Third.RightTime), "Третий");
                }

                double timeForStorms = Storm.GetExponentialTime();//определение времён наступлений штормов
                if (stormTimes.Count > 0)
                {
                    int j = (int)timeForStorms + stormTimes[stormTimes.Count - 1]; //последний элемент из списка
                    if (j == stormTimes[stormTimes.Count - 1])
                    {
                        continue;
                    }
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
                    stormTimes.Add((int)timeForStorms); //lобавляем первый элемент если список пуст
                    int stormDuration = rnd.Next(Storm.LeftBorder, Storm.RightBorder);//добавление времён фактического шторма
                    
                    for (int b = 1; b <= stormDuration; b++)
                    {
                        stormTimes.Add((int)timeForStorms + b);
                    }
                }
            }
        }
        bool TakeChannel(bool IsThereNewShip)//попробовать занять канал
        {
            for(int i = 0; i < 3; i++)
            {
                if (channels[i].b == true)
                {
                    if (queue.Count == 0) { // в очереди никого
                        //когда первый встречный канал свободен, помещаем очередной корабль в канал, закрываем канал,
                        channels[i].ship = ships[countOfShips[0] + countOfShips[1] + countOfShips[2]];
                    }
                    else
                    {
                        channels[i].ship = queue.Dequeue();//берем из очереди
                        if (IsThereNewShip)//если есть новый корабль, то в очередь
                            queue.Enqueue(ships[countOfShips[0] + countOfShips[1] + countOfShips[2]]);
                    }
                    channels[i].b = false;
                    countOfShips[i]++;
                    //shipCount++;
                    return true;
                }
            }
            return false;
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
            }
            
            for (int i = 0; i < SimulationTime; i++)//через час
            {
                
                

                
                for (int u = 0; u < channels.Length; u++)
                {
                    Channel c = channels[u]; // Получаем канал
                    if (!c.b) // Канал занят
                    {
                        switch (c.ship.ShipType)
                        {
                            case "Первый":
                                firstChannelLoadingCounter++;
                                if (c.ship.LoadTime <= firstChannelLoadingCounter) // если время загрузки закончилось
                                {
                                    FreeChannel(u);   // освободить канал
                                    countOfShips[0]++;
                                    firstChannelLoadingCounter = 0;
                                }
                                break;
                            case "Второй":
                                secondChannelLoadingCounter++;
                                if (c.ship.LoadTime <= secondChannelLoadingCounter)
                                {
                                    FreeChannel(u);
                                    countOfShips[1]++;
                                    secondChannelLoadingCounter = 0;
                                }
                                break;
                            case "Третий":
                                thirdChannelLoadingCounter++;
                                if (c.ship.LoadTime <= thirdChannelLoadingCounter)
                                {
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
                    
                    if (queue.Count == 0)//и в очереди никого -> ничего не происходит
                    {
                        //if (!TakeChannel())//все каналы заняты
                        //{
                        //    queue.Enqueue(ships[countOfShips[0] + countOfShips[1] + countOfShips[2]]);
                        //}
                    }
                    else // в очереди кто то есть
                    {
                        //queue.Enqueue(ships[countOfShips[0] + countOfShips[1] + countOfShips[2]]);
                        TakeChannel(false);
                    }
                }
                else // корабль прибыл
                {
                    if (!TakeChannel(true))//все каналы заняты
                    {
                        queue.Enqueue(ships[countOfShips[0] + countOfShips[1] + countOfShips[2]]);
                    }
                    nextArrivalTimeForShipiterator++;
                }

                

                IncreaseChannelsWorkingTime(1);

            }
            Console.WriteLine(countOfShips[0] + countOfShips[1] + countOfShips[2]);
            
        }
    }
}
