using System;
using System.Linq;
using System.Collections.Generic;
using VS = VissimSimulator;

/// <summary>
/// Summary description for Class1
/// </summary>
namespace VissimSimulator
{
    public class Event
    {
        #region public properties
        ///<summary>Event Type</summary>
        public EventType EventType { get; private set; }

        ///<summary>Event ID</summary>
        public Guid guid { get; private set; }

        ///<summary>Event timespan</summary>
        public VS.TimeSpan TimeSpan { get; private set; }
        #endregion //public properties

        #region public methods
        /// <summary>
        /// Constructor - Construct the event for a given event type
        /// </summary>
        /// <param name="type">EventType</param>
        public Event(EventType type)
        {
            EventType = type;

            TimeSpan = new VS.TimeSpan(0, 3600);
        }

        /// <summary>
        /// Constructor - Construct the event for a given event type and timespan
        /// </summary>
        /// <param name="type">EventType</param>
        /// <param name="timeSpan">TimeSpan</param>
        public Event(EventType type, TimeSpan timeSpan)
        {
            EventType = type;

            TimeSpan = TimeSpan;
        }

        /// <summary>
        /// Is this event an active event?
        /// The criteria here is if the event is happening, then it's active, according to its defined timespan
        /// </summary>
        /// <param name="currentTick">current time tick</param>
        /// <returns>True if it's active, otherwise false</returns>
        public bool IsActive(long currentTick)
        {
            if (TimeSpan.StartTick <= currentTick && TimeSpan.EndTick >= currentTick)
            {
                return true;
            }
            return false;
        }
        #endregion //public methods
    }

    public enum EventType
    {
        PowerOn = 0,  //Cell phone is powerd on

        OnCall = 1,  //Cell phone is on call
    }

    public class TimeSpan
    {
        ///<summary>Start time</summary>
        public long StartTick { get; private set; }

        ///<summary>End time</summary>
        public long EndTick { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="start">start time tick</param>
        /// <param name="end">end time tick</param>
        public TimeSpan(long start, long end)
        {
            StartTick = start;
            EndTick = end;
        }
    }
}


