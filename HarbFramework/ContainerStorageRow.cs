using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Gruppe8.HarbNet
{
    /// <summary>
    /// ContainerRow used as storage space for containers in harbor.
    /// </summary>
    public class ContainerStorageRow : IContainerStorageRow
    {
        /// <summary>
        /// Gets the unique ID for the containerRow.
        /// </summary>
        /// <returns>Returns a Guid object representing the containerRows unique ID.</returns>
        public Guid ID { get; } = Guid.NewGuid();

        /// <summary>
        /// Gets a IList of ContainerSpace object containing information about the space in the ContainerRow.
        /// </summary>
        /// <returns>Returns a IList with ContainerSpace object with information on the containerspace in a containerRow.</returns>
        internal IList<ContainerSpace> RowOfContainerSpaces { get; set; } = new List<ContainerSpace>();

        /// <summary>
        /// Gets the amount of ContainerSpace in a ContainerRow.
        /// </summary>
        /// <param name="numberOfContainerStorageSpaces">Amount of ContainerSpaces to be created.</param>
        public ContainerStorageRow(int numberOfContainerStorageSpaces)
        {
            for (int i = 0; i < numberOfContainerStorageSpaces; i++)
            {
                RowOfContainerSpaces.Add(new ContainerSpace());
            }
        }

        /// <summary>
        /// Gets the cCntainerSpace that contains a container.
        /// </summary>
        /// <param name="container">Name of the container to be checked if it's contained.</param>
        /// <returns>Returns the containerSpace that contains the specified container, null if not found.</returns>
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
        /// Gets the ContainerSpace that contains a container.
        /// </summary>
        /// <param name="containerID">Unique ID for container to be checked if it's contained.</param>
        /// <returns>Returns the containerSpace that contains the specified container, null if not found.</returns>
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
        /// Adds container to available ContainerSpace.
        /// </summary>
        /// <param name="container">Unique name for the container to be added to containerSpace.</param>
        /// <returns>Returns the containerSpace the container was added to, null if not found.</returns>
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
        /// Checks if there is available ContainerSpace based on size.
        /// </summary>
        /// <param name="size">Size of containerSpace that will be checked for availability.</param>
        /// <returns>Returns true if containerSpace of given size is available, false if not found.</returns>
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
        /// Add container to available ContainerSpace.
        /// </summary>
        /// <param name="containerID">Unique ID for the container to be added.</param>
        /// <param name="sizeOfContainer">Size of the container to be added.</param>
        /// <returns>Returns the containerSpace the container was added to. If container was not not added, null is returned.</returns>
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
        /// Removes container from ContainerSpace.
        /// </summary>
        /// <param name="containerToBeRemoved">Name of the container to be removed.</param>
        /// <returns>Returns the ContainerSpacethe container was removed from, null if not found.</returns>
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
        /// Removes container from ContainerSpace.
        /// </summary>
        /// <param name="idOfContainerToBeRemoved">Unique ID of the container to be removed.</param>
        /// <returns>Returns the containerSpace the container was removed from, null if not found.</returns>
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
        /// Gets the amount of available containerSpaces.
        /// </summary>
        /// <returns>Returns an int value representing the total amount of available containerSpaces.</returns>
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
        /// Gets the size of the containers stored.
        /// </summary>
        /// <returns>Returns the ContainerSize enum representing the containers size of the ContainerSpaces that contains containers, if none is stored, none is returned.</returns>
        public ContainerSize SizeOfContainersStored()
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
        /// Gets all stored containers.
        /// </summary>
        /// <returns>Returns a IList with Guid objects with information of all the containers stored in a ContainerSpace.</returns>
        public IList<Guid> GetIDOfAllStoredContainers()
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
        /// Returns a String containing information about the ContainerSpace.
        /// </summary>
        /// <returns>Returns a String containing information about the ContainerSpace.</returns>
        public override String ToString()
        {
            return $"Row ID: {ID.ToString()}, Container storage spaces: {RowOfContainerSpaces}, Stored containers: {GetIDOfAllStoredContainers().Count}";
        }

    }

}
