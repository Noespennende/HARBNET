using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarbFramework
{
    /// <summary>
    /// Dock for docking ships to the harbor
    /// </summary>
    internal class Dock
    {
        /// <summary>
        /// Gets the unique ID for the dock
        /// </summary>
        /// <returns>Returns a Guid representing the docks unique ID</returns>
        internal Guid ID { get; } = Guid.NewGuid();
        /// <summary>
        /// Gets or sets the unique ID for the dock
        /// </summary>
        /// <returns>Returns a ShipSize enum representing the size of ships the dock can recieve/returns>
        internal ShipSize Size { get; set; }
        /// <summary>
        /// Gets or sets wether or not the Dock is free to recieve a ship or not.
        /// </summary>
        /// <returns>Returns a boolean that is true if the dock is free and false if it is not</returns>
        internal bool Free { get; set; }
        /// <summary>
        /// Gets or sets the ID of the ship docked to the dock.
        /// </summary>
        /// <returns>Returns a Guid representing the unique ID of the ship docked to this dock</returns>
        internal Guid DockedShip {  get; set; }

        /// <summary>
        /// Creates a new dock object
        /// </summary>
        ///  <param name="shipSize">Size of the ships that the dock will allow to dock to it</param>
        internal Dock (ShipSize shipSize)
        {
            this.Size = shipSize;
            this.Free = true;
        }

    }
}
