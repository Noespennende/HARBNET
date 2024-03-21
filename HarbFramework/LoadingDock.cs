using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe8.HarbNet
{
    internal class LoadingDock : Dock
    {
        internal IList<Crane> AssignedCranes { get; set; } = new List<Crane>();

        internal IDictionary<Guid, Truck?> TruckLoadingSpots { get; set; } = new Dictionary<Guid, Truck?>();

        internal LoadingDock(ShipSize shipSize, IList<Crane> cranesNextToDock)
        {
            this.Size = shipSize;
            this.AssignedCranes = cranesNextToDock;
        }

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
