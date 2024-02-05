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
        public IList<Ship> ShipsInQueue { get; }
        public IList<Ship> ShipsInTransit { get; }
        public IList<Ship> DockedShips { get; }
        public IList<Guid> ContainersInHarbour { get; }
        
    }
}
