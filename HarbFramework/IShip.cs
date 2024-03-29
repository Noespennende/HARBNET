﻿using Gruppe8.HarbNet;
using System.Collections.ObjectModel;

namespace Gruppe8.HarbNet
{
    /// <summary>
    /// Interface defining the public API of the Ship class.
    /// </summary>
    public interface IShip
    {
        /// <summary>
        /// Gets the unique ID for the ship.
        /// </summary>
        /// <returns>Returns a Guid object representing the ships unique ID.</returns>
        public Guid ID { get;  }
        /// <summary>
        /// Gets the ships size. 
        /// </summary>
        /// <returns>Returns a ShipSize enumm representing the ships size.</returns>
        public ShipSize ShipSize { get; }
        /// <summary>
        /// Gets the ships name. 
        /// </summary>
        /// <returns>Returns a string value representing the ships name.</returns>
        public String Name { get; }
        /// <summary>
        /// Gets the date and time the ship first started its voyage.
        /// </summary>
        /// <returns>Returns a DateTime object representing the date and time the ship first started its voyage.</returns>
        public DateTime StartDate { get; }
        /// <summary>
        /// Gets the number of days the ship uses to complete a roundtrip at sea before returning to harbour.
        /// </summary>
        /// <returns>Returns an int value representing the number of days the ship uses to do a round trip at sea.</returns>
        public int RoundTripInDays { get; }
        /// <summary>
        /// Gets the ID of the ships current location.
        /// </summary>
        /// <returns>Returns a Guid object representing the ID of the ships current location.</returns>
        public Guid CurrentLocation { get; }
        /// <summary>
        /// Gets a ReadOnlyCollecntion of StatusLog objects containing information on status changes the ship has gone through throughout a simulation.
        /// </summary>
        /// <returns>Returns an ReadOnlyCollection with StatusLog objects with information on status changes the ship has gone through throughout a simulation.</returns>
        public ReadOnlyCollection<StatusLog> History { get; }
        /// <summary>
        /// Gets all the containers in the ships storage.
        /// </summary>
        /// <returns>Returns an IList with Container objects representing the containers in the ships storage.</returns>
        public IList<Container> ContainersOnBoard { get; }
        /// <summary>
        /// Gets the container capacity of the ship.
        /// </summary>
        /// <returns>Returns an int value representing the max number of containers the ship can store.</returns>
        public int ContainerCapacity { get; }
        /// <summary>
        /// Gets the ships max weight the ship in tonns can be before it sinks.
        /// </summary>
        /// <returns>Returns an int value representing the max weight the ship can be in tonns.</returns>
        public int MaxWeightInTonn { get; }
        /// <summary>
        /// Gets the weight of the ship when its storage is empty.
        /// </summary>
        /// <returns>Returns an int value representing the weight of the ship when the storage is empty.</returns>
        public int BaseWeightInTonn { get; }
        /// <summary>
        /// Gets the current weight of the ship including the cargo weight. 
        /// </summary>
        /// <returns>Returns an int value representing the current weight of the ship.</returns>
        public int CurrentWeightInTonn { get; }
        /// <summary>
        /// Prints the ships entire history to console.
        /// </summary>
        public void PrintHistory();
        /// <summary>
        /// Returns the ships entire history in the form of a string.
        /// </summary>
        /// <returns> a String containing the ships entire history.</returns>
        public String HistoryToString();
        /// <summary>
        /// Returns a string containing information about the ship.
        /// </summary>
        /// <returns> a String containing information about the ship.</returns>
        public String ToString();
    }
}