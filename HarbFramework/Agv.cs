namespace Gruppe8.HarbNet
{
    /// <summary>
    /// Agv class representing Agv viechles. Automated Guided Viechles that can deliver containers from point A to B in the _harbor area.
    /// </summary>
    internal class Agv
    {
        /// <summary>
        /// Gets the unique ID for the Agv.
        /// </summary>
        /// <return>Returns a Guid object representing the Agv's unique ID.</return>
        internal Guid ID { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Represents the container stored in the AGV's cargo.
        /// </summary>
        /// <return>Returns a container object representing the container stored in the AGV's cargo.</return>
        internal Container? Container { get; set; }

        /// <summary>
        /// Get the ID of the current location of the AGV.
        /// </summary>
        /// <return>Returns a Guid object representing the current location of the AGV.</return>
        internal Guid Location {  get; set; }

        /// <summary>
        /// Create new objects of the AGV class.
        /// </summary>
        /// <param name="location">Guid object representing the ID of the location the Agv will start at.</param>
        internal Agv(Guid location)
        {
            Container = null;
            Location = location;
        }

        /// <summary>
        /// Loads a container on to the AGV's cargo.
        /// </summary>
        /// <param name="containerToBeLoaded">Container object to be loaded to the AGV's cargo.</param>
        /// <return>Returns a Guid object representing the ID of the container loaded to the AGV's cargo.</return>
        internal Guid LoadContainer(Container containerToBeLoaded)
        {
            Container = containerToBeLoaded;
            return containerToBeLoaded.ID;
        }

        /// <summary>
        /// Unloads the container in the AGV's cargo.
        /// </summary>
        /// <return>Returns a container object representing the container that was unloaded from the AGV's cargo.</return>
        internal Container? UnloadContainer()
        {
            Container? containerToBeUnloaded = Container;
            Container = null;
            return containerToBeUnloaded;
        }
    }
}
