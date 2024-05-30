using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Gruppe8.HarbNet.Advanced;

namespace Gruppe8.HarbNet
{
    /// <summary>
    /// ContainerRow used as storage space for containers in a _harbor.
    /// Each container storage row represents one row of storage spaces where containers can be stored in the _harbor storage area.
    /// </summary>
    public class ContainerStorageRow : StorageArea
    {
        /// <summary>
        /// Gets the unique ID for the ContainerRow.
        /// </summary>
        /// <returns>Returns a Guid object representing the containerRows unique ID.</returns>
        public override Guid ID { get; } = Guid.NewGuid();

        /// <summary>
        /// Gets an IList of ContainerSpace objects that can be used to store containers in the ContainerStorageRow. Each ContainerSpace object has room to store
        /// one full size container or two half size containers.
        /// </summary>
        /// <returns>Returns a IList with ContainerSpace objects representing the total storage space of the ContainerStorageRow.</returns>
        internal IList<ContainerSpace> RowOfContainerSpaces { get; set; } = new List<ContainerSpace>();

        /// <summary>
        /// Constructor used to create objects of the ContainerStorageRow class.
        /// Each container storage row represents one row of storage spaces where containers can be stored in the _harbor storage area.
        /// </summary>
        /// <param name="numberOfContainerStorageSpaces">Int value representing the amount of container storage spaces to be created in a ContainerStorage row. Each container storage space has room to store
        /// one full size container or two half size containers.</param>
        public ContainerStorageRow(int numberOfContainerStorageSpaces)
        {
            for (int i = 0; i < numberOfContainerStorageSpaces; i++)
            {
                RowOfContainerSpaces.Add(new ContainerSpace());
            }
        }

