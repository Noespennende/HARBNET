using System;
using HarbFramework;
using harbNet;

namespace harbNet
{
    public interface IHarbor
    {
        public string GetStatusAllLoadingDocks();

        public Status GetShipStatus(Guid ShipID);

        public string GetLoadingDockStatus(Guid dockID);

        public Dictionary<Guid, bool> StatusAllDocks();

        public Dictionary<Ship, Status> GetStatusAllShips();
    }
}