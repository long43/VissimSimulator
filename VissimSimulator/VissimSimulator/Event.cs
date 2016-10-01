using System;
using VS = VissimSimulator;

/// <summary>
/// Summary description for Class1
/// </summary>
namespace VissimSimulator
{
    public static class EventFactory
    {
        ///Decide which EventType and Event TimeSpan.
        public static EventType CreateEventType(int i)
        {
            if (i == 0)
            {
                return EventType.OnCall;
            }

            if (i != 0)
            {
                return EventType.PowerOn;
            }


        }
        public static VS.TimeSpan CreateTimeSpan(EventType)
        {
            if (EventType == OnCall)
            {
                return new VS.TimeSpan
                {
                    StartTick = 0,
                    EndTick = 3600
                };
            }
            if (EventType == OnCall)
            {
                return new VS.TimeSpan
                {
                    StartTick = 0,
                    EndTick = 1
                };
            }
        }
    }
}

    public class Event
    {
        public EventType EventType { get; set; }

        public Guid guid { get; set; }

        public VS.TimeSpan TimeSpan { get; set; }

        public Event() 
        {
            EventType = EventFactory.CreateEventType();

            TimeSpan = EventFactory.CreateTimeSpan();
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
    }
}


