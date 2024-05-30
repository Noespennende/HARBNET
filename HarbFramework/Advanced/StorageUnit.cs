using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe8.HarbNet.Advanced
{
    /// <summary>
    /// Abstract class defining the public API of StorageUnits such as the container class.
    /// This abstract class can be used to make fakes to be used in testing of the API. 
    /// </summary>
    public abstract class StorageUnit
    {
        /// <summary>
        /// Gets the unique ID for StorageUnit.
        /// </summary>
        /// <return>Returns a Guid object representing the StorageUnit's unique ID.</return>
        public abstract Guid ID { get; }
        
        /// <summary>
        /// Gets a ReadOnlyCollection of StatusLog objects containing information on all status changes the StorageUnit has gone through throughout a simulation.
        /// Each StatusLog object stores information about a single status change the StorageUnit has gone trough.
        /// </summary>
        /// <return>Returns a ReadOnlyCollection with StatusLog objects with information on status changes the StorageUnit has gone through throughout a simulation.</return>
        public abstract ReadOnlyCollection<StatusLog> History { get; }
        
        /// <summary>
        /// Gets the size of the StorageUnit.
        /// </summary>
        /// <returns>Returns a ContainerSize enum representing the StorageUnit's size.</returns>
        public abstract ContainerSize Size { get; internal set; }
        
        /// <summary>
        /// Gets the StorageUnit's weight in tonn.
        /// </summary>
        /// <returns>Returns an int value representing the StorageUnit's weight in tonn.</returns>
        public abstract int WeightInTonn { get; internal set; }
        
        /// <summary>
        /// Gets the ID if the StorageUnit's current position.
        /// </summary>
        /// <returns>Returns a Guid object representing the ID of the StorageUnit's current position.</returns>
        public abstract Guid CurrentLocation { get; internal set; }
        
        /// <summary>
        /// Gets current status of the StorageUnit.
        /// </summary>
        /// <returns>Returns a Status enum with the current status of the StorageUnit.</returns>
        public abstract Status GetCurrentStatus();
        
        /// <summary>
        /// Prints a StorageUnit's entire History to console, with each Status the StorageUnit has had troughout the simulation and the date and time each status change took place.
        /// </summary>
        public abstract void PrintHistory();
        
        /// <summary>
        /// Gets a string with information about the StorageUnit's entire History, with each Status the StorageUnit has had troughout the simulation and the date and time each status change took place.
        /// </summary>
        /// <returns>Returns a String representing the entire history of a the StorageUnit.</returns>
        public abstract string HistoryToString();
        
        /// <summary>
        /// Prints a String with the container's ID, Size and int value representing it's weight in tonn. 
        /// </summary>
        /// <returns>Returns a String with the container's ID, Size and int value representing it's weight in tonn.</returns>
        public abstract override string ToString();

        /// <summary>
        /// Constructor for the StorageUnit class.
        /// </summary>
        internal StorageUnit() 
        { 
        }
    }
}
