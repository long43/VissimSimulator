using System;
using System.Collections.Concurrent;
using System.IO;
using System.Configuration;
using System.Data.SqlClient;

namespace VissimSimulator
{
    public class CollectorWorker
    {
        private string filePath;
        private BlockingCollection<CellularTowerEvent> cellularTowerEvents;

        public CollectorWorker(string filePath, BlockingCollection<CellularTowerEvent> cellularTowerEvents)
        {
            this.filePath = filePath;
            this.cellularTowerEvents = cellularTowerEvents;
        }

        public void Run()
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (CellularTowerEvent cEvent in cellularTowerEvents.GetConsumingEnumerable())
                {
                    Process(writer, cEvent);
                }
            }
        }

        /// <summary>
        /// This is the main method of the CollectorWork's business logic.
        /// It should at least do something like:
        /// 1. Persist the event into either file or DB
        /// 2. Do some aggregation that might be helpful for the research
        /// </summary>
        /// <param name="evt"></param>
        public void Process(StreamWriter writer, CellularTowerEvent evt)
        {
            string eventType = Convert.ToString(evt.Event.EventType);
            long eventTimeSpan = evt.CurrentTick;
            AddEvent(writer, evt.IMSI, evt.LocationId, evt.CellularTowerId, eventType, eventTimeSpan);
        }

        /// <summary>
        /// Insert output data into the SQL database table.
        /// </summary>
        /// <param name="locatioanId">The location id of the cell station.</param>
        /// <param name="cellularTowerId">The cell id of the cell station.</param>
        /// <param name="eventType">The type of the event.</param>
        /// <param name="eventTimestamp">The time of the event when it occurs.</param>
        //static void AddEvent(int locationId, int cellularTowerId, string eventType, long eventTimeTick)
        //{
        //    using (SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["SqlConnectionString"]))
        //    {
        //        con.Open();
        //        try
        //        {
        //            using (SqlCommand command = new SqlCommand(
        //                "INSERT INTO OUTPUT VALUES(@LocationId, @CellularTowerId, @EventType, @EventTimeSpan)", con))
        //            {
        //                command.Parameters.Add(new SqlParameter("LocationId", locationId));
        //                command.Parameters.Add(new SqlParameter("CellularTowerId", cellularTowerId));
        //                command.Parameters.Add(new SqlParameter("EventType", eventType));
        //                command.Parameters.Add(new SqlParameter("EventTimeSpan", eventTimeTick));
        //                command.ExecuteNonQuery();
        //            }
        //        }
        //        catch
        //        {
        //            Console.WriteLine("Count not insert.");
        //        }
        //    }
        //}

        /// <summary>
        /// Insert output data into the SQL database table.
        /// </summary>
        /// <param name="locatioanId">The location id of the cell station.</param>
        /// <param name="cellularTowerId">The cell id of the cell station.</param>
        /// <param name="eventType">The type of the event.</param>
        /// <param name="eventTimestamp">The time of the event when it occurs.</param>
        public void AddEvent(StreamWriter writer, string IMSI, string locationId, string cellularTowerId, string eventType, long eventTimeTick)
        {
            Console.WriteLine(string.Format("{0},{1},{2},{3},{4}", IMSI, locationId, cellularTowerId, eventType, eventTimeTick));
            writer.WriteLine(string.Format("{0},{1},{2},{3},{4}", IMSI, locationId, cellularTowerId, eventType, eventTimeTick));
        }
    }
}
