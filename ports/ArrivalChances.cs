using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace ports
{
    
    static class ArrivalChances
    {
        public struct Charasteristics
        {
            public double Chance;
            public int LeftTime;
            public int RightTime;
            public Charasteristics(double chance, int x, int y)
            {
                this.Chance = chance;
                this.LeftTime = x;
                this.RightTime = y;
            }
        }
        public static Charasteristics First = new(0.25, 16, 20);
        public static Charasteristics Second = new(0.8, 21, 27);
        public static Charasteristics Third = new(1, 31, 39);   
    }
}
