using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe8.HarbNet
{
    /// <summary>
    /// Dock for docking ships to the harbor
    /// </summary>
    internal class Dock
    {
        /// <summary>
        /// Gets the unique ID for the dock
        /// </summary>
        /// <returns>Returns a Guid representing the docks unique ID</returns>
        internal Guid ID { get; } = Guid.NewGuid();
        /// <summary>
        /// Gets or sets the unique ID for the dock
        /// </summary>
        /// <returns>Returns a ShipSize enum representing the size of ships the dock can recieve/returns>
        internal ShipSize Size { get; set; }
        /// <summary>
        /// Gets or sets wether or not the Dock is free to recieve a ship or not.
        /// </summary>
        /// <returns>Returns a boolean that is true if the dock is free and false if it is not</returns>
        internal bool Free { get; set; }
        /// <summary>
        /// Gets or sets the ID of the ship docked to the dock.
        /// </summary>
        /// <returns>Returns a Guid representing the unique ID of the ship docked to this dock</returns>
        internal Guid DockedShip {  get; set; }

        /// <summary>
        /// Gets all the cranes in the harbor
        /// </summary>
        /// <returns>Returns an IList with crane objects representing the cranes in the harbor</returns>
        internal IList<Crane> AssignedCranes { get; set; } = new List<Crane>();

        /// <summary>
        /// Gets all the truck loading spots
        /// </summary>
        /// <returns>Returns a dictionary with all the loading spots for trucks</returns>
        internal IDictionary<Guid, Truck?> TruckLoadingSpots { get; set; } = new Dictionary<Guid, Truck?>();

        /// <summary>
        /// Creates a new dock object
        /// </summary>
        ///  <param name="shipSize">Size of the ships that the dock will allow to dock to it</param>
        internal Dock (ShipSize shipSize)
        {
            this.Size = shipSize;
            this.Free = true;
        }

        /// <summary>
        /// Assigns specific truck to available loading spot for trucks
        /// </summary>
        /// <param name="truck">name of spesific truck to be assigned loading spot</param>
        /// <returns>Returns null</returns>
        internal Truck? AssignTruckToTruckLoadingSpot(Truck truck)
        {
            
            foreach (var spot in TruckLoadingSpots)
            {
                if (spot.Value == null)
                {
                    TruckLoadingSpots.Add(spot.Key, truck);
                }
            }
            return null;
        }

        /// <summary>
        /// Checks if specific truck has been assigned a loading spot
        /// </summary>
        /// <param name="truck">name of specific truck</param>
        /// <returns>>Returns true if the specified truck exists in a loading spot, and returns false if the truck does not have a loading spot</returns>
        internal bool TruckExistsInTruckLoadingSpots(Truck truck)
        {
            foreach (var spot in TruckLoadingSpots)
            {
                if (spot.Value == truck)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Removes truck from loading spot
        /// </summary>
        /// <param name="truck">name of truck to be removed from loading spot</param>
        /// <returns>Returns the truck that was found in a loading spot and has been removed, or returns null if truck was not found in loading spot</returns>
        internal Truck? RemoveTruckFromTruckLoadingSpot(Truck truck)
        {
            foreach (var spot in TruckLoadingSpots)
            {
                if (spot.Value == truck)
                {
                    TruckLoadingSpots[spot.Key] = null;
                    return truck;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets cranes that has free loading docks
        /// </summary>
        /// <returns>Returns cranes that does not load any containers, or null if they are loading any containers</returns>
        internal Crane? GetFreeLoadingDockCrane()
        {
            foreach (Crane crane in AssignedCranes)
            {
                if (crane.Container == null)
                {
                    return crane;
                }
            }
            return null;
        }


    }
}
