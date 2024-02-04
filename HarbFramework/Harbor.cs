﻿using HarbFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace harbNet
{
    public class Harbor : IHarbor
    {
        internal ArrayList AllLoadingDocks = new ArrayList();
        internal ArrayList FreeLoadingDocks = new ArrayList();
        internal Dictionary<Ship, Dock> ShipsInLoadingDock = new Dictionary<Ship, Dock>(); // Ship : Dock

        internal ArrayList AllShipDocks = new ArrayList();
        internal ArrayList FreeShipDocks = new ArrayList();
        internal Dictionary<Ship,Dock> ShipsInShipDock = new Dictionary<Ship, Dock>(); // Ship : Dock

        internal ArrayList Anchorage = new ArrayList();
        internal Hashtable ShipsInTransit = new Hashtable(); // ship: int number of days until return
        internal ArrayList AllShips { get; set; } = new ArrayList(); // Sikkert midlertidig, til vi kan regne på det
        internal Dictionary<ContainerSize, List<ContainerSpace>> AllContainerSpaces = new();
        //internal Hashtable allContainerSpaces = new Hashtable(); // størelse : antall
        internal Dictionary<ContainerSize, List<ContainerSpace>> FreeContainerSpaces = new();
        //internal Hashtable freeContainerSpaces = new Hashtable(); // størelse : antall ledige
        internal Dictionary<Container, ContainerSpace> StoredContainers = new(); // Container : ContainerSpace
        internal Guid TransitLocationID = Guid.NewGuid();
        internal Guid AnchorageID = Guid.NewGuid();


        public Harbor(ICollection<Ship> listOfShips, int numberOfSmallLoadingDocks, int numberOfMediumLoadingDocks, int numberOfLargeLoadingDocks,
            int numberOfSmallShipDocks, int numberOfMediumShipDocks, int numberOfLargeShipDocks,
            int numberOfSmallContainerSpaces, int numberOfMediumContainerSpaces, int numberOfLargeContainerSpaces)
        {
            for (int i = 0; i < numberOfSmallLoadingDocks; i++)
            {
                AllLoadingDocks.Add(new Dock(ShipSize.Small));
            }

            for (int i = 0; i < numberOfMediumLoadingDocks; i++)
            {
                AllLoadingDocks.Add(new Dock(ShipSize.Medium));
            }
            for (int i = 0; i < numberOfLargeLoadingDocks; i++)
            {
                AllLoadingDocks.Add(new Dock(ShipSize.Large));
            }

            for (int i = 0; i < numberOfSmallShipDocks; i++)
            {
                AllShipDocks.Add(new Dock(ShipSize.Small));
            }

            for (int i = 0; i < numberOfMediumShipDocks; i++)
            {
                AllShipDocks.Add(new Dock(ShipSize.Medium));
            }
            for (int i = 0; i < numberOfLargeShipDocks; i++)
            {
                AllShipDocks.Add(new Dock(ShipSize.Large));
            }

            CreateContainerSpaces(ContainerSize.Small, numberOfSmallContainerSpaces);
            CreateContainerSpaces(ContainerSize.Medium, numberOfMediumContainerSpaces);
            CreateContainerSpaces(ContainerSize.Large, numberOfLargeContainerSpaces);

            void CreateContainerSpaces(ContainerSize containerSize, int numberOfSpaces)
            {
                List<ContainerSpace> spaces = new List<ContainerSpace>();
                for (int j = 0; j < numberOfSpaces; j++)
                {
                    spaces.Add(new ContainerSpace(containerSize));
                }

                AllContainerSpaces[containerSize] = spaces; // Gir allContainerSpaces de gitte opprettede spacene
                FreeContainerSpaces[containerSize] = spaces; // Gir freeContainerSpaces de gitte opperettede spaces
                                                             // (Siden de er alle tomme ved oppstart av harbor og heller fylles opp senere med andre metodekall)
            }
            /* Koden fra da allContainerSpaces var HashTable : 
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
            */


            AllShips.AddRange((ICollection)listOfShips);
            Anchorage.AddRange((ICollection)listOfShips);


            foreach (Ship ship in Anchorage)
            {
                ship.CurrentLocation = AnchorageID;
            }

            FreeLoadingDocks = (ArrayList)AllLoadingDocks.Clone();
        }

        internal Guid DockShipToLoadingDock(Guid shipID, DateTime currentTime) //omskriv til å sende inn størrelse. 
        {
            Ship shipToBeDocked = GetShipFromAnchorage(shipID);
            ShipSize size = shipToBeDocked.ShipSize;
            Dock dock;

            if (FreeLoadingDockExists(size))
            {
                dock = GetFreeLoadingDock(size);
                dock.DockedShip = shipToBeDocked.GetID();
                dock.Free = false;

                shipToBeDocked.CurrentLocation = dock.GetID();
                shipToBeDocked.AddHistoryEvent(currentTime, dock.ID, Status.DockingToLoadingDock);
                ShipsInLoadingDock.Add(shipToBeDocked, dock);

                RemoveShipFromAnchorage(shipToBeDocked.GetID());
                RemoveLoadingDockFromFreeLoadingDocks(dock.GetID());

                return dock.GetID();
            }

            return Guid.Empty; //returnerer en Guid med verdi "00000000-0000-0000-0000-000000000000" hvis han ikke finner noen ledige docker.
        }//returnerer Guid til docken skipet docker til

        internal Guid DockShipToShipDock(Guid shipID, DateTime currentTime)
        {
            Ship shipToBeDocked = GetShipFromLoadingDock(shipID);
            Dock loadingDock = GetLoadingDockContainingShip(shipID);
            ShipSize size = shipToBeDocked.ShipSize;
            Dock dock;

            if (FreeLoadingDockExists(size))
            {
                dock = GetFreeShipDock(size);
                dock.DockedShip = shipToBeDocked.GetID();
                dock.Free = false;

                shipToBeDocked.CurrentLocation = dock.GetID();
                shipToBeDocked.AddHistoryEvent(currentTime, dock.ID, Status.DockingToShipDock);

                ShipsInShipDock.Add(shipToBeDocked, dock);
                ShipsInLoadingDock.Remove(shipToBeDocked);
                FreeLoadingDocks.Add(loadingDock);
                FreeShipDocks.Remove(dock);

                return dock.GetID();
            }

            return Guid.Empty;
        }

        internal Guid UnDockShipFromLoadingDockToTransit(Guid shipID, DateTime currentTime)
        {
            Ship shipToBeUndocked = GetShipFromLoadingDock(shipID);

            if (shipToBeUndocked != null)
            {
                Dock dock = (Dock)ShipsInLoadingDock[shipToBeUndocked];
                //need to add history event for ship
                dock.DockedShip = Guid.Empty;
                dock.Free = true;

                shipToBeUndocked.CurrentLocation = TransitLocationID;
                shipToBeUndocked.AddHistoryEvent(currentTime, Guid.Empty, Status.Transit);

                ShipsInLoadingDock.Remove(shipToBeUndocked);
                FreeLoadingDocks.Add(dock);
                if (!ShipsInTransit.ContainsKey(shipToBeUndocked))
                {
                    ShipsInTransit.Add(shipToBeUndocked, shipToBeUndocked.RoundTripInDays);
                }
                return dock.GetID();
            }

            return Guid.Empty;
        } //returnerer Guid til docken skipet docket fra

        internal Ship GetShipFromAnchorage(Guid shipID)
        {
            foreach (Ship ship in Anchorage)
            {
                if (ship.GetID().Equals(shipID))
                {
                    return ship;
                }
            }

            return null;
        }

        internal Ship GetShipFromLoadingDock(Guid shipID)
        {

            foreach (Ship ship in ShipsInLoadingDock.Keys)
            {
                if (ship.GetID().Equals(shipID))
                {
                    return ship;
                }
            }
            return null;
        }
        internal Dock GetLoadingDockContainingShip(Guid shipID)
        {
            foreach (var item in ShipsInLoadingDock)
            {
                if (item.Key.GetID() == shipID)
                {
                    return item.Value;
                }
            }

            return null;
        }

        internal ArrayList DockedShipsInLoadingDock()
        {
            ArrayList ships = new ArrayList();

            foreach (var item in ShipsInLoadingDock)
            {
                ships.Add(item.Key); // Legg til hvert individuelle Ship-objekt, ikke hele Hashtable
            }

            return ships;
        }
        internal bool FreeLoadingDockExists(ShipSize shipSize)
        {
            foreach (Dock dock in FreeLoadingDocks)
            {
                if (dock.Free == true && dock.Size == shipSize)
                {
                    return true;
                }
            }
            return false;
        }

        internal bool FreeShipDockExists(ShipSize shipSize)
        {
            foreach (Dock dock in FreeShipDocks)
            {
                if (dock.Free == true && dock.Size == shipSize)
                {
                    return true;
                }
            }
            return false;
        }
        internal Dock GetFreeLoadingDock(ShipSize shipSize)
        {

            foreach (Dock dock in FreeLoadingDocks)
            {
                if (dock.Free == true && dock.Size == shipSize)
                {
                    return dock;
                }
            }

            return null;
        }

        internal Dock GetFreeShipDock(ShipSize shipSize)
        {

            foreach (Dock dock in FreeShipDocks)
            {
                if (dock.Free == true && dock.Size == shipSize)
                {
                    return dock;
                }
            }

            return null;
        }
        internal bool RemoveShipFromAnchorage(Guid shipID)
        {
            foreach (Ship ship in Anchorage)
            {
                if (ship.GetID() == shipID)
                {
                    Anchorage.Remove(ship);
                    return true;
                }
            }
            return false;
        }
        internal bool RemoveLoadingDockFromFreeLoadingDocks(Guid dockID)
        {
            foreach (Dock dock in FreeLoadingDocks)
            {
                if (dock.GetID() == dockID)
                {
                    Anchorage.Remove(dock);
                    return true;
                }
            }
            return false;
        }
        internal int NumberOfFreeLoadingDocks(ShipSize shipSize)
        {
            int count = 0;
            foreach (Dock dock in FreeLoadingDocks)
            {
                if (dock.Free == true && dock.Size == shipSize)
                {
                    count++;
                }
            }
            return count;
        } //Returnerer antall ledige plasser av den gitte typen
        internal int NumberOfFreeContainerSpaces(ContainerSize containerSize)
        {
            int count = 0;
            foreach (ContainerSpace containerSpace in FreeContainerSpaces[containerSize])
            {
                if (containerSpace.Size == containerSize && containerSpace.Free == true)
                {
                    count++;
                }
            }
            return count;


        } //returnerer antallet ledige container plasser av den gitte typen.
        internal int GetNumberOfOccupiedContainerSpaces(ContainerSize containerSize)
        {
            return StoredContainers.Count;

        } //returnerer antallet okuperte plasser av den gitte typen

        internal ContainerSpace GetFreeContainerSpace(ContainerSize containerSize)
        {
            foreach (ContainerSpace containerSpace in FreeContainerSpaces[containerSize]) // Sto originalt storedContainers, går ut ifra det skulle stå freeContainerSpaces
            {

                if (containerSpace.Free == true && containerSpace.Size == containerSize)
                {
                    return containerSpace;

                }
            }
            return null;

        } //returnerer en Guid til en ledig plass av den gitte typen

        internal Container GetStoredContainer(ContainerSize containerSize)
        {
            foreach (Container container in StoredContainers.Keys)
            {
                if (container.Size == containerSize)
                {
                    return container;
                }
            }
            return null;

        }

        internal bool RemoveContainerSpaceFromFreeContainerSpaces(Guid containerSpaceID)
        {
            foreach (KeyValuePair<ContainerSize, List<ContainerSpace>> pair in FreeContainerSpaces)
            {
                List<ContainerSpace> containerSpaces = pair.Value;
                for (int i = 0; i < containerSpaces.Count; i++)
                {
                    ContainerSpace containerSpace = containerSpaces[i];
                    if (containerSpace.ID == containerSpaceID)
                    {
                        containerSpaces.RemoveAt(i);
                        return true;
                    }
                }
            }
            return false;
        }

        internal Guid UnloadContainer(ContainerSize containerSize, Ship ship, DateTime currentTime)
        {
            Container containerToBeUnloaded = ship.GetContainer(containerSize);
            ContainerSpace containerSpace = GetFreeContainerSpace(containerSize);

            if (containerToBeUnloaded == null || containerSpace == null)
            {

                return Guid.Empty;

            }

            ship.RemoveContainer(containerToBeUnloaded.ID);

            FreeContainerSpaces[containerSize].Remove(containerSpace);

            StoredContainers.Add(containerToBeUnloaded, containerSpace);

            containerSpace.StoredContainer = containerToBeUnloaded.ID;
            containerSpace.Free = false;

            containerToBeUnloaded.CurrentPosition = containerSpace.ID;
            containerToBeUnloaded.AddHistoryEvent(Status.InStorage, currentTime);

            return containerSpace.ID;

        } // returnerer Guid til container spaces containeren ble lagret, returnerer empty Guid hvis containeren ikke finnes

        internal Guid LoadContainer(ContainerSize containerSize, Ship ship, DateTime currentTime)
        {
            Container containerToBeLoaded = GetStoredContainer(containerSize);

            if (containerToBeLoaded == null || !StoredContainers.ContainsKey(containerToBeLoaded))
            {

                return Guid.Empty;

            }

            ContainerSpace containerSpace = StoredContainers[containerToBeLoaded];

            ship.AddContainer(containerToBeLoaded);
            containerToBeLoaded.CurrentPosition = ship.ID;

            containerToBeLoaded.AddHistoryEvent(Status.Transit, currentTime);

            containerSpace.Free = true;
            containerSpace.StoredContainer = Guid.Empty;

            FreeContainerSpaces[containerSize].Add(containerSpace);
            StoredContainers.Remove(containerToBeLoaded);

            return ship.GetID();

        }

        internal void AddNewShipToHarbourQueue(Ship ship)
        {
            Anchorage.Add(ship);
        }

        /* ** Interface implementasjon som må gjøres ** */

        // Obs obs - sjekk kommentert ut metode i Interface (Fra nylig push av Andreas)
        // De måtte kommenteres ut, kan ikke ha samme navn. Vet ikke hvilke som er riktige
        public Status GetShipStatus(Guid ShipID)
        {
            Event lastEvent = null;
            StringBuilder sb = new StringBuilder();
            foreach (Ship ship in AllShips)
            {
                if (ship.ID == ShipID && ship.History != null && ship.History.Count > 0)
                {
                    // Måtte kommentere ut for å kjøre fordi ship.History[ship.History.Count - 1] gir error
                    // lastEvent = ship.History[ship.History.Count - 1] as Event;
                    String shipStatus = $"ShipId: {ship.ID}, Last event: {lastEvent}";
                    sb.Append(shipStatus);
                }

            }
            return Status.None;
        }



        // Obs obs - sjekk kommentert ut metode i Interface (Fra nylig push av Andreas)
        // De måtte kommenteres ut, kan ikke ha samme navn. Vet ikke hvilke som er riktige

        //må kjøre denne for å se om den funker som tenkt
        public Dictionary<Ship, Status> GetStatusAllShips()
        {
            Dictionary<Ship, Status> shipStatus = new Dictionary<Ship, Status>();
            
            foreach (Ship ship in AllShips)
            {
                Event test = ship.History.Last();
                if (test != null)
                {
                    shipStatus[ship] = test.Status;
                }
            }
            return shipStatus;
        }

        public Dictionary<Guid, bool> StatusAllDocks()
        {
            Dictionary<Guid, bool> dockStatus = new Dictionary<Guid, bool>();
            foreach(Dock dock in AllShipDocks)
            {
                dockStatus[dock.ID] = dock.Free;
            }
            return dockStatus;
        }

        //Denne kan potensielt endres
        //må endre på toString til en representasjon som fungerer
        public string GetLoadingDockStatus(Guid dockID)
        {
            StringBuilder sb = new StringBuilder();
            bool dockFree = false;
            foreach (Dock dock in AllLoadingDocks)
            {
                if (dockID == dock.ID)
                {
                    dockFree = dock.Free;
                    String dockStatus = $"DockId: {dock.ID}, dock free: {dockFree}";
                    sb.Append(dockStatus);
                }
            }
            return sb.ToString();

        }

        // Obs obs - sjekk kommentert ut metode i Interface (Fra nylig push av Andreas)
        // De måtte kommenteres ut, kan ikke ha samme navn. Vet ikke hvilke som er riktige

        //må endre på toString til en representasjon som fungerer
        public string GetStatusAllLoadingDocks()
        {
            StringBuilder sb = new StringBuilder();
            Dictionary<Dock, bool> dockStatus = new Dictionary<Dock, bool>();

            foreach (Dock dock in AllLoadingDocks)
            {
                dockStatus[dock] = dock.Free;

            }
            foreach (var keyValue in dockStatus)
            {
                sb.AppendLine($"dockId: {keyValue.Key}, dock free: {keyValue.Value}");
            }
            return dockStatus.ToString();
        }

        //må endre på toString til en representasjon som fungerer
        public string GetContainerStatus(Guid ContainerId)
        {
            StringBuilder sb = new StringBuilder();
            Dictionary<Container, Status> containerStatus = new Dictionary<Container, Status>();
            foreach (Container container in StoredContainers.Keys)
            {
                if (container.ID == ContainerId)
                {
                    containerStatus[container] = container.History.Last().Status;

                    foreach (var keyvalue in containerStatus)
                    {
                        sb.Append($"ContainerId: {keyvalue.Key}, containerStatus: {keyvalue.Value}");
                    }
                }
            }
            return sb.ToString();
        }



        //må endre på toString til en representasjon som fungerer
        public string GetAllContainerStatus()
        {
            StringBuilder sb = new StringBuilder();
            Dictionary<Container, Status> containerStatus = new Dictionary<Container, Status>();
            Status lastEventStatus = Status.None;
            foreach (Container container in StoredContainers.Keys)
            {
                if (container != null && container.History != null && container.History.Count > 0)
                {
                    lastEventStatus = container.History.Last().Status;
                    containerStatus[container] = lastEventStatus;

                    foreach (var keyvalue in containerStatus)
                    {
                        sb.AppendLine($"ContainerId: {keyvalue.Key}, containerStatus: {keyvalue.Value}");
                    }
                }
            }
            return sb.ToString();
        }




    }
}
       

               
