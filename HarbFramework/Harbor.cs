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
        /// Gets the unique ID for the harbor.
        /// </summary>
        /// <return>Returns a Guid object representing the harbors unique ID.</return>
        public Guid ID { get; internal set; } = Guid.NewGuid();
        /// <summary>
        /// Gets all loading docks.
        /// </summary>
        /// <return>Returns an IList with LoadingDock objects representing the loadingdocks in the harbor.</return>
        internal IList<LoadingDock> allLoadingDocks = new List<LoadingDock>();
        /// <summary>
        /// Gets all available loading docks.
        /// </summary>
        /// <return>Returns an Ilist with LoadingDock objects representing the available loadingdocks in the harbor.</return>
        internal IList<LoadingDock> freeLoadingDocks = new List<LoadingDock>();
        /// <summary>
        /// Gets all ships in loading dock.
        /// </summary>
        /// <return>Returns a dictionary with all ships in loadingdocks.</return>
        internal IDictionary<Ship, LoadingDock> shipsInLoadingDock = new Dictionary<Ship, LoadingDock>(); // Ship : Dock
        /// <summary>
        /// Gets all ship docks.
        /// </summary>
        /// <return>Returns an IList with ShipDick objects representing the shipdocks in the harbor.</return>
        internal IList<ShipDock> allShipDocks = new List<ShipDock>();
        /// <summary>
        /// Gets all available ship docks.
        /// </summary>
        /// <return>Returns an Ilist with ShipDock objects representing the available shipdocks in the harbor.</return>
        internal IList<ShipDock> freeShipDocks = new List<ShipDock>();
        /// <summary>
        /// Gets all ships in ship dock.
        /// </summary>
        /// <return>Returns a dictionary with all ships in ship docks.</return>
        internal IDictionary<Ship, ShipDock> shipsInShipDock = new Dictionary<Ship, ShipDock>(); // Ship : Dock
        /// <summary>
        /// Gets all ships in anchorage.
        /// </summary>
        /// <return>Returns an Ilist with Ship objects representing the ships in anchorage in the harbor.</return>
        internal IList<Ship> Anchorage { get; } = new List<Ship>();
        /// <summary>
        /// Gets all ships in transit.
        /// </summary>
        /// <return>Returns a dictionary with all ships in transit.</return>
        internal IDictionary<Ship, int> ShipsInTransit { get; } = new Dictionary<Ship, int>(); // ship: int number of days until return
        /// <summary>
        /// Gets all ships.
        /// </summary>
        /// <return>Returns a list of all ships</return>
        internal IList<Ship> AllShips { get; } = new List<Ship>();
        /// <summary>
        /// Gets all container spaces.
        /// </summary>
        /// <return>Returns an Ilist with ContainerStorageRow objects representing the containerRows in the harbor.</return>
        internal IList<ContainerStorageRow> allContainerRows { get; set; }
        /// <summary>
        /// Gets all stored containers.
        /// </summary>
        /// <return>Returns a dictionary of all stored containers.</return>
        internal IDictionary<Container, ContainerStorageRow> storedContainers = new Dictionary<Container, ContainerStorageRow>(); // Container : ContainerRow
        /// <summary>
        /// Gets cranes in storage area.
        /// </summary>
        /// <return>Returns an Ilist with Crane objects representing the cranes in the storage area in the harbor.</return>
       internal IList<Crane> HarborStorageAreaCranes { get; set; } = new List<Crane>();
        /// <summary>
        /// Get all containers that have left the harbor and arived at their destination
        /// </summary>
        /// <return>Returns a IList of all containers that have arrived at their destination during a simulation</return>
        public IList<Container> ArrivedAtDestination { get; internal set; } = new List<Container>();
        /// <summary>
        /// Gets cranes in dock.
        /// </summary>
        /// <return>Returns an Ilist with Crane objects representing the cranes in the dock in the harbor.</return>
        internal IList<Crane> DockCranes { get; set; } = new List<Crane>();
        /// <summary>
        /// Gets the working Advs.
        /// </summary>
        /// <return>Returns an Ilist with Adv objects representing the working Advs in the harbor.</return>
        internal IList<Adv> AdvWorking { get; set; } = new List<Adv>();
        /// <summary>
        /// Gets the available Advs.
        /// </summary>
        /// <return>Returns an Ilist with Adv objects representing the available Advs in the harbor.</return>
        internal IList<Adv> AdvFree { get; set; } = new List<Adv>();
        /// <summary>
        /// Gets the Trucks in transit.
        /// </summary>
        /// <return>Returns an Ilist with Truck objects representing the Trucks in transit in the harbor.</return>
        internal IList<Truck> TrucksInTransit { get; set; } = new List<Truck>();
        /// <summary>
        /// Gets the Trucks in queue.
        /// </summary>
        /// <return>Returns an Ilist with Truck objects representing the Trucks in queue in the harbor.</return>
        internal IList<Truck> TrucksInQueue { get; set; } = new List<Truck>();
        /// <summary>
        /// Gets the percentege of containers loaded directly from the ships.
        /// </summary>
        /// <returns>Returns a double value representing the percentege of the total amount of containers that are directly loaded from the ships.</returns>
        internal double PercentOfContainersDirectlyLoadedFromShips { get; set; }
        /// <summary>
        /// Gets the percentege of containers loaded directly from the storage area.
        /// </summary>
        /// <returns>Returns a double value representing the percentege of the total amount of containers that are directly loaded from the storage area.</returns>
        internal double PercentOfContainersDirectlyLoadedFromStorageArea { get; set; }
        /// <summary>
        /// Gets the amount of trucks that arrive per hour.
        /// </summary>
        /// <returns>Returns an int value representing the amount of trucks in harbor that arrive per hour.</returns>
        internal int TrucksArrivePerHour { get; set; }
        /// <summary>
        /// Gets the amount of loads each adv do per hour.
        /// </summary>
        /// <returns>Returns an int value representing the amount of loads each adv in harbor does per hour.</returns>
        internal int LoadsPerAdvPerHour { get; set; }
        /// <summary>
        /// Gets the unique ID for the transit location.
        /// </summary>
        /// <return>Returns a Guid representing the harbors transit location.</return>
        public Guid TransitLocationID { get; } = Guid.NewGuid();
        /// <summary>
        /// Gets the unique ID for the anchorage.
        /// </summary>
        /// <return>Returns a Guid representing the harbors anchorages.</return>
        public Guid AnchorageID { get; } = Guid.NewGuid();
        /// <summary>
        /// Gets the unique ID for the Adv cargo.
        /// </summary>
        /// <return>Returns a Guid representing the harbors Adv cargos.</return>
        public Guid AdvCargoID { get; } = Guid.NewGuid();
        /// <summary>
        /// Gets the unique ID for the truck transit location.
        /// </summary>
        /// <return>Returns a Guid representing the harbors truck transit locations.</return>
        public Guid TruckTransitLocationID { get; } = Guid.NewGuid();
        /// <summary>
        /// Gets the unique ID for the truck queue location.
        /// </summary>
        /// <return>Returns a Guid representing the harbors truck queue locations.</return>
        public Guid TruckQueueLocationID { get; } = Guid.NewGuid();
        /// <summary>
        /// Gets the unique ID for the harbor storage area.
        /// </summary>
        /// <return>Returns a Guid representing the harbors storage areas.</return>
        public Guid HarborStorageAreaID { get; } = Guid.NewGuid();
        /// <summary>
        /// Gets the unique ID for the dock area.
        /// </summary>
        /// <return>Returns a Guid representing the harbors dock areas.</return>
        public Guid HarborDockAreaID { get; } = Guid.NewGuid();
        /// <summary>
        /// The ID of a containers destination.
        /// </summary>
        /// <return>The ID of a containers destination.</return>
        public Guid DestinationID { get; } = Guid.NewGuid();


        /// <summary>
        /// Creates new harbor object.
        /// </summary>
        /// <param name="listOfShips">List of ships in harbor to be created.</param>
        /// <param name="listOfContainerStorageRows">List of container storage rows in harbor to be created.</param>
        /// <param name="numberOfSmallLoadingDocks">Number of small loading docks in harbor to be created.</param>
        /// <param name="numberOfMediumLoadingDocks">Number of medium loading docks in harbor to be created.</param>
        /// <param name="numberOfLargeLoadingDocks">Number of large loading docks in harbor to be created.</param>
        /// <param name="numberOfCranesNextToLoadingDocks">Number of cranes next to loading docks in harbor to be created.</param>
        /// <param name="LoadsPerCranePerHour">Number of loads per crane per hour in harbor to be created.</param>
        /// <param name="numberOfCranesOnHarborStorageArea">Number of cranes on harbor storage area in harbor to be created.</param>
        /// <param name="numberOfSmallShipDocks">Number of small ship docks in harbor to be created.</param>
        /// <param name="numberOfMediumShipDocks">Number of medium ship docks in harbor to be created.</param>
        /// <param name="numberOfLargeShipDocks">Number of large ship docks in harbor to be created.</param>
        /// <param name="numberOfTrucksArriveToHarborPerHour">Number of truck arriving to the harbor to be created per hour</param>
        /// <param name="percentageOfContainersDirectlyLoadedFromShipToTrucks">Percentage of containers directly loaded from ship to trucks in harbor to be created.</param>
        /// <param name="percentageOfContainersDirectlyLoadedFromHarborStorageToTrucks">Percentage of containers directly loaded from harbor storage to trucks in harbor to be created.</param>
        /// <param name="numberOfAdv">Number of Advs in harbor to be created.</param>
        /// <param name="loadsPerAdvPerHour">Number of loads per adv per hour in harbor to be created.</param>
        /// <exception cref="ArgumentOutOfRangeException">Exception to be thrown in Harbor if parameter is out of set range.</exception>
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
        /// Creates container space in harbor.
        /// </summary>
        /// <param name="numberOfContainerSpaces">Number of container spaces to be created.</param>
        /// <param name="numberOfContainerRows">Number of container rows to be created.</param>
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
        /// Loads container from ship to crane
        /// </summary>
        /// <param name="ship">Ship object container is unloaded from.</param>
        /// <param name="crane">Crane object that loads container.</param>
        /// <param name="currentTime">The current time container is loaded.</param>
        /// <returns>Returns the container to be loaded from ship to crane.</returns>
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
        /// Loads container from crane to ship.
        /// </summary>
        /// <param name="ship">Ship object that loads container.</param>
        /// <param name="crane">Crane object container is unloaded from.</param>
        /// <param name="currentTime">The current time container is loaded.</param>
        /// <returns>Returns container to be loaded from crane to ship.</returns>
        internal Container CraneToShip(Crane crane, Ship ship, DateTime currentTime)
        {
            Container containerToBeLoaded = crane.UnloadContainer();
            containerToBeLoaded.CurrentPosition = crane.Location;
            containerToBeLoaded.AddStatusChangeToHistory(Status.Loading, currentTime);
            ship.AddContainer(containerToBeLoaded);
            
            return containerToBeLoaded;
        }

        /// <summary>
        /// Loads container from crane to truck.
        /// </summary>
        /// <param name="crane">Crane object container is unloaded from.</param>
        /// <param name="truck">Truck object that loads container.</param>
        /// <param name="currentTime">The current time container is loaded.</param>
        /// <returns>Returns container to be loaded from crane to truck.</returns>
        /// <exception cref="TruckCantBeLoadedExeption">Exception to be thrown if truck can't be loaded.</exception>
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

        /// <summary>
        /// Gets available loading dock for cranes.
        /// </summary>
        /// <returns>Returns crane if the crane holds on no container, if crane holds on container then null is returned.</returns>
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

        /// <summary>
        /// Removes truck from queue.
        /// </summary>
        /// <param name="truck">Truck object to be removed from queue.</param>
        internal void RemoveTruckFromQueue(Truck truck)
        {
            TrucksInQueue.Remove(truck);
        }

        /// <summary>
        /// Sends truck on transit.
        /// </summary>
        /// <param name="loadingDock">The loading dock for truck to be sent to transit.</param>
        /// <param name="container">Container to be transported on truck.</param>
        /// <exception cref="NullReferenceException">Exception to be thrown if truck is not found.</exception>
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
                RemoveTruckFromQueue(truck);
                TrucksInTransit.Add(truck);
                truck.Status = Status.Transit;
                truck.Location = TruckTransitLocationID;
            }
            
        }

        /// <summary>
        /// Sends truck on transit.
        /// </summary>
        /// <param name="container">Container to be transported on truck.</param>
        /// <returns>Returns truck object transporting container, if truck is not transporting container null is returned.</returns>
        internal Truck? SendTruckOnTransit(Container container)
        {
            
            foreach (Truck truck in TrucksInQueue)
            {
                if (truck.Container == container && container != null)
                {
                    RemoveTruckFromQueue(truck);
                    TrucksInTransit.Add(truck);
                    truck.Status = Status.Transit;
                    truck.Location = TruckTransitLocationID;

                    return truck;
                }
                
            }
            return null;
        }

        /// <summary>
        /// Amount of container in storage to ships.
        /// </summary>
        /// <returns>Returns an int value representing the amount of containers in storage to ships.</returns>
        internal int NumberOfContainersInStorageToShips()
        {
            int numberOfContainersToShips = storedContainers.Count - NumberOfContainersInStorageToTrucks();

            return numberOfContainersToShips;
        }

        /// <summary>
        /// Amount of containers in storage to trucks.
        /// </summary>
        /// <returns>Returns an int value representing the amount of containers in storage to trucks.</returns>
        internal int NumberOfContainersInStorageToTrucks()
        {
            double percentTrucks = 0.10; // PercentOfContainersDirectlyLoadedFromStorageArea;

            double decimalNumberOfContainers = storedContainers.Count * percentTrucks;

            double decimalPart = decimalNumberOfContainers - Math.Floor(decimalNumberOfContainers);

            int numberOfContainersToTrucks;

            if (decimalPart < 0.5)
            {
                numberOfContainersToTrucks = (int)Math.Floor(decimalNumberOfContainers);
            }
            else
            {
                numberOfContainersToTrucks = (int)Math.Ceiling(decimalNumberOfContainers);
            }

            return numberOfContainersToTrucks;
        }

        /// <summary>
        /// Loads container from Crane to Adv
        /// </summary>
        /// <param name="crane">Crane object container is unloaded from.</param>
        /// <param name="adv">Adv object that loads container.</param>
        /// <param name="currentTime">The current time container is loaded.</param>
        /// <returns>Returns container to be loaded from crane to adv.</returns>
        /// <exception cref="AdvCantBeLoadedExeption">Exception to be thrown if Adv can't be loaded.</exception>
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
        /// Loads container from Adv to Crane.
        /// </summary>
        /// <param name="crane">Crane object that loads container.</param>
        /// <param name="adv">Adv object container is unloaded from.</param>
        /// <param name="currentTime">The current time container is loaded.</param>
        /// <returns>Returns container to be loaded from crane to adv.</returns>
        /// <exception cref="CraneCantBeLoadedExeption">Exception the be thrown if Crane can't be loaded.</exception>
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
        /// Loads container from crane to container storage if there is available space.
        /// </summary>
        /// <param name="crane">Crane object container is unloaded from</param>
        /// <param name="currentTime">The current time container is loaded.</param>
        /// <returns>Returns True if there is available space for the container and it's size to be loaded to, or false if nothing is available.</returns>
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
        /// Loads container from container row to crane.
        /// </summary>
        /// <param name="size">the size of the container to be loaded</param>
        /// <param name="crane">Crane object that loads container.</param>
        /// <param name="currentTime">The current time container is loaded.</param>
        /// <returns>Returns container to be loaded from container row to crane, if container is not found null is returned.</returns>
        /// <exception cref="CraneCantBeLoadedExeption">Throws exception if crane can't be loaded.</exception>
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

        /// <summary>
        /// Gets available adv.
        /// </summary>
        /// <returns>Returns available adv found, if none is available null is returned.</returns>
        internal Adv GetFreeAdv()
        {
            if (AdvFree.Count > 0)
            {
                return AdvFree[0];
            }

            return null;
        }

        /// <summary>
        /// Gets avaulable truck.
        /// </summary>
        /// <returns>Returns available truck found, if none is available null is returned.</returns>
        internal Truck? GetFreeTruck()
        {
            if (TrucksInQueue.Count > 0)
            {
                return TrucksInQueue[0];
            }

            return null;
        }

        /// <summary>
        /// Creates new truck objects.
        /// </summary>
        internal void GenerateTrucks()
        {
            for (int i = 0; i < TrucksArrivePerHour; i++)
            {
                TrucksInQueue.Add(new Truck(TruckQueueLocationID));
            }
        }

        /// <summary>
        /// Gets available crane from storage area.
        /// </summary>
        /// <returns>Returns available crane found in storage area, if none is available null is returned.</returns>
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

        /// <summary>
        /// Gets adv containing a container.
        /// </summary>
        /// <param name="container">Container to be checked if held by adv.</param>
        /// <returns>Returns adv if it holds an container, if it's not holding an container then null is returned</returns>
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
        /// Finds the number of free container spaces.
        /// </summary>
        /// <returns>Returns an int value representing how many free spaces there are.</returns>
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
        /// Docks ship to loading dock.
        /// </summary>
        /// <param name="shipID">unique ID of ship to be docked</param>
        /// <param name="currentTime">The current time ship is docked.</param>
        /// <returns>Returns a Guid object representing the dock the ship gets docked to, if correct dock does not exist nothing is returned.</returns>
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
        /// Transfering ship from dock to loading dock.
        /// </summary>
        /// <param name="shipID">Unique ID of ship to be transfered.</param>
        /// <param name="currentTime">The current time ship is transfered.</param>
        /// <returns>Returns a Guid object representing the loading dock the ship gets docked to, if correct dock does not exist nothing is returned.</returns>
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
        /// Ship gets docked to dock.
        /// </summary>
        /// <param name="shipID">Unique ID of the ship to be docked.</param>
        /// <returns>Returns a Guid object representing the dock the ship gets docked to, if correct dock does not exist nothing is returned.</returns>
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
        /// Ship in anchorage gets moved to loading dock.
        /// </summary>
        /// <param name="shipID">Unique ID of ship to be moved to loading dock.</param>
        /// <returns>Returns a Guid object representing the dock the ship gets docked to, if correct dock does not exist nothing is returned.</returns>

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
        /// Ship in loading dock got get moved to dock for ships in transit.
        /// </summary>
        /// <param name="shipID">Unique ID of ship to be moved to transit.</param>
        /// <param name="currentTime">The current time ship is transfered.</param>
        /// <returns>Returns a Guid object representing the dock the ship gets docked from, if correct dock does not exist nothing is returned.</returns>
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
        /// Undock Ship from Anchorage to Transit.
        /// </summary>
        /// <param name="shipID">Unique ID of specific ship.</param>
        /// <returns>Returns the Guid of the Anchorage the ship was undocked from.</returns>
        internal Guid UnDockShipFromAnchorageToTransit(Guid shipID)
        {
            Ship shipToBeUndocked = GetShipFromAnchorage(shipID);

            if (shipToBeUndocked != null)
            {
                bool removed = RemoveShipFromAnchorage(shipID);

                if (removed)
                {
                    if (!ShipsInTransit.ContainsKey(shipToBeUndocked))
                    {
                        ShipsInTransit.Add(shipToBeUndocked, shipToBeUndocked.RoundTripInDays);
                    }
                    return AnchorageID;
                }
                
            }

            return Guid.Empty;
        }

        /// <summary>
        /// Gets specific ship from anchorage
        /// </summary>
        /// <param name="shipID">Unique ID of ship to be found.</param>
        /// <returns>Returns ship object if ship is found in anchorage.</returns>
        /// <exception cref="ArgumentException">Throws exception if ship is not found.</exception>
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
        /// Gets specific ship from loading dock.
        /// </summary>
        /// <param name="shipID">Unique ID of ship to be found.</param>
        /// <returns>Returns ship object if ship is found in loading dock.</returns>
        /// <exception cref="ArgumentException">Throws exception if ship is not found.</exception>
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
        /// Gets specific ship from ship docks.
        /// </summary>
        /// <param name="shipID">Unique ID of ship to be found.</param>
        /// <returns>Returns ship object if ship is found in ship dock.</returns>
        /// <exception cref="ArgumentException">Throws exception if ship is not found.</exception>
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
        /// Gets loading dock that has a specific ship docked.
        /// </summary>
        /// <param name="shipID">Unique ID of ship to be found in loading dock.</param>
        /// <returns>Returns loading dock that contains the specified ship.</returns>
        /// <exception cref="ArgumentException">Throws exception if ship is not found.</exception>
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
        /// Creates a list of all docked ships in loading docks.
        /// </summary>
        /// <returns>Returns an IList with Ship object container information on all ships docked in loading dock.</returns>
        internal List<Ship> DockedShipsInLoadingDock()
        {
            List<Ship> ships = new List<Ship>();

            foreach (var item in shipsInLoadingDock)
            {
                ships.Add(item.Key);
            }

            return ships;
        }

        internal void RestockContainers(Ship ship, DateTime time)
        {
            int size = ship.ContainersOnBoard.Count;
            Random rand = new Random();
            for (int i = 0; i < size; i++)
            {
                Container container = ship.UnloadContainer();
                container.AddStatusChangeToHistory(Status.ArrivedAtDestination, time);
                ArrivedAtDestination.Add(container);
            }

            for (int i = 0; i < rand.Next(ship.ContainerCapacity/3, ship.ContainerCapacity - 1); i++)
            {
                ship.GenerateContainer(time);
            }


        }

        /// <summary>
        /// Checks if a loading dock matching the specified shipsize is available.
        /// </summary>
        /// <param name="shipSize">Size of ship the dock has to fit.</param>
        /// <returns>Returns true if there is available loading docks for the shipsize, false if there is no available loading docks.</returns>
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
        /// Checks if a free ship dock matching the specified shipsize is available.
        /// </summary>
        /// <param name="shipSize">Size of ship the dock has to fit.</param>
        /// <returns>Returns true if there is available ship docks for the shipsize, false if there is no available ship docks.</returns>
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
        /// Gets all free loading dock matching the specified shipsize,
        /// </summary>
        /// <param name="shipSize">Size of ship the dock has to fit.</param>
        /// <returns>Returns loadingdock if there is available loading docks for the shipsize, if no docks are available null is returned.</returns>
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
        /// Gets all free ship dock matching the specified shipsize.
        /// </summary>
        /// <param name="shipSize">Size of ship the dock has to fit.</param>
        /// <returns>Returns shipdocks if there is available ship docks for the shipsize.</returns>
        /// <exception cref="ArgumentException">Throws exception if shipSize is not found.</exception>
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
        /// Removes specified ship from anchorage.
        /// </summary>
        /// <param name="shipID">Unique ID of ship to be removed.</param>
        /// <returns>Returns true if specified ship was in anchorage, if ship was not found in anchorage false is returned.</returns>
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
        /// Undocks ship from loading lock to ship dock.
        /// </summary>
        /// <param name="shipID">Unique ID of ship to be undocked.</param>
        /// <returns>Returns a Guid object representing the loading dock the ship docked from.</returns>
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
        /// Undocks ship from ship dock to loading dock.
        /// </summary>
        /// <param name="shipID">Unique ID of ship to be undocked.</param>
        /// <returns>Returns a Guid object representing the ship dock the ship docked from.</returns>
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
        /// Removes loading dock from list of free loading docks.
        /// </summary>
        /// <param name="dockID">Unique ID of dock to be removed.</param>
        /// <returns>Returns a boolean that is true if specified dock was in the list of free loading docks, if specified dock was not in list flase is returned.</returns>
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
        /// Counts the number of available loading docks of the specified size.
        /// </summary>
        /// <param name="shipSize">Size of ship the dock has to fit.</param>
        /// <returns>Returns an int value representing the total amount of available loading docks of specified size.</returns>
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
        /// Counts the number of available containerspaces of specified size.
        /// </summary>
        /// <param name="containerSize">Size of the container the containerSpace has to fit.</param>
        /// <returns>Returns an int value representing the total number of available loading containerspaces of specified size.</returns>
        /// <exception cref="ArgumentException">Throws exception if containerSize is not found.</exception>
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
        /// Counts the number of occupied containerspaces of specified size.
        /// </summary>
        /// <param name="containerSize">Size of the container the containerSpace has to fit.</param>
        /// <returns>Returns an int value representing the total number of occupoed loading containerspaces of specified size.</returns>
        internal int GetNumberOfOccupiedContainerSpaces(ContainerSize containerSize)
        {
            return storedContainers.Count;

        } //returnerer antallet okuperte plasser av den gitte typen

        /// <summary>
        /// Gets the available container space of specified size.
        /// </summary>
        /// <param name="containerSize">Size of the container the containerRow has to fit.</param>
        /// <returns>Returns a Guid representing the available containerspace of specified size.</returns>
        /// <exception cref="ArgumentException">Throws exception if containerSize is not found.</exception>
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
        /// Gets the stored containers of specified size.
        /// </summary>
        /// <param name="containerSize">Size of the container the containerSpace has to fit.</param>
        /// <returns>Returns a Guid object representing the stored container of specified size, if container of specified size is not found null is returned.</returns>
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
        /// Unloads container of specific size from ship to containerspace.
        /// </summary>
        /// <param name="containerSize">Size of the container the containerSpace has to fit.</param>
        /// <param name="ship">Ship object container is unloaded from.</param>
        /// <param name="currentTime">The current time container is unloaded</param>
        /// <returns>Returns a Guid object representing the containerspaces the unloaded container was stored, if container did not exist empty is returned.</returns>
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
        /// Adds new ship to anchorage.
        /// </summary>
        /// <param name="ship">Ship object to be added to anchorage.</param>
        internal void AddNewShipToAnchorage(Ship ship)
        {
            Anchorage.Add(ship);
            if (ShipsInTransit.ContainsKey(ship))
                ShipsInTransit.Remove(ship);
        }

        /// <summary>
        /// Gets last registered status of specific ship.
        /// </summary>
        /// <param name="ShipID">Unique ID of ship to get status from.</param>
        /// <returns>Returns a Status enum representing the last registered status of specified ship if the ship has a history, if no status is registered null is returned.</returns>
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
        /// <returns>Returns a dictionary representing the status of all shipdocks.</returns>
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
        /// Gets the status of all loading docks.
        /// </summary>
        /// <returns>Returns a string value representing all the dock IDs and their status.</returns>
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
        /// Gets the status of specified container.
        /// </summary>
        /// <param name="ContainerId">Unique ID of container to get status from.</param>
        /// <returns>Returns a string value representing the dock ID and their status.</returns>
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
        /// Gets the last registered status of all containers.
        /// </summary>
        /// <returns>Returns a string value representing the container ID and their last registered status.</returns>
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
        /// Checks if loading dock is available for all dock sizes.
        /// </summary>
        /// <returns>Returns a dictionary with all the available loading docks, if no docks are available null is returned.</returns>
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
        /// Checks if specified dock is available.
        /// </summary>
        /// <param name="dockID">Unique ID of dock to be checked if available.</param>
        /// <returns>Returns a bollean that is true if specified dock is free, or false if it's not.</returns>
        bool LoadingDockIsFree(Guid dockID)
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
        /// Checks if ship dock is available for all dock sizes.
        /// </summary>
        /// <returns>Returns a dictionary with ship docks available for all dock sizes.</returns>
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
        /// Gets the last registered status from all ships.
        /// </summary>
        /// <returns>Return a dictionary with the last registered status of all ships, if they have a status.</returns>
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
        /// Gets all containers stored in harbor.
        /// </summary>
        /// <returns>Returns an Ilist with Container objects with all containors stored in harbor.</returns>
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
        /// Gets all ships in a loading dock.
        /// </summary>
        /// <returns>Returns a Ilist with Ship objects with all ships in a loading dock.</returns>
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
        /// Gets all ships in a ship dock.
        /// </summary>
        /// <returns>Returns an Ilist with Ship objects with all ships in a ship dock.</returns>
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
        /// Gets all ships in transit.
        /// </summary>
        /// <returns>Returns an Ilist with Ship objects with all ships in transit.</returns>
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
       

               
