namespace Gruppe8.HarbNet.PublicApiAbstractions
{
    /// <summary>
    /// Abstract class defining the public API for StorageAreas such as ContainerStorageRow
    /// </summary>
    public abstract class StorageArea
    {
        /// <summary>
        /// Gets the unique ID for the containerRow.
        /// </summary>
        /// <returns>Returns a Guid object representing the containerRows unique ID.</returns>
        public abstract Guid ID { get; }
        /// <summary>
        /// Gets the amount of available ContainerSpace in RowOfContainerSpaces.
        /// </summary>
        /// <param name="size">ContinerSize enum the ContainerSpace use to be checked for availability.</param>
        /// <returns>Returns an int value representing the total amount of available ContainerSpace.</returns>
        public abstract int NumberOfFreeContainerSpaces(ContainerSize size);
        /// <summary>
        /// Gets all the ContainerSize enums from the containers stored in RowOfContainerSpaces.
        /// </summary>
        /// <returns>Returns the ContainerSize enum representing the containers size from the ContainerSpaces in RowOfContainerSpaces that contains containers, if no ContainerSize is found, none is returned.</returns>
        public abstract ContainerSize SizeOfContainersStored();
        /// <summary>
        /// Gets an IList with Guid objects representing the ID of the stored containers registered in RowOfContainerSpaces.
        /// </summary>
        /// <returns>Returns a IList with Guid objects from all the containers stored in RowOfContainerSpaces.</returns>
        public abstract IList<Guid> GetIDOfAllStoredContainers();
        /// <summary>
        /// Returns a String with the Storage Row ID, the RowOfContainerSpaces list and amount of stored containers registered.
        /// </summary>
        /// <returns>Returns a String with the Storage Row ID, the RowOfContainerSpaceList IList with ContainerSpace objects and an int value representing the amount of stored containers registered.</returns>
        public abstract override string ToString();

        /// <summary>
        /// Constructor for the StorageArea class.
        /// </summary>
        internal StorageArea() { }

    }
}