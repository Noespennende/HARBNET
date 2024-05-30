using Gruppe8.HarbNet.Advanced;
using System.Text;

namespace Gruppe8.HarbNet
{
    /// <summary>
    /// harbor to be used in a simulation. A harbor represents the place where all activety in the simulation takes place. 
    /// </summary>
    public class Harbor : Port
    {
        /// <summary>
        /// Gets a list containing all containers stored in the harbor storage area.
        /// </summary>
        /// <returns>Returns an Ilist with Container objects containing all containers stored in the harbor storage area.</returns>
        /// <summary>
        /// Gets the unique ID for the harbor.
        /// </summary>
        /// <return>Returns a Guid object representing the harbors unique ID.</return>
        public override Guid ID { get; internal set; } = Guid.NewGuid();

        /// <summary>
        /// Get a list containing all the containers that have left the harbor and arived at their final destination
        /// </summary>
        /// <return>Returns a IList of container objects that represent all the containers that have arrived at their final destination during the simulation.</return>
        public override IList<Container> ArrivedAtDestination { get; internal set; } = new List<Container>();

        /// <summary>
        /// Gets the location ID for ships who are in transit at sea.
        /// </summary>
        /// <return>Returns a Guid representing the location of the ships who are in transit at sea.</return>
        public override Guid TransitLocationID { get; } = Guid.NewGuid();

        /// <summary>
        /// Gets the location ID of the harbors anchorage.
        /// </summary>
        /// <return>Returns a Guid representing the location of the harbors anchorage.</return>
        public override Guid AnchorageID { get; } = Guid.NewGuid();

        /// <summary>
        /// Gets the location ID of the location a container is in when it's being transported by an AGV. 
        /// </summary>
        /// <return>Returns a Guid object representing the ID of the location of an AGVs cargo.</return>
        public override Guid AgvCargoID { get; } = Guid.NewGuid();

        /// <summary>
        /// Gets the location ID of the location of trucks in transit to their destination.
        /// </summary>
        /// <return>Returns a Guid object representing the ID of the location of trucks who are in transit to their destination.</return>
        public override Guid TruckTransitLocationID { get; } = Guid.NewGuid();

        /// <summary>
        /// Gets the location ID of the location of the truck queue in to the harbor.
        /// </summary>
        /// <return>Returns a Guid object representing the location of trucks queueing to enter the harbor.</return>
        public override Guid TruckQueueLocationID { get; } = Guid.NewGuid();

        /// <summary>
        /// Gets the location ID of the harbors container storage area. 
        /// </summary>
        /// <return>Returns a Guid object representing the location ID of the harbors container storage areas.</return>
        public override Guid HarborStorageAreaID { get; } = Guid.NewGuid();

        /// <summary>
        /// Gets the location ID of the Harbors dock area. The dock area is the area of the harbor both LoadingDocks and ShipDocks is located.
        /// </summary>
        /// <return>Returns a Guid representing the harbors dock areas.</return>
        public override Guid HarborDockAreaID { get; } = Guid.NewGuid();

        /// <summary>
        /// The location ID of containers who have arrived at their destination. The containers who have this ID as their location have left the harbor
        /// and arrived at their final destination.
        /// </summary>
        /// <return>A Guid object representing the location ID of containers who have arrived at their destination.</return>
        public override Guid DestinationID { get; } = Guid.NewGuid();

        /// <summary>
        /// Gets a list containing all loadingdocks in the harbor. A loading dock is a place where ships can dock to the harbor
        /// to load and/or unload their cargo from/to the harbor.
        /// </summary>
        /// <return>Returns an IList with LoadingDock objects representing the all the loadingdocks in the harbor.</return>
        internal IList<LoadingDock> allLoadingDocks = new List<LoadingDock>();

        /// <summary>
        /// Gets a list containing all the loadingdocks that are free for ships to dock to them.
        /// A loading dock is a place where ships can dock to the harbor to load and/or unload their cargo from/to the harbor.
        /// </summary>
        /// <return>Returns an Ilist with LoadingDock objects representing the available loadingdocks in the harbor.</return>
        internal IList<LoadingDock> freeLoadingDocks = new List<LoadingDock>();

        /// <summary>
        /// Gets all the ships currently docked to a loading dock.
        /// </summary>
        /// <return>Returns a IDictionary containing Ship objects as keys and the LoadingDock objects they are currently docked to as values.</return>
        internal IDictionary<Ship, LoadingDock> shipsInLoadingDock = new Dictionary<Ship, LoadingDock>();

        /// <summary>
        /// Gets a list containing all the ShipDocks in the harbor. ShipDocks are docks where ships can be stored once they are done
        /// delivering their cargo and have no more trips to make. If no shipdocks are available ships will achor in the anchorage instead.
        /// </summary>
        /// <return>Returns an IList with ShipDick objects representing all the shipdocks in the harbor.</return>
        internal IList<ShipDock> allShipDocks = new List<ShipDock>();

        /// <summary>
        /// Gets all the shipdocks in the harbor that are currently free for ships to dock to them.ShipDocks are docks where ships can be stored once they are done
        /// delivering their cargo and have no more trips to make. If no shipdocks are available ships will achor in the anchorage instead.
        /// </summary>
        /// <return>Returns an Ilist with ShipDock objects representing the shipdocks witch are available for ships to dock to them.</return>
        internal IList<ShipDock> freeShipDocks = new List<ShipDock>();

        /// <summary>
        /// Gets a dictionary containing Ships and the ship docks they are docked to.
        /// </summary>
        /// <return>Returns a IDictionary containing Ship objects as keys and the ShipDock objects they are docked to as values.</return>
        internal IDictionary<Ship, ShipDock> shipsInShipDock = new Dictionary<Ship, ShipDock>();

        /// <summary>
        /// Gets a list containing all the ships currently anchored in the anchorage. The anchorage is a place where ships anchor while waiting for
        /// docks to get free in the harbor or when they are done with their trip and have no new trips to make.
        /// </summary>
        /// <return>Returns an Ilist with Ship objects representing the ships anchored in the anchorage.</return>
        internal IList<Ship> Anchorage { get; } = new List<Ship>();

        /// <summary>
        /// Gets a dictionary of all the ships currently in transit and how many days in to the trip they are.
        /// </summary>
        /// <return>Returns a IDictionary containing ship objects as keys and an int value representing how many days in to their transit they are as values.</return>
        internal IDictionary<Ship, int> ShipsInTransit { get; } = new Dictionary<Ship, int>();

        /// <summary>
        /// Gets a list containing all the ships in the simulation.
        /// </summary>
        /// <return>Returns a Ilist containing all the ship objects used in the harbor simulation</return>
        internal IList<Ship> AllShips { get; } = new List<Ship>();

        /// <summary>
        /// Gets a list containing all the ContainerStorageRows in the simulation.
        /// </summary>
        /// <return>Returns an Ilist with ContainerStorageRow objects representing all the containerRows in the harbor.</return>
        internal IList<ContainerStorageRow> AllContainerRows { get; set; }

        /// <summary>
        /// Gets a dictionary containing all containers in the harbor storage area and the ContainerStorageRow they are stored in.
        /// </summary>
        /// <return>Returns a IDictionary containing Container objects as keys and the ContainerStorageRow object they are stored in as values</return>
        internal IDictionary<Container, ContainerStorageRow> storedContainers = new Dictionary<Container, ContainerStorageRow>();

        /// <summary>
        /// Gets a list containing all cranes in the harbor storage area.
        /// </summary>
        /// <return>Returns an Ilist with Crane objects representing the cranes in the harbor's storage area.</return>
        internal IList<Crane> HarborStorageAreaCranes { get; set; } = new List<Crane>();

        /// <summary>
        /// Gets a list containing all the cranes next to the harbor loading docks.
        /// </summary>
        /// <return>Returns an Ilist with Crane objects representing the cranes next to loading docks in the harbor.</return>
        internal IList<Crane> DockCranes { get; set; } = new List<Crane>();

        /// <summary>
        /// Gets a list containing all AGVs in the harbor who are currently moving a container around the harbor area.
        /// </summary>
        /// <return>Returns an Ilist with Agv objects representing all the AGV's who are currently moving containers.</return>
        internal IList<Agv> AgvWorking { get; set; } = new List<Agv>();

