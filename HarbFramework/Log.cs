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

        public DateTime time => throw new NotImplementedException();

        ICollection<Ship> ILog.DockedShips()
        {
            List<Ship> dockedShipsList = new List<Ship>();
            foreach (var ship in DockedShips.Values)
            {
                dockedShipsList.Add((Ship)ship);
            }
            return dockedShipsList;
        }

        ICollection<Guid> ILog.ContainersInHarbour()
        {
            List<Guid> containersList = new List<Guid>();
            foreach(Container container in ContainersInHarbour.Values)
            {
                containersList.Add(container.ID);
            }
            return containersList;
        }
    }
}
