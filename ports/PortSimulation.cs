using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
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
            public int LoadingCounter = 0;
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
        int[] countOfShips = new int[4];
        List<int> queueTimes = new List<int>(new int[3000]);





        //
        Ship[] fourthships = new Ship[5];
        int[] arrivalTimesForFourthShips = new int[5] { 8641, 8641, 8641, 8641, 8641 };
        int timesWhenShipFourthDone = int.MaxValue;
        bool IsShipInPortA = false; //стоит ли корабль в портах, если оба false - поступает новый корабль 4 типа
        bool IsShipInPortB = false;
        bool IsShip4Exists = false;
        
        //int nextTimeFourthShipGeneration = 0;



        public PortSimulation() //создаются корабли в соответствии с вероятностями
        {
            //объявляем корабли 4 типа
            fourthships[0] = new Ship(rnd.Next(ArrivalChances.Fourth.LeftTime, ArrivalChances.Fourth.RightTime), "Четвертый", 2990, rnd.Next(216, 264), rnd.Next(18, 24), rnd.Next(18, 24), rnd.Next(18, 24));
            fourthships[1] = new Ship(rnd.Next(ArrivalChances.Fourth.LeftTime, ArrivalChances.Fourth.RightTime), "Четвертый", 2991, rnd.Next(216, 264), rnd.Next(18, 24), rnd.Next(18, 24), rnd.Next(18, 24));
            fourthships[2] = new Ship(rnd.Next(ArrivalChances.Fourth.LeftTime, ArrivalChances.Fourth.RightTime), "Четвертый", 2992, rnd.Next(216, 264), rnd.Next(18, 24), rnd.Next(18, 24), rnd.Next(18, 24));
            fourthships[3] = new Ship(rnd.Next(ArrivalChances.Fourth.LeftTime, ArrivalChances.Fourth.RightTime), "Четвертый", 2993, rnd.Next(216, 264), rnd.Next(18, 24), rnd.Next(18, 24), rnd.Next(18, 24));
            fourthships[4] = new Ship(rnd.Next(ArrivalChances.Fourth.LeftTime, ArrivalChances.Fourth.RightTime), "Четвертый", 2994, rnd.Next(216, 264), rnd.Next(18, 24), rnd.Next(18, 24), rnd.Next(18, 24));
            
            for (int i = 0; i < 3000; i++)
            {
                double random = rnd.NextDouble();
                if (random <= ArrivalChances.First.Chance)
                {
                    ships[i] = (new Ship(rnd.Next(ArrivalChances.First.LeftTime, ArrivalChances.First.RightTime), "Первый", i));
                    //Console.WriteLine(ships[i].LoadTime + " 1"); 
                }
                else if (random > ArrivalChances.First.Chance && random <= ArrivalChances.Second.Chance)
                {
                    ships[i] = (new Ship(rnd.Next(ArrivalChances.Second.LeftTime, ArrivalChances.Second.RightTime), "Второй", i));
                    //Console.WriteLine(ships[i].LoadTime + " 2");
                }
                else if (random > ArrivalChances.Second.Chance && random <= ArrivalChances.Third.Chance)
                {
                    ships[i] = (new Ship(rnd.Next(ArrivalChances.Third.LeftTime, ArrivalChances.Third.RightTime), "Третий", i));
                    //Console.WriteLine(ships[i].LoadTime + " 3");
                }
                

                //штормы
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
            
        }
        bool TakeChannel(bool IsThereNewShip, int currTime, Ship newShip, int nextFourthShip = 0)//попробовать занять канал
        {
            for(int i = 0; i < channels.Length; i++)
            {
                if (newShip.TimePortA != 0)//если 4 тип пробует занять канал значит он в порту
                {
                    IsShip4Exists = true;
                }
                if (channels[i].b == true)//свободен
                {
                    if (queue.Count == 0)
                    { // в очереди никого
                        channels[i].ship = newShip;
                        channels[i].ship.f = true;
                        if (channels[i].ship.TimePortA != 0)
                        {
                            arrivalTimesForFourthShips[nextFourthShip] = 8641;
                        }

                    }
                    else
                    {
                        channels[i].ship = queue.Dequeue();//берем из очереди
                        channels[i].ship.f = true;                      //время прибытия

                        if (channels[i].ship.TimePortA != 0)//проверка на 4 тип
                        {
                            arrivalTimesForFourthShips[nextFourthShip] = 8641;
                            WriteLine("eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee");
                            //для кораблей 4 типа отдельно
                            queueTimes[channels[i].ship.ID] = currTime - channels[i].ship.ArrivalTime;
                        }
                        else
                        {
                            queueTimes[channels[i].ship.ID] = currTime - channels[i].ship.ArrivalTime;// минус время прибытия корабля
                        }
                        
                        WriteLine($"Текущее время{currTime} - Время прибытия {channels[i].ship.ArrivalTime} - ID {channels[i].ship.ID} - A {channels[i].ship.TimePortA}"); 
                        //если есть новый корабль и вышло так, что каналы были заняты всем кораблями из очереди
                        if (IsThereNewShip && i == channels.Length - 1)
                        {
                            queue.Enqueue(newShip);
                        }
                    }
                    channels[i].b = false;
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
        
        public void Simulate()
        {
            int nextArrivalTimeForShipiterator = 0;
            int nextFourthShip = 0;

            

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
                ships[i].SetTime(arrivalTimesForShips[i]);
                //Console.WriteLine(arrivalTimesForShips[i]);
            }
            
            for (int i = 0; i < SimulationTime; i++)//через час
            {
                if (i > timesWhenShipFourthDone)//можем делать новый 4 танкер
                {
                    IsShipInPortA = false;
                    IsShipInPortB = false;
                    IsShip4Exists = false;
                    timesWhenShipFourthDone = int.MaxValue;
                }
                //генерация времени прибытия нового танкера если корабля 4 типа нет
                if (!IsShipInPortA && !IsShipInPortB && nextFourthShip < 5) 
                {
                    arrivalTimesForFourthShips[nextFourthShip] = i + (fourthships[nextFourthShip].TimePortB + fourthships[nextFourthShip].TimeSailing);
                    fourthships[nextFourthShip].SetTime(arrivalTimesForFourthShips[nextFourthShip]);
                    nextFourthShip++;
                    IsShipInPortB = true;
                    IsShipInPortA = false;
                    
                }

                for (int u = 0; u < channels.Length; u++)
                {
                    if (channels[u].b == false) // Канал занят
                    {
                        channels[u].LoadingCounter++;
                        if (channels[u].ship.LoadTime <= channels[u].LoadingCounter) // если время загрузки закончилось
                        {
                            switch (channels[u].ship.ShipType)//проверка на освобождение канала
                            {
                                case "Первый":
                                    countOfShips[0]++;
                                    break;
                                case "Второй":
                                    countOfShips[1]++;
                                    break;
                                case "Третий":
                                    countOfShips[2]++;
                                    break;
                                case "Четвертый":
                                    countOfShips[3]++;//4 тип+
                                    WriteLine("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF");
                                    IsShipInPortA = true;
                                    IsShipInPortB = false;
                                    
                                    //когда танкер идет в B ставим время когда он закончит
                                    timesWhenShipFourthDone = i + fourthships[nextFourthShip - 1].TimeSailing + fourthships[nextFourthShip - 1].TimePortB2;
                                    arrivalTimesForFourthShips[nextFourthShip - 1] = 8641;
                                    break;
                            }
                            FreeChannel(u);   // освободить канал
                            channels[u].LoadingCounter = 0;// обнулить счётчик
                        }
                    }
                }
                if (i < arrivalTimesForFourthShips[nextFourthShip - 1] || stormTimes.Contains(i))
                {
                    //никто не прибыл
                    if (queue.Count > 0)//в очереди кто то есть
                    {
                        TakeChannel(false, i, fourthships[nextFourthShip - 1]);
                    }
                }
                else
                {
                    //корабль прибыл
                    if (!TakeChannel(true, i, fourthships[nextFourthShip - 1], nextFourthShip - 1))//все каналы заняты
                    {
                        
                        queue.Enqueue(fourthships[nextFourthShip - 1]);
                        arrivalTimesForFourthShips[nextFourthShip - 1] = 8641;
                    }


                }

                if (i < arrivalTimesForShips[nextArrivalTimeForShipiterator] || stormTimes.Contains(i))
                {
                    if (queue.Count > 0)//в очереди кто то есть
                    {
                        TakeChannel(false, i, ships[nextArrivalTimeForShipiterator]);
                    }
                }
                else
                {
                    if (!TakeChannel(true, i, ships[nextArrivalTimeForShipiterator]))//все каналы заняты
                    {
                        queue.Enqueue(ships[nextArrivalTimeForShipiterator]);
                    }
                    nextArrivalTimeForShipiterator++;
                }
                //if (((i < arrivalTimesForShips[nextArrivalTimeForShipiterator]) && (i < arrivalTimesForFourthShips[nextFourthShip - 1])) || stormTimes.Contains(i))//новый корабль не прибыл
                //{

                //    if (queue.Count > 0)//в очереди кто то есть
                //    {
                //        TakeChannel(false, i, ships[nextArrivalTimeForShipiterator]);
                //    }

                //}
                //else // корабль прибыл
                //{
                //                    // в случае прибытия 4 типа
                //    if (arrivalTimesForShips[nextArrivalTimeForShipiterator] == arrivalTimesForFourthShips[nextFourthShip - 1] || i == arrivalTimesForFourthShips[nextFourthShip - 1])
                //    {
                //        if (!TakeChannel(true, i, fourthships[nextFourthShip - 1]))//все каналы заняты
                //        {
                //            queue.Enqueue(fourthships[nextFourthShip - 1]);
                //        }
                //        //IsShipInPortA = true;
                //        //IsShipInPortB = false;
                //    }
                //                    //в случае прибытия 1 2 3 типов
                //    else
                //    {
                //        if (!TakeChannel(true, i, ships[nextArrivalTimeForShipiterator]))//все каналы заняты
                //        {
                //            queue.Enqueue(ships[nextArrivalTimeForShipiterator]);
                //        }
                //        nextArrivalTimeForShipiterator++;
                //    }

                //}
                IncreaseChannelsWorkingTime(1);
                
            }



            Console.WriteLine("Всего кораблей" + (countOfShips[0] + countOfShips[1] + countOfShips[2]));
            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine("Работа канала: " + (i + 1) + " " + channels[i].workingTime);
                Console.WriteLine("Простой канала: " + (i + 1) + " " + (8640 - channels[i].workingTime));
                
            }
            double sr = 0;
            double h = 0;
            Console.WriteLine("Типы: " + countOfShips[0] + " " + countOfShips[1] + " " + countOfShips[2] + " " + countOfShips[3]);
            for (int i = 0; i < queueTimes.Count; i++)
            {
                //WriteLine(queueTimes[i]);
                if (queueTimes[i] > 0)
                {
                    sr += queueTimes[i];
                    h++;
                }
            }
            WriteLine("Среднее время пребывания танкера в очереди: " + sr / h);
            //Console.WriteLine("Среднее " + sr / h);
            //WriteLine("Времена прибытя в кораблях");
            //for(int i = 0; i < ships.Length; i ++)
            //{
            //    Console.WriteLine(ships[i].ArrivalTime + " " + ships[i].ID);
            //}
            
            //for(int i = 0; i < ships.Count(); i++)
            //{
            //    if (ships[i].f == true)
            //    {
            //        WriteLine(ships[i].ID);
            //    }
            //}
        }
    }
}


