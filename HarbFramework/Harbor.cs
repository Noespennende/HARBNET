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
    public class Harbor : IHarbor
    {
        internal ArrayList allLoadingDocks = new ArrayList();
        internal ArrayList freeLoadingDocks = new ArrayList();
        internal IDictionary<Ship, Dock> shipsInLoadingDock = new Dictionary<Ship, Dock>(); // Ship : Dock

        internal ArrayList allShipDocks = new ArrayList();
        internal ArrayList freeShipDocks = new ArrayList();
        internal IDictionary<Ship,Dock> shipsInShipDock = new Dictionary<Ship, Dock>(); // Ship : Dock

        internal ArrayList anchorage = new ArrayList();
        internal Hashtable shipsInTransit = new Hashtable(); // ship: int number of days until return
        internal ArrayList AllShips { get; set; } = new ArrayList(); // Sikkert midlertidig, til vi kan regne på det
        internal IDictionary<ContainerSize, List<ContainerSpace>> allContainerSpaces = new Dictionary<ContainerSize, List<ContainerSpace>>();
        //internal Hashtable allContainerSpaces = new Hashtable(); // størelse : antall
        internal IDictionary<ContainerSize, List<ContainerSpace>> freeContainerSpaces = new Dictionary<ContainerSize, List<ContainerSpace>>();
        //internal Hashtable freeContainerSpaces = new Hashtable(); // størelse : antall ledige
        internal IDictionary<Container, ContainerSpace> storedContainers = new Dictionary<Container, ContainerSpace>(); // Container : ContainerSpace
        internal Guid transitLocationID = Guid.NewGuid();
        internal Guid anchorageID = Guid.NewGuid();


        public Harbor(ICollection<Ship> listOfShips, int numberOfSmallLoadingDocks, int numberOfMediumLoadingDocks, int numberOfLargeLoadingDocks,
            int numberOfSmallShipDocks, int numberOfMediumShipDocks, int numberOfLargeShipDocks,
            int numberOfSmallContainerSpaces, int numberOfMediumContainerSpaces, int numberOfLargeContainerSpaces)
        {
            for (int i = 0; i < numberOfSmallLoadingDocks; i++)
            {
                allLoadingDocks.Add(new Dock(ShipSize.Small));
            }

            for (int i = 0; i < numberOfMediumLoadingDocks; i++)
            {
                allLoadingDocks.Add(new Dock(ShipSize.Medium));
            }
            for (int i = 0; i < numberOfLargeLoadingDocks; i++)
            {
                allLoadingDocks.Add(new Dock(ShipSize.Large));
            }

            for (int i = 0; i < numberOfSmallShipDocks; i++)
            {
                allShipDocks.Add(new Dock(ShipSize.Small));
            }

            for (int i = 0; i < numberOfMediumShipDocks; i++)
            {
                allShipDocks.Add(new Dock(ShipSize.Medium));
            }
            for (int i = 0; i < numberOfLargeShipDocks; i++)
            {
                allShipDocks.Add(new Dock(ShipSize.Large));
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

                allContainerSpaces[containerSize] = spaces; // Gir allContainerSpaces de gitte opprettede spacene
                freeContainerSpaces[containerSize] = spaces; // Gir freeContainerSpaces de gitte opperettede spaces
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
            anchorage.AddRange((ICollection)listOfShips);


            foreach (Ship ship in anchorage)
            {
                ship.CurrentLocation = anchorageID;
            }

            freeLoadingDocks = (ArrayList)allLoadingDocks.Clone();
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
                
                shipsInLoadingDock.Add(shipToBeDocked, dock);

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

                shipsInShipDock.Add(shipToBeDocked, dock);
                shipsInLoadingDock.Remove(shipToBeDocked);
                freeLoadingDocks.Add(loadingDock);
                freeShipDocks.Remove(dock);

                return dock.GetID();
            }

            return Guid.Empty;
        }

        internal Guid UnDockShipFromLoadingDockToTransit(Guid shipID, DateTime currentTime)
        {
            Ship shipToBeUndocked = GetShipFromLoadingDock(shipID);

            if (shipToBeUndocked != null)
            {
                Dock dock = (Dock)shipsInLoadingDock[shipToBeUndocked];
                //need to add history event for ship
                dock.DockedShip = Guid.Empty;
                dock.Free = true;

                shipToBeUndocked.CurrentLocation = transitLocationID;
        

                shipsInLoadingDock.Remove(shipToBeUndocked);
                freeLoadingDocks.Add(dock);
                if (!shipsInTransit.ContainsKey(shipToBeUndocked))
                {
                    shipsInTransit.Add(shipToBeUndocked, shipToBeUndocked.RoundTripInDays);
                }
                return dock.GetID();
            }

            return Guid.Empty;
        } //returnerer Guid til docken skipet docket fra

        internal Ship GetShipFromAnchorage(Guid shipID)
        {
            foreach (Ship ship in anchorage)
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

            foreach (Ship ship in shipsInLoadingDock.Keys)
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
            foreach (var item in shipsInLoadingDock)
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

            foreach (var item in shipsInLoadingDock)
            {
                ships.Add(item.Key); // Legg til hvert individuelle Ship-objekt, ikke hele Hashtable
            }

            return ships;
        }
        internal bool FreeLoadingDockExists(ShipSize shipSize)
        {
            foreach (Dock dock in freeLoadingDocks)
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
            foreach (Dock dock in freeShipDocks)
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

            foreach (Dock dock in freeLoadingDocks)
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

            foreach (Dock dock in freeShipDocks)
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
            foreach (Ship ship in anchorage)
            {
                if (ship.GetID() == shipID)
                {
                    anchorage.Remove(ship);
                    return true;
                }
            }
            return false;
        }
        internal bool RemoveLoadingDockFromFreeLoadingDocks(Guid dockID)
        {
            foreach (Dock dock in freeLoadingDocks)
            {
                if (dock.GetID() == dockID)
                {
                    anchorage.Remove(dock);
                    return true;
                }
            }
            return false;
        }
        internal int NumberOfFreeLoadingDocks(ShipSize shipSize)
        {
            int count = 0;
            foreach (Dock dock in freeLoadingDocks)
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
            foreach (ContainerSpace containerSpace in freeContainerSpaces[containerSize])
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
            return storedContainers.Count;

        } //returnerer antallet okuperte plasser av den gitte typen

        internal ContainerSpace GetFreeContainerSpace(ContainerSize containerSize)
        {
            foreach (ContainerSpace containerSpace in freeContainerSpaces[containerSize]) // Sto originalt storedContainers, går ut ifra det skulle stå freeContainerSpaces
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
            foreach (Container container in storedContainers.Keys)
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
            foreach (KeyValuePair<ContainerSize, List<ContainerSpace>> pair in freeContainerSpaces)
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

            freeContainerSpaces[containerSize].Remove(containerSpace);

            storedContainers.Add(containerToBeUnloaded, containerSpace);

            containerSpace.storedContainer = containerToBeUnloaded.ID;
            containerSpace.Free = false;

            containerToBeUnloaded.CurrentPosition = containerSpace.ID;
            containerToBeUnloaded.AddHistoryEvent(Status.InStorage, currentTime);

            return containerSpace.ID;

        } // returnerer Guid til container spaces containeren ble lagret, returnerer empty Guid hvis containeren ikke finnes

        internal Guid LoadContainer(ContainerSize containerSize, Ship ship, DateTime currentTime)
        {
            Container containerToBeLoaded = GetStoredContainer(containerSize);

            if (containerToBeLoaded == null || !storedContainers.ContainsKey(containerToBeLoaded))
            {

                return Guid.Empty;

            }

            ContainerSpace containerSpace = storedContainers[containerToBeLoaded];

            ship.AddContainer(containerToBeLoaded);
            containerToBeLoaded.CurrentPosition = ship.ID;

            containerToBeLoaded.AddHistoryEvent(Status.Transit, currentTime);

            containerSpace.Free = true;
            containerSpace.storedContainer = Guid.Empty;

            freeContainerSpaces[containerSize].Add(containerSpace);
            storedContainers.Remove(containerToBeLoaded);

            return ship.GetID();

        }

        internal void AddNewShipToHarbourQueue(Ship ship)
        {
            anchorage.Add(ship);
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
            foreach(Dock dock in allShipDocks)
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
            foreach (Dock dock in allLoadingDocks)
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

            foreach (Dock dock in allLoadingDocks)
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
            foreach (Container container in storedContainers.Keys)
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
            foreach (Container container in storedContainers.Keys)
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

        public Guid GetAnchorageID()
        {
            throw new NotImplementedException();
        }

        public Guid GetTransitID()
        {
            throw new NotImplementedException();
        }
    }
}
       

               
