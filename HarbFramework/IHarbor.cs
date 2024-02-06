using System;
using HarbFramework;
using harbNet;

namespace harbNet
{
    public interface IHarbor
    {
        public Guid ID { get; }
        public IDictionary<Guid, bool> LoadingDockIsFreeForAllDocks();

        public bool LoadingDockIsFree(Guid dockID);

        public Status GetShipStatus(Guid ShipID);

        public IDictionary<Guid, bool> ShipDockIsFreeForAllDocks();

        public IDictionary<Ship, Status> GetStatusAllShips();
        public Guid AnchorageID { get; }
        public Guid TransitLocationID { get; }
    }
}