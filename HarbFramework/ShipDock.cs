using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe8.HarbNet
{
    /// <summary>
    /// ShipDock to be used in simulation.
    /// </summary>
    internal class ShipDock : Dock
    {
        /// <summary>
        /// Creates new ShipDock object.
        /// </summary>
        /// <param name="shipSize">ShipSize enum representing the size of ships the ShipDock to be created can hold.</param>
        internal ShipDock(ShipSize shipSize) : base(shipSize)
        {
            this.Size = shipSize;
            this.Free = true;
        }
    }
}
