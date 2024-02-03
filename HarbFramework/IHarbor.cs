using System;
using HarbFramework;
using harbNet;

namespace harbNet
{
    public interface IHarbor
    {
        public string GetShipStatus(Guid ShipID);
        public string GetStatusAllDocks();
        public string GetStatusAllShips();

        // public Status GetShipStatus(Guid ShipID);

        public string GetDockStatus(Guid dockID);

        public Dictionary<Guid, bool> StatusAllDocks();

        //public Dictionary<Ship, Status> GetStatusAllShips();


        // OBS OBS !! måtte kommentere ut
        // De kan ikke ha samme navn - hvilken Datatype er riktig? String eller de andre?

    }
}