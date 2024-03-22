namespace Gruppe8.HarbNet
{
    public interface IContainerStorageRow
    {
        /// <summary>
        /// Gets the unique ID for the containerRow
        /// </summary>
        /// <returns>Returns the unique ID defining a specific containerRow</returns>
        public Guid ID { get; }
        public int numberOfFreeContainerSpaces(ContainerSize size);
        public ContainerSize SizeOfContainersStored();
        public IList<Guid> GetIDOfAllStoredContainers();
        public String ToString();

    }
}