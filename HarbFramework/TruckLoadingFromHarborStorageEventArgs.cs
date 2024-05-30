namespace Gruppe8.HarbNet
{
    /// <summary>
    /// The EventArgs class for the TruckLoadingFromStorage event. This event is raised when a truck is loading a container from the harbor's storage
    /// </summary>
    public class TruckLoadingFromHarborStorageEventArgs : EventArgs
    {
        /// <summary>
        /// The truck involved in the event.
        /// </summary>
        /// <returns>Returns a truck object that is involved in the event in the simulation.</returns>
        public Truck Truck { get; internal set; }

        /// <summary>
        /// The current time in the simulation.
        /// </summary>
        /// <returns>Returns a DateTime object representing the current date and time in the simulation.</returns>
        public DateTime CurrentTime { get; internal set; }

        /// <summary>
        /// A description of the event.
        /// </summary>
        /// <returns>Returns a string value containing a description of the event.</returns>
        public string Description { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the TruckLoadingFromStorageEvent.
        /// </summary>
        /// <param name="truck">The truck object involved in the event.</param>
        /// <param name="currentTime">The current date and time in the simulation.</param>
        /// <param name="description">String value containing a description of the event.</param>
        public TruckLoadingFromHarborStorageEventArgs(Truck truck, DateTime currentTime, string description)
        {
            Truck = truck;
            CurrentTime = currentTime;
            Description = description;
        }
    }
}
