using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ports
{
    class Ship
    {
        public string ShipType { get; set; }
        public int LoadTime { get; set; }
        public int TimeSailing { get; set; }
        public int TimePortA {  get; set; }
        public int TimePortB {  get; set; }
        public int TimePortB2 {  get; set; }
        public int ID {  get; set; }
        //public int EnterTime { get; set; }
        //public int ExitTime { get; set; }
        public int ArrivalTime { get; set; }
        public bool f = false;
        
        public void SetTime(int arrivalTime)
        {
            ArrivalTime = arrivalTime;
        }
        public Ship(int loadTime,  string shipType, int iD, int plavaet = 0, int timeA = 0, int timeB = 0, int timeB2 = 0)
        {
            LoadTime = loadTime;
            //ArrivalTime = arrivalTime;
            ShipType = shipType;
            ID = iD;
            TimeSailing = plavaet / 2;//время в одну сторону
            TimePortA = timeA;
            TimePortB = timeB;
            TimePortB2 = timeB2;
        }
    }
}
