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
        public EventType EventType { get; set; }

        public Guid guid { get; set; }

        public VS.TimeSpan TimeSpan { get; set; }

        public Event(EventType type)
        {
            EventType = type;

            TimeSpan = new VS.TimeSpan(0, 3600);
        }

        public Event(EventType type, TimeSpan timeSpan)
        {
            EventType = type;

            TimeSpan = TimeSpan;
        }

        public bool IsActive(long currentTick)
        {
            if (TimeSpan.StartTick <= currentTick && TimeSpan.EndTick >= currentTick)
            {
                return true;
            }
            return false;
        }
    }

    public enum EventType
    {
        PowerOn = 0,

        OnCall = 1,
    }

    public class TimeSpan
    {
        public long StartTick { get; set; }

        public long EndTick { get; set; }

        public TimeSpan(long start, long end)
        {
            StartTick = start;
            EndTick = end;
        }
    }
}


