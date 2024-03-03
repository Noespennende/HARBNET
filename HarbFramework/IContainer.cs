using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe8.HarbNet
{
    /// <summary>
    /// Interface defining the contract the public API of the container class
    /// </summary>
    internal interface IContainer
    {
        /// <summary>
        /// Unique ID for container
        /// </summary>
        /// <return>Returns the unique ID defining a specific container</return>
        public Guid ID { get; }
        /// <summary>
        /// Gets the history of the container
        /// </summary>
        /// <return>Returns a ReadOnlyCollection of StatusLog objects,each object containing information about one status change of the subject in a simulation.</return>
        public ReadOnlyCollection<StatusLog> History { get; }

        /// <summary>
        /// Gets the size of the container
        /// </summary>
        /// <return>Returns the size of the container</return>
        public ContainerSize Size { get; }

        /// <summary>
        /// Gets the containers weight in tonn
        /// </summary>
        /// <return>Returns the int value of the containers weight in tonn</return>
        public int WeightInTonn { get;  }

        /// <summary>
        /// Unique ID for the current position
        /// </summary>
        /// <return>Returns the Guid for the current position of container</return>
        public Guid CurrentPosition { get; }

        /// <summary>
        /// Gets current status of container
        /// </summary>
        /// <returns>Returns last status change of container if they have a history. Returns a none status if there is no history registered</returns>
        public Status GetCurrentStatus();

        /// <summary>
        /// Prints the containers entire history to console 
        /// </summary>
        public void PrintHistory();
        /// <summary>
        /// Returns a String representing the history of the container. 
        /// </summary>
        /// <returns>String representing the history of a the container</returns>
        public String HistoryToString();
        /// <summary>
        /// Returns a String containing information about the container. 
        /// </summary>
        /// <returns>String containing information about the container.</returns>
        public string ToString();
    }
}
