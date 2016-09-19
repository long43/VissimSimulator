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
