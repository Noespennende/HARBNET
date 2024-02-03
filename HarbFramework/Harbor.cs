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
        internal ArrayList AllDocks = new ArrayList();
        internal ArrayList FreeDocks = new ArrayList();
        internal Hashtable ShipsInDock = new Hashtable(); // Ship : Dock
        internal ArrayList HarbourQueInn = new ArrayList();
        internal Hashtable ShipsInTransit = new Hashtable(); // ship: int number of days until return

        internal ArrayList AllShips { get; set; } = new ArrayList(); // Sikkert midlertidig, til vi kan regne på det

        internal Dictionary<ContainerSize, List<ContainerSpace>> AllContainerSpaces = new();
        //internal Hashtable allContainerSpaces = new Hashtable(); // størelse : antall
        internal Dictionary<ContainerSize, List<ContainerSpace>> FreeContainerSpaces = new();
        //internal Hashtable freeContainerSpaces = new Hashtable(); // størelse : antall ledige
        internal Dictionary<Container, ContainerSpace> StoredContainers = new(); // Container : ContainerSpace
        internal Guid TransitLocationID = Guid.NewGuid();
        internal Guid HarbourQueInnID = Guid.NewGuid();


        public Harbor(ICollection<Ship> listOfShips, int numberOfSmallDocks, int numberOfMediumDocks, int numberOfLargeDocks, int numberOfSmallContainerSpaces, int numberOfMediumContainerSpaces,
            int numberOfLargeContainerSpaces)
        {
            for (int i = 0; i < numberOfSmallDocks; i++)
            {
                AllDocks.Add(new Dock(ShipSize.Small));
            }

            for (int i = 0; i < numberOfMediumDocks; i++)
            {
                AllDocks.Add(new Dock(ShipSize.Medium));
            }
            for (int i = 0; i < numberOfLargeDocks; i++)
            {
                AllDocks.Add(new Dock(ShipSize.Large));
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
            HarbourQueInn.AddRange((ICollection)listOfShips);


            foreach (Ship ship in HarbourQueInn)
            {
                ship.CurrentLocation = HarbourQueInnID;
            }

            FreeDocks = (ArrayList)AllDocks.Clone();
        }

        internal Guid DockShip(Guid shipID, DateTime currentTime) //omskriv til å sende inn størrelse. 
        {
            Ship shipToBeDocked = GetShipFromQueue(shipID);
            ShipSize size = shipToBeDocked.ShipSize;
            Dock dock;

            if (FreeDockExists(size))
            {
                dock = GetFreeDock(size);
                dock.DockedShip = shipToBeDocked.GetID();
                dock.Free = false;

                shipToBeDocked.CurrentLocation = dock.GetID();
                shipToBeDocked.AddHistoryEvent(currentTime, dock.ID, Status.Docking);
                ShipsInDock.Add(shipToBeDocked, dock);

                RemoveShipFromQueue(shipToBeDocked.GetID());
                RemoveDockFromFreeDocks(dock.GetID());

                return dock.GetID();
            }

            return Guid.Empty; //returnerer en Guid med verdi "00000000-0000-0000-0000-000000000000" hvis han ikke finner noen ledige docker.
        }//returnerer Guid til docken skipet docker til

        internal Guid UnDockShip(Guid shipID, DateTime currentTime)
        {
            Ship shipToBeUndocked = GetShipFromDock(shipID);

            if (shipToBeUndocked != null)
            {
                Dock dock = (Dock)ShipsInDock[shipToBeUndocked];
                //need to add history event for ship
                dock.DockedShip = Guid.Empty;
                dock.Free = true;

                shipToBeUndocked.CurrentLocation = TransitLocationID;
                shipToBeUndocked.AddHistoryEvent(currentTime, Guid.Empty, Status.Transit);

                ShipsInDock.Remove(shipToBeUndocked);
                FreeDocks.Add(dock);
                if (!ShipsInTransit.ContainsKey(shipToBeUndocked))
                {
                    ShipsInTransit.Add(shipToBeUndocked, shipToBeUndocked.RoundTripInDays);
                }
                return dock.GetID();
            }

            return Guid.Empty;
        } //returnerer Guid til docken skipet docket fra

        internal Ship GetShipFromQueue(Guid shipID)
        {
            foreach (Ship ship in HarbourQueInn)
            {
                if (ship.GetID().Equals(shipID))
                {
                    return ship;
                }
            }

            return null;
        }

        internal Ship GetShipFromDock(Guid shipID)
        {
            foreach (Ship ship in ShipsInDock.Keys)
            {
                if (ship.GetID() == shipID)
                {
                    return ship;
                }
            }
            return null;
        }
        internal ArrayList DockedShips()
        {
            ArrayList ships = new ArrayList();

            foreach (Ship ship in ShipsInDock.Keys)
            {
                ships.Add(ship); // Legg til hvert individuelle Ship-objekt, ikke hele Hashtable
            }

            return ships;
        }
        internal bool FreeDockExists(ShipSize shipSize)
        {
            foreach (Dock dock in FreeDocks)
            {
                if (dock.Free == true && dock.Size == shipSize)
                {
                    return true;
                }
            }
            return false;
        }
        internal Dock GetFreeDock(ShipSize shipSize)
        {

            foreach (Dock dock in FreeDocks)
            {
                if (dock.Free == true && dock.Size == shipSize)
                {
                    return dock;
                }
            }

            return null;
        }
        internal bool RemoveShipFromQueue(Guid shipID)
        {
            foreach (Ship ship in HarbourQueInn)
            {
                if (ship.GetID() == shipID)
                {
                    HarbourQueInn.Remove(ship);
                    return true;
                }
            }
            return false;
        }
        internal bool RemoveDockFromFreeDocks(Guid dockID)
        {
            foreach (Dock dock in FreeDocks)
            {
                if (dock.GetID() == dockID)
                {
                    HarbourQueInn.Remove(dock);
                    return true;
                }
            }
            return false;
        }
        internal int NumberOfFreeDocks(ShipSize shipSize)
        {
            int count = 0;
            foreach (Dock dock in FreeDocks)
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

            return ship.ID;

        }

        internal void AddNewShipToHarbourQueue(Ship ship)
        {
            HarbourQueInn.Add(ship);
        }

        /* ** Interface implementasjon som må gjøres ** */

        // Obs obs - sjekk kommentert ut metode i Interface (Fra nylig push av Andreas)
        // De måtte kommenteres ut, kan ikke ha samme navn. Vet ikke hvilke som er riktige
        public string GetShipStatus(Guid ShipID)
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
            return sb.ToString();
        }



        // Obs obs - sjekk kommentert ut metode i Interface (Fra nylig push av Andreas)
        // De måtte kommenteres ut, kan ikke ha samme navn. Vet ikke hvilke som er riktige

        //må kjøre denne for å se om den funker som tenkt
        public string GetStatusAllShips()
        {

            StringBuilder sb = new StringBuilder();
            foreach (Ship ship in AllShips)
            {
                Event lastEvent = null;

                for (int i = 0; i < ship.History.Count; i++)
                {
                    // Måtte kommentere ut for å kunne kjøre fordi ship.History[i]; gir error
                    // lastEvent = (Event)ship.History[i];

                    string shipStatus = $"ShipId: {ship.ID} Last event: {lastEvent}";

                    sb.AppendLine(shipStatus);
                }
            }
            return sb.ToString();
        }

        //Denne kan potensielt endres
        //må endre på toString til en representasjon som fungerer
        public string GetDockStatus(Guid dockID)
        {
            StringBuilder sb = new StringBuilder();
            bool dockFree = false;
            foreach (Dock dock in AllDocks)
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
        public string GetStatusAllDocks()
        {
            StringBuilder sb = new StringBuilder();
            Dictionary<Dock, bool> dockStatus = new Dictionary<Dock, bool>();

            foreach (Dock dock in AllDocks)
            {
                dockStatus[dock] = dock.Free;

            }
            foreach (var keyValue in dockStatus)
            {
                sb.AppendLine($"dockId: {keyValue.Key}, dock free: {keyValue.Value}");
            }
            return sb.ToString();
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
               
