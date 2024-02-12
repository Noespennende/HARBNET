using System;
using HarbFramework;
using harbNet;

namespace harbNet
{
    /// <summary>
    /// Interface defining the contract for the public API of the Harbor class.
    /// </summary>
    public interface IHarbor
    {
        /// <summary>
        /// Unique ID for harbor
        /// </summary>
        /// <return>Returns the unique ID defining a specific harbor</return>
        public Guid ID { get; }

        /// <summary>
        /// Checks if loading dock is free/available for all dock sizes
        /// </summary>
        /// <returns>Returns all the docks or null</returns>
        public IDictionary<Guid, bool> LoadingDockIsFreeForAllDocks();

        /// <summary>
        /// Checks if specified dock is free
        /// </summary>
        /// <param name="dockID">Unique ID of specific dock</param>
        /// <returns>Returns true if specified dock is free, or false if not</returns>
        public bool LoadingDockIsFree(Guid dockID);

        /// <summary>
        /// Gets last registered status of specific ship
        /// </summary>
        /// <param name="ShipID">Unique ID of specific ship</param>
        /// <returns>Returns last registered status of specified ship if the ship has a history, or returns none</returns>
        public Status GetShipStatus(Guid ShipID);

        /// <summary>
        /// Checks if ship dock is free/available for all dock sizes
        /// </summary>
        /// <returns>Returns true if all ship docka is free, or false if not</returns>
        public IDictionary<Guid, bool> ShipDockIsFreeForAllDocks();

        /// <summary>
        /// Gets the last registered status from all ships
        /// </summary>
        /// <returns>Return the last registered status of all ships, if they have a status</returns>
        public IDictionary<Ship, Status> GetStatusAllShips();

        /// <summary>
        /// Gets the unique ID for the anchorage
        /// </summary>
        /// <return>Returns the unique Guid defining a specific anchorage</return>
        public Guid AnchorageID { get; }

        /// <summary>
        /// Gets the unique ID for the transit location
        /// </summary>
        /// <return>Returns an unique Guid defining a specific transitlocation</return>
        public Guid TransitLocationID { get; }
    }
}