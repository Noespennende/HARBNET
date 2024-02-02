using System;
using HarbFramework;
using harbNet;

namespace harbNet
{
    public interface IHarbor
    {
        public string GetShipStatus(Guid ShipID);
        public string GetDockStatus(Guid dockID);
        public string GetStatusAllDocks();
        public string GetStatusAllShips();

        public Status GetShipStatus(Guid ShipID);

        public string GetDockStatus(Guid dockID);

        public Dictionary<Guid,String>  GetStatusAllDocks();

        public Dictionary<Ship, Status> GetStatusAllShips();

    }