        /// <summary>
        /// Gets a list containing all the Agvs who are currently free to move a container.
        /// </summary>
        /// <return>Returns an Ilist with Agv objects representing all the AGV's who are available to move a container.</return>
        internal IList<Agv> AgvFree { get; set; } = new List<Agv>();

        /// <summary>
        /// Gets a list containing all Trucks in transit to their delivery destination.
        /// </summary>
        /// <return>Returns an Ilist with Truck objects representing the Trucks in transit to their delivery destination.</return>
        internal IList<Truck> TrucksInTransit { get; set; } = new List<Truck>();

        /// <summary>
        /// Gets a list containing all trucks standing in queue to enter the harbor.
        /// </summary>
        /// <return>Returns an Ilist with Truck objects representing the Trucks in queue to enter the harbor.</return>
        internal IList<Truck> TrucksInQueue { get; set; } = new List<Truck>();

        /// <summary>
        /// Gets a number representing the percentage of containers directly loaded from ships on to trucks. Containers that are not loaded on to trucks
        /// get brought to the harbor storage area for storage.
        /// </summary>
        /// <returns>Returns a double value representing the percentege of the total amount of containers that are directly loaded from the ships on to trucks.
        /// A value of 1.0 represents 100%, a value of 0.5 represents 50%.</returns>
        internal double PercentOfContainersDirectlyLoadedFromShips { get; set; }

        /// <summary>
        /// Gets the percentege of containers loaded directly from the container storage area on to trucks. Conntainers who are not loaded on to trucks are instead
        /// loaded on to ships. 
        /// </summary>
        /// <returns>Returns a double value representing the percentege of the total amount of containers that are directly loaded from the harbor storage area on to trucks.
        /// A value of 1.0 represents 100%, a value of 0.5 represents 50%.</returns>
        internal double PercentOfContainersDirectlyLoadedFromStorageArea { get; set; }

        /// <summary>
        /// Gets a number representing the amount of trucks that arrive to the harbor per hour. Trucks who arrive will place themselves at the back of the
        /// TrucksInQueue queue.
        /// </summary>
        /// <returns>Returns an int value representing the amount of trucks that arrive to the harbor per hour.</returns>
        internal int TrucksArrivePerHour { get; set; }

        /// <summary>
        /// Gets the amount of loads each agv can do per hour. One load is considered to be one AGV loading a container in to its storage or
        /// one AGV unloading a container from its storage. This also includes the time it takes to transport the container from A to B.
        /// </summary>
        /// <returns>Returns an int value representing the amount of loads each AGV is capable of doing in one hour.</returns>
        internal int LoadsPerAgvPerHour { get; set; }

        /// <summary>
        /// Creates a new object of the harbor class.
        /// </summary>
        /// <param name="listOfShips">An IList containing ship objects representing all the ships that will be used in a simulation of a harbor.</param>
        /// <param name="listOfContainerStorageRows">An IList containing ContainerStorageRows available in the harbor's storage area.
        /// Each container storage row represents one row of storage spaces where containers can be stored in the harbor storage area.</param>
        /// <param name="numberOfSmallLoadingDocks">Int value representing the amount of small loading docks that will be available in the harbor dock area.
        ///  LoadingDocks are docks where ships can load or unload their cargo from or to the harbor. Small loading docks can only recieve ships of size small.</param>
        /// <param name="numberOfMediumLoadingDocks">Int value representing the amount of medium loading that will be available in the harbor dock area.
        ///  LoadingDocks are docks where ships can load or unload their cargo from or to the harbor. Medium loading docks can only recieve ships of size medium.</param>
        /// <param name="numberOfLargeLoadingDocks">Int value representing the amount of large loadingthat will be available in the harbor dock area.
        ///  LoadingDocks are docks where ships can load or unload their cargo from or to the harbor. Large loading docks can only recieve ships of size large.</param>
        /// <param name="numberOfCranesNextToLoadingDocks">Int value representing the amount of cranes to be placed in the harbor's docking area. These cranes will be used to load
        /// or unload containers to and from ships</param>
        /// <param name="LoadsPerCranePerHour">Int value representing the amount of loads a single crane can do in one hour. A load is defined as either loading a container on the the cranes storage
        /// or unloading a container from a cranes storage.</param>
        /// <param name="numberOfCranesOnHarborStorageArea">Int value representing the amount of cranes to be placed in the harbor's own storage area for containers. These cranes will be used to
        /// load or unload containers from or to the container storage rows inside the storage area</param>
        /// <param name="numberOfSmallShipDocks">Int value representing the amount of small loading docks that will be available in the harbor dock area.
        /// Ship docks are docks where ships will permanently dock once they are done with all their voyages. A small loading dock can only recieve ships of size small. </param>
        /// <param name="numberOfMediumShipDocks">Int value representing the amount of medium loading docks that will be available in the harbor dock area.
        /// Ship docks are docks where ships will permanently dock once they are done with all their voyages. A medium loading dock can only recieve ships of size medium.</param>
        /// <param name="numberOfLargeShipDocks">Int value representing the amount of large loading docks that will be available in the harbor dock area.
        /// Ship docks are docks where ships will permanently dock once they are done with all their voyages. A large loading dock can only recieve ships of size large.</param>
        /// <param name="numberOfTrucksArriveToHarborPerHour">Int value representing the amount of trucks arriving to the harbor per hour. Trucks are cargo viechles that can take one Container object
        /// and deliver it to its final destination.</param>
        /// <param name="percentageOfContainersDirectlyLoadedFromShipToTrucks">Int value representing the percentage of containers directly loaded from ship on to trucks. Containers that are not
        /// directly loaded on to trucks will instead go to the harbor's storage area. A value of 100 represents 100%. A value of 50 represents 50%.</param>
        /// <param name="percentageOfContainersDirectlyLoadedFromHarborStorageToTrucks">Int value representing the percentage of containers directly loaded from harbor storage to trucks.
        /// Containers that are not loaded directly to trucks will instead be loaded on to Ships that will carry the cargo to its final destination. A value of 100 represents 100%. A value of 50 represents 50%.</param>
        /// <param name="numberOfAgvs">Int value representing the amount of AGVs in the harbor to be created. AGV's are Automated Guided Viechles that can deliver containers from point A to B in the harbor area.
        /// Typicly from the harbor's docking area to the harbor's storage area.</param>
        /// <param name="loadsPerAgvPerHour">Int value representing the amount of loads each AGV is capable of doing in one hour. One load is defined as loading one container on to the AGVs cargo and driving it to its destination,
        /// or unloading one container from the AGVs cargo.</param>
        /// <exception cref="ArgumentOutOfRangeException">Exception to be thrown in harbor if a parameter is out of range.</exception>
        public Harbor(
            IList<Ship> listOfShips,
            IList<ContainerStorageRow> listOfContainerStorageRows,
            int numberOfSmallLoadingDocks,
            int numberOfMediumLoadingDocks,
            int numberOfLargeLoadingDocks,
            int numberOfCranesNextToLoadingDocks,
            int numberOfCranesOnHarborStorageArea,
            int LoadsPerCranePerHour,
            int numberOfSmallShipDocks,
            int numberOfMediumShipDocks,
            int numberOfLargeShipDocks,
            int numberOfTrucksArriveToHarborPerHour,
            int percentageOfContainersDirectlyLoadedFromShipToTrucks,
            int percentageOfContainersDirectlyLoadedFromHarborStorageToTrucks,
            int numberOfAgvs,
            int loadsPerAgvPerHour)
        {
            TrucksArrivePerHour = numberOfTrucksArriveToHarborPerHour;
            AllContainerRows = listOfContainerStorageRows.ToList();
            LoadsPerAgvPerHour = loadsPerAgvPerHour;

            AllShips = listOfShips.ToList();

            Initiliaze(
                listOfShips,
                numberOfSmallLoadingDocks,
                numberOfMediumLoadingDocks,
                numberOfLargeLoadingDocks,
                numberOfCranesNextToLoadingDocks,
                LoadsPerCranePerHour,
                numberOfCranesOnHarborStorageArea,
                numberOfSmallShipDocks,
                numberOfMediumShipDocks,
                numberOfLargeShipDocks,
                percentageOfContainersDirectlyLoadedFromShipToTrucks,
                percentageOfContainersDirectlyLoadedFromHarborStorageToTrucks,
                numberOfAgvs);
        }

