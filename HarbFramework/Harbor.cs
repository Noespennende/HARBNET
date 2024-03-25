using Gruppe8.HarbNet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Drawing;
using System.Linq;
using System.Numerics;
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
        internal IList<LoadingDock> allLoadingDocks = new List<LoadingDock>();

        /// <summary>
        /// Gets all available loading docks
        /// </summary>
        /// <return>Returns a list of all available loading docks</return>
        internal IList<LoadingDock> freeLoadingDocks = new List<LoadingDock>();

        /// <summary>
        /// Gets all ships in loading dock
        /// </summary>
        /// <return>Returns a dictionary with all the ships that are in a loading dock</return>
        internal IDictionary<Ship, LoadingDock> shipsInLoadingDock = new Dictionary<Ship, LoadingDock>(); // Ship : Dock

        /// <summary>
        /// Gets all ship docks
        /// </summary>
        /// <return>Returns a list of all ship docks</return>
        internal IList<ShipDock> allShipDocks = new List<ShipDock>();

        /// <summary>
        /// Gets all available ship docks
        /// </summary>
        /// <return>Return a list of all available ship docks</return>
        internal IList<ShipDock> freeShipDocks = new List<ShipDock>();

        /// <summary>
        /// Gets all ships in ship dock
        /// </summary>
        /// <return>Returns a dictionary with all ships in ship dock</return>
        internal IDictionary<Ship, ShipDock> shipsInShipDock = new Dictionary<Ship, ShipDock>(); // Ship : Dock

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
        internal IList<ContainerStorageRow> allContainerRows { get; set; }

        /// <summary>
        /// Gets all stored containers
        /// </summary>
        /// <return>Returns a dictionary of all stored containers</return>
        internal IDictionary<Container, ContainerStorageRow> storedContainers = new Dictionary<Container, ContainerStorageRow>(); // Container : ContainerRow

        internal IList<Crane> HarborStorageAreaCranes { get; set; } = new List<Crane>();
        internal IList<Crane> DockCranes { get; set; } = new List<Crane>();

        internal IList<Adv> AdvWorking { get; set; } = new List<Adv>();

        internal IList<Adv> AdvFree { get; set; } = new List<Adv>();

        internal IList<Truck> TrucksInTransit { get; set; } = new List<Truck>();

        internal IList<Truck> TrucksInQueue { get; set; } = new List<Truck>();

        internal double PercentOfContainersDirectlyLoadedFromShips { get; set; }
        internal double PercentOfContainersDirectlyLoadedFromStorageArea { get; set; }

        internal int TrucksArrivePerHour { get; set; }

        internal int LoadsPerAdvPerHour { get; set; }

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
        public Guid TruckQueueLocationID { get; } = Guid.NewGuid();
        public Guid HarborStorageAreaID { get; } = Guid.NewGuid();
        public Guid HarborDockAreaID { get; } = Guid.NewGuid();


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
        public Harbor(IList<Ship> listOfShips, IList<ContainerStorageRow> listOfContainerStorageRows, int numberOfSmallLoadingDocks, int numberOfMediumLoadingDocks, int numberOfLargeLoadingDocks,
            int numberOfCranesNextToLoadingDocks, int LoadsPerCranePerHour, int numberOfCranesOnHarborStorageArea,
            int numberOfSmallShipDocks, int numberOfMediumShipDocks, int numberOfLargeShipDocks, int numberOfTrucksArriveToHarborPerHour,
            int percentageOfContainersDirectlyLoadedFromShipToTrucks, int percentageOfContainersDirectlyLoadedFromHarborStorageToTrucks,
            int numberOfAdv, int loadsPerAdvPerHour)
        {

            this.TrucksArrivePerHour = numberOfTrucksArriveToHarborPerHour;
            this.allContainerRows = listOfContainerStorageRows.ToList();
            this.LoadsPerAdvPerHour = loadsPerAdvPerHour;

            if (percentageOfContainersDirectlyLoadedFromShipToTrucks > 100 || percentageOfContainersDirectlyLoadedFromShipToTrucks < 0)
            {
                throw new ArgumentOutOfRangeException("percentageOfContainersDirectlyLoadedFromShipToTrucks must be a number between 0 and 100");
            }

            if (percentageOfContainersDirectlyLoadedFromHarborStorageToTrucks > 100 || percentageOfContainersDirectlyLoadedFromHarborStorageToTrucks < 0)
            {
                throw new ArgumentOutOfRangeException("percentageOfContainersDirectlyLoadedFromHarborStorageToTrucks must be a number between 0 and 100");
            }

            this.PercentOfContainersDirectlyLoadedFromShips = (percentageOfContainersDirectlyLoadedFromShipToTrucks / 100);
            this.PercentOfContainersDirectlyLoadedFromStorageArea = (percentageOfContainersDirectlyLoadedFromHarborStorageToTrucks / 100);

            for (int i = 0; i < numberOfCranesNextToLoadingDocks; i++)
            {
                DockCranes.Add(new Crane(LoadsPerCranePerHour, HarborDockAreaID));
            }

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

            for (int i = 0; i < numberOfAdv; i++)
            {
                AdvFree.Add(new(HarborDockAreaID));
            }

            
            for (int i = 0; i < numberOfSmallLoadingDocks; i++)
            {
                allLoadingDocks.Add(new LoadingDock(ShipSize.Small));
            }

            for (int i = 0; i < numberOfMediumLoadingDocks; i++)
            {
                allLoadingDocks.Add(new LoadingDock(ShipSize.Medium));
            }
            for (int i = 0; i < numberOfLargeLoadingDocks; i++)
            {
                allLoadingDocks.Add(new LoadingDock(ShipSize.Large));
            }

            for (int i = 0; i < numberOfSmallShipDocks; i++)
            {
                allShipDocks.Add(new ShipDock(ShipSize.Small));
            }

            for (int i = 0; i < numberOfMediumShipDocks; i++)
            {
                allShipDocks.Add(new ShipDock(ShipSize.Medium));
            }
            for (int i = 0; i < numberOfLargeShipDocks; i++)
            {
                allShipDocks.Add(new ShipDock(ShipSize.Large));
            }

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
        internal void CreateContainerSpaces(int numberOfContainerSpaces, int numberOfContainerRows)
        {
            IList <ContainerStorageRow> containerRows = new List <ContainerStorageRow>();

            for (int j = 0; j < numberOfContainerRows; j++)
            {
                containerRows.Add(new ContainerStorageRow(numberOfContainerSpaces));
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
            if (!(crane.Container == null))
            {
                throw new CraneCantBeLoadedExeption("The crane you are trying to load already holds a container in its cargo and therefore can not load another one from the ship.");
            }
            Container containerToBeLoaded = ship.UnloadContainer();
            containerToBeLoaded.CurrentPosition = ship.CurrentLocation;
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
            containerToBeLoaded.CurrentPosition = crane.Location;
            containerToBeLoaded.AddStatusChangeToHistory(Status.Loading, currentTime);
            ship.AddContainer(containerToBeLoaded);
            
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
            if (!TrucksInQueue.Contains(truck))
            {
                if (TrucksInTransit.Contains(truck))
                {
                    throw new TruckCantBeLoadedExeption("The truck you are trying to load is already in transit away from the harbor and therefore can't load the container from the crane");
                } else
                {
                    throw new TruckCantBeLoadedExeption("The truck you are trying to load does not exist in the simulation and therefore can't load the container from the crane.");
                }
                
            }

            if (!(truck.Container == null))
            {
                throw new TruckCantBeLoadedExeption("The truck you are trying to load already has a container in its storage and therefore don't have room for the container from the given crane");
            }

            Container containerToBeLoaded = crane.UnloadContainer();
            containerToBeLoaded.CurrentPosition = crane.Location;
            truck.LoadContainer(containerToBeLoaded);
            containerToBeLoaded.AddStatusChangeToHistory(Status.LoadingToTruck, currentTime);

            return containerToBeLoaded;
        }

        internal Crane? GetFreeLoadingDockCrane()
        {
            foreach (Crane crane in DockCranes)
            {
                if (crane.Container == null)
                {
                    return crane;
                }
            }
            return null;
        }

        internal void RemoveTruckFromQueue(Truck truck)
        {
            TrucksInQueue.Remove(truck);
        }

        internal void SendTruckOnTransit(LoadingDock loadingDock, Container container)
        {
            Truck? truck = null;

            foreach (var pair in loadingDock.TruckLoadingSpots)
            {
                if (pair.Value.Container == container)
                {
                    truck = pair.Value; 
                    break; 
                }
            }
            
            if (truck == null)
            {
                throw new NullReferenceException("Did not find truck containing the given container");
                // EXCEPTION?
            }

            if (!loadingDock.TruckExistsInTruckLoadingSpots(truck))
            {
                if (TrucksInTransit.Contains(truck))
                {
                    // exception når trucken allerede er i transit ??

                }
                else
                {
                    // exception når trucken ikke finnes i truckloadingspot ??

                }
            }
            else if (truck.Container != null)
            {
                TrucksInTransit.Add(truck);
            }
            
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
            if (!AdvFree.Contains(adv))
            {
                if (AdvWorking.Contains(adv))
                {
                    throw new AdvCantBeLoadedExeption("The ADV you are trying to load is already transporting goods and therefore can not load a container from the crane.");
                } else
                {
                    throw new AdvCantBeLoadedExeption("The ADV you are trying to load does not exist in the simulation and therefore can't be loaded.");
                }
                
            }

            if (!(adv.Container == null))
            {
                throw new AdvCantBeLoadedExeption("The Adv given already has a container in its storage and therefore has no room for the container the crane is trying to load.");
            }
            Container containerToBeLoaded = crane.UnloadContainer();
            containerToBeLoaded.CurrentPosition = crane.Location;
            adv.LoadContainer(containerToBeLoaded);
            containerToBeLoaded.AddStatusChangeToHistory(Status.LoadingToAdv, currentTime);

            AdvFree.Remove(adv);
            AdvWorking.Add(adv);

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
            if (adv.Container == null)
            {
                throw new CraneCantBeLoadedExeption("The ADV you are trying to unload doesn't have a container in its storage and therefore can't unload to the crane.");
            }

            if (!AdvWorking.Contains(adv))
            {
                if (AdvFree.Contains(adv))
                {
                    throw new CraneCantBeLoadedExeption("The ADV you are trying to unload is set as free and therefore is not working to unload cargo. ADVs must be working for cargo to be unloaded.");
                } else
                {
                    throw new CraneCantBeLoadedExeption("The ADV you are trying to unload does not exist within the simulation and therefore can not unload to the crane");
                }
            }

            if (!(crane.Container == null))
            {
                throw new CraneCantBeLoadedExeption("The crane you are trying to load already has a container in its storage and therefore has no room to load the container from the ADV");
            }

            Container containerToBeLoaded = adv.UnloadContainer();
            containerToBeLoaded.CurrentPosition = adv.Location;
            crane.LoadContainer(containerToBeLoaded);
            containerToBeLoaded.AddStatusChangeToHistory(Status.LoadingToCrane, currentTime);

            AdvWorking.Remove(adv);
            AdvFree.Add(adv);

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

            foreach (ContainerStorageRow CR in allContainerRows)
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
        internal Container ContainerRowToCrane(ContainerSize size, Crane crane,DateTime currentTime)
        {
            if (!(crane.Container == null))
            {
                throw new CraneCantBeLoadedExeption("The crane you are trying to load already has a container in its storage and therefore has no room to load the container from the harbor storage area");
            }

            foreach (Container container in storedContainers.Keys){
                if (container.Size == size)
                {
                    crane.LoadContainer(container);
                    container.AddStatusChangeToHistory(Status.LoadingToCrane, currentTime );
                    storedContainers[container].RemoveContainerFromContainerRow(container);
                    storedContainers.Remove(container);
                    
                    return container;
                }
            }
            return null;
        }

        internal Adv GetFreeAdv()
        {
            if (AdvFree.Count > 0)
            {
                return AdvFree[0];
            }

            return null;
        }

        internal Truck GetFreeTruck()
        {
            if (TrucksInQueue.Count > 0)
            {
                return TrucksInQueue[0];
            }

            return null;
        }

        internal void GenerateTrucks()
        {
            for (int i = 0; i < TrucksArrivePerHour; i++)
            {
                TrucksInQueue.Add(new Truck(TruckQueueLocationID));
            }
        }

        internal Crane? GetFreeStorageAreaCrane()
        {
            foreach (Crane crane in HarborStorageAreaCranes)
            {
                if (crane.Container == null)
                {
                    return crane;
                }
            }
            return null;
        }

        internal Adv? GetAdvContainingContainer(Container container)
        {
            foreach (Adv adv in AdvWorking)
            {
                if (adv.Container == container)
                {
                    return adv;
                }
            }

            return null;
        }

        /// <summary>
        /// finds the number of free container spaces
        /// </summary>
        /// <returns>an int with how many free spaces there are</returns>
        internal int numberOfFreeContainerSpaces (ContainerSize size)
        {
            int count = 0;

            foreach (ContainerStorageRow containerRow in allContainerRows)
            {
                count += containerRow.numberOfFreeContainerSpaces(size);
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
            LoadingDock loadingDock;

            if (FreeLoadingDockExists(size))
            {
                loadingDock = GetFreeLoadingDock(size);

                loadingDock.DockedShip = shipToBeDocked.ID;
                loadingDock.Free = false;

                shipToBeDocked.CurrentLocation = loadingDock.ID;

                shipsInLoadingDock.Add(shipToBeDocked, loadingDock);

                RemoveShipFromAnchorage(shipToBeDocked.ID);
                RemoveLoadingDockFromFreeLoadingDocks(loadingDock.ID);
               
                return loadingDock.ID;
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
            LoadingDock loadingDock;

            if (FreeLoadingDockExists(size))
            {
                loadingDock = GetFreeLoadingDock(size);

                loadingDock.DockedShip = shipToBeDocked.ID;
                loadingDock.Free = false;

                shipToBeDocked.CurrentLocation = loadingDock.ID;

                shipsInLoadingDock.Add(shipToBeDocked, loadingDock);

                UnDockShipFromShipDockToLoadingDock(shipID);

                RemoveLoadingDockFromFreeLoadingDocks(loadingDock.ID);
                
                return loadingDock.ID;
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
            LoadingDock loadingDock = GetLoadingDockContainingShip(shipID);
            ShipSize size = shipToBeDocked.ShipSize;
            ShipDock shipDock;
            
            if (FreeShipDockExists(size))
            {

                shipDock = GetFreeShipDock(size);
                shipDock.DockedShip = shipToBeDocked.ID;
                shipDock.Free = false;

                shipToBeDocked.CurrentLocation = shipDock.ID;
                
                shipsInShipDock.Add(shipToBeDocked, shipDock);
                shipsInLoadingDock.Remove(shipToBeDocked);
                freeLoadingDocks.Add(loadingDock);
                loadingDock.DockedShip = Guid.Empty;
                loadingDock.Free = true;

                freeShipDocks.Remove(shipDock);

                return shipDock.ID;
                
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
            LoadingDock loadingDock;

            if (FreeLoadingDockExists(size))
            {
                loadingDock = GetFreeLoadingDock(size);
                loadingDock.DockedShip = shipToBeDocked.ID;
                loadingDock.Free = false;
                shipToBeDocked.CurrentLocation = loadingDock.ID;

                shipsInLoadingDock.Add(shipToBeDocked, loadingDock);
                freeLoadingDocks.Remove(loadingDock);
                RemoveShipFromAnchorage(shipID);

                return loadingDock.ID;

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
                LoadingDock loadingDock = shipsInLoadingDock[shipToBeUndocked];

                loadingDock.DockedShip = Guid.Empty;
                loadingDock.Free = true;

                shipToBeUndocked.CurrentLocation = TransitLocationID;
        

                shipsInLoadingDock.Remove(shipToBeUndocked);
                freeLoadingDocks.Add(loadingDock);
                if (!ShipsInTransit.ContainsKey(shipToBeUndocked))
                {
                    ShipsInTransit.Add(shipToBeUndocked, shipToBeUndocked.RoundTripInDays);
                  
                }
                return loadingDock.ID;
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
        internal LoadingDock GetLoadingDockContainingShip(Guid shipID)
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
            foreach (LoadingDock loadingDock in freeLoadingDocks)
            {
                if (loadingDock.Free == true && loadingDock.Size == shipSize)
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
            foreach (ShipDock shipDock in freeShipDocks)
            {
                if (shipDock.Free == true && shipDock.Size == shipSize)
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
        internal LoadingDock GetFreeLoadingDock(ShipSize shipSize)
        {

            foreach (LoadingDock loadingDock in freeLoadingDocks)
            {
                if (loadingDock.Free == true && loadingDock.Size == shipSize)
                {
                    return loadingDock;
                }

            }
            return null;
            //throw new ArgumentException("Invalid input. That shipSize does not exist. Valid shipSize is: shipSize.Small, shipSize.Medium or shipSize.Large.", nameof(shipSize));
        }

        /// <summary>
        /// Gets all free ship dock matching the specified shipsize
        /// </summary>
        /// <param name="shipSize">The size of the ship in Small, Medium om Large</param>
        /// <returns>Returns available ship docks</returns>
        internal ShipDock GetFreeShipDock(ShipSize shipSize)
        {

            foreach (ShipDock shipDock in freeShipDocks)
            {
                if (shipDock.Free == true && shipDock.Size == shipSize)
                {
                    return shipDock;
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
                LoadingDock loadingDock = shipsInLoadingDock[shipToBeUndocked];

                loadingDock.DockedShip = Guid.Empty;
                loadingDock.Free = true;

                ShipDock shipDock = GetFreeShipDock(shipToBeUndocked.ShipSize);

                shipToBeUndocked.CurrentLocation = shipDock.ID;
                shipsInLoadingDock.Remove(shipToBeUndocked);
                freeLoadingDocks.Add(loadingDock);
                if (!shipsInShipDock.ContainsKey(shipToBeUndocked))
                {
                    shipsInShipDock.Add(shipToBeUndocked, shipDock);
                }

                return loadingDock.ID;
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
                ShipDock shipDock = shipsInShipDock[shipToBeUndocked];

                shipDock.DockedShip = Guid.Empty;
                shipDock.Free = true;

                LoadingDock loadingDock = GetFreeLoadingDock(shipToBeUndocked.ShipSize);

                shipToBeUndocked.CurrentLocation = loadingDock.ID;
                shipsInShipDock.Remove(shipToBeUndocked);
                freeShipDocks.Add(shipDock);
                if (!shipsInLoadingDock.ContainsKey(shipToBeUndocked))
                {
                    shipsInLoadingDock.Add(shipToBeUndocked, loadingDock);
                }

                return shipDock.ID;
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
            foreach (LoadingDock loadingDock in freeLoadingDocks)
            {
                if (loadingDock.ID == dockID)
                {
                    freeLoadingDocks.Remove(loadingDock);
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
            foreach (LoadingDock loadingDock in freeLoadingDocks)
            {
                if (loadingDock.Free == true && loadingDock.Size == shipSize)
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
            if (containerSize == ContainerSize.None)
            {
                throw new ArgumentException("Invalid input. That containerSize does not exist. Valid containerSize is: ContainerSize.Half or ContainerSize.Full", nameof(containerSize));
            }

            int count = 0;
            foreach (ContainerStorageRow containerRow in allContainerRows)
            {
                count += containerRow.numberOfFreeContainerSpaces(containerSize);
            }

            return count;
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
        internal ContainerStorageRow GetContainerRowWithFreeSpace(ContainerSize containerSize)
        {
            foreach (ContainerStorageRow containerRow in allContainerRows)
            {
                if (containerRow.CheckIfFreeContainerSpaceExists(containerSize))
                {
                    return containerRow;
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
        /// Unloads container of specific size from ship to containerspace
        /// </summary>
        /// <param name="containerSize">The size of the container in Small, Medium om Large</param>
        /// <param name="ship">A ship object</param>
        /// <param name="currentTime">Time container is unloaded</param>
        /// <returns>Returns Guid to the containerspaces the unloaded container was stored, returns empty if container did not exist</returns>
        internal Guid UnloadContainer(ContainerSize containerSize, Ship ship, DateTime currentTime)
        {
            Container containerToBeUnloaded = ship.GetContainer(containerSize);
            ContainerStorageRow containerRow = GetContainerRowWithFreeSpace(containerSize);
            
            if (containerToBeUnloaded == null || containerRow == null)
            {
                return Guid.Empty;
            }

            ship.RemoveContainer(containerToBeUnloaded.ID);
            storedContainers.Add(containerToBeUnloaded, containerRow);

            ContainerSpace containerSpace = containerRow.AddContainerToFreeSpace(containerToBeUnloaded);
            containerToBeUnloaded.CurrentPosition = containerSpace.ID;
            containerToBeUnloaded.AddStatusChangeToHistory(Status.InStorage, currentTime);

            return containerSpace.ID;

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
        /// Gets the status of all shipdocks
        /// </summary>
        /// <returns>Returns the status of all shipdocks</returns>
        public Dictionary<Guid, bool> StatusAllShipDocks()
        {
            Dictionary<Guid, bool> dockStatus = new Dictionary<Guid, bool>();
            foreach(ShipDock shipDock in allShipDocks)
            {
                dockStatus[shipDock.ID] = shipDock.Free;
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
            foreach (LoadingDock loadingDock in allLoadingDocks)
            {
                if (dockID == loadingDock.ID)
                {
                    dockFree = loadingDock.Free;
                    String dockStatus = $"DockId: {loadingDock.ID}, dock free: {dockFree}";
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
            Dictionary<LoadingDock, bool> dockStatus = new Dictionary<LoadingDock, bool>();

            foreach (LoadingDock loadingDock in allLoadingDocks)
            {
                dockStatus[loadingDock] = loadingDock.Free;

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

            foreach (LoadingDock loadingDock in allLoadingDocks)
            {
                if (loadingDock.Free == true)
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
            foreach (LoadingDock loadingDock in allLoadingDocks)
            {
                if (dockID == loadingDock.ID)
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

            foreach (ShipDock shipDock in allShipDocks)
            {
                if (shipDock.Free == true)
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
        public IDictionary<Ship, Status> GetStatusAllShips()
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
                $", Ships in anchorage: {Anchorage.Count}, Ships in transit: {ShipsInTransit.Count}, Containers stored in harbor: {storedContainers.Count}");
        }
    }
}
       

               
