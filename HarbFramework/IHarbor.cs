using System;
using Gruppe8.HarbNet;

namespace Gruppe8.HarbNet
{
    /// <summary>
    /// Interface defining the contract for the public API of the Harbor class.
    /// </summary>
    public interface IHarbor
    {
        /// <summary>
        /// Gets the unique ID for the harbor.
        /// </summary>
        /// <return>Returns a Guid object representing the harbors unique ID.</return>
        public Guid ID { get; }
        /// <summary>
        /// Checks if loading dock is available for all dock sizes.
        /// </summary>
        /// <returns>Returns a dictionary with all the available loading docks, if no docks are available null is returned.</returns>
        public IDictionary<Guid, bool> LoadingDockIsFreeForAllDocks();
        /// <summary>
        /// Get all containers that have left the harbor and arived at their destination
        /// </summary>
        /// <return>Returns a IList of all containers that have arrived at their destination during a simulation</return>
        public IList<Container> ArrivedAtDestination { get; }
        /// <summary>
        /// Gets last registered status of specific ship.
        /// </summary>
        /// <param name="ShipID">Unique ID of ship to get status from.</param>
        /// <returns>Returns a Status enum representing the last registered status of specified ship if the ship has a history, if no status is registered null is returned.</returns>
        public Status GetShipStatus(Guid ShipID);
        /// <summary>
        /// Checks if ship dock is available for all dock sizes.
        /// </summary>
        /// <returns>Returns a dictionary with ship docks available for all dock sizes.</returns>
        public IDictionary<Guid, bool> ShipDockIsFreeForAllDocks();
        /// <summary>
        /// Gets the last registered status from all ships.
        /// </summary>
        /// <returns>Return a dictionary with the last registered status of all ships, if they have a status.</returns>
        public IDictionary<Ship, Status> GetStatusAllShips();
        /// <summary>
        /// Gets the unique ID for the anchorage.
        /// </summary>
        /// <return>Returns a Guid representing the harbors anchorages.</return>
        public Guid AnchorageID { get; }
        /// <summary>
        /// Gets the unique ID for the transit location.
        /// </summary>
        /// <return>Returns a Guid representing the harbors transit location.</return>
        public Guid TransitLocationID { get; }
        /// <summary>
        /// Gets the unique ID for the Adv cargo.
        /// </summary>
        /// <return>Returns a Guid representing the harbors Adv cargos.</return>
        public Guid AdvCargoID { get; }
        /// <summary>
        /// Gets the unique ID for the truck transit location.
        /// </summary>
        /// <return>Returns a Guid representing the harbors truck transit locations.</return>
        public Guid TruckTransitLocationID { get; }
        /// <summary>
        /// Gets the unique ID for the truck queue location.
        /// </summary>
        /// <return>Returns a Guid representing the harbors truck queue locations.</return>
        public Guid TruckQueueLocationID { get; }
        /// <summary>
        /// Gets the unique ID for the harbor storage area.
        /// </summary>
        /// <return>Returns a Guid representing the harbors storage areas.</return>
        public Guid HarborStorageAreaID { get; }
        /// <summary>
        /// Gets the unique ID for the dock area.
        /// </summary>
        /// <return>Returns a Guid representing the harbors dock areas.</return>
        public Guid HarborDockAreaID { get; }
        /// <summary>
        /// The ID of a containers destination.
        /// </summary>
        /// <return>The ID of a containers destination.</return>
        public Guid DestinationID { get; }
        /// <summary>
        /// Returns a string value containing information about the harbour, its ships and container spaces.
        /// </summary>
        /// <returns>String value containing information about the harbour, its ships and container spaces.</returns>
        public string ToString();
    }
}