        /// <summary>
        /// Creates a new object of the harbor class.
        /// </summary>
        /// <param name="numberOfShips">Int value representing the number of ships to be created and used in the harbor simulation. Each ship generated will be of a random size.</param>
        /// <param name="numberOfHarborContainerStorageRows">An int indicating the number of ContainerStorageRows that will be available in the harbor's storage area.
        /// Each container storage row represents one row of storage spaces where containers can be stored in the harbor storage area.</param>
        /// <param name="containerStorageCapacityInEachStorageRow">Int value representing the amount of containers each storage row can hold in its storage.</param>
        /// <param name="numberOfLoadingDocks">Int value representing the amount of  loading docks that will be available in the harbor dock area.
        ///  LoadingDocks are docks where ships can load or unload their cargo from or to the harbor. An equal amount of small, medium and large loading docks will be created,
        ///  each dock can only recieve ships of the coresponding size.</param>
        /// <param name="numberOfCranesNextToLoadingDocks">Int value representing the amount of cranes to be placed in the harbor's docking area. These cranes will be used to load
        /// or unload containers to and from ships</param>
        /// <param name="numberOfCranesOnHarborStorageArea">Int value representing the amount of cranes to be placed in the harbor's own storage area for containers. These cranes will be used to
        /// load or unload containers from or to the container storage rows inside the storage area</param>
        /// <param name="numberOfAgvs">Int value representing the amount of AGVs in the harbor to be created. AGV's are Automated Guided Viechles that can deliver containers from point A to B in the harbor area.
        /// Typicly from the harbor's docking area to the harbor's storage area.</param>
        /// <param name="loadsPerCranePerHour">Int value representing the amount of loads a crane does per hour.</param>
        /// <param name="loadsPerAgvPerHour">Int value representing the amount of loads each AGV is capable of doing in one hour. One load is defined as loading one container on to the AGVs cargo and driving it to its destination,
        /// or unloading one container from the AGVs cargo.</param>
        /// <param name="numberOftrucksArriveToHarborPerHour">Int value representing the amount of trucks arriving to the harbor per hour. Trucks are cargo viechles that can take one Container object
        /// and deliver it to its final destination.</param>
        /// <param name="percentageOfContainersDirectlyLoadedFromShipToTrucks">Int value representing the percentage of containers directly loaded from ship on to trucks. Containers that are not
        /// directly loaded on to trucks will instead go to the harbor's storage area. A value of 100 represents 100%. A value of 50 represents 50%.</param>
        /// <param name="percentageOfContainersDirectlyLoadedFromHarborStorageToTrucks">Int value representing the percentage of containers directly loaded from harbor storage to trucks.
        /// Containers that are not loaded directly to trucks will instead be loaded on to Ships that will carry the cargo to its final destination. A value of 100 represents 100%. A value of 50 represents 50%.</param>
        /// <exception cref="ArgumentOutOfRangeException">Exception to be thrown in harbor if parameter is out of set range.</exception>
        public Harbor(
            int numberOfShips,
            int numberOfHarborContainerStorageRows,
            int containerStorageCapacityInEachStorageRow,
            int numberOfLoadingDocks,
            int numberOfCranesNextToLoadingDocks,
            int numberOfCranesOnHarborStorageArea,
            int numberOfAgvs,
            int loadsPerCranePerHour = 35,
            int numberOftrucksArriveToHarborPerHour = 10,
            int loadsPerAgvPerHour = 25,
            int percentageOfContainersDirectlyLoadedFromShipToTrucks = 10,
            int percentageOfContainersDirectlyLoadedFromHarborStorageToTrucks = 15)
        {
            DateTime startDate = DateTime.Now;
            List<Ship> listOfShips = new();

            int smallShips = numberOfShips / 3;
            int mediumShips = numberOfShips / 3;
            int largeShips = numberOfShips / 3;

            if (numberOfShips % 3 == 1)
            {
                smallShips++;
            }

            else if (numberOfShips % 3 == 2)
            {
                smallShips++;
                mediumShips++;
            }

            for (int i = 0; i < smallShips; i++)
            {
                listOfShips.Add(new($"Ship Small {i}", ShipSize.Small, startDate, false, 7, 10, 10));
            }

            for (int i = 0; i < mediumShips; i++)
            {
                listOfShips.Add(new($"Ship Medium {i}", ShipSize.Medium, startDate, false, 7, 25, 25));
            }

            for (int i = 0; i < largeShips; i++)
            {
                listOfShips.Add(new($"Ship Large {i}", ShipSize.Large, startDate, false, 7, 50, 50));
            }

            AllShips = listOfShips.ToList();

            List<ContainerStorageRow> listOfContainerStorageRows = new();

            for (int i = 0; i < numberOfHarborContainerStorageRows; i++)
            {
                listOfContainerStorageRows.Add(new(containerStorageCapacityInEachStorageRow));
            }

            int smallLoadingDocks = numberOfLoadingDocks / 3;
            int mediumLoadingDocks = numberOfLoadingDocks / 3;
            int largeLoadingDocks = numberOfLoadingDocks / 3;

            if (numberOfLoadingDocks % 3 == 1)
            {
                smallLoadingDocks++;
            }

            if (numberOfLoadingDocks % 3 == 2)
            {
                smallLoadingDocks++;
                mediumLoadingDocks++;
            }

            TrucksArrivePerHour = numberOftrucksArriveToHarborPerHour;
            LoadsPerAgvPerHour = loadsPerAgvPerHour;
            AllContainerRows = listOfContainerStorageRows.ToList();


            Initiliaze(
                listOfShips,
                smallLoadingDocks,
                mediumLoadingDocks,
                largeLoadingDocks,
                numberOfCranesNextToLoadingDocks,
                loadsPerCranePerHour,
                numberOfCranesOnHarborStorageArea,
                0,
                0,
                0,
                percentageOfContainersDirectlyLoadedFromShipToTrucks,
                percentageOfContainersDirectlyLoadedFromHarborStorageToTrucks,
                numberOfAgvs);
        }

