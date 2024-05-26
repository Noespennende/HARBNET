using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gruppe8.HarbNet.Advanced;

namespace Gruppe8.HarbNet
{
    /// <summary>
    /// LoadingDocks used in a simulation. LoadingDocks are docks where ships can load or unload their cargo from or to the Harbor. 
    /// </summary>
    internal class LoadingDock : Dock
    {
        /// <summary>
        /// Gets a dictionary containing the location ID of a spot where trucks can load cargo directly from ships and the truck currently occupying this spot.
        /// </summary>
        /// <returns>Returns a IDictionary with the Guid object representing the location ID of a spot where a truck can load cargo directly from a ship as keys
        /// and the truck currently occupying this spot as values.</returns>
        internal IDictionary<Guid, Truck?> TruckLoadingSpots { get; set; } = new Dictionary<Guid, Truck?>();
        
        /// <summary>
        /// Creates a new LoadingDock object
        /// </summary>
        /// <param name="shipSize">ShipSize enum representing the size of ships that can dock to the LoadingDock. A LoadingDock can only hold
        /// ships of the cooresponding size.</param>
        internal LoadingDock(ShipSize shipSize) : base(shipSize)
        {
            this.Size = shipSize;
        }

        /// <summary>
        /// Assings the given truck to an available loadingspot. A loading spot is a place where trucks can load cargo directly from ships.
        /// </summary>
        /// <param name="truck">Truck object to be assigned to a loading spot.</param>
        /// <returns>Returns truck object if truck was assined loading spot, if not assigned null is returned.</returns>
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
        /// Checks if the given truck is stationed at one of the loadingspots in the LoadingDock.
        /// </summary>
        /// <param name="truck">Truck object to be checked if assigned to a loading spot.</param>
        /// <returns>Returns a bool value. True is returned if truck exists in one of the docks loadingspots, if not false is returned.</returns>
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
        /// Removes the given truck from the LoadingDocks loadingspot.
        /// </summary>
        /// <param name="truck">Truck object to be removed from loading spot.</param>
        /// <returns>Returns the truck object if removed from a loading spot, if not null is returned.</returns>
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
    }
}
