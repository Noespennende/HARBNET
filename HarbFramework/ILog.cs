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
        DateTime Time { get; }
        ICollection<Ship> ShipsInQueue { get; }
        ICollection<Ship> ShipsInTransit {  get; }
        ICollection<Ship> GetDockedShips();
        ICollection<Guid> GetContainersInHarbour();

    }
}
