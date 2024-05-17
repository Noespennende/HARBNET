using System.Collections.ObjectModel;

namespace Gruppe8.HarbNet.PublicApiAbstractions
{
    /// <summary>
    /// Abstract class defining the public API for CargoVessels such as the Ship class.
    /// </summary>
    public abstract class CargoVessel
    {
        /// <summary>
        /// Gets the unique ID for the ship.
        /// </summary>
        /// <returns>Returns a Guid object representing the ships unique ID.</returns>
        public abstract Guid ID { get; }
        /// <summary>
        /// Gets the ships size. 
        /// </summary>
        /// <returns>Returns a ShipSize enumm representing the ships size.</returns>
        public abstract ShipSize ShipSize { get; internal set; }
        /// <summary>
        /// Gets the ships name. 
        /// </summary>
        /// <returns>Returns a string value representing the ships name.</returns>
        public abstract String Name { get; internal set; }
        /// <summary>
        /// Gets the date and time the ship first started its voyage.
        /// </summary>
        /// <returns>Returns a DateTime object representing the date and time the ship first started its voyage.</returns>
        public abstract DateTime StartDate { get; internal set; }
        /// <summary>
        /// Gets the number of days the ship uses to complete a roundtrip at sea before returning to harbour.
        /// </summary>
        /// <returns>Returns an int value representing the number of days the ship uses to do a round trip at sea.</returns>
        public abstract int RoundTripInDays { get; internal set; }
        /// <summary>
        /// Gets the ID of the ships current location.
        /// </summary>
        /// <returns>Returns a Guid object representing the ID of the ships current location.</returns>
        public abstract Guid CurrentLocation { get; internal set; }
        /// <summary>
        /// Gets a ReadOnlyCollecntion of StatusLog objects containing information on status changes the ship has gone through throughout a simulation.
        /// </summary>
        /// <returns>Returns an ReadOnlyCollection with StatusLog objects with information on status changes the ship has gone through throughout a simulation.</returns>
        public abstract ReadOnlyCollection<StatusRecord> History { get; }
        /// <summary>
        /// Gets all the containers in the ships storage.
        /// </summary>
        /// <returns>Returns an IList with Container objects representing the containers in the ships storage.</returns>
        public abstract IList<Container> ContainersOnBoard { get; }
        /// <summary>
        /// Gets the container capacity of the ship.
        /// </summary>
        /// <returns>Returns an int value representing the max number of containers the ship can store.</returns>
        public abstract int ContainerCapacity { get; internal set; }
        /// <summary>
        /// Gets the ships max weight the ship in tonns can be before it sinks.
        /// </summary>
        /// <returns>Returns an int value representing the max weight the ship can be in tonns.</returns>
        public abstract int MaxWeightInTonn { get; internal set; }
        /// <summary>
        /// Gets the weight of the ship when its storage is empty.
        /// </summary>
        /// <returns>Returns an int value representing the weight of the ship when the storage is empty.</returns>
        public abstract int BaseWeightInTonn { get; internal set; }
        /// <summary>
        /// Gets the current weight of the ship including the cargo weight. 
        /// </summary>
        /// <returns>Returns an int value representing the current weight of the ship.</returns>
        public abstract int CurrentWeightInTonn { get; internal set; }
        /// <summary>
        /// Prints the ships entire history to console.
        /// </summary>
        public abstract void PrintHistory();
        /// <summary>
        /// Returns the ships entire history in the form of a string.
        /// </summary>
        /// <returns> a String containing the ships entire history.</returns>
        public abstract String HistoryToString();
        /// <summary>
        /// Returns a string with the Ships ID, name, size, startdate, round trip time, amount on containers of the differenct containerSizes on board, base weight in tonn, current weight in tonn and max weigth in tonn the ship can handle.
        /// </summary>
        /// <returns> a String containing information about the ship.</returns>
        public abstract override String ToString();

        /// <summary>
        /// Constructor used to create objects for the class.
        /// </summary>
        internal CargoVessel()
        {
        }
    }
}