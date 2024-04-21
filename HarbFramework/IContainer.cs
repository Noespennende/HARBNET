using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe8.HarbNet
{
    /// <summary>
    /// Interface defining the contract the public API of the container class.
    /// </summary>
    internal interface IContainer
    {
        /// <summary>
        /// Gets the unique ID for container.
        /// </summary>
        /// <return>Returns a Guid object representing the containers unique ID.</return>
        public Guid ID { get; }
        /// <summary>
        /// Gets a ReadOnlyCollection of StatusLog objects containing information on status changes the ship has gone through throughout a simulation.
        /// </summary>
        /// <return>Returns a ReadOnlyCollection with StatusLog objects with information on status changes the container has gone through throughout a simulation.</return>
        public ReadOnlyCollection<StatusLog> History { get; }
        /// <summary>
        /// Gets the containers size.
        /// </summary>
        /// <returns>Returns a ContainerSize enum representing the containers size.</returns>
        public ContainerSize Size { get; }
        /// <summary>
        /// Gets the containers weight in tonn.
        /// </summary>
        /// <returns>Returns an int value representing the containers weight in tonn.</returns>
        public int WeightInTonn { get; }
        /// <summary>
        /// Gets the ID if the containers current position.
        /// </summary>
        /// <returns>Returns a Guid object representing the ID of the containers current position.</returns>
        public Guid CurrentPosition { get; }
        /// <summary>
        /// Gets current status of the container, which is the most recent status change in the HistoryList with StatusLogs.
        /// </summary>
        /// <returns>Returns a Status enum with the most recent status of the container.</returns>
        public Status GetCurrentStatus();
        /// <summary>
        /// Prints a container's entire HistoryList to console, with the date and time the status change took place and status enum for each StatusLog change.
        /// </summary>
        public void PrintHistory();
        /// <summary>
        /// Prints the containers entire HistoryList, with the container ID, date and time the status change took place and status enum for each StatusLog change in the form of a String. 
        /// </summary>
        /// <returns>Returns a String representing the history of a the container, printing each Statuslog object in the HistoryList.</returns>
        public String HistoryToString();
        /// <summary>
        /// Prints a String with the container's ID, ContainerSize enum and int value representing it's weight in tonn. 
        /// </summary>
        /// <returns>Returns a String with the container's ID, ContainerSize enum and int value representing it's weight in tonn.</returns>
        public string ToString();
    }
}
