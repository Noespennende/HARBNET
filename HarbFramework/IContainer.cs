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
        /// Gets current status of the container.
        /// </summary>
        /// <returns>Returns a Status enum with the current status of the container.</returns>
        public Status GetCurrentStatus();
        /// <summary>
        /// Prints the containers entire history to console.
        /// </summary>
        public void PrintHistory();
        /// <summary>
        /// Returns a String representing the history of the container. 
        /// </summary>
        /// <returns>String representing the history of a the container.</returns>
        public String HistoryToString();
        /// <summary>
        /// Returns a String containing information about the container. 
        /// </summary>
        /// <returns>String containing information about the container.</returns>
        public string ToString();
    }
}
