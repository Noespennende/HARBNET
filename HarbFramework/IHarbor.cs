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

        Status GetShipStatus(Guid ShipID);

        string GetDockStatus(Guid dockID);

        Dictionary<Guid,String>  GetStatusAllDocks();

        Dictionary<Ship, Status> GetStatusAllShips();

    }
