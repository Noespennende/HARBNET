using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe8.HarbNet.PublicApiAbstractions
{
    /// <summary>
    /// Abstract class defining the public API of StorageUnits such as the container class.
    /// </summary>
    public abstract class StorageUnit
    {
        /// <summary>
        /// Gets the unique ID for container.
        /// </summary>
        /// <return>Returns a Guid object representing the containers unique ID.</return>
        public abstract Guid ID { get; }
        /// <summary>
        /// Gets a ReadOnlyCollection of StatusLog objects containing information on status changes the ship has gone through throughout a simulation.
        /// </summary>
        /// <return>Returns a ReadOnlyCollection with StatusLog objects with information on status changes the container has gone through throughout a simulation.</return>
        public abstract ReadOnlyCollection<StatusLog> History { get; }
        /// <summary>
        /// Gets the containers size.
        /// </summary>
        /// <returns>Returns a ContainerSize enum representing the containers size.</returns>
        public abstract ContainerSize Size { get; internal set; }
        /// <summary>
        /// Gets the containers weight in tonn.
        /// </summary>
        /// <returns>Returns an int value representing the containers weight in tonn.</returns>
        public abstract int WeightInTonn { get; internal set; }
        /// <summary>
        /// Gets the ID if the containers current position.
        /// </summary>
        /// <returns>Returns a Guid object representing the ID of the containers current position.</returns>
        public abstract Guid CurrentPosition { get; internal set; }
        /// <summary>
        /// Gets current status of the container, which is the most recent status change in the HistoryList with StatusLogs.
        /// </summary>
        /// <returns>Returns a Status enum with the most recent status of the container.</returns>
        public abstract Status GetCurrentStatus();
        /// <summary>
        /// Prints a container's entire HistoryList to console, with the date and time the status change took place and status enum for each StatusLog change.
        /// </summary>
        public abstract void PrintHistory();
        /// <summary>
        /// Prints the containers entire HistoryList, with the container ID, date and time the status change took place and status enum for each StatusLog change in the form of a String. 
        /// </summary>
        /// <returns>Returns a String representing the history of a the container, printing each Statuslog object in the HistoryList.</returns>
        public abstract String HistoryToString();
        /// <summary>
        /// Prints a String with the container's ID, ContainerSize enum and int value representing it's weight in tonn. 
        /// </summary>
        /// <returns>Returns a String with the container's ID, ContainerSize enum and int value representing it's weight in tonn.</returns>
        public abstract override string ToString();

        /// <summary>
        /// Constructor for the StorageUnit class.
        /// </summary>
        internal StorageUnit() { }
    }
}
