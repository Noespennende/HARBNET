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
        internal Guid ID { get; } = new Guid();
        internal ShipSize Size { get; set; }
        internal bool Free { get; set; }
        internal Guid DockedShip {  get; set; }

        internal Dock (ShipSize shipSize)
        {
            this.Size = shipSize;
            this.Free = true;
        }

    }
}
