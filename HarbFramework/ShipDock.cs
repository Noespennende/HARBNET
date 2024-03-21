using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe8.HarbNet
{
    internal class ShipDock : Dock
    {
        internal ShipDock(ShipSize shipSize) : base(shipSize)
        {
            this.Size = shipSize;
            this.Free = true;
        }
    }
}
