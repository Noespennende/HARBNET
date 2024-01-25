using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarbFramework
{
    internal class Dock
    {
        internal Guid id = new Guid();
        internal ShipSize size { get; set; }
        internal bool free { get; set; }
        internal Guid dockedShip {  get; set; }

        internal Dock (ShipSize shipSize)
        {
            this.size = shipSize;
            this.free = true;
        }

        internal Guid getID()
        {
            return id;
        }

    }
}
