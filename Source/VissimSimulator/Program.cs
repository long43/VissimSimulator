
namespace VissimSimulator
{
    /// <summary>
    /// Main entrance of the simulation program. 
    /// To run this simulation, please remove the above // before the "using VISSIMLIB"
    /// and comment out the entire VissimInterface.cs file. 
    /// </summary>
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
