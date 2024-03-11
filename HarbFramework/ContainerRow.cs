using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe8.HarbNet
{
    internal class ContainerRow
    {
        internal Guid ID { get; } = Guid.NewGuid();
        internal IList<ContainerSpace> RowOfContainerSpaces { get; set; }

        internal ContainerRow(IList<ContainerSpace> RowOfContainerSpaces)
        {
            this.RowOfContainerSpaces = RowOfContainerSpaces;
        }

        internal ContainerRow (int numberOfFullSizeContainerSpaces, int numberOfHalfSizeContainerSpaces)
        {
            RowOfContainerSpaces = new List<ContainerSpace>();

            for (int i = 0; i < numberOfFullSizeContainerSpaces; i++)
            {
                RowOfContainerSpaces.Add(new ContainerSpace(ContainerSize.Full));
            }

            for (int i = 0;i < numberOfHalfSizeContainerSpaces; i++)
            {
                RowOfContainerSpaces.Add(new ContainerSpace(ContainerSize.Half));
            }
        }

        internal ContainerSpace GetContainerSpaceContainingContainer (Container container)
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

        internal ContainerSpace AddContainerToFreeSpace (Container container)
        {
            foreach(ContainerSpace space in RowOfContainerSpaces)
            {
                if (space.Size == container.Size && space.Free == true)
                {
                    space.storedContainer = container.ID;
                    return space;
                }
            }

            return null;
        }

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

        internal ContainerSpace RemoveContainerFromContainerRow (Container containerToBeRemoved)
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

    }

}
