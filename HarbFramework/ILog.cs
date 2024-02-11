using harbNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarbFramework
{
    public interface ILog
    {
        public DateTime Time { get; }
        public IList<Ship> ShipsInAnchorage { get; }
        public IList<Ship> ShipsInTransit { get; }
        public IList<Ship> ShipsDockedInLoadingDocks { get; }
        public IList<Ship> ShipsDockedInShipDocks { get; }
        public IList<Container> ContainersInHarbour { get; }
        public void PrintInfoForAllShips();
        public void PrintInfoForAllContainers();

    }
}
