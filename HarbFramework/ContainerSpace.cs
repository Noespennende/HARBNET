using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe8.HarbNet
{
    /// <summary>
    /// Storage space for containers in harbour
    /// </summary>
    internal class ContainerSpace
    {
        /// <summary>
        /// Gets the unique ID of the container storage space.
        /// </summary>
        /// <returns>Returns a Guid object representing the unique ID of the container storage space</returns>
        internal Guid ID { get; } = Guid.NewGuid();
        /// <summary>
        /// Gets or sets the size of the containers that can be stored in this container space
        /// </summary>
        /// <returns>Returns a containerSize enum representing the size of the containers that can be stored</returns>
        internal ContainerSize Size { get; set; }
        /// <summary>
        /// Returns wether or not the dock currently free for ships to dock
        /// </summary>
        /// <returns>Returns a boolean that is true if the dock is free and false if it is not</returns>
        internal bool Free {  get; set; }
        /// <summary>
        /// Gets the ID of the container currently stored in the container space
        /// </summary>
        /// <returns>Returns a Guid object representing the ID of the container currently stored in the container space</returns>
        internal Guid storedContainer { get; set; }
        /// <summary>
        /// Creates a new container space
        /// </summary>
        internal ContainerSpace (ContainerSize size)
        {
            this.Size = size;
            this.Free = true;
            this.storedContainer = Guid.Empty;
        }
        
    }
}
