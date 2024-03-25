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
    /// Each container storage row represents one row of storage spaces where containers can be stored.
    /// </summary>
    public class ContainerStorageRow : IContainerStorageRow
    {
        /// <summary>
        /// Gets the unique ID for the ContainerRow
        /// </summary>
        /// <returns>Returns the unique ID defining a specific containerRow</returns>
        public Guid ID { get; } = Guid.NewGuid();

        /// <summary>
        /// Gets the storage spaces in the containerRow
        /// </summary>
        /// <returns>Returns a list of the containerspace in a containerRow</returns>
        internal IList<ContainerSpace> RowOfContainerSpaces { get; set; } = new List<ContainerSpace>();

        /// <summary>
        /// Gets the containerRow containing a list of containerSpace
        /// </summary>
        /// <param name="RowOfContainerSpaces">The containerSpace in a row</param>
        public ContainerStorageRow(int numberOfContainerStorageSpaces)
        {
            for (int i = 0; i < numberOfContainerStorageSpaces; i++)
            {
                RowOfContainerSpaces.Add(new ContainerSpace());
            }
        }

        /// <summary>
        /// Gets the containerSpace that contains a specific container
        /// </summary>
        /// <param name="container">unique name for container</param>
        /// <returns>Returns the containerRow that contains the specified container. If container is not found, null is returned</returns>
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
        /// Gets the containerSpace that contains a specific container
        /// </summary>
        /// <param name="containerID">Unique ID for container</param>
        /// <returns>Returns the containerRow as a containerSpace list that contains the space the specified container is in. If container is not found, null is returned</returns>
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
        /// Adds specified container to available containerSpace
        /// </summary>
        /// <param name="container">Unique name for container to be added to containerSpace</param>
        /// <returns>Returns the containerRow as a containerSpace list that contains the space the specified container was added to. If free space is not found for the container, null is returned</returns>
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
        /// Checks if there is available containerSpace based on size
        /// </summary>
        /// <param name="size">Size of containerSpace that will be checked for availability</param>
        /// <returns>Returns true if containerSpace of speicified size is available, returns false if not found</returns>
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
        /// Adds specific container to available containerSpace
        /// </summary>
        /// <param name="containerID">Unique ID for the container to be added</param>
        /// <param name="sizeOfContainer">Size of the container to be added</param>
        /// <returns>Returns the containerRow as a containerSpace list that contains the space the container was added to, if the size of the container matched with available space size. If not, null is returned</returns>
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
        /// Removes specific container from containerRow
        /// </summary>
        /// <param name="containerToBeRemoved">Unique name of specific container to be removed</param>
        /// <returns>Returns the containerRow as a containerSpace list that contains the space the container was removed from. If container was not found, null is returned</returns>
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
        /// Removes specific container from containerRow
        /// </summary>
        /// <param name="idOfContainerToBeRemoved">Unique ID of specific container to be removed</param>
        /// <returns>Returns the containerRow as a containerSpace list that contains the space the container was removed from. If container was not found, null is returned</returns>
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
        /// Gets the amount of available containerSpaces
        /// </summary>
        /// <returns>Returns the total amount of available containerSpaces</returns>
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

        public override String ToString()
        {
            return $"Storage row ID: {ID.ToString()}, Container storage spaces: {RowOfContainerSpaces}, Stored containers: {GetIDOfAllStoredContainers().Count}";
        }

    }

}
