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
        public IList<Ship> ShipsInAnchorage { get; internal set; }
        public IList<Ship> ShipsInTransit { get; internal set; }
        public IList<Container> ContainersInHarbour { get; internal set; }
        public IList<Ship> ShipsDockedInLoadingDocks { get; internal set; }
        public IList<Ship> ShipsDockedInShippingDocks { get; internal set; }

        internal Log(DateTime time, IList<Ship> shipsInAnchorage, IList<Ship> shipsInTransit, IList<Container> containersInHarbour, IList<Ship> shipsDockedInLoadingDocks, IList<Ship> ShipsDockedInShippingDocks)
        {
            this.Time = time;
            this.ShipsDockedInLoadingDocks = shipsDockedInLoadingDocks;
            this.ShipsInTransit = shipsInTransit;
            this.ContainersInHarbour = containersInHarbour;
            this.ShipsDockedInLoadingDocks = shipsDockedInLoadingDocks;
            this.ShipsDockedInShippingDocks = ShipsDockedInShippingDocks;
        }

    }
}
