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
        DateTime time { get; }
        ICollection<Ship> shipsInQueue { get; }
        ICollection<Ship> shipsInTransit {  get; }
        ICollection<Ship> GetDockedShips();
        ICollection<Guid> GetContainersInHarbour();

    }
}
