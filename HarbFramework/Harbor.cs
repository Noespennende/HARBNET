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
        public Guid ID { get; internal set; } = Guid.NewGuid();
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


            /// <summary>
            /// Creates what size and number of containers a harbor can hold
            /// </summary>
            /// <param name="containerSize">The size of the container in Small, Medium om Large</param>
            /// <param name="numberOfSpaces">The number of containers the harbor can hold</param>
            /// <return>Returns nothing</return>
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

        /// <summary>
        /// Docks ship to loading dock
        /// </summary>
        /// <param name="shipID">unique ID of specific ship</param>
        /// <param name="currentTime">Time the ship is docked</param>
        /// <returns>Returns the Guid of the dock the ship gets docked to</returns>
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


        /// <summary>
        /// Transfering ship from dock to loading dock
        /// </summary>
        /// <param name="shipID">Unique ID of specific ship</param>
        /// <param name="currentTime">Time ship is transfered</param>
        /// <returns>Returns the Guid of the loading dock the ship gets docked to</returns>
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

        /// <summary>
        /// Ship gets docked to dock
        /// </summary>
        /// <param name="shipID">Unique ID of specific ship</param>
        /// <returns>Returns the Guid of the dock the ship gets docked to</returns>
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

        /// <summary>
        /// Ship in anchorage gets moved to dock
        /// </summary>
        /// <param name="shipID">Unique ID of specific ship</param>
        /// <returns>Returns the Guid of the dock the ship gets docked to</returns>
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
                RemoveShipFromAnchorage(shipID);

                return dock.ID;
                
            }

            return Guid.Empty;

        }

        /// <summary>
        /// Ship in loading dock got get moved to dock for ships in transit 
        /// </summary>
        /// <param name="shipID">Unique ID of specific ship</param>
        /// <param name="currentTime">Time ship is transfered</param>
        /// <returns>Returns the Guid of the dock the ship gets docked from</returns>
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

        /// <summary>
        /// Gets specific ship from anchorage
        /// </summary>
        /// <param name="shipID">Unique ID of specific ship</param>
        /// <returns>Returns ship with the unique ID</returns>
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

        /// <summary>
        /// Gets specific ship from loading dock
        /// </summary>
        /// <param name="shipID">Unique ID of specific ship</param>
        /// <returns>Returns ship with the unique ID</returns>
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

        /// <summary>
        /// Gets specific ship from ship docks
        /// </summary>
        /// <param name="shipID">Unique ID of specific ship</param>
        /// <returns>Returns ship with the unique ID</returns>
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

        /// <summary>
        /// Gets all the loading docks that has a ship docked
        /// </summary>
        /// <param name="shipID">Unique ID of specific ship</param>
        /// <returns>Loading dock that contains docked ships</returns>
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

        /// <summary>
        /// Creates a list of all docked ships in loading docks
        /// </summary>
        /// <returns>List of all ships docked in loading dock</returns>
        internal List<Ship> DockedShipsInLoadingDock()
        {
            List<Ship> ships = new List<Ship>();

            foreach (var item in shipsInLoadingDock)
            {
                ships.Add(item.Key); // Legg til hvert individuelle Ship-objekt, ikke hele Hashtable
            }

            return ships;
        }

        /// <summary>
        /// Checks if a loading dock matching the specified shipsize is available
        /// </summary>
        /// <param name="shipSize">The size of the ship in Small, Medium om Large</param>
        /// <returns>Returns true og false depending on if a free loading dock is available</returns>
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

        /// <summary>
        /// Checks if a free ship dock matching the specified shipsize is available
        /// </summary>
        /// <param name="shipSize">The size of the ship in Small, Medium om Large</param>
        /// <returns>Returns true og false depending on if a free ship dock is available</returns>
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

        /// <summary>
        /// Gets all free loading dock matching the specified shipsize
        /// </summary>
        /// <param name="shipSize">The size of the ship in Small, Medium om Large</param>
        /// <returns>Returns available loading docks</returns>
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

        /// <summary>
        /// Gets all free ship dock matching the specified shipsize
        /// </summary>
        /// <param name="shipSize">The size of the ship in Small, Medium om Large</param>
        /// <returns>Returns available ship docks</returns>
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

        /// <summary>
        /// Removes specified ship from anchorage
        /// </summary>
        /// <param name="shipID">Unique ID of specific ship</param>
        /// <returns>Returns true if specified ship was in anchorage, or false if specified ship wasn't in anchorage</returns>
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

        /// <summary>
        /// Undocks ship from loading lock to ship dock
        /// </summary>
        /// <param name="shipID">Unique ID of specific ship</param>
        /// <returns>Returns Guid from the dock the ship docked from</returns>
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

        /// <summary>
        /// Undocks ship from ship dock to loading dock
        /// </summary>
        /// <param name="shipID">Unique ID of specific ship</param>
        /// <returns>Returns Guid from the dock the ship docked from</returns>
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

        /// <summary>
        /// Removes loading dock from list of free loading docks
        /// </summary>
        /// <param name="dockID">Unique ID of specific dock</param>
        /// <returns>Returns true if specified dock was in the list of free loading docks, or false if specified dock wasn't in the list of free loading docks</returns>
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

        /// <summary>
        /// Counts the number of available loading docks of the specified size
        /// </summary>
        /// <param name="shipSize">The size of the ship in Small, Medium om Large</param>
        /// <returns>Returns the total amount of available loading docks of specified size</returns>
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

        /// <summary>
        /// Counts the number of available containerspaces of specified size
        /// </summary>
        /// <param name="containerSize">The size of the container in Small, Medium om Large</param>
        /// <returns>Returns the total number of available loading containerspaces of specified size</returns>
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

        /// <summary>
        /// Counts the number of occupied containerspaces of specified size
        /// </summary>
        /// <param name="containerSize">The size of the container in Small, Medium om Large</param>
        /// <returns>Returns the total number of occupoed loading containerspaces of specified size</returns>
        internal int GetNumberOfOccupiedContainerSpaces(ContainerSize containerSize)
        {
            return storedContainers.Count;

        } //returnerer antallet okuperte plasser av den gitte typen


        /// <summary>
        /// Gets the available container space of specified size
        /// </summary>
        /// <param name="containerSize">The size of the container in Small, Medium om Large</param>
        /// <returns>Returns the Guid of an avaiable containerspace of specified size</returns>
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

        /// <summary>
        /// Gets the stored containers of specified size
        /// </summary>
        /// <param name="containerSize">The size of the container in Small, Medium om Large</param>
        /// <returns>Returns the Guid of a stored container of specified size</returns>
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

        /// <summary>
        /// Removes specific containerspace from free containerspace
        /// </summary>
        /// <param name="containerSpaceID">Unique ID of specific container</param>
        /// <returns>Returns true if specified containerspace is succesfully removed, or returns false if containerspace is not removed</returns>
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

        /// <summary>
        /// Unloads container of specific size from ship to containerspace
        /// </summary>
        /// <param name="containerSize">The size of the container in Small, Medium om Large</param>
        /// <param name="ship">A ship object</param>
        /// <param name="currentTime">Time container is unloaded</param>
        /// <returns>Returns Guid to the containerspaces the unloaded container was stored, returns empty if container did not exist</returns>
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

        /// <summary>
        /// Loads container from containerspace to ship
        /// </summary>
        /// <param name="containerSize">The size of the container in Small, Medium om Large</param>
        /// <param name="ship">A ship object</param>
        /// <param name="currentTime">Time container is unloaded</param>
        /// <returns>Returns the guid to the ship the container was loaded on, or returns empty if container did not exist</returns>
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

        /// <summary>
        /// Adds new ship to anchorage
        /// </summary>
        /// <param name="ship">A ship object</param>
        internal void AddNewShipToAnchorage(Ship ship)
        {
            Anchorage.Add(ship);
            if (ShipsInTransit.ContainsKey(ship))
                ShipsInTransit.Remove(ship);
        }

        // Obs obs - sjekk kommentert ut metode i Interface (Fra nylig push av Andreas)
        // De måtte kommenteres ut, kan ikke ha samme navn. Vet ikke hvilke som er riktige

        /// <summary>
        /// Gets last registered status of specific ship
        /// </summary>
        /// <param name="ShipID">Unique ID of specific ship</param>
        /// <returns>Returns last registered status of specified ship if the ship has a history, or returns none</returns>
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

        /// <summary>
        /// Gets last registered status of all ships
        /// </summary>
        /// <returns>Returns last registered status of all ships</returns>
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

        /// <summary>
        /// Gets the status of all docks
        /// </summary>
        /// <returns>Returns the status of all docks</returns>
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
        /// <summary>
        /// Checks if specified dock is free
        /// </summary>
        /// <param name="dockID">Unique ID of specific dock</param>
        /// <returns>Returns string informing of specified dock and if it's free</returns>
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
        /// <summary>
        /// Gets the status of all loading docks
        /// </summary>
        /// <returns>Returns a string with all the dock IDs and their status</returns>
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
        /// <summary>
        /// Gets the status of specified container
        /// </summary>
        /// <param name="ContainerId">Unique ID of specific container</param>
        /// <returns>Returns a string with the dock ID and their status</returns>
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
        /// <summary>
        /// Gets the last registered status of all containers
        /// </summary>
        /// <returns>Returns a string with the container ID and their last registered status</returns>
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

        /// <summary>
        /// Checks if loading dock is free/available for all dock sizes
        /// </summary>
        /// <returns>Returns all the docks or null</returns>
        public IDictionary<Guid, bool> LoadingDockIsFreeForAllDocks() // må dobbeltsjekke om er riktig
        {
            Dictionary<Guid, bool> freeLoadingDock = new Dictionary<Guid, bool>();

            foreach (Dock dock in allShipDocks)
            {
                if (dock.Free == true)
                {
                    return freeLoadingDock;
                }
            }
            return null;
        }

        /// <summary>
        /// Checks if specified dock is free
        /// </summary>
        /// <param name="dockID">Unique ID of specific dock</param>
        /// <returns>Returns true if specified dock is free, or false if not</returns>
        bool IHarbor.LoadingDockIsFree(Guid dockID) // ferdig
        {
            bool dockIsFree = false;
            foreach (Dock dock in allLoadingDocks)
            {
                if (dockID == dock.ID)
                {
                    dockIsFree = true;
                }
            }
            return dockIsFree;
        }

        /// <summary>
        /// Checks if ship dock is free/available for all dock sizes
        /// </summary>
        /// <returns>Returns true if all ship docka is free, or false if not</returns>
        public IDictionary<Guid, bool> ShipDockIsFreeForAllDocks() // må dobbeltsjekke om er riktig
        {
            Dictionary<Guid, bool> freeShipDock = new Dictionary<Guid, bool>();

            foreach (Dock dock in allShipDocks)
            {
                if (dock.Free == true)
                {
                    return freeShipDock;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the last registered status from all ships
        /// </summary>
        /// <returns>Return the last registered status of all ships, if they have a status</returns>
        IDictionary<Ship, Status> IHarbor.GetStatusAllShips()
        {
            Dictionary<Ship, Status> statusOfAllShips = new Dictionary<Ship, Status>();

            foreach (Ship ship in AllShips)
            {
                Event test = ship.History.Last();
                if (test != null)
                {
                    statusOfAllShips[ship] = test.Status;
                }
            }
            return statusOfAllShips;
        }

        /// <summary>
        /// Add specified number of containers to storedcontainers in harbor
        /// </summary>
        /// <param name="numberOfcontainers">Specified number of containers</param>
        /// <param name="currentTime">Time container is added to harbor</param>
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

        /// <summary>
        /// Gets all containers stored in harbor
        /// </summary>
        /// <returns>Returns list of all containors stored in harbor</returns>
        internal IList<Container> GetContainersStoredInHarbour()
        {
            IList<Container> list = new List<Container>();

            foreach (Container container in storedContainers.Keys)
            {
                list.Add(container);
            }

            return list;
        }

        /// <summary>
        /// Gets all ships in a loading dock
        /// </summary>
        /// <returns>Returns a list of all ships in a loading dock</returns>
        internal IList<Ship> GetShipsInLoadingDock()
        {
            IList<Ship> list = new List<Ship>();

            foreach (Ship ship in shipsInLoadingDock.Keys)
            {
                list.Add(ship);
            }
            return list;
        }

        /// <summary>
        /// Gets all ships in a ship dock
        /// </summary>
        /// <returns>Returns a list of all ships in a ship dock</returns>
        internal IList<Ship> GetShipsInShipDock()
        {
            IList<Ship> list = new List<Ship>();

            foreach (Ship ship in shipsInShipDock.Keys)
            {
                list.Add(ship);
            }
            return list;
        }

        /// <summary>
        /// Gets all ships in transit
        /// </summary>
        /// <returns>Returns a list of all ships in transit</returns>
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
       

               
