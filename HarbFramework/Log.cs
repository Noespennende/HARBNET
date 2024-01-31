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
        public DateTime time { get; set; }
        internal Hashtable dockedShips { get; set; }
        public ICollection<Ship> shipsInQueue { get; internal set; }
        public ICollection<Ship> shipsInTransit { get; internal set; }
        internal Hashtable containersInHarbour { get; set; }

        internal Log(DateTime time, Hashtable dockedShips, ICollection<Ship> shipsInQueue, Hashtable containersInHarbour, ICollection<Ship> shipsInTransit ) { 
            this.time = time;
            this.containersInHarbour = containersInHarbour;
            this.shipsInQueue = shipsInQueue;
            this.shipsInTransit = shipsInTransit;
            this.dockedShips = dockedShips;
        }

        public ICollection<Ship> GetDockedShips()
        {
            ICollection<Ship> ships = new List<Ship>();
            foreach ( Ship ship in dockedShips.Values ) {
                ships.Add( ship );
            }

            return ships;
        }

        public ICollection<Guid> GetContainersInHarbour()
        {
            ICollection<Guid> containers = new List<Guid>();
            foreach (Container container in containersInHarbour.Values )
            {
                containers.Add(container.id);
            }
            return containers;
        }
    }
}
