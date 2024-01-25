using HarbFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace harbNet
{
    internal class Harbor
    {
        internal ArrayList allDocks = new ArrayList();
        internal ArrayList freeDocks = new ArrayList();
        internal Hashtable shipsInDock = new Hashtable(); // Ship : Dock
        internal ArrayList harbourQueInn = new ArrayList();  
        internal Hashtable shipsInTransit = new Hashtable(); // ship: int number of days until return
        internal ArrayList allContainerSpaces = new ArrayList(); // størelse : antall
        internal ArrayList freeContainerSpaces = new ArrayList(); // størelse : antall ledige
        internal Hashtable storedContainers = new Hashtable(); // Container : ContainerSpace
        Guid transitLocationID = Guid.NewGuid();
        Guid portQueInnID = Guid.NewGuid();


        internal Harbor (int numberOfSmallDocks, int numberOfMediumDocks, int numberOfLargeDocks, int numberOfSmallContainerSpaces, int numberOfMediumContainerSpaces, int numberOfLargeContainerSpaces)
        {
            for (int i = 0; i < numberOfSmallDocks; i++)
            {
                allDocks.Add(new Dock(ShipSize.Small));
            }

            for(int i = 0;i < numberOfMediumDocks; i++)
            {
                allDocks.Add(new Dock(ShipSize.Medium));
            }
            for (int i = 0; i < numberOfLargeDocks; i++)
            {
                allDocks.Add(new Dock(ShipSize.Large));
            }

            for (int i = 0; i < numberOfSmallContainerSpaces; i++)
            {
                allContainerSpaces.Add(new ContainerSpace(ContainerSize.Small));
            }

            for (int i = 0; i < numberOfMediumContainerSpaces; i++)
            {
                allContainerSpaces.Add(new ContainerSpace(ContainerSize.Medium));
            }

            for (int i = 0; i < numberOfLargeContainerSpaces; i++)
            {
                allContainerSpaces.Add(new ContainerSpace(ContainerSize.Large));
            }

            freeDocks = (ArrayList)allDocks.Clone();
            freeContainerSpaces = (ArrayList)allContainerSpaces.Clone();
        }

        private Guid dockShip(Guid shipID, DateTime currentTime) //omskriv til å sende inn størrelse. 
        {
            Ship shipToBeDocked = getShipFromQueue(shipID);
            ShipSize size = shipToBeDocked.shipSize;
            Dock dock;

            if (freeDockExists(size))
            {
                dock = getFreeDock(size);
                dock.dockedShip = shipToBeDocked.getID();
                dock.free = false;

                shipToBeDocked.currentLocation = dock.getID();
                shipToBeDocked.addHistoryEvent(currentTime, dock.id, Status.Docking);
                shipsInDock.Add(shipToBeDocked, dock);
                
                removeShipFromQueue(shipToBeDocked.getID());
                removeDockFromFreeDocks(dock.getID());

                return dock.getID();
            }

            return Guid.Empty; //returnerer en Guid med verdi "00000000-0000-0000-0000-000000000000" hvis han ikke finner noen ledige docker.
        }//returnerer Guid til docken skipet docker til

        private Guid unDockShip (Guid shipID, DateTime currentTime) {
            Ship shipToBeUndocked = getShipFromDock(shipID);

            if (shipToBeUndocked != null)
            {
                Dock dock = (Dock) shipsInDock[shipToBeUndocked];
                //need to add history event for ship
                dock.dockedShip = Guid.Empty;
                dock.free = true;

                shipToBeUndocked.currentLocation = transitLocationID;
                shipToBeUndocked.addHistoryEvent(currentTime, Guid.Empty, Status.Transit);

                shipsInDock.Remove(shipToBeUndocked);
                freeDocks.Add(dock);
                shipsInTransit.Add(shipToBeUndocked, shipToBeUndocked.roundTripInDays);

                return dock.getID();
            }

            return Guid.Empty;
        } //returnerer Guid til docken skipet docket fra

        private Ship getShipFromQueue(Guid shipID)
        {
            foreach (Ship ship in harbourQueInn)
            {
                if (ship.getID().Equals(shipID))
                {
                    return ship;
                }
            }

            return null;
        }

        private Ship getShipFromDock(Guid shipID)
        {
            foreach (Ship ship in shipsInDock.Keys)
            {
                if (ship.getID() == shipID)
                {
                    return ship;
                }
            }
            return null;
        }
        internal ArrayList dockedShips()
        {
            ArrayList ships = new ArrayList();
            ships.Add(shipsInDock);
            return ships;
        }
        internal bool freeDockExists (ShipSize shipSize)
        {
            foreach (Dock dock in freeDocks)
            {
                if (dock.free == true && dock.size == shipSize)
                {
                    return true;
                }
            }
            return false;
        }
        internal Dock getFreeDock (ShipSize shipSize)
        {
            
            foreach (Dock dock in freeDocks)
            {
                if (dock.free == true && dock.size == shipSize) {
                    return dock;
                }
            }

            return null;
        }
        internal bool removeShipFromQueue (Guid shipID)
        {
            foreach (Ship ship in harbourQueInn)
            {
                if (ship.getID() == shipID)
                {
                    harbourQueInn.Remove(ship);
                    return true;
                }
            }
            return false;
        }
        internal bool removeDockFromFreeDocks (Guid dockID)
        {
            foreach (Dock dock in freeDocks)
            {
                if (dock.getID() == dockID)
                {
                    harbourQueInn.Remove(dock);
                    return true;
                }
            }
            return false;
        }
        internal int NumberOfFreeDocks(ShipSize shipSize) {
            int count = 0;
            foreach (Dock dock in freeDocks)
            {
                if (dock.free == true && dock.size == shipSize)
                {
                    count++;
                }
            }
            return count;
        } //Returnerer antall ledige plasser av den gitte typen
        internal int NumberOfFreeContainerSpaces(ContainerSize containerSize) {
            int count = 0;
            foreach (ContainerSpace containerSpace in freeContainerSpaces)
            {
                if (containerSpace.size == containerSize && containerSpace.free == true)
                {
                    count++;
                }
            }
            return count;

        } //returnerer antallet ledige container plasser av den gitte typen.
        internal int getNumberOfOccupiedContainerSpaces(ContainerSize containerSize) {
            return storedContainers.Count;
        } //returnerer antallet okuperte plasser av den gitte typen
        internal ContainerSpace getFreeContainerSpace(ContainerSize containerSize) {
            foreach (ContainerSpace containerSpace in storedContainers)
            {
                if (containerSpace.free == true && containerSpace.size == containerSize)
                {
                    return containerSpace;
                }
            }
            return null;
        } //returnerer en Guid til en ledig plass av den gitte typen
    
        internal Container getStoredContainer (ContainerSize containerSize)
        {
            foreach (Container container in storedContainers.Keys)
            {
                if (container.size == containerSize)
                {
                    return container;
                }
            }
            return null;
        }
        internal bool removeContainerSpaceFromFreeContainerSpaces(Guid containerSpaceID)
        {
            foreach (ContainerSpace containerSpace in freeContainerSpaces)
            {
                if (containerSpace.id == containerSpaceID)
                {
                    freeContainerSpaces.Remove(containerSpace);
                    return true;
                }
            }
            return false;
        }
        internal Guid unloadContainer (ContainerSize containerSize, Ship ship, DateTime currentTime)
        {
            Container containerToBeUnloaded = ship.getContainer(containerSize);
            ContainerSpace containerSpace = getFreeContainerSpace (containerSize);

            if (containerToBeUnloaded == null || containerSpace == null)
            {
                return Guid.Empty;
            }

            ship.removeContainer(containerToBeUnloaded.id);
            freeContainerSpaces.Remove(containerSpace);
            storedContainers.Add(containerToBeUnloaded, containerSpace);

            containerSpace.storedContainer = containerToBeUnloaded.id;
            containerSpace.free = false;

            containerToBeUnloaded.currentPoison = containerSpace.id;
            containerToBeUnloaded.addHistoryEvent(Status.InStorage, currentTime);

            return containerSpace.id;

        } // returnerer Guid til container spaces containeren ble lagret, returnerer empty Guid hvis containeren ikke finnes

        internal Guid loadContainer (ContainerSize containerSize, Ship ship, DateTime currentTime)
        {
            Container containerToBeLoaded = getStoredContainer(containerSize);
            ContainerSpace containerSpace = (ContainerSpace)storedContainers[containerToBeLoaded];

            if (containerToBeLoaded == null || containerSpace == null)
            {
                return Guid.Empty;
            }

            ship.addContainer(containerToBeLoaded);
            containerToBeLoaded.currentPoison = ship.id;
            containerToBeLoaded.addHistoryEvent (Status.Transit, currentTime);

            containerSpace.free = true;
            containerSpace.storedContainer = Guid.Empty;

            freeContainerSpaces.Add(containerSpace);
            storedContainers.Remove(containerToBeLoaded);

            return ship.id;
        }
    }
}
