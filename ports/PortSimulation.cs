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
            //public Channel(bool b, Ship ship)
            //{
            //    this.b = b;
            //    this.ship = ship;
            //}
        }
        Random rnd = new Random();
        Ship[] ships = new Ship[3000];
        Queue<Ship> queue = new Queue<Ship>();
        int SimulationTime { get; } = 8640;
        int LeftBorder { get; } = 6;
        int RightBorder { get; } = 8;
        int[] arrivalTimesForShips = new int[3000];
        Channel[] channels = new Channel[3];
        
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
            }
        }
        bool TakeChannel()//занять канал
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
        void FreeChannel(int channelNumber, string type)//освободить канал
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
                foreach (Channel c in channels)//проверка каналов на наличие корабля, отплытие, счетчики часов пребывания
                {
                    int u = 0;
                    if (!c.b)//канал занят
                    {
                        switch(c.ship.ShipType)//в завимиости от типа кораблся прибавляем к счетчику загрузки 1 час
                        {
                            case "Первый":
                                firstChannelLoadingCounter++;
                                if (c.ship.LoadTime >= firstChannelLoadingCounter)//если время загрузки сравнялось
                                {
                                    FreeChannel(u, "Первый");   //освободить канал и +1 в счетчик
                                    countOfShips[0]++;
                                    firstChannelLoadingCounter = 0;
                                }
                                break;
                            case "Второй":
                                secondChannelLoadingCounter++;
                                if (c.ship.LoadTime >= secondChannelLoadingCounter)//если время загрузки сравнялось
                                {
                                    FreeChannel(u, "Второй");   //освободить канал и +1 в счетчик
                                    countOfShips[1]++;
                                    secondChannelLoadingCounter = 0;
                                }
                                break;
                            case "Третий":
                                thirdChannelLoadingCounter++;
                                if (c.ship.LoadTime >= thirdChannelLoadingCounter)//если время загрузки сравнялось
                                {
                                    FreeChannel(u, "Третий");   //освободить канал и +1 в счетчик
                                    countOfShips[2]++;
                                    thirdChannelLoadingCounter = 0;
                                }
                                break;
                        }
                    }
                    u++;
                }
                
                
                if (i < arrivalTimesForShips[nextArrivalTimeForShipiterator])//новый корабль не прибыл
                {
                    if (queue.Count == 0)//и в очереди никого
                    {
                        if (!TakeChannel())//все каналы заняты
                        {
                            queue.Enqueue(ships[countOfShips[0] + countOfShips[1] + countOfShips[2]]);
                        }
                    }
                    else
                    {
                        queue.Enqueue(ships[countOfShips[0] + countOfShips[1] + countOfShips[2]]);
                    }
                }
                else // корабль прибыл
                {
                    if (!TakeChannel())//все каналы заняты
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
