using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Xml.Linq;
using Gruppe8.HarbNet.Advanced;

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
        private  DateTime startTime;

        /// <summary>
        /// Gets the current date and time in simulation.
        /// </summary>
        /// <returns>Returns a DateTime object representing the date and time the simulation currently is in.</returns>
        private DateTime currentTime;

        /// <summary>
        /// Gets the date and time when the simulation ended.
        /// </summary>
        /// <returns>Returns a DateTime object representing the date and time when the simulation ended.</returns>
        private DateTime endTime;

        /// <summary>
        /// Gets the harbor object.
        /// </summary>
        /// <returns>Returns a harbor object.</returns>
        private Harbor harbor;

        /// <summary>
        /// Gets the number of containers transported from storage to ship on a loading round lasting an hour.
        /// </summary>
        /// <returns>Returns an int value representing the number of containers transported from storage to ship in a hour.</returns>
        private int numberOfStorageContainersToShipThisRound;

        /// <summary>
        /// Gets the number of containers transported from storage to truck on a loading round lasting an hour.
        /// </summary>
        /// <returns>Returns an int value representing the number of containers transported from storage to truck in a hour.</returns>
        private int numberOfStorageContainersToTrucksThisRound;

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

        /// <summary>
        /// History for all ships and containers in the simulation in the form of DailyLog objects. Each DailyLog object stores information for one day in the simulation and contains information about the location and status of all ships and containers that day.
        /// </summary>
        /// <returns>Returns a readOnlyCollection of Dailylog objects each representing one day of the simulation. Together the list represent the entire history of one simulation.</returns>
        public override ReadOnlyCollection<DailyLog> History { get { return HistoryIList.AsReadOnly(); } }

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
            this.harbor = (Harbor)harbor;
            this.startTime = simulationStartTime;
            this.endTime = simulationEndTime;
        }

        /// <summary>
        /// Running the simulation.
        /// </summary>
        /// <returns>Returns the history of the simulation in the form of log objects where each object contains information about all ships and containers on one day of the simulation.</returns>
        public override IList<DailyLog> Run()
        {

            this.currentTime = startTime;

            SimulationStartingEventArgs simulationStartingEventArgs = new(harbor, currentTime, "The simulation has started");
            
            SimulationStarting?.Invoke(this, simulationStartingEventArgs);

            HistoryIList.Add(new DailyLog(currentTime, harbor.Anchorage, harbor.GetShipsInTransit(), harbor.GetContainersStoredInHarbour(), harbor.ArrivedAtDestination,
                        harbor.GetShipsInLoadingDock(), harbor.GetShipsInShipDock()));

            while (currentTime < endTime)
            {
                SetStartLocationForAllShips();

                foreach (Ship ship in harbor.AllShips)
                {
                    ship.HasBeenAlteredThisHour = false;

                }

                harbor.GenerateTrucks();

                UndockingShips();

                AnchoringShips();

                DockingShips();

                UnloadingShips();

                LoadingShips();

                LoadingTrucksFromStorage();

                ContainersOnTrucksArrivingToDestination();

                InTransitShips();

                EndOf24HourPeriod();



                OneHourHasPassedEventArgs oneHourHasPassedEventArgs = new(currentTime, "One hour has passed in the simulation, which equals to one 'round'");
                OneHourHasPassed?.Invoke(this, oneHourHasPassedEventArgs);

                continue;
            }
            SimulationEndedEventArgs simulationEndedEventArgs = new(History, "The simulation has reached the end time and has ended.");

            SimulationEnded?.Invoke(this, simulationEndedEventArgs);

            return History;

        }

        /// <summary>
        /// Sets start location and initiates first status log for all ships, based on their startTime.
        /// </summary>
        private void SetStartLocationForAllShips()
        {
            foreach (Ship ship in harbor.AllShips)
            {
                ShipAnchoredEventArgs shipAnchoredEventArgs = new(ship, currentTime, "Ship has anchored to Anchorage.", harbor.AnchorageID);


                if (ship.StartDate.Year == currentTime.Year &&
                    ship.StartDate.Month == currentTime.Month &&
                    ship.StartDate.Day == currentTime.Day &&
                    ship.StartDate.Hour == currentTime.Hour &&
                    ship.StartDate.Minute == currentTime.Minute)
                {

                    harbor.AddNewShipToAnchorage(ship);

                    if (ship.IsForASingleTrip == true && harbor.GetFreeLoadingDock(ship.ShipSize) != null)
                    {
                        Guid loadingDock = harbor.MoveShipFromAnchorageToLoadingDock(ship.ID);

                        ShipIsDockedToLoadingDock(ship, loadingDock);
                    }
                    else if (ship.IsForASingleTrip == true && harbor.GetFreeLoadingDock(ship.ShipSize) == null)
                    {
                        ship.AddStatusChangeToHistory(currentTime, harbor.AnchorageID, Status.Anchored);
                        ShipAnchored?.Invoke(this, shipAnchoredEventArgs);
                    }
                    else
                    {
                        ship.AddStatusChangeToHistory(currentTime, harbor.AnchorageID, Status.Anchoring);
                        ship.AddStatusChangeToHistory(currentTime, harbor.AnchorageID, Status.Anchored);
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
            DateTime past24Hours = currentTime.AddHours(-24);
            if (currentTime.Hour == 0)
            {
                DailyLog harborDayLog = new(currentTime, harbor.Anchorage, harbor.GetShipsInTransit(), harbor.GetContainersStoredInHarbour(), harbor.ArrivedAtDestination,
                    harbor.GetShipsInLoadingDock(), harbor.GetShipsInShipDock());
                
                HistoryIList.Add(harborDayLog);


                foreach (Container container in harbor.GetContainersStoredInHarbour())
                {
                    container.AddAnotherDayInStorage();
                }

                Dictionary<Ship, List<StatusLog>> dayReviewAllShipLogs = new();
                foreach (Ship ship in harbor.AllShips)
                {
                    List<StatusLog> dayReviewShipLogs = new();

                    foreach (StatusLog log in ship.HistoryIList)
                    {

                        if (log.Timestamp >= past24Hours && log.Timestamp <= currentTime)
                        {
                            dayReviewShipLogs.Add(log);
                        }
                    }

                    dayReviewAllShipLogs.Add(ship, dayReviewShipLogs);
                }


                DayEndedEventArgs dayOverEventArgs = new(harborDayLog,dayReviewAllShipLogs, currentTime, "The day has passed and the state of the harbor on day-shifty has been logged.");
                DayEnded?.Invoke(this, dayOverEventArgs);

            }

            currentTime = currentTime.AddHours(1);
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


            foreach (Ship ship in harbor.AllShips)
            {
                if (ship.ID.Equals(shipID))
                {
                    ship.PrintHistory();
                    return;
                }

            
            }

            throw new ArgumentException("The ship you are trying to print does not exist in the Harbor the simulation is using. In order for the simulation to be able to print the ships " +
                "history the ship must be part of the simulated harbor.");

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
        /// Anchoring ship to anchorage outside harbor, ship.Status is set to Anchoring.
        /// </summary>
        private void AnchoringShips()
        {
            Guid anchorageID = harbor.AnchorageID;
            List<Ship> Anchorage = harbor.Anchorage.ToList();

            foreach (Ship ship in Anchorage)
            {
                ShipAnchoredEventArgs shipAnchoredEventArgs = new(ship, currentTime, "Ship has anchored to anchorage.", harbor.AnchorageID);

                Guid shipID = ship.ID;

                StatusLog lastStatusLog = GetStatusLog(ship);

                if (ShipIsAnchoring(ship, lastStatusLog))
                {
                    ship.HasBeenAlteredThisHour = true;

                    ship.AddStatusChangeToHistory(currentTime, anchorageID, Status.Anchored);

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
            return (StatusLog)ship.HistoryIList.Last();
        }

        /// <summary>
        /// Get the second to last StatusLog object from the ship's history.
        /// </summary>
        /// <param name="ship">The ship object the StatusLog belongs to.</param>
        /// <returns>The second to last StatusLog object.</returns>
        private StatusLog? GetSecondLastStatusLog(Ship ship)
        {
            return ship.HistoryIList != null && ship.HistoryIList.Count >= 2
                    ? (StatusLog)ship.HistoryIList[ship.HistoryIList.Count - 2]
                    : null;
        }

        /// <summary>
        /// Docking ships to the harbor, the shipStatus is set to docking.
        /// </summary>
        private void DockingShips()
        {

            List<Ship> Anchorage = harbor.Anchorage.ToList();
            List<Ship> ShipsInLoadingDock = new(harbor.shipsInLoadingDock.Keys);

            foreach (Ship ship in ShipsInLoadingDock)
            {
                Guid shipID = ship.ID;
                StatusLog lastStatusLog = GetStatusLog(ship);

                if (ShipCanBeAltered(ship, lastStatusLog) &&
                    (lastStatusLog.Status == Status.DockingToLoadingDock ||
                    (lastStatusLog.Status == Status.UnloadingDone && SingleTripShipInTransit(ship))))
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

                    else if (EmptySingleTripShip(ship)
                        && FreeDockExists(ship))
                    {
                        if (FreeDockExists(ship) && SingleTripShipInTransit(ship)
                            && ShipHasNoContainers(ship) && currentTime != startTime
                            && lastStatusLog.Status != Status.DockingToShipDock)
                        {
                            ShipNowDockingToShipDock(ship, shipID);
                        }
                    }

                    else if (ship.IsForASingleTrip && !FreeDockExists(ship) && ship.TransitStatus == TransitStatus.Anchoring)
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
                            dockID = harbor.DockShipToLoadingDock(shipID);
                            ShipIsDockedToLoadingDock(ship, dockID);
                        }
                    }
                    
                    else if (SingleTripShipAnchoring(ship))
                    {                
                        if (FreeDockExists(ship) && SingleTripShipInTransit(ship)
                            && ShipHasNoContainers(ship) && currentTime != startTime
                            && lastStatusLog.Status != Status.DockingToShipDock)
                        {
                            dockID = harbor.DockShipToShipDock(shipID);

                            ShipNowDockingToShipDock(ship, dockID);
                        }
                    }

                    else if (ship.IsForASingleTrip && !(FreeDockExists(ship)) &&
                        ContainsTransitStatus(ship) && ShipHasNoContainers(ship) && lastStatusLog.Status == Status.Anchoring)
                    {
                        ship.AddStatusChangeToHistory(currentTime, lastStatusLog.SubjectLocation, Status.Anchored);
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
            return ship.IsForASingleTrip && FreeDockExists(ship) && ship.TransitStatus == TransitStatus.Anchoring;
        }

        /// <summary>
        /// Checks if the ship is not empty and there is a loading dock for it.
        /// </summary>
        /// <param name="ship">Ship object that is checked if it is empty and if there is available loading dock.</param>
        /// <returns>Returns true if the ship contains containers and there is an available loading dock for the ship, if not then false is returned.</returns>
        private bool NonEmptyShipReadyForLoadingDock(Ship ship)
        {
            return harbor.FreeLoadingDockExists(ship.ShipSize) && ship.ContainersOnBoard.Count != 0 && !(ship.IsForASingleTrip && !ContainsTransitStatus(ship));
        }
        /// <summary>
        /// checks the status of the ship for conditions acceptable for docking.
        /// </summary>
        /// <param name="ship">Ship object that is checked if it can dock.</param>
        /// <param name="lastStatusLog">StatusLog object belonging to the specified ship.</param>
        /// <returns>Returns true if the ship is fit for dockings, if not then false is returned.</returns>
        private static bool ShipCanDock(Ship ship, StatusLog lastStatusLog)
        {
            return ShipCanBeAltered(ship, lastStatusLog) &&
                                (lastStatusLog.Status == Status.Anchored ||
                                lastStatusLog.Status == Status.DockedToShipDock ||
                                lastStatusLog.Status == Status.DockingToLoadingDock ||
                                lastStatusLog.Status == Status.DockingToShipDock ||
                                (lastStatusLog.Status == Status.UnloadingDone && SingleTripShipInTransit(ship)));
        }
        /// <summary>
        /// Ship is anchoring to anchorage.
        /// </summary>
        /// <param name="ship">Ship object that is anchoring to anchorage.</param>
        private void ShipAnchoringToAnchorage(Ship ship)
        {
            ShipAnchoringEventArgs shipAnchoringEventArgs = new(ship, currentTime, "Ship is anchoring to anchorage.", harbor.AnchorageID);

            ship.TransitStatus = TransitStatus.Anchoring;

            harbor.AddNewShipToAnchorage(ship);
            ship.AddStatusChangeToHistory(currentTime, harbor.AnchorageID, Status.Anchoring);
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
            Guid dockID = harbor.DockShipToShipDock(shipID);

            ship.AddStatusChangeToHistory(currentTime, dockID, Status.DockingToShipDock);

            ShipDockingToShipDockEventArgs shipDockingToShipDockEventArgs = new(ship, currentTime, "Ship is docking to ship dock.", dockID);
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
        private bool FreeDockExists(Ship ship)
        {
            return harbor.FreeShipDockExists(ship.ShipSize);
        }
        /// <summary>
        /// Starting the loading process.
        /// </summary>
        /// <param name="ship">Ship object that is loading.</param>
        /// <param name="dockID">Unique ID for the loading dock object the ship object is docked to.</param>
        private void StartLoadingProcess(Ship ship, Guid dockID)
        {
            ShipStartingLoadingEventArgs shipStartingLoadingEventArgs = new(ship, currentTime, "The ship is starting the loading process.", ship.CurrentLocation);

            ship.AddStatusChangeToHistory(currentTime, dockID, Status.Loading);

            ShipStartingLoading?.Invoke(this, shipStartingLoadingEventArgs);
        }
        /// <summary>
        /// Checks if the ship is a single trip ship and the status is in transit.
        /// </summary>
        /// <param name="ship">Ship object.</param>
        /// <returns>Returns true if the ship is set to single trip and the current status is transit, if not false is returned.</returns>
        private static bool SingleTripShipInTransit(Ship ship)
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
            ShipStartingUnloadingEventArgs shipStartingUnloadingEventArgs = new(ship, currentTime, "The ship is starting the unloading process.", ship.CurrentLocation);

            double percentTrucks = harbor.PercentOfContainersDirectlyLoadedFromShips;
            ship.ContainersLeftForTrucks = ship.GetNumberOfContainersToTrucks(percentTrucks);

            ship.AddStatusChangeToHistory(currentTime, dockID, Status.Unloading);

            ShipStartingUnloading?.Invoke(this, shipStartingUnloadingEventArgs);
        }
        /// <summary>
        /// Checks if the ship has been trying to dock for more than one hour.
        /// </summary>
        /// <param name="lastStatusLog">The last registered Statuslog object.</param>
        /// <returns>Returns true if the ship has been trying to dock to loading dock for more than one hour, if not then false is returned.</returns>
        private bool ShipDockingForMoreThanOneHour(StatusLog lastStatusLog)
        {
            return lastStatusLog.Status == Status.DockingToLoadingDock && (currentTime - lastStatusLog.Timestamp).TotalHours >= 1;
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
        /// Ship docked to loading dock.
        /// </summary>
        /// <param name="ship">Ship object docking to loading dock.</param>
        /// <param name="dockID">Unique ID for the loading dock object the ship object is docking to.</param>
        private void ShipIsDockedToLoadingDock(Ship ship, Guid dockID)
        {
            ship.AddStatusChangeToHistory(currentTime, dockID, Status.DockedToLoadingDock);

            ShipDockedToLoadingDockEventArgs shipDockedToLoadingDockEventArgs = new(ship, currentTime, "Ship has docked to loading dock.", dockID);

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

            foreach (Ship ship in harbor.DockedShipsInLoadingDock())
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
                        UpdateShipStatus(ship, lastStatusLog, secondLastStatusLog);

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
            return ShipCanBeAltered(ship, lastStatusLog) && secondLastStatusLog != null &&
                    (lastStatusLog.Status == Status.Unloading || lastStatusLog.Status == Status.DockedToLoadingDock && secondLastStatusLog != null);
        }
        /// <summary>
        /// Checks if the ship is unloading.
        /// </summary>
        /// <param name="ship">ship object that is checked if it is unloading.</param>
        /// <param name="lastStatusLog">The last registered Statuslog object.</param>
        /// <returns>Returns true if ship is currently unloading, if not then false is returned.</returns>
        private bool ShipNowUnloading(Ship ship, StatusLog lastStatusLog)
        {
            return ship.ContainersOnBoard.Count != 0 && (lastStatusLog.Status == Status.DockedToLoadingDock || lastStatusLog.Status == Status.Unloading);
        }
        /// <summary>
        /// Ship is finished unloading containers.
        /// </summary>
        /// <param name="ship">Ship object to be checked if it is done unloading.</param>
        /// <param name="currentLocation">Unique ID for the current location specified ship is located.</param>
        private void ShipFinishedUnloading(Ship ship, Guid currentLocation)
        {
            ShipDoneUnloadingEventArgs shipDoneUnloadingEventArgs = new(ship, currentTime, "The ship has finished unloading.", currentLocation);
            ship.AddStatusChangeToHistory(currentTime, currentLocation, Status.UnloadingDone);

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
        private void UpdateShipStatus(Ship ship, StatusLog lastStatusLog, StatusLog? secondLastStatusLog)
        {
            Guid currentLocation = lastStatusLog.SubjectLocation;

            if (ShipHasNoContainers(ship) && !SingleTripShipInTransit(ship))
            {

                ShipFinishedUnloading(ship, currentLocation);

                StartLoadingProcess(ship, currentLocation);

            }

            else if (ShipHasNoContainers(ship) && SingleTripShipInTransit(ship))
            {
                ShipFinishedUnloading(ship, currentLocation);

                Guid dockID = harbor.DockShipToShipDock(ship.ID);
                if (dockID != Guid.Empty)
                { 
                    ship.AddStatusChangeToHistory(currentTime, currentLocation, Status.DockingToShipDock); 
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
            double percentTrucks = harbor.PercentOfContainersDirectlyLoadedFromShips; 
            int numberOfContainersForTrucks = ship.GetNumberOfContainersToTrucks(percentTrucks);
            int numberOfContainersForStorage = ship.GetNumberOfContainersToStorage(percentTrucks);

            LoadingDock loadingDock = harbor.GetLoadingDockContainingShip(ship.ID);

            Crane? testCrane = harbor.GetFreeLoadingDockCrane();

            int maxLoadsPerHour = 0;
            int numberOfRepeats;

            if (testCrane != null)
            {
                maxLoadsPerHour = Math.Min(harbor.LoadsPerAgvPerHour, testCrane.ContainersLoadedPerHour);
            }

            if (maxLoadsPerHour == harbor.LoadsPerAgvPerHour)
            {
                numberOfRepeats = maxLoadsPerHour * harbor.AgvFree.Count;
            }
            else
            {
                numberOfRepeats = maxLoadsPerHour * Math.Min(harbor.DockCranes.Count, harbor.HarborStorageAreaCranes.Count);
            }

            for (int i = 0; i < ship.ContainersOnBoard.Count && i < numberOfRepeats; i++)
            {
                Container? movedContainer = MoveOneContainerFromShip(ship, numberOfContainersForTrucks, numberOfContainersForStorage, loadingDock);

                if (movedContainer != null)
                {
                    ShipUnloadedContainerEventArgs shipUnloadedContainerEventArgs = new(ship, currentTime, "Ship has unloaded one container.", movedContainer);

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

            if (numberOfContainersForTrucks != 0 && harbor.TrucksInQueue.Count != 0)
            {
                container = MoveContainerFromShipToTruck(ship);
                if (container != null)
                {
                    harbor.SendTruckOnTransit(loadingDock, container);
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
            Crane? craneDock = harbor.GetFreeLoadingDockCrane();

            Agv? agv = harbor.GetFreeAgv();


            if (agv != null && craneDock!= null)
            {
                Container? unloadedContainer = harbor.ShipToCrane(ship, craneDock, currentTime);
                if (unloadedContainer != null)
                {
                    harbor.CraneToAgv(craneDock, agv, currentTime);

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
            Agv? agv = harbor.GetAgvContainingContainer(container);
            if (agv == null)
            {
                return;
            }

            Crane? crane = harbor.GetFreeStorageAreaCrane();
            if (crane == null)
            {
                return; 
            }

            harbor.AgvToCrane(crane, agv, currentTime);
            harbor.CraneToContainerRow(crane, currentTime);

        }

        /// <summary>
        /// Unloads a container from the given ship's cargo on to a crane and then on to an available trucks cargo.
        /// </summary>
        /// <param name="ship">The ship object the container is unloaded from.</param>
        /// <returns>Returns the container object that is being moved from the ship to crane to truck.</returns>
        private Container? MoveContainerFromShipToTruck(Ship ship)
        {
            LoadingDock loadingDock = harbor.GetLoadingDockContainingShip(ship.ID);

            Crane? craneDock = harbor.GetFreeLoadingDockCrane();

            Truck? truck = harbor.GetFreeTruck();
            
            if (truck != null)
            {
                harbor.RemoveTruckFromQueue(truck);
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
            
            Container? container = harbor.ShipToCrane(ship, craneDock, currentTime);
            if (container == null)
            {
                harbor.CraneToTruck(craneDock, truck, currentTime);
                ship.ContainersLeftForTrucks--;
            }

            return container;
        }

        /// <summary>
        /// Undock ship from harbor, and set its status to Transit.
        /// </summary>
        private void UndockingShips()
        {
            foreach (Ship ship in harbor.DockedShipsInLoadingDock())
            {

                Guid shipID = ship.ID;

                StatusLog lastStatusLog = GetStatusLog(ship);

                if (ShipCanUndockFromDock(ship, lastStatusLog)) 
                {
                    bool containsTransitStatus = ContainsTransitStatus(ship);

                    if (SingleTripShipCanDockToShipDock(ship, lastStatusLog))
                    {
                        Guid dockID = harbor.DockShipToShipDock(ship.ID);
                        ship.AddStatusChangeToHistory(currentTime, dockID, Status.DockingToShipDock);

                    }

                    else if (ship.IsForASingleTrip && !containsTransitStatus && lastStatusLog.Status == Status.DockedToLoadingDock)
                    {
                        ship.AddStatusChangeToHistory(currentTime, harbor.TransitLocationID, Status.Undocking);
                        ship.TransitStatus = TransitStatus.Leaving;
                    }

                    else if (ShipIsDockingToShipDock(lastStatusLog))
                    {
                        Guid dockID = lastStatusLog.SubjectLocation;
                        ship.AddStatusChangeToHistory(currentTime, dockID, Status.DockedToShipDock);

                        ShipDockedToShipDockEventArgs shipDockedToShipDockEventArgs = new(ship, currentTime, "Ship has docked to ship dock.", dockID);
                        ShipDockedToShipDock?.Invoke(this, shipDockedToShipDockEventArgs);
                    }

                    else if (ShipIsFinishedLoadingContainers(lastStatusLog))
                    {
                        ship.AddStatusChangeToHistory(currentTime, harbor.TransitLocationID, Status.Undocking);
                        ship.TransitStatus = TransitStatus.Leaving;
                    }

                    else if (ShipIsUndocking(lastStatusLog))
                    {

                        harbor.UnDockShipFromLoadingDockToTransit(shipID);
                        ship.AddStatusChangeToHistory(currentTime, harbor.TransitLocationID, Status.Transit);

                        ShipInTransitEventArgs shipInTransitEventArgs = new(ship, currentTime, "Ship has left the harbor and is in transit.", harbor.TransitLocationID);

                        ShipInTransit?.Invoke(this, shipInTransitEventArgs);
                    }

                    ship.HasBeenAlteredThisHour = true;

                }
            }

            foreach (Ship ship in harbor.GetShipsInShipDock())
            {
                Guid shipID = ship.ID;

                StatusLog lastStatusLog = GetStatusLog(ship);
                
                if (ShipCanUndockFromDock(ship, lastStatusLog))
                {
                    bool containsTransitStatus = ContainsTransitStatus(ship);

                    if (ShipIsDockingToShipDock(lastStatusLog))
                    {
                        Guid dockID = lastStatusLog.SubjectLocation;
                        ship.AddStatusChangeToHistory(currentTime, dockID, Status.DockedToShipDock);
                        ShipDockedToShipDockEventArgs shipDockedToShipDockEventArgs = new(ship, currentTime, "Ship has docked to ship dock.", dockID);
                        ShipDockedToShipDock?.Invoke(this, shipDockedToShipDockEventArgs);
                    }
                    ship.HasBeenAlteredThisHour = true;

                }
            }

            Guid anchorageID = harbor.AnchorageID;
            List<Ship> Anchorage = harbor.Anchorage.ToList();

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
            Guid oldLocation = harbor.UnDockShipFromAnchorageToTransit(ship.ID);

            ShipInTransitEventArgs shipInTransitEventArgs = new(ship, currentTime, "Ship as left the harbor and is in transit.", harbor.TransitLocationID);

            ship.AddStatusChangeToHistory(currentTime, harbor.TransitLocationID, Status.Transit);

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

            ShipUndockingEventArgs shipUndockingEventArgs = new(ship, currentTime, "Ship is undocking from Anchorage.", currentLocation);

            ship.AddStatusChangeToHistory(currentTime, harbor.TransitLocationID, Status.Undocking);
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
            return ship.HasBeenAlteredThisHour == false && lastStatusLog != null &&
                    (lastStatusLog.Status == Status.Undocking || lastStatusLog.Status == Status.LoadingDone || 
                    (lastStatusLog.Status == Status.DockedToLoadingDock && ship.IsForASingleTrip && !ContainsTransitStatus(ship)) ||
                    (lastStatusLog.Status == Status.UnloadingDone && ContainsTransitStatus(ship)) || lastStatusLog.Status == Status.DockingToShipDock);
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
            return ship.IsForASingleTrip == true && containsTransitStatus && lastStatusLog.Status != Status.DockingToShipDock && FreeDockExists(ship) != false;
        }

        /// <summary>
        /// Checks if ship is in the process docking to ship dock.
        /// </summary>
        /// <param name="lastStatusLog">The last StatusLog in the ship's history.</param>
        /// <returns>Returns true if the ship object can dock to ship dock, if not then false is returned.</returns>
        private bool ShipIsDockingToShipDock(StatusLog lastStatusLog)
        {
            return lastStatusLog.Status == Status.DockingToShipDock && (currentTime - lastStatusLog.Timestamp).TotalHours >= 1;
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
            return lastStatusLog.Status == Status.Undocking && (currentTime - lastStatusLog.Timestamp).TotalHours >= 1;
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
            foreach (Ship ship in harbor.DockedShipsInLoadingDock())
            {

                StatusLog lastStatusLog = GetStatusLog(ship);
                StatusLog? secondLastStatusLog = GetSecondLastStatusLog(ship);

                bool shipIsNotSingleTripAndIsDoneUnloading = (lastStatusLog.Status == Status.UnloadingDone && (ship.IsForASingleTrip != true));
                bool shipIsSingleTripAndHasLastUnloadedAndHasNotBeenOnTrip = (lastStatusLog.Status == Status.UnloadingDone && (ship.IsForASingleTrip == true && !ContainsTransitStatus(ship)));
                bool LastLocationWasDockedToShipDock = (lastStatusLog.Status == Status.DockedToLoadingDock && secondLastStatusLog != null && secondLastStatusLog.Status == Status.DockedToShipDock);
                bool shipIsSingleTripAndHasNotBeenOnTrip = (ship.IsForASingleTrip == true && !ContainsTransitStatus(ship));

                if (ShipCanBeAltered(ship, lastStatusLog) &&
                    shipIsNotSingleTripAndIsDoneUnloading || 
                    shipIsSingleTripAndHasLastUnloadedAndHasNotBeenOnTrip ||
                     (lastStatusLog.Status == Status.Loading) ||
                     (LastLocationWasDockedToShipDock && shipIsSingleTripAndHasNotBeenOnTrip))
                {
                    Guid currentLocation = lastStatusLog.SubjectLocation;
                    
                    if (ship.ContainersOnBoard.Count < ship.ContainerCapacity && ship.CurrentWeightInTonn < ship.MaxWeightInTonn)
                    {
                        if (harbor.storedContainers.Keys.Count != 0 && ship.ContainersOnBoard.Count < ship.ContainerCapacity)
                        {
                            if (lastStatusLog.Status != Status.Loading)
                            {
                                ship.AddStatusChangeToHistory(currentTime, currentLocation, Status.Loading);
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
            ship.AddStatusChangeToHistory(currentTime, currentLocation, Status.Undocking);

            foreach (Container container in ship.ContainersOnBoard)
            {
                container.AddStatusChangeToHistory(Status.Transit, currentTime);
            }


            ShipUndockingEventArgs shipUndockingEventArgs = new(ship, currentTime, "Ship is undocking from dock.", currentLocation);
            ShipUndocking?.Invoke(this, shipUndockingEventArgs);
        }
        /// <summary>
        /// Ship is now finished Loading containers. Adds the statuschange to the ship's history.
        /// </summary>
        /// <param name="ship">Ship object to be checked if it is done loading containers.</param>
        /// <param name="currentLocation">Unique ID for the current location specified ship is located.</param>
        private void ShipFinishedLoading(Ship ship, Guid currentLocation)
        {
            ShipDoneLoadingEventArgs shipDoneLoadingEventArgs = new(ship, currentTime, "The ship is done loading.", currentLocation);
            ship.AddStatusChangeToHistory(currentTime, currentLocation, Status.LoadingDone);

            ShipDoneLoading?.Invoke(this, shipDoneLoadingEventArgs);
        }

        /// <summary>
        /// Loads the given ship with containers for one hour.
        /// </summary>
        /// <param name="ship">Ship object to be loading containers for one hour.</param>
        /// <param name="currentLocation">Unique ID for the current location specified ship is located.</param>
        private void LoadShipForOneHour(Ship ship, Guid currentLocation)
        {
            numberOfStorageContainersToShipThisRound = harbor.NumberOfContainersInStorageToShips();

            Crane? testCrane = harbor.GetFreeLoadingDockCrane();

            int maxLoadsPerHour = 0;
            int numberOfRepeats;

            if (testCrane != null)
            {
                maxLoadsPerHour = Math.Min(harbor.LoadsPerAgvPerHour, testCrane.ContainersLoadedPerHour);
            }

            if (maxLoadsPerHour == harbor.LoadsPerAgvPerHour)
            { 
                numberOfRepeats = maxLoadsPerHour * harbor.AgvFree.Count;
            }
            else
            {
                numberOfRepeats = maxLoadsPerHour * Math.Min(harbor.DockCranes.Count, harbor.HarborStorageAreaCranes.Count);
            }

            for (int i = 0; i < numberOfStorageContainersToShipThisRound && i < numberOfRepeats; i++)
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
                    ShipLoadedContainerEventArgs shipLoadedContainerEventArgs = new(ship, currentTime, "Ship has loaded one container.", loadedContainer);
                    ShipLoadedContainer?.Invoke(this, shipLoadedContainerEventArgs);
                }
            }
        }

        /// <summary>
        /// Loading container on to ship.
        /// </summary>
        /// <param name="ship">Ship object the containers will be loaded on.</param>
        /// <returns>Returns the container object of containers moved on ship.</returns>
        internal Container? LoadContainerOnShip(Ship ship)
        {
            numberOfStorageContainersToShipThisRound = harbor.NumberOfContainersInStorageToShips();
            Crane? storageCrane = harbor.GetFreeStorageAreaCrane();
            Crane? dockCrane = harbor.GetFreeLoadingDockCrane();

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

            if (ShipHasNoContainers(ship) || ship.ContainersOnBoard.Last().Size == ContainerSize.Full && harbor.GetStoredContainer(ContainerSize.Half) != null
                || harbor.GetStoredContainer(ContainerSize.Full) == null && harbor.GetStoredContainer(ContainerSize.Half) != null)
            {
                underMaxWeight = ship.MaxWeightInTonn >= ship.CurrentWeightInTonn + (int)ContainerSize.Half;
                underMaxCapacity = ship.ContainerCapacity > ship.ContainersOnBoard.Count + 1;

                if (underMaxCapacity && underMaxWeight)
                {
                    containerToBeLoaded = MoveOneContainerFromContainerRowToAgv(ContainerSize.Half);

                    return containerToBeLoaded;

                }
            }

            else if (ship.ContainersOnBoard.Last().Size == ContainerSize.Half && harbor.GetStoredContainer(ContainerSize.Full) != null
                || harbor.GetStoredContainer(ContainerSize.Half) == null && harbor.GetStoredContainer(ContainerSize.Full) != null)
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
        /// Moves a container from the harbor storage to AGV.
        /// </summary>
        /// <param name="containerSize">ContainerSize enum representing the size of the container.</param>
        /// <returns>Returns the container object that was moved.</returns>
        private Container? MoveOneContainerFromContainerRowToAgv(ContainerSize containerSize)
        {
            Agv? agv = harbor.GetFreeAgv();
            Crane? storageCrane = harbor.GetFreeStorageAreaCrane();
            if (storageCrane == null)
            {
                return null;
            }

            Container? container = harbor.ContainerRowToCrane(containerSize, storageCrane, currentTime);

            if (container == null || agv == null)
            {
                return null;
            }
            harbor.CraneToAgv(storageCrane, agv, currentTime);

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
                agv = harbor.GetAgvContainingContainer(container);
            }

            Crane? loadingDockCrane = harbor.GetFreeLoadingDockCrane();

            if (agv == null || loadingDockCrane == null)
            {
                return null;
            }
            
            harbor.AgvToCrane(loadingDockCrane, agv, currentTime);
            harbor.CraneToShip(loadingDockCrane, ship, currentTime);
            return container;

        }

        /// <summary>
        /// Checks if any of the ships currently in transit is ready to enter the harbor, if so adds the ship to the anchorage.
        /// </summary>
        private void InTransitShips()
        {
            foreach (Ship ship in harbor.ShipsInTransit.Keys)
            {
                StatusLog lastStatusLog = GetStatusLog(ship);

                if (IsShipInTransit(ship, lastStatusLog))
                {

                    if (ShipHasReturnedToHarbor(ship, lastStatusLog))
                    {
                        harbor.RestockContainers(ship, currentTime);

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
        private bool IsShipInTransit(Ship ship, StatusLog lastStatusLog)
        {
            return ship.HasBeenAlteredThisHour == false && lastStatusLog != null && lastStatusLog.Status == Status.Transit;
        }

        /// <summary>
        /// Checks if the given ship has returned to harbor.
        /// </summary>
        /// <param name="ship">Ship object to check on to see if it has returned to harbor.</param>
        /// <param name="LastHistoryStatusLog">The last StatusLog in the ship's history.</param>
        /// <returns>Returns true if the given ship has returned to harbor from its roundtrip, if not then false is returned.</returns>
        private bool ShipHasReturnedToHarbor(Ship ship, StatusLog LastHistoryStatusLog)
        {
            double DaysSinceTransitStart = (currentTime - LastHistoryStatusLog.Timestamp).TotalDays;
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

            while (harbor.TrucksInQueue.Count > 0 && numberOfContainersToTrucks > containersToTruck)
            {
                
                foreach (Crane crane in harbor.HarborStorageAreaCranes)
                {
                    int maxLoadsPerHour = crane.ContainersLoadedPerHour;

                    for (int i = 0; i < maxLoadsPerHour && containersToTruck < numberOfContainersToTrucks; i++)
                    {
                        container = harbor.storedContainers.FirstOrDefault().Key;
                        if (container == null)
                        {
                            break;
                        }

                        if (harbor.TrucksInQueue.Count > 0)
                        {
                          
                            Container? loadedContainer = MoveOneContainerFromContainerRowToTruck(container);
                            Truck? truck = null;

                            if (loadedContainer != null)
                            {
                                containersToTruck++;
                                truck = harbor.SendTruckOnTransit(container);

                                if (truck != null)
                                {
                                    TruckLoadingFromHarborStorageEventArgs truckLoadingFromStorageEventArgs = new(truck, currentTime, "One truck has loaded a container and has left");
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
            foreach(Truck truck in harbor.TrucksInTransit)
            {
                
                Container? container = truck.Container;
                if(container != null)
                {
                    StatusLog? lastLogContainer = container.HistoryIList.Last();

                    if ((currentTime - lastLogContainer.Timestamp).TotalHours >= 1)
                    {
                        Container arrivedContainer = truck.UnloadContainer();
                        harbor.ArrivedAtDestination.Add(arrivedContainer);

                        truck.Location = harbor.DestinationID;

                        arrivedContainer.CurrentLocation = harbor.DestinationID;
                        arrivedContainer.AddStatusChangeToHistory(Status.ArrivedAtDestination, currentTime);
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
            if (harbor.shipsInLoadingDock.Count != 0)
            {
                numberOfStorageContainersToTrucksThisRound = harbor.NumberOfContainersInStorageToTrucks();
                return numberOfStorageContainersToTrucksThisRound;
            }
            else
            {
                return numberOfStorageContainersToTrucksThisRound;
            }
                
        }
        /// <summary>
        /// Moves the given container the harbor storage area on to a available truck.
        /// </summary>
        /// <param name="container">Container object to be moved from storage to truck.</param>
        /// <returns>Returns the container object moved from storage to a truck.</returns>
        private Container? MoveOneContainerFromContainerRowToTruck(Container container)
        {
            Crane? storageCrane = harbor.GetFreeStorageAreaCrane();
            if (storageCrane == null)
            {
                return null;
            }

            Container? containerFound = harbor.ContainerRowToCrane(container.Size, storageCrane, currentTime);

            if (container == null)
            {
                return null;
            }
            Truck? truck = harbor.GetFreeTruck();

            if (truck != null && containerFound != null)
            {
                Container containerOnTruck = harbor.CraneToTruck(storageCrane, truck, currentTime);

                return containerOnTruck;
            }

            return null;

        }


        /// <summary>
        /// Returns a string that contains information about the entire history of each ship in the harbor simulation. Information in the string includes the historical data regarding the
        /// Location, Name, size, status, max weight, Current weight, container capacity, number of containers onboard and ID of 
        /// all ships at the end of every day of the simulation.
        /// </summary>
        /// <returns> a string that contains information about all ships in the previous simulation. Returns an empty string if no simulation has been run.</returns>
        public override String HistoryToString()
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
            else { return ""; }
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

            foreach (Ship ship in harbor.AllShips)
            {
                if (ship.ID.Equals(shipID))
                {
                    return ship.HistoryToString();
                }
            }

            throw new ArgumentException("The ship you are trying to get the history from does not exist in the Harbor object the simulation is using. In order for the simulation to be able to provide a String of the ships history " +
                "the ship must be added to the harbor the simulation is using.");
        }

        /// <summary>
        /// Returns a string containing information about the entire history of each ship or each container in the simulation.
        /// </summary>
        /// <param name="ShipsOrContainers">Sending inn "ships" returns the history of all ships in the previous simulation. Sending inn "containers" return the history of each container in the previous simulation</param>
        /// <returns>Returns a String value containing the entire history of all ships or all containers of the simulation. Returns an empty string if wrong value is given in param or no simulation has been run.</returns>
        public override String HistoryToString(String ShipsOrContainers)
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
                else { return ""; }
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
        public override String HistoryToString(Ship ship)
        {
            return ship.HistoryToString();
        }

        /// <summary>
        /// Returns a string that contains information about the start time of the simulation, end time of the simulation and the ID of the harbour used.
        /// </summary>
        /// <returns> a string that contains information about the start time, end time of the simulation and the ID of the harbour used.</returns>
        public override string ToString()
        {
            return ($"Simulation start time: {startTime.ToString()}, end time: {endTime.ToString()}, harbor ID: {harbor.ID}");
        }
    
    }

    /// <summary>
    /// The EventArgs class for the SimulationStarting event. This event is raised when the simulation starts.
    /// </summary>
    public class SimulationStartingEventArgs : EventArgs
    {
        /// <summary>
        /// The harbor that is being simulated.
        /// </summary>
        /// <returns>The harbor object of the harbor being simulated.</returns>
        public Harbor HarborToBeSimulated { get; internal set; }

        /// <summary>
        /// The time the simulation will start from.
        /// </summary>
        /// <returns>Datetime object representing the simulations start time.</returns>
        public DateTime StartDate { get; internal set; }

        /// <summary>
        /// A description of the event.
        /// </summary>
        /// <returns>String describing the event.</returns>
        public string Description { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the SimulationStartingEventArgs class.
        /// </summary>
        /// <param name="harborToBeSimulated">The harbor object that is being simulated.</param>
        /// <param name="startDate">The date and time the simulation starts.</param>
        /// <param name="description">String value containing a description of the event.</param>
        public SimulationStartingEventArgs(Harbor harborToBeSimulated, DateTime startDate, string description)
        {
            HarborToBeSimulated = harborToBeSimulated;
            StartDate = startDate;
            Description = description;
        }
    }

    /// <summary>
    /// The EventArgs class for the OneHourHasPassed event. This event is raised every hour of the simulation.
    /// </summary>
    public class OneHourHasPassedEventArgs : EventArgs
    {

        /// <summary>
        /// The time in the simulation the event was raised.
        /// </summary>
        /// <returns>Returns a DateTime object representing the date and time an event was raised in the simulation.</returns>
        public DateTime CurrentTime { get; internal set; }

        /// <summary>
        /// A description of the event.
        /// </summary>
        /// <returns>Returns a string value containing a description of the event.</returns>
        public string Description { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the OneHourHasPassedEventArgs class.
        /// </summary>
        /// <param name="currentTime">The date and time in the simulation the event was raised.</param>
        /// <param name="description">A string value containing a description of the event.</param>
        public OneHourHasPassedEventArgs(DateTime currentTime, string description)
        {
            CurrentTime = currentTime;
            Description = description;
        }
    }

    /// <summary>
    /// The EventArgs class for the SimulationEnded event. This event is raised when the simulation ends.
    /// </summary>
    public class SimulationEndedEventArgs : EventArgs
    {
        /// <summary>
        /// Returns the history for all ships and containers in the simulation in the form of DailyLog objects. Each DailyLog object stores information for one day in the simulation and contains information about the location and status of all ships and containers that day.
        /// </summary>
        /// <returns>Returns a IList of Dailylog objects each representing one day of the simulation. Together the list represent the entire history of one simulation.</returns>
        public ReadOnlyCollection<DailyLog> SimulationHistory { get; internal set; }

        /// <summary>
        /// A description of the event.
        /// </summary>
        /// <returns>String describing the event.</returns>
        public string Description { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the SimulationEndedEventArgs class.
        /// </summary>
        /// <param name="simulationHistory">A collection of DailyLog objects that together represent the history of the simulation.</param>
        /// <param name="description">A string value containing a description of the event.</param>
        public SimulationEndedEventArgs(ReadOnlyCollection<DailyLog> simulationHistory, string description)
        {
            SimulationHistory = simulationHistory;
            Description = description;
        }
    }
    
    /// <summary>
    /// The EventArgs class for the DayEnded event.
    /// </summary>
    public class DayEndedEventArgs : EventArgs
    {
        /// <summary>
        /// A DailyLog object containing information about the previous day in the simulation.
        /// </summary>
        /// <returns>Returns a DailyLog object containing information about the state of the simulation at the time the object was created</returns>
        public DailyLog TodaysLog { get; internal set; }

        /// <summary>
        /// A Dictionary collection containing all ships and their logs from the previous day in the simulation.
        /// </summary>
        /// <returns>Returns a Dictionary collection containing Ship-List pairs, where List is the history of the ship from the previous day.</returns>
        public Dictionary<Ship, List<StatusLog>> DayReviewAllShipLogs { get; internal set; }

        /// <summary>
        /// The time in the simulation the event was raised.
        /// </summary>
        /// <returns>Returns a DateTime object representing the time in the simulation the event was raised</returns>
        public DateTime CurrentTime { get; internal set; }

        /// <summary>
        /// A description of the event.
        /// </summary>
        /// <returns>Returns a String value describing the event.</returns>
        public string Description { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the DayEndedEventArgs class.
        /// </summary>
        /// <param name="todaysLog">A DailyLog object containing information about the previous day in the simulation.</param>
        /// <param name="dayReviewAllShipLogs">A Dictionary collection with Ship-List pair, where the List is the history of the ship from the previous day.</param>
        /// <param name="currentTime">The date and time in the simulation the event was raised.</param>
        /// <param name="description">A string value containing a description of the event.</param>
        public DayEndedEventArgs(DailyLog todaysLog, Dictionary<Ship, List<StatusLog>> dayReviewAllShipLogs, DateTime currentTime, string description)
        {
            TodaysLog = todaysLog;
            DayReviewAllShipLogs = dayReviewAllShipLogs;
            CurrentTime = currentTime;
            Description = description;
        }
    }

    /// <summary>
    /// Base EventArgs with basic args to be used as a base in more complex events where a ship is involved.
    /// </summary>
    public class BaseShipEventArgs : EventArgs
    {
        /// <summary>
        /// The ship involved in the event.
        /// </summary>
        /// <returns>Returns a ship object that is involved in the event in the simulation.</returns>
        public Ship Ship { get; internal set; }

        /// <summary>
        /// The time in the simulation the event was raised.
        /// </summary>
        /// <returns>Returns a DateTime object representing the date and time an event was raised in the simulation.</returns>
        public DateTime CurrentTime { get; internal set; }
        /// <summary>
        /// A description of the event.
        /// </summary>
        /// <returns>Returns a String value describing the event.</returns>
        public string Description { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the BaseShipEventArgs class.
        /// </summary>
        /// <param name="ship">The ship object involved in the event.</param>
        /// <param name="currentTime">The current date time in the simulation.</param>
        /// <param name="description">String value containing a description of the event.</param>
        public BaseShipEventArgs(Ship ship, DateTime currentTime, string description)
        {
            Ship = ship;
            CurrentTime = currentTime;
            Description = description;
        }
    }
    /// <summary>
    /// The EventArgs class for the ShipUndocked event. This event is raised when a ship undocks from any dock.
    /// </summary>
    public class ShipUndockingEventArgs : BaseShipEventArgs
    {
        /// <summary>
        /// The unique ID of the location the ship undocked from, Anchorage or Dock.
        /// </summary>
        /// <returns>Guid object representing the ID of the location the ship undocked from.</returns>
        public Guid LocationID { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the ShipUndockingEventArgs class.
        /// </summary>
        /// <param name="ship">The ship object involved in the event.</param>
        /// <param name="currentTime">The current date and time in the simulation.</param>
        /// <param name="locationID">The unique ID of the location the ship undocked from.</param>
        /// <param name="description">String value containing a description of the event.</param>
        public ShipUndockingEventArgs(Ship ship, DateTime currentTime, string description, Guid locationID)
            : base(ship, currentTime, description)
        {
            LocationID = locationID;
        }
    }

    /// <summary>
    /// The EventArgs class for the ShipInTransit event. This event is raised when a ship enters transit.
    /// </summary>
    public class ShipInTransitEventArgs : BaseShipEventArgs
    {
        /// <summary>
        /// The unique ID of the transit location the ship is located at.
        /// </summary>
        /// <returns>Guid object representing the ID of the transit location the ship is located at.</returns>
        public Guid TransitLocationID { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the ShipUndockingEventArgs class.
        /// </summary>
        /// <param name="ship">The ship object involved in the event.</param>
        /// <param name="currentTime">The current date and time in the simulation.</param>
        /// <param name="description">String value containing a description of the event.</param>
        /// <param name="transitLocationID">The unique ID of the transit location the ship is located at.</param>
        public ShipInTransitEventArgs(Ship ship, DateTime currentTime, string description, Guid transitLocationID) 
            : base(ship, currentTime, description)
        {
            this.TransitLocationID = transitLocationID;
        }
    }

    /// <summary>
    /// The EventArgs class for the ShipDockingToShipDock event. This event is raised when a ship is docking to a harbor shipdock.
    /// </summary>
    public class ShipDockingToShipDockEventArgs : BaseShipEventArgs
    {
        /// <summary>
        /// The unique ID of the dock the ship is docking to.
        /// </summary>
        /// <returns>Returns a Guid object representing the ID of the dock the ship is docking to.</returns>
        public Guid DockID { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the ShipUndockingEventArgs class.
        /// </summary>
        /// <param name="ship">The ship object involved in the event.</param>
        /// <param name="currentTime">The current date and time in the simulation.</param>
        /// <param name="description">String value containing a description of the event.</param>
        /// <param name="dockID">The unique ID of the dock the ship is docking to.</param>
        public ShipDockingToShipDockEventArgs(Ship ship, DateTime currentTime, string description, Guid dockID)
            : base(ship, currentTime, description)
        {
            DockID = dockID;
        }
    }

    /// <summary>
    /// The EventArgs class for the shipDockedToShipDock event. This event is raised when a ship has successfully docked to a harbor shipdock.
    /// </summary>
    public class ShipDockedToShipDockEventArgs : BaseShipEventArgs
    {
        /// <summary>
        /// The unique ID of the dock the ship docked to.
        /// </summary>
        /// <returns>Guid object representing the ID of the dock the ship docked to.</returns>
        public Guid DockID { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the ShipUndockingEventArgs class.
        /// </summary>
        /// <param name="ship">The ship object involved in the event.</param>
        /// <param name="currentTime">The current date and time in the simulation.</param>
        /// <param name="description">String value containing a description of the event.</param>
        /// <param name="dockID">The unique ID of the dock the ship docked to.</param>
        public ShipDockedToShipDockEventArgs(Ship ship, DateTime currentTime, string description, Guid dockID) 
            : base(ship, currentTime, description)
        {
            DockID = dockID;
        }

    }

    /// <summary>
    /// The EventArgs class for the shipDockingToLoadingDock event. This event is raised when a ship is docking to a harbor loading dock.
    /// </summary>
    public class ShipDockingToLoadingDockEventArgs : BaseShipEventArgs
    {
        /// <summary>
        /// The unique ID of the dock the ship is docking to.
        /// </summary>
        /// <returns>Guid object representing the ID of the dock the ship is docking to</returns>
        public Guid DockID { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the ShipUndockingEventArgs class.
        /// </summary>
        /// <param name="ship">The ship object involved in the event.</param>
        /// <param name="currentTime">The current date and time in the simulation.</param>
        /// <param name="description">String value containing a description of the event.</param>
        /// <param name="dockID">The unique ID of the dock the ship is docking to.</param>
        public ShipDockingToLoadingDockEventArgs(Ship ship, DateTime currentTime, string description, Guid dockID) 
            : base(ship, currentTime, description)
        {
            DockID = dockID;
        }
    }

    /// <summary>
    /// The EventArgs class for the shipDockedToLoadingDock event. This event is raised when a ship has successfully docked to a harbor loading dock.
    /// </summary>
    public class ShipDockedToLoadingDockEventArgs : BaseShipEventArgs
    {
        /// <summary>
        /// The unique ID of the dock the ship docked to.
        /// </summary>
        /// <returns>Guid object representing the ID of the dock the ship docked to.</returns>
        public Guid DockID { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the ShipUndockingEventArgs class.
        /// </summary>
        /// <param name="ship">The ship object involved in the event.</param>
        /// <param name="currentTime">The current date and time in the simulation.</param>
        /// <param name="description">String value containing a description of the event.</param>
        /// <param name="dockID">The unique ID of the dock the ship docked to.</param>
        public ShipDockedToLoadingDockEventArgs(Ship ship, DateTime currentTime, string description, Guid dockID) 
            : base(ship, currentTime, description)
        {
            DockID = dockID;
        }

    }

    /// <summary>
    /// The EventArgs class for the ShipStartingLoading event. This event is raised when a ship starts to load containers from the harbor in to the ship's cargohold.
    /// </summary>
    public class ShipStartingLoadingEventArgs : BaseShipEventArgs
    {
        /// <summary>
        /// The unique ID of the dock the ship is located at and loading from.
        /// </summary>
        /// <returns>Guid object representing the ID of the dock the ship is docking to</returns>
        public Guid DockID { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the ShipStartingUnloadingEventArgs class.
        /// </summary>
        /// <param name="ship">The ship object involved in the event.</param>
        /// <param name="currentTime">The current date and time in the simulation.</param>
        /// <param name="description">String value containing a description of the event.</param>
        /// <param name="dockID">The unique ID of the dock the ship is located at and loading from.</param>
        public ShipStartingLoadingEventArgs(Ship ship, DateTime currentTime, string description, Guid dockID)
            : base(ship, currentTime, description)
        {
            DockID = dockID;
        }
    }

    /// <summary>
    /// The EventArgs class for the ShipLoadedContainer event. This event is raised when a container is loaded onboard a ships cargo.
    /// </summary>
    public class ShipLoadedContainerEventArgs : BaseShipEventArgs
    {
        /// <summary>
        /// The container loaded onboard the ship.
        /// </summary>
        /// <returns>Container object representing the container loaded omboard the ship.</returns>
        public Container Container { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the ShipUndockingEventArgs class.
        /// </summary>
        /// <param name="ship">The ship object involved in the event.</param>
        /// <param name="currentTime">The current date and time in the simulation.</param>
        /// <param name="description">String value containing a description of the event.</param>
        /// <param name="container">The container loaded onboard the ship.</param>
        public ShipLoadedContainerEventArgs(Ship ship, DateTime currentTime, string description, Container container)
            : base(ship, currentTime, description)
        {
            Container = container;
        }

    }

    /// <summary>
    /// The EventArgs class for the ShipStartingUnloading event. This event is raised when a ship is done loading cargo from the harbor.
    /// </summary>
    public class ShipDoneLoadingEventArgs : BaseShipEventArgs
    {
        /// <summary>
        /// The unique ID of the dock the ship is located and loaded at.
        /// </summary>
        /// <returns>Guid object representing the ID of the dock the ship is docking to</returns>
        public Guid DockID { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the ShipDoneLoadingEventArgs class.
        /// </summary>
        /// <param name="ship">The ship object involved in the event.</param>
        /// <param name="currentTime">The current date and time in the simulation.</param>
        /// <param name="description">String value containing a description of the event.</param>
        /// <param name="dockID">The unique ID of the dock the ship is located and loaded at.</param>
        public ShipDoneLoadingEventArgs(Ship ship, DateTime currentTime, string description, Guid dockID)
            : base(ship, currentTime, description)
        {
            DockID = dockID;
        }
    }

    /// <summary>
    /// The EventArgs class for the ShipStartingUnloading event. This event is raised when a ship starts unloading its cargo to the harbor.
    /// </summary>
    public class ShipStartingUnloadingEventArgs : BaseShipEventArgs
    {
        /// <summary>
        /// The unique ID of the dock the ship is located and unloading at.
        /// </summary>
        /// <returns>Guid object representing the ID of the dock the ship is docking to</returns>
        public Guid DockID { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the ShipStartingUnloadingEventArgs class.
        /// </summary>
        /// <param name="ship">The ship object involved in the event.</param>
        /// <param name="currentTime">The current date and time in the simulation.</param>
        /// <param name="description">String value containing a description of the event.</param>
        /// <param name="dockID">The unique ID of the dock the ship is located and unloading at.</param>
        public ShipStartingUnloadingEventArgs(Ship ship, DateTime currentTime, string description, Guid dockID)
            : base(ship, currentTime, description)
        {
           DockID = dockID;
        }
    }
    
    /// <summary>
    /// The EventArgs class for the ShipUnloadedContainer event. This event is raised when a container is unloaded from a ships cargo.
    /// </summary>
    public class ShipUnloadedContainerEventArgs : BaseShipEventArgs
    {
        /// <summary>
        /// The container unloaded from the ship and on to the harbor.
        /// </summary>
        /// <returns>Container object representing the container unloaded from the ship and on to the harbor.</returns>
        public Container Container { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the ShipUndockingEventArgs class.
        /// </summary>
        /// <param name="ship">The ship object involved in the event.</param>
        /// <param name="currentTime">The current date and time in the simulation.</param>
        /// <param name="description">String value containing a description of the event.</param>
        /// <param name="container">The container unloaded from the ship and on the harbor.</param>
        public ShipUnloadedContainerEventArgs(Ship ship, DateTime currentTime, string description, Container container)
            : base(ship, currentTime, description)
        {
            Container = container;
        }

    }

    /// <summary>
    /// The EventArgs class for the ShipDoneUnloading event. This event is raised when a ship is done unloading its cargo to the harbor.
    /// </summary>
    public class ShipDoneUnloadingEventArgs : BaseShipEventArgs
    {
        /// <summary>
        /// The unique ID of the dock the ship is located and unloaded at.
        /// </summary>
        /// <returns>Guid object representing the ID of the dock the ship is docking to</returns>
        public Guid DockID { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the ShipDoneUnloadingEventArgs class.
        /// </summary>
        /// <param name="ship">The ship object involved in the event.</param>
        /// <param name="currentTime">The current date and time in the simulation.</param>
        /// <param name="description">String value containing a description of the event.</param>
        /// <param name="dockID">The unique ID of the dock the ship is located and unloaded at.</param>
        public ShipDoneUnloadingEventArgs(Ship ship, DateTime currentTime, string description, Guid dockID)
            : base(ship, currentTime, description)
        {
            DockID = dockID;
        }
    }
    
    /// <summary>
    /// The EventArgs class for the ShipAnchored event. This event is raised when a ship has anchored to the anchorage.
    /// </summary>
    public class ShipAnchoredEventArgs : BaseShipEventArgs
    {
        /// <summary>
        /// The unique ID of the anchorage.
        /// </summary>
        /// <returns>Guid object representing the ID of the anchorage</returns>
        public Guid AnchorageID { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the ShipUndockingEventArgs class.
        /// </summary>
        /// <param name="ship">The ship object involved in the event.</param>
        /// <param name="currentTime">The current date and time in the simulation.</param>
        /// <param name="description">String value containing a description of the event.</param>
        /// <param name="anchorageID">The unique ID of the anchorage.</param>
        public ShipAnchoredEventArgs(Ship ship, DateTime currentTime, string description, Guid anchorageID) 
            : base(ship, currentTime, description)
        {
            AnchorageID = anchorageID;
        }
    }

    /// <summary>
    /// The EventArgs class for the ShipAnchoring event. This event is raised when a ship has started the process of anchoring to the anchorage.
    /// </summary>
    public class ShipAnchoringEventArgs : BaseShipEventArgs
    {
        /// <summary>
        /// The unique ID of the anchorage.
        /// </summary>
        /// <returns>Guid object representing the ID of the anchorage</returns>
        public Guid AnchorageID { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the ShipUndockingEventArgs class.
        /// </summary>
        /// <param name="ship">The ship object involved in the event.</param>
        /// <param name="currentTime">The current date and time in the simulation.</param>
        /// <param name="description">String value containing a description of the event.</param>
        /// <param name="anchorageID">The unique ID of the anchorage.</param>
        public ShipAnchoringEventArgs(Ship ship, DateTime currentTime, string description, Guid anchorageID) 
            : base(ship, currentTime, description)
        {
            AnchorageID = anchorageID;
        }
    }

    /// <summary>
    /// The EventArgs class for the TruckLoadingFromStorage event. This event is raised when a truck is loading a container from the harbor's storage
    /// </summary>
    public class TruckLoadingFromHarborStorageEventArgs : EventArgs
    {
        /// <summary>
        /// The truck involved in the event.
        /// </summary>
        /// <returns>Returns a truck object that is involved in the event in the simulation.</returns>
        public Truck Truck { get; internal set; }

        /// <summary>
        /// The current time in the simulation.
        /// </summary>
        /// <returns>Returns a DateTime object representing the current date and time in the simulation.</returns>
        public DateTime CurrentTime { get; internal set; }
        /// <summary>
        /// A description of the event.
        /// </summary>
        /// <returns>Returns a string value containing a description of the event.</returns>
        public string Description { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the TruckLoadingFromStorageEvent.
        /// </summary>
        /// <param name="truck">The truck object involved in the event.</param>
        /// <param name="currentTime">The current date and time in the simulation.</param>
        /// <param name="description">String value containing a description of the event.</param>
        public TruckLoadingFromHarborStorageEventArgs(Truck truck, DateTime currentTime, string description)
        {
            Truck = truck;
            CurrentTime = currentTime;
            Description = description;
        }
    }
}
