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
            ///Load Vissim net work
            VissimSimulator.LoadNet(@"C:\Users\Public\Documents\PTV Vision\PTV Vissim 6\Examples Demo\Urban Intersection Beijing.CN\Intersection Beijing.inpx");
            ///Read table contains Cellular tower information and correspoing link information
            var cellTowerInformation = new StreamReader(File.OpenRead(@"C:\test.csv"));
            CellularTowerEvent cte = new CellularTowerEvent();
            while (!cellTowerInformation.EndOfStream)
            {
                var line = cellTowerInformation.ReadLine();
                var values = line.Split(';');

                cte.CellularTowerId = CellTower.AddLink(Int32.Parse(values[0]));
                cte.LocationId = Location.AddCellTower(Int32.Parse(values[1]));
            }
            ///Generate the random event, when vehicle passing a fixed location and the Timespan is satisfied.
            foreach (IVehicle vehicle in vissim.Net.Vehicles)
            {
                ///Select Random Vehicle
                int vehiclePossible = rnd.Next(0, 10);
                //Only Selected Vehicle can generate the Event.
                if (vehiclePossible == 0)
                {
                    Event evet = new Event();
                    ///Create random event type.
                    int i = rnd.Next(0, 1);
                    evet.TimeSpan = EventFactory.CreateTimeSpan();
                    evet.EventType = EventFactory.CreateEventType;
                    cte.Event = evet;
                }
                ///record the event information 
                CollectorWorker collect = new CollectorWorker();
                if (vehicle.Location == cte.CellTowerId & cte.Event.TimeSpan == simulationTime)
                {
                    collect.ProcessEvent(cte);
                }
            }
            simulator.Exit();
        }
    }
}
