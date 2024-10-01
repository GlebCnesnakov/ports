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
        //public int EnterTime { get; set; }
        //public int ExitTime { get; set; }
        //public int ArrivalTime { get; set; }
        public Ship() { }
        public Ship(int loadTime,  string shipType)
        {
            LoadTime = loadTime;
            //ArrivalTime = arrivalTime;
            ShipType = shipType;
        }
    }
}