/*
 В порту А танкеры загружаются сырой нефтью, которую морским
путем доставляют по назначению. Мощности порта позволяют загружать не
более трех танкеров одновременно. Танкеры, прибывающие в порт через
каждые 1it7 ч (все величины, заданные интервалом„ распределены
равномерно), относятся к трем различным типам. Значения относительной
частоты появления танкеров данного типа и времени, требуемого на их
погрузку, приведены ниже:
Тип Относительная
частота Время погрузки [ч]
1
2
3
0,25
0,55
0,20
18  2
24  3
35  4
Судовладелец предлагает заключить контракт с дирекцией порта В.
При этом обеспечить перевозку нефти с помощью пяти танкеров особого,
четвертого типа, которые на погрузку требуют 21  3 ч. После погрузки
танкер отчаливает и следует в пункт А, там загружается и снова
возвращается в пункт В для погрузки. Время цикла обращения танкера,
включая время разгрузки, составляет 240  24 ч.
Факторам, осложняющим перевозку нефти, являются штормы, которым
подвергается порт. Интервал времени между штормами распределен
экспоненциально с математическим ожиданием 48 ч, причем шторм
продолжается 4  2 ч. Во время шторма танкеры не причаливают.
Перед принятием окончательного решения дирекция порта решила
определить влияние, которое окажут пять дополнительных танкеров на время
пребывания в порту остальных судов. Выводы решено сделать по
результатам имитации работы порта в течение 1 года (8640 ч)
*/