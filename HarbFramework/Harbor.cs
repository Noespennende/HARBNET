﻿using Gruppe8.HarbNet.Advanced;
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
    /// Harbor to be used in a simulation. A Harbor represents the place where all activety in the simulation takes place. 
    /// </summary>
    public class Harbor : Port
    {
        /// <summary>
        /// Gets the unique ID for the harbor.
        /// </summary>
        /// <return>Returns a Guid object representing the harbors unique ID.</return>
        public override Guid ID { get; internal set; } = Guid.NewGuid();
        /// <summary>
        /// Gets a list containing all loadingdocks in the harbor. A loading dock is a place where ships can dock to the harbor
        /// to load and/or unload their cargo from/to the Harbor.
        /// </summary>
        /// <return>Returns an IList with LoadingDock objects representing the all the loadingdocks in the harbor.</return>
        internal IList<LoadingDock> allLoadingDocks = new List<LoadingDock>();
        /// <summary>
        /// Gets a list containing all the loadingdocks that are free for ships to dock to them.
        /// A loading dock is a place where ships can dock to the harbor to load and/or unload their cargo from/to the Harbor.
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
        /// docks to get free in the Harbor or when they are done with their trip and have no new trips to make.
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
        /// Gets a list containing all cranes in the Harbor storage area.
        /// </summary>
        /// <return>Returns an Ilist with Crane objects representing the cranes in the harbor's storage area.</return>
       internal IList<Crane> HarborStorageAreaCranes { get; set; } = new List<Crane>();
        /// <summary>
        /// Get a list containing all the containers that have left the harbor and arived at their final destination
        /// </summary>
        /// <return>Returns a IList of container objects that represent all the containers that have arrived at their final destination during the simulation.</return>
        public override IList<Container> ArrivedAtDestination { get; internal set; } = new List<Container>();
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
        /// get brought to the Harbor storage area for storage.
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
        /// Creates new harbor object.
        /// </summary>
        /// <param name="listOfShips">An IList containing ship objects to be created in harbor.</param>
        /// <param name="listOfContainerStorageRows">An IList containing ContainerStorageRows objects to be created in the harbor.</param>
        /// <param name="numberOfSmallLoadingDocks">Int value representing the amount of small loading docks to be created in the harbor.</param>
        /// <param name="numberOfMediumLoadingDocks">Int value representing the amount of medium loading docks to be created in the harbor.</param>
        /// <param name="numberOfLargeLoadingDocks">Int value representing the amount of large loading docks to be created in the harbor.</param>
        /// <param name="numberOfCranesNextToLoadingDocks">Int value representing the amount of cranes next to loading docks to be created in the harbor.</param>
        /// <param name="LoadsPerCranePerHour">Int value representing the amount of loads a crane does per hour.</param>
        /// <param name="numberOfCranesOnHarborStorageArea">Int value representing the amount of cranes in the harbor storage area to be created in the harbor.</param>
        /// <param name="numberOfSmallShipDocks">Int value representing the amount of small ship docks to be created in the harbor.</param>
        /// <param name="numberOfMediumShipDocks">Int value representing the amount of medium ship docks to be created in the harbor.</param>
        /// <param name="numberOfLargeShipDocks">Int value representing the amount of large ship docks to be created in the harbor.</param>
        /// <param name="numberOfTrucksArriveToHarborPerHour">Int value representing the amount of trucks arriving to the harbor per hour</param>
        /// <param name="percentageOfContainersDirectlyLoadedFromShipToTrucks">Int value representing the percentage of containers directly loaded from ship to trucks.</param>
        /// <param name="percentageOfContainersDirectlyLoadedFromHarborStorageToTrucks">Int value representing the percentage of containers directly loaded from harbor storage to trucks.</param>
        /// <param name="numberOfAgvs">Int value representing the amount of AGVs in the harbor to be created.</param>
        /// <param name="loadsPerAgvPerHour">Int value representing the amount of loads each AGV does per hour.</param>
        /// <exception cref="ArgumentOutOfRangeException">Exception to be thrown in Harbor if parameter is out of set range.</exception>
        public Harbor(IList<Ship> listOfShips, IList<ContainerStorageRow> listOfContainerStorageRows, int numberOfSmallLoadingDocks, int numberOfMediumLoadingDocks, int numberOfLargeLoadingDocks,
            int numberOfCranesNextToLoadingDocks, int LoadsPerCranePerHour, int numberOfCranesOnHarborStorageArea,
            int numberOfSmallShipDocks, int numberOfMediumShipDocks, int numberOfLargeShipDocks, int numberOfTrucksArriveToHarborPerHour,
            int percentageOfContainersDirectlyLoadedFromShipToTrucks, int percentageOfContainersDirectlyLoadedFromHarborStorageToTrucks,
            int numberOfAgvs, int loadsPerAgvPerHour)
        {

            this.TrucksArrivePerHour = numberOfTrucksArriveToHarborPerHour;
            this.AllContainerRows = listOfContainerStorageRows.ToList();
            this.LoadsPerAgvPerHour = loadsPerAgvPerHour;

            AllShips = listOfShips.ToList();

            Initiliaze(
                listOfShips, numberOfSmallLoadingDocks, numberOfMediumLoadingDocks, numberOfLargeLoadingDocks, numberOfCranesNextToLoadingDocks, 
                LoadsPerCranePerHour, numberOfCranesOnHarborStorageArea, numberOfSmallShipDocks, numberOfMediumShipDocks, numberOfLargeShipDocks, 
                percentageOfContainersDirectlyLoadedFromShipToTrucks, percentageOfContainersDirectlyLoadedFromHarborStorageToTrucks, numberOfAgvs
                );

        }

        /// <summary>
        /// Creates new harbor object. Overload constructor where ships and container storage rows are automatically created, and default values are used.
        /// </summary>
        /// <param name="numberOfShips">Int value representing the number of ships to be created.</param>
        /// <param name="numberOfHarborContainerStorageRows">Int value representing the amount of rows the harbor storage contains.</param>
        /// /// <param name="containerStorageCapacityInEachStorageRow">Int value representing the amount of containers the harbor storage can store.</param>
        /// <param name="numberOfLoadingDocks">Int value representing the number of loading docks. Number is distributed between small, medium and large loading docks.</param>
        /// <param name="numberOfCranesNextToLoadingDocks">Int value representing the amount of cranes next to loading docks to be created in the harbor.</param>
        /// <param name="numberOfCranesOnHarborStorageArea">Int value representing the amount of cranes in the harbor storage area to be created in the harbor.</param>
        /// <param name="numberOfAgvs">Int value representing the amount of AGVs in the harbor to be created.</param>
        /// <param name="loadsPerCranePerHour">Int value representing the amount of loads a crane does per hour.</param>
        /// <param name="loadsPerAgvPerHour">Int value representing the amount of loads each AGV does per hour.</param>
        /// <param name="percentageOfContainersDirectlyLoadedFromShipToTrucks">Int value representing the percentage of containers directly loaded from ship to trucks.</param>
        /// <param name="percentageOfContainersDirectlyLoadedFromHarborStorageToTrucks">Int value representing the percentage of containers directly loaded from harbor storage to trucks.</param>
        /// <exception cref="ArgumentOutOfRangeException">Exception to be thrown in Harbor if parameter is out of set range.</exception>
        /// 
        public Harbor(int numberOfShips, int numberOfHarborContainerStorageRows, int containerStorageCapacityInEachStorageRow, int numberOfLoadingDocks, 
            int numberOfCranesNextToLoadingDocks, int numberOfCranesOnHarborStorageArea, int numberOfAgvs, 
            int loadsPerCranePerHour = 35, int numberOftrucksArriveToHarborPerHour = 10, int loadsPerAgvPerHour = 25, 
            int percentageOfContainersDirectlyLoadedFromShipToTrucks = 10, int percentageOfContainersDirectlyLoadedFromHarborStorageToTrucks = 15) 
        {
            

            // ** Creating Ships ** 

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

            // ** Creating ContainerRows (Storage Area) ** 

            List<ContainerStorageRow> listOfContainerStorageRows = new();

            for (int i = 0; i < numberOfHarborContainerStorageRows; i++)
            {
                listOfContainerStorageRows.Add(new(containerStorageCapacityInEachStorageRow));
            }

            // ** Loading Docks ** 

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

            this.TrucksArrivePerHour = numberOftrucksArriveToHarborPerHour;
            this.LoadsPerAgvPerHour = loadsPerAgvPerHour;
            this.AllContainerRows = listOfContainerStorageRows.ToList();


            Initiliaze(
                listOfShips, 
                smallLoadingDocks, mediumLoadingDocks, largeLoadingDocks, numberOfCranesNextToLoadingDocks, loadsPerCranePerHour, numberOfCranesOnHarborStorageArea, 
                0, 0, 0, 
                percentageOfContainersDirectlyLoadedFromShipToTrucks, percentageOfContainersDirectlyLoadedFromHarborStorageToTrucks, numberOfAgvs
                );

        }


        private void Initiliaze(IList<Ship> listOfShips, int numberOfSmallLoadingDocks, int numberOfMediumLoadingDocks, int numberOfLargeLoadingDocks, int numberOfCranesNextToLoadingDocks, 
            int LoadsPerCranePerHour, int numberOfCranesOnHarborStorageArea, int numberOfSmallShipDocks, int numberOfMediumShipDocks, int numberOfLargeShipDocks, 
            int percentageOfContainersDirectlyLoadedFromShipToTrucks, int percentageOfContainersDirectlyLoadedFromHarborStorageToTrucks, int numberOfAgv)
        {
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


        /// <summary>
        /// Creates container space in harbor based on the amount of numberOfContainerSpace and numberOfContainerRows.
        /// </summary>
        /// <param name="numberOfContainerSpaces">Int value representing the amount of container spaces to be created in harbor.</param>
        /// <param name="numberOfContainerRows">Int value representing the amount of container rows to be created in harbor.</param>
        internal void CreateContainerSpaces(int numberOfContainerSpaces, int numberOfContainerRows)
        {
            IList <ContainerStorageRow> containerRows = new List <ContainerStorageRow>();

            for (int j = 0; j < numberOfContainerRows; j++)
            {
                containerRows.Add(new ContainerStorageRow(numberOfContainerSpaces));
            }

            this.AllContainerRows = containerRows;
        }

        /// <summary>
        /// Loads container from ship to crane.
        /// </summary>
        /// <param name="ship">Ship object the container is unloaded from.</param>
        /// <param name="crane">Crane object that loads the container.</param>
        /// <param name="currentTime">The Date and time container is loaded from ship to crane.</param>
        /// <returns>Returns the container object to be loaded from ship to crane.</returns>
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
        /// Loads container from crane to ship.
        /// </summary>
        /// <param name="ship">Ship object that loads the container.</param>
        /// <param name="crane">Crane object the container is unloaded from.</param>
        /// <param name="currentTime">The Date and time container is loaded from crane to ship.</param>
        /// <returns>Returns the container object to be loaded from crane to ship.</returns>
        internal Container CraneToShip(Crane crane, Ship ship, DateTime currentTime)
        {
            Container containerToBeLoaded = crane.UnloadContainer();
            containerToBeLoaded.CurrentLocation = crane.Location;
            containerToBeLoaded.AddStatusChangeToHistory(Status.Loading, currentTime);
            ship.AddContainer(containerToBeLoaded);
            
            return containerToBeLoaded;
        }

        /// <summary>
        /// Loads container from crane to truck.
        /// </summary>
        /// <param name="crane">Crane object the container is unloaded from.</param>
        /// <param name="truck">Truck object that loads the container.</param>
        /// <param name="currentTime">The Date and time container is loaded from crane to truck.</param>
        /// <returns>Returns the container object to be loaded from crane to truck.</returns>
        /// <exception cref="TruckCantBeLoadedException">Exception to be thrown if truck can't be loaded.</exception>
        internal Container CraneToTruck(Crane crane, Truck truck, DateTime currentTime)
        {
            if (!TrucksInQueue.Contains(truck))
            {
                if (TrucksInTransit.Contains(truck))
                {
                    throw new TruckCantBeLoadedException("The truck you are trying to load is already in transit away from the harbor and therefore can't load the container from the crane");
                } else
                {
                    throw new TruckCantBeLoadedException("The truck you are trying to load does not exist in the simulation and therefore can't load the container from the crane.");
                }
                
            }

            if (!(truck.Container == null))
            {
                throw new TruckCantBeLoadedException("The truck you are trying to load already has a container in its storage and therefore don't have room for the container from the given crane");
            }

            Container containerToBeLoaded = crane.UnloadContainer();
            containerToBeLoaded.CurrentLocation = crane.Location;
            truck.LoadContainer(containerToBeLoaded);
            containerToBeLoaded.AddStatusChangeToHistory(Status.LoadingToTruck, currentTime);
            

            return containerToBeLoaded;
        }

        /// <summary>
        /// Gets all available cranes from loading dock.
        /// </summary>
        /// <returns>Returns crane from docked cranes if the crane holds on no container, if crane holds on container then null is returned.</returns>
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
        /// <param name="truck">Truck object to be removed from truck queue.</param>
        internal void RemoveTruckFromQueue(Truck truck)
        {
            TrucksInQueue.Remove(truck);
        }

        /// <summary>
        /// Sends truck to transit.
        /// </summary>
        /// <param name="loadingDock">The loading dock object the truck is being sent to for transit.</param>
        /// <param name="container">The container object to be transported on truck.</param>
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
        /// Sends truck to transit.
        /// </summary>
        /// <param name="container">The container object to be transported on truck.</param>
        /// <returns>Returns the truck object transporting the dedined container object, if the truck is not transporting the container null is returned.</returns>
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
        /// Amount of containers in the ships storage.
        /// </summary>
        /// <returns>Returns an int value representing the amount of containers in the ships storage.</returns>
        internal int NumberOfContainersInStorageToShips()
        {
            int numberOfContainersToShips = storedContainers.Count - NumberOfContainersInStorageToTrucks();

            return numberOfContainersToShips;
        }

        /// <summary>
        /// Amount of containers in the trucks storage.
        /// </summary>
        /// <returns>Returns an int value representing the amount of containers in the trucks storage.</returns>
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
        /// Loads container from Crane to Agv
        /// </summary>
        /// <param name="crane">The crane object the container is unloaded from.</param>
        /// <param name="agv">The agv object that loads the container.</param>
        /// <param name="currentTime">The Date and Timestamp the container is loaded from crane to agv.</param>
        /// <returns>Returns the container object to be loaded from crane to agv.</returns>
        /// <exception cref="AgvCantBeLoadedException">Exception to be thrown if Agv can't be loaded.</exception>
        internal Container CraneToAgv(Crane crane, Agv agv, DateTime currentTime)
        {
            if (!AgvFree.Contains(agv))
            {
                if (AgvWorking.Contains(agv))
                {
                    throw new AgvCantBeLoadedException("The AGV you are trying to load is already transporting goods and therefore can not load a container from the crane.");
                } else
                {
                    throw new AgvCantBeLoadedException("The AGV you are trying to load does not exist in the simulation and therefore can't be loaded.");
                }
                
            }

            if (!(agv.Container == null))
            {
                throw new AgvCantBeLoadedException("The AGV given already has a container in its storage and therefore has no room for the container the crane is trying to load.");
            }
            Container containerToBeLoaded = crane.UnloadContainer();
            containerToBeLoaded.CurrentLocation = crane.Location;
            agv.LoadContainer(containerToBeLoaded);
            containerToBeLoaded.AddStatusChangeToHistory(Status.LoadingToAgv, currentTime);

            AgvFree.Remove(agv);
            AgvWorking.Add(agv);

            return containerToBeLoaded;
        }
        /// <summary>
        /// Loads container from Agv to Crane.
        /// </summary>
        /// <param name="crane">The crane object that loads the container.</param>
        /// <param name="agv">The Agv object the container is unloaded from.</param>
        /// <param name="currentTime">The Date and Timestamp the container is loaded.</param>
        /// <returns>Returns the container object to be loaded from crane to agv.</returns>
        /// <exception cref="CraneCantBeLoadedException">Exception the be thrown if Crane can't be loaded.</exception>
        internal Container AgvToCrane(Crane crane, Agv agv, DateTime currentTime)
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
                } else
                {
                    throw new CraneCantBeLoadedException("The AGV you are trying to unload does not exist within the simulation and therefore can not unload to the crane");
                }
            }

            if (!(crane.Container == null))
            {
                throw new CraneCantBeLoadedException("The crane you are trying to load already has a container in its storage and therefore has no room to load the container from the AGV");
            }

            Container containerToBeLoaded = agv.UnloadContainer();
            containerToBeLoaded.CurrentLocation = agv.Location;
            crane.LoadContainer(containerToBeLoaded);
            containerToBeLoaded.AddStatusChangeToHistory(Status.LoadingToCrane, currentTime);

            AgvWorking.Remove(agv);
            AgvFree.Add(agv);

            return containerToBeLoaded;
        }

        /// <summary>
        /// Loads container from crane to container storage row if there is available space.
        /// </summary>
        /// <param name="crane">Crane object container is unloaded from</param>
        /// <param name="currentTime">The current time container is loaded.</param>
        /// <returns>Returns True if there is available space for the container and it's size to be loaded to, or false if nothing is available.</returns>
        internal bool CraneToContainerRow(Crane crane,DateTime currentTime)
        {
            Container container = crane.UnloadContainer();

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

            crane.LoadContainer(container);
            return false;
        }

        /// <summary>
        /// Loads container from container row to crane.
        /// </summary>
        /// <param name="size">The containerSize enum representing the Size of container to be created.</param>
        /// <param name="crane">Crane object that loads the container.</param>
        /// <param name="currentTime">The date and time the container is loaded.</param>
        /// <returns>Returns the container object to be loaded from container row to crane, if container is not found then null is returned.</returns>
        /// <exception cref="CraneCantBeLoadedException">Throws exception if crane can't be loaded.</exception>
        internal Container? ContainerRowToCrane(ContainerSize size, Crane crane,DateTime currentTime)
        {
            if (!(crane.Container == null))
            {
                throw new CraneCantBeLoadedException("The crane you are trying to load already has a container in its storage and therefore has no room to load the container from the harbor storage area");
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
        /// Gets available agv.
        /// </summary>
        /// <returns>Returns the first available agv object, if none is available null is returned.</returns>
        internal Agv? GetFreeAgv()
        {
            if (AgvFree.Count > 0)
            {
                return AgvFree[0];
            }

            return null;
        }

        /// <summary>
        /// Gets available truck.
        /// </summary>
        /// <returns>Returns the first available truck found, if none is available null is returned.</returns>
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
        /// Gets available crane from the storage area.
        /// </summary>
        /// <returns>Returns the available crane object found in the storage area, if none is available null is returned.</returns>
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
        /// Gets agv containing a container.
        /// </summary>
        /// <param name="container">The container object to be checked if it's held by an agv.</param>
        /// <returns>Returns agv object if it hold an container, if it's not holding an container then null is returned</returns>
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
        /// Docks ship to loading dock.
        /// </summary>
        /// <param name="shipID">unique ID of ship object to be docked</param>
        /// <returns>Returns a Guid object representing the dock the ship gets docked to, if available loading dock matching the ShipSize enum does not exist nothing is returned.</returns>
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
        /// Docking ship from ship dock to loading dock.
        /// </summary>
        /// <param name="shipID">Unique ID of ship object to be docked from ship dock to loading dock.</param>
        /// <returns>Returns a Guid object representing the loading dock the ship gets docked to, if available loading dock matching the ShipSize enum does not exist nothing is returned.</returns>
        internal Guid DockShipFromShipDockToLoadingDock(Guid shipID)
        {
            Ship shipToBeDocked = GetShipFromShipDock(shipID);

            ShipSize size = shipToBeDocked.ShipSize;
            LoadingDock? loadingDock;

            if (FreeLoadingDockExists(size))
            {
                loadingDock = GetFreeLoadingDock(size);

                if(loadingDock != null)
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
        /// Ship gets docked to ship dock.
        /// </summary>
        /// <param name="shipID">Unique ID of the ship object to be docked to ship dock.</param>
        /// <returns>Returns a Guid object representing the ship dock object the ship gets docked to, if available ship dock matching the ShipSize enum does not exist nothing is returned.</returns>
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
        /// <param name="shipID">Unique ID of ship object to be moved to loading dock.</param>
        /// <returns>Returns a Guid object representing the ladong dock the ship gets docked to, if available loading dock matching the ShipSize enum does not exist nothing is returned.</returns>

        internal Guid StartShipInLoadingDock(Guid shipID)
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
                    freeLoadingDocks.Remove(loadingDock);
                    RemoveShipFromAnchorage(shipID);

                    return loadingDock.ID;
                }

            }

            return Guid.Empty;

        }

        /// <summary>
        /// Ship in loading dock got get moved to dock for ships in transit.
        /// </summary>
        /// <param name="shipID">Unique ID of the ship object to be moved to transit.</param>
        /// <returns>Returns a Guid object representing the loading dock the ship gets docked from, if correct dock does not exist nothing is returned.</returns>
        internal Guid UnDockShipFromLoadingDockToTransit(Guid shipID)
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
        /// <param name="shipID">Unique ID of the ship object to be undocked from anchorage to transit.</param>
        /// <returns>Returns the Guid of the Anchorage the ship object was undocked from, if there is no ships to be undocked from anchorage null is returned.</returns>
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
        /// Gets specific ship object from anchorage.
        /// </summary>
        /// <param name="shipID">Unique ID of the ship object to be found in anchorage.</param>
        /// <returns>Returns the ship object if ship is found in anchorage.</returns>
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
        /// <param name="shipID">Unique ID of the ship object to be found in loading dock.</param>
        /// <returns>Returns the ship object if the ship is found in loading dock.</returns>
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
        /// <param name="shipID">Unique ID of the ship object to be found in ship docks.</param>
        /// <returns>Returns the ship object if ship is found in ship dock.</returns>
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
        /// Gets loading dock that has the specified ship docked.
        /// </summary>
        /// <param name="shipID">Unique ID of the ship object to be found in loading dock.</param>
        /// <returns>Returns the loading dock object that contains the specified ship.</returns>
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
        /// Creates an IList containing ship objects of all docked ships in loading docks.
        /// </summary>
        /// <returns>Returns an IList with Ship object containing all ships docked in loading dock.</returns>
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
        /// New containers are generated when container arrives at destination.
        /// </summary>
        /// <param name="ship">Ship object transfering containers to their destination.</param>
        /// <param name="time">The date and time new containers are generated.</param>
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

            for (int i = 0; i < rand.Next(ship.ContainerCapacity/3, ship.ContainerCapacity - 1); i++)
            {
                ship.GenerateContainer(time);
            }


        }

        /// <summary>
        /// Checks if a loading dock matching the specified shipsize enum is available.
        /// </summary>
        /// <param name="shipSize">The shipSize enum representing the Size of ships to fit in loading dock.</param>
        /// <returns>Returns true if there is available loading docks for the defined shipsize, false if there is no available loading docks.</returns>
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
        /// Checks if a free ship dock matching the specified shipsize enum is available.
        /// </summary>
        /// <param name="shipSize">The shipSize enum representing the Size of ships to fit in ship dock.</param>
        /// <returns>Returns true if there is available ship docks for the defined shipsize, false if there is no available ship docks.</returns>
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
        /// Gets all free loading dock matching the specified shipsize.
        /// </summary>
        /// <param name="shipSize">The shipSize enum representing the Size of ships to fit in loading dock.</param>
        /// <returns>Returns loadingdock if there is available loading docks for the defined shipsize, if no docks are available null is returned.</returns>
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
        /// Gets all free ship dock matching the specified shipsize.
        /// </summary>
        /// <param name="shipSize">The shipSize enum representing the Size of ships to fit in ship dock.</param>
        /// <returns>Returns the shipdocks if there is available ship docks for the defined shipsize.</returns>
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
        /// <param name="shipID">Unique ID of the ship object to be removed from anchorage.</param>
        /// <returns>Returns true if specified ship object was in anchorage, if ship was not found in anchorage false is returned.</returns>
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
        /// <param name="shipID">Unique ID of ship to be undocked from loading dock to ship dock.</param>
        /// <returns>Returns a Guid object representing the loading dock the ship object undocked from, if there are no ship objects to be undocked null is returned.</returns>
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
        /// <param name="shipID">Unique ID of the ship object to be undocked from ship dock to loading dock.</param>
        /// <returns>Returns a Guid object representing the ship dock the ship object undocked from, if there are no ships to be undocked null is returned.</returns>
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
        /// Removes loading dock from list of free loading docks.
        /// </summary>
        /// <param name="dockID">Unique ID the of dock to be removed from list of available loading docks.</param>
        /// <returns>Returns a boolean that is true if specified dock was in the list of free loading docks, if specified dock was not in the list false is returned.</returns>
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
        /// Counts the number of available loading docks of the specified shipSize enum.
        /// </summary>
        /// <param name="shipSize">The shipSize enum representing the size of ships to fit in loading dock.</param>
        /// <returns>Returns an int value representing the total amount of available loading docks of specified shipSize enum.</returns>
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
        /// <param name="containerSize">The containerSize enum representing the size the containerSpace has to fit.</param>
        /// <returns>Returns an int value representing the total number of available loading containerspaces of specified size.</returns>
        /// <exception cref="ArgumentException">Throws exception if containerSize is not found.</exception>
        internal int NumberOfFreeContainerSpaces(ContainerSize containerSize)
        {
            if (containerSize == ContainerSize.None)
            {
                throw new ArgumentException("Invalid input. That containerSize does not exist. Valid containerSize is: ContainerSize.Half or ContainerSize.Full", nameof(containerSize));
            }

            int count = 0;
            foreach (ContainerStorageRow containerRow in AllContainerRows)
            {
                count += containerRow.NumberOfFreeContainerSpaces(containerSize);
            }

            return count;
        }

        /// <summary>
        /// Counts the number of occupied containerspaces of specified size.
        /// </summary>
        /// <param name="containerSize">The containerSize enum representing the size the containerSpace has to fit.</param>
        /// <returns>Returns an int value representing the total number of occupied loading containerspaces of specified size.</returns>
        internal int GetNumberOfOccupiedContainerSpaces(ContainerSize containerSize)
        {
            return storedContainers.Count;

        } //returnerer antallet okuperte plasser av den gitte typen

        /// <summary>
        /// Gets the available container row of specified containerSize.
        /// </summary>
        /// <param name="containerSize">The containerSize enum representing the size the containerRow has to fit.</param>
        /// <returns>Returns a Guid representing the available containerRow of specified size.</returns>
        /// <exception cref="ArgumentException">Throws exception if containerSize is not found.</exception>
        internal ContainerStorageRow GetContainerRowWithFreeSpace(ContainerSize containerSize)
        {
            foreach (ContainerStorageRow containerRow in AllContainerRows)
            {
                if (containerRow.CheckIfFreeContainerSpaceExists(containerSize))
                {
                    return containerRow;
                }
            }
            throw new ArgumentException("Invalid input. That containerSize does not exist. Valid containerSize is: containerSize.Small, containerSize.Medium or containerSize.Large.", nameof(containerSize));

        } //returnerer en Guid til en ledig plass av den gitte typen

        /// <summary>
        /// Gets the stored containers of specified containerSize.
        /// </summary>
        /// <param name="containerSize">The containerSize enum representing the size the containers to be retrieved.</param>
        /// <returns>Returns a Guid object representing the stored container objects of specified containerSize, if container of specified size is not found null is returned.</returns>
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
        /// Unloads container of specific containerSize from ship to ContainerSpace in ContainerStorageRow.
        /// </summary>
        /// <param name="containerSize">The containerSize enum representing the size the containers to be unloaded from ship to containerspace.</param>
        /// <param name="ship">The ship object the container is unloaded from.</param>
        /// <param name="currentTime">The date and time the container is unloaded from ship to available containerspace.</param>
        /// <returns>Returns a Guid object representing the containerspaces the unloaded container was stored in, if there were no containers to unload from the ship empty is returned.</returns>
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
            containerToBeUnloaded.CurrentLocation = containerSpace.ID;
            containerToBeUnloaded.AddStatusChangeToHistory(Status.InStorage, currentTime);

            return containerSpace.ID;

        }

        
        /// <summary>
        /// Adds new ship object to anchorage.
        /// </summary>
        /// <param name="ship">Ship object to be added to anchorage.</param>
        internal void AddNewShipToAnchorage(Ship ship)
        {
            Anchorage.Add(ship);
            if (ShipsInTransit.ContainsKey(ship))
                ShipsInTransit.Remove(ship);
        }

        /// <summary>
        /// Gets last registered status of specific ship object.
        /// </summary>
        /// <param name="ShipID">Unique ID of the ship object to get status from.</param>
        /// <returns>Returns a Status enum representing the last registered status of specified ship if the ship has a history, if no status is registered null is returned.</returns>
        public override Status GetShipStatus(Guid ShipID)
        {
            StatusLog? lastStatusChange = null;
            StringBuilder sb = new();
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
        /// Gets the status of all shipdocks if they are avilable or not.
        /// </summary>
        /// <returns>Returns a dictionary containing the Guid of the ship docks and bool values representing the availability of the ship docks.</returns>
        internal IDictionary<Guid, bool> StatusAllShipDocks()
        {
            IDictionary<Guid, bool> dockStatus = new Dictionary<Guid, bool>();
            foreach(ShipDock shipDock in allShipDocks)
            {
                dockStatus[shipDock.ID] = shipDock.Free;
            }
            return dockStatus;
        }

        /// <summary>
        /// Gets the status of all loading docks if they are available or not.
        /// </summary>
        /// <returns>Returns a string value representing all the dock IDs and if they are available or not.</returns>
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
            return dockStatus.ToString();
        }

        /// <summary>
        /// Gets the status of specified container.
        /// </summary>
        /// <param name="ContainerId">Unique ID of the container object to get last registered status from.</param>
        /// <returns>Returns a string value representing the container ID and their last registered status from Status enums.</returns>
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
        /// Gets the status of all containers.
        /// </summary>
        /// <returns>Returns a string value representing the container ID and their last registered status from Status enums.</returns>
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
        /// Returns an IDictionary with the IDs of all loading docks, and their current free-status.
        /// </summary>
        /// <returns>Returns an IDictionary containing Guid representing the loading docks and bool value representing if the loading docks are available or not.</returns>
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
        /// Checks if specified loading dock is available.
        /// </summary>
        /// <param name="dockID">Unique ID of the dock object to be checked if available.</param>
        /// <returns>Returns a boolean that is true if specified loading dock is free, or false if it's not.</returns>
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

        /// <summary>
        /// Returns an IDictionary with the IDs of all ship docks, and their current free-status.
        /// </summary>
        /// <returns>Returns an IDictionary containing Guid representing the ship docks and bool value representing if the ship docks are available or not.</returns>
        public override IDictionary<Guid, bool> GetAvailabilityStatusForAllShipDocks()
        {
            Dictionary<Guid, bool> availabilityStatuses = new();

            foreach (ShipDock shipDock in allShipDocks)
            {
                availabilityStatuses.Add(shipDock.ID, shipDock.Free);
            }

            return availabilityStatuses;
        }

        /// <summary>
        /// Gets the last registered status from all ships.
        /// </summary>
        /// <returns>Return an IDictionary containing Ship objects and Status enum representing the last registered status of the ships, if they have a status.</returns>
        public override IDictionary<Ship, Status> GetStatusAllShips()
        {
            IDictionary<Ship, Status> statusOfAllShips = new Dictionary<Ship, Status>();

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
        /// <returns>Returns an Ilist with Container objects containing all containors stored in harbor.</returns>
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
        /// <returns>Returns a Ilist with Ship objects containing all ships in a loading dock.</returns>
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
        /// <returns>Returns an Ilist with Ship objects containing all ships in a ship dock.</returns>
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
        /// Returns a string with the harbor ID, amount of ships in loading dock, amount of free loading docks, amount of ships in ship dock, amount of free ship docks, amount of ships in anchorage, amount of ships in transit and amount of containers stored in harbor. .
        /// </summary>
        /// <returns>String value containing information about the harbour, its ships and container spaces.</returns>
        public override string ToString()
        {
            return ($"ID: {ID}, Total number of ships: {AllShips.Count}, Ships in loading docks: {shipsInLoadingDock.Count}, Free loading docks: {freeLoadingDocks.Count}, Ships in ship docks: {shipsInShipDock.Count}, Free ship docks: {freeShipDocks.Count}, " +
                $", Ships in anchorage: {Anchorage.Count}, Ships in transit: {ShipsInTransit.Count}, Containers stored in harbor: {storedContainers.Count}");
        }
    }
}
       

               
