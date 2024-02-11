using HarbFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace harbNet
{
    public class Harbor : IHarbor
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        internal IList<Dock> allLoadingDocks = new List<Dock>();
        internal IList<Dock> freeLoadingDocks = new List<Dock>();
        internal IDictionary<Ship, Dock> shipsInLoadingDock = new Dictionary<Ship, Dock>(); // Ship : Dock

        internal IList<Dock> allShipDocks = new List<Dock>();
        internal IList<Dock> freeShipDocks = new List<Dock>();
        internal IDictionary<Ship,Dock> shipsInShipDock = new Dictionary<Ship, Dock>(); // Ship : Dock

        internal IList<Ship> Anchorage { get; set; } = new List<Ship>();
        internal IDictionary<Ship, int> ShipsInTransit { get; set; } = new Dictionary<Ship, int>(); // ship: int number of days until return
        internal IList<Ship> AllShips { get; set; } = new List<Ship>(); // Sikkert midlertidig, til vi kan regne på det
        internal IDictionary<ContainerSize, List<ContainerSpace>> allContainerSpaces = new Dictionary<ContainerSize, List<ContainerSpace>>();

        internal IDictionary<ContainerSize, List<ContainerSpace>> freeContainerSpaces = new Dictionary<ContainerSize, List<ContainerSpace>>();

        internal IDictionary<Container, ContainerSpace> storedContainers = new Dictionary<Container, ContainerSpace>(); // Container : ContainerSpace

        public Guid TransitLocationID { get; } = Guid.NewGuid();
        public Guid AnchorageID { get; } = Guid.NewGuid();


        public Harbor(IList<Ship> listOfShips, int numberOfSmallLoadingDocks, int numberOfMediumLoadingDocks, int numberOfLargeLoadingDocks,
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

            AllShips = listOfShips.ToList();
            Anchorage = listOfShips.ToList();


            foreach (Ship ship in Anchorage)
            {
                ship.CurrentLocation = AnchorageID;
            }

            freeShipDocks = allShipDocks.ToList();
            freeLoadingDocks = allLoadingDocks.ToList();
        }

        internal Guid DockShipToLoadingDock(Guid shipID, DateTime currentTime) //omskriv til å sende inn størrelse. 
        {
            Ship shipToBeDocked = GetShipFromAnchorage(shipID);
            
            ShipSize size = shipToBeDocked.ShipSize;
            Dock dock;

            if (FreeLoadingDockExists(size))
            {
                dock = GetFreeLoadingDock(size);

                dock.DockedShip = shipToBeDocked.ID;
                dock.Free = false;

                shipToBeDocked.CurrentLocation = dock.ID;

                shipsInLoadingDock.Add(shipToBeDocked, dock);

                RemoveShipFromAnchorage(shipToBeDocked.ID);
                RemoveLoadingDockFromFreeLoadingDocks(dock.ID);

                return dock.ID;
            }

            return Guid.Empty; //returnerer en Guid med verdi "00000000-0000-0000-0000-000000000000" hvis han ikke finner noen ledige docker.
        }//returnerer Guid til docken skipet docker til

        internal Guid DockShipFromShipDockToLoadingDock(Guid shipID, DateTime currentTime) //omskriv til å sende inn størrelse. 
        {
            Ship shipToBeDocked = GetShipFromShipDock(shipID);

            ShipSize size = shipToBeDocked.ShipSize;
            Dock dock;

            if (FreeLoadingDockExists(size))
            {
                dock = GetFreeLoadingDock(size);

                dock.DockedShip = shipToBeDocked.ID;
                dock.Free = false;

                shipToBeDocked.CurrentLocation = dock.ID;

                shipsInLoadingDock.Add(shipToBeDocked, dock);

                UnDockShipFromShipDockToLoadingDock(shipID);

                RemoveLoadingDockFromFreeLoadingDocks(dock.ID);
                
                return dock.ID;
            }

            return Guid.Empty; //returnerer en Guid med verdi "00000000-0000-0000-0000-000000000000" hvis han ikke finner noen ledige docker.
        }//returnerer Guid til docken skipet docker til


        internal Guid DockShipToShipDock(Guid shipID)
        {
            Ship shipToBeDocked = GetShipFromLoadingDock(shipID);
            Dock loadingDock = GetLoadingDockContainingShip(shipID);
            ShipSize size = shipToBeDocked.ShipSize;
            Dock dock;

            if (FreeShipDockExists(size))
            {
                
                dock = GetFreeShipDock(size);
                dock.DockedShip = shipToBeDocked.ID;
                dock.Free = false;

                shipToBeDocked.CurrentLocation = dock.ID;
                
                shipsInShipDock.Add(shipToBeDocked, dock);
                shipsInLoadingDock.Remove(shipToBeDocked);
                freeLoadingDocks.Add(loadingDock);
                freeShipDocks.Remove(dock);

                return dock.ID;
                
            }

            return Guid.Empty;
        }

        internal Guid StartShipInShipDock(Guid shipID)
        {
            Ship shipToBeDocked = GetShipFromAnchorage(shipID);
            ShipSize size = shipToBeDocked.ShipSize;
            Dock dock;

            if (FreeShipDockExists(size))
            {
                dock = GetFreeShipDock(size);
                dock.DockedShip = shipToBeDocked.ID;
                dock.Free = false;
                shipToBeDocked.CurrentLocation = dock.ID;

                shipsInShipDock.Add(shipToBeDocked, dock);
                freeShipDocks.Remove(dock);

                return dock.ID;
                
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

                shipToBeUndocked.CurrentLocation = TransitLocationID;
        

                shipsInLoadingDock.Remove(shipToBeUndocked);
                freeLoadingDocks.Add(dock);
                if (!ShipsInTransit.ContainsKey(shipToBeUndocked))
                {
                    ShipsInTransit.Add(shipToBeUndocked, shipToBeUndocked.RoundTripInDays);
                }
                return dock.ID;
            }

            return Guid.Empty;
        } //returnerer Guid til docken skipet docket fra

        internal Ship GetShipFromAnchorage(Guid shipID)
        {
            foreach (Ship ship in Anchorage)
            {
                if (ship.ID.Equals(shipID))
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
                if (ship.ID.Equals(shipID))
                {
                    return ship;
                }
            }
            return null;
        }

        internal Ship GetShipFromShipDock(Guid shipID)
        {

            foreach (Ship ship in shipsInShipDock.Keys)
            {
                if (ship.ID.Equals(shipID))
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
                if (item.Key.ID == shipID)
                {
                    return item.Value;
                }
            }

            return null;
        }

        internal List<Ship> DockedShipsInLoadingDock()
        {
            List<Ship> ships = new List<Ship>();

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
            foreach (Ship ship in Anchorage)
            {
                if (ship.ID == shipID)
                {
                    Anchorage.Remove(ship);
                    return true;
                }
            }
            return false;
        }

        internal Guid UnDockShipFromLoadingDockToShipDock(Guid shipID)
        {
            Ship shipToBeUndocked = GetShipFromLoadingDock(shipID);

            if (shipToBeUndocked != null)
            {
                Dock oldDock = (Dock)shipsInLoadingDock[shipToBeUndocked];

                oldDock.DockedShip = Guid.Empty;
                oldDock.Free = true;

                Dock newDock = GetFreeShipDock(shipToBeUndocked.ShipSize);

                shipToBeUndocked.CurrentLocation = newDock.ID;
                shipsInLoadingDock.Remove(shipToBeUndocked);
                freeLoadingDocks.Add(oldDock);
                if (!shipsInShipDock.ContainsKey(shipToBeUndocked))
                {
                    shipsInShipDock.Add(shipToBeUndocked, newDock);
                }

                return oldDock.ID;
            }

            return Guid.Empty;
        } //returnerer Guid til docken skipet docket fra


        internal Guid UnDockShipFromShipDockToLoadingDock(Guid shipID)
        {
            Ship shipToBeUndocked = GetShipFromShipDock(shipID);

            if (shipToBeUndocked != null)
            {
                Dock oldDock = (Dock)shipsInShipDock[shipToBeUndocked];

                oldDock.DockedShip = Guid.Empty;
                oldDock.Free = true;

                Dock newDock = GetFreeLoadingDock(shipToBeUndocked.ShipSize);

                shipToBeUndocked.CurrentLocation = newDock.ID;
                shipsInShipDock.Remove(shipToBeUndocked);
                freeShipDocks.Add(oldDock);
                if (!shipsInLoadingDock.ContainsKey(shipToBeUndocked))
                {
                    shipsInLoadingDock.Add(shipToBeUndocked, newDock);
                }

                return oldDock.ID;
            }

            return Guid.Empty;
        } //returnerer Guid til docken skipet docket fra

        internal bool RemoveLoadingDockFromFreeLoadingDocks(Guid dockID)
        {
            foreach (Dock dock in freeLoadingDocks)
            {
                if (dock.ID == dockID)
                {
                    freeLoadingDocks.Remove(dock);
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

            return ship.ID;

        }

        internal void AddNewShipToAnchorage(Ship ship)
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
            foreach(Dock dock in allShipDocks)
            {
                dockStatus[dock.ID] = dock.Free;
            }
            return dockStatus;
        }

        //Denne kan potensielt endres
        //må endre på toString til en representasjon som fungerer
        public string LoadingDockIsFree(Guid dockID)
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
        //La de til for kompilering
        public Guid GetTransitID()
        {
            throw new NotImplementedException();
        }

        public IDictionary<Guid, bool> LoadingDockIsFreeForAllDocks()
        {
            throw new NotImplementedException();
        }

        bool IHarbor.LoadingDockIsFree(Guid dockID)
        {
            throw new NotImplementedException();
        }

        public IDictionary<Guid, bool> ShipDockIsFreeForAllDocks()
        {
            throw new NotImplementedException();
        }

        IDictionary<Ship, Status> IHarbor.GetStatusAllShips()
        {
            throw new NotImplementedException();
        }

        internal void AddContainersToHarbor(int numberOfcontainers, DateTime currentTime)
        {

            for (int i = 0; i < numberOfcontainers; i++)
            {
                Container containerToBeStored = new Container(ContainerSize.Small, 10, this.ID);

                if (i % 3 == 0)
                {
                    containerToBeStored = new Container(ContainerSize.Small, 10, this.ID);
                }
                if (i % 3 == 1)
                {
                    containerToBeStored = new Container(ContainerSize.Medium, 15, this.ID);
                }
                if (i % 3 == 2)
                {
                    containerToBeStored = new Container(ContainerSize.Large, 15, this.ID);
                }

                ContainerSize containerSize = containerToBeStored.Size;
                ContainerSpace containerSpace = GetFreeContainerSpace(containerSize);

                freeContainerSpaces[containerSize].Remove(containerSpace);

                storedContainers.Add(containerToBeStored, containerSpace);

                containerSpace.storedContainer = containerToBeStored.ID;
                containerSpace.Free = false;

                containerToBeStored.CurrentPosition = containerSpace.ID;
                containerToBeStored.AddHistoryEvent(Status.InStorage, currentTime);
            }
                
        }

        internal IList<Container> GetContainersStoredInHarbour()
        {
            IList<Container> list = new List<Container>();

            foreach (Container container in storedContainers.Keys)
            {
                list.Add(container);
            }

            return list;
        }

        internal IList<Ship> GetShipsInLoadingDock()
        {
            IList<Ship> list = new List<Ship>();

            foreach (Ship ship in shipsInLoadingDock.Keys)
            {
                list.Add(ship);
            }
            return list;
        }

        internal IList<Ship> GetShipsInShipDock()
        {
            IList<Ship> list = new List<Ship>();

            foreach (Ship ship in shipsInShipDock.Keys)
            {
                list.Add(ship);
            }
            return list;
        }

        internal IList<Ship> GetShipsInTransit()
        {
            IList<Ship> list = new List<Ship>();
            foreach (Ship ship in ShipsInTransit.Keys)
            {
                list.Add(ship);
            }
            return list;
        }
    }
}
       

               
