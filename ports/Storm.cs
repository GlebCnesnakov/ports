using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ports
{
    static class Storm
    {
        public static int Excpectation { get; } = 48;
        public static int LeftBorder { get; } = 2;
        public static int RightBorder { get; } = 6;
        public static double GetExponentialTime(double lambda = 1 / 48.0)
        {
            Random rnd = new Random();
            return -Math.Log(rnd.NextDouble()) / lambda;
        }
        
    }
}
