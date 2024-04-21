using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe8.HarbNet
{
    internal class LoadingDock : Dock
    {
        /// <summary>
        /// Gets all truck loading spots.
        /// </summary>
        /// <returns>Returns a dictionary with all trucks that are in a loading spot.</returns>
        internal IDictionary<Guid, Truck?> TruckLoadingSpots { get; set; } = new Dictionary<Guid, Truck?>();
        
        /// <summary>
        /// Creates a new LoadingDock object.
        /// </summary>
        /// <param name="shipSize">ShipSize enum representing the size of the ship the loading dock to be created can hold.</param>
        internal LoadingDock(ShipSize shipSize) : base(shipSize)
        {
            this.Size = shipSize;
        }

        /// <summary>
        /// Assigns truck to available loading spot.
        /// </summary>
        /// <param name="truck">Truck object to be assigned loading spot.</param>
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
        /// Checks if truck exists in loading spot.
        /// </summary>
        /// <param name="truck">Truck object to be checked if assigned to a loading spot.</param>
        /// <returns>Returns a bool value. True is returned if truck exists in loading spot, if not false is returned.</returns>
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
        /// Removes truck from loading spot.
        /// </summary>
        /// <param name="truck">Truck object to be removed from loading spot.</param>
        /// <returns>Returns truck object if removed from loading spot, if not null is returned.</returns>
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

        // FIKSES ELLER FJERNES NÅR HARBOR HAR IMPLEMENTASJON FOR ALLE LOADINGDOCK-CRANER 
        /*
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
        */

    }
}
