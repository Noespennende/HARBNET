using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Xml.Linq;

namespace Gruppe8.HarbNet
{
    /// <summary>
    /// Class to run a simulation of a harbour.
    /// </summary>
    public class Simulation : ISimulation
    {
        /// <summary>
        /// Start time for simulation
        /// </summary>
        private DateTime startTime;

        /// <summary>
        /// Current time in simulation
        /// </summary>
        private DateTime currentTime;

        /// <summary>
        /// End time for simulation
        /// </summary>
        private DateTime endTime;

        /// <summary>
        /// Gets the harbor object
        /// </summary>
        private Harbor harbor;

        private int numberOfStorageContainersToShipThisRound;

        private int numberOfStorageContainersToTrucksThisRound;

        public EventHandler? SimulationEnded;
        public EventHandler? SimulationStarting;
        public EventHandler? OneHourHasPassed;
        public EventHandler? DayEnded;
        public EventHandler? DayLoggedToSimulationHistory;
        public EventHandler? ShipUndocking;
        public EventHandler? ShipInTransit;
        public EventHandler? ShipDockingToShipDock;
        public EventHandler? ShipDockedToShipDock;
        public EventHandler? ShipDockingtoLoadingDock;
        public EventHandler? ShipDockedtoLoadingDock;
        public EventHandler? ShipStartingLoading;
        public EventHandler? ShipLoadedContainer;
        public EventHandler? ShipDoneLoading;
        public EventHandler? ShipStartingUnloading;
        public EventHandler? ShipUnloadedContainer;
        public EventHandler? ShipDoneUnloading;
        public EventHandler? ShipAnchored;
        public EventHandler? ShipAnchoring;

        public EventHandler? TruckLoadingFromStorage;

        /// <summary>
        /// History for all ships and containers in the simulation in the form of Log objects. Each Log object stores information for one day in the simulation and contains information about the location and status of all ships and containers that day.
        /// </summary>
        /// <returns>returns a readOnlyCollection of log objects each representing one day of the simulation. Together the list represent the entire history of one simulation.</returns>
        public ReadOnlyCollection<DailyLog> History { get { return HistoryIList.AsReadOnly(); } }

        internal IList<DailyLog> HistoryIList { get; } = new List<DailyLog>();

        /// <summary>
        /// Simulation constructor.
        /// </summary>
        /// <param name="harbor">The harbor which will be used in the simulation</param>
        /// <param name="simulationStartTime">The start time of the simulation</param>
        /// <param name="simulationEndTime">The end time of simulation</param>
        public Simulation(Harbor harbor, DateTime simulationStartTime, DateTime simulationEndTime)
        {
            this.harbor = harbor;
            this.startTime = simulationStartTime;
            this.endTime = simulationEndTime;
        }

        /// <summary>
        /// Running the simulation
        /// </summary>
        /// <returns>returns the history of the simulation in the form of log objects where each object contains information about all ships and containers on one day of the simulation.</returns>
        public IList<DailyLog> Run()
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
            SimulationEndedEventArgs simulationEndedEventArgs = new SimulationEndedEventArgs(History, "The simulation has reached the end time and has ended.");

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
                

