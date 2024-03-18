using Gruppe8.HarbNet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Tracing;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Formats.Asn1.AsnWriter;

namespace Gruppe8.HarbNet
{
    /// <summary>
    /// Harbor to be used in a simulation
    /// </summary>
    public class Harbor : IHarbor
    {
        /// <summary>
        /// Unique ID for harbor
        /// </summary>
        /// <return>Returns the unique ID defining a specific harbor</return>
        public Guid ID { get; internal set; } = Guid.NewGuid();

        /// <summary>
        /// Gets all loading docks
        /// </summary>
        /// <return>Returns a list of all loading docks</return>
        internal IList<Dock> allLoadingDocks = new List<Dock>();

        /// <summary>
        /// Gets all available loading docks
        /// </summary>
        /// <return>Returns a list of all available loading docks</return>
        internal IList<Dock> freeLoadingDocks = new List<Dock>();

        /// <summary>
        /// Gets all ships in loading dock
        /// </summary>
        /// <return>Returns a dictionary with all the ships that are in a loading dock</return>
        internal IDictionary<Ship, Dock> shipsInLoadingDock = new Dictionary<Ship, Dock>(); // Ship : Dock

        /// <summary>
        /// Gets all ship docks
        /// </summary>
        /// <return>Returns a list of all ship docks</return>
        internal IList<Dock> allShipDocks = new List<Dock>();

        /// <summary>
        /// Gets all available ship docks
        /// </summary>
        /// <return>Return a list of all available ship docks</return>
        internal IList<Dock> freeShipDocks = new List<Dock>();

        /// <summary>
        /// Gets all ships in ship dock
        /// </summary>
        /// <return>Returns a dictionary with all ships in ship dock</return>
        internal IDictionary<Ship, Dock> shipsInShipDock = new Dictionary<Ship, Dock>(); // Ship : Dock

        /// <summary>
        /// Gets all ships in anchorage
        /// </summary>
        /// <return>Returns a list of all ships in anchorage</return>
        internal IList<Ship> Anchorage { get; } = new List<Ship>();

        /// <summary>
        /// Gets all ships in transit
        /// </summary>
        /// <return>Returns a dictionary with all ships in transit</return>
        internal IDictionary<Ship, int> ShipsInTransit { get; } = new Dictionary<Ship, int>(); // ship: int number of days until return

        /// <summary>
        /// Gets all ships
        /// </summary>
        /// <return>Returns a list of all ships</return>
        internal IList<Ship> AllShips { get; } = new List<Ship>(); // Sikkert midlertidig, til vi kan regne på det

        /// <summary>
        /// Gets all container spaces
        /// </summary>
        /// <return>Returns a dictionary of all container spaces</return>
        internal IList<ContainerRow> allContainerRows { get; set; }

        /// <summary>
        /// Gets all stored containers
        /// </summary>
        /// <return>Returns a dictionary of all stored containers</return>
        internal IDictionary<Container, ContainerRow> storedContainers = new Dictionary<Container, ContainerRow>(); // Container : ContainerRow

        internal IList<Crane> HarborStorageAreaCranes { get; set; } = new List<Crane>();

        internal IList<Adv> AdvWorking { get; set; } = new List<Adv>();

        internal IList<Adv> AdvFree { get; set; } = new List<Adv>();

        internal IList<Truck> TrucksInTransit { get; set; } = new List<Truck>();

        internal IList<Truck> TrucksInQueue { get; set; } = new List<Truck>();

        internal double PercentOfContainersDirectlyLoaded { get; set; }

        internal int TrucksArrivePerHour { get; set; }

        internal int AdvLoadsPerHour { get; set; }

        /// <summary>
        /// Gets the unique ID for the transit location
        /// </summary>
        /// <return>Returns an unique Guid defining a specific transitlocation</return>
        public Guid TransitLocationID { get; } = Guid.NewGuid();

