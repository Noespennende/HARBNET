using System;
using HarbFramework;
using harbNet;

namespace harbNet
{
    public interface IHarbor
    {
        public Guid ID { get; }
        public IDictionary<Guid, bool> LoadingDockIsFreeForAllDocks();

        public Status GetShipStatus(Guid ShipID);

        public bool LoadingDockIsFree(Guid dockID);

        public Dictionary<Guid, bool> StatusAllDocks();

        public Dictionary<Ship, Status> GetStatusAllShips();
        public Guid GetAnchorageID();
        public Guid GetTransitID();
    }
}