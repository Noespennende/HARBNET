using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe8.HarbNet
{
    /// <summary>
    /// ContainerRow used as storage space for containers in harbor
    /// </summary>
    internal class ContainerRow
    {
        /// <summary>
        /// Gets the unique ID for the containerRow
        /// </summary>
        /// <returns>Returns the unique ID defining a specific containerRow</returns>
        internal Guid ID { get; } = Guid.NewGuid();

        internal ContainerSize SizeOfContainersStored { get; }

        /// <summary>
        /// Gets the space in the containerRow
        /// </summary>
        /// <returns>Returns a list of the containerspace in a containerRow</returns>
        internal IList<ContainerSpace> RowOfContainerSpaces { get; set; }

        /// <summary>
        /// Gets the containerRow containing a list of containerSpace
        /// </summary>
        /// <param name="RowOfContainerSpaces">The containerSpace in a row</param>
        internal ContainerRow(IList<ContainerSpace> RowOfContainerSpaces)
        {
            this.RowOfContainerSpaces = RowOfContainerSpaces;
        }

        /// <summary>
        /// Gets the fullsize- and halfSizeContainerspaces in a ContainerRow by adding the specified amount of a containerSize to the containerSpaces in a containerRow
        /// </summary>
        /// <param name="numberOfFullSizeContainerSpaces">Amount of fullSize containerSpaces</param>
        /// <param name="numberOfHalfSizeContainerSpaces">Amount of halfSize containerSpaces</param>
        /// <returns>Returns the containerRow with the amount of fullSize- and halfSizeContainers</returns>
        internal ContainerRow (ContainerSize containerSpaceSize, int numberOfContainerSpaces)
        {
            RowOfContainerSpaces = new List<ContainerSpace>();
            this.SizeOfContainersStored = containerSpaceSize;

            if (containerSpaceSize == ContainerSize.Full)
            {
                
                for (int i = 0; i < numberOfContainerSpaces; i++)
                {
                    RowOfContainerSpaces.Add(new ContainerSpace(ContainerSize.Full));
                }
            } else if (containerSpaceSize == ContainerSize.Full)
            {
                for (int i = 0; i < numberOfContainerSpaces; i++)
                {
                    RowOfContainerSpaces.Add(new ContainerSpace(ContainerSize.Half));
                }

            } else
            {
                throw new ArgumentOutOfRangeException("The containerSpaceSize you are trying to send inn must be either Half or Full");
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
                if (space.storedContainer == container.ID)
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
                if (space.storedContainer == containerID)
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
            foreach (ContainerSpace space in RowOfContainerSpaces)
            {
                if (space.Size == container.Size && space.Free == true)
                {
                    space.storedContainer = container.ID;
                    return space;
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
            foreach (ContainerSpace space in RowOfContainerSpaces)
            {
                if (space.Size == size && space.Free == true)
                {
                    return true;
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
            foreach (ContainerSpace space in RowOfContainerSpaces)
            {
                if (space.Size == sizeOfContainer && space.Free == true)
                {
                    space.storedContainer = containerID;
                    return space;
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
                if (containerToBeRemoved.ID == space.storedContainer)
                {
                    space.storedContainer = Guid.Empty;
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
                if (idOfContainerToBeRemoved == space.storedContainer)
                {
                    space.storedContainer = Guid.Empty;
                    return space;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the amount of available containerSpaces
        /// </summary>
        /// <returns>Returns the total amount of available containerSpaces</returns>
        internal int numberOfFreeContainerSpaces ()
        {
            int count = 0;
            foreach (ContainerSpace space in RowOfContainerSpaces)
            {
                if (space.Free)
                {
                    count++;
                }
            }
            return count;
        }

    }

}
