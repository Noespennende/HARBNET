using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe8.HarbNet
{
    /// <summary>
    /// Storage space for containers in harbour.
    /// </summary>
    internal class ContainerSpace
    {
        /// <summary>
        /// Gets the unique ID of the container storage space.
        /// </summary>
        /// <returns>Returns a Guid object representing the unique ID of the container storage space.</returns>
        internal Guid ID { get; } = Guid.NewGuid();
        /// <summary>
        /// Gets or sets the size of the containers that can be stored in this container space.
        /// </summary>
        /// <returns>Returns a containerSize enum representing the size of the containers that can be stored.</returns>
        internal ContainerSize SizeOfContainerStored { get; set; }
        /// <summary>
        /// Checks if container space is available for a full size container.
        /// </summary>
        /// <returns>Returns a boolean that is true if the container space is available and false if it is not</returns>
        internal bool Free {  get; set; }

        /// <summary>
        /// Gets the ID of the full size container currently stored in the container space.
        /// </summary>
        /// <returns>Returns a Guid object representing the ID of the full container currently stored in the container space.</returns>
        private Stack<Guid> StoredContainers { get; set; } = new Stack<Guid>();
        
        internal int StackSize { get { return StoredContainers.Count(); } }
        internal int MaxStackSize { get; private set; }

        /// <summary>
        /// Creates a new ContainerSpace.
        /// </summary>
        internal ContainerSpace (int maxStackSize)
        {
            this.MaxStackSize= maxStackSize;
            this.Free = true;
            SizeOfContainerStored = ContainerSize.None;
        }

        
        internal Guid unload()
        {
            if ( StoredContainers.Count > 0)
            {
                if (StackSize < MaxStackSize )
                {
                    Free = true;
                }

                return StoredContainers.Pop();
            }
            return Guid.Empty;
        }

        internal bool load(Guid containerID)
        {
            if (StackSize < MaxStackSize)
            {
                StoredContainers.Push(containerID);
                if ( StackSize >= MaxStackSize)
                {
                    Free = false;
                }

                return true;
            } 
            return false;
        }

        internal Guid nextContainer ()
        { 
            return StoredContainers.Peek();
        }
        
    }
}
