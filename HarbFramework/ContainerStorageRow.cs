using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Gruppe8.HarbNet
{
    /// <summary>
    /// ContainerRow used as storage space for containers in harbor.
    /// Each container storage row represents one row of storage spaces where containers can be stored.
    /// </summary>
    public class ContainerStorageRow : IContainerStorageRow
    {
        /// <summary>
        /// Gets the unique ID for the ContainerRow.
        /// </summary>
        /// <returns>Returns a Guid object representing the containerRows unique ID.</returns>
        public Guid ID { get; } = Guid.NewGuid();

        /// <summary>
        /// Gets a IList of ContainerSpace object containing information about the storage space in the ContainerRow.
        /// </summary>
        /// <returns>Returns a IList with ContainerSpace object with information on the containerspace in a containerRow.</returns>
        internal IList<ContainerSpace> RowOfContainerSpaces { get; set; } = new List<ContainerSpace>();

        public ReadOnlyCollection<Guid> IDOfStoredContainers { get { return StoredContainersList.AsReadOnly<Guid>(); } }

        private IList<Guid> StoredContainersList { get; set; } = new List<Guid>();

        internal int StackSize { get; private set; } 
        internal int MaxStackSize { get; private set; }
        private int CurrentIndex { get; set; }

        public ContainerSize SizeOfContainersStored { get; private set; }

        /// <summary>
        /// Gets the amount of ContainerSpace in a ContainerRow.
        /// </summary>
        /// <param name="numberOfContainerStorageSpaces">Amount of ContainerSpaces to be created.</param>
        public ContainerStorageRow(int numberOfContainerStorageSpaces, int maxStackSize)
        {
            this.MaxStackSize = maxStackSize;
            this.StackSize = 0;
            this.CurrentIndex = 0;
            for (int i = 0; i < numberOfContainerStorageSpaces; i++)
            {
                RowOfContainerSpaces.Add(new ContainerSpace(maxStackSize));
            }
        }


        /// <summary>
        /// Adds container to available ContainerSpace.
        /// </summary>
        /// <param name="container">Unique name for the container to be added to containerSpace.</param>
        /// <returns>Returns the containerSpace the container was added to, null if not found.</returns>
        internal bool AddContainer(Container container)
        {
            if ((SizeOfContainersStored == container.Size || SizeOfContainersStored == ContainerSize.None) && (StackSize < MaxStackSize) )
            {
               if (CurrentIndex < RowOfContainerSpaces.Count()-1)
                {
                    RowOfContainerSpaces[CurrentIndex].load(container.ID);
                    StoredContainersList.Add(container.ID);

                    if (SizeOfContainersStored == ContainerSize.None)
                    {
                        SizeOfContainersStored = container.Size;
                    }

                    CurrentIndex++;
                    return true;
                }
               else
                {
                    RowOfContainerSpaces[CurrentIndex].load(container.ID);
                    StoredContainersList.Add(container.ID);
                    StackSize++;
                    CurrentIndex = 0;
                    return true;
                }

            }
           
            return false;
        }

        /// <summary>
        /// Add container to available ContainerSpace.
        /// </summary>
        /// <param name="containerID">Unique ID for the container to be added.</param>
        /// <param name="sizeOfContainer">Size of the container to be added.</param>
        /// <returns>Returns the containerSpace the container was added to. If container was not not added, null is returned.</returns>
        internal bool AddContainer(Guid containerID, ContainerSize sizeOfContainer)
        {
            if ((SizeOfContainersStored == sizeOfContainer || SizeOfContainersStored == ContainerSize.None) && (StackSize < MaxStackSize))
            {
                if (CurrentIndex < RowOfContainerSpaces.Count() - 1)
                {
                    RowOfContainerSpaces[CurrentIndex].load(containerID);
                    CurrentIndex++;
                    StoredContainersList.Add(containerID);

                    if (SizeOfContainersStored == ContainerSize.None)
                    {
                        SizeOfContainersStored = sizeOfContainer;
                    }

                    return true;
                }
                else
                {
                    RowOfContainerSpaces[CurrentIndex].load(containerID);
                    StoredContainersList.Add(containerID);
                    StackSize++;
                    CurrentIndex = 0;
                    return true;
                }

            }

            return false;
        }

        /// <summary>
        /// Checks if there is available ContainerSpace based on size.
        /// </summary>
        /// <param name="size">Size of containerSpace that will be checked for availability.</param>
        /// <returns>Returns true if containerSpace of given size is available, false if not found.</returns>
        /// DENNE MÅ GJØRES!!!!!!!!!!!!!!!!!!!!
        internal bool CheckIfFreeContainerSpaceExists (ContainerSize size)
        {
            if (SizeOfContainersStored() == size || SizeOfContainersStored() == ContainerSize.None)
            {
                foreach (ContainerSpace space in RowOfContainerSpaces)
                {

                    if (space.FreeOne == true)
                    {
                        return true;
                    }

                    if (space.FreeTwo == true || size == ContainerSize.Half)
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        /// <summary>
        /// Removes container from ContainerSpace.
        /// </summary>
        /// <param name="containerToBeRemoved">Name of the container to be removed.</param>
        /// <returns>Returns the ContainerSpacethe container was removed from, null if not found.</returns>
        internal Guid RemoveContainer()
        {
            if (StackSize == 0 && CurrentIndex == 0)
            {
                return Guid.Empty;
            }

            if (CurrentIndex > 0)
            {
                Guid containerID = RowOfContainerSpaces[CurrentIndex - 1].unload();
                StoredContainersList.Remove(containerID);
                CurrentIndex--;
                return containerID;
            }
            else
            {
                Guid containerID = RowOfContainerSpaces[CurrentIndex].unload();
                StoredContainersList.Remove(containerID);
                if (StackSize > 0)
                {
                    StackSize--;
                    CurrentIndex = RowOfContainerSpaces.Count - 1;
                }
                else
                {
                    SizeOfContainersStored = ContainerSize.None;
                }
                return containerID;

            }
        }

        /// <summary>
        /// Gets the amount of available containerSpaces.
        /// </summary>
        /// <returns>Returns an int value representing the total amount of available containerSpaces.</returns>
        /// IMPLEMENTER DETTE!!!!!!!!!!!!!!!!!
        public int numberOfFreeContainerSpaces (ContainerSize size)
        {
            int count = 0;
            ContainerSize sizeOfContainersStored = SizeOfContainersStored();

            if (sizeOfContainersStored == ContainerSize.None || size == sizeOfContainersStored)
            {
                foreach (ContainerSpace space in RowOfContainerSpaces)
                {
                    if (space.FreeOne)
                    {
                        count++;
                    }
                    if (space.FreeTwo && size == ContainerSize.Half)
                    {
                        count++;
                    }
                }
            }
            
            return count;
        }


        /// <summary>
        /// Returns a String containing information about the ContainerSpace.
        /// </summary>
        /// <returns>Returns a String containing information about the ContainerSpace.</returns>
        public override String ToString()
        {
            return $"Storage row ID: {ID.ToString()}, Container storage spaces: {RowOfContainerSpaces.Count}, Stored containers: {IDOfStoredContainers.Count}";
        }

    }

}
