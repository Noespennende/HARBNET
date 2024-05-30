using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gruppe8.HarbNet.Advanced;

namespace Gruppe8.HarbNet
{
    /// <summary>
    /// ShipDock to be used in a simulation. A shipDock is a dock where ships can be stored once they are finished with their
    /// trip and have no new trips to complete. If no shipdocks are available in the harbor a ship will go to the anchorage instead.
    /// </summary>
    internal class ShipDock : Dock
    {
        /// <summary>
        /// Creates a new object of the ShipDock class.
        /// </summary>
        /// <param name="shipSize">ShipSize enum representing the size of the ships that can dock to the shipdock. A dock can only
        /// hold ships of a coresponding size</param>
        internal ShipDock(ShipSize shipSize) : base(shipSize)
        {
            Size = shipSize;
            Free = true;
        }
    }
}
