using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OracleClient;

namespace VissimSimulator
{
    public class CollectorWorker
    {
        /// <summary>
        /// This is the main method of the CollectorWork's business logic.
        /// It should at least do something like:
        /// 1. Persist the event into either file or DB
        /// 2. Do some aggregation that might be helpful for the research
        /// </summary>
        /// <param name="evt"></param>
        public void Process(CellularTowerEvent evt)
        {
            //at least we need to persisit the CellularTowerEvent
            TryCreateTbale();
            //read the data into database;
            while (true)
            {
                try
                {
                    int locationId = int.Parse(evt.LocationId);
                    int cellularTowerId = int.Parse(evt.CellularTowerId);
                    string eventType = Convert.ToString(evt.Event.EventType);
                    //TODO Which time we should use?
                    //Is the data format correct? Does C# have the data type like Timestamp for SQL?
                    string eventTimeSpan = string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", evt.Tick);
                    AddEvent(locationId, cellularTowerId, eventType, eventTimeSpan);
                }
                catch
                {
                    Console.WriteLine("Input error");
                }
            }
        }
        /// <summary>
        /// This method attempts to create the OUTPUT SQL table.
        /// If will do nothing but print an error if the table already exists.
        /// </summary>
        public void TryCreateTbale()
        {
            using (OracleConnection con = new OracleConnection(
               ///TODO add the right connection path for SQL database
               "user id=system;" +
               "password=its123; server=serverurl;" +
               "Trusted_Connection=yes;" +
               "database=database; " +
               "connection timeout=30"
                ))
            {
                con.Open();
                try
                {
                    using (OracleCommand command = new OracleCommand(
                        "CREATE TBALE OUTPUT1(LocationId INT, CellularTowerId INT, EventType TEXT, EventTimeSpan TEXT)", con))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                catch
                {
                    Console.WriteLine("Table not created.");
                }
            }
        }
        /// <summary>
        /// Insert output data into the SQL database table.
        /// </summary>
        /// <param name="locatioanId">The location id of the cell station.</param>
        /// <param name="cellularTowerId">The cell id of the cell station.</param>
        /// <param name="eventType">The type of the event.</param>
        /// <param name="eventTimeSpan">The time of the event when it occurs.</param>
        static void AddEvent(int locationId, int cellularTowerId, string eventType, string eventTimeSpan)
        {
            using (OracleConnection con = new OracleConnection(
                ///TODO add the right connection path for SQL database
                ))
            {
                con.Open();
                try
                {
                    using (OracleCommand command = new OracleCommand(
                        "INSERT INTO OUTPUT VALUES(@LocationId, @CellularTowerId, @EventType, @EventTimeSpan)", con))
                    {
                        command.Parameters.Add(new OracleParameter("LocationId", locationId));
                        command.Parameters.Add(new OracleParameter("CellularTowerId", cellularTowerId));
                        command.Parameters.Add(new OracleParameter("EventType", eventType));
                        command.Parameters.Add(new OracleParameter("EventTimeSpan", eventTimeSpan));
                        command.ExecuteNonQuery();
                    }
                }
                catch
                {
                    Console.WriteLine("Count not insert.");
                }
            }
        }
    }
}
