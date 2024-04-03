using System.Collections.ObjectModel;

namespace Gruppe8.HarbNet
{
    public interface IContainerStorageRow
    {
        /// <summary>
        /// Gets the unique ID for the containerRow.
        /// </summary>
        /// <returns>Returns a Guid object representing the containerRows unique ID.</returns>
        public Guid ID { get; }
        /// <summary>
        /// Gets the amount of available containerSpaces.
        /// </summary>
        /// <returns>Returns an int value representing the total amount of available containerSpaces.</returns>
        public int numberOfFreeContainerSpaces(ContainerSize size);
        /// <summary>
        /// Gets the size of the containers stored.
        /// </summary>
        /// <returns>Returns the ContainerSize enum representing the containers size of the ContainerSpaces that contains containers, if none is stored, none is returned.</returns>
        public ContainerSize SizeOfContainersStored { get; }
        /// <summary>
        /// Gets all stored containers.
        /// </summary>
        /// <returns>Returns a IList with Guid objects with information of all the containers stored in a ContainerSpace.</returns>
        public ReadOnlyCollection<Guid> IDOfStoredContainers { get; }
        /// <summary>
        /// Returns a String containing information about the ContainerSpace.
        /// </summary>
        /// <returns>Returns a String containing information about the ContainerSpace.</returns>
        public String ToString();

    }
}