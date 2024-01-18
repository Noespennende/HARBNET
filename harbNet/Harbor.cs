using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace harbNet
{
    internal class Harbor
    {
        Hashtable maxDocks = new Hashtable();
        // størelse : antall
        Hashtable freeDocks = new Hashtable();
        // størelse : antall ledige
        Hashtable dockedShips = new Hashtable();
        // uid Ship : Dock størelse

        Queue portQueueInn = new Queue();
        // ship ID
        Queue portQueueOut = new Queue();
        // ship ID

        Hashtable maxContainerSpace = new Hashtable();
        // størelse : antall
        Hashtable freeContainerSpace = new Hashtable();
        // størelse : antall ledige
        Hashtable storedContainers = new Hashtable();
        // uid Container : Container space størelse

        //Enum weather

        private void dockShip (Guid shipId) { }
        private void unDockShip (Guid shipId) { }
        private void NumberFreeContainerSpaces() { }
        private void FreeContainerSpaces() { }
        private void OccupiedContainerSpaces() { }
        private void FreeDocingSpaces() { }
        private void shipsInHarbour() { }
        private void currentWeather() { }
    }
}
