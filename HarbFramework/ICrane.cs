using System.Collections.ObjectModel;

namespace Gruppe8.HarbNet
{
    /// <summary>
    /// Interface defining the public API of the Crane class
    /// </summary>
    public interface ICrane
    {
        /// <summary>
        /// Gets the unique ID for the crane
        /// </summary>
        /// <returns>Returns a Guid object representing the cranes unique ID</returns>
        public Guid ID { get;}

        /// <summary>
        /// Gets the container object
        /// </summary>
        /// <returns>Returns the container object that will be loaded or unloaded by the crane</returns>
        public Container Container { get; }

        /// <summary>
        /// Gets the containers loaded per hour
        /// </summary>
        /// <returns>Returns the int value representing the amount of containers loaded per hour</returns>
        public int ContainersLoadedPerHour { get;}

        /// <summary>
        /// Gets the unique ID for the cranes current location
        /// </summary>
        /// <returns>Returns a Guid object representing the location of the crane</returns>
        public Guid Location { get; }

        /// <summary>
        /// Returns a string that represents the object, containing the ID, container load per hour and location ID.
        /// </summary>
        /// <returns>Returns a Guid object representing the location of the crane</returns>
        public String ToString();
    }
}