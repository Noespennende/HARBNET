using Microsoft.VisualBasic;
using System;

namespace Gruppe8.HarbNet.Advanced
{
    /// <summary>
    /// Abstract class defining the public API of Ports such as the Harbor class.
    /// This abstract class can be used to make fakes to be used in testing of the API. 
    /// </summary>
    public abstract class Port
    {
        /// <summary>
        /// Gets the unique ID for the Port.
        /// </summary>
        /// <return>Returns a Guid object representing the Port's unique ID.</return>
        public abstract Guid ID { get; internal set; }
        /// <summary>
        /// Gets a IDictionary containing information about the availabilety of all the loading docks in the Port.
        /// </summary>
        /// <returns>Returns an IDictionary containing Guid representing the loading docks and bool value representing if the loading docks are available or not. The key value is a Guid object
        /// representing the unque ID of a given dock. The value is a bool representing wether or not the dock is free. A bool value of True means the coresponding dock is free.</returns>
        public abstract IList<Container> ArrivedAtDestination { get; internal set; }
        /// <summary>
        /// Gets the unique ID for the location representing the Port's anchorage.
        /// </summary>
        /// <return>Returns a Guid representing the location of the Port's anchorage.</return>
        public abstract Guid AnchorageID { get; }
        /// <summary>
        /// Gets the unique ID for the location of ships in transit to their delivery destination.
        /// </summary>
        /// <return>Returns a Guid representing the location of ships in transit to their delivery destination.</return>
        public abstract Guid TransitLocationID { get; }
        /// <summary>
        /// Gets the unique ID representing the location of an AVGs cargo. A container who's location is represented by this ID is currently being moved by an AGV.
        /// </summary>
        /// <return>Returns a Guid object containing the ID representing an AVGs cargo.</return>
        public abstract Guid AgvCargoID { get; }
        /// <summary>
        /// Gets the unique ID of the loaction representing trucks who are in transit to their delivery destination.
        /// </summary>
        /// <return>Returns a Guid representing the loaction representing trucks who are in transit to their delivery destination.</return>
        public abstract Guid TruckTransitLocationID { get; }
        /// <summary>
        /// Gets the unique ID for the location where trucks queue to enter the port.
        /// </summary>
        /// <return>Returns a Guid representing the location where trucks queue to enter the port.</return>
        public abstract Guid TruckQueueLocationID { get; }
        /// <summary>
        /// Gets the unique ID representing the location of the Port's storage area for containers.
        /// </summary>
        /// <return>Returns a Guid representingthe Port's storage area for containers.</return>
        public abstract Guid HarborStorageAreaID { get; }
        /// <summary>
        /// Gets the unique ID representing the loaction of the Ports dock area.
        /// </summary>
        /// <return>Returns a Guid representing the loaction of the Ports dock area.</return>
        public abstract Guid HarborDockAreaID { get; }
        /// <summary>
        /// The unique ID representing the location of a containers destination. If a container has this ID as their current location it means that they have arrived at
        /// their final destination.
        /// </summary>
        /// <return> A Guid object containing the ID representing the location of a containers final destination..</return>
        public abstract Guid DestinationID { get; }
        /// <summary>
        /// Returns a string with information about the status of all loading docks in the port. A loading dock is a dock used for loading cargo from and to Ships.
        /// </summary>
        /// <returns>String value containing information about the status of all the loading docks in the port.</returns>
        public abstract IDictionary<Guid, bool> GetAvailabilityStatusForAllLoadingDocks();
        /// <summary>
        /// Get all containers that have left the Port and arived at their destination
        /// </summary>
        /// <return>Returns a IList containing all the containers that have arrived at their destination during a simulation</return>
        public abstract string GetStatusAllLoadingDocks();

        /// <summary>
        /// Gets a IDictionary containing information about the status of all the ships in the simulation.
        /// </summary>
        /// <return>Returns a IDictionary containing information about the status of all the ships in the simulation. The Keyvalue in the dictonary is a ship object representing the ship
        /// and the Value is a Status enum representing the current status of the ship.</return>
        public abstract IDictionary<Ship, Status> GetStatusAllShips();

        /// <summary>
        /// Gets the last registered status of a specific ship object.
        /// </summary>
        /// <param name="ShipID">Guid representing the unique ID of the ship object witch status is to be returned.</param>
        /// <returns>Returns a Status enum representing the last registered status of specified ship if the ship has been used in a simulation.</returns>
        public abstract Status GetShipStatus(Guid ShipID);

        /// <summary>
        /// Gets a IDictionary containing information about the availabilety of all the ship docks in the Port. A ship dock is a dock where ships can be stored once their voyage is completed.
        /// </summary>
        /// <returns>Returns an IDictionary containing Guid representing the ship docks and bool value representing if the ship docks are available or not. The key value is a Guid object
        /// representing the unque ID of a given dock. The value is a bool representing wether or not the dock is free. A bool value of True means the coresponding dock is free.</returns>
        public abstract IDictionary<Guid, bool> GetAvailabilityStatusForAllShipDocks();

        /// <summary>
        /// Gets the current status of the container with the given ID.
        /// </summary>
        /// <param name="ContainerId">Guid object represting the unique ID of the container object in which the current status is to be returned.</param>
        /// <returns>Returns a string value representing the container's ID and their last registered status.</returns>
        public abstract string GetContainerStatus(Guid ContainerId);

        /// <summary>
        /// Gets the current status of all containers.
        /// </summary>
        /// <returns>Returns a string value representing the container's ID and their last registered status.</returns>
        public abstract string GetAllContainerStatus();

        /// <summary>
        /// Gets a string containing information about the Port.
        /// </summary>
        /// <returns>Returns a string value containing information about the Port.</returns>
        public abstract override string ToString();

        /// <summary>
        /// Constructor for the Port class.
        /// </summary>
        internal Port() { }
    }
}