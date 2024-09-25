

using System.Linq.Expressions;

namespace ports
{
    class Porgram
    {
        public static void Main(string[] args)
        {
            PortSimulation psm = new PortSimulation();
            psm.Simulate();
            //Random rnd = new Random();
            //double lambda = 1 / 15.0;
            //for(int i = 0; i < 10;i++)
            //    Console.WriteLine(-Math.Log(rnd.NextDouble()) / lambda);
        }
    }
}