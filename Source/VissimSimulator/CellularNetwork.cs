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

        public Location GetLocation(string locationId)
        {
            return network[locationId];
        }

        public void AddLocation(Location location)
        {
            network.Add(location.LocationId, location);
        }

        public bool ContainsLocation(string locationId)
        {
            return network.ContainsKey(locationId);
        }

        public IEnumerable<string> FindLinksByLocationId(string locationId)
        {
            return from cell in network[locationId].Cells
                   from link in cell.Links
                   select link;
        }

        public IEnumerable<CellTower> FindCellTowersByLocationId(string locationId)
        {
            return from cell in network[locationId].Cells
                   select cell;
        }

        public IEnumerable<string> FindLinksByCellId(string cellId)
        {
            return from location in network.Values
                   from cell in location.Cells
                   from link in cell.Links
                   where cell.CellTowerId == cellId
                   select link;
        }
             
        public Location FindLocationByLinkId(string linkId)
        {
            return (from location in network.Values
                    from cell in location.Cells
                   where cell.Links.Contains(linkId)
                   select location).FirstOrDefault();
        }

        public CellTower FindCellTowerByLinkId(string linkId)
        { 
            return (from location in network.Values
                    from cell in location.Cells
                    where cell.Links.Contains(linkId)
                    select cell).FirstOrDefault();
        }

        public Location FindLocationByCellId(string cellId)
        {
            return (from location in network.Values
                    where location.ContainsCell(cellId)
                    select location).FirstOrDefault();
        }
    }
}
