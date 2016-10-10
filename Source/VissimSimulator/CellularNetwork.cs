using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace VissimSimulator
{
    public class CellularNetwork
    {
        private Dictionary<string, Location> network;

        public ICollection<Location> Locations
        {
            get 
            {
                return network.Values;
            }
        }

        public CellularNetwork()
        {
            network = new Dictionary<string, Location>();
        }

        /// <summary>
        /// Read the csv files to initialize the cellular network
        /// The format of the CellLinkRelation file is as follows:
        /// LINK_ID,CELLID,LAC
        /// </summary>
        /// <param name="networkFilePath">csv file of the cellular network definition</param>
        /// <param name="delimiter">delimiter</param>
        public void LoadFromFile(string networkFilePath, char delimiter)
        {
            //read the cell-location relation file
            using (StreamReader cellLinkReader = new StreamReader(File.OpenRead(networkFilePath)))
            {
                //skip the header line
                string line = cellLinkReader.ReadLine();

                //read the rest of the file
                while ((line = cellLinkReader.ReadLine()) != null)
                {
                    string[] values = line.Split(delimiter);

                    string locationId = values[2];
                    string cellId = values[1];
                    string linkId = values[0];
                    Location location;

                    if (!this.ContainsLocation(locationId))
                    {
                        location = new Location(locationId);
                        //if the Location is new location, then the cell must be new cell
                        CellTower cell = new CellTower();
                        cell.CellTowerId = cellId;
                        //if the cell is a new cell, then the link must be a new link
                        cell.Links.Add(linkId);

                        location.AddCellTower(cell);
                        //add the location to the cellular network
                        this.AddLocation(location);
                    }
                    else
                    { 
                        //if this location pre exists
                        location = this.GetLocation(locationId);
                        //check if the cell also exists
                        if (location.ContainsCell(cellId))
                        {
                            //if cell exists
                            CellTower cell = location.GetCell(cellId);

                            //check if link exists, most likely it doesn't exist otherwise the file is corrupted
                            if (cell.Links.Contains(linkId))
                            {
                                throw new Exception("the link is already exists");
                            }

                            cell.AddLink(linkId);
                        }
                        else //if cell doesn't pre exist
                        {
                            CellTower cell = new CellTower(cellId);
                            cell.AddLink(linkId);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get the Location from the location Id
        /// </summary>
        /// <param name="locationId"></param>
        /// <returns></returns>
        public Location GetLocation(string locationId)
        {
            return network[locationId];
        }

        /// <summary>
        /// Add a location to the cellular network
        /// </summary>
        /// <param name="location">Location</param>
        public void AddLocation(Location location)
        {
            network.Add(location.LocationId, location);
        }

        /// <summary>
        /// Check if the cellular network contains a given location
        /// </summary>
        /// <param name="locationId">location id</param>
        /// <returns>true if contains the location, otherwise false</returns>
        public bool ContainsLocation(string locationId)
        {
            return network.ContainsKey(locationId);
        }

        /// <summary>
        /// Find all the links covered by a given location
        /// </summary>
        /// <param name="locationId">location id</param>
        /// <returns>Enumeration of the link ids</returns>
        public IEnumerable<string> FindLinksByLocationId(string locationId)
        {
            return from cell in network[locationId].Cells
                   from link in cell.Links
                   select link;
        }

        /// <summary>
        /// Find all the cells covered by a given location
        /// </summary>
        /// <param name="locationId">location id</param>
        /// <returns>Enumeration of the cell towers</returns>
        public IEnumerable<CellTower> FindCellTowersByLocationId(string locationId)
        {
            return from cell in network[locationId].Cells
                   select cell;
        }

        /// <summary>
        /// Find all links covered by a given cell
        /// </summary>
        /// <param name="cellId">cell id</param>
        /// <returns>Enumeration of the link ids</returns>
        public IEnumerable<string> FindLinksByCellId(string cellId)
        {
            return from location in network.Values
                   from cell in location.Cells
                   from link in cell.Links
                   where cell.CellTowerId == cellId
                   select link;
        }
        
        /// <summary>
        /// Find the location that cover a given link. In theory, a link can only be covered by one location
        /// If there are multiple locations that covers a link, then it must be something wrong in the network
        /// </summary>
        /// <param name="linkId">link id</param>
        /// <returns>Location</returns>
        public Location FindLocationByLinkId(string linkId)
        {
            return (from location in network.Values
                    from cell in location.Cells
                   where cell.Links.Contains(linkId)
                   select location).FirstOrDefault();
        }

        /// <summary>
        /// Find the cell that cover a given link. In theory, a link can only be covered by one cell
        /// If there are multiple cells that covers a link, then it must be something wrong in the network
        /// </summary>
        /// <param name="linkId">link id</param>
        /// <returns>CellTower</returns>
        public CellTower FindCellTowerByLinkId(string linkId)
        { 
            return (from location in network.Values
                    from cell in location.Cells
                    where cell.Links.Contains(linkId)
                    select cell).FirstOrDefault();
        }

        /// <summary>
        /// Find a location with a given cell id
        /// </summary>
        /// <param name="cellId">cell id</param>
        /// <returns>Location</returns>
        public Location FindLocationByCellId(string cellId)
        {
            return (from location in network.Values
                    where location.ContainsCell(cellId)
                    select location).FirstOrDefault();
        }
    }
}
