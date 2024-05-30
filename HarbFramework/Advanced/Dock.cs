namespace Gruppe8.HarbNet.Advanced
{
    /// <summary>
    /// Dock used for docking ships to the harbor. There are multiple types of docks that inherit from this class suck as
    /// LoadingDocks and ShipDocks.
    /// </summary>
    internal abstract class Dock
    {
        /// <summary>
        /// Gets the unique ID for the dock.
        /// </summary>
        /// <returns>Returns a Guid object representing the docks unique ID.</returns>
        internal Guid ID { get; } = Guid.NewGuid();
        
        /// <summary>
        /// Gets the dock's size. Only ships with a coresponding size can dock to this dock.
        /// </summary>
        /// <returns>Returns a ShipSize enum representing the size of ships the dock can recieve.</returns>
        internal ShipSize Size { get; set; }
        
        /// <summary>
        /// Gets or sets wether or not the Dock is free for a ship to dock to it. The value is True if the dock is free to be docked to
        /// and False otherwise.
        /// </summary>
        /// <returns>Returns a boolean that is true if the dock is free to be docked to and false if it is not</returns>
        internal bool Free { get; set; }
        
        /// <summary>
        /// Gets or sets the ID of the ship docked to the dock.
        /// </summary>
        /// <returns>Returns a Guid representing the unique ID of the ship docked to this dock.</returns>
        internal Guid DockedShip { get; set; }

        /// <summary>
        /// Creates a new object of the Dock class. A dock is a place where a ship can Dock to the _harbor, either to load/unload its cargo
        /// or for the ship itself to be stored in the harbor area.
        /// </summary>
        ///  <param name="shipSize">The size of the Dock to be created. A dock can only recieve ships of the coresponding size as itself.</param>
        internal Dock(ShipSize shipSize)
        {
            Size = shipSize;
            Free = true;
        }
    }
}
