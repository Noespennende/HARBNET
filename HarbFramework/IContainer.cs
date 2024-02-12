using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarbFramework
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
        /// <return>Returns a list of history events the container has been through</return>
        public IList<Event> History { get; }

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
        /// <returns>Returns last history event of container if they have a history, or returns none if there is no history registered</returns>
        public Status GetCurrentStatus();
    }
}
