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

        public EventHandler? SimulationEnded;
        public EventHandler? SimulationStarting;
        public EventHandler? DayEnded;
        public EventHandler? DayLoggedToSimulationHistory;
        public EventHandler? ShipUndocking;
        public EventHandler? ShipInTransit;
        public EventHandler? ShipDockingToShipDock;
        public EventHandler? ShipDockedToShipDock;
        public EventHandler? ShipDockingtoLoadingDock;
        public EventHandler? ShipDockedtoLoadingDock;
        public EventHandler? ShipLoadedContainer;
        public EventHandler? ShipUnloadedContainer;
        public EventHandler? ShipAnchored;
        public EventHandler? ShipAnchoring;

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
            
            
            SimulationStartingEventArgs simulationStartingEventArgs = new(harbor, currentTime, "The simulation has started");
            
            SimulationStarting?.Invoke(this, simulationStartingEventArgs);

            HistoryIList.Add(new DailyLog(currentTime, harbor.Anchorage, harbor.GetShipsInTransit(), harbor.GetContainersStoredInHarbour(),
                        harbor.GetShipsInLoadingDock(), harbor.GetShipsInShipDock()));


            this.currentTime = startTime;

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

                InTransitShips();

                EndOf24HourPeriod();

                continue;
            }
            SimulationEndedEventArgs simulationEndedEventArgs = new SimulationEndedEventArgs(History, "The simulation is has reached the end time and ha ended.");

            SimulationEnded?.Invoke(this, simulationEndedEventArgs);
            Thread.Sleep(1000);

            return History;

        }

        /// <summary>
        /// Sets start location and initiates first status log for all ships, based on their startTime.
        /// </summary>
        private void SetStartLocationForAllShips()
        {
            foreach (Ship ship in harbor.AllShips)
            {
                ShipAnchoringEventArgs shipAnchoringEventArgs = new(ship, currentTime, "Ship is anchoring to anchorage.", harbor.AnchorageID);

                if (ship.StartDate == currentTime)
                {

                    harbor.AddNewShipToAnchorage(ship);

                    if (ship.IsForASingleTrip == true && harbor.GetFreeLoadingDock(ship.ShipSize) != null)
                    {
                        Guid loadingDock = harbor.StartShipInLoadingDock(ship.ID);
                        
                        ship.AddStatusChangeToHistory(currentTime, loadingDock, Status.DockedToLoadingDock);
                    }
                    else
                    {
                        ship.AddStatusChangeToHistory(currentTime, harbor.AnchorageID, Status.Anchoring);
                        ship.AddStatusChangeToHistory(currentTime, harbor.AnchorageID, Status.Anchored);
                        ShipAnchoring?.Invoke(this, shipAnchoringEventArgs);
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
                DailyLog harborDayLog = new DailyLog(currentTime, harbor.Anchorage, harbor.GetShipsInTransit(), harbor.GetContainersStoredInHarbour(),
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

            throw new ShipNotFoundExeption("The ship you are trying to print does not exist in the Harbor the simulation is using.");

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
            return !ship.HasBeenAlteredThisHour && lastStatusLog != null && lastStatusLog.Status == Status.Anchoring;
        }

        /*
        private void DockingShips()
        {
            DockShipsInShipDock();
            DockShipsInLoadingDock();
            
        }

        private void DockShipsInShipDock()
        {
            foreach(Ship ship in harbor.shipsInShipDock.Keys)
            {
                if (ShipCanDockInShipDock(ship)){
                    ShipNowDockingToShipDock(ship);
                }
            }
        }

        private void ShipNowDockingToShipDock(Ship ship)
        {
            StatusLog lastStatusLog = GetStatusLog(ship);
            Guid dockID;

            if (CanDocktoLoadingDock(ship, lastStatusLog))
            {
                if (lastStatusLog.Status == Status.Anchored)
                {
                    DocktoLoadingDock(ship);
                }

                if (CanDockToShipDock(ship, lastStatusLog))
                {
                    DockToShipDock(ship);

                }

            }

            ship.HasBeenAlteredThisHour = true;
        }

        private bool CanDockToShipDock(Ship ship, StatusLog lastStatusLog)
        {
            return harbor.FreeShipDockExists(ship.ShipSize) && ship.IsForASingleTrip == true && ContainsTransitStatus(ship)
                    && ship.ContainersOnBoard.Count == 0 && currentTime != startTime
                    && lastStatusLog.Status != Status.DockingToShipDock;
        }

        private void DockToShipDock(Ship ship)
        {
            Guid dockID = harbor.DockShipToShipDock(ship.ID);
            ship.AddStatusChangeToHistory(currentTime, dockID, Status.DockingToShipDock);

            ShipDockingToShipDockEventArgs shipDockingToShipDock = new(ship, currentTime, "Ship is docking to ship dock.", dockID);
            ShipDockingToShipDock?.Invoke(this, shipDockingToShipDock); 
        }

        private void DocktoLoadingDock(Ship ship)
        {
            Guid dockID = harbor.DockShipToLoadingDock(ship.ID, currentTime);
            ship.AddStatusChangeToHistory(currentTime, dockID, Status.DockingToLoadingDock);

            ShipDockingToLoadingDockEventArgs shipDockingToLoadingDockEventArgs = new(ship, currentTime, "Ship is docking to loading dock.", dockID);
            ShipDockingtoLoadingDock?.Invoke(this, shipDockingToLoadingDockEventArgs);
        }

       

        private bool CanDocktoLoadingDock(Ship ship, StatusLog lastStatusLog)
        {
            return harbor.FreeLoadingDockExists(ship.ShipSize) && lastStatusLog.Status != Status.DockedToShipDock;
        }

        private bool ShipCanDockInShipDock(Ship ship)
        {
            StatusLog lastStatusLog = ship.HistoryIList.Last();

            return !ship.HasBeenAlteredThisHour && lastStatusLog != null &&
           (lastStatusLog.Status == Status.Anchored ||
            lastStatusLog.Status == Status.DockedToShipDock ||
            lastStatusLog.Status == Status.DockingToLoadingDock ||
            (lastStatusLog.Status == Status.UnloadingDone && (ship.IsForASingleTrip == true && ContainsTransitStatus(ship))));
        }

        private void DockShipsInLoadingDock()
        {
            foreach (Ship ship in harbor.shipsInShipDock.Keys)
            {
                if (ShipCanDockToLoadingDock(ship))
                {
                    ShipNowDockingToLoadingDock(ship);
                }
            }
        }

        private bool ShipCanDockToLoadingDock(Ship ship)
        {
            StatusLog lastStatusLog = ship.HistoryIList.Last();

            return !ship.HasBeenAlteredThisHour && lastStatusLog != null &&
                   (lastStatusLog.Status == Status.DockedToShipDock ||
                    lastStatusLog.Status == Status.DockingToLoadingDock ||
                    (lastStatusLog.Status == Status.UnloadingDone && (ship.IsForASingleTrip == true && ContainsTransitStatus(ship))));
        }
        
        // Dette er vel når skipet har docket? Docked. Og starter loading. (Status settes til Status.DockedToLoadingDock)
        private void ShipNowDockingToLoadingDock(Ship ship)
        {
            Guid shipID = ship.ID;
            StatusLog lastStatusLog = ship.HistoryIList.Last();

            if (lastStatusLog.Status == Status.DockingToLoadingDock && (currentTime - lastStatusLog.PointInTime).TotalHours >= 1)
            {

                Guid dockID = lastStatusLog.SubjectLocation;
                ship.AddStatusChangeToHistory(currentTime, dockID, Status.DockedToLoadingDock);

                ShipDockedToLoadingDockEventArgs shipDockedToLoadingDockEventArgs = new(ship, currentTime, "Ship has docked to loading dock.", dockID);
                ShipDockingtoLoadingDock?.Invoke(this, shipDockedToLoadingDockEventArgs);

                if (CanStartLoading(ship))
                {
                    StartLoading(ship, dockID);
                }
                else
                {
                    StartUnloading(ship, dockID );
                }
            }

        }

        private bool CanStartLoading(Ship ship)
        {
            return ship.IsForASingleTrip && !ContainsTransitStatus(ship);
        }

        private void StartLoading(Ship ship, Guid dockID)
        {
            ship.AddStatusChangeToHistory(currentTime, dockID, Status.Loading);
        }

        private void StartUnloading(Ship ship, Guid dockID)
        {
            ship.AddStatusChangeToHistory(currentTime, dockID, Status.Unloading);
        }


        private void DockShipsInAnchorage()
        {
            foreach (Ship ship in harbor.shipsInShipDock.Keys)
            {
                if (ShipCanDockToAnchorage(ship))
                {
                    ShipNowDockingToAnchorage();
                }
            }
        }

        private bool ShipCanDockToAnchorage(Ship ship)
        {
            StatusLog lastStatusLog = ship.HistoryIList.LastOrDefault();

            return !ship.HasBeenAlteredThisHour && lastStatusLog != null &&
                   (lastStatusLog.Status == Status.Anchored ||
                    lastStatusLog.Status == Status.DockedToShipDock ||
                    lastStatusLog.Status == Status.DockingToLoadingDock ||
                    lastStatusLog.Status == Status.DockingToShipDock ||
                    (lastStatusLog.Status == Status.UnloadingDone && (ship.IsForASingleTrip == true && ContainsTransitStatus(ship))));
        }


        private void ShipNowDockingToAnchorage()
        {
            throw new NotImplementedException();
        }
        */

        private StatusLog GetStatusLog(Ship ship)
        {
            return ship.HistoryIList.Last();
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
                StatusLog lastStatusLog = ship.HistoryIList.Last();

                // Skipet har fått plass "inne" i Harbor (ikke Anchorage) (DockingToLoadingDock) -> De skal unloade/loade
                if (!ship.HasBeenAlteredThisHour && lastStatusLog != null &&
                    (lastStatusLog.Status == Status.DockingToLoadingDock ||
                    (lastStatusLog.Status == Status.UnloadingDone && (ship.IsForASingleTrip == true && ContainsTransitStatus(ship)))))
                {
                    // Skipet holder på med å docke ("in the process of docking" (Pga DockingToLoadingDock statusen), og har gjort det i minst 1 time)
                    if (lastStatusLog.Status == Status.DockingToLoadingDock && (currentTime - lastStatusLog.PointInTime).TotalHours >= 1)
                    {
                        Guid dockID = lastStatusLog.SubjectLocation;

                        ship.AddStatusChangeToHistory(currentTime, dockID, Status.DockedToLoadingDock);

                        ShipDockedToLoadingDockEventArgs shipDockedToLoadingDockEventArgs = new(ship, currentTime, "Ship has docked to loading dock.", dockID);
                        
                        ShipDockedtoLoadingDock?.Invoke(this, shipDockedToLoadingDockEventArgs);

                        // Hvis skipet er et enkeltseilingskip og har ikke "InTransit" historie (har aldri vært ute på tur)
                        
                        // Skipet har vært i Transit og er ikke tomt
                        if (ContainsTransitStatus(ship) && ship.ContainersOnBoard.Count != 0)
                        {
                            ship.AddStatusChangeToHistory(currentTime, dockID, Status.Unloading);
                        }
                        // Skipet er tomt og er ikke enkeltseiling 
                        else if (ship.ContainersOnBoard.Count == 0 && !ship.IsForASingleTrip)
                        {
                            ship.AddStatusChangeToHistory(currentTime, dockID, Status.Loading);
                        }

                        // Anchoring av enkeltskip i ShipsInLoadingDock blir flyttet til anchorage via AnchorageShips()
                    }

                    // Skipet er et enkeltseilingskip og er tomt og det er ledig shipdock
                    else if (ship.IsForASingleTrip && ship.ContainersOnBoard.Count == 0 
                        && harbor.FreeShipDockExists(ship.ShipSize))
                    {
                        // Det er ledig shipdock, ingen containere om bord, og siste status er ikke DockingToShipDock (unødig dobbel if?)
                        if (harbor.FreeShipDockExists(ship.ShipSize) && ship.IsForASingleTrip == true && ContainsTransitStatus(ship)
                            && ship.ContainersOnBoard.Count == 0 && currentTime != startTime
                            && lastStatusLog.Status != Status.DockingToShipDock)
                        {
                            Guid dockID = harbor.DockShipToShipDock(shipID);

                            ship.AddStatusChangeToHistory(currentTime, dockID, Status.DockingToShipDock);

                            ShipDockingToShipDockEventArgs shipDockingToShipDockEventArgs = new(ship, currentTime, "Ship is docking to ship dock.", dockID);
                            ShipDockingToShipDock?.Invoke(this, shipDockingToShipDockEventArgs);
                        }
                    }

                    // Det er IKKE ledig shipDock, så enkeltskip går til Anchorage
                    else if (ship.IsForASingleTrip && !harbor.FreeShipDockExists(ship.ShipSize) && ship.TransitStatus == TransitStatus.Anchoring)
                    {
                        ShipAnchoringEventArgs shipAnchoringEventArgs = new(ship, currentTime, "Ship is anchoring to anchorage.", harbor.AnchorageID);

                        ship.TransitStatus = TransitStatus.Anchoring;

                        harbor.AddNewShipToAnchorage(ship);
                        ship.AddStatusChangeToHistory(currentTime, harbor.AnchorageID, Status.Anchoring);
                    }

                }
            }

            // Skipet ligger i Anchorage og docking skal begynne / er ønsket (Skip skal få status til "DockingTo...")
            foreach (Ship ship in Anchorage)
            {
                
                Guid shipID = ship.ID;
                StatusLog lastStatusLog = ship.HistoryIList.Last();

                
                if (!ship.HasBeenAlteredThisHour && lastStatusLog != null &&
                    (lastStatusLog.Status == Status.Anchored ||
                    lastStatusLog.Status == Status.DockedToShipDock ||
                    lastStatusLog.Status == Status.DockingToLoadingDock ||
                    lastStatusLog.Status == Status.DockingToShipDock ||
                    (lastStatusLog.Status == Status.UnloadingDone && (ship.IsForASingleTrip == true && ContainsTransitStatus(ship)))))
                {

                    Guid dockID;

                    // Skip ønsker å begynne å docke til loading dock (trigges så videre i LoadingShips() overordnede metoden) 
                    if (harbor.FreeLoadingDockExists(ship.ShipSize) && ship.ContainersOnBoard.Count != 0)
                    {

                        if (lastStatusLog.Status == Status.Anchored)
                        {
                            dockID = harbor.DockShipToLoadingDock(shipID, currentTime);

                            ShipDockingToLoadingDockEventArgs ShipDockingToLoadingDockEventArgs = new(ship, currentTime, "Ship is docking to loading dock.", dockID);
                            ship.AddStatusChangeToHistory(currentTime, dockID, Status.DockingToLoadingDock);
                            ShipDockingtoLoadingDock?.Invoke(this, ShipDockingToLoadingDockEventArgs);
                        }
                    }
                    // Skipet er et enkeltseilingskip og det er ledig shipdock (VIKTIG: TransitStatus == TransitStatus.Anchoring)
                    else if (ship.IsForASingleTrip && harbor.FreeShipDockExists(ship.ShipSize))
                    {
                        // Det er ledig shipdock, ingen containere om bord, og siste status er ikke DockingToShipDock (unødig dobbel if?)
                        if (harbor.FreeShipDockExists(ship.ShipSize) && ship.IsForASingleTrip == true && ContainsTransitStatus(ship)
                            && ship.ContainersOnBoard.Count == 0 && currentTime != startTime
                            && lastStatusLog.Status != Status.DockingToShipDock)
                        {
                            dockID = harbor.DockShipToShipDock(shipID);

                            ship.AddStatusChangeToHistory(currentTime, dockID, Status.DockingToShipDock);

                            ShipDockingToShipDockEventArgs shipDockingToShipDockEventArgs = new(ship, currentTime, "Ship is docking to ship dock.", dockID);
                            ShipDockingToShipDock?.Invoke(this, shipDockingToShipDockEventArgs);
                        }
                    }

                    // Skipet er singleTrip, det er ikke plass i ShipDock, den har vært i transit (TransitStatus eksisterer) og siste status var Anchoring
                    // Kanskje unødig sjekk for "ikke plass i ShipDock", da skipet ikke skal få Anchoring status om det er plass
                    else if (ship.IsForASingleTrip && !(harbor.FreeShipDockExists(ship.ShipSize)) && 
                        ContainsTransitStatus(ship) && ship.ContainersOnBoard.Count == 0 && lastStatusLog.Status == Status.Anchoring)
                    {
                        ship.AddStatusChangeToHistory(currentTime, lastStatusLog.SubjectLocation, Status.Anchored);
                    }

                    ship.HasBeenAlteredThisHour = true;
                }
            }


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

                StatusLog? secondLastStatusLog = ship.HistoryIList?[ship.HistoryIList.Count - 2];


                if (IsShipReadyForUnloading(ship,lastStatusLog,secondLastStatusLog))
                    /*(!ship.HasBeenAlteredThisHour && lastStatusLog != null && secondLastStatusLog != null &&
                    (lastStatusLog.Status == Status.Unloading || lastStatusLog.Status == Status.DockedToLoadingDock && secondLastStatusLog != null))*/
                {

                    Guid currentPosition = lastStatusLog.SubjectLocation;

                    // Unloading container
                    if (ShipNowUnloading(ship, lastStatusLog))  //ship.ContainersOnBoard.Count != 0 && lastStatusLog.Status == Status.DockedToLoadingDock || lastStatusLog.Status == Status.Unloading
                    {
                        if (ShipIsDockedToLoadingDock(ship, lastStatusLog)) //lastStatusLog.Status == Status.DockedToLoadingDock
                        {
                            ship.AddStatusChangeToHistory(currentTime, currentPosition, Status.Unloading);

                            ship.ContainersLeftForTrucks = ship.GetNumberOfContainersToTrucks();
                        }

                        // ** Unloading **

                        UnloadShipForOneHour(ship);
                        UpdateShipStatus(ship, lastStatusLog, secondLastStatusLog);

                        // Skipet er enkeltseiling og ferdig unloadet, OG det er ingen ledig shipDock
                        // -> Skipet må "parkere selv" utenfor harbor
                        if (ship.IsForASingleTrip && ship.ContainersOnBoard.Count == 0 && harbor.GetFreeShipDock == null)
                        {
                            harbor.AddNewShipToAnchorage(ship);
                            ship.TransitStatus = TransitStatus.Anchoring;
                            ship.AddStatusChangeToHistory(currentTime, harbor.AnchorageID, Status.Anchoring);

                        }
                        
                    }

                    // Status oppdateringer
                    
                    
                    /*
                    if (secondLastStatusLog.Status == Status.DockedToShipDock)
                    {
                        ship.AddStatusChangeToHistory(currentTime, currentPosition, Status.Loading);
                    }

                    else if (ship.ContainersOnBoard.Count == 0 && !(ship.IsForASingleTrip == true && ContainsTransitStatus(ship)))
                    {
                        ship.AddStatusChangeToHistory(currentTime, currentPosition, Status.UnloadingDone);

                        ship.AddStatusChangeToHistory(currentTime, currentPosition, Status.Loading);
                    }
                    else if (ship.ContainersOnBoard.Count == 0 && (ship.IsForASingleTrip == true && ContainsTransitStatus(ship)))
                    {
                        ship.AddStatusChangeToHistory(currentTime, currentPosition, Status.UnloadingDone);

                        ship.AddStatusChangeToHistory(currentTime, currentPosition, Status.DockingToShipDock);
                    }
                    */

                    ship.HasBeenAlteredThisHour = true;
                    
                }

            }
        }

        private bool IsShipReadyForUnloading(Ship ship, StatusLog lastStatusLog, StatusLog secondLastStatusLog)
        {
            return !ship.HasBeenAlteredThisHour && lastStatusLog != null && secondLastStatusLog != null &&
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
            Guid currentPosition = lastStatusLog.SubjectLocation;

            if (secondLastStatusLog.Status == Status.DockedToShipDock)
            {
                ship.AddStatusChangeToHistory(currentTime, currentPosition, Status.Loading);
            }

            else if (ship.ContainersOnBoard.Count == 0 && !(ship.IsForASingleTrip == true && ContainsTransitStatus(ship)))
            {
                ship.AddStatusChangeToHistory(currentTime, currentPosition, Status.UnloadingDone);

                ship.AddStatusChangeToHistory(currentTime, currentPosition, Status.Loading);
            }
            else if (ship.ContainersOnBoard.Count == 0 && (ship.IsForASingleTrip == true && ContainsTransitStatus(ship)))
            {
                ship.AddStatusChangeToHistory(currentTime, currentPosition, Status.UnloadingDone);

                Guid dockID = harbor.DockShipToShipDock(ship.ID);
                if (dockID != Guid.Empty)
                { 
                    ship.AddStatusChangeToHistory(currentTime, currentPosition, Status.DockingToShipDock); 
                }
                else
                {
                    harbor.AddNewShipToAnchorage(ship);
                    ship.AddStatusChangeToHistory(currentTime, currentPosition, Status.Anchoring);

                }
            }

            ship.HasBeenAlteredThisHour = true;
        }

        /// <summary>
        /// Simulates the unloading of one ship, for one hour.
        /// </summary>
        /// <param name="ship">The ship that is being unloaded</param>
        private void UnloadShipForOneHour(Ship ship)
        {
            int numberOfContainersForTrucks = ship.GetNumberOfContainersToTrucks();
            int numberOfContainersForStorage = ship.GetNumberOfContainersToStorage();

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
                        if (numberOfContainersForStorage != 0 && (ship.ContainersOnBoard.Count - ship.ContainersLeftForTrucks) > 0) // ?? Lage advsInQueue også?
                        {
                            Container container = MoveContainerFromShipToAdv(ship, crane);
                            MoveContainerFromAdvToStorage(container);
                            //Console.WriteLine("Unloading " + ship.Name + " " + container.ID);
                        }

                        if (numberOfContainersForTrucks != 0 && harbor.TrucksInQueue.Count != 0)
                        {
                            Container? container = MoveContainerFromShipToTruck(ship, crane);
                            if (container != null)
                            {
                                harbor.SendTruckOnTransit(loadingDock, container);
                            }
                            //Console.WriteLine("Unloading " + ship.Name + " " + container.ID);
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
        private Container MoveContainerFromShipToAdv(Ship ship, Crane crane)
        {
            Adv adv = harbor.GetFreeAdv();

            Container unloadedContainer = harbor.ShipToCrane(ship, crane, currentTime);
            harbor.CraneToAdv(crane, adv, currentTime);

            return unloadedContainer;

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


                    if (SingleTripShipCanUndock(ship, lastStatusLog)) //ship.IsForASingleTrip == true && containsTransitStatus && lastStatusLog.Status != Status.DockingToShipDock;
                    {
                        Guid dockID = harbor.DockShipToShipDock(ship.ID);
                        ship.AddStatusChangeToHistory(currentTime, dockID, Status.DockingToShipDock);

                    }

                    else if (ShipIsDockingToDock(ship, lastStatusLog)) //lastStatusLog.Status == Status.DockingToShipDock && (CurrentTime - lastStatusLog.PointInTime).TotalHours >= 1
                    {
                        Guid dockID = lastStatusLog.SubjectLocation;
                        ship.AddStatusChangeToHistory(currentTime, dockID, Status.DockedToShipDock);
                    }

                    else if (ShipIsFinishedLoadingContainers(ship, lastStatusLog)) //lastStatusLog.Status == Status.LoadingDone || lastStatusLog.Status == Status.DockedToLoadingDock
                    {
                        ship.AddStatusChangeToHistory(currentTime, harbor.TransitLocationID, Status.Undocking);
                    }

                    else if (ShipIsUndockingFromDock(ship, lastStatusLog)) //lastStatusLog.Status == Status.Undocking && (CurrentTime - lastStatusLog.PointInTime).TotalHours >= 1
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
        }
        private bool ShipCanUndockFromDock(Ship ship, StatusLog lastStatusLog)
        {
            bool containsTransitStatus = ContainsTransitStatus(ship);

            return ship.HasBeenAlteredThisHour == false && lastStatusLog != null &&
                    (lastStatusLog.Status == Status.Undocking || lastStatusLog.Status == Status.LoadingDone || lastStatusLog.Status == Status.DockedToLoadingDock ||
                    (lastStatusLog.Status == Status.UnloadingDone && ContainsTransitStatus(ship)) || lastStatusLog.Status == Status.DockingToShipDock);
        }
        private bool SingleTripShipCanUndock(Ship ship, StatusLog lastStatusLog)
        {
            bool containsTransitStatus = ContainsTransitStatus(ship);
            return ship.IsForASingleTrip == true && containsTransitStatus && lastStatusLog.Status != Status.DockingToShipDock && harbor.FreeShipDockExists(ship.ShipSize) != null;
        }
        private bool ShipIsDockingToDock(Ship ship, StatusLog lastStatusLog)
        {
            return lastStatusLog.Status == Status.DockingToShipDock && (currentTime - lastStatusLog.PointInTime).TotalHours >= 1;
        }
        private bool ShipIsFinishedLoadingContainers(Ship ship, StatusLog lastStatusLog)
        {
            return lastStatusLog.Status == Status.LoadingDone || lastStatusLog.Status == Status.DockedToLoadingDock;
        }
        private bool ShipIsUndockingFromDock(Ship ship, StatusLog lastStatusLog)
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
                StatusLog secondLastStatusLog = ship.HistoryIList[ship.HistoryIList.Count - 2];

                // Dette kan simplifiseres og refaktoriseres ned (f.eks singleTripShip går igjen her..)
                bool shipIsNotSingleTripAndIsDoneUnloading = (lastStatusLog.Status == Status.UnloadingDone && (ship.IsForASingleTrip != true));
                bool shipIsSingleTripAndHasLastUnloadedAndHasNotBeenOnTrip = (lastStatusLog.Status == Status.UnloadingDone && (ship.IsForASingleTrip == true && !ContainsTransitStatus(ship)));
                bool LastLocationWasDockedToShipDock = (lastStatusLog.Status == Status.DockedToLoadingDock && secondLastStatusLog != null && secondLastStatusLog.Status == Status.DockedToShipDock);
                bool shipIsSingleTripAndHasNotBeenOnTrip = (ship.IsForASingleTrip == true && !ContainsTransitStatus(ship));

                if (!ship.HasBeenAlteredThisHour && lastStatusLog != null &&
                    shipIsNotSingleTripAndIsDoneUnloading || 
                    shipIsSingleTripAndHasLastUnloadedAndHasNotBeenOnTrip ||
                     (lastStatusLog.Status == Status.Loading) ||
                     (LastLocationWasDockedToShipDock && shipIsSingleTripAndHasNotBeenOnTrip))
                {
                    Guid currentPosition = lastStatusLog.SubjectLocation;

                    if (ship.ContainersOnBoard.Count < ship.ContainerCapacity && ship.CurrentWeightInTonn < ship.MaxWeightInTonn)
                    {
                        if (harbor.storedContainers.Keys.Count != 0 && ship.ContainersOnBoard.Count < ship.ContainerCapacity)
                        {
                            if (lastStatusLog.Status != Status.Loading)
                            {
                                ship.AddStatusChangeToHistory(currentTime, currentPosition, Status.Loading);
                            }

                            LoadShipForOneHour(ship, currentPosition);

                        }

                        else
                        {
                            ship.AddStatusChangeToHistory(currentTime, currentPosition, Status.LoadingDone);
                            ship.AddStatusChangeToHistory(currentTime, currentPosition, Status.Undocking);

                            ShipUndockingEventArgs shipUndockingEventArgs = new(ship, currentTime, "Ship is undocking from dock.", currentPosition);
                            ShipUndocking?.Invoke(this, shipUndockingEventArgs);
                        }

                    }
                    else
                    {

                        ship.AddStatusChangeToHistory(currentTime, currentPosition, Status.LoadingDone);
                        ship.AddStatusChangeToHistory(currentTime, harbor.TransitLocationID, Status.Undocking);

                        ShipUndockingEventArgs shipUndockingEventArgs = new(ship, currentTime, "Ship is undocking from dock.", currentPosition);
                        ShipUndocking?.Invoke(this, shipUndockingEventArgs);

                    }

                    ship.HasBeenAlteredThisHour = true;

                }
            }
        }

        private void LoadShipForOneHour(Ship ship, Guid currentPosition)
        {
            for (int i = 0; i < ship.ContainersLoadedPerHour; i++)
            {

                Container? loadedContainer = LoadContainerOnShip(ship);

                
                // Event and Status handling
                if (loadedContainer == null)
                {
                    ship.AddStatusChangeToHistory(currentTime, currentPosition, Status.LoadingDone);
                    ship.AddStatusChangeToHistory(currentTime, currentPosition, Status.Undocking);

                    ShipUndockingEventArgs shipUndockingEventArgs = new(ship, currentTime, "Ship is undocking from dock.", currentPosition);
                    ShipUndocking?.Invoke(this, shipUndockingEventArgs);
                    break;
                }
                else
                {
                    // Legger eventet her siden det blir dobbelt opp i LoadContainerOnShip ..?
                    ShipLoadedContainerEventArgs shipLoadedContainerEventArgs = new(ship, currentTime, "Ship has loaded one container.", loadedContainer);
                    ShipLoadedContainer?.Invoke(this, shipLoadedContainerEventArgs);
                }

            }
        }

        /// <summary>
        /// Loads one container from Adv to Ship
        /// </summary>
        /// <param name="ship">The ship that is loading the container onboard.</param>
        /// <returns></returns>
        internal Container? LoadContainerOnShip(Ship ship)
        {

            Container? containerToBeLoaded;

            if (ship.ContainersOnBoard.Count == 0 || ship.ContainersOnBoard.Last().Size == ContainerSize.Full && harbor.GetStoredContainer(ContainerSize.Half) != null)
            {

                bool exceedsMaxWeight = ship.CurrentWeightInTonn + (int)ContainerSize.Half > ship.MaxWeightInTonn;
                bool exceedsMaxCapacity = ship.ContainersOnBoard.Count + 1 > ship.ContainerCapacity;
                if (!(exceedsMaxWeight || exceedsMaxCapacity))
                {
                    containerToBeLoaded = MoveOneContainerFromContainerRowToShip(ContainerSize.Half, ship);
                    return containerToBeLoaded;
                }
            }

            else if (ship.ContainersOnBoard.Last().Size == ContainerSize.Half && harbor.GetStoredContainer(ContainerSize.Full) != null)
            {
                bool exceedsMaxWeight = ship.CurrentWeightInTonn + (int)ContainerSize.Full > ship.MaxWeightInTonn;
                bool exceedsMaxCapacity = ship.ContainersOnBoard.Count + 1 > ship.ContainerCapacity;

                if (!(exceedsMaxWeight || exceedsMaxCapacity))
                {

                    containerToBeLoaded = MoveOneContainerFromContainerRowToShip(ContainerSize.Full, ship);

                    return containerToBeLoaded;

                }
            }

            return null;
        }

        private Container? MoveOneContainerFromContainerRowToShip(ContainerSize containerSize, Ship ship)
        {
            Adv adv = harbor.GetFreeAdv();
            Crane? storageCrane = harbor.GetFreeStorageAreaCrane();
            if (storageCrane == null)
            {
                // EXCEPTION HÅNDTERING HVOR ?
                return null;
            }

            Container? container = harbor.ContainerRowToCrane(containerSize, storageCrane, currentTime);

            if (container == null)
            {
                // Exception ??
            }
            harbor.CraneToAdv(storageCrane, adv, currentTime);

            Dock loadingDock = harbor.GetLoadingDockContainingShip(ship.ID);
            Crane loadingDockCrane = harbor.GetFreeLoadingDockCrane();
            if (adv == null || loadingDockCrane == null)
            {
                // EXCEPTION HÅNDTERING ?
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
                        harbor.AddNewShipToAnchorage(ship);
                        ship.AddStatusChangeToHistory(currentTime, CurrentPosition, Status.Anchoring);

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

            throw new ShipNotFoundExeption("The ship you are trying to get the history from does not exist in the Harbor object the simulation is using.");
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
        /// The unique ID of the dock the ship undocked from.
        /// </summary>
        /// <returns>Guid object representing the ID of the dock the ship undocked from</returns>
        public Guid DockID { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the ShipUndockingEventArgs class.
        /// </summary>
        /// <param name="ship">The ship involved in the event.</param>
        /// <param name="currentTime">The current time in the simulation.</param>
        /// <param name="dockID">The unique ID of the dock the ship undocked from.</param>
        /// <param name="description">A description of the event.</param>
        public ShipUndockingEventArgs(Ship ship, DateTime currentTime, string description, Guid dockID)
            : base(ship, currentTime, description)
        {
            DockID = dockID;
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
}

