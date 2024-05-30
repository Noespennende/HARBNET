using Gruppe8.HarbNet.Advanced;
using System.Collections.ObjectModel;
using System.Text;

namespace Gruppe8.HarbNet
{
    /// <summary>
    /// Class to run a simulation of a harbour.
    /// </summary>
    public class SimpleSimulation : Simulation
    {
        /// <summary>
        /// Gets the date and time when the simulation started.
        /// </summary>
        /// <returns>Returns a DateTime object representing the date and time when the simulation started.</returns>
        private readonly DateTime _startTime;

        /// <summary>
        /// Gets the current date and time in simulation.
        /// </summary>
        /// <returns>Returns a DateTime object representing the date and time the simulation currently is in.</returns>
        private DateTime _currentTime;

        /// <summary>
        /// Gets the date and time when the simulation ended.
        /// </summary>
        /// <returns>Returns a DateTime object representing the date and time when the simulation ended.</returns>
        private readonly DateTime _endTime;

        /// <summary>
        /// Gets the harbor object.
        /// </summary>
        /// <returns>Returns a harbor object.</returns>
        private readonly Harbor _harbor;

        /// <summary>
        /// Gets the number of containers transported from storage to ship on a loading round lasting an hour.
        /// </summary>
        /// <returns>Returns an int value representing the number of containers transported from storage to ship in a hour.</returns>
        private int _numberOfStorageContainersToShipThisRound;

        /// <summary>
        /// History for all ships and containers in the simulation in the form of DailyLog objects. Each DailyLog object stores information for one day in the simulation and contains information about the location and status of all ships and containers that day.
        /// </summary>
        /// <returns>Returns a readOnlyCollection of Dailylog objects each representing one day of the simulation. Together the list represent the entire history of one simulation.</returns>
        public override ReadOnlyCollection<DailyLog> History => HistoryIList.AsReadOnly();

        /// <summary>
        /// History for all ships and containers in the simulation in the form of DailyLog objects. Each DailyLog object stores information for one day in the simulation and contains information about the location and status of all ships and containers that day.
        /// </summary>
        /// <returns>Returns a IList of Dailylog objects each representing one day of the simulation. Together the list represent the entire history of one simulation.</returns>
        internal IList<DailyLog> HistoryIList { get; } = new List<DailyLog>();

        /// <summary>
        /// Creates an object of the SimpleSimulation class. This object can be used to run a simulation on a harbor.
        /// </summary>
        /// <param name="harbor">The harbor object which will be used in the simulation.</param>
        /// <param name="simulationStartTime">The date and time the simulation starts.</param>
        /// <param name="simulationEndTime">The date and time the simulation ends.</param>
        public SimpleSimulation(Port harbor, DateTime simulationStartTime, DateTime simulationEndTime)
        {
            _harbor = (Harbor)harbor;
            _startTime = simulationStartTime;
            _endTime = simulationEndTime;
        }

        /// <summary>
        /// Running the simulation.
        /// </summary>
        /// <returns>Returns the history of the simulation in the form of log objects where each object contains information about all ships and containers on one day of the simulation.</returns>
        public override IList<DailyLog> Run()
        {

            _currentTime = _startTime;

            SimulationStartingEventArgs simulationStartingEventArgs = new(_harbor, _currentTime, "The simulation has started");

            SimulationStarting?.Invoke(this, simulationStartingEventArgs);

            HistoryIList.Add(
                new DailyLog(
                    _currentTime,
                    _harbor.Anchorage,
                    _harbor.GetShipsInTransit(),
                    _harbor.GetContainersStoredInHarbour(),
                    _harbor.ArrivedAtDestination,
                    _harbor.GetShipsInLoadingDock(),
                    _harbor.GetShipsInShipDock()));

            while (_currentTime < _endTime)
            {
                SetStartLocationForAllShips();

                foreach (Ship ship in _harbor.AllShips)
                {
                    ship.HasBeenAlteredThisHour = false;

                }

                _harbor.GenerateTrucks();

                UndockingShips();

                AnchoringShips();

                DockingShips();

                UnloadingShips();

                LoadingShips();

                LoadingTrucksFromStorage();

                ContainersOnTrucksArrivingToDestination();

                InTransitShips();

                EndOf24HourPeriod();

                OneHourHasPassedEventArgs oneHourHasPassedEventArgs = new(_currentTime, "One hour has passed in the simulation, which equals to one 'round'");
                OneHourHasPassed?.Invoke(this, oneHourHasPassedEventArgs);

                continue;
            }

            SimulationEndedEventArgs simulationEndedEventArgs = new(History, "The simulation has reached the end time and has ended.");

            SimulationEnded?.Invoke(this, simulationEndedEventArgs);

            return History;

        }

        /// <summary>
        /// Prints to console the historical data regarding the Location, Name, size, status, max weight, Current weight, container capacity, number of containers onboard and ID of 
        /// all ships in the simulation for each day the simulation was run.
        /// </summary>
        public override void PrintShipHistory()
        {
            foreach (DailyLog log in History)
            {
                log.PrintInfoForAllShips();
            }
        }

        /// <summary>
        /// Prints the history of the given ship to console.
        /// <param name="shipToBePrinted">The ship object who's history will be printed.</param>
        /// </summary>
        public override void PrintShipHistory(Ship shipToBePrinted)
        {
            shipToBePrinted.PrintHistory();
        }

        /// <summary>
        /// Prints the history of a given ship to console.
        /// </summary>
        /// <param name="shipID">The unique ID of the ship who's history will be printed.</param>
        /// <exception cref="ArgumentException">Throws exception if ship is not found in harbor object.</exception>
        public override void PrintShipHistory(Guid shipID)
        {
            foreach (Ship ship in _harbor.AllShips)
            {
                if (ship.ID.Equals(shipID))
                {
                    ship.PrintHistory();
                    return;
                }
            }

            throw new ArgumentException(
                "The ship you are trying to print does not exist in the harbor the simulation is using. " +
                "In order for the simulation to be able to print the ships history the ship must be part of the simulated harbor.");
        }

        /// <summary>
        /// Printing each container in the simulations entire history to console.
        /// </summary>
        public override void PrintContainerHistory()
        {
            foreach (DailyLog log in History)
            {
                log.PrintInfoForAllContainers();
            }
        }

