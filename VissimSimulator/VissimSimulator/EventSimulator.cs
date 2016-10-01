using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using VISSIMLIB;

namespace VissimSimulator
{
    public class EventSimulator
    {
        public Dictionary<int, Location> CellularNetwork = new Dictionary<int,Location>();

        public Dictionary<int, VehicleEvent> VehicleEvents = new Dictionary<int,VehicleEvent>();

        public ConcurrentQueue<CellularTowerEvent> CellularTowerEvents = new ConcurrentQueue<CellularTowerEvent>();

        public List<CollectorWorker> CollectorWorkers;

        public void Run()
        {
            Vissim vissim = new Vissim();
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
            ///Make the program check the all vehicle in Vissim network every 1 sec.
            for (int i = 0; i < simulationTime; i++)
            {
                vissim.Simulation.RunSingleStep();
                foreach (CollectorWorker worker in CollectorWorkers)
                {
                    Task workerTask = Task.Run(() =>
                    {
                        worker.ProcessEvent(CellularTowerEvents);
                    });

                    workerTask.Wait();
                }
            }
        }
        public void Exit()
        {
            vissim.Exit();
        }
    }

   

    public class VehicleEvent
    {
        public int Vehicleid { get; set; }
        public int VehicleLink { get; set; }
        public Dictionary<Guid, Event> Events = new Dictionary<Guid, Event>();
        public void addEvent(Event events){
            Events.Add(events.guid, events);
        }
        public void removeEvent(Event events)
        {
            Events.Remove(events.guid);
        }
    }

    public class CellularTowerEvent
    {
        public int LocationId { set; get; }
        public int CellularTowerId { set; get; }
        public Event Event;
    }



    public class CollectorWorker
    {
        public void ProcessEvent(ConcurrentQueue<CellularTowerEvent> cellularTowerEvents)
        {
            CellularTowerEvent evt = null;
            if (cellularTowerEvents.TryDequeue(out evt))
            {
                process(evt);
            }
        }

        private void process(CellularTowerEvent evt)
        {


        }
    }
}
