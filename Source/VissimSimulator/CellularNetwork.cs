using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace VissimSimulator
{
    public class CellularNetwork
    {
        #region private fields
        private Dictionary<string, Location> network;
        #endregion //private fields

        #region public methods
        public ICollection<Location> Locations
        {
            get 
            {
                return network.Values;
            }
        }

        public IDictionary<string, Link> Links
        {
            get 
            {
                return (from location in network.Values
                        from cell in network[location.LocationId].Cells
                        from link in cell.Links.Values
                        select link).ToDictionary(x => x.LinkId);
            }
        }

        public IDictionary<string, CellTower> Cells
        {
            get
            {
                return (from location in network.Values
                        from cell in network[location.LocationId].Cells
                        select cell).ToDictionary(x => x.CellTowerId);
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
                        CellTower cell = new CellTower(cellId, locationId);
                        //if the cell is a new cell, then the link must be a new link
                        cell.AddLink(linkId);

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
                            if (cell.Links.ContainsKey(linkId))
                            {
                                throw new Exception("the link is already exists");
                            }

                            cell.AddLink(linkId);
                        }
                        else //if cell doesn't pre exist
                        {
                            CellTower cell = new CellTower(cellId, locationId);
                            cell.AddLink(linkId);
                            location.AddCellTower(cell);
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
        /// Find the location that cover a given link. In theory, a link can only be covered by one location
        /// If there are multiple locations that covers a link, then it must be something wrong in the network
        /// </summary>
        /// <param name="linkId">link id</param>
        /// <returns>Location</returns>
        public Location FindLocationByLinkId(string linkId)
        {
            if (!string.IsNullOrEmpty(linkId) && Links.ContainsKey(linkId))
            {
                if (Cells.ContainsKey(Links[linkId].CellId))
                {
                    return GetLocation(Cells[Links[linkId].CellId].LocationId);
                }
            }
            return null;
        }

        /// <summary>
        /// Find the cell that cover a given link. In theory, a link can only be covered by one cell
        /// If there are multiple cells that covers a link, then it must be something wrong in the network
        /// </summary>
        /// <param name="linkId">link id</param>
        /// <returns>CellTower</returns>
        public CellTower FindCellTowerByLinkId(string linkId)
        {
            if (!string.IsNullOrEmpty(linkId) && Links.ContainsKey(linkId))
            {
                if (Cells.ContainsKey(Links[linkId].CellId))
                {
                    return Cells[Links[linkId].CellId];
                }
            }
            return null;
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

        #endregion //public methods
    }
}
