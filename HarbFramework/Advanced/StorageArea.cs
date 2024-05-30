namespace Gruppe8.HarbNet.Advanced
{
    /// <summary>
    /// Abstract class defining the public API for StorageAreas such as the ContainerStorageRow class. Each storage Area can only store containers of one size at the time.
    /// This abstract class can be used to make fakes to be used in testing of the API. 
    /// </summary>
    public abstract class StorageArea
    {
        /// <summary>
        /// Gets the unique ID of the StorageArea.
        /// </summary>
        /// <returns>Returns a Guid object representing the StorageAreaæs unique ID.</returns>
        public abstract Guid ID { get; }
        
        /// <summary>
        /// Gets the amount of available ContainerSpace in the StorageArea.
        /// </summary>
        /// <param name="size">ContinerSize enum representing the size of the free container spaces to be counted.</param>
        /// <returns>Returns an int value representing the total amount of available Container space of the given size.</returns>
        public abstract int NumberOfFreeContainerSpaces(ContainerSize size);
        
        /// <summary>
        /// Gets the size of the containers stored in the StorageArea.
        /// </summary>
        /// <returns>Returns a ContainerSize enum representing the size of the containers stored in the StorageArea, if no ContainerSize is found,the enum size of "None" is returned.</returns>
        public abstract ContainerSize SizeOfContainersStored();
        
        /// <summary>
        /// Gets an IList with Guid objects representing the ID of all stored containers in the StorageArea.
        /// </summary>
        /// <returns>Returns a IList with Guid objects from all the containers stored in the StorageArea.</returns>
        public abstract IList<Guid> GetIDOfAllStoredContainers();
        
        /// <summary>
        /// Returns a string containing information about the StorageArea.
        /// </summary>
        /// <returns>Returns a String value containing information about the StorageArea.</returns>
        public abstract override string ToString();

        /// <summary>
        /// Constructor for the StorageArea class.
        /// </summary>
        internal StorageArea() 
        { 
        }

    }
}