        /// <summary>
        /// Gets the unique ID for the anchorage
        /// </summary>
        /// <return>Returns the unique Guid defining a specific anchorage</return>
        public Guid AnchorageID { get; } = Guid.NewGuid();
        public Guid AdvCargoID { get; } = Guid.NewGuid();
        public Guid TruckTransitLocationID { get; } = Guid.NewGuid();
        public Guid HarborStorageAreaID { get; } = Guid.NewGuid();


        /// <summary>
        /// OPPDATER DENNE!
        /// Constructor for Harbor, creates a new harbor object
        /// </summary>
        /// <param name="listOfShips">List of all ships</param>
        /// <param name="numberOfSmallLoadingDocks">Total number of small loading docks</param>
        /// <param name="numberOfMediumLoadingDocks">Total number of medium loading docks</param>
        /// <param name="numberOfLargeLoadingDocks">Total number of large loading docks</param>
        /// <param name="numberOfSmallShipDocks">Total number of small ship docks</param>
        /// <param name="numberOfMediumShipDocks">Total number of medium ship docks</param>
        /// <param name="numberOfLargeShipDocks">Total number of large ship docks</param>
        /// <param name="numberOfSmallContainerSpaces">Total number of small container spaces</param>
        /// <param name="numberOfMediumContainerSpaces">Total number of medium contrainer spaces</param>
        /// <param name="numberOfLargeContainerSpaces">Total number of large container spaces</param>
        public Harbor(IList<Ship> listOfShips, int numberOfSmallLoadingDocks, int numberOfMediumLoadingDocks, int numberOfLargeLoadingDocks, int numberOfCranesPerLoadingDock, int LoadsPerCranePerHour, int numberOfCranesOnHarborStorageArea,
            int numberOfSmallShipDocks, int numberOfMediumShipDocks, int numberOfLargeShipDocks,
            int numberOfContainerRows, int numberOfHalfSizeContainersInEachRow, int numberOfFullSizeContainersInEachRow, int numberOfTrucksArriveToHarborPerHour, int percentageOfContainersDirectlyLoadedToTrucks, int AdvLoadsPerHour)
        {

            this.TrucksArrivePerHour = numberOfTrucksArriveToHarborPerHour;
            this.PercentOfContainersDirectlyLoaded = (percentageOfContainersDirectlyLoadedToTrucks / 100);
            this.AdvLoadsPerHour = AdvLoadsPerHour;

            for (int i = 0; i < numberOfCranesOnHarborStorageArea; i++)
            {
                HarborStorageAreaCranes.Add(new Crane(LoadsPerCranePerHour, HarborStorageAreaID));
            }

            int smallSingleTripShipCount = 0;
            int mediumSingleTripShipCount = 0;
            int largeSingleTripShipCount = 0;

            foreach (Ship ship in listOfShips)
            {
                if (ship.IsForASingleTrip)
                {
                    if (ship.ShipSize == ShipSize.Small) { smallSingleTripShipCount++; }
                    else if (ship.ShipSize == ShipSize.Medium) { mediumSingleTripShipCount++; }
                    else if (ship.ShipSize == ShipSize.Large) { largeSingleTripShipCount++; }
                }
            }

            if (smallSingleTripShipCount > numberOfSmallLoadingDocks)
            {
                throw new ArgumentOutOfRangeException("There are fewer smallLoadingDocks than there are single-trip ships of the size small in the listOfShips given." +
                    "Single trip ships needs a loading dock of similar size to start their trip from. Make sure numberOfSmallLoadingDocks is equal to or larger than the number of small sized singletrip ships in your list.");
            }
            if (mediumSingleTripShipCount > numberOfMediumLoadingDocks)
            {
                throw new ArgumentOutOfRangeException("There are fewer medium loading docks than there are single-trip ships of the size medium in the listOfShips given." +
                    "Single trip ships needs a loading dock of similar size to start their trip from. Make sure numberOfMediumlLoadingDocks is equal to or larger than the number of medium sized singletrip ships in your list.");
            }
            if (largeSingleTripShipCount > numberOfLargeLoadingDocks)
            {
                throw new ArgumentOutOfRangeException("There are fewer large loading docks than there are single-trip ships of the size large in the listOfShips given." +
                    "Single trip ships needs a loading dock of similar size to start their trip from. Make sure numberOfLargelLoadingDocks is equal to or larger than the number of large sized singletrip ships in your list.");
            }


            for (int i = 0; i < numberOfSmallLoadingDocks; i++)
            {
                allLoadingDocks.Add(new Dock(ShipSize.Small, numberOfCranesPerLoadingDock, LoadsPerCranePerHour));
            }

            for (int i = 0; i < numberOfMediumLoadingDocks; i++)
            {
                allLoadingDocks.Add(new Dock(ShipSize.Medium, numberOfCranesPerLoadingDock, LoadsPerCranePerHour));
            }
            for (int i = 0; i < numberOfLargeLoadingDocks; i++)
            {
                allLoadingDocks.Add(new Dock(ShipSize.Large, numberOfCranesPerLoadingDock, LoadsPerCranePerHour));
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

            CreateContainerSpaces(numberOfFullSizeContainersInEachRow, numberOfHalfSizeContainersInEachRow, numberOfContainerRows);

            AllShips = listOfShips.ToList();

            freeShipDocks = allShipDocks.ToList();
            freeLoadingDocks = allLoadingDocks.ToList();

        }

        /// <summary>
        /// OPPDATER DENNE
        /// Creates what size and number of containers a harbor can hold
        /// </summary>
        /// <param name="containerSize">The size of the container in Small, Medium om Large</param>
        /// <param name="numberOfSpaces">The number of containers the harbor can hold</param>
        /// <return>Returns nothing</return>
        internal void CreateContainerSpaces(int numberOfFullSizeContainerSpaces, int numberOfHalfSizeContainerSpaces, int numberOfContainerRows)
        {
            IList <ContainerRow> containerRows = new List <ContainerRow>();

            for (int j = 0; j < numberOfContainerRows; j++)
            {
                containerRows.Add(new ContainerRow(numberOfFullSizeContainerSpaces, numberOfHalfSizeContainerSpaces));
            }

            this.allContainerRows = containerRows;
        }
        /// <summary>
        /// Container unloads from the ship
        /// </summary>
        /// <param name="ship">Ship object</param>
        /// <param name="crane">Crane object</param>
        /// <param name="currentTime">The current time</param>
        /// <returns>the container being unloaded</returns>
        internal Container ShipToCrane(Ship ship, Crane crane, DateTime currentTime)
        {
            Container containerToBeLoaded = ship.UnloadContainer();
            containerToBeLoaded.AddStatusChangeToHistory(Status.LoadingToCrane, currentTime);
            crane.LoadContainer(containerToBeLoaded);

            return containerToBeLoaded;

        }

        /// <summary>
        /// The crane loads a container to a ship
        /// </summary>
        /// <param name="ship">Ship object</param>
        /// <param name="crane">Crane object</param>
        /// <param name="currentTime">The current time</param>
        /// <returns>the container being loaded to ship</returns>
        internal Container CraneToShip(Crane crane, Ship ship, DateTime currentTime)
        {
            Container containerToBeLoaded = crane.UnloadContainer();
            containerToBeLoaded.AddStatusChangeToHistory(Status.Loading, currentTime);
            ship.AddContainer(crane.UnloadContainer());

            return containerToBeLoaded;
        }

          /// <summary>
          /// Loads a container to a truck
          /// </summary>
          /// <param name="crane">Crane object</param>
          /// <param name="truck">Truck object</param>
          /// <param name="currentTime">the Current Time</param>
          /// <returns>Container being loaded to the truck</returns>
        internal Container CraneToTruck(Crane crane, Truck truck, DateTime currentTime)
        {
            Container containerToBeLoaded = crane.UnloadContainer();
            truck.LoadContainer(crane.UnloadContainer());
            containerToBeLoaded.AddStatusChangeToHistory(Status.LoadingToTruck, currentTime);

            return containerToBeLoaded;
        }
        /// <summary>
        /// Loading an ADV with a container
        /// </summary>
        /// <param name="crane">Crane object</param>
        /// <param name="adv">ADV object</param>
        /// <param name="currentTime">The current time</param>
        /// <returns>Container object being Loaded</returns>
        internal Container CraneToAdv(Crane crane, Adv adv, DateTime currentTime)
        {
            Container containerToBeLoaded = crane.UnloadContainer();
            adv.LoadContainer(crane.UnloadContainer());
            containerToBeLoaded.AddStatusChangeToHistory(Status.LoadingToAdv, currentTime);

            return containerToBeLoaded;
        }
        /// <summary>
        /// Unload from ADV 
        /// </summary>
        /// <param name="crane">Crane object</param>
        /// <param name="adv">ADV object</param>
        /// <param name="currentTime">The current time</param>
        /// <returns>Container object being unloaded</returns>
        internal Container AdvToCrane(Crane crane, Adv adv, DateTime currentTime)
        {

            Container containerToBeLoaded = adv.UnloadContainer();
            crane.LoadContainer(adv.UnloadContainer());
            containerToBeLoaded.AddStatusChangeToHistory(Status.LoadingToCrane, currentTime);

            return containerToBeLoaded;
        }
        /// <summary>
        /// Crane Load container to storage if there is room
        /// </summary>
        /// <param name="crane">Crane object</param>
        /// <param name="currentTime">the current Time</param>
        /// <returns>True or false</returns>
        internal bool CraneToContainerRow(Crane crane,DateTime currentTime)
        {
            Container container = crane.UnloadContainer();

            foreach (ContainerRow CR in allContainerRows)
            {
                if (CR.CheckIfFreeContainerSpaceExists(container.Size))
                {
                    CR.AddContainerToFreeSpace(container);
                    storedContainers.Add(container, CR);
                    container.AddStatusChangeToHistory(Status.InStorage, currentTime);
                    return true;
                }
            }

            crane.LoadContainer(container);
            return false;
        }
        /// <summary>
        /// removes container from containerrow
        /// </summary>
        /// <param name="size">the size of the container</param>
        /// <param name="crane">crane object</param>
        /// <param name="currentTime">the current time</param>
        /// <returns>True or false</returns>
        internal bool ContainerRowToCrane(ContainerSize size, Crane crane,DateTime currentTime)
        {
            foreach (Container container in storedContainers.Keys){
                if (container.Size == size)
                {
                    crane.LoadContainer(container);
                    container.AddStatusChangeToHistory(Status.LoadingToCrane, currentTime );
                    storedContainers[container].RemoveContainerFromContainerRow(container);
                    
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// finds the number of free container spaces
        /// </summary>
        /// <returns>an int with how many free spaces there are</returns>
        internal int numberOfFreeContainerSpaces ()
        {
            int count = 0;

            foreach (ContainerRow containerRow in allContainerRows)
            {
                count += containerRow.numberOfFreeContainerSpaces();
            }

            return count;
        }

        /// <summary>
        /// Docks ship to loading dock
        /// </summary>
        /// <param name="shipID">unique ID of specific ship</param>
        /// <param name="currentTime">Time the ship is docked</param>
        /// <returns>Returns the Guid of the dock the ship gets docked to</returns>
        internal Guid DockShipToLoadingDock(Guid shipID, DateTime currentTime)
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

            return Guid.Empty; 
        }


        /// <summary>
        /// Transfering ship from dock to loading dock
        /// </summary>
        /// <param name="shipID">Unique ID of specific ship</param>
        /// <param name="currentTime">Time ship is transfered</param>
        /// <returns>Returns the Guid of the loading dock the ship gets docked to</returns>
        internal Guid DockShipFromShipDockToLoadingDock(Guid shipID, DateTime currentTime)
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

            return Guid.Empty;
        }

        /// <summary>
        /// Ship gets docked to dock
        /// </summary>
        /// <param name="shipID">Unique ID of specific ship</param>
        /// <returns>Returns the Guid of the dock the ship gets docked to</returns>
        internal Guid DockShipToShipDock(Guid shipID)
        {
            Ship shipToBeDocked = GetShipFromLoadingDock(shipID) ?? GetShipFromAnchorage(shipID);
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
        /// Ship in anchorage gets moved to loading dock
        /// </summary>
        /// <param name="shipID">Unique ID of specific ship</param>
        /// <returns>Returns the Guid of the dock the ship gets docked to</returns>

        internal Guid StartShipInLoadingDock(Guid shipID)
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
                freeLoadingDocks.Remove(dock);
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
        }

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

            throw new ArgumentException("Invalid input. That shipID does not exist.", nameof(shipID));
                    
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
            throw new ArgumentException("Invalid input. That shipID does not exist.", nameof(shipID));
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
            throw new ArgumentException("Invalid input. That shipID does not exist.", nameof(shipID));
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

            throw new ArgumentException("Invalid input. That shipID does not exist, can't retrieve loadingDock.", nameof(shipID));
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
                ships.Add(item.Key);
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
            throw new ArgumentException("Invalid input. That shipSize does not exist. Valid shipSize is: shipSize.Small, shipSize.Medium or shipSize.Large.", nameof(shipSize));
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

            throw new ArgumentException("Invalid input. That shipSize does not exist. Valid shipSize is: shipSize.Small, shipSize.Medium or shipSize.Large.", nameof(shipSize));
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
        }

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
        }

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
        }