                if (ship.StartDate == currentTime)
                {

                    harbor.AddNewShipToAnchorage(ship);

                    if (ship.IsForASingleTrip == true && harbor.GetFreeLoadingDock(ship.ShipSize) != null)
                    {
                        Guid loadingDock = harbor.StartShipInLoadingDock(ship.ID);

                        ShipDockedToLoadingDockEventArgs shipDockedToLoadingDockEventArgs = new(ship, currentTime, "Ship has docked to loading dock.", loadingDock);

                        ship.AddStatusChangeToHistory(currentTime, loadingDock, Status.DockedToLoadingDock);

                        ShipDockedtoLoadingDock?.Invoke(this, shipDockedToLoadingDockEventArgs);

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
        /// <param name="dayLoggedEventArgs"></param>
        private void EndOf24HourPeriod()
        {
            DateTime past24Hours = currentTime.AddHours(-24);
            if (currentTime.Hour == 0)
            {
                DailyLog harborDayLog = new DailyLog(currentTime, harbor.Anchorage, harbor.GetShipsInTransit(), harbor.GetContainersStoredInHarbour(), harbor.ArrivedAtDestination,
                    harbor.GetShipsInLoadingDock(), harbor.GetShipsInShipDock());
                
                HistoryIList.Add(harborDayLog);

                DayOverEventArgs dayOverEventArgs = new(harborDayLog, currentTime, "The day has passed and the state of the harbor on day-shifty has been logged.");
                DayEnded?.Invoke(this, dayOverEventArgs);

                foreach (Container container in harbor.GetContainersStoredInHarbour())
                {
                    container.AddAnotherDayInStorage();
                }

                foreach (Ship ship in harbor.AllShips)
                {
                    List<StatusLog> dayReviewShipLogs = new();

                    foreach (StatusLog log in ship.HistoryIList)
                    {

                        if (log.PointInTime >= past24Hours && log.PointInTime <= currentTime)
                        {
                            dayReviewShipLogs.Add(log);
                        }
                    }
                    DayLoggedEventArgs dayLoggedEventArgs = new(harborDayLog, currentTime, 
                        "The day has passed and ship movement throughout the day and the state of the harbor on day-shift has been logged.", ship, dayReviewShipLogs);
                    
                    //dayLoggedEventArgs.message = $"\nDay over\n Current Time: ";
                    //shipAnchoringEventArgs.TodaysLog = 
                    
                    DayLoggedToSimulationHistory?.Invoke(this, dayLoggedEventArgs);
                }

            }

            currentTime = currentTime.AddHours(1);
        }


        /// <summary>
        /// Prints history for each ship in the harbor simulation to console.
        /// </summary>
        public void PrintShipHistory()
        {
            foreach (DailyLog log in History)
            {
                log.PrintInfoForAllShips();
            }
        }

        /// <summary>
        /// Prints the history of a given ship to console.
        /// <param name="shipToBePrinted">The ship who's history will be printed</param>
        /// </summary>
        public void PrintShipHistory(Ship shipToBePrinted)
        {
            shipToBePrinted.PrintHistory();
        }

        public void PrintShipHistory(Guid shipID)
        {


            foreach (Ship ship in harbor.AllShips)
            {
                if (ship.ID.Equals(shipID))
                {
                    ship.PrintHistory();
                    break;
                }

            
            }

            throw new ArgumentException("The ship you are trying to print does not exist in the Harbor the simulation is using.");

        }

        /// <summary>
        /// Printing each container in the simulations entire history to console.
        /// </summary>
        public void PrintContainerHistory()
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

            // Forandrer status fra Anchoring til Anchored bare

            Guid anchorageID = harbor.AnchorageID;
            List<Ship> Anchorage = harbor.Anchorage.ToList();

            foreach (Ship ship in Anchorage)
            {
                ShipAnchoredEventArgs shipAnchoredEventArgs = new(ship, currentTime, "Ship has anchored to anchorage.", harbor.AnchorageID);

                Guid shipID = ship.ID;
                //StatusLog lastStatusLog = ship.HistoryIList.Last();
                StatusLog lastStatusLog = GetStatusLog(ship);

                if (ShipIsAnchoring(ship, lastStatusLog))
                {
                    ship.HasBeenAlteredThisHour = true;

                    ship.AddStatusChangeToHistory(currentTime, anchorageID, Status.Anchored);

                    ShipAnchored?.Invoke(this, shipAnchoredEventArgs);

                }
            }
        }

        private bool ShipIsAnchoring(Ship ship, StatusLog lastStatusLog)
        {
            return ShipNotAlteredWithStatusLog(ship, lastStatusLog) && lastStatusLog.Status == Status.Anchoring;
        }

        private StatusLog GetStatusLog(Ship ship)
        {
            return ship.HistoryIList.Last();
        }

        private StatusLog? GetSecondLastStatusLog(Ship ship)
        {
            return ship.HistoryIList != null && ship.HistoryIList.Count >= 2
                    ? ship.HistoryIList[ship.HistoryIList.Count - 2]
                    : null;
        }

        /// <summary>
        /// Docking ships to the harbor, the shipStatus is set to docking
        /// </summary>
        private void DockingShips()
        {

            List<Ship> Anchorage = harbor.Anchorage.ToList();
            List<Ship> ShipsInShipDock = new(harbor.shipsInShipDock.Keys);
            List<Ship> ShipsInLoadingDock = new(harbor.shipsInLoadingDock.Keys);


            // Tror denne går ut (kan slettes?) Flytter skip FRA ShipDock til LoadingDock.
            // Siden skip skal begynne i LoadingDock i stedet, så skal et skip aldri ut av SkipDock.
            // Ingenting her inne skal bli trigget basically - 
            /*
            foreach (Ship ship in ShipsInShipDock)
            {
                Guid shipID = ship.ID;
                StatusLog lastStatusLog = ship.HistoryIList.Last();

                // Skipet har vært i ShipDock (DockedToShipDock), har begynt å docke til loading dock (DockingToLoadingDock), og har ikke 
                if (!ship.HasBeenAlteredThisHour && lastStatusLog != null &&
                    (lastStatusLog.Status == Status.Anchored ||
                    lastStatusLog.Status == Status.DockedToShipDock ||
                    lastStatusLog.Status == Status.DockingToLoadingDock ||
                    (lastStatusLog.Status == Status.UnloadingDone && (ship.IsForASingleTrip == true && ContainsTransitStatus(ship)))))
                {
                    
                    // Logikk for når skipet skal fra ShipDock til LoadingDock ved simulasjonsstart (currentTime == startTime)
                    if (currentTime == startTime && lastStatusLog.Status == Status.DockedToShipDock)
                    {

                        Guid dockID = harbor.DockShipFromShipDockToLoadingDock(ship.ID, currentTime);

                        ShipDockingToLoadingDockEventArgs shipDockingToLoadingDockEventArgs = new(ship, currentTime, "Ship is docking to loading dock.", dockID);

                        ship.AddStatusChangeToHistory(currentTime, dockID, Status.DockingToLoadingDock);

                        ShipDockingtoLoadingDock?.Invoke(this, shipDockingToLoadingDockEventArgs); ;
                    }
                }
            }
            */

            // Skipet har fått plass i LoadingDock, og er da enten "på vei" til å docke (status DockingTo..., eller har docket til LoadingDock (DockedTo..)
            foreach (Ship ship in ShipsInLoadingDock)
            {
                Guid shipID = ship.ID;
                StatusLog lastStatusLog = GetStatusLog(ship);

                // Skipet har fått plass "inne" i Harbor (ikke Anchorage) (DockingToLoadingDock) -> De skal unloade/loade
                if (ShipNotAlteredWithStatusLog(ship, lastStatusLog) &&
                    (lastStatusLog.Status == Status.DockingToLoadingDock ||
                    (lastStatusLog.Status == Status.UnloadingDone && SingleTripShipInTransit(ship))))
                {
                    // Skipet holder på med å docke ("in the process of docking" (Pga DockingToLoadingDock statusen), og har gjort det i minst 1 time)
                    if (ShipDockingForMoreThanOneHour(lastStatusLog))
                    {
                        Guid dockID = lastStatusLog.SubjectLocation;

                        ShipDockingToLoadingDock(ship, dockID);


                        // Skipet har vært i Transit og er ikke tomt -> Skal Unloade
                        if (ShipFromTransitWithContainersToUnload(ship))
                        {
                            StartUnloadProcess(ship, dockID);
                        }
                        // Skipet er tomt og er ikke enkeltseiling -> Skal Loade
                        else if (ship.ContainersOnBoard.Count == 0 && !ship.IsForASingleTrip)
                        {
                            StartLoadingProcess(ship, dockID);
                        }

                        // Ikke noe ship.HasBeenAlteredThisHour = true; fordi det er ingen ventetid mellom DockedToLoadingDock og Unloading/Loading nødvendig.
                        // Er med andre ord en rask status-forandring som skjer her.
                    }

                    // Skipet er et enkeltseilingskip og er tomt og det er ledig shipdock
                    else if (ship.IsForASingleTrip && ship.ContainersOnBoard.Count == 0
                        && FreeDockExists(ship))
                    {
                        // Det er ledig shipdock, ingen containere om bord, og siste status er ikke DockingToShipDock (unødig dobbel if?)
                        if (FreeDockExists(ship) && SingleTripShipInTransit(ship)
                            && ShipHasNoContainers(ship) && currentTime != startTime
                            && lastStatusLog.Status != Status.DockingToShipDock)
                        {
                            ShipNowDockingToShipDock(ship, shipID);
                        }
                    }

                    // Det er IKKE ledig shipDock, så enkeltskip går til Anchorage
                    else if (ship.IsForASingleTrip && !FreeDockExists(ship) && ship.TransitStatus == TransitStatus.Anchoring)
                    {
                        ShipAnchoringToAnchorage(ship);
                    }

                }
            }

            // Skipet ligger i Anchorage og docking skal begynne / er ønsket (Skip skal få status til "DockingTo...")
            foreach (Ship ship in Anchorage)
            {

                Guid shipID = ship.ID;
                StatusLog lastStatusLog = GetStatusLog(ship);

                if (ShipCanDock(ship, lastStatusLog))
                {

                    Guid dockID;

                    // Unngår at singleTripShip i Anchorage som måtte docke i Anchorage på starten av simuleringen får loadingDock før den har vært i Transit
                    // Dette fordi singleTripShip i Anchorage skal dra direkte i Transit og ikke måtte innom loadingDock


                    // Skip ønsker å begynne å docke til loading dock (trigges så videre i LoadingShips() overordnede metoden) 
                    //
                    // (ship.IsForASingleTrip && !ContainsTransitStatus(ship)) brukes fordi....:
                    // Unngår at enkeltseiling-skip i Anchorage som måtte docke i starten av simulering får loadingDock før den har vært i Transit.
                    if (NonEmptyShipReadyForLoadingDock(ship))
                    {

                        if (lastStatusLog.Status == Status.Anchored)
                        {
                            dockID = harbor.DockShipToLoadingDock(shipID, currentTime);
                            ShipDockingToLoadingDock(ship, dockID);
                        }
                    }
                    // Skipet er et enkeltseilingskip og det er ledig shipdock (VIKTIG: TransitStatus == TransitStatus.Anchoring)
                    // Hele denne kan være unødig, siden et skip ikke forlater ShipDock, og da skal det aldri bli en ledig ShipDock når et skip først på i Anchorage pga plassmangel.
                    // Kanskje grei å beholde i tilfelle rottefelle fremtidige forandringer?
                    else if (SingleTripShipAnchoring(ship))
                    {
                        // Det er ledig shipdock, ingen containere om bord, og siste status er ikke DockingToShipDock (unødig dobbel if-setning?)
                        
                        if (FreeDockExists(ship) && SingleTripShipInTransit(ship)
                            && ShipHasNoContainers(ship) && currentTime != startTime
                            && lastStatusLog.Status != Status.DockingToShipDock)
                        {
                            dockID = harbor.DockShipToShipDock(shipID);

                            ShipNowDockingToShipDock(ship, dockID);
                        }
                    }

                    // Skipet er singleTrip, det er ikke plass i ShipDock, den har vært i transit (ContainsTransitStatus) og siste status var Anchoring
                    // Kanskje unødig sjekk for "ikke plass i ShipDock", da skipet ikke skal få Anchoring status om det er plass
                    else if (ship.IsForASingleTrip && !(FreeDockExists(ship)) &&
                        ContainsTransitStatus(ship) && ship.ContainersOnBoard.Count == 0 && lastStatusLog.Status == Status.Anchoring)
                    {
                        ship.AddStatusChangeToHistory(currentTime, lastStatusLog.SubjectLocation, Status.Anchored);
                    }

                    ship.HasBeenAlteredThisHour = true;
                }
            }


        }

        private bool SingleTripShipAnchoring(Ship ship)
        {
            return ship.IsForASingleTrip && FreeDockExists(ship) && ship.TransitStatus == TransitStatus.Anchoring;
        }


        private bool NonEmptyShipReadyForLoadingDock(Ship ship)
        {
            return harbor.FreeLoadingDockExists(ship.ShipSize) && ship.ContainersOnBoard.Count != 0 && !(ship.IsForASingleTrip && !ContainsTransitStatus(ship));
        }

        private static bool ShipCanDock(Ship ship, StatusLog lastStatusLog)
        {
            return ShipNotAlteredWithStatusLog(ship, lastStatusLog) &&
                                (lastStatusLog.Status == Status.Anchored ||
                                lastStatusLog.Status == Status.DockedToShipDock ||
                                lastStatusLog.Status == Status.DockingToLoadingDock ||
                                lastStatusLog.Status == Status.DockingToShipDock ||
                                (lastStatusLog.Status == Status.UnloadingDone && SingleTripShipInTransit(ship)));
        }

        private void ShipAnchoringToAnchorage(Ship ship)
        {
            ShipAnchoringEventArgs shipAnchoringEventArgs = new(ship, currentTime, "Ship is anchoring to anchorage.", harbor.AnchorageID);

            ship.TransitStatus = TransitStatus.Anchoring;

            harbor.AddNewShipToAnchorage(ship);
            ship.AddStatusChangeToHistory(currentTime, harbor.AnchorageID, Status.Anchoring);
            ShipAnchoring?.Invoke(this, shipAnchoringEventArgs);

            ship.HasBeenAlteredThisHour = true;
        }

        private void ShipNowDockingToShipDock(Ship ship, Guid shipID)
        {
            Guid dockID = harbor.DockShipToShipDock(shipID);

            ship.AddStatusChangeToHistory(currentTime, dockID, Status.DockingToShipDock);

            ShipDockingToShipDockEventArgs shipDockingToShipDockEventArgs = new(ship, currentTime, "Ship is docking to ship dock.", dockID);
            ShipDockingToShipDock?.Invoke(this, shipDockingToShipDockEventArgs);

            ship.HasBeenAlteredThisHour = true;

        }

        private static bool ShipHasNoContainers(Ship ship)
        {
            return ship.ContainersOnBoard.Count == 0;
        }

        private bool FreeDockExists(Ship ship)
        {
            return harbor.FreeShipDockExists(ship.ShipSize);
        }

        private void StartLoadingProcess(Ship ship, Guid dockID)
        {
            ShipStartingLoadingEventArgs shipStartingLoadingEventArgs = new(ship, currentTime, "The ship is starting the loading process.", ship.CurrentLocation);

            ship.AddStatusChangeToHistory(currentTime, dockID, Status.Loading);

            ShipStartingLoading?.Invoke(this, shipStartingLoadingEventArgs);
        }

        private static bool SingleTripShipInTransit(Ship ship)
        {
            return (ship.IsForASingleTrip == true && ContainsTransitStatus(ship));
        }

        private void StartUnloadProcess(Ship ship, Guid dockID)
        {
            ShipStartingUnloadingEventArgs shipStartingUnloadingEventArgs = new(ship, currentTime, "The ship is starting the unloading process.", ship.CurrentLocation);

            double percentTrucks = harbor.PercentOfContainersDirectlyLoadedFromShips;
            ship.ContainersLeftForTrucks = ship.GetNumberOfContainersToTrucks(percentTrucks);

            ship.AddStatusChangeToHistory(currentTime, dockID, Status.Unloading);

            ShipStartingUnloading?.Invoke(this, shipStartingUnloadingEventArgs);
        }

        private bool ShipDockingForMoreThanOneHour(StatusLog lastStatusLog)
        {
            return lastStatusLog.Status == Status.DockingToLoadingDock && (currentTime - lastStatusLog.PointInTime).TotalHours >= 1;
        }

        private bool ShipFromTransitWithContainersToUnload(Ship ship)
        {
            return ContainsTransitStatus(ship) && ship.ContainersOnBoard.Count != 0;
        }

      

        private void ShipDockingToLoadingDock(Ship ship, Guid dockID)
        {
            ship.AddStatusChangeToHistory(currentTime, dockID, Status.DockedToLoadingDock);

            ShipDockedToLoadingDockEventArgs shipDockedToLoadingDockEventArgs = new(ship, currentTime, "Ship has docked to loading dock.", dockID);

            ShipDockedtoLoadingDock?.Invoke(this, shipDockedToLoadingDockEventArgs);
        }

        private static bool ShipNotAlteredWithStatusLog(Ship ship, StatusLog lastStatusLog)
        {
            return !ship.HasBeenAlteredThisHour && lastStatusLog != null;
        }


        /// <summary>
        /// unload containers from ship to harbor
        /// </summary>
        private void UnloadingShips()
        {

            foreach (Ship ship in harbor.DockedShipsInLoadingDock())
            {

                //StatusLog? lastStatusLog = ship.HistoryIList?.Last();
                StatusLog? lastStatusLog = GetStatusLog(ship);

                StatusLog? secondLastStatusLog = GetSecondLastStatusLog(ship);


                if (IsShipReadyForUnloading(ship,lastStatusLog,secondLastStatusLog))
                    /*(!ship.HasBeenAlteredThisHour && lastStatusLog != null && secondLastStatusLog != null &&
                    (lastStatusLog.Status == Status.Unloading || lastStatusLog.Status == Status.DockedToLoadingDock && secondLastStatusLog != null))*/
                {

                    Guid currentPosition = lastStatusLog.SubjectLocation;
                    Guid dockID = lastStatusLog.SubjectLocation;

                    // Unloading container
                    if (ShipNowUnloading(ship, lastStatusLog))  //ship.ContainersOnBoard.Count != 0 && lastStatusLog.Status == Status.DockedToLoadingDock || lastStatusLog.Status == Status.Unloading
                    {

                        if (ShipIsDockedToLoadingDock(ship, lastStatusLog)) //lastStatusLog.Status == Status.DockedToLoadingDock
                        {
                            StartUnloadProcess(ship, dockID);
                            
                        }

                        // ** Unloading **

                        UnloadShipForOneHour(ship);
                        UpdateShipStatus(ship, lastStatusLog, secondLastStatusLog);

                       

                        ship.HasBeenAlteredThisHour = true;
                    }

                }

            }
        }

        private bool IsShipReadyForUnloading(Ship ship, StatusLog lastStatusLog, StatusLog secondLastStatusLog)
        {
            return ShipNotAlteredWithStatusLog(ship, lastStatusLog) && secondLastStatusLog != null &&
                    (lastStatusLog.Status == Status.Unloading || lastStatusLog.Status == Status.DockedToLoadingDock && secondLastStatusLog != null);
        }

        private bool ShipNowUnloading(Ship ship, StatusLog lastStatusLog)
        {
            // Legg til ContainsTransitStatus(ship) hvis skipet *ikke* skal unloade ved simulasjon-start
            return ship.ContainersOnBoard.Count != 0 && (lastStatusLog.Status == Status.DockedToLoadingDock || lastStatusLog.Status == Status.Unloading);
        }

        private bool ShipIsDockedToLoadingDock(Ship ship, StatusLog lastStatusLog)
        {
            return lastStatusLog.Status == Status.DockedToLoadingDock;
        }

        private void UpdateShipStatus(Ship ship, StatusLog lastStatusLog, StatusLog secondLastStatusLog)
        {
            Guid currentLocation = lastStatusLog.SubjectLocation;

            // Denne tror jeg kan slettes
            if (secondLastStatusLog.Status == Status.DockedToShipDock)
            {
                ShipStartingUnloadingEventArgs shipStartingUnloadingEventArgs = new(ship, currentTime, "The ship is starting the unloading process.", ship.CurrentLocation);

                double percentTrucks = harbor.PercentOfContainersDirectlyLoadedFromShips;
                ship.ContainersLeftForTrucks = ship.GetNumberOfContainersToTrucks(percentTrucks);

                ship.AddStatusChangeToHistory(currentTime, currentLocation, Status.Unloading);

                ShipStartingUnloading?.Invoke(this, shipStartingUnloadingEventArgs);
            }

            // Skipet er ferdig med unloading, og *er ikke* enkeltseiling-skip som har vært på tur
            else if (ship.ContainersOnBoard.Count == 0 && !SingleTripShipInTransit(ship))
            {

                ShipDoneUnloadingEventArgs shipDoneUnloadingEventArgs = new(ship, currentTime, "The ship has finished unloading.", currentLocation);
                ship.AddStatusChangeToHistory(currentTime, currentLocation, Status.UnloadingDone);

                ShipDoneUnloading?.Invoke(this, shipDoneUnloadingEventArgs);

                ShipStartingLoadingEventArgs shipStartingLoadingEventArgs = new(ship, currentTime, "The ship is starting the loading process.", ship.CurrentLocation);
                ship.AddStatusChangeToHistory(currentTime, currentLocation, Status.Loading);

                ShipStartingLoading?.Invoke(this, shipStartingLoadingEventArgs);

                
            }

            // Skipet er ferdig med unloading og *er* enkeltseiling-skip som har vært på tur
            else if (ship.ContainersOnBoard.Count == 0 && SingleTripShipInTransit(ship))
            {
                ShipDoneUnloadingEventArgs shipDoneUnloadingEventArgs = new(ship, currentTime, "The ship has finished unloading.", currentLocation);
                ship.AddStatusChangeToHistory(currentTime, currentLocation, Status.UnloadingDone);

                ShipDoneUnloading?.Invoke(this, shipDoneUnloadingEventArgs);

                Guid dockID = harbor.DockShipToShipDock(ship.ID);
                if (dockID != Guid.Empty)
                { 
                    ship.AddStatusChangeToHistory(currentTime, currentLocation, Status.DockingToShipDock); 
                }

                // Skipet er enkeltseiling og ferdig unloadet, OG det er *ingen ledig shipDock* (Får derfor TransitStatus = TransitStatus.Anchoring)
                // -> Skipet må "parkere selv" i Anchorage.
                else
                {
                    ShipAnchoringEventArgs shipAnchoringEventArgs = new(ship, currentTime, "Ship is anchoring to anchorage.", harbor.AnchorageID);

                    harbor.AddNewShipToAnchorage(ship);
                    ship.TransitStatus = TransitStatus.Anchoring;
                    ship.AddStatusChangeToHistory(currentTime, currentLocation, Status.Anchoring);

                    ShipAnchoring?.Invoke(this, shipAnchoringEventArgs);

                }
            }
        }

        /// <summary>
        /// Simulates the unloading of one ship, for one hour.
        /// </summary>
        /// <param name="ship">The ship that is being unloaded</param>
        private void UnloadShipForOneHour(Ship ship)
        {
            double percentTrucks = harbor.PercentOfContainersDirectlyLoadedFromShips; 
            int numberOfContainersForTrucks = ship.GetNumberOfContainersToTrucks(percentTrucks);
            int numberOfContainersForStorage = ship.GetNumberOfContainersToStorage(percentTrucks);

            LoadingDock loadingDock = harbor.GetLoadingDockContainingShip(ship.ID);


            // Før: for (int i = 0; i < ship.ContainersLoadedPerHour && ship.ContainersOnBoard.Count > 0; i++)
            // Hver kran på LoadingDock gjør max antall avlast - simulerer da at hver kran laster av "samtidig", da hver kran gjør hvert sitt maksimum per time
            foreach (Crane crane in harbor.DockCranes)
            {
                if (crane.Container == null)
                {
                    // Regner ut maks mulige avlastinger basert på det minste tallet - kan ikke ADVer gjøre mer enn 10 i timen, men kran kan gjøre 20, så gjør vi aldri mer enn 10.
                    int maxLoadsPerHour = Math.Min(harbor.LoadsPerAdvPerHour, crane.ContainersLoadedPerHour);

                    // Gjør det til maks per time er nådd, eller skipet er tomt
                    for (int i = 0; i < maxLoadsPerHour && ship.ContainersOnBoard.Count > 0; i++)
                    {

                        Container? container = null;

                        if (numberOfContainersForStorage != 0 && (ship.ContainersOnBoard.Count - ship.ContainersLeftForTrucks) > 0) // ?? Lage advsInQueue også?
                        {
                            container = MoveContainerFromShipToAdv(ship, crane);
                            MoveContainerFromAdvToStorage(container);
                        }

                        if (numberOfContainersForTrucks != 0 && harbor.TrucksInQueue.Count != 0)
                        {
                            container = MoveContainerFromShipToTruck(ship, crane);
                            if (container != null)
                            {
                                harbor.SendTruckOnTransit(loadingDock, container);
                            }
                        }

                        if (container != null)
                        {
                            ShipUnloadedContainerEventArgs shipUnloadedContainerEventArgs = new(ship, currentTime, "Ship has unloaded one container.", container);

                            ShipUnloadedContainer?.Invoke(this, shipUnloadedContainerEventArgs);

                        }
                        
                    }
                }
            }
        }

        /// <summary>
        /// Moves one container from ship to an ADV.
        /// </summary>
        /// <param name="ship">The ship the container is being unloaded from.</param>
        /// <param name="crane">The crane at the dock that is being used for moving between ship and adv.</param>
        /// <returns>The container that has been unloaded off ship.</returns>
        private Container? MoveContainerFromShipToAdv(Ship ship, Crane crane)
        {
            Adv adv = harbor.GetFreeAdv();

            if (adv != null)
            {
                Container unloadedContainer = harbor.ShipToCrane(ship, crane, currentTime);
                harbor.CraneToAdv(crane, adv, currentTime);

                return unloadedContainer;

            }

            return null;

            // Event og status håndtering kommer //



            /*ShipUnloadedContainerEventArgs shipUnloadedContainerEventArgs = new();
            shipUnloadedContainerEventArgs.CurrentTime = CurrentTime;
            shipUnloadedContainerEventArgs.ship = ship;
            shipUnloadedContainerEventArgs.Container = containerToBeUnloaded;

            ShipUnloadedContainer?.Invoke(this, shipUnloadedContainerEventArgs); */
        }

        /// <summary>
        /// Moves one container from ADV to Storage area crane to storage space.
        /// </summary>
        /// <param name="container">The container that is being moved to storage</param>
        private void MoveContainerFromAdvToStorage(Container container)
        {
            Adv? adv = harbor.GetAdvContainingContainer(container);
            if (adv == null)
            {
                // EXCEPTION HÅNDTERING HVOR ?
                return;
            }

            Crane? crane = harbor.GetFreeStorageAreaCrane();
            if (crane == null)
            {
                // EXCEPTION HÅNDTERING HVOR ?
                return; 
            }

            harbor.AdvToCrane(crane, adv, currentTime);
            harbor.CraneToContainerRow(crane, currentTime);

            // Returnere plasseringen her kanskje (ContainerRow) eller container igjen?

            // Event og status håndtering kommer //

            /*ShipUnloadedContainerEventArgs shipUnloadedContainerEventArgs = new();
            shipUnloadedContainerEventArgs.CurrentTime = CurrentTime;
            shipUnloadedContainerEventArgs.ship = ship;
            shipUnloadedContainerEventArgs.Container = containerToBeUnloaded;

            ShipUnloadedContainer?.Invoke(this, shipUnloadedContainerEventArgs); */
        }

        /// <summary>
        /// Moves one container from ship to crane to truck.
        /// </summary>
        /// <param name="ship">The ship the container is unloaded from</param>
        /// <param name="crane">The crane at the dock that is used for moving between ship and truck</param>
        private Container? MoveContainerFromShipToTruck(Ship ship, Crane crane)
        {

            LoadingDock loadingDock = harbor.GetLoadingDockContainingShip(ship.ID);

            Truck? truck = harbor.GetFreeTruck();
            harbor.RemoveTruckFromQueue(truck);
            loadingDock.AssignTruckToTruckLoadingSpot(truck);

            if (truck == null)
            {
                // EXCEPTION HÅNDTERING HVOR ?
                return null;
            }
            if (crane == null)
            {
                // EXCEPTION HÅNDTERING HVOR ?
                return null;
            }
            
            Container container = harbor.ShipToCrane(ship, crane, currentTime);
            harbor.CraneToTruck(crane, truck, currentTime);
            ship.ContainersLeftForTrucks--;

            return container;
            //return truck;

            // Event og status håndtering kommer //

            /*ShipUnloadedContainerEventArgs shipUnloadedContainerEventArgs = new();
            shipUnloadedContainerEventArgs.CurrentTime = CurrentTime;
            shipUnloadedContainerEventArgs.ship = ship;
            shipUnloadedContainerEventArgs.Container = containerToBeUnloaded;

            ShipUnloadedContainer?.Invoke(this, shipUnloadedContainerEventArgs);*/
        }

        /// <summary>
        /// undock ship from harbor, and set status to Transit
        /// </summary>
        private void UndockingShips()
        {
            foreach (Ship ship in harbor.DockedShipsInLoadingDock())
            {

                Guid shipID = ship.ID;
                //StatusLog lastStatusLog = ship.HistoryIList.Last();
                StatusLog lastStatusLog = GetStatusLog(ship);

                /*if (ship.HasBeenAlteredThisHour == false && lastStatusLog != null &&
                    (lastStatusLog.Status == Status.Undocking || lastStatusLog.Status == Status.LoadingDone || lastStatusLog.Status == Status.DockedToLoadingDock ||
                    (lastStatusLog.Status == Status.UnloadingDone && ContainsTransitStatus(ship)) || lastStatusLog.Status == Status.DockingToShipDock))*/


                if (ShipCanUndockFromDock(ship, lastStatusLog)) // ship.HasBeenAlteredThisHour == false && lastStatusLog != null &&(lastStatusLog.Status == Status.Undocking || lastStatusLog.Status == Status.LoadingDone || lastStatusLog.Status == Status.DockedToLoadingDock(lastStatusLog.Status == Status.UnloadingDone && ContainsTransitStatus(ship)) || lastStatusLog.Status == Status.DockingToShipDock);
                {
                    bool containsTransitStatus = ContainsTransitStatus(ship);

                    if (SingleTripShipCanDockToShipDock(ship, lastStatusLog)) //ship.IsForASingleTrip == true && containsTransitStatus && lastStatusLog.Status != Status.DockingToShipDock;
                    {
                        Guid dockID = harbor.DockShipToShipDock(ship.ID);
                        ship.AddStatusChangeToHistory(currentTime, dockID, Status.DockingToShipDock);

                    }

                    else if (ship.IsForASingleTrip && !containsTransitStatus && lastStatusLog.Status == Status.DockedToLoadingDock)
                    {
                        ship.AddStatusChangeToHistory(currentTime, harbor.TransitLocationID, Status.Undocking);
                        ship.TransitStatus = TransitStatus.Leaving;
                    }

                    else if (ShipIsDockingToDock(ship, lastStatusLog)) //lastStatusLog.Status == Status.DockingToShipDock && (CurrentTime - lastStatusLog.PointInTime).TotalHours >= 1
                    {
                        Guid dockID = lastStatusLog.SubjectLocation;
                        ship.AddStatusChangeToHistory(currentTime, dockID, Status.DockedToShipDock);
                    }

                    else if (ShipIsFinishedLoadingContainers(ship, lastStatusLog)) //lastStatusLog.Status == Status.LoadingDone || lastStatusLog.Status == Status.DockedToLoadingDock
                    {
                        ship.AddStatusChangeToHistory(currentTime, harbor.TransitLocationID, Status.Undocking);
                        ship.TransitStatus = TransitStatus.Leaving;
                    }

                    else if (ShipIsUndocking(ship, lastStatusLog)) //lastStatusLog.Status == Status.Undocking && (CurrentTime - lastStatusLog.PointInTime).TotalHours >= 1
                    {

                        harbor.UnDockShipFromLoadingDockToTransit(shipID, currentTime);
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
                //StatusLog lastStatusLog = ship.HistoryIList.Last();
                StatusLog lastStatusLog = GetStatusLog(ship);


                /*if (ship.HasBeenAlteredThisHour == false && lastStatusLog != null &&
                    (lastStatusLog.Status == Status.Undocking || lastStatusLog.Status == Status.LoadingDone || lastStatusLog.Status == Status.DockedToLoadingDock ||
                    (lastStatusLog.Status == Status.UnloadingDone && ContainsTransitStatus(ship)) || lastStatusLog.Status == Status.DockingToShipDock))*/


                if (ShipCanUndockFromDock(ship, lastStatusLog)) // ship.HasBeenAlteredThisHour == false && lastStatusLog != null &&(lastStatusLog.Status == Status.Undocking || lastStatusLog.Status == Status.LoadingDone || lastStatusLog.Status == Status.DockedToLoadingDock(lastStatusLog.Status == Status.UnloadingDone && ContainsTransitStatus(ship)) || lastStatusLog.Status == Status.DockingToShipDock);
                {
                    bool containsTransitStatus = ContainsTransitStatus(ship);

                    if (ShipIsDockingToDock(ship, lastStatusLog)) //lastStatusLog.Status == Status.DockingToShipDock && (CurrentTime - lastStatusLog.PointInTime).TotalHours >= 1
                    {
                        Guid dockID = lastStatusLog.SubjectLocation;
                        ship.AddStatusChangeToHistory(currentTime, dockID, Status.DockedToShipDock);
                    }
                    ship.HasBeenAlteredThisHour = true;

                }
            }

            Guid anchorageID = harbor.AnchorageID;
            List<Ship> Anchorage = harbor.Anchorage.ToList();

            foreach (Ship ship in Anchorage)
            {
                StatusLog lastStatusLog = GetStatusLog(ship);

                // Gjelder enkeltseiling-skip som måtte docke til Anchorage ved simulering-start (fordi: !ContainsTransitStatus)
                if (ship.IsForASingleTrip && !ContainsTransitStatus(ship))
                {
                    if (lastStatusLog.Status == Status.Anchored)
                    { 
                        Guid currentLocation = lastStatusLog.SubjectLocation;

                        ShipUndockingEventArgs shipUndockingEventArgs = new(ship, currentTime, "Ship is undocking from Anchorage.", currentLocation);

                        ship.AddStatusChangeToHistory(currentTime, harbor.TransitLocationID, Status.Undocking);
                        ship.TransitStatus = TransitStatus.Leaving;

                        ShipUndocking?.Invoke(this, shipUndockingEventArgs);
                    }

                    else if (ShipIsUndocking(ship, lastStatusLog))
                    {
                        Guid oldLocation = harbor.UnDockShipFromAnchorageToTransit(ship.ID);

                        ShipInTransitEventArgs shipInTransitEventArgs = new(ship, currentTime, "Ship as left the harbor and is in transit.", harbor.TransitLocationID);

                        ship.AddStatusChangeToHistory(currentTime, harbor.TransitLocationID, Status.Transit);

                        ShipInTransit?.Invoke(this, shipInTransitEventArgs);
                    }

                    ship.HasBeenAlteredThisHour = true;
                }

               
            }
        }

        private bool ShipCanUndockFromDock(Ship ship, StatusLog lastStatusLog)
        {
            bool containsTransitStatus = ContainsTransitStatus(ship);

            return ship.HasBeenAlteredThisHour == false && lastStatusLog != null &&
                    (lastStatusLog.Status == Status.Undocking || lastStatusLog.Status == Status.LoadingDone || 
                    (lastStatusLog.Status == Status.DockedToLoadingDock && ship.IsForASingleTrip && !ContainsTransitStatus(ship)) ||
                    (lastStatusLog.Status == Status.UnloadingDone && ContainsTransitStatus(ship)) || lastStatusLog.Status == Status.DockingToShipDock);
        }
        private bool SingleTripShipCanDockToShipDock(Ship ship, StatusLog lastStatusLog)
        {
            bool containsTransitStatus = ContainsTransitStatus(ship);
            return ship.IsForASingleTrip == true && containsTransitStatus && lastStatusLog.Status != Status.DockingToShipDock && FreeDockExists(ship) != null;
        }
        private bool ShipIsDockingToDock(Ship ship, StatusLog lastStatusLog)
        {
            return lastStatusLog.Status == Status.DockingToShipDock && (currentTime - lastStatusLog.PointInTime).TotalHours >= 1;
        }
        private bool ShipIsFinishedLoadingContainers(Ship ship, StatusLog lastStatusLog)
        {
            return lastStatusLog.Status == Status.LoadingDone || lastStatusLog.Status == Status.DockedToLoadingDock;
        }
        private bool ShipIsUndocking(Ship ship, StatusLog lastStatusLog)
        {
            return lastStatusLog.Status == Status.Undocking && (currentTime - lastStatusLog.PointInTime).TotalHours >= 1;
        }

        /// <summary>
        /// check to see if the ship has the Status "Transit".
        /// </summary>
        /// <param name="ship"> The ship to be checked</param>
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
        /// Loading containers onboard ships
        /// </summary>
        private void LoadingShips()
        {
            foreach (Ship ship in harbor.DockedShipsInLoadingDock())
            {

                StatusLog lastStatusLog = ship.HistoryIList.Last();
                StatusLog secondLastStatusLog = GetSecondLastStatusLog(ship);

                // Dette kan simplifiseres og refaktoriseres ned (f.eks singleTripShip går igjen her..)
                bool shipIsNotSingleTripAndIsDoneUnloading = (lastStatusLog.Status == Status.UnloadingDone && (ship.IsForASingleTrip != true));
                bool shipIsSingleTripAndHasLastUnloadedAndHasNotBeenOnTrip = (lastStatusLog.Status == Status.UnloadingDone && (ship.IsForASingleTrip == true && !ContainsTransitStatus(ship)));
                bool LastLocationWasDockedToShipDock = (lastStatusLog.Status == Status.DockedToLoadingDock && secondLastStatusLog != null && secondLastStatusLog.Status == Status.DockedToShipDock);
                bool shipIsSingleTripAndHasNotBeenOnTrip = (ship.IsForASingleTrip == true && !ContainsTransitStatus(ship));

                if (ShipNotAlteredWithStatusLog(ship, lastStatusLog) &&
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
                            ShipDoneLoadingEventArgs shipDoneLoadingEventArgs = new(ship, currentTime, "The ship is done loading.", currentLocation);
                            ship.AddStatusChangeToHistory(currentTime, currentLocation, Status.LoadingDone);

                            ShipDoneLoading?.Invoke(this, shipDoneLoadingEventArgs);

                            ship.AddStatusChangeToHistory(currentTime, currentLocation, Status.Undocking);
                            ship.TransitStatus = TransitStatus.Leaving;

                            ShipUndockingEventArgs shipUndockingEventArgs = new(ship, currentTime, "Ship is undocking from dock.", currentLocation);
                            ShipUndocking?.Invoke(this, shipUndockingEventArgs);
                        }

                    }
                    else
                    {
                        ShipDoneLoadingEventArgs shipDoneLoadingEventArgs = new(ship, currentTime, "The ship is done loading.", currentLocation);
                        ship.AddStatusChangeToHistory(currentTime, currentLocation, Status.LoadingDone);

                        ShipDoneLoading?.Invoke(this, shipDoneLoadingEventArgs);

                        ship.AddStatusChangeToHistory(currentTime, harbor.TransitLocationID, Status.Undocking);
                        ship.TransitStatus = TransitStatus.Leaving;

                        ShipUndockingEventArgs shipUndockingEventArgs = new(ship, currentTime, "Ship is undocking from dock.", currentLocation);
                        ShipUndocking?.Invoke(this, shipUndockingEventArgs);

                    }

                    ship.HasBeenAlteredThisHour = true;

                }
            }
        }

        private void LoadShipForOneHour(Ship ship, Guid currentPosition)
        {

            numberOfStorageContainersToShipThisRound = harbor.NumberOfContainersInStorageToShips();

            Crane? testCrane = harbor.GetFreeLoadingDockCrane();

            // Regner ut maks mulige avlastinger basert på det minste tallet - kan ikke ADVer gjøre mer enn 10 i timen, men kran kan gjøre 20, så gjør vi aldri mer enn 10.
            int maxLoadsPerHour = Math.Min(harbor.LoadsPerAdvPerHour, testCrane.ContainersLoadedPerHour);
            int numberOfRepeats;

            if (maxLoadsPerHour == harbor.LoadsPerAdvPerHour)
            {
                
                numberOfRepeats = maxLoadsPerHour * harbor.AdvFree.Count;
            }
            else
            {
                numberOfRepeats = maxLoadsPerHour * Math.Min(harbor.DockCranes.Count, harbor.HarborStorageAreaCranes.Count);
            }

            for (int i = 0; i < numberOfStorageContainersToShipThisRound && i < numberOfRepeats; i++)
            {
                // Gjør det til maks per time er nådd, eller nådd antall containere i storage for ship
                Container? loadedContainer = LoadContainerOnShip(ship);

                // Event and Status handling
                if (loadedContainer == null)
                {
                    ShipDoneLoadingEventArgs shipDoneLoadingEventArgs = new(ship, currentTime, "The ship is done loading.", currentPosition);
                    ship.AddStatusChangeToHistory(currentTime, currentPosition, Status.LoadingDone);

                    ShipDoneLoading?.Invoke(this, shipDoneLoadingEventArgs);

                    ship.AddStatusChangeToHistory(currentTime, currentPosition, Status.Undocking);
                    ship.TransitStatus = TransitStatus.Leaving;

                    ShipUndockingEventArgs shipUndockingEventArgs = new(ship, currentTime, "Ship is undocking from dock.", currentPosition);
                    ShipUndocking?.Invoke(this, shipUndockingEventArgs);
                    //break;
                }
                else
                {
                    // Legger eventet her siden det blir dobbelt opp i LoadContainerOnStorageAdv ..?
                    ShipLoadedContainerEventArgs shipLoadedContainerEventArgs = new(ship, currentTime, "Ship has loaded one container.", loadedContainer);
                    ShipLoadedContainer?.Invoke(this, shipLoadedContainerEventArgs);
                }
            }
        }

        internal Container? LoadContainerOnShip(Ship ship)
        {
            numberOfStorageContainersToShipThisRound = harbor.NumberOfContainersInStorageToShips();

            Container? loadedContainer = null;
            Container? movedContainer = null;

            Crane? storageCrane = harbor.GetFreeStorageAreaCrane();
            Crane? dockCrane = harbor.GetFreeLoadingDockCrane();

            if (storageCrane.Container == null && dockCrane.Container == null)
            {
                Console.WriteLine($"containers to ship: {numberOfStorageContainersToShipThisRound} - in storage: {harbor.storedContainers.Count} - for truck: {harbor.NumberOfContainersInStorageToTrucks()}");

                loadedContainer = LoadContainerOnStorageAdv(ship);

                movedContainer = MoveOneContainerFromAdvToShip(loadedContainer, ship);



                return movedContainer;

            }

            return null;
        }

        /// <summary>
        /// Loads one container from Adv to Ship
        /// </summary>
        /// <param name="ship">The ship that is loading the container onboard.</param>
        /// <returns></returns>
        internal Container? LoadContainerOnStorageAdv(Ship ship)
        {

            Container? containerToBeLoaded;

            bool underMaxWeight;
            bool underMaxCapacity;

            if (ship.ContainersOnBoard.Count == 0 || ship.ContainersOnBoard.Last().Size == ContainerSize.Full && harbor.GetStoredContainer(ContainerSize.Half) != null
                || harbor.GetStoredContainer(ContainerSize.Full) == null && harbor.GetStoredContainer(ContainerSize.Half) != null)
            {
                underMaxWeight = ship.MaxWeightInTonn >= ship.CurrentWeightInTonn + (int)ContainerSize.Half;
                underMaxCapacity = ship.ContainerCapacity > ship.ContainersOnBoard.Count + 1;

                if (underMaxCapacity && underMaxWeight)
                {
                    containerToBeLoaded = MoveOneContainerFromContainerRowToAdv(ContainerSize.Half);

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
                    containerToBeLoaded = MoveOneContainerFromContainerRowToAdv(ContainerSize.Full);

                    return containerToBeLoaded;
                }
            }

            return null;
        }

        internal Container? LoadContainerFromStorageAdvToShip(Container container, Ship ship)
        {

            Container? loadedContainer;

            // For hver DockCrane, så kan cont
            foreach (Crane crane in harbor.DockCranes)
            {
                if (crane.Container == null)
                {
                    // Regner ut maks mulige avlastinger basert på det minste tallet - kan ikke ADVer gjøre mer enn 10 i timen, men kran kan gjøre 20, så gjør vi aldri mer enn 10.
                    int maxLoadsPerHour = Math.Min(harbor.LoadsPerAdvPerHour, crane.ContainersLoadedPerHour);

                    // Gjør det til maks per time er nådd, eller skipet er tomt
                    for (int i = 0; i < maxLoadsPerHour && ship.ContainersOnBoard.Count > 0; i++)
                    {
                        loadedContainer = MoveOneContainerFromAdvToShip(container, ship);
                        return loadedContainer;
                    }
                }
            }

            return null;
        }

        private Container? MoveOneContainerFromContainerRowToAdv(ContainerSize containerSize)
        {
            Adv adv = harbor.GetFreeAdv();
            Crane? storageCrane = harbor.GetFreeStorageAreaCrane();
            if (storageCrane == null)
            {
                // EXCEPTION HÅNDTERING HVOR ?
                return null;
            }

            Container? container = harbor.ContainerRowToCrane(containerSize, storageCrane, currentTime);

            if (container == null || adv == null)
            {
                return null;
            }
            harbor.CraneToAdv(storageCrane, adv, currentTime);

            return container;

        }

        private Container? MoveOneContainerFromAdvToShip(Container? container, Ship ship)
        {
            Adv? adv = null;

            if (container != null) 
            { 
                adv = harbor.GetAdvContainingContainer(container);
            }

            Crane? loadingDockCrane = harbor.GetFreeLoadingDockCrane();

            if (adv == null || loadingDockCrane == null)
            {
                return null;
            }
            
            harbor.AdvToCrane(loadingDockCrane, adv, currentTime);
            harbor.CraneToShip(loadingDockCrane, ship, currentTime);
            return container;

        }

        /// <summary>
        /// Returns ships in transit to harbor.
        /// </summary>
        private void InTransitShips()
        {
            foreach (Ship ship in harbor.ShipsInTransit.Keys)
            {
                //StatusLog lastStatusLog = ship.HistoryIList.Last();
                StatusLog lastStatusLog = GetStatusLog(ship);

                if (IsShipInTransit(ship, lastStatusLog))
                {

                    Guid CurrentPosition = GetStatusLog(ship).SubjectLocation;
                    StatusLog LastHistoryStatusLog = ship.HistoryIList.Last();

                    

                    if (ShipHasReturnedToHarbor(ship, lastStatusLog)) //DaysSinceTransitStart >= ship.RoundTripInDays & double DaysSinceTransitStart = (currentTime - LastHistoryStatusLog.PointInTime).TotalDays;
                    {
                        harbor.RestockContainers(ship, currentTime);
                        harbor.AddNewShipToAnchorage(ship);
                        ship.AddStatusChangeToHistory(currentTime, CurrentPosition, Status.Anchoring);
                        ship.TransitStatus = TransitStatus.Arriving;

                        ShipAnchoringEventArgs shipAnchoringEventArgs = new(ship, currentTime, "Ship is anchoring to anchorage.", harbor.AnchorageID);
                        
                        ShipAnchoring?.Invoke(this, shipAnchoringEventArgs);
                    }

                    ship.HasBeenAlteredThisHour = true;

                }
            }
        }

        /// <summary>
        /// Checking ship status to see if it's in transit
        /// </summary>
        /// <param name="ship">ship object</param>
        /// <param name="lastStatusLog">StatusLog object</param>
        /// <returns>Boolean</returns>
        private bool IsShipInTransit(Ship ship, StatusLog lastStatusLog)
        {
            return ship.HasBeenAlteredThisHour == false && lastStatusLog != null && lastStatusLog.Status == Status.Transit;
        }
        /// <summary>
        /// Checks if ship has returned to harbor
        /// </summary>
        /// <param name="ship">ship object</param>
        /// <param name="LastHistoryStatusLog">Statuslog object</param>
        /// <returns>Boolean</returns>
        private bool ShipHasReturnedToHarbor(Ship ship, StatusLog LastHistoryStatusLog)
        {
            double DaysSinceTransitStart = (currentTime - LastHistoryStatusLog.PointInTime).TotalDays;
            return DaysSinceTransitStart >= ship.RoundTripInDays;


        }



        private void LoadingTrucksFromStorage()
        {
            int numberOfContainersToTrucks = CalculateNumberOfStorageContainersToTrucks();

            Container? container = null;
            int containersToTruck = 0;

            Console.WriteLine($"I Storage: {harbor.storedContainers.Count} - Antall containere fra storage til trucks - {numberOfContainersToTrucks}");

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
                            break; // Avslutt løkken hvis det ikke er flere containere igjen
                        }

                        if (harbor.TrucksInQueue.Count > 0)
                        {
                          
                            Container? loadedContainer = MoveOneContainerFromContainerRowToTruck(container);
                            Truck? truck = null;

                            if (loadedContainer != null)
                            {
                                containersToTruck++;
                                truck = harbor.SendTruckOnTransit(container);

                                TruckLoadingFromStorageEventArgs truckLoadingFromStorageEventArgs = new(truck, currentTime, "One truck has loaded a container and has left");
                                TruckLoadingFromStorage?.Invoke(this, truckLoadingFromStorageEventArgs);

                            }
                            
                        }

                        if (containersToTruck == maxLoadsPerHour)
                        {
                            break;
                        }
                    }