        /// <summary>
        /// Returns a string that contains information about the entire history of each ship in the harbor simulation. Information in the string includes the historical data regarding the
        /// Location, Name, size, status, max weight, Current weight, container capacity, number of containers onboard and ID of 
        /// all ships at the end of every day of the simulation.
        /// </summary>
        /// <returns> a string that contains information about all ships in the previous simulation. Returns an empty string if no simulation has been run.</returns>
        public override string HistoryToString()
        {
            if (History.Count > 0)
            {
                StringBuilder sb = new();

                foreach (DailyLog log in History)
                {
                    sb.Append(log.HistoryToString());
                }

                return sb.ToString();
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Gives a string containing information of the ship matching the given shipID's entire history. Information in the String includes the ship's name, ID,
        /// Date and Time of all status changes and the coresponding status the ship had at those times.
        /// </summary>
        /// <param name="shipID">The unique ID of the ship the history belongs to.</param>
        /// <returns>Returns a string value containing information about the ship's entire history.</returns>
        /// <exception cref="ArgumentException">Exception thrown ship does not exist within the simulation.</exception>
        public override string HistoryToString(Guid shipID)
        {

            foreach (Ship ship in _harbor.AllShips)
            {
                if (ship.ID.Equals(shipID))
                {
                    return ship.HistoryToString();
                }
            }

            throw new ArgumentException(
                "The ship you are trying to get the history from does not exist in the harbor object the simulation is using. " +
                "In order for the simulation to be able to provide a String of the ships history " +
                "the ship must be added to the harbor the simulation is using.");
        }

        /// <summary>
        /// Returns a string containing information about the entire history of each ship or each container in the simulation.
        /// </summary>
        /// <param name="ShipsOrContainers">Sending inn "ships" returns the history of all ships in the previous simulation. Sending inn "containers" return the history of each container in the previous simulation</param>
        /// <returns>Returns a String value containing the entire history of all ships or all containers of the simulation. Returns an empty string if wrong value is given in param or no simulation has been run.</returns>
        public override string HistoryToString(String ShipsOrContainers)
        {
            if (ShipsOrContainers.ToLower().Equals("ships") || ShipsOrContainers.ToLower().Equals("ship"))
            {
                return HistoryToString();
            }

            else if (ShipsOrContainers.ToLower().Equals("containers") || ShipsOrContainers.ToLower().Equals("container"))
            {
                if (History.Count > 0)
                {
                    StringBuilder sb = new();

                    foreach (DailyLog log in History)
                    {
                        sb.Append(log.HistoryToString("containers"));
                    }
                    return sb.ToString();
                }

                else
                {
                    return "";
                }
            }

            else
            {
                return "";
            }
        }

        /// <summary>
        /// Gives a string containing information of the given ship entire history. Information in the String includes the ship's name, ID,
        /// Date and Time of all status changes and the coresponding status the ship had at those times.
        /// </summary>
        /// <param name="ship">The ship object in the simulation that information is retrieved from.</param>
        /// <returns>Returns a string value containing information about the given ship's entire history.</returns>
        public override string HistoryToString(Ship ship)
        {
            return ship.HistoryToString();
        }

        /// <summary>
        /// Returns a string that contains information about the start time of the simulation, end time of the simulation and the ID of the harbour used.
        /// </summary>
        /// <returns> a string that contains information about the start time, end time of the simulation and the ID of the harbour used.</returns>
        public override string ToString()
        {
            return ($"Simulation start time: {_startTime}, end time: {_endTime}, harbor ID: {_harbor.ID}");
        }

        /// <summary>
        /// Loading container on to ship.
        /// </summary>
        /// <param name="ship">Ship object the containers will be loaded on.</param>
        /// <returns>Returns the container object of containers moved on ship.</returns>
        internal Container? LoadContainerOnShip(Ship ship)
        {
            _numberOfStorageContainersToShipThisRound = _harbor.NumberOfContainersInStorageToShips();
            Crane? storageCrane = _harbor.GetFreeStorageAreaCrane();
            Crane? dockCrane = _harbor.GetFreeLoadingDockCrane();

            if (storageCrane?.Container == null && dockCrane?.Container == null)
            {

                Container? loadedContainer = LoadContainerOnStorageAgv(ship);

                if (loadedContainer != null)
                {
                    Container? movedContainer = MoveOneContainerFromAgvToShip(loadedContainer, ship);
                    return movedContainer;
                }
            }

            return null;
        }

        /// <summary>
        /// Loads one container from Agv to the given Ship.
        /// </summary>
        /// <param name="ship">Ship object that is loading the container onboard.</param>
        /// <returns>Returns the container object that will be loaded.</returns>
        internal Container? LoadContainerOnStorageAgv(Ship ship)
        {

            Container? containerToBeLoaded;

            bool underMaxWeight;
            bool underMaxCapacity;

            bool shipHasNoContainers = ShipHasNoContainers(ship);
            bool lastContainerIsFullSize = ship.ContainersOnBoard.LastOrDefault()?.Size == ContainerSize.Full;
            bool harborStorageHasHalfContainer = _harbor.GetStoredContainer(ContainerSize.Half) != null;
            bool harborStorageHasFullContainer = _harbor.GetStoredContainer(ContainerSize.Full) != null;

            if (shipHasNoContainers
                || (lastContainerIsFullSize && harborStorageHasHalfContainer)
                || (!harborStorageHasFullContainer && harborStorageHasHalfContainer))
            {
                underMaxWeight = ship.MaxWeightInTonn >= ship.CurrentWeightInTonn + (int)ContainerSize.Half;
                underMaxCapacity = ship.ContainerCapacity > ship.ContainersOnBoard.Count + 1;

                if (underMaxCapacity && underMaxWeight)
                {
                    containerToBeLoaded = MoveOneContainerFromContainerRowToAgv(ContainerSize.Half);

                    return containerToBeLoaded;
                }
            }

            else if (!lastContainerIsFullSize && harborStorageHasFullContainer
                || (!harborStorageHasHalfContainer && harborStorageHasFullContainer))
            {
                underMaxWeight = ship.MaxWeightInTonn >= ship.CurrentWeightInTonn + (int)ContainerSize.Full;
                underMaxCapacity = ship.ContainerCapacity > ship.ContainersOnBoard.Count + 1;

                if (underMaxCapacity && underMaxWeight)
                {
                    containerToBeLoaded = MoveOneContainerFromContainerRowToAgv(ContainerSize.Full);

                    return containerToBeLoaded;
                }
            }

            return null;
        }

        /// <summary>
        /// Sets start location and initiates first status log for all ships, based on their _startTime.
        /// </summary>
        private void SetStartLocationForAllShips()
        {
            foreach (Ship ship in _harbor.AllShips)
            {
                ShipAnchoredEventArgs shipAnchoredEventArgs = new(ship, _currentTime, "Ship has anchored to Anchorage.", _harbor.AnchorageID);


                if (
                    ship.StartDate.Year == _currentTime.Year &&
                    ship.StartDate.Month == _currentTime.Month &&
                    ship.StartDate.Day == _currentTime.Day &&
                    ship.StartDate.Hour == _currentTime.Hour &&
                    ship.StartDate.Minute == _currentTime.Minute)
                {
                    _harbor.AddNewShipToAnchorage(ship);

                    bool isSingleTripShip = ship.IsForASingleTrip == true;
                    bool freeLoadingDockExists = _harbor.GetFreeLoadingDock(ship.ShipSize) != null;

                    if (isSingleTripShip && freeLoadingDockExists)
                    {
                        Guid loadingDock = _harbor.MoveShipFromAnchorageToLoadingDock(ship.ID);

                        ShipIsDockedToLoadingDock(ship, loadingDock);
                    }

                    else if (isSingleTripShip && !freeLoadingDockExists)
                    {
                        ship.AddStatusChangeToHistory(_currentTime, _harbor.AnchorageID, Status.Anchored);
                        ShipAnchored?.Invoke(this, shipAnchoredEventArgs);
                    }

                    else
                    {
                        ship.AddStatusChangeToHistory(_currentTime, _harbor.AnchorageID, Status.Anchoring);
                        ship.AddStatusChangeToHistory(_currentTime, _harbor.AnchorageID, Status.Anchored);
                        ShipAnchored?.Invoke(this, shipAnchoredEventArgs);
                    } 
                }
            }
        }

        /// <summary>
        /// Ends the 24 hour period with log and status updates, and raises event.
        /// </summary>
        private void EndOf24HourPeriod()
        {
            DateTime past24Hours = _currentTime.AddHours(-24);
            if (_currentTime.Hour == 0)
            {
                DailyLog harborDayLog = new(
                    _currentTime, _harbor.Anchorage, 
                    _harbor.GetShipsInTransit(), 
                    _harbor.GetContainersStoredInHarbour(), 
                    _harbor.ArrivedAtDestination,
                    _harbor.GetShipsInLoadingDock(), 
                    _harbor.GetShipsInShipDock());
                
                HistoryIList.Add(harborDayLog);


                foreach (Container container in _harbor.GetContainersStoredInHarbour())
                {
                    container.AddAnotherDayInStorage();
                }

                Dictionary<Ship, List<StatusLog>> dayReviewAllShipLogs = new();
                foreach (Ship ship in _harbor.AllShips)
                {
                    List<StatusLog> dayReviewShipLogs = new();

                    foreach (StatusLog log in ship.HistoryIList)
                    {

                        if (log.Timestamp >= past24Hours && log.Timestamp <= _currentTime)
                        {
                            dayReviewShipLogs.Add(log);
                        }
                    }

                    dayReviewAllShipLogs.Add(ship, dayReviewShipLogs);
                }

                DayEndedEventArgs dayOverEventArgs = new(harborDayLog,dayReviewAllShipLogs, _currentTime, "The day has passed and the state of the _harbor on day-shifty has been logged.");
                DayEnded?.Invoke(this, dayOverEventArgs);
            }

            _currentTime = _currentTime.AddHours(1);
        }

        /// <summary>
        /// Anchoring ship to anchorage outside harbor, ship.Status is set to Anchoring.
        /// </summary>
        private void AnchoringShips()
        {
            Guid anchorageID = _harbor.AnchorageID;
            List<Ship> Anchorage = _harbor.Anchorage.ToList();

            foreach (Ship ship in Anchorage)
            {
                ShipAnchoredEventArgs shipAnchoredEventArgs = new(ship, _currentTime, "Ship has anchored to anchorage.", _harbor.AnchorageID);

                Guid shipID = ship.ID;

                StatusLog lastStatusLog = GetStatusLog(ship);

                if (ShipIsAnchoring(ship, lastStatusLog))
                {
                    ship.HasBeenAlteredThisHour = true;

                    ship.AddStatusChangeToHistory(_currentTime, anchorageID, Status.Anchored);
                    ShipAnchored?.Invoke(this, shipAnchoredEventArgs);
                }
            }
        }

        /// <summary>
        /// Checks if the ship is anchoring.
        /// </summary>
        /// <param name="ship">The ship that is to be checked if is anchoring.</param>
        /// <param name="lastStatusLog">The last StatusLog in the specified ship's history.</param>
        /// <returns>Returns true if ship is currently anchoring, if not then false is returned.</returns>
        private bool ShipIsAnchoring(Ship ship, StatusLog lastStatusLog)
        {
            return ShipCanBeAltered(ship, lastStatusLog) && lastStatusLog.Status == Status.Anchoring;
        }

        /// <summary>
        /// Get the last StatusLog object from the ship's history.
        /// </summary>
        /// <param name="ship">The ship the StatusLog belongs to.</param>
        /// <returns>The last StatusLog object.</returns>
        private StatusLog GetStatusLog(Ship ship)
        {
            return ship.HistoryIList.Last();
        }

        /// <summary>
        /// Get the second to last StatusLog object from the ship's history.
        /// </summary>
        /// <param name="ship">The ship object the StatusLog belongs to.</param>
        /// <returns>The second to last StatusLog object.</returns>
        private StatusLog? GetSecondLastStatusLog(Ship ship)
        {
            return 
                ship.HistoryIList != null && ship.HistoryIList.Count >= 2 
                ? ship.HistoryIList[ship.HistoryIList.Count - 2] 
                : null;
        }

        /// <summary>
        /// Docking ships to the harbor, the shipStatus is set to docking.
        /// </summary>
        private void DockingShips()
        {
            List<Ship> Anchorage = _harbor.Anchorage.ToList();
            List<Ship> ShipsInLoadingDock = new(_harbor.shipsInLoadingDock.Keys);

            foreach (Ship ship in ShipsInLoadingDock)
            {
                Guid shipID = ship.ID;
                StatusLog lastStatusLog = GetStatusLog(ship);

                bool shipCanBeAltered = ShipCanBeAltered(ship, lastStatusLog);
                bool singleTripShipReadyForShipDock = lastStatusLog.Status == Status.UnloadingDone && SingleTripShipHasBeenInTransit(ship);

                if (shipCanBeAltered 
                    && (lastStatusLog.Status == Status.DockingToLoadingDock || singleTripShipReadyForShipDock))
                {
                    if (ShipDockingForMoreThanOneHour(lastStatusLog))
                    {
                        Guid dockID = lastStatusLog.SubjectLocation;

                        ShipIsDockedToLoadingDock(ship, dockID);

                        if (ShipFromTransitWithContainersToUnload(ship))
                        {
                            StartUnloadProcess(ship, dockID);
                        }
                        
                        else if (ShipHasNoContainers(ship) && !ship.IsForASingleTrip)
                        {
                            StartLoadingProcess(ship, dockID);
                        }
                    }

                    else if (EmptySingleTripShip(ship) && FreeShipDockExists(ship))
                    {
                        if (FreeShipDockExists(ship) && SingleTripShipHasBeenInTransit(ship)
                            && ShipHasNoContainers(ship) && _currentTime != _startTime
                            && lastStatusLog.Status != Status.DockingToShipDock)
                        {
                            ShipNowDockingToShipDock(ship, shipID);
                        }
                    }

                    else if (
                        ship.IsForASingleTrip && 
                        !FreeShipDockExists(ship) && 
                        ship.TransitStatus == TransitStatus.Anchoring)
                    {
                        ShipAnchoringToAnchorage(ship);
                    }
                }
            }

            foreach (Ship ship in Anchorage)
            {

                Guid shipID = ship.ID;
                StatusLog lastStatusLog = GetStatusLog(ship);

                if (ShipCanDock(ship, lastStatusLog))
                {
                    Guid dockID;
                    
                    if (NonEmptyShipReadyForLoadingDock(ship))
                    {

                        if (lastStatusLog.Status == Status.Anchored)
                        {
                            dockID = _harbor.DockShipToLoadingDock(shipID);
                            ShipIsDockingToLoadingDock(ship, dockID);
                        }

                        if (lastStatusLog.Status == Status.DockingToLoadingDock)
                        {
                            dockID = lastStatusLog.SubjectLocation;
                            ShipIsDockedToLoadingDock(ship, dockID);
                        }
                    }
                    
                    else if (SingleTripShipAnchoring(ship))
                    { 
                        if (FreeShipDockExists(ship) 
                            && SingleTripShipHasBeenInTransit(ship) 
                            && ShipHasNoContainers(ship) 
                            && _currentTime != _startTime 
                            && lastStatusLog.Status != Status.DockingToShipDock)
                        {
                            dockID = _harbor.DockShipToShipDock(shipID);

                            ShipNowDockingToShipDock(ship, dockID);
                        }
                    }

                    else if (ship.IsForASingleTrip 
                            && !(FreeShipDockExists(ship)) 
                            && ContainsTransitStatus(ship) 
                            && ShipHasNoContainers(ship) 
                            && lastStatusLog.Status == Status.Anchoring)
                    {
                        ship.AddStatusChangeToHistory(_currentTime, lastStatusLog.SubjectLocation, Status.Anchored);
                    }

                    ship.HasBeenAlteredThisHour = true;
                }
            }
        }
        
        /// <summary>
        /// Checks if the ship is empty and for a single trip.
        /// </summary>
        /// <param name="ship">Ship object that is checked if it is empty and set for single trip.</param>
        /// <returns>Returns true if ship is empty and set for single trip, if not then false is returned.</returns>
        private static bool EmptySingleTripShip(Ship ship)
        {
            return ship.IsForASingleTrip && ship.ContainersOnBoard.Count == 0;
        }
        
        /// <summary>
        /// checks if it's a single trip ship with status anchoring.
        /// </summary>
        /// <param name="ship">Ship object that is checked if it is set for single trip and is currently anchoring.</param>
        /// <returns>Returns true if the ship is set for single trip and is currently anchoring, if not then false is returned.</returns>
        private bool SingleTripShipAnchoring(Ship ship)
        {
            return ship.IsForASingleTrip && FreeShipDockExists(ship) && ship.TransitStatus == TransitStatus.Anchoring;
        }

        /// <summary>
        /// Checks if the ship is not empty and there is a loading dock for it.
        /// </summary>
        /// <param name="ship">Ship object that is checked if it is empty and if there is available loading dock.</param>
        /// <returns>Returns true if the ship contains containers and there is an available loading dock for the ship, if not then false is returned.</returns>
        private bool NonEmptyShipReadyForLoadingDock(Ship ship)
        {
            bool freeLoadingDockExists = _harbor.FreeLoadingDockExists(ship.ShipSize);
            bool shipHasContainers = ship.ContainersOnBoard.Count != 0;
            bool singleTripNotYetTransited = !(ship.IsForASingleTrip && !ContainsTransitStatus(ship));

            return freeLoadingDockExists && shipHasContainers && singleTripNotYetTransited;
        }
        
        /// <summary>
        /// checks the status of the ship for conditions acceptable for docking.
        /// </summary>
        /// <param name="ship">Ship object that is checked if it can dock.</param>
        /// <param name="lastStatusLog">StatusLog object belonging to the specified ship.</param>
        /// <returns>Returns true if the ship is fit for dockings, if not then false is returned.</returns>
        private static bool ShipCanDock(Ship ship, StatusLog lastStatusLog)
        {
            bool shipIsReadyToDock = lastStatusLog.Status == Status.Anchored || lastStatusLog.Status == Status.DockedToShipDock ||
                lastStatusLog.Status == Status.DockingToLoadingDock || lastStatusLog.Status == Status.DockingToShipDock;

            bool singleTripShipIsReadyForShipDock = lastStatusLog.Status == Status.UnloadingDone && SingleTripShipHasBeenInTransit(ship);

            return ShipCanBeAltered(ship, lastStatusLog) && shipIsReadyToDock || singleTripShipIsReadyForShipDock;
        }
        
        /// <summary>
        /// Ship is anchoring to anchorage.
        /// </summary>
        /// <param name="ship">Ship object that is anchoring to anchorage.</param>
        private void ShipAnchoringToAnchorage(Ship ship)
        {
            ShipAnchoringEventArgs shipAnchoringEventArgs = new(ship, _currentTime, "Ship is anchoring to anchorage.", _harbor.AnchorageID);

            ship.TransitStatus = TransitStatus.Anchoring;

            _harbor.AddNewShipToAnchorage(ship);

            ship.AddStatusChangeToHistory(_currentTime, _harbor.AnchorageID, Status.Anchoring);
            ShipAnchoring?.Invoke(this, shipAnchoringEventArgs);

            ship.HasBeenAlteredThisHour = true;
        }
        
        /// <summary>
        /// Ship is docking to ship dock.
        /// </summary>
        /// <param name="ship">Ship object that is docking to ship dock.</param>
        /// <param name="shipID">Unique ID for the ship object to be docked to ship dock.</param>
        private void ShipNowDockingToShipDock(Ship ship, Guid shipID)
        {
            Guid dockID = _harbor.DockShipToShipDock(shipID);

            ship.AddStatusChangeToHistory(_currentTime, dockID, Status.DockingToShipDock);

            ShipDockingToShipDockEventArgs shipDockingToShipDockEventArgs = new(ship, _currentTime, "Ship is docking to ship dock.", dockID);
            ShipDockingToShipDock?.Invoke(this, shipDockingToShipDockEventArgs);

            ship.HasBeenAlteredThisHour = true;
        }
        
        /// <summary>
        /// Checks to see if the ship has no container.
        /// </summary>
        /// <param name="ship">Ship object to be checked if it is empty.</param>
        /// <returns>Returns true if ship contains no containers, if not then false is returned.</returns>
        private static bool ShipHasNoContainers(Ship ship)
        {
            return ship.ContainersOnBoard.Count == 0;
        }
        
        /// <summary>
        /// Checks if there is a free dock.
        /// </summary>
        /// <param name="ship">Ship object to be docked.</param>
        /// <returns>Returns true if there is an available dock matching the ship's shipSize enum, if not then false is returned.</returns>
        private bool FreeShipDockExists(Ship ship)
        {
            return _harbor.FreeShipDockExists(ship.ShipSize);
        }
        
        /// <summary>
        /// Starting the loading process.
        /// </summary>
        /// <param name="ship">Ship object that is loading.</param>
        /// <param name="dockID">Unique ID for the loading dock object the ship object is docked to.</param>
        private void StartLoadingProcess(Ship ship, Guid dockID)
        {
            ShipStartingLoadingEventArgs shipStartingLoadingEventArgs = new(ship, _currentTime, "The ship is starting the loading process.", ship.CurrentLocation);

            ship.AddStatusChangeToHistory(_currentTime, dockID, Status.Loading);

            ShipStartingLoading?.Invoke(this, shipStartingLoadingEventArgs);
        }
        
        /// <summary>
        /// Checks if the ship is a single trip ship and has been in transit in the simulation.
        /// </summary>
        /// <param name="ship">Ship object.</param>
        /// <returns>Returns true if the ship is set to single trip and has been in transit in the simulation, if not false is returned.</returns>
        private static bool SingleTripShipHasBeenInTransit(Ship ship)
        {
            return (ship.IsForASingleTrip == true && ContainsTransitStatus(ship));
        }
        
        /// <summary>
        /// Starting the unload process.
        /// </summary>
        /// <param name="ship">Ship object that is unloading.</param>
        /// <param name="dockID">Unique ID for the loading dock object the ship object is docked to.</param>
        private void StartUnloadProcess(Ship ship, Guid dockID)
        {
            ShipStartingUnloadingEventArgs shipStartingUnloadingEventArgs = new(ship, _currentTime, "The ship is starting the unloading process.", ship.CurrentLocation);

            double percentTrucks = _harbor.PercentOfContainersDirectlyLoadedFromShips;
            ship.ContainersLeftForTrucks = ship.GetNumberOfContainersToTrucks(percentTrucks);

            ship.AddStatusChangeToHistory(_currentTime, dockID, Status.Unloading);

            ShipStartingUnloading?.Invoke(this, shipStartingUnloadingEventArgs);
        }
        
        /// <summary>
        /// Checks if the ship has been trying to dock for more than one hour.
        /// </summary>
        /// <param name="lastStatusLog">The last registered Statuslog object.</param>
        /// <returns>Returns true if the ship has been trying to dock to loading dock for more than one hour, if not then false is returned.</returns>
        private bool ShipDockingForMoreThanOneHour(StatusLog lastStatusLog)
        {
            return lastStatusLog.Status == Status.DockingToLoadingDock && (_currentTime - lastStatusLog.Timestamp).TotalHours >= 1;
        }
        
        /// <summary>
        /// Checks if the ship is coming from transit and is not empty.
        /// </summary>
        /// <param name="ship">Ship object that is checked if it is empty and coming from transit.</param>
        /// <returns>Returns true if ship contains containers and has a tranist status, if not then false is returned.</returns>
        private bool ShipFromTransitWithContainersToUnload(Ship ship)
        {
            return ContainsTransitStatus(ship) && ship.ContainersOnBoard.Count != 0;
        }

        /// <summary>
        /// Ship docking to loading dock.
        /// </summary>
        /// <param name="ship">Ship object docking to loading dock.</param>
        /// <param name="dockID">Unique ID for the loading dock object the ship object is docking to.</param>
        private void ShipIsDockingToLoadingDock(Ship ship, Guid dockID)
        {
            ship.AddStatusChangeToHistory(_currentTime, dockID, Status.DockingToLoadingDock);

            ShipDockingToLoadingDockEventArgs shipDockingToLoadingDockEventArgs = new(ship, _currentTime, "Ship is docking to loading dock.", dockID);
            ShipDockingToLoadingDock?.Invoke(this, shipDockingToLoadingDockEventArgs);
        }

        /// <summary>
        /// Ship docked to loading dock.
        /// </summary>
        /// <param name="ship">Ship object docked to loading dock.</param>
        /// <param name="dockID">Unique ID for the loading dock object the ship object is docked to.</param>
        private void ShipIsDockedToLoadingDock(Ship ship, Guid dockID)
        {
            ship.AddStatusChangeToHistory(_currentTime, dockID, Status.DockedToLoadingDock);

            ShipDockedToLoadingDockEventArgs shipDockedToLoadingDockEventArgs = new(ship, _currentTime, "Ship has docked to loading dock.", dockID);
            ShipDockedToLoadingDock?.Invoke(this, shipDockedToLoadingDockEventArgs);
        }

        /// <summary>
        /// checks if the ship has not been altered this hour and it has a StatusLog object.
        /// </summary>
        /// <param name="ship">Ship object to check if it has been altered this hour and if it has a StatusLog object.</param>
        /// <param name="lastStatusLog">The last registered Statuslog object.</param>
        /// <returns>Returns true if ship has been altered this hour and has a StatusLog object registered, if not then false is returned.</returns>
        private static bool ShipCanBeAltered(Ship ship, StatusLog lastStatusLog)
        {
            return !ship.HasBeenAlteredThisHour && lastStatusLog != null;
        }

        /// <summary>
        /// Unload containers from ship to harbor.
        /// </summary>
        private void UnloadingShips()
        {
            foreach (Ship ship in _harbor.DockedShipsInLoadingDock())
            {
                StatusLog? lastStatusLog = GetStatusLog(ship);

                StatusLog? secondLastStatusLog = GetSecondLastStatusLog(ship);

                bool ShipIsReadyForUnloading = false;

                if (secondLastStatusLog != null)
                {
                   ShipIsReadyForUnloading = IsShipReadyForUnloading(ship, lastStatusLog, secondLastStatusLog);
                }

                if (ShipIsReadyForUnloading)
                {
                    Guid dockID = lastStatusLog.SubjectLocation;

                    if (ShipNowUnloading(ship, lastStatusLog))
                    {
                        if (ShipIsDockedToLoadingDock(lastStatusLog))
                        {
                            StartUnloadProcess(ship, dockID);
                        }

                        UnloadShipForOneHour(ship);

                        UpdateShipStatus(ship, lastStatusLog);

                        ship.HasBeenAlteredThisHour = true;
                    }
                }
            }
        }
        
        /// <summary>
        /// Checks if the ship meets the criteria for unloading.
        /// </summary>
        /// <param name="ship">ship object that is checked if it can unload.</param>
        /// <param name="lastStatusLog">The last registered Statuslog object.</param>
        /// <param name="secondLastStatusLog">The second last registered Statuslog object.</param>
        /// <returns>Returns true if the ship meets the criteria to unload, if not then false is returned.</returns>
        private bool IsShipReadyForUnloading(Ship ship, StatusLog lastStatusLog, StatusLog secondLastStatusLog)
        {
            bool shipCanUnload = lastStatusLog.Status == Status.Unloading || lastStatusLog.Status == Status.DockedToLoadingDock;

            return ShipCanBeAltered(ship, lastStatusLog) && secondLastStatusLog != null && shipCanUnload;
        }
        
        /// <summary>
        /// Checks if the ship is unloading.
        /// </summary>
        /// <param name="ship">ship object that is checked if it is unloading.</param>
        /// <param name="lastStatusLog">The last registered Statuslog object.</param>
        /// <returns>Returns true if ship is currently unloading, if not then false is returned.</returns>
        private bool ShipNowUnloading(Ship ship, StatusLog lastStatusLog)
        {
            bool shipIsNotEmpty = ship.ContainersOnBoard.Count != 0;
            bool shipCanTryUnloading = lastStatusLog.Status == Status.DockedToLoadingDock || lastStatusLog.Status == Status.Unloading;
            
            return shipIsNotEmpty && shipCanTryUnloading;
        }
        
        /// <summary>
        /// Ship is finished unloading containers.
        /// </summary>
        /// <param name="ship">Ship object to be checked if it is done unloading.</param>
        /// <param name="currentLocation">Unique ID for the current location specified ship is located.</param>
        private void ShipFinishedUnloading(Ship ship, Guid currentLocation)
        {
            ShipDoneUnloadingEventArgs shipDoneUnloadingEventArgs = new(ship, _currentTime, "The ship has finished unloading.", currentLocation);
            ship.AddStatusChangeToHistory(_currentTime, currentLocation, Status.UnloadingDone);

            ShipDoneUnloading?.Invoke(this, shipDoneUnloadingEventArgs);
        }
        
        /// <summary>
        /// Checks if the ship status is DockedToLoadingDock.
        /// </summary>
        /// <param name="lastStatusLog">The last registered Statuslog object.</param>
        /// <returns>Returns true if the last registered status of ship is DockedToLoadingDock, if not then false is returned.</returns>
        private bool ShipIsDockedToLoadingDock(StatusLog lastStatusLog)
        {
            return lastStatusLog.Status == Status.DockedToLoadingDock;
        }

        /// <summary>
        /// Update the given ships status and adds the statuschange to the ship's history.
        /// </summary>
        /// <param name="ship">Ship object to get status updated.</param>
        /// <param name="lastStatusLog">The most recent registered Statuslog object in the ship's history.</param>
        /// <param name="secondLastStatusLog">The second last registered Statuslog object in the ship's history.</param>
        private void UpdateShipStatus(Ship ship, StatusLog lastStatusLog)
        {
            Guid currentLocation = lastStatusLog.SubjectLocation;

            if (ShipHasNoContainers(ship) && !SingleTripShipHasBeenInTransit(ship))
            {
                ShipFinishedUnloading(ship, currentLocation);

                StartLoadingProcess(ship, currentLocation);
            }

            else if (ShipHasNoContainers(ship) && SingleTripShipHasBeenInTransit(ship))
            {
                ShipFinishedUnloading(ship, currentLocation);

                Guid dockID = _harbor.DockShipToShipDock(ship.ID);
                if (dockID != Guid.Empty)
                { 
                    ship.AddStatusChangeToHistory(_currentTime, currentLocation, Status.DockingToShipDock); 
                }

                else
                {
                    ShipAnchoringToAnchorage(ship);
                }
            }
        }

        /// <summary>
        /// Unloads containers from the given ship's cargo on to the harbor for one hour.
        /// </summary>
        /// <param name="ship">The ship object that is to be unloaded.</param>
        private void UnloadShipForOneHour(Ship ship)
        {
            double percentTrucks = _harbor.PercentOfContainersDirectlyLoadedFromShips; 
            int numberOfContainersForTrucks = ship.GetNumberOfContainersToTrucks(percentTrucks);
            int numberOfContainersForStorage = ship.GetNumberOfContainersToStorage(percentTrucks);

            LoadingDock loadingDock = _harbor.GetLoadingDockContainingShip(ship.ID);

            Crane? craneForCalculating = _harbor.GetFreeLoadingDockCrane();

            int maxLoadsPerHour = 0;
            int numberOfRepeats;

            if (craneForCalculating != null)
            {
                maxLoadsPerHour = Math.Min(_harbor.LoadsPerAgvPerHour, craneForCalculating.ContainersLoadedPerHour);
            }

            if (maxLoadsPerHour == _harbor.LoadsPerAgvPerHour)
            {
                numberOfRepeats = maxLoadsPerHour * _harbor.AgvFree.Count;
            }

            else
            {
                numberOfRepeats = maxLoadsPerHour * Math.Min(_harbor.DockCranes.Count, _harbor.HarborStorageAreaCranes.Count);
            }

            for (int i = 0; i < ship.ContainersOnBoard.Count && i < numberOfRepeats; i++)
            {
                Container? movedContainer = MoveOneContainerFromShip(ship, numberOfContainersForTrucks, numberOfContainersForStorage, loadingDock);

                if (movedContainer != null)
                {
                    ShipUnloadedContainerEventArgs shipUnloadedContainerEventArgs = new(ship, _currentTime, "Ship has unloaded one container.", movedContainer);
                    ShipUnloadedContainer?.Invoke(this, shipUnloadedContainerEventArgs);
                }
            }
        }
        
        /// <summary>
        /// Moves one container from ship the ship's cargo to the given number of AGVs or Trucks
        /// </summary>
        /// <param name="ship">Ship object in witch containers is to be unloaded from.</param>
        /// <param name="numberOfContainersForTrucks">Int value representing the number of containers for trucks to move.</param>
        /// <param name="numberOfContainersForStorage">Int value representing the number of containers to be moved to the harbor's storage.</param>
        /// <param name="loadingDock">Loading dock object the ship is docked to.</param>
        private Container? MoveOneContainerFromShip(Ship ship, int numberOfContainersForTrucks, int numberOfContainersForStorage, LoadingDock loadingDock)
        {
            Container? container = null;

            if (numberOfContainersForStorage != 0 && (ship.ContainersOnBoard.Count - ship.ContainersLeftForTrucks) > 0)
            {
                container = MoveContainerFromShipToAgv(ship);

                if (container != null)
                {
                    MoveContainerFromAgvToStorage(container);
                }
            }

            if (numberOfContainersForTrucks != 0 && _harbor.TrucksInQueue.Count != 0)
            {
                container = MoveContainerFromShipToTruck(ship);
                if (container != null)
                {
                    _harbor.SendTruckOnTransit(loadingDock, container);
                }
            }

            return container;
        }

        /// <summary>
        /// Moves one container from the given ship's cargo to an AGV.
        /// </summary>
        /// <param name="ship">The ship object the container is being unloaded from.</param>
        /// <returns>Returns the container object that has been unloaded from the ship.</returns>
        private Container? MoveContainerFromShipToAgv(Ship ship)
        {
            Crane? craneDock = _harbor.GetFreeLoadingDockCrane();

            Agv? agv = _harbor.GetFreeAgv();

            if (agv != null && craneDock!= null)
            {
                Container? unloadedContainer = _harbor.ShipToCrane(ship, craneDock, _currentTime);
                if (unloadedContainer != null)
                {
                    _harbor.CraneToAgv(craneDock, agv, _currentTime);

                    return unloadedContainer;
                } 
            }

            return null;
        }

        /// <summary>
        /// Moves the given container from ADV's storage to a crane at the harbor's storage area.
        /// </summary>
        /// <param name="container">The container object that is to be moved to the harbor's storage.</param>
        private void MoveContainerFromAgvToStorage(Container container)
        {
            Agv? agv = _harbor.GetAgvContainingContainer(container);
            if (agv == null)
            {
                return;
            }

            Crane? crane = _harbor.GetFreeStorageAreaCrane();
            if (crane == null)
            {
                return; 
            }

            _harbor.AgvToCrane(crane, agv, _currentTime);
            _harbor.CraneToContainerRow(crane, _currentTime);
        }

        /// <summary>
        /// Unloads a container from the given ship's cargo on to a crane and then on to an available trucks cargo.
        /// </summary>
        /// <param name="ship">The ship object the container is unloaded from.</param>
        /// <returns>Returns the container object that is being moved from the ship to crane to truck.</returns>
        private Container? MoveContainerFromShipToTruck(Ship ship)
        {
            LoadingDock loadingDock = _harbor.GetLoadingDockContainingShip(ship.ID);

            Crane? craneDock = _harbor.GetFreeLoadingDockCrane();

            Truck? truck = _harbor.GetFreeTruck();
            
            if (truck != null)
            {
                _harbor.RemoveTruckFromQueue(truck);
                loadingDock.AssignTruckToTruckLoadingSpot(truck);
            }

            if (truck == null)
            {
                return null;
            }
            if (craneDock == null)
            {
                return null;
            }
            
            Container? container = _harbor.ShipToCrane(ship, craneDock, _currentTime);
            if (container != null)
            {
                _harbor.CraneToTruck(craneDock, truck, _currentTime);

                ship.ContainersLeftForTrucks--;

                loadingDock.RemoveTruckFromTruckLoadingSpot(truck);
                _harbor.SendTruckOnTransit(container);

                return container;
            }

            return null;
        }

        /// <summary>
        /// Undock ship from harbor, and set its status to Transit.
        /// </summary>
        private void UndockingShips()
        {
            foreach (Ship ship in _harbor.DockedShipsInLoadingDock())
            {

                Guid shipID = ship.ID;

                StatusLog lastStatusLog = GetStatusLog(ship);

                if (ShipCanUndockFromDock(ship, lastStatusLog)) 
                {
                    bool containsTransitStatus = ContainsTransitStatus(ship);

                    if (SingleTripShipCanDockToShipDock(ship, lastStatusLog))
                    {
                        Guid dockID = _harbor.DockShipToShipDock(ship.ID);
                        ship.AddStatusChangeToHistory(_currentTime, dockID, Status.DockingToShipDock);

                    }

                    else if (
                        ship.IsForASingleTrip && 
                            !containsTransitStatus && 
                                lastStatusLog.Status == Status.DockedToLoadingDock)
                    {
                        ship.AddStatusChangeToHistory(_currentTime, _harbor.TransitLocationID, Status.Undocking);
                        ship.TransitStatus = TransitStatus.Leaving;
                    }

                    else if (ShipIsDockingToShipDock(lastStatusLog))
                    {
                        Guid dockID = lastStatusLog.SubjectLocation;
                        ship.AddStatusChangeToHistory(_currentTime, dockID, Status.DockedToShipDock);

                        ShipDockedToShipDockEventArgs shipDockedToShipDockEventArgs = new(ship, _currentTime, "Ship has docked to ship dock.", dockID);
                        ShipDockedToShipDock?.Invoke(this, shipDockedToShipDockEventArgs);
                    }

                    else if (ShipIsFinishedLoadingContainers(lastStatusLog))
                    {
                        ship.AddStatusChangeToHistory(_currentTime, _harbor.TransitLocationID, Status.Undocking);
                        ship.TransitStatus = TransitStatus.Leaving;
                    }

                    else if (ShipIsUndocking(lastStatusLog))
                    {

                        _harbor.UnDockShipFromLoadingDockToTransit(shipID);
                        ship.AddStatusChangeToHistory(_currentTime, _harbor.TransitLocationID, Status.Transit);

                        ShipInTransitEventArgs shipInTransitEventArgs = new(ship, _currentTime, "Ship has left the _harbor and is in transit.", _harbor.TransitLocationID);

                        ShipInTransit?.Invoke(this, shipInTransitEventArgs);
                    }

                    ship.HasBeenAlteredThisHour = true;
                }
            }

            foreach (Ship ship in _harbor.GetShipsInShipDock())
            {
                Guid shipID = ship.ID;

                StatusLog lastStatusLog = GetStatusLog(ship);
                
                if (ShipCanUndockFromDock(ship, lastStatusLog))
                {
                    bool containsTransitStatus = ContainsTransitStatus(ship);

                    if (ShipIsDockingToShipDock(lastStatusLog))
                    {
                        Guid dockID = lastStatusLog.SubjectLocation;
                        ship.AddStatusChangeToHistory(_currentTime, dockID, Status.DockedToShipDock);
                        
                        ShipDockedToShipDockEventArgs shipDockedToShipDockEventArgs = new(ship, _currentTime, "Ship has docked to ship dock.", dockID);
                        ShipDockedToShipDock?.Invoke(this, shipDockedToShipDockEventArgs);
                    }

                    ship.HasBeenAlteredThisHour = true;
                }
            }

            Guid anchorageID = _harbor.AnchorageID;
            List<Ship> Anchorage = _harbor.Anchorage.ToList();

            foreach (Ship ship in Anchorage)
            {
                StatusLog lastStatusLog = GetStatusLog(ship);

                if (ship.IsForASingleTrip && !ContainsTransitStatus(ship))
                {
                    if (lastStatusLog.Status == Status.Anchored)
                    {
                        UndockFromAnchorage(ship, lastStatusLog);
                    }

                    else if (ShipIsUndocking(lastStatusLog))
                    {
                        UndockToTransit(ship);
                    }

                    ship.HasBeenAlteredThisHour = true;
                }
            }
        }

        /// <summary>
        /// Finishes the undocking process for a ship and adds the statuschange to the ship's history.
        /// </summary>
        /// <param name="ship">The ship object that is to finish undocking to transit.</param>
        private void UndockToTransit(Ship ship)
        {
            Guid oldLocation = _harbor.UnDockShipFromAnchorageToTransit(ship.ID);

            ShipInTransitEventArgs shipInTransitEventArgs = new(ship, _currentTime, "Ship as left the _harbor and is in transit.", _harbor.TransitLocationID);
            ship.AddStatusChangeToHistory(_currentTime, _harbor.TransitLocationID, Status.Transit);

            ShipInTransit?.Invoke(this, shipInTransitEventArgs);
        }

        /// <summary>
        /// Start the undocking process for the given ship from Anchorage.
        /// </summary>
        /// <param name="ship">The ship object that is to start undocking from Anchorage.</param>
        /// <param name="lastStatusLog">The most recent StatusLog in the ship's history.</param>
        private void UndockFromAnchorage(Ship ship, StatusLog lastStatusLog)
        {
            Guid currentLocation = lastStatusLog.SubjectLocation;

            ShipUndockingEventArgs shipUndockingEventArgs = new(ship, _currentTime, "Ship is undocking from Anchorage.", currentLocation);

            ship.AddStatusChangeToHistory(_currentTime, _harbor.TransitLocationID, Status.Undocking);
            ship.TransitStatus = TransitStatus.Leaving;

            ShipUndocking?.Invoke(this, shipUndockingEventArgs);
        }

        /// <summary>
        /// Checks if ship can undock from loading dock. 
        /// </summary>
        /// <param name="ship">The ship object that is being checked if can undock from loading dock.</param>
        /// <param name="lastStatusLog">The last StatusLog in the ship's history.</param>
        /// <returns>Returns true if the ship object can undock from loading dock, if not false is returned.</returns>
        private bool ShipCanUndockFromDock(Ship ship, StatusLog lastStatusLog)
        {
            bool shipFinishedInHarbor = lastStatusLog.Status == Status.Undocking || lastStatusLog.Status == Status.LoadingDone;
            bool singleTripShipCanleave = lastStatusLog.Status == Status.DockedToLoadingDock && ship.IsForASingleTrip && !ContainsTransitStatus(ship);

            bool singleTripHasTransited = lastStatusLog.Status == Status.UnloadingDone || lastStatusLog.Status == Status.DockingToShipDock;

            bool singletripCanDockToShipDock = ContainsTransitStatus(ship) && singleTripHasTransited;

            return ship.HasBeenAlteredThisHour == false && lastStatusLog != null 
                && (shipFinishedInHarbor || singleTripShipCanleave || singletripCanDockToShipDock);
        }

        /// <summary>
        /// Checks if ship is single-trip-ship and can dock to ship dock.
        /// </summary>
        /// <param name="ship">The ship object that is being checked if is single-trip-ship and able to dock to ship dock.</param>
        /// <param name="lastStatusLog">The last StatusLog in the ship's history.</param>
        /// <returns>Returns true if the ship object set for single trip can dock to ship dock, if not then false is returned.</returns>
        private bool SingleTripShipCanDockToShipDock(Ship ship, StatusLog lastStatusLog)
        {
            bool containsTransitStatus = ContainsTransitStatus(ship);

            bool singletripCanDockToShipDock = lastStatusLog.Status != Status.DockingToShipDock && FreeShipDockExists(ship) != false;

            return ship.IsForASingleTrip == true && containsTransitStatus && singletripCanDockToShipDock;
        }

        /// <summary>
        /// Checks if ship is in the process docking to ship dock.
        /// </summary>
        /// <param name="lastStatusLog">The last StatusLog in the ship's history.</param>
        /// <returns>Returns true if the ship object can dock to ship dock, if not then false is returned.</returns>
        private bool ShipIsDockingToShipDock(StatusLog lastStatusLog)
        {
            return lastStatusLog.Status == Status.DockingToShipDock && (_currentTime - lastStatusLog.Timestamp).TotalHours >= 1;
        }

        /// <summary>
        /// Checks if the ship is finished loading containers.
        /// </summary>
        /// <param name="lastStatusLog">The last StatusLog in the ship's history.</param>
        /// <returns>Returns true if the ship object is done loading containers, if not then false is returned.</returns>
        private bool ShipIsFinishedLoadingContainers(StatusLog lastStatusLog)
        {
            return lastStatusLog.Status == Status.LoadingDone || lastStatusLog.Status == Status.DockedToLoadingDock;
        }

        /// <summary>
        /// Checks if ship is in the process of undocking.
        /// </summary>
        /// <param name="lastStatusLog">The last StatusLog in the ship's history.</param>
        /// <returns>Returns true if the ship is currently undocking from dock, if not then false is returned.</returns>
        private bool ShipIsUndocking(StatusLog lastStatusLog)
        {
            return lastStatusLog.Status == Status.Undocking && (_currentTime - lastStatusLog.Timestamp).TotalHours >= 1;
        }

        /// <summary>
        /// Check to see if the ship has the Status "Transit".
        /// </summary>
        /// <param name="ship"> The ship object to be checked if it is or has been in transit.</param>
        /// <reuturns>Returns true if the ship object contains the StatusLog Transit, if not then false is returned.</reuturns>
        private static bool ContainsTransitStatus(Ship ship)
        {
            bool containsTransitStatus = false;
            foreach (StatusLog his in ship.HistoryIList)
            {
                if (his.Status == Status.Transit)
                {
                    containsTransitStatus = true;
                    break;
                }
            }

            return containsTransitStatus;
        }

        /// <summary>
        /// Loading containers onboard ships.
        /// </summary>
        private void LoadingShips()
        {
            foreach (Ship ship in _harbor.DockedShipsInLoadingDock())
            {

                StatusLog lastStatusLog = GetStatusLog(ship);
                StatusLog? secondLastStatusLog = GetSecondLastStatusLog(ship);

                bool shipIsNotSingleTripAndIsDoneUnloading = (lastStatusLog.Status == Status.UnloadingDone && (ship.IsForASingleTrip != true));
                bool shipIsSingleTripAndHasLastUnloadedAndHasNotBeenOnTrip = (lastStatusLog.Status == Status.UnloadingDone && (ship.IsForASingleTrip == true && !ContainsTransitStatus(ship)));
                bool LastLocationWasDockedToShipDock = (lastStatusLog.Status == Status.DockedToLoadingDock && secondLastStatusLog != null && secondLastStatusLog.Status == Status.DockedToShipDock);
                bool shipIsSingleTripAndHasNotBeenOnTrip = (ship.IsForASingleTrip == true && !ContainsTransitStatus(ship));

                if (
                    ShipCanBeAltered(ship, lastStatusLog) && 
                        shipIsNotSingleTripAndIsDoneUnloading || 
                        shipIsSingleTripAndHasLastUnloadedAndHasNotBeenOnTrip ||
                        (lastStatusLog.Status == Status.Loading) ||
                            (LastLocationWasDockedToShipDock && shipIsSingleTripAndHasNotBeenOnTrip))
                {
                    Guid currentLocation = lastStatusLog.SubjectLocation;
                    
                    if (ship.ContainersOnBoard.Count < ship.ContainerCapacity && ship.CurrentWeightInTonn < ship.MaxWeightInTonn)
                    {
                        if (_harbor.storedContainers.Keys.Count != 0 && ship.ContainersOnBoard.Count < ship.ContainerCapacity)
                        {
                            if (lastStatusLog.Status != Status.Loading)
                            {
                                ship.AddStatusChangeToHistory(_currentTime, currentLocation, Status.Loading);
                            }
                            LoadShipForOneHour(ship, currentLocation);
                        }

                        else
                        {
                            ShipFinishedLoading(ship, currentLocation);

                            ShipNowUndocking(ship, currentLocation);
                        }
                    }

                    else
                    {
                        ShipFinishedLoading(ship, currentLocation);

                        ShipNowUndocking(ship, currentLocation);
                    }

                    ship.HasBeenAlteredThisHour = true;
                }
            }
        }

        /// <summary>
        /// Starts the process of undocking the given ship from a dock. Updates the ships history with the statuschange.
        /// </summary>
        /// <param name="ship">Ship object currently undocking from dock.</param>
        /// <param name="currentLocation">Unique ID for the current location specified ship is located.</param>
        private void ShipNowUndocking(Ship ship, Guid currentLocation)
        {
            ship.TransitStatus = TransitStatus.Leaving;
            ship.AddStatusChangeToHistory(_currentTime, currentLocation, Status.Undocking);

            foreach (Container container in ship.ContainersOnBoard)
            {
                container.AddStatusChangeToHistory(Status.Transit, _currentTime);
            }

            ShipUndockingEventArgs shipUndockingEventArgs = new(ship, _currentTime, "Ship is undocking from dock.", currentLocation);
            ShipUndocking?.Invoke(this, shipUndockingEventArgs);
        }
        
        /// <summary>
        /// Ship is now finished Loading containers. Adds the statuschange to the ship's history.
        /// </summary>
        /// <param name="ship">Ship object to be checked if it is done loading containers.</param>
        /// <param name="currentLocation">Unique ID for the current location specified ship is located.</param>
        private void ShipFinishedLoading(Ship ship, Guid currentLocation)
        {
            ShipDoneLoadingEventArgs shipDoneLoadingEventArgs = new(ship, _currentTime, "The ship is done loading.", currentLocation);
            ship.AddStatusChangeToHistory(_currentTime, currentLocation, Status.LoadingDone);

            ShipDoneLoading?.Invoke(this, shipDoneLoadingEventArgs);
        }

        /// <summary>
        /// Loads the given ship with containers for one hour.
        /// </summary>
        /// <param name="ship">Ship object to be loading containers for one hour.</param>
        /// <param name="currentLocation">Unique ID for the current location specified ship is located.</param>
        private void LoadShipForOneHour(Ship ship, Guid currentLocation)
        {
            _numberOfStorageContainersToShipThisRound = _harbor.NumberOfContainersInStorageToShips();

            Crane? testCrane = _harbor.GetFreeLoadingDockCrane();

            int maxLoadsPerHour = 0;
            int numberOfRepeats;

            if (testCrane != null)
            {
                maxLoadsPerHour = Math.Min(_harbor.LoadsPerAgvPerHour, testCrane.ContainersLoadedPerHour);
            }

            if (maxLoadsPerHour == _harbor.LoadsPerAgvPerHour)
            { 
                numberOfRepeats = maxLoadsPerHour * _harbor.AgvFree.Count;
            }

            else
            {
                numberOfRepeats = maxLoadsPerHour * Math.Min(_harbor.DockCranes.Count, _harbor.HarborStorageAreaCranes.Count);
            }

            for (int i = 0; i < _numberOfStorageContainersToShipThisRound && i < numberOfRepeats; i++)
            {
                Container? loadedContainer = LoadContainerOnShip(ship);

                if (loadedContainer == null)
                {
                    ShipFinishedLoading(ship, currentLocation);
                    ShipNowUndocking(ship, currentLocation);
                    break;
                }

                else
                {
                    ShipLoadedContainerEventArgs shipLoadedContainerEventArgs = new(ship, _currentTime, "Ship has loaded one container.", loadedContainer);
                    ShipLoadedContainer?.Invoke(this, shipLoadedContainerEventArgs);
                }
            }
        }

        /// <summary>
        /// Moves a container from the harbor storage to AGV.
        /// </summary>
        /// <param name="containerSize">ContainerSize enum representing the size of the container.</param>
        /// <returns>Returns the container object that was moved.</returns>
        private Container? MoveOneContainerFromContainerRowToAgv(ContainerSize containerSize)
        {
            Agv? agv = _harbor.GetFreeAgv();
            Crane? storageCrane = _harbor.GetFreeStorageAreaCrane();
            if (storageCrane == null)
            {
                return null;
            }

            Container? container = _harbor.ContainerRowToCrane(containerSize, storageCrane, _currentTime);

            if (container == null || agv == null)
            {
                return null;
            }

            _harbor.CraneToAgv(storageCrane, agv, _currentTime);

            return container;
        }
        
        /// <summary>
        /// Moves the given container from the AGV to the given ship.
        /// </summary>
        /// <param name="container">Container object to be moved from AGV to ship.</param>
        /// <param name="ship">Ship object the container objects are loaded on.</param>
        /// <returns>Returns the container moved from AGV to ship.</returns>
        private Container? MoveOneContainerFromAgvToShip(Container? container, Ship ship)
        {
            Agv? agv = null;

            if (container != null) 
            { 
                agv = _harbor.GetAgvContainingContainer(container);
            }

            Crane? loadingDockCrane = _harbor.GetFreeLoadingDockCrane();

            if (agv == null || loadingDockCrane == null)
            {
                return null;
            }
            
            _harbor.AgvToCrane(loadingDockCrane, agv, _currentTime);
            _harbor.CraneToShip(loadingDockCrane, ship, _currentTime);

            return container;
        }

        /// <summary>
        /// Checks if any of the ships currently in transit is ready to enter the harbor, if so adds the ship to the anchorage.
        /// </summary>
        private void InTransitShips()
        {
            foreach (Ship ship in _harbor.ShipsInTransit.Keys)
            {
                StatusLog lastStatusLog = GetStatusLog(ship);

                if (ShipIsInTransit(ship, lastStatusLog))
                {

                    if (ShipIsReadyToReturnToHarbor(ship, lastStatusLog))
                    {
                        _harbor.RestockContainers(ship, _currentTime);

                        ShipAnchoringToAnchorage(ship);
                        ship.TransitStatus = TransitStatus.Arriving;
                    }

                    ship.HasBeenAlteredThisHour = true;
                }
            }
        }

        /// <summary>
        /// Checking the given ship's status to see if it's in transit.
        /// </summary>
        /// <param name="ship">Ship object to check status on to see if it's current status is in transit.</param>
        /// <param name="lastStatusLog">The last StatusLog in the ship's history.</param>
        /// <returns>Returns true if the ship's most recent registered StatusLog is "Transit", if not the false is returned.</returns>
        private bool ShipIsInTransit(Ship ship, StatusLog lastStatusLog)
        {
            return ship.HasBeenAlteredThisHour == false && lastStatusLog != null && lastStatusLog.Status == Status.Transit;
        }

        /// <summary>
        /// Checks if the given ship is ready return to harbor and has been out in transit for its given round-trip days.
        /// </summary>
        /// <param name="ship">Ship object to check on to see if it is ready to return to harbor.</param>
        /// <param name="LastHistoryStatusLog">The last StatusLog in the ship's history.</param>
        /// <returns>Returns true if the given ship is ready to return to harbor from its roundtrip, if not then false is returned.</returns>
        private bool ShipIsReadyToReturnToHarbor(Ship ship, StatusLog LastHistoryStatusLog)
        {
            double DaysSinceTransitStart = (_currentTime - LastHistoryStatusLog.Timestamp).TotalDays;
            
            return DaysSinceTransitStart >= ship.RoundTripInDays;
        }

        /// <summary>
        /// Loading the truck with containers from the harbor's storage area.
        /// </summary>
        private void LoadingTrucksFromStorage()
        {
            int numberOfContainersToTrucks = CalculateNumberOfStorageContainersToTrucks();

            Container? container = null;
            int containersToTruck = 0;

            while (_harbor.TrucksInQueue.Count > 0 && numberOfContainersToTrucks > containersToTruck)
            {
                
                foreach (Crane crane in _harbor.HarborStorageAreaCranes)
                {
                    int maxLoadsPerHour = crane.ContainersLoadedPerHour;

                    for (int i = 0; i < maxLoadsPerHour && containersToTruck < numberOfContainersToTrucks; i++)
                    {
                        container = _harbor.storedContainers.FirstOrDefault().Key;
                        if (container == null)
                        {
                            break;
                        }

                        if (_harbor.TrucksInQueue.Count > 0)
                        {
                          
                            Container? loadedContainer = MoveOneContainerFromContainerRowToTruck(container);
                            Truck? truck = null;

                            if (loadedContainer != null)
                            {
                                containersToTruck++;
                                truck = _harbor.SendTruckOnTransit(container);

                                if (truck != null)
                                {
                                    TruckLoadingFromHarborStorageEventArgs truckLoadingFromStorageEventArgs = new(truck, _currentTime, "One truck has loaded a container and has left");
                                    TruckLoadingFromStorage?.Invoke(this, truckLoadingFromStorageEventArgs);
                                }
                            }
                        }

                        else
                        {
                            break;
                        }
                    }

                    break;
                }

                break;
            }
        }
        
        /// <summary>
        /// Checks if containers have arrived by truck to their final destination. If so updates the containers History with the new status.
        /// </summary>
        private void ContainersOnTrucksArrivingToDestination()
        {
            foreach(Truck truck in _harbor.TrucksInTransit)
            {
                
                Container? container = truck.Container;
                if(container != null)
                {
                    StatusLog? lastLogContainer = container.HistoryIList.Last();

                    if ((_currentTime - lastLogContainer.Timestamp).TotalHours >= 1)
                    {
                        Container? arrivedContainer = truck.UnloadContainer();

                        if (arrivedContainer != null)
                        {
                            _harbor.ArrivedAtDestination.Add(arrivedContainer);

                            truck.Location = _harbor.DestinationID;

                            arrivedContainer.CurrentLocation = _harbor.DestinationID;
                            arrivedContainer.AddStatusChangeToHistory(Status.ArrivedAtDestination, _currentTime);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gives a number representing the amount of containers in the harbor storage area that should be directly loaded on to trucks.
        /// </summary>
        /// <returns>Returns an int value representing the number of containers in storage to be loaded on to trucks.</returns>
        private int CalculateNumberOfStorageContainersToTrucks()
        {
            return _harbor.NumberOfContainersInStorageToTrucks();      
        }

        /// <summary>
        /// Moves the given container the harbor storage area on to a available truck.
        /// </summary>
        /// <param name="container">Container object to be moved from storage to truck.</param>
        /// <returns>Returns the container object moved from storage to a truck.</returns>
        private Container? MoveOneContainerFromContainerRowToTruck(Container container)
        {
            Crane? storageCrane = _harbor.GetFreeStorageAreaCrane();
            if (storageCrane == null)
            {
                return null;
            }

            Container? containerFound = _harbor.ContainerRowToCrane(container.Size, storageCrane, _currentTime);
            if (container == null)
            {
                return null;
            }

            Truck? truck = _harbor.GetFreeTruck();
            if (truck != null && containerFound != null)
            {
                Container? containerOnTruck = _harbor.CraneToTruck(storageCrane, truck, _currentTime);

                return containerOnTruck;
            }

            return null;
        }

        public event EventHandler<SimulationEndedEventArgs>? SimulationEnded;
        public event EventHandler<SimulationStartingEventArgs>? SimulationStarting;
        public event EventHandler<OneHourHasPassedEventArgs>? OneHourHasPassed;
        public event EventHandler<DayEndedEventArgs>? DayEnded;
        public event EventHandler<ShipUndockingEventArgs>? ShipUndocking;
        public event EventHandler<ShipInTransitEventArgs>? ShipInTransit;
        public event EventHandler<ShipDockingToShipDockEventArgs>? ShipDockingToShipDock;
        public event EventHandler<ShipDockedToShipDockEventArgs>? ShipDockedToShipDock;
        public event EventHandler<ShipDockingToLoadingDockEventArgs>? ShipDockingToLoadingDock;
        public event EventHandler<ShipDockedToLoadingDockEventArgs>? ShipDockedToLoadingDock;
        public event EventHandler<ShipStartingLoadingEventArgs>? ShipStartingLoading;
        public event EventHandler<ShipLoadedContainerEventArgs>? ShipLoadedContainer;
        public event EventHandler<ShipDoneLoadingEventArgs>? ShipDoneLoading;
        public event EventHandler<ShipStartingUnloadingEventArgs>? ShipStartingUnloading;
        public event EventHandler<ShipUnloadedContainerEventArgs>? ShipUnloadedContainer;
        public event EventHandler<ShipDoneUnloadingEventArgs>? ShipDoneUnloading;
        public event EventHandler<ShipAnchoringEventArgs>? ShipAnchoring;
        public event EventHandler<ShipAnchoredEventArgs>? ShipAnchored;
        public event EventHandler<TruckLoadingFromHarborStorageEventArgs>? TruckLoadingFromStorage;
    }
}