        /// <summary>
        /// Counts the number of available containerspaces of specified size
        /// </summary>
        /// <param name="containerSize">The size of the container in Small, Medium om Large</param>
        /// <returns>Returns the total number of available loading containerspaces of specified size</returns>
        internal int NumberOfFreeContainerSpaces(ContainerSize containerSize)
        {
            int count = 0;
            foreach (ContainerSpace containerSpace in allContainerRows)
            {
                if (containerSpace.Size == containerSize && containerSpace.Free == true)
                {
                    count++;
                }
            }
            throw new ArgumentException("Invalid input. That containerSize does not exist. Valid containerSize is: containerSize.Small, containerSize.Medium or containerSize.Large.", nameof(containerSize));
        }

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
            foreach (ContainerSpace containerSpace in freeContainerSpaces[containerSize])
            {

                if (containerSpace.Free == true && containerSpace.Size == containerSize)
                {
                    return containerSpace;
                }
            }
            throw new ArgumentException("Invalid input. That containerSize does not exist. Valid containerSize is: containerSize.Small, containerSize.Medium or containerSize.Large.", nameof(containerSize));

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
            containerToBeUnloaded.AddStatusChangeToHistory(Status.InStorage, currentTime);

            return containerSpace.ID;

        }

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
            containerToBeLoaded.DaysInStorage = 0;
            containerToBeLoaded.CurrentPosition = ship.ID;

            containerToBeLoaded.AddStatusChangeToHistory(Status.Transit, currentTime);

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

        /// <summary>
        /// Gets last registered status of specific ship
        /// </summary>
        /// <param name="ShipID">Unique ID of specific ship</param>
        /// <returns>Returns last registered status of specified ship if the ship has a history, or returns none</returns>
        public Status GetShipStatus(Guid ShipID)
        {
            StatusLog lastStatusChange = null;
            StringBuilder sb = new StringBuilder();
            foreach (Ship ship in AllShips)
            {
                if (ship.ID == ShipID && ship.HistoryIList != null && ship.HistoryIList.Count > 0)
                {
                    String shipStatus = $"ShipId: {ship.ID}, Last status Change: {lastStatusChange}";
                    sb.Append(shipStatus);
                }

            }
            return Status.None;
        }

        /// <summary>
        /// Gets last registered status of all ships
        /// </summary>
        /// <returns>Returns last registered status of all ships</returns>
        public Dictionary<Ship, Status> GetStatusAllShips()
        {
            Dictionary<Ship, Status> shipStatus = new Dictionary<Ship, Status>();
            
            foreach (Ship ship in AllShips)
            {
                StatusLog test = ship.HistoryIList.Last();
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
                    containerStatus[container] = container.HistoryIList.Last().Status;

                    foreach (var keyvalue in containerStatus)
                    {
                        sb.Append($"ContainerId: {keyvalue.Key}, containerStatus: {keyvalue.Value}");
                    }
                }
                else if (container.ID != ContainerId)
                {
                    throw new ArgumentException("Invalid input. Container with that ID does not exist", nameof(ContainerId));
                }
            }
            return sb.ToString();


        }

        /// <summary>
        /// Gets the last registered status of all containers
        /// </summary>
        /// <returns>Returns a string with the container ID and their last registered status</returns>
        public string GetAllContainerStatus()
        {
            StringBuilder sb = new StringBuilder();
            Dictionary<Container, Status> containerStatus = new Dictionary<Container, Status>();
            Status lastStatus = Status.None;
            foreach (Container container in storedContainers.Keys)
            {
                if (container != null && container.HistoryIList != null && container.HistoryIList.Count > 0)
                {
                    lastStatus = container.HistoryIList.Last().Status;
                    containerStatus[container] = lastStatus;

                    foreach (var keyvalue in containerStatus)
                    {
                        sb.AppendLine($"ContainerId: {keyvalue.Key}, containerStatus: {keyvalue.Value}");
                    }
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Checks if loading dock is free/available for all dock sizes
        /// </summary>
        /// <returns>Returns all the docks or null</returns>
        public IDictionary<Guid, bool> LoadingDockIsFreeForAllDocks()
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
        bool IHarbor.LoadingDockIsFree(Guid dockID)
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
        public IDictionary<Guid, bool> ShipDockIsFreeForAllDocks()
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
                StatusLog test = ship.HistoryIList.Last();
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
                Container containerToBeStored = new Container(ContainerSize.Half, 10, this.ID);

                if (i % 3 == 0)
                {
                    containerToBeStored = new Container(ContainerSize.Half, 10, this.ID);
                }
                if (i % 3 == 1)
                {
                    containerToBeStored = new Container(ContainerSize.Medium, 15, this.ID);
                }
                if (i % 3 == 2)
                {
                    containerToBeStored = new Container(ContainerSize.Full, 15, this.ID);
                }

                ContainerSize containerSize = containerToBeStored.Size;
                ContainerSpace containerSpace = GetFreeContainerSpace(containerSize);

                freeContainerSpaces[containerSize].Remove(containerSpace);

                storedContainers.Add(containerToBeStored, containerSpace);

                containerSpace.storedContainer = containerToBeStored.ID;
                containerSpace.Free = false;

                containerToBeStored.CurrentPosition = containerSpace.ID;
                containerToBeStored.AddStatusChangeToHistory(Status.InStorage, currentTime);
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

       

        /// <summary>
        /// Returns a string value containing information about the harbour, its ships and container spaces.
        /// </summary>
        /// <returns>String value containing information about the harbour, its ships and container spaces.</returns>
        public override string ToString()
        {
            return ($"ID: {ID}, Ships in loading docks: {shipsInLoadingDock.Count}, Free loading docks: {freeLoadingDocks.Count}, Ships in ship docks: {shipsInShipDock.Count}, Free ship docks: {freeShipDocks.Count}, " +
                $", Ships in anchorage: {Anchorage.Count}, Ships in transit: {ShipsInTransit.Count}, Containers stored in harbor: {storedContainers.Count}, Free harbor container spaces: {freeContainerSpaces.Count}");
        }
    }
}
       

               
