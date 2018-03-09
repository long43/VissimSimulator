using System;
using System.Collections.Generic;
using System.Linq;
using VS = VissimSimulator;

namespace VissimSimulator
{
    public class VehicleEvent
    {
        #region private fields
        private Dictionary<Guid, Event> events = new Dictionary<Guid, Event>();
        #endregion //private fields

        #region public properties
        public string VehicleId { get; private set; }

        public string CurLinkId { get; set; }

        public string PreLinkId { get; set; }

        public int EventCount
        {
            get
            {
                return events.Count();
            }
        }

        #endregion //public properties

        #region public methods
        public VehicleEvent(string vehicleId)
        {
            VehicleId = vehicleId;
        }

        public VehicleEvent(string vehicleId, string linkId)
        {
            VehicleId = vehicleId;
            CurLinkId = linkId;
        }

        public VehicleEvent(string vehicleId, string curLinkId, string preLinkId)
        {
            VehicleId = vehicleId;
            CurLinkId = curLinkId;
            PreLinkId = preLinkId;
        }

        /// <summary>
        /// Get an active event from this vehicle
        /// </summary>
        /// <param name="currentTicks">current time tick</param>
        /// <returns>Event</returns>
        public IEnumerable<Event> GetActiveEvent(long currentTicks)
        {
            return events.Values.Where(x => x.IsActive(currentTicks));
        }

        /// <summary>
        /// Get a future event from this vehicle
        /// </summary>
        /// <param name="currentTicks">current time tick</param>
        /// <returns>Event</returns>
        public Event GetFutureOnCallEvent(long currentTicks)
        {
            return events.Values.Where(x => x.TimeSpan.StartTick > currentTicks && x.EventType == EventType.OnCall).FirstOrDefault();
        }

        /// <summary>
        /// If this vehicle has OnCall event?
        /// </summary>
        /// <returns>True if it has, otherwise false</returns>
        public bool HasOnCallEvent()
        {
            return events.Values.Where(x => x.EventType == EventType.OnCall).Any();
        }

        /// <summary>
        /// Add a power-on event to this vehicle
        /// </summary>
        public void AddPowerOnEvent()
        {
            Event evet = new Event(EventType.PowerOn);
            events.Add(evet.guid, evet);
        }

        /// <summary>
        /// Add a on-call event to this vehicle
        /// </summary>
        /// <param name="currentTick">Current time tick</param>
        public void AddOnCallEvent(long currentTick)
        {
            long upperBoundTime = (EventSimulator.SimulationTicks / EventSimulator.DetectionInterval) + 1;
            Random rnd = new Random();
            //set the timespan range. start tick will always be current range to
            long startTick = rnd.Next((int)currentTick / 30 + 1, Math.Min((int)upperBoundTime, (int)currentTick / 30 + 2));
            long endTick = startTick + rnd.Next(60, Math.Max((int)(upperBoundTime - startTick), 120));
            Event evet = new Event(EventType.OnCall, new VS.TimeSpan(startTick * 30, endTick * 30));
            events.Add(evet.guid, evet);
        }

        /// <summary>
        /// Remove an event from this vehicle
        /// </summary>
        /// <param name="evt">Event</param>
        public void removeEvent(Event evt)
        {
            events.Remove(evt.guid);
        }

        #endregion //public methods
    }
}
