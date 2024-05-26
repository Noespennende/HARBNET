using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe8.HarbNet
{
    /// <summary>
    /// Enum representing the size of ships
    /// </summary>
    public enum ShipSize
    {
        /// <summary>
        /// No size
        /// </summary>
        None = 0,
        /// <summary>
        /// Small ship. base weight: 5000 tonns, container capacity: 20.
        /// </summary>
        Small = 5000,
        /// <summary>
        /// Medium ship, base weight: 50000 tonns, container capacity: 50
        /// </summary>
        Medium = 50000,
        /// <summary>
        /// Large ship, base weight: 100000 tonns, container capacity: 100
        /// </summary>
        Large = 100000
    }
}
