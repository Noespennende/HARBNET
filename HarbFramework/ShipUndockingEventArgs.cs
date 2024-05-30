namespace Gruppe8.HarbNet
{
    /// <summary>
    /// The EventArgs class for the ShipUndocked event. This event is raised when a ship undocks from any dock.
    /// </summary>
    public class ShipUndockingEventArgs : BaseShipEventArgs
    {
        /// <summary>
        /// The unique ID of the location the ship undocked from, Anchorage or Dock.
        /// </summary>
        /// <returns>Guid object representing the ID of the location the ship undocked from.</returns>
        public Guid LocationID { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the ShipUndockingEventArgs class.
        /// </summary>
        /// <param name="ship">The ship object involved in the event.</param>
        /// <param name="currentTime">The current date and time in the simulation.</param>
        /// <param name="locationID">The unique ID of the location the ship undocked from.</param>
        /// <param name="description">String value containing a description of the event.</param>
        public ShipUndockingEventArgs(Ship ship, DateTime currentTime, string description, Guid locationID)
            : base(ship, currentTime, description)
        {
            LocationID = locationID;
        }
    }
}
