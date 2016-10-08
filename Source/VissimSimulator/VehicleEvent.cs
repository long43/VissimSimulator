using System;
using System.Collections.Generic;
using System.Linq;
using VS = VissimSimulator;

namespace VissimSimulator
{
    public class VehicleEvent
    {
        public string Vehicleid { get; set; }
        public string VehicleLink { get; set; }
        public Dictionary<Guid, Event> Events = new Dictionary<Guid, Event>();

        public Event GetActiveEvent(long currentTicks)
        {
            return Events.Values.Where(x => x.IsActive(currentTicks)).FirstOrDefault();
        }

        public Event GetFutureOnCallEvent(long currentTicks)
        {
            return Events.Values.Where(x => x.TimeSpan.StartTick > currentTicks && x.EventType == EventType.OnCall).FirstOrDefault();
        }

        public bool HasOnCallEvent()
        {
            return Events.Values.Where(x => x.EventType == EventType.OnCall).Any();
        }

        public void AddPowerOnEvent()
        {
            Event evet = new Event(EventType.PowerOn);
            this.Events.Add(evet.guid, evet);
        }

        public void AddOnCallEvent(long currentTick)
        {
            Random rnd = new Random();
            //set the timespan range. start tick will always be current range to 
            long endTick = rnd.Next((int)currentTick + 60, 3600);
            Event evet = new Event(EventType.OnCall, new VS.TimeSpan(currentTick, endTick));
            this.Events.Add(evet.guid, evet);
        }

        public void removeEvent(Event events)
        {
            Events.Remove(events.guid);
        }
    }
}