                    break;
                }

                if (numberOfContainersToTrucks <= containersToTruck)
                {
                    break; // Avslutt hovedløkken hvis vi har lastet nok containere
                }
            }
        }

        private void ContainersOnTrucksArrivingToDestination()
        {
            foreach(Truck truck in harbor.TrucksInTransit)
            {
                
                Container? container = truck.Container;
                if(container != null)
                {
                    StatusLog? lastLogContainer = container.HistoryIList.Last();

                    if ((currentTime - lastLogContainer.PointInTime).TotalHours >= 1)
                    {
                        Container arrivedContainer = truck.UnloadContainer();
                        harbor.ArrivedAtDestination.Add(arrivedContainer);

                        truck.Location = harbor.DestinationID;

                        arrivedContainer.CurrentPosition = harbor.DestinationID;
                        arrivedContainer.AddStatusChangeToHistory(Status.ArrivedAtDestination, currentTime);
                    }
                }
            }

            Console.WriteLine($"ArrivedToDestination: {harbor.ArrivedAtDestination.Count}");
        }


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

        private Container? MoveOneContainerFromContainerRowToTruck(Container container)
        {
            Crane? storageCrane = harbor.GetFreeStorageAreaCrane();
            if (storageCrane == null)
            {
                // EXCEPTION HÅNDTERING HVOR ?
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
        /// Returns a string that contains information about all ships in the previous simulation.
        /// </summary>
        /// <returns> a string that contains information about all ships in the previous simulation. Returns empty string if no simulation has been run.</returns>
        public String HistoryToString()
        {
            if (History.Count > 0)
            {
                StringBuilder sb = new StringBuilder();

                foreach (DailyLog log in History)
                {
                    sb.Append(log.HistoryToString());
                }
                return sb.ToString();
            }
            else { return ""; }
        }

        public string HistoryToString(Guid shipID)
        {

            foreach (Ship ship in harbor.AllShips)
            {
                if (ship.ID.Equals(shipID))
                {
                    return ship.HistoryToString();
                }


            }

            throw new ArgumentException("The ship you are trying to get the history from does not exist in the Harbor object the simulation is using.");
        }

        /// <summary>
        /// Returns a string containing information about the history of all ships or all containers in the simulation.
        /// </summary>
        /// <param name="ShipsOrContainers">Sending in the value "ships" returns information on all ships, sending in "containers" return information on all containers</param>
        /// <returns>Returns a String containing information about all ships or containers of the simulation. Returns an empty string if wrong value is given in param or no simulation has been ran.</returns>
        public String HistoryToString(String ShipsOrContainers)
        {
            if (ShipsOrContainers.ToLower().Equals("ships") || ShipsOrContainers.ToLower().Equals("ship"))
            {
                return HistoryToString();
            }
            else if (ShipsOrContainers.ToLower().Equals("containers") || ShipsOrContainers.ToLower().Equals("container"))
            {
                if (History.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();

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
        /// Returns a string that represents the information about one ship in the simulation.
        /// </summary>
        /// <param name="ship">The ship you want information on</param>
        /// <returns>Returns a String containing information about the given ship in the simulation</returns>
        public String HistoryToString(Ship ship)
        {
            return ship.HistoryToString();
        }


        /// <summary>
        /// Returns a string that contains information about the start time, end time of the simulation and the ID of the harbour used.
        /// </summary>
        /// <returns> a string that contains information about the start time, end time of the simulation and the ID of the harbour used.</returns>
        public override string ToString()
        {
            return ($"Simulation start time: {startTime.ToString()}, end time: {endTime.ToString()}, harbor ID: {harbor.ID}");
        }

    
    }

    /// <summary>
    /// The EventArgs class for the SimulationStarting event.
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
        /// <param name="harborToBeSimulated">The harbor that is being simulated.</param>
        /// <param name="startDate"> The time the simulation will start from.</param>
        /// <param name="description">A description of the event.</param>
        public SimulationStartingEventArgs(Harbor harborToBeSimulated, DateTime startDate, string description)
        {
            HarborToBeSimulated = harborToBeSimulated;
            StartDate = startDate;
            Description = description;
        }
    }

    public class OneHourHasPassedEventArgs : EventArgs
    {
        /// <summary>
        /// The time in the simulation the event was raised.
        /// </summary>
        public DateTime CurrentTime { get; internal set; }

        /// <summary>
        /// A description of the event.
        /// </summary>
        public string Description { get; internal set; }

        public OneHourHasPassedEventArgs(DateTime currentTime, string description)
        {
            CurrentTime = currentTime;
            Description = description;
        }
    }

    /// <summary>
    /// The EventArgs class for the SimulationEnded event.
    /// </summary>
    public class SimulationEndedEventArgs : EventArgs
    {
        /// <summary>
        /// A collection of DailyLog objects that together represent the history of the simulation.
        /// </summary>
        /// <returns>ReadOnlyCollection of DailyLog objects. Each one contains information about a single day of the simulation.</returns>
        public ReadOnlyCollection<DailyLog> SimulationHistory { get; internal set; }

        /// <summary>
        /// A description of the event.
        /// </summary>
        /// <returns>String describing the event.</returns>
        public string Description { get; internal set; }

        /// <summary>
        ///  Initializes a new instance of the SimulationEndedEventArgs class.
        /// </summary>
        /// <param name="simulationHistory">A collection of DailyLog objects that together represent the history of the simulation.</param>
        /// <param name="description">A description of the event.</param>
        public SimulationEndedEventArgs(ReadOnlyCollection<DailyLog> simulationHistory, string description)
        {
            SimulationHistory = simulationHistory;
            Description = description;
        }
    }
    
    /// <summary>
    /// The EventArgs class for the DayOver event.
    /// </summary>
    public class DayOverEventArgs : EventArgs
    {
        /// <summary>
        /// A DailyLog object containing information about the previous day in the simulation.
        /// </summary>
        /// <returns>DailyLog object containing information about the state of the simulation at the time the object was created</returns>
        public DailyLog TodaysLog { get; internal set; }
        
        /// <summary>
        /// The time in the simulation the event was raised.
        /// </summary>
        /// <returns>DateTime object representing the time in the simulation the event was raised</returns>
        public DateTime CurrentTime { get; internal set; }

        /// <summary>
        /// A description of the event.
        /// </summary>
        /// <returns>String describing the event.</returns>
        public string Description { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the DayOverEventArgs class.
        /// </summary>
        /// <param name="todaysLog">A DailyLog object containing information about the previous day in the simulation.</param>
        /// <param name="currentTime">The time in the simulation the event was raised.</param>
        /// <param name="description">A description of the event.</param>
        public DayOverEventArgs(DailyLog todaysLog, DateTime currentTime, string description)
        {
            TodaysLog = todaysLog;
            CurrentTime = currentTime;
            Description = description;
        }
    }
   
    /// <summary>
    /// The EventArgs class for the DayLogged event
    /// </summary>
    public class DayLoggedEventArgs : EventArgs
    {
        /// <summary>
        /// A DailyLog object containing information about the state of the harbor the day the event was raised.
        /// </summary>
        /// <returns>DailyLog containing information about the state of the harbor the day the event was raised.</returns>
        public DailyLog TodaysLog { get; internal set; }
        
        /// <summary>
        /// The time in the simulation the event was raised.
        /// </summary>
        /// <returns>DateTime object representing the time in the simulation the event was raised</returns>
        public DateTime CurrentTime { get; internal set; }

        /// <summary>
        /// A description of the event.
        /// </summary>
        /// <returns>String describing the event.</returns>
        public string Description { get; internal set; }

        /// <summary>
        /// The ship the dayReviewShipLogs logs come from.
        /// </summary>
        /// <returns>Ship object representing the ship that logged the DayReview logs.</returns>
        public Ship Ship { get; internal set; }
        
        /// <summary>
        /// A list of all logs registered by ship in the past day.
        /// </summary>
        /// <returns>List with all logs registered by ship in the past day.</returns>
        public IList<StatusLog>? DayReviewShipLogs { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="todaysLog">A DailyLog object containing information about the state of the harbor the day the event was raised.</param>
        /// <param name="currentTime">The time in the simulation the event was raised.</param>
        /// <param name="description">A description of the event.</param>
        /// <param name="ship">The ship the dayReviewShipLogs logs come from.</param>
        /// <param name="dayReviewShipLogs">A list of all logs registered by ship in the past day.</param>
        public DayLoggedEventArgs(DailyLog todaysLog, DateTime currentTime, string description, Ship ship, IList<StatusLog>? dayReviewShipLogs)
        {
            TodaysLog = todaysLog;
            CurrentTime = currentTime;
            Description = description;
            Ship = ship;
            DayReviewShipLogs = dayReviewShipLogs;
        }
    }

    public class BaseShipEventArgs : EventArgs
    {
        /// <summary>
        /// The ship involved in the event.
        /// </summary>
        public Ship Ship { get; internal set; }

        /// <summary>
        /// The current time in the simulation.
        /// </summary>
        public DateTime CurrentTime { get; internal set; }
        /// <summary>
        /// A description of the event.
        /// </summary>
        /// <returns>String describing the event.</returns>
        public string Description { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the BaseShipEventArgs class.
        /// </summary>
        /// <param name="ship">The ship involved in the event.</param>
        /// <param name="currentTime">The current time in the simulation.</param>
        /// <param name="description">A description of the event.</param>
        public BaseShipEventArgs(Ship ship, DateTime currentTime, string description)
        {
            Ship = ship;
            CurrentTime = currentTime;
            Description = description;
        }
    }
    /// <summary>
    /// The EventArgs class for the ShipUndocked event.
    /// </summary>
    public class ShipUndockingEventArgs : BaseShipEventArgs
    {
        /// <summary>
        /// The unique ID of the location the ship undocked from, Anchorage or Dock.
        /// </summary>
        /// <returns>Guid object representing the ID of the location the ship undocked from</returns>
        public Guid LocationID { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the ShipUndockingEventArgs class.
        /// </summary>
        /// <param name="ship">The ship involved in the event.</param>
        /// <param name="currentTime">The current time in the simulation.</param>
        /// <param name="locationID">The unique ID of the location the ship undocked from.</param>
        /// <param name="description">A description of the event.</param>
        public ShipUndockingEventArgs(Ship ship, DateTime currentTime, string description, Guid locationID)
            : base(ship, currentTime, description)
        {
            LocationID = locationID;
        }
    }

    /// <summary>
    /// The EventArgs class for the ShipInTransit event.
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
        /// <param name="ship">The ship involved in the event.</param>
        /// <param name="currentTime">The current time in the simulation.</param>
        /// <param name="description">A description of the event.</param>
        /// <param name="transitLocationID">The unique ID of the transit location the ship is located at.</param>
        public ShipInTransitEventArgs(Ship ship, DateTime currentTime, string description, Guid transitLocationID) 
            : base(ship, currentTime, description)
        {
            this.TransitLocationID = transitLocationID;
        }
    }

    /// <summary>
    /// The EventArgs class for the ShipDockingToShipDock event.
    /// </summary>
    public class ShipDockingToShipDockEventArgs : BaseShipEventArgs
    {
        /// <summary>
        /// The unique ID of the dock the ship is docking to.
        /// </summary>
        /// <returns>Guid object representing the ID of the dock the ship is docking to</returns>
        public Guid DockID { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the ShipUndockingEventArgs class.
        /// </summary>
        /// <param name="ship">The ship involved in the event.</param>
        /// <param name="currentTime">The current time in the simulation.</param>
        /// <param name="description">A description of the event.</param>
        /// <param name="dockID">The unique ID of the odck the ship is docking to.</param>
        public ShipDockingToShipDockEventArgs(Ship ship, DateTime currentTime, string description, Guid dockID)
            : base(ship, currentTime, description)
        {
            DockID = dockID;
        }
    }

    /// <summary>
    /// The EventArgs class for the shipDockedToShipDock event.
    /// </summary>
    public class ShipDockedToShipDockEventArgs : BaseShipEventArgs
    {
        /// <summary>
        /// The unique ID of the dock the ship docked to.
        /// </summary>
        /// <returns>Guid object representing the ID of the dock the ship docked to</returns>
        public Guid DockID { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the ShipUndockingEventArgs class.
        /// </summary>
        /// <param name="ship">The ship involved in the event.</param>
        /// <param name="currentTime">The current time in the simulation.</param>
        /// <param name="description">A description of the event.</param>
        /// <param name="dockID">The unique ID of the dock the ship docked to.</param>
        public ShipDockedToShipDockEventArgs(Ship ship, DateTime currentTime, string description, Guid dockID) 
            : base(ship, currentTime, description)
        {
            DockID = dockID;
        }

    }

    /// <summary>
    /// The EventArgs class for the shipDockingToLoadingDock event.
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
        /// <param name="ship">The ship involved in the event.</param>
        /// <param name="currentTime">The current time in the simulation.</param>
        /// <param name="description">A description of the event.</param>
        /// <param name="dockID">The unique ID of the dock the ship is docking to.</param>
        public ShipDockingToLoadingDockEventArgs(Ship ship, DateTime currentTime, string description, Guid dockID) 
            : base(ship, currentTime, description)
        {
            DockID = dockID;
        }
    }
   
    /// <summary>
    /// The EventArgs class for the shipDockedToLoadingDock event.
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
        /// <param name="ship">The ship involved in the event.</param>
        /// <param name="currentTime">The current time in the simulation.</param>
        /// <param name="description">A description of the event.</param>
        /// <param name="dockID">The unique ID of the dock the ship docked to.</param>
        public ShipDockedToLoadingDockEventArgs(Ship ship, DateTime currentTime, string description, Guid dockID) 
            : base(ship, currentTime, description)
        {
            DockID = dockID;
        }

    }

    public class ShipStartingLoadingEventArgs : BaseShipEventArgs
    {
        /// <summary>
        /// The unique ID of the dock the ship is located at and loading from.
        /// </summary>
        public Guid DockID { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the ShipStartingUnloadingEventArgs class.
        /// </summary>
        /// <param name="ship">The ship involved in the event.</param>
        /// <param name="currentTime">The current time in the simulation.</param>
        /// <param name="description">A description of the event.</param>
        /// <param name="dockID">The unique ID of the dock the ship is located at and loading from.</param>
        public ShipStartingLoadingEventArgs(Ship ship, DateTime currentTime, string description, Guid dockID)
            : base(ship, currentTime, description)
        {
            DockID = dockID;
        }
    }

    /// <summary>
    /// The EventArgs class for the ShipLoadedContainer event.
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
        /// <param name="ship">The ship involved in the event.</param>
        /// <param name="currentTime">The current time in the simulation.</param>
        /// <param name="description">A description of the event.</param>
        /// <param name="container">The container loaded onboard the ship.</param>
        public ShipLoadedContainerEventArgs(Ship ship, DateTime currentTime, string description, Container container)
            : base(ship, currentTime, description)
        {
            Container = container;
        }

    }

    /// <summary>
    /// The EventArgs class for the ShipStartingUnloading event.
    /// </summary>
    public class ShipDoneLoadingEventArgs : BaseShipEventArgs
    {
        /// <summary>
        /// The unique ID of the dock the ship is located and loaded at.
        /// </summary>
        public Guid DockID { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the ShipDoneLoadingEventArgs class.
        /// </summary>
        /// <param name="ship">The ship involved in the event.</param>
        /// <param name="currentTime">The current time in the simulation.</param>
        /// <param name="description">A description of the event.</param>
        /// <param name="dockID">The unique ID of the dock the ship is located and loaded at.</param>
        public ShipDoneLoadingEventArgs(Ship ship, DateTime currentTime, string description, Guid dockID)
            : base(ship, currentTime, description)
        {
            DockID = dockID;
        }
    }

    /// <summary>
    /// The EventArgs class for the ShipStartingUnloading event.
    /// </summary>
    public class ShipStartingUnloadingEventArgs : BaseShipEventArgs
    {
        /// <summary>
        /// The unique ID of the dock the ship is located and unloading at.
        /// </summary>
        public Guid DockID { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the ShipStartingUnloadingEventArgs class.
        /// </summary>
        /// <param name="ship">The ship involved in the event.</param>
        /// <param name="currentTime">The current time in the simulation.</param>
        /// <param name="description">A description of the event.</param>
        /// <param name="dockID">The unique ID of the dock the ship is located and unloading at.</param>
        public ShipStartingUnloadingEventArgs(Ship ship, DateTime currentTime, string description, Guid dockID)
            : base(ship, currentTime, description)
        {
           DockID = dockID;
        }
    }
    
    /// <summary>
    /// The EventArgs class for the ShipUnloadedContainer event.
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
        /// <param name="ship">The ship involved in the event.</param>
        /// <param name="currentTime">The current time in the simulation.</param>
        /// <param name="description">A description of the event.</param>
        /// <param name="container">The container unloaded from the ship and on the harbor.</param>
        public ShipUnloadedContainerEventArgs(Ship ship, DateTime currentTime, string description, Container container)
            : base(ship, currentTime, description)
        {
            Container = container;
        }

    }

    /// <summary>
    /// The EventArgs class for the ShipDoneUnloading event.
    /// </summary>
    public class ShipDoneUnloadingEventArgs : BaseShipEventArgs
    {
        /// <summary>
        /// The unique ID of the dock the ship is located and unloaded at.
        /// </summary>
        public Guid DockID { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the ShipDoneUnloadingEventArgs class.
        /// </summary>
        /// <param name="ship">The ship involved in the event.</param>
        /// <param name="currentTime">The current time in the simulation.</param>
        /// <param name="description">A description of the event.</param>
        /// <param name="dockID">The unique ID of the dock the ship is located and unloaded at.</param>
        public ShipDoneUnloadingEventArgs(Ship ship, DateTime currentTime, string description, Guid dockID)
            : base(ship, currentTime, description)
        {
            DockID = dockID;
        }
    }
    
    /// <summary>
    /// The EventArgs class for the ShipAnchored event.
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
        /// <param name="ship">The ship involved in the event.</param>
        /// <param name="currentTime">The current time in the simulation.</param>
        /// <param name="description">A description of the event.</param>
        /// <param name="anchorageID">The unique ID of the anchorage.</param>
        public ShipAnchoredEventArgs(Ship ship, DateTime currentTime, string description, Guid anchorageID) 
            : base(ship, currentTime, description)
        {
            AnchorageID = anchorageID;
        }
    }

    /// <summary>
    /// The EventArgs class for the ShipAnchoring event.
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
        /// <param name="ship">The ship involved in the event.</param>
        /// <param name="currentTime">The current time in the simulation.</param>
        /// <param name="description">A description of the event.</param>
        /// <param name="anchorageID">The unique ID of the anchorage.</param>
        public ShipAnchoringEventArgs(Ship ship, DateTime currentTime, string description, Guid anchorageID) 
            : base(ship, currentTime, description)
        {
            AnchorageID = anchorageID;
        }
    }

    public class TruckLoadingFromStorageEventArgs : EventArgs
    {
        public Truck Truck { get; internal set; }

        public DateTime CurrentTime { get; internal set; }
        public string Description { get; internal set; }

        public TruckLoadingFromStorageEventArgs(Truck truck, DateTime currentTime, string description)
        {
            Truck = truck;
            CurrentTime = currentTime;
            Description = description;
        }
    }
}

