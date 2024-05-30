using System.Collections.ObjectModel;

namespace Gruppe8.HarbNet.Advanced
{
    /// <summary>
    /// Abstract class defining the public API contract for CargoVessels such as the Ship class.
    /// This abstract class can be used to make fakes to be used in testing of the API. 
    /// </summary>
    public abstract class CargoVessel
    {
        /// <summary>
        /// Gets the unique ID for the Cargo Vessel.
        /// </summary>
        /// <returns>Returns a Guid object representing the Cargo Vessel's unique ID.</returns>
        public abstract Guid ID { get; }
        /// <summary>
        /// Gets the size of the Cargo Vessel. 
        /// </summary>
        /// <returns>Returns a ShipSize enumm representing the size of the Cargo Vessel.</returns>
        public abstract ShipSize ShipSize { get; internal set; }
        /// <summary>
        /// Gets the name of the Cargo Vessel. 
        /// </summary>
        /// <returns>Returns a string value representing the Cargo Vessel's name.</returns>
        public abstract String Name { get; internal set; }
        /// <summary>
        /// Gets the date and time the Cargo Vessel started its first voyage in the simulation.
        /// </summary>
        /// <returns>Returns a DateTime object representing the date and time the Cargo Vessel started its first voyage in the simulation.</returns>
        public abstract DateTime StartDate { get; internal set; }
        /// <summary>
        /// Gets the number of days the Cargo Vessel uses to complete a roundtrip to its delivery destination before returning to harbour. The int value represents
        /// how many days it takes from the Cargo Vessel leaving its harbor for its delivery destination before it arrives back at the harbor again.
        /// </summary>
        /// <returns>Returns an int value representing the number of days the Cargo Vessel uses to do a round trip to its delivery destination and back to the user's harbor.</returns>
        public abstract int RoundTripInDays { get; internal set; }
        /// <summary>
        /// Gets the ID of the Cargo Vessel's current location.
        /// </summary>
        /// <returns>Returns a Guid object representing the ID of the Cargo Vessel's current location.</returns>
        public abstract Guid CurrentLocation { get; internal set; }
        /// <summary>
        /// Gets a ReadOnlyCollection of StatusLog objects containing information on status changes the Cargo Vessel has gone through throughout a simulation.
        /// Each object in the collection represent information about one status change to the Cargo Vessel. 
        /// </summary>
        /// <returns>Returns an ReadOnlyCollection with StatusLog objects with information on status changes the Cargo Vessel has gone through throughout a simulation.</returns>
        public abstract ReadOnlyCollection<StatusLog> History { get; }
        /// <summary>
        /// Gets all the containers in the Cargo Vessel's storage.
        /// </summary>
        /// <returns>Returns an IList with Container objects representing the containers in the Cargo Vessel's storage.</returns>
        public abstract IList<Container> ContainersOnBoard { get; }
        /// <summary>
        /// Gets the container capacity of the Cargo Vessel
        /// </summary>
        /// <returns>Returns an int value representing the max number of containers the Cargo Vessel can store.</returns>
        public abstract int ContainerCapacity { get; internal set; }
        /// <summary>
        /// Gets the Cargo Vessel's max weight in tonns. This number represents the maximum weight the Cargo Vessel can reach before it becomes dangerous.
        /// </summary>
        /// <returns>Returns an int value representing the max weight in tonns the Cargo Vessel can be before it becomes dangerous.</returns>
        public abstract int MaxWeightInTonn { get; internal set; }
        /// <summary>
        /// Gets the weight in tonns of the Cargo Vessel when its storage is empty.
        /// </summary>
        /// <returns>Returns an int value representing the weight in tonns of the Cargo Vessel when the storage is empty.</returns>
        public abstract int BaseWeightInTonn { get; internal set; }
        /// <summary>
        /// Gets the current weight in tonns of the Cargo Vessel including the weight of its cargo. 
        /// </summary>
        /// <returns>Returns an int value representing the current weight in tonns of the Cargo Vessel including the weight of its cargo.</returns>
        public abstract int CurrentWeightInTonn { get; internal set; }
        /// <summary>
        /// Prints the Cargo Vessel's entire history to console.
        /// </summary>
        public abstract void PrintHistory();
        /// <summary>
        /// Returns the Cargo Vessel's entire history in the form of a string.
        /// </summary>
        /// <returns> a String containing the Cargo Vessel's entire history.</returns>
        public abstract String HistoryToString();
        /// <summary>
        /// Returns a string with information about the Cargo Vessel.
        /// </summary>
        /// <returns> a String containing information about the Cargo Vessel.</returns>
        public abstract override String ToString();

        /// <summary>
        /// Constructor used to create objects of the CargoVessel class.
        /// </summary>
        internal CargoVessel()
        {
        }
    }
}