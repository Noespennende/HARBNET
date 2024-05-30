using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe8.HarbNet
{
    /// <summary>
    /// Class representing the Storage space for a container in a harbour. Each object of this class represents the space to store 1 full-size container or 2 half-size containers.
    /// </summary>
    internal class ContainerSpace
    {
        /// <summary>
        /// Gets the unique ID of the container storage space.
        /// </summary>
        /// <returns>Returns a Guid object representing the unique ID of the container storage space.</returns>
        internal Guid ID { get; } = Guid.NewGuid();

        /// <summary>
        /// Gets or sets the size of the containers that can be stored in this container space. If a container size is set only containers of that size
        /// can be stored in the storage space.
        /// </summary>
        /// <returns>Returns a containerSize enum representing the size of the containers that can be stored in this container space.</returns>
        internal ContainerSize SizeOfContainerStored { get; set; }

        /// <summary>
        /// Checks if the first half of the containerspace is free to store a container. This half can be used to store one half size container. If a full size container is to be stored
        /// both halves of the container space must be free to store the container.
        /// </summary>
        /// <returns>Returns a boolean that is true if the first half of the container space is available and false if it is not</returns>
        internal bool FreeOne { get; set; }

        /// <summary>
        /// Checks if the second half of the container space is available. This half can be used to store one half size container. If a full size container is to be stored
        /// both halves of the container space must be free to store the container.
        /// </summary>
        /// <returns>Returns a boolean that is true if the second half of the container space is available and false if it is not</returns>
        internal bool FreeTwo { get; set; }

        /// <summary>
        /// Gets the ID of the full size container or the first half-sized container stored in the container space.
        /// </summary>
        /// <returns>Returns a Guid object representing the ID of the full sized container or the first half-sized container currently stored in the container space.</returns>
        internal Guid StoredContainerOne { get; set; }

        /// <summary>
        /// Gets the ID of the second half size containers currently stored in the container space.
        /// </summary>
        /// <returns>Returns a Guid object representing the ID of the second half size containers currently stored in the container space.</returns>
        internal Guid StoredContainerTwo { get; set; }

        /// <summary>
        /// Creates a new ContainerSpace.
        /// </summary>
        internal ContainerSpace()
        {
            FreeOne = true;
            FreeTwo = true;
            StoredContainerOne = Guid.Empty;
            StoredContainerTwo = Guid.Empty;
        }

    }
}
