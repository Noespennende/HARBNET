using harbNet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarbFramework
{
    public class Log : ILog
    {
        public DateTime Time { get; set; }
        internal Hashtable DockedShips { get; set; }
        public ICollection<Ship> ShipsInQueue { get; internal set; }
        public ICollection<Ship> ShipsInTransit { get; internal set; }
        internal Hashtable ContainersInHarbour { get; set; }

        internal Log(DateTime time, Hashtable dockedShips, ICollection<Ship> shipsInQueue, Hashtable containersInHarbour, ICollection<Ship> shipsInTransit ) { 
            this.Time = time;
            this.ContainersInHarbour = containersInHarbour;
            this.ShipsInQueue = shipsInQueue;
            this.ShipsInTransit = shipsInTransit;
            this.DockedShips = dockedShips;
        }

        public ICollection<Ship> GetDockedShips()
        {
            ICollection<Ship> ships = new List<Ship>();
            foreach ( Ship ship in DockedShips.Values ) {
                ships.Add( ship );
            }

            return ships;
        }

        public ICollection<Guid> GetContainersInHarbour()
        {
            ICollection<Guid> containers = new List<Guid>();
            foreach (Container container in ContainersInHarbour.Values )
            {
                containers.Add(container.ID);
            }
            return containers;
        }
    }
}
