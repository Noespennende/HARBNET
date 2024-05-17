using System;

namespace Gruppe8.HarbNet.PublicApiAbstractions
{
    /// <summary>
    /// Abstract class defining the public API of Ports such as the Harbor class.
    /// </summary>
    public abstract class Port
    {
        /// <summary>
        /// Gets the unique ID for the harbor.
        /// </summary>
        /// <return>Returns a Guid object representing the harbors unique ID.</return>
        public abstract Guid ID { get; internal set; }
        /// <summary>
        /// Checks if loading dock is available for all dock sizes.
        /// </summary>
        /// <returns>Returns an IDictionary containing Guid representing the loading docks and bool value representing if the loading docks are available or not. The Dictionary with available loading docks are returned, if no docks are available null is returned.</returns>
        public abstract IDictionary<Guid, bool> LoadingDockIsFreeForAllDocks();
        /// <summary>
        /// Get all containers that have left the harbor and arived at their destination
        /// </summary>
        /// <return>Returns a IList of all containers that have arrived at their destination during a simulation</return>
        public abstract IList<Container> ArrivedAtDestination { get; internal set; }
        /// <summary>
        /// Gets last registered status of specific ship object.
        /// </summary>
        /// <param name="ShipID">Unique ID of the ship object to get status from.</param>
        /// <returns>Returns a Status enum representing the last registered status of specified ship if the ship has a history, if no status is registered null is returned.</returns>
        public abstract Status GetShipStatus(Guid ShipID);
        /// <summary>
        /// Checks if ship dock is available for all dock sizes.
        /// </summary>
        /// <returns>Returns an IDictionary containing Guid representing the ship docks and bool value representing if the ship docks are available or not. The Dictionary with available ship docks are returned, if no docks are available null is returned.</returns>
        public abstract IDictionary<Guid, bool> ShipDockIsFreeForAllDocks();
        /// <summary>
        /// Gets the last registered status from all ships.
        /// </summary>
        /// <returns>Return an IDictionary containing Ship objects and Status enum representing the last registered status of the ships, if they have a status.</returns>
        public abstract IDictionary<Ship, Status> GetStatusAllShips();
        /// <summary>
        /// Gets the unique ID for the anchorage.
        /// </summary>
        /// <return>Returns a Guid representing the harbors anchorages.</return>
        public abstract Guid AnchorageID { get; }
        /// <summary>
        /// Gets the unique ID for the transit location.
        /// </summary>
        /// <return>Returns a Guid representing the harbors transit location.</return>
        public abstract Guid TransitLocationID { get; }
        /// <summary>
        /// Gets the unique ID for the Agv cargo.
        /// </summary>
        /// <return>Returns a Guid representing the harbors Agv cargos.</return>
        public abstract Guid AgvCargoID { get; }
        /// <summary>
        /// Gets the unique ID for the truck transit location.
        /// </summary>
        /// <return>Returns a Guid representing the harbors truck transit locations.</return>
        public abstract Guid TruckTransitLocationID { get; }
        /// <summary>
        /// Gets the unique ID for the truck queue location.
        /// </summary>
        /// <return>Returns a Guid representing the harbors truck queue locations.</return>
        public abstract Guid TruckQueueLocationID { get; }
        /// <summary>
        /// Gets the unique ID for the harbor storage area.
        /// </summary>
        /// <return>Returns a Guid representing the harbors storage areas.</return>
        public abstract Guid HarborStorageAreaID { get; }
        /// <summary>
        /// Gets the unique ID for the dock area.
        /// </summary>
        /// <return>Returns a Guid representing the harbors dock areas.</return>
        public abstract Guid HarborDockAreaID { get; }
        /// <summary>
        /// The ID of a containers destination.
        /// </summary>
        /// <return>The ID of a containers destination.</return>
        public abstract Guid DestinationID { get; }
        /// <summary>
        /// Returns a string with the harbor ID, amount of ships in loading dock, amount of free loading docks, amount of ships in ship dock, amount of free ship docks, amount of ships in anchorage, amount of ships in transit and amount of containers stored in harbor. .
        /// </summary>
        /// <returns>String value containing information about the harbour, its ships and container spaces.</returns>
        public abstract string GetStatusAllLoadingDocks();

        /// <summary>
        /// Gets the status of specified container.
        /// </summary>
        /// <param name="ContainerId">Unique ID of the container object to get last registered status from.</param>
        /// <returns>Returns a string value representing the container ID and their last registered status from Status enums.</returns>
        public abstract string GetContainerStatus(Guid ContainerId);

      
        /// <summary>
        /// Gets the status of all containers.
        /// </summary>
        /// <returns>Returns a string value representing the container ID and their last registered status from Status enums.</returns>
        public abstract string GetAllContainerStatus();


        /// <summary>
        /// Gets the status of all loading docks if they are available or not.
        /// </summary>
        /// <returns>Returns a string value representing all the dock IDs and if they are available or not.</returns>
        public abstract override string ToString();

        /// <summary>
        /// Constructor for the Port class.
        /// </summary>
        internal Port() { }
    }
}