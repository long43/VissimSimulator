using System;
using VS = VISSIMSimulator;

/// <summary>
/// Summary description for Class1
/// </summary>
namespace VISSIMSimulator
{
    public static class EventFactory
    { 
        public static EventType CreateEventType()
        {
            return EventType.OnCall;
        }

        public static VS.TimeSpan CreateTimeSpan()
        {
            return new VS.TimeSpan
            {
                StartTick = 0,
                EndTick = 3600
            };
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

