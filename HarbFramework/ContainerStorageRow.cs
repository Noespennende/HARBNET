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
    /// ContainerRow used as storage space for containers in harbor.
    /// Each container storage row represents one row of storage spaces where containers can be stored.
    /// </summary>
    public class ContainerStorageRow : StorageArea
    {
        /// <summary>
        /// Gets the unique ID for the ContainerRow.
        /// </summary>
        /// <returns>Returns a Guid object representing the containerRows unique ID.</returns>
        public override Guid ID { get; } = Guid.NewGuid();

        /// <summary>
        /// Gets a IList of ContainerSpace objects containing information about the storage space in the ContainerRow.
        /// </summary>
        /// <returns>Returns a IList with ContainerSpace object with information on the containerspace in a containerRow.</returns>
        internal IList<ContainerSpace> RowOfContainerSpaces { get; set; } = new List<ContainerSpace>();

        /// <summary>
        /// Adds the specified amount of ContainerSpaces to RowOfContainerSpace, which represents the ContainerStorageRow.
        /// </summary>
        /// <param name="numberOfContainerStorageSpaces">Int value representing the amount of ContainerSpace to be created in a ContainerStorage row.</param>
        public ContainerStorageRow(int numberOfContainerStorageSpaces)
        {
            for (int i = 0; i < numberOfContainerStorageSpaces; i++)
            {
                RowOfContainerSpaces.Add(new ContainerSpace());
            }
        }

        /// <summary>
        /// Gets the ContainerSpace from row of containerspaces that contains the container specified by name.
        /// </summary>
        /// <param name="container">The container object to be checked if it's contained in a ContainerSpace.</param>
        /// <returns>Returns the ContainerSpace that contains the specified container, null is returned if the container is not found.</returns>
        internal ContainerSpace GetContainerSpaceContainingContainer(Container container)
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
        /// Gets the ContainerSpace from row of containerspaces that contains the container specified by ID.
        /// </summary>
        /// <param name="containerID">The unique ID for the container to be checked if it's contained in a ContainerSpace.</param>
        /// <returns>Returns the ContainerSpace that contains the specified container, null is returned if the container is not found.</returns>
        internal ContainerSpace GetContainerSpaceContainingContainer(Guid containerID)
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
        /// Adds container object to available ContainerSpace in RowOfContainerSpaces.
        /// </summary>
        /// <param name="container">The container object to be checked if it can be added to available ContainerSpace.</param>
        /// <returns>Returns the containerSpace the container was added to, null is returned if the size of the container did not match any of the sizeOfContainersStored registered or if there was no available space.</returns>
        internal ContainerSpace AddContainerToFreeSpace(Container container)
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
        /// Checks if it exists available ContainerSpace based on the ContainerSize enum.
        /// </summary>
        /// <param name="size">ContinerSize enum the ContainerSpace use to be checked for availability.</param>
        /// <returns>Returns true if containerSpace of given ContainerSize enum is available, false if not found.</returns>
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
        /// Adds container object to ContainerSpace in RowOfContainerSpaces if they have matching ContainerSize enums and there is available ContainerSpace.
        /// </summary>
        /// <param name="containerID">The unique ID for the container to be checked if it can be added to available ContainerSpace.</param>
        /// <param name="sizeOfContainer">Size of the container to be added.</param>
        /// <returns>Returns the containerSpace the container was added to. If container was not added, null is returned.</returns>
        internal ContainerSpace AddContainerToFreeSpace(Guid containerID, ContainerSize sizeOfContainer)
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
        /// Removes container object from ContainerSpace in RowOfContainerSpace.
        /// </summary>
        /// <param name="containerToBeRemoved">The Container object to be removed from ContainerSpace.</param>
        /// <returns>Returns the ContainerSpace the container was removed from, returns null if the container is not found in ContainerSpace.</returns>
        internal ContainerSpace RemoveContainerFromContainerRow(Container containerToBeRemoved)
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
        /// Removes container object specified by ID from ContainerSpace in RowOfContainerSpace.
        /// </summary>
        /// <param name="idOfContainerToBeRemoved">Unique ID of the container object to be removed from ContainerSpace.</param>
        /// <returns>Returns the ContainerSpace the container was removed from, returns null if the container is not found in ContainerSpace.</returns>
        internal ContainerSpace RemoveContainerFromContainerRow(Guid idOfContainerToBeRemoved)
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
        /// Gets the amount of available ContainerSpace in RowOfContainerSpaces.
        /// </summary>
        /// <param name="size">ContinerSize enum the ContainerSpace use to be checked for availability.</param>
        /// <returns>Returns an int value representing the total amount of available ContainerSpace.</returns>
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
        /// Gets all the ContainerSize enums from the containers stored in RowOfContainerSpaces.
        /// </summary>
        /// <returns>Returns the ContainerSize enum representing the containers size from the ContainerSpaces in RowOfContainerSpaces that contains containers, if no ContainerSize is found, none is returned.</returns>
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
        /// Gets an IList with Guid objects representing the ID of the stored containers registered in RowOfContainerSpaces.
        /// </summary>
        /// <returns>Returns a IList with Guid objects from all the containers stored in RowOfContainerSpaces.</returns>
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
        /// Returns a String with the Storage Row ID, the RowOfContainerSpaces list and amount of stored containers registered.
        /// </summary>
        /// <returns>Returns a String with the Storage Row ID, the RowOfContainerSpaceList IList with ContainerSpace objects and an int value representing the amount of stored containers registered.</returns>
        public override String ToString()
        {
            return $"Storage row ID: {ID.ToString()}, Container storage spaces: {RowOfContainerSpaces}, Stored containers: {GetIDOfAllStoredContainers().Count}";
        }

    }

}