        /// <summary>
        /// Gets the current status of all containers in the simulation.
        /// </summary>
        /// <returns>Returns a string value representing the container's ID and their current status.</returns>
        public override string GetAllContainerStatus()
        {
            StringBuilder sb = new();
            Dictionary<Container, Status> containerStatus = new();

            foreach (Container container in storedContainers.Keys)
            {
                if (container != null && container.HistoryIList != null && container.HistoryIList.Count > 0)
                {
                    Status lastStatus = container.HistoryIList.LastOrDefault()?.Status ?? Status.None;
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
        /// Gets a IDictionary containing information about the availabilety of all the loading docks in the harbor.
        /// </summary>
        /// <returns>Returns an IDictionary containing Guid representing the loading docks and bool value representing if the loading docks are available or not. The key value is a Guid object
        /// representing the unque ID of a given dock. The value is a bool representing wether or not the dock is free. A bool value of True means the coresponding dock is free.</returns>
        public override IDictionary<Guid, bool> GetAvailabilityStatusForAllLoadingDocks()
        {
            IDictionary<Guid, bool> availabilityStatuses = new Dictionary<Guid, bool>();

            foreach (LoadingDock loadingDock in allLoadingDocks)
            {
                availabilityStatuses.Add(loadingDock.ID, loadingDock.Free);
            }

            return availabilityStatuses;
        }

        /// <summary>
        /// Checks if specified loading dock is available to recieve a ship.
        /// </summary>
        /// <param name="dockID">Guid representing the ID of the dock to be checked if available.</param>
        /// <returns>Returns a boolean that is true if the loadingdock with the given ID is free to recieve a ship, or false otherwise.</returns>

        /// <summary>
        /// Gets a IDictionary containing information about the status of all the ships in the simulation.
        /// </summary>
        /// <return>Returns a IDictionary containing information about the status of all the ships in the simulation. The Keyvalue in the dictonary is a ship object representing the ship
        /// and the Value is a Status enum representing the current status of the ship.</return>
        public override IDictionary<Ship, Status> GetStatusAllShips()
        {
            IDictionary<Ship, Status> statusOfAllShips = new Dictionary<Ship, Status>();

            foreach (Ship ship in AllShips)
            {
                StatusLog? lastStatus = ship.HistoryIList.Last();
                if (lastStatus != null)
                {
                    statusOfAllShips[ship] = lastStatus.Status;
                }
            }
            return statusOfAllShips;
        }

        /// <summary>
        /// Gets a string containing information about the harbor. This information includes the harbor's ID, total amount of ships, amount of ships docked to loading docks, amount of free loading docks, amount of ships docked to shipdocks, amount of
        /// free ship docks, amount of ships in anchorage, amount of ships in transit and amount of containers stored in the harbor storage area.
        /// </summary>
        /// <returns>Returns a string value containing information about the harbor.</returns>
        public override string ToString()
        {
            return
                $"ID: {ID}, " +
                $"Total number of ships: {AllShips.Count}, " +
                $"Ships in loading docks: {shipsInLoadingDock.Count}, " +
                $"Free loading docks: {freeLoadingDocks.Count}, " +
                $"Ships in ship docks: {shipsInShipDock.Count}, " +
                $"Free ship docks: {freeShipDocks.Count}, " +
                $", Ships in anchorage: {Anchorage.Count}, " +
                $"Ships in transit: {ShipsInTransit.Count}, " +
                $"Containers stored in harbor: {storedContainers.Count}";
        }


        /// <summary>
        /// Gets the current status of the ship with the given ID.
        /// </summary>
        /// <param name="ShipID">Guid object represnting the ID of the ship to get the status from.</param>
        /// <returns>Returns a Status enum representing current status of the ship with the given ID. If no status is registered Status.None is returned.</returns>
        public override Status GetShipStatus(Guid ShipID)
        {
            StatusLog? lastStatusChange = null;
            StringBuilder sb = new();

            foreach (Ship ship in AllShips)
            {
                if (ship.ID == ShipID && ship.HistoryIList != null && ship.HistoryIList.Count > 0)
                {
                    string shipStatus = $"ShipId: {ship.ID}, Last status Change: {lastStatusChange}";
                    sb.Append(shipStatus);
                }
            }

            return Status.None;
        }

        /// <summary>
        /// Gets a IDictionary containing information about the availabilety of all the ship docks in the harbor. A ship dock is a dock where ships can be stored once their voyage is completed.
        /// </summary>
        /// <returns>Returns an IDictionary containing Guid representing the ship docks and bool value representing if the ship docks are available or not. The key value is a Guid object
        /// representing the unque ID of a given dock. The value is a bool representing wether or not the dock is free. A bool value of True means the coresponding dock is free.</returns>
        public override IDictionary<Guid, bool> GetAvailabilityStatusForAllShipDocks()
        {
            Dictionary<Guid, bool> availabilityStatuses = new();

            foreach (ShipDock shipDock in allShipDocks)
            {
                availabilityStatuses.Add(shipDock.ID, shipDock.Free);
            }

            return availabilityStatuses;
        }

        public override string GetStatusAllLoadingDocks()
        {
            StringBuilder sb = new();
            Dictionary<LoadingDock, bool> dockStatus = new();

            foreach (LoadingDock loadingDock in allLoadingDocks)
            {
                dockStatus[loadingDock] = loadingDock.Free;
            }

            foreach (var keyValue in dockStatus)
            {
                sb.AppendLine($"dockId: {keyValue.Key}, dock free: {keyValue.Value}");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Gets the current status of the container with the given ID.
        /// </summary>
        /// <param name="ContainerId">Guid object represting the unique ID of the container object in which the current status is to be returned.</param>
        /// <returns>Returns a string value representing the container's ID and their last registered status.</returns>
        public override string GetContainerStatus(Guid ContainerId)
        {
            StringBuilder sb = new();
            Dictionary<Container, Status> containerStatus = new();
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
        /// Creates the amount of ContainerRows given and fills each one with the amount of container spaces given.
        /// Then adds the containerrows to the harbor's storage area.
        /// </summary>
        /// <param name="numberOfContainerSpaces">Int value representing the amount of container spaces to be created in each ContainerStorageRow.</param>
        /// <param name="numberOfContainerRows">Int value representing the amount of container rows to be created and added to the harbor's storage area.</param>
        internal void CreateContainerSpaces(int numberOfContainerSpaces, int numberOfContainerRows)
        {
            IList<ContainerStorageRow> containerRows = new List<ContainerStorageRow>();

            for (int j = 0; j < numberOfContainerRows; j++)
            {
                containerRows.Add(new ContainerStorageRow(numberOfContainerSpaces));
            }

            AllContainerRows = containerRows;
        }

        /// <summary>
        /// Loads container from ship's storage to the given crane.
        /// </summary>
        /// <param name="ship">Ship object the container is unloaded from.</param>
        /// <param name="crane">Crane object the container is loaded to.</param>
        /// <param name="currentTime">The Date and time container is loaded from ship's storage on to the crane.</param>
        /// <returns>Returns the container object that was loaded from the ship's storage to the crane.</returns>
        /// <exception cref="CraneCantBeLoadedException">Exeption thrown if the crane can not be loaded from the ship's storage to the crane. This happens if the crane is already holding a container
        /// in its cargo.</exception>
        internal Container? ShipToCrane(Ship ship, Crane crane, DateTime currentTime)
        {
            if (!(crane.Container == null))
            {
                throw new CraneCantBeLoadedException("The crane you are trying to load already holds a container in its cargo and therefore can not load another one from the ship.");
            }

            Container? containerToBeLoaded = ship.UnloadContainer();

            if (containerToBeLoaded != null)
            {
                containerToBeLoaded.CurrentLocation = ship.CurrentLocation;
                containerToBeLoaded.AddStatusChangeToHistory(Status.LoadingToCrane, currentTime);
                crane.LoadContainer(containerToBeLoaded);
            }

            return containerToBeLoaded;
        }

        /// <summary>
        /// Loads container to a ship's cargo from the given crane.
        /// </summary>
        /// <param name="ship">Ship object the container is loaded to.</param>
        /// <param name="crane">Crane object the container is unloaded from.</param>
        /// <param name="currentTime">The Date and time container is loaded from crane's storage to ship's storage.</param>
        /// <returns>Returns the container object that was loaded to the ship's storage from the crane.</returns>
        internal Container? CraneToShip(Crane crane, Ship ship, DateTime currentTime)
        {
            Container? containerToBeLoaded = crane.UnloadContainer();

            if (containerToBeLoaded != null)
            {
                containerToBeLoaded.CurrentLocation = crane.Location;
                containerToBeLoaded.AddStatusChangeToHistory(Status.Loading, currentTime);
                ship.AddContainer(containerToBeLoaded);
            }

            return containerToBeLoaded;
        }

        /// <summary>
        /// Loads container from the given crane to the given truck.
        /// </summary>
        /// <param name="crane">Crane object the container is unloaded from.</param>
        /// <param name="truck">Truck object that the container is loaded to.</param>
        /// <param name="currentTime">The Date and time container is loaded from crane's storage to the truck's storage.</param>
        /// <returns>Returns the container object that was loaded from the crane to the truck.</returns>
        /// <exception cref="TruckCantBeLoadedException">Exception to be thrown if truck can't be loaded. This happens if the truck is already in transit or if the given truck does not exist
        /// within the simulation</exception>
        internal Container? CraneToTruck(Crane crane, Truck truck, DateTime currentTime)
        {
            if (!TrucksInQueue.Contains(truck))
            {
                if (TrucksInTransit.Contains(truck))
                {
                    throw new TruckCantBeLoadedException("The truck you are trying to load is already in transit away from the harbor and therefore can't load the container from the crane");
                }

                else
                {
                    throw new TruckCantBeLoadedException("The truck you are trying to load does not exist in the simulation and therefore can't load the container from the crane.");
                }
            }

            if (!(truck.Container == null))
            {
                throw new TruckCantBeLoadedException("The truck you are trying to load already has a container in its storage and therefore don't have room for the container from the given crane");
            }

            Container? containerToBeLoaded = crane.UnloadContainer();
            if (containerToBeLoaded != null)
            {
                containerToBeLoaded.CurrentLocation = crane.Location;
                truck.LoadContainer(containerToBeLoaded);
                containerToBeLoaded.AddStatusChangeToHistory(Status.LoadingToTruck, currentTime);
            }

            return containerToBeLoaded;
        }

        /// <summary>
        /// Gets one available cranes from the harbor dock area.
        /// </summary>
        /// <returns>Returns one crane available from the harbors docking area. If no cranes are available null is returned. </returns>
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
        /// Removes truck from the truck queue inn to the harbor.
        /// </summary>
        /// <param name="truck">Truck object to be removed from truck queue.</param>
        internal void RemoveTruckFromQueue(Truck truck)
        {
            TrucksInQueue.Remove(truck);
        }

        /// <summary>
        /// Sends truck from a loading dock to transit to its delivery destination.
        /// </summary>
        /// <param name="loadingDock">The loading dock the truck is leaving from.</param>
        /// <param name="container">The container object to be transported on the truck to the final destination.</param>
        /// <exception cref="NullReferenceException">Exception to be thrown if truck is not found.</exception>
        internal void SendTruckOnTransit(LoadingDock loadingDock, Container container)
        {
            Truck? truck = null;

            foreach (var pair in loadingDock.TruckLoadingSpots)
            {
                if (pair.Value?.Container == container)
                {
                    truck = pair.Value;
                    break;
                }
            }

            if (truck?.Container != null)
            {
                RemoveTruckFromQueue(truck);
                TrucksInTransit.Add(truck);
                truck.Status = Status.Transit;
                truck.Location = TruckTransitLocationID;
            }
        }

        /// <summary>
        /// Sends truck to transit from the truck queue to its delivery destination.
        /// </summary>
        /// <param name="container">The container object to be transported on truck.</param>
        /// <returns>Returns the truck object transporting the given container object, if no truck is transporting the given container null is returned.</returns>
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
        /// Returns a number indicating the amount of all the containers in the simulation stored in the cargoholds of ships.
        /// </summary>
        /// <returns>Returns an int value representing the amount of containers stored in the cargoholds of ships.</returns>
        internal int NumberOfContainersInStorageToShips()
        {
            int numberOfContainersToShips = storedContainers.Count - NumberOfContainersInStorageToTrucks();

            return numberOfContainersToShips;
        }

        /// <summary>
        /// Returns a number indicating the amount of all the containers in the simulation that are stored in the cargoholds of trucks.
        /// </summary>
        /// <returns>Returns an int value representing the amount of containers stored in the cargoholds of trucks.</returns>
        internal int NumberOfContainersInStorageToTrucks()
        {
            double percentTrucks = PercentOfContainersDirectlyLoadedFromStorageArea;

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
        /// Loads container from the given crane to the given Agv
        /// </summary>
        /// <param name="crane">The crane object the container is unloaded from.</param>
        /// <param name="agv">The agv object that the container is loaded to.</param>
        /// <param name="currentTime">The Date and Time the container is loaded from crane to the agv.</param>
        /// <returns>Returns the container object that were loaded from the crane to the Agv.</returns>
        /// <exception cref="AgvCantBeLoadedException">Exception to be thrown if Agv can't be loaded.</exception>
        internal Container? CraneToAgv(Crane crane, Agv agv, DateTime currentTime)
        {
            if (!AgvFree.Contains(agv))
            {
                if (AgvWorking.Contains(agv))
                {
                    throw new AgvCantBeLoadedException("The AGV you are trying to load is already transporting goods and therefore can not load a container from the crane.");
                }

                else
                {
                    throw new AgvCantBeLoadedException("The AGV you are trying to load does not exist in the simulation and therefore can't be loaded.");
                }
            }

            if (!(agv.Container == null))
            {
                throw new AgvCantBeLoadedException("The AGV given already has a container in its storage and therefore has no room for the container the crane is trying to load.");
            }

            Container? containerToBeLoaded = crane.UnloadContainer();

            if (containerToBeLoaded != null)
            {
                containerToBeLoaded.CurrentLocation = crane.Location;
                agv.LoadContainer(containerToBeLoaded);
                containerToBeLoaded.AddStatusChangeToHistory(Status.LoadingToAgv, currentTime);

                AgvFree.Remove(agv);
                AgvWorking.Add(agv);
            }

            return containerToBeLoaded;
        }

        /// <summary>
        /// Loads container from the given Agv to the given Crane.
        /// </summary>
        /// <param name="crane">The crane object that the container is loaded to.</param>
        /// <param name="agv">The Agv object the container is unloaded from.</param>
        /// <param name="currentTime">The Date and Time the container is loaded.</param>
        /// <returns>Returns the container object that were loaded from the crane to the agv.</returns>
        /// <exception cref="CraneCantBeLoadedException">Exception the be thrown if Crane can't be loaded.</exception>
        internal Container? AgvToCrane(Crane crane, Agv agv, DateTime currentTime)
        {
            if (agv.Container == null)
            {
                throw new CraneCantBeLoadedException("The AGV you are trying to unload doesn't have a container in its storage and therefore can't unload to the crane.");
            }

            if (!AgvWorking.Contains(agv))
            {
                if (AgvFree.Contains(agv))
                {
                    throw new CraneCantBeLoadedException("The AGV you are trying to unload is set as free and therefore is not working to unload cargo. AGVs must be working for cargo to be unloaded.");
                }

                else
                {
                    throw new CraneCantBeLoadedException("The AGV you are trying to unload does not exist within the simulation and therefore can not unload to the crane");
                }
            }

            if (!(crane.Container == null))
            {
                throw new CraneCantBeLoadedException("The crane you are trying to load already has a container in its storage and therefore has no room to load the container from the AGV");
            }

            Container? containerToBeLoaded = agv.UnloadContainer();
            if (containerToBeLoaded != null)
            {
                containerToBeLoaded.CurrentLocation = agv.Location;
                crane.LoadContainer(containerToBeLoaded);

                containerToBeLoaded.AddStatusChangeToHistory(Status.LoadingToCrane, currentTime);

                AgvWorking.Remove(agv);
                AgvFree.Add(agv);
            }

            return containerToBeLoaded;
        }

        /// <summary>
        /// Loads the given container from the given crane to a free ContainerStorageRow in the harbor storage area if there is available space.
        /// </summary>
        /// <param name="crane">Crane object container is unloaded from</param>
        /// <param name="currentTime">The Date and Time the container is loaded.</param>
        /// <returns>Returns True if the container was successfully loaded from the given crane to a free ContainerStorageRow. Returns false otherwise.</returns>
        internal bool CraneToContainerRow(Crane crane, DateTime currentTime)
        {
            Container? container = crane.UnloadContainer();

            if (container != null)
            {
                foreach (ContainerStorageRow CR in AllContainerRows)
                {
                    if (CR.CheckIfFreeContainerSpaceExists(container.Size))
                    {
                        CR.AddContainerToFreeSpace(container);
                        storedContainers.Add(container, CR);
                        container.AddStatusChangeToHistory(Status.InStorage, currentTime);
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Loads a container of the given size from a ContainerStorageRow in the harbor storage area to the given crane.
        /// </summary>
        /// <param name="size">ContainerSize enum representing the Size of the container to be loaded on to the crane.</param>
        /// <param name="crane">Crane object that will load the container.</param>
        /// <param name="currentTime">The date and time the container is loaded.</param>
        /// <returns>Returns the container object that were loaded on to the crane, if no containers were loaded null is returned.</returns>
        /// <exception cref="CraneCantBeLoadedException">Throws exception if crane can't be loaded.</exception>
        internal Container? ContainerRowToCrane(ContainerSize size, Crane crane, DateTime currentTime)
        {
            if (!(crane.Container == null))
            {
                throw new CraneCantBeLoadedException("The crane you are trying to load already has a container in its storage and therefore has no room to load the container from the harbor storage area");
            }

            foreach (Container container in storedContainers.Keys)
            {
                if (container.Size == size)
                {
                    crane.LoadContainer(container);
                    container.AddStatusChangeToHistory(Status.LoadingToCrane, currentTime);
                    storedContainers[container].RemoveContainerFromContainerRow(container);
                    storedContainers.Remove(container);

                    return container;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets a Agv with a free storage that is available to move a container.
        /// </summary>
        /// <returns>Returns the first available agv object, if None is available null is returned.</returns>
        internal Agv? GetFreeAgv()
        {
            if (AgvFree.Count > 0)
            {
                return AgvFree[0];
            }

            return null;
        }

        /// <summary>
        /// Gets a Truck from the truck queue in to the harbor.
        /// </summary>
        /// <returns>Returns the first truck in the queue in to the harbor, if there are no trucks in the queue null is returned.</returns>
        internal Truck? GetFreeTruck()
        {
            if (TrucksInQueue.Count > 0)
            {
                return TrucksInQueue[0];
            }

            return null;
        }

        /// <summary>
        /// Creates new truck objects and adds them to the TruckInQueue list.
        /// </summary>
        internal void GenerateTrucks()
        {
            for (int i = 0; i < TrucksArrivePerHour; i++)
            {
                TrucksInQueue.Add(new Truck(TruckQueueLocationID));
            }
        }

        /// <summary>
        /// Gets available crane from the harbor storage area.
        /// </summary>
        /// <returns>Returns the available crane object found in the storage area, if None is available null is returned.</returns>
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
        /// Gets the Agv containing the given container.
        /// </summary>
        /// <param name="container">The container object to be checked if it's held by an agv.</param>
        /// <returns>Returns agv object holding the given container, if no Agv is holding the given container null is returned</returns>
        internal Agv? GetAgvContainingContainer(Container container)
        {
            foreach (Agv agv in AgvWorking)
            {
                if (agv.Container == container)
                {
                    return agv;
                }
            }

            return null;
        }

        /// <summary>
        /// Docks the ship with the given ID to a loading dock.
        /// </summary>
        /// <param name="shipID">Guid object representing the unique ID of ship that is to be docked</param>
        /// <returns>Returns a Guid object representing the dock the ship got docked to, if no available loading dock matching the ShipSize enum exists an empty Guid is returned.</returns>
        internal Guid DockShipToLoadingDock(Guid shipID)
        {
            Ship shipToBeDocked = GetShipFromAnchorage(shipID);

            ShipSize size = shipToBeDocked.ShipSize;
            LoadingDock? loadingDock;

            if (FreeLoadingDockExists(size))
            {
                loadingDock = GetFreeLoadingDock(size);

                if (loadingDock != null)
                {
                    loadingDock.DockedShip = shipToBeDocked.ID;
                    loadingDock.Free = false;

                    shipToBeDocked.CurrentLocation = loadingDock.ID;

                    shipsInLoadingDock.Add(shipToBeDocked, loadingDock);

                    RemoveShipFromAnchorage(shipToBeDocked.ID);
                    RemoveLoadingDockFromFreeLoadingDocks(loadingDock.ID);

                    return loadingDock.ID;
                }
            }

            return Guid.Empty;
        }

        /// <summary>
        /// Undocks the given ship from a ShipDock and docks it to a LoadingDock
        /// </summary>
        /// <param name="shipID">Guid object representing the unique ID of ship to be undocked from a ship dock and docked to a loading dock.</param>
        /// <returns>Returns a Guid object representing the ID of the loading dock the ship gets docked to, if available loading dock matching the ship's size does not exist an empty Guid object is returned.</returns>
        internal Guid DockShipFromShipDockToLoadingDock(Guid shipID)
        {
            Ship shipToBeDocked = GetShipFromShipDock(shipID);

            ShipSize size = shipToBeDocked.ShipSize;
            LoadingDock? loadingDock;

            if (FreeLoadingDockExists(size))
            {
                loadingDock = GetFreeLoadingDock(size);

                if (loadingDock != null)
                {
                    loadingDock.DockedShip = shipToBeDocked.ID;
                    loadingDock.Free = false;

                    shipToBeDocked.CurrentLocation = loadingDock.ID;

                    shipsInLoadingDock.Add(shipToBeDocked, loadingDock);

                    UnDockShipFromShipDockToLoadingDock(shipID);

                    RemoveLoadingDockFromFreeLoadingDocks(loadingDock.ID);

                    return loadingDock.ID;
                }
            }

            return Guid.Empty;
        }

        /// <summary>
        /// Docks the ship with the given ID to an available ShipDock.
        /// </summary>
        /// <param name="shipID">Guid object representing the ID of the ship to be docked to an available ship dock.</param>
        /// <returns>Returns a Guid object representing the ID of the ship dock the ship gets docked to, if available ship dock matching the ship's size does not exist an empty Guid object is returned.</returns>
        internal Guid DockShipToShipDock(Guid shipID)
        {
            Ship? shipToBeDocked = GetShipFromLoadingDock(shipID) ?? GetShipFromAnchorage(shipID);
            LoadingDock? loadingDock = GetLoadingDockContainingShip(shipID);
            ShipSize size = shipToBeDocked.ShipSize;
            ShipDock? shipDock;

            if (FreeShipDockExists(size))
            {

                shipDock = GetFreeShipDock(size);

                if (shipDock != null)
                {
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
            }

            return Guid.Empty;
        }

        /// <summary>
        /// Moves a ship with the given ID from the Anchorage to an available LoadingDock.
        /// </summary>
        /// <param name="shipID">Guid object representing the ID of ship object to be moved to an available loading dock.</param>
        /// <returns>Returns a Guid object representing the ID of the LoadingDock the ship gets docked to, if no LoadingDocks matching the ship's size is available an empty Guid object is returned.</returns>
        internal Guid MoveShipFromAnchorageToLoadingDock(Guid shipID)
        {
            Ship? shipToBeDocked = GetShipFromAnchorage(shipID);
            ShipSize size = shipToBeDocked.ShipSize;
            LoadingDock? loadingDock;

            if (FreeLoadingDockExists(size))
            {
                loadingDock = GetFreeLoadingDock(size);

                if (loadingDock != null)
                {
                    loadingDock.DockedShip = shipToBeDocked.ID;
                    loadingDock.Free = false;
                    shipToBeDocked.CurrentLocation = loadingDock.ID;

                    shipsInLoadingDock.Add(shipToBeDocked, loadingDock);
                    freeLoadingDocks.Remove(loadingDock);
                    RemoveShipFromAnchorage(shipID);

                    return loadingDock.ID;
                }

            }

            return Guid.Empty;

        }

        /// <summary>
        /// Undocks the given ship from a loading dock and puts it in to transit to its cargo delivery destination.
        /// </summary>
        /// <param name="shipID">Guid representing the ID of the ship and put in to transit.</param>
        /// <returns>Returns a Guid object representing the ID of the loading dock the ship gets docked from, if the ship is not found an empty Guid is returned.</returns>
        internal Guid UnDockShipFromLoadingDockToTransit(Guid shipID)
        {
            Ship? shipToBeUndocked = GetShipFromLoadingDock(shipID);

            if (shipToBeUndocked != null)
            {
                LoadingDock? loadingDock = shipsInLoadingDock[shipToBeUndocked];

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
        /// Undock Ship from Anchorage and puts it in to transit to its cargo delivery destination.
        /// </summary>
        /// <param name="shipID">Guid representing the ID of the ship to be undocked from anchorage and put in to transit.</param>
        /// <returns>Returns a Guid object representing the ID of the Anchorage, if the ship with the given ID was not found in the anchorage an empty Guid is returned.</returns>
        internal Guid UnDockShipFromAnchorageToTransit(Guid shipID)
        {
            Ship? shipToBeUndocked = GetShipFromAnchorage(shipID);

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
        /// Gets the ship with the given ID from anchorage.
        /// </summary>
        /// <param name="shipID">>Guid representing the ID of the ship to be found in anchorage.</param>
        /// <returns>Returns the ship object with the given ID if it is found in anchorage.</returns>
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
        /// Gets the ship with the given ID from the harbor loading docks.
        /// </summary>
        /// <param name="shipID">Guid representing the ID of the ship to be found in loading dock.</param>
        /// <returns>Returns the ship object if the ship is found in any of the harbor loading docks.</returns>
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
        /// Gets the ship with the given ID from the harbor ship docks.
        /// </summary>
        /// <param name="shipID">Guid representing the ID of the ship object to be found in any of the harbor ship docks.</param>
        /// <returns>Returns the ship object if the ship is found in any of the harbor ship dock.</returns>
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
        /// Gets the loading dock which has the ship with the given ID docked to it.
        /// </summary>
        /// <param name="shipID">Guid representing the ID of the ship object to be found in a loading dock.</param>
        /// <returns>Returns the loading dock object that contains the ship with the given ID.</returns>
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
        /// Gets a list containing all ships currently docked to a loading dock in the harbor.
        /// </summary>
        /// <returns>Returns an IList with Ship object containing all ships currently docked in loading docks.</returns>
        internal List<Ship> DockedShipsInLoadingDock()
        {
            List<Ship> ships = new();

            foreach (var item in shipsInLoadingDock)
            {
                ships.Add(item.Key);
            }

            return ships;
        }

        /// <summary>
        /// Unloads all containers from the ships cargo and generates new containers and restocks them to a ships cargo.
        /// </summary>
        /// <param name="ship">Ship object witch cargo is to be restocked.</param>
        /// <param name="time">The date and time of the simulation the restocking takes place.</param>
        internal void RestockContainers(Ship ship, DateTime time)
        {
            int size = ship.ContainersOnBoard.Count;
            Random rand = new();

            for (int i = 0; i < size; i++)
            {
                Container? container = ship.UnloadContainer();
                if (container != null)
                {
                    container.AddStatusChangeToHistory(Status.ArrivedAtDestination, time);
                    ArrivedAtDestination.Add(container);
                }
            }

            for (int i = 0; i < rand.Next(ship.ContainerCapacity / 3, ship.ContainerCapacity - 1); i++)
            {
                ship.GenerateContainer(time);
            }
        }

        /// <summary>
        /// Checks if a free loading dock matching the given size exists in the harbor.
        /// </summary>
        /// <param name="shipSize">ShipSize enum representing the Size of ships that can dock to the loading dock.</param>
        /// <returns>Returns true if there is available loading docks for the given ShipSize, false if no available loading docks exist.</returns>
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
        /// Checks if a free ship dock matching the given size exists in the harbor.
        /// </summary>
        /// <param name="shipSize">ShipSize enum representing the Size of ships that can dock to the ship dock.</param>
        /// <returns>Returns true if there is available ship docks for the given ShipSize, false if no available ship docks exist.</returns>
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
        /// Gets an available LoadingDock matching the given ShipSize.
        /// </summary>
        /// <param name="shipSize">ShipSize enum representing the Size of ships that can dock to the loading dock.</param>
        /// <returns>Returns the LoadingDock object if there is an available loading docks for the given ShipSize. If no free docks are available null is returned.</returns>
        internal LoadingDock? GetFreeLoadingDock(ShipSize shipSize)
        {

            foreach (LoadingDock loadingDock in freeLoadingDocks)
            {
                if (loadingDock.Free == true && loadingDock.Size == shipSize)
                {
                    return loadingDock;
                }

            }

            return null;
        }

        /// <summary>
        /// Gets an available ShipDock matching the given ShipSize
        /// </summary>
        /// <param name="shipSize">The shipSize enum representing the Size of ships to fit in ship dock.</param>
        /// <returns>Returns the shipdocks object if there is an available ship docks for the given ShipSize. If no free docks are available null is returned</returns>
        internal ShipDock? GetFreeShipDock(ShipSize shipSize)
        {

            foreach (ShipDock shipDock in freeShipDocks)
            {
                if (shipDock.Free == true && shipDock.Size == shipSize)
                {
                    return shipDock;
                }
            }

            return null;
        }

        /// <summary>
        /// Removes the ship with ID matching the given ID from the Anchorage.
        /// </summary>
        /// <param name="shipID">Guid object representing the ID of the ship to be removed from anchorage.</param>
        /// <returns>Returns true if specified ship object was removed from anchorage, if ship was not found in anchorage false is returned.</returns>
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
        /// Undocks the ship with the given ID from loading lock and docks it to an available ship dock.
        /// </summary>
        /// <param name="shipID">Guid object representing the ID of ship to be undocked from loading dock and docked to an available ship dock.</param>
        /// <returns>Returns a Guid object representing the ID of the loading dock the ship with the given ID undocked from, if the ship with the given ID is not found in any loading docks null is returned.</returns>
        internal Guid UnDockShipFromLoadingDockToShipDock(Guid shipID)
        {
            Ship? shipToBeUndocked = GetShipFromLoadingDock(shipID);

            if (shipToBeUndocked != null)
            {
                LoadingDock? loadingDock = shipsInLoadingDock[shipToBeUndocked];

                if (loadingDock != null)
                {
                    loadingDock.DockedShip = Guid.Empty;
                    loadingDock.Free = true;

                    ShipDock? shipDock = GetFreeShipDock(shipToBeUndocked.ShipSize);

                    if (shipDock != null)
                    {
                        shipToBeUndocked.CurrentLocation = shipDock.ID;
                        shipsInLoadingDock.Remove(shipToBeUndocked);
                        freeLoadingDocks.Add(loadingDock);
                        if (!shipsInShipDock.ContainsKey(shipToBeUndocked))
                        {
                            shipsInShipDock.Add(shipToBeUndocked, shipDock);
                        }

                        return loadingDock.ID;
                    }
                }
            }

            return Guid.Empty;
        }

        /// <summary>
        /// Undocks the ship with the given ID from a ship dock and docks it to a loading dock.
        /// </summary>
        /// <param name="shipID">Guid object representing the ID of the ship to be undocked from ship dock and docked to an available loading dock.</param>
        /// <returns>Returns a Guid object representing the ID of the ship dock the Ship undocked from, if the ship with the given ID is not found in any ship dock null is returned.</returns>
        internal Guid UnDockShipFromShipDockToLoadingDock(Guid shipID)
        {
            Ship? shipToBeUndocked = GetShipFromShipDock(shipID);

            if (shipToBeUndocked != null)
            {
                ShipDock? shipDock = shipsInShipDock[shipToBeUndocked];

                shipDock.DockedShip = Guid.Empty;
                shipDock.Free = true;

                LoadingDock? loadingDock = GetFreeLoadingDock(shipToBeUndocked.ShipSize);

                if (loadingDock != null)
                {
                    shipToBeUndocked.CurrentLocation = loadingDock.ID;
                    shipsInShipDock.Remove(shipToBeUndocked);
                    freeShipDocks.Add(shipDock);
                    if (!shipsInLoadingDock.ContainsKey(shipToBeUndocked))
                    {
                        shipsInLoadingDock.Add(shipToBeUndocked, loadingDock);
                    }

                    return shipDock.ID;
                }
            }

            return Guid.Empty;
        }

        /// <summary>
        /// Removes loading dock with the given ID from the list of free loading docks.
        /// </summary>
        /// <param name="dockID">Guid object representing the ID of the dock to be removed from list of free loading docks.</param>
        /// <returns>Returns a boolean that is true if specified dock was removed from the list of free loading docks, if specified dock was not found then false is returned.</returns>
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
        /// Returns a number indicating the count of how many free loading docks exist of the given shipSize.
        /// </summary>
        /// <param name="shipSize">The shipSize enum representing the size of ships to fit in loading docks to be counted.</param>
        /// <returns>Returns an int value representing the total amount of available loading docks of given shipSize.</returns>
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
        /// Gets a number indicating the count of how many free container spaces exist in the harbor storage area to store containers of the given size.
        /// </summary>
        /// <param name="containerSize">ContainerSize enum representing the size of the containers the containerSpace has to fit.</param>
        /// <returns>Returns an int value representing the total number of free containerspaces exist of given size.</returns>
        /// <exception cref="ArgumentException">Throws exception if an invalid ContainerSize is given.</exception>
        internal int NumberOfFreeContainerSpaces(ContainerSize containerSize)
        {
            if (containerSize == ContainerSize.None)
            {
                throw new ArgumentException("That containerSize is not meant for concrete implementation. Valid containerSize is: ContainerSize.Half or ContainerSize.Full", nameof(containerSize));
            }

            int count = 0;
            foreach (ContainerStorageRow containerRow in AllContainerRows)
            {
                count += containerRow.NumberOfFreeContainerSpaces(containerSize);
            }

            return count;
        }

        /// <summary>
        /// Gets a number indicating the count of how many occupied container spaces exist in the harbor storage area of the given size.
        /// </summary>
        /// <param name="containerSize">ContainerSize enum representing the size the containerSpace has to fit.</param>
        /// <returns>Returns an int value representing the total number of occupied containerspaces of given size exist in the harbor storage area.</returns>
        internal int GetNumberOfOccupiedContainerSpaces()
        {
            return storedContainers.Count;
        }

        /// <summary>
        /// Gets a ContainerStorageRow whitch has free space for a container of the given size
        /// </summary>
        /// <param name="containerSize">ContainerSize enum representing the size the containerRow has to have available space for.</param>
        /// <returns>Returns a ContainerStorageRow object with available space to store a container of the given size if one exist. Otherwise returns null.</returns>
        /// <exception cref="ArgumentException">Throws this exeption if an invalid ContainerSize is given</exception>
        internal ContainerStorageRow? GetContainerRowWithFreeSpace(ContainerSize containerSize)
        {
            if (containerSize == ContainerSize.None)
            {
                throw new ArgumentException("Invalid input. That containerSize is not meant for concrete implementation. Valid containerSize is: containerSize.Half or ContainerSize.Full.", nameof(containerSize));
            }

            foreach (ContainerStorageRow containerRow in AllContainerRows)
            {
                if (containerRow.CheckIfFreeContainerSpaceExists(containerSize))
                {
                    return containerRow;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the a Container of the given size from the harbor storage area.
        /// </summary>
        /// <param name="containerSize">ContainerSize enum representing the size the containers to be retrieved.</param>
        /// <returns>Returns a Container of the given Size from the harbor storage area, if container of specified size is not found null is returned.</returns>
        internal Container? GetStoredContainer(ContainerSize containerSize)
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
        /// Adds the given ship to the Anchorage.
        /// </summary>
        /// <param name="ship">Ship object to be added to anchorage.</param>
        internal void AddNewShipToAnchorage(Ship ship)
        {
            Anchorage.Add(ship);
            if (ShipsInTransit.ContainsKey(ship))
            {
                ShipsInTransit.Remove(ship);
            }
        }

        /// <summary>
        /// Returns a string with information about the status of all loading docks in the port. A loading dock is a dock used for loading cargo from and to Ships.
        /// </summary>
        /// <returns>String value containing information about the status of all the loading docks in the port.</returns>
        internal bool LoadingDockIsFree(Guid dockID)
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
        /// Gets a list containing all ships docked to loading dock.
        /// </summary>
        /// <returns>Returns a Ilist with Ship objects containing all ships docked to loading dock.</returns>
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
        /// Gets a list containing all ships docked to ship docks.
        /// </summary>
        /// <returns>Returns an Ilist with Ship objects containing all ships docked to a ship dock.</returns>
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
        /// Gets a list containing all ships in transit.
        /// </summary>
        /// <returns>Returns an Ilist with Ship objects containing all ships in transit.</returns>
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
        /// Sets the informatin of the harbor based on the values given in the constructor.
        /// </summary>
        /// <param name="listOfShips">An IList containing ship objects representing all the ships that will be used in a simulation of a harbor.</param>
        /// <param name="numberOfSmallLoadingDocks">Int value representing the amount of small loading docks that will be available in the harbor dock area.
        ///  LoadingDocks are docks where ships can load or unload their cargo from or to the harbor. Small loading docks can only recieve ships of size small.</param>
        /// <param name="numberOfMediumLoadingDocks">Int value representing the amount of medium loading that will be available in the harbor dock area.
        ///  LoadingDocks are docks where ships can load or unload their cargo from or to the harbor. Medium loading docks can only recieve ships of size medium.</param>
        /// <param name="numberOfLargeLoadingDocks">Int value representing the amount of large loadingthat will be available in the harbor dock area.
        ///  LoadingDocks are docks where ships can load or unload their cargo from or to the harbor. Large loading docks can only recieve ships of size large.</param>
        /// <param name="numberOfCranesNextToLoadingDocks">Int value representing the amount of cranes to be placed in the harbor's docking area. These cranes will be used to load
        /// or unload containers to and from ships</param>
        /// <param name="LoadsPerCranePerHour">Int value representing the amount of loads a single crane can do in one hour. A load is defined as either loading a container on the the cranes storage
        /// or unloading a container from a cranes storage.</param>
        /// <param name="numberOfCranesOnHarborStorageArea">Int value representing the amount of cranes to be placed in the harbor's own storage area for containers. These cranes will be used to
        /// load or unload containers from or to the container storage rows inside the storage area</param>
        /// <param name="numberOfSmallShipDocks">Int value representing the amount of small loading docks that will be available in the harbor dock area.
        /// Ship docks are docks where ships will permanently dock once they are done with all their voyages. A small loading dock can only recieve ships of size small. </param>
        /// <param name="numberOfMediumShipDocks">Int value representing the amount of medium loading docks that will be available in the harbor dock area.
        /// Ship docks are docks where ships will permanently dock once they are done with all their voyages. A medium loading dock can only recieve ships of size medium.</param>
        /// <param name="numberOfLargeShipDocks">Int value representing the amount of large loading docks that will be available in the harbor dock area.
        /// Ship docks are docks where ships will permanently dock once they are done with all their voyages. A large loading dock can only recieve ships of size large.</param>
        /// <param name="percentageOfContainersDirectlyLoadedFromShipToTrucks">Int value representing the percentage of containers directly loaded from ship on to trucks. Containers that are not
        /// directly loaded on to trucks will instead go to the harbor's storage area. A value of 100 represents 100%. A value of 50 represents 50%.</param>
        /// <param name="percentageOfContainersDirectlyLoadedFromHarborStorageToTrucks">Int value representing the percentage of containers directly loaded from harbor storage to trucks.
        /// Containers that are not loaded directly to trucks will instead be loaded on to Ships that will carry the cargo to its final destination. A value of 100 represents 100%. A value of 50 represents 50%.</param>
        /// /// <param name="numberOfAgv">Int value representing the amount of AGVs in the harbor to be created. AGV's are Automated Guided Viechles that can deliver containers from point A to B in the harbor area.
        /// Typicly from the harbor's docking area to the harbor's storage area.</param>
        private void Initiliaze(
            IList<Ship> listOfShips,
            int numberOfSmallLoadingDocks,
            int numberOfMediumLoadingDocks,
            int numberOfLargeLoadingDocks,
            int numberOfCranesNextToLoadingDocks,
            int LoadsPerCranePerHour,
            int numberOfCranesOnHarborStorageArea,
            int numberOfSmallShipDocks,
            int numberOfMediumShipDocks,
            int numberOfLargeShipDocks,
            int percentageOfContainersDirectlyLoadedFromShipToTrucks,
            int percentageOfContainersDirectlyLoadedFromHarborStorageToTrucks,
            int numberOfAgv)
        {
            if (percentageOfContainersDirectlyLoadedFromShipToTrucks > 100 || percentageOfContainersDirectlyLoadedFromShipToTrucks < 0)
            {
                throw new ArgumentOutOfRangeException("percentageOfContainersDirectlyLoadedFromShipToTrucks must be a number between 0 and 100");
            }

            if (percentageOfContainersDirectlyLoadedFromHarborStorageToTrucks > 100 || percentageOfContainersDirectlyLoadedFromHarborStorageToTrucks < 0)
            {
                throw new ArgumentOutOfRangeException("percentageOfContainersDirectlyLoadedFromHarborStorageToTrucks must be a number between 0 and 100");
            }

            PercentOfContainersDirectlyLoadedFromShips = percentageOfContainersDirectlyLoadedFromShipToTrucks / 100;
            PercentOfContainersDirectlyLoadedFromStorageArea = (double)percentageOfContainersDirectlyLoadedFromHarborStorageToTrucks / 100;

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

            for (int i = 0; i < numberOfAgv; i++)
            {
                AgvFree.Add(new(HarborDockAreaID));
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

            freeShipDocks = allShipDocks.ToList();
            freeLoadingDocks = allLoadingDocks.ToList();
        }
    }
}