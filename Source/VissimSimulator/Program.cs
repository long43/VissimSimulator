using System;
using VISSIMLIB;
using System.IO;
namespace VissimSimulator
{
    public class Program
    {
        public static void Main()
        {
            EventSimulator simulator = new EventSimulator();
            simulator.Run();
            simulator.Exit();
        }
    }
}