        /// <summary>
        /// Gets the ContainerSpace object used to store the given container. 
        /// </summary>
        /// <param name="container">The container in witch the coresponding container space storing this Container is to be returned.</param>
        /// <returns>Returns the ContainerSpace that contains the specified container, null is returned if the container is not found.</returns>
        internal ContainerSpace? GetContainerSpaceContainingContainer(Container container)
        {
            foreach (ContainerSpace space in RowOfContainerSpaces)
            {
                if (space.StoredContainerOne == container.ID)
                {
                    return space;
                }
                if (space.StoredContainerTwo == container.ID)
                {
                    return space;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the ContainerSpace object used to store the given container. 
        /// </summary>
        /// <param name="containerID">The ID of the container in witch the coresponding container space storing this Container is to be returned.</param>
        /// <returns>Returns the ContainerSpace that contains the specified container, null is returned if the container is not found.</returns>
        internal ContainerSpace? GetContainerSpaceContainingContainer(Guid containerID)
        {
            foreach (ContainerSpace space in RowOfContainerSpaces)
            {
                if (space.StoredContainerOne == containerID)
                {
                    return space;
                }
                if (space.StoredContainerTwo == containerID)
                {
                    return space;
                }
            }
            return null;
        }

        /// <summary>
        /// Adds the given container object to an available free container storage space in the container storage row.
        /// </summary>
        /// <param name="container">The container object to be stored in the container storage row</param>
        /// <returns>Returns the containerSpace the container was stored in if a free space was found. Null is returned if the size of the container did not match the size of containers stored in this storage row or if there was no available space to store the container.</returns>
        internal ContainerSpace? AddContainerToFreeSpace(Container container)
        {
            if (SizeOfContainersStored() == container.Size || SizeOfContainersStored() == ContainerSize.None)
            {
                foreach (ContainerSpace space in RowOfContainerSpaces)
                {
                    if (space.FreeOne == true)
                    {
                        space.StoredContainerOne = container.ID;
                        space.FreeOne = false;
                        return space;
                    }
                    if (space.FreeTwo == true || container.Size == ContainerSize.Half)
                    {
                        space.StoredContainerTwo = container.ID;
                        space.FreeTwo = false;
                        return space;
                    }
                }
            }
           
            return null;
        }

        /// <summary>
        /// Checks if a free storage space exists for the given container size.
        /// </summary>
        /// <param name="size">ContinerSize enum representing the size of the container you want a free space for.</param>
        /// <returns>Returns true if a free container storage space of the given size is available, false if there is no free spaces of the given size.</returns>
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
        /// Adds a container to a free container storage space if one exist of the same size as the container given.
        /// </summary>
        /// <param name="containerID">Guid object representing the unique ID of the container to be stored in the storage row.</param>
        /// <param name="sizeOfContainer">Size of the container to be added.</param>
        /// <returns>If a free storage space exist of the same size as the given container: returns the containerSpace the container was added to. If no free storage space was found null is returned.</returns>
        internal ContainerSpace? AddContainerToFreeSpace(Guid containerID, ContainerSize sizeOfContainer)
        {
            if (SizeOfContainersStored() == sizeOfContainer || SizeOfContainersStored() == ContainerSize.None)
            {
                foreach (ContainerSpace space in RowOfContainerSpaces)
                {
                    if (space.FreeOne == true)
                    {
                        space.StoredContainerOne = containerID;
                        space.FreeOne = false;
                        return space;
                    }
                    if (space.FreeTwo == true || sizeOfContainer == ContainerSize.Half)
                    {
                        space.StoredContainerTwo = containerID;
                        space.FreeTwo = false;
                        return space;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Removes the specified container from the ContainerStorageRows storage.
        /// </summary>
        /// <param name="containerToBeRemoved">The Container object to be removed from ContainerStorageRow.</param>
        /// <returns>Returns the ContainerSpace the container was removed from, returns null if the container is not found in the Storage Row's storage.</returns>
        internal ContainerSpace? RemoveContainerFromContainerRow(Container containerToBeRemoved)
        {
            foreach (ContainerSpace space in RowOfContainerSpaces)
            {
                if (containerToBeRemoved.ID == space.StoredContainerOne)
                {
                    space.StoredContainerOne = Guid.Empty;
                    space.FreeOne = true;

                    if (space.StoredContainerTwo == Guid.Empty)
                    {
                        space.SizeOfContainerStored = ContainerSize.None;
                    }

                    return space;
                }

                else if (containerToBeRemoved.ID == space.StoredContainerTwo)
                {
                    space.StoredContainerTwo = Guid.Empty;
                    space .FreeTwo = true;

                    if (space.StoredContainerOne == Guid.Empty)
                    {
                        space.SizeOfContainerStored = ContainerSize.None;
                    }

                    return space;
                }
            }

            return null;
        }

        /// <summary>
        /// Removes the container with the given ID from the ContainerStorageRows storage.
        /// </summary>
        /// <param name="idOfContainerToBeRemoved">Guid object representing the ID of the container object to be removed from the Storage Row.</param>
        /// <returns>Returns the ContainerSpace the container was removed from, returns null if the container is not found in the Storage Row's storage.</returns>
        internal ContainerSpace? RemoveContainerFromContainerRow(Guid idOfContainerToBeRemoved)
        {
            foreach (ContainerSpace space in RowOfContainerSpaces)
            {
                if (idOfContainerToBeRemoved == space.StoredContainerOne)
                {
                    space.StoredContainerOne = Guid.Empty;
                    space.FreeOne = true;

                    if (space.StoredContainerTwo == Guid.Empty)
                    {
                        space.SizeOfContainerStored = ContainerSize.None;
                    }

                    return space;
                } 

                else if (idOfContainerToBeRemoved == space.StoredContainerTwo)
                {
                    space.StoredContainerTwo = Guid.Empty;
                    space.FreeTwo = true;

                    if (space.StoredContainerOne == Guid.Empty)
                    {
                        space.SizeOfContainerStored = ContainerSize.None;
                    }
                    return space;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the amount of available Container spaces in the Storage Row.
        /// </summary>
        /// <param name="size">ContainerSize enum representing the size of the containers you want to check how many free spaces is available for.</param>
        /// <returns>Returns an int value representing the total amount of available free space of the given container size.</returns>
        public override int NumberOfFreeContainerSpaces (ContainerSize size)
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
        /// Gets the size of the containers being stored in this Storage Row.
        /// </summary>
        /// <returns>Returns a ContainerSize enum representing the size of the containers being stored in this Storage Row. If no containers are being stored a ContainerSize enum with the value of None is returned.</returns>
        public override ContainerSize SizeOfContainersStored()
        {
            foreach (ContainerSpace space in RowOfContainerSpaces)
            {
                if (space.SizeOfContainerStored != ContainerSize.None)
                {
                    return space.SizeOfContainerStored;
                }
            }

            return ContainerSize.None;
        }

        /// <summary>
        /// Gets an IList containing the IDs of all containers stored in this Storage Row.
        /// </summary>
        /// <returns>Returns a IList with Guid object representing the IDs of all containers being stored in this storage row.</returns>
        public override IList<Guid> GetIDOfAllStoredContainers()
        {
            IList<Guid> idList = new List<Guid>();

            foreach (ContainerSpace space in RowOfContainerSpaces)
            {
                if (space.StoredContainerOne != Guid.Empty)
                {
                    idList.Add(space.StoredContainerOne);
                }
                if (space.StoredContainerTwo != Guid.Empty)
                {
                    idList.Add(space.StoredContainerOne);
                }
            }

            return idList;
        }

        /// <summary>
        /// Gets a String value containing information about the Storage Row's ID, the total amount of container storage spaces and the amount of containers currently being stored.
        /// </summary>
        /// <returns>Returns a String containing information about the Storage Row's ID, the total amount of container storage spaces and the amount of containers currently being stored.</returns>
        public override string ToString()
        {
            return 
                $"Storage row ID: {ID}, " +
                $"Container storage spaces: {RowOfContainerSpaces.Count}, " +
                $"Stored containers: {GetIDOfAllStoredContainers().Count}";
        }
    }
}
