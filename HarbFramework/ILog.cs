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
        public DateTime time { get; }
        public ICollection<Ship> shipsInQueue { get; }
        public ICollection<Ship> shipsInTransit {  get; }
        public ICollection<Ship> DockedShips();
        public ICollection<Guid> ContainersInHarbour();
    }
}
