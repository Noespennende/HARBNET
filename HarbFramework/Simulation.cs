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
        private DateTime startTime;
        private DateTime currentTime;
        private DateTime endTime;

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
        /// <returns>returns a list of log objects each representing one day of the simulation. Together the list represent the entire history of one simulation.</returns>
        public IList<DailyLog> History { get; } = new List<DailyLog>();

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

            SimulationStartingEventArgs simulationStartingEventArgs = new();
            simulationStartingEventArgs.harborToBeSimulated = harbor;
            simulationStartingEventArgs.startDate = startTime;

            SimulationStarting?.Invoke(this, simulationStartingEventArgs);

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

                DayLoggedEventArgs dayLoggedEventArgs = new DayLoggedEventArgs();
                dayLoggedEventArgs.currentTime = currentTime;
                dayLoggedEventArgs.message = $"\nDay over\n Current Time: ";
                //shipAnchoringEventArgs.todaysLog = 

                EndOf24HourPeriod(dayLoggedEventArgs);

                continue;
            }
            SimulationEndedEventArgs simulationEndedEventArgs = new SimulationEndedEventArgs();

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
                shipAnchoringEventArgs shipAnchoringEventArgs = new();
                shipAnchoringEventArgs.ship = ship;
                shipAnchoringEventArgs.currentTime = currentTime;

                if (ship.StartDate == currentTime)
                {

                    harbor.AddNewShipToAnchorage(ship);

                    if (ship.IsForASingleTrip == true)
                    {
                        Guid shipDock = harbor.StartShipInLoadingDock(ship.ID);

                        ship.AddStatusChangeToHistory(currentTime, shipDock, Status.DockedToLoadingDock);
                    }
                    else
                    {
                        ship.AddStatusChangeToHistory(currentTime, harbor.AnchorageID, Status.Anchoring);
                        shipAnchoringEventArgs.anchorageID = harbor.AnchorageID;
                        ShipAnchoring.Invoke(this, shipAnchoringEventArgs);
                    }

                    History.Add(new DailyLog(currentTime, harbor.Anchorage, harbor.GetShipsInTransit(), harbor.GetContainersStoredInHarbour(),
                        harbor.GetShipsInLoadingDock(), harbor.GetShipsInShipDock()));
                }
            }
        }

        /// <summary>
        /// Ends the 24 hour period with log and status updates, and raises event.
        /// </summary>
        /// <param name="dayLoggedEventArgs"></param>
        private void EndOf24HourPeriod(DayLoggedEventArgs dayLoggedEventArgs)
        {
            DateTime past24Hours = currentTime.AddHours(-24);
            if (currentTime.Hour == 0)
            {
                DayEnded.Invoke(this, dayLoggedEventArgs);


                History.Add(new DailyLog(currentTime, harbor.Anchorage, harbor.GetShipsInTransit(), harbor.GetContainersStoredInHarbour(),
                    harbor.GetShipsInLoadingDock(), harbor.GetShipsInShipDock()));

                foreach (Container container in harbor.GetContainersStoredInHarbour())
                {
                    container.AddAnotherDayInStorage();
                }

                foreach (Ship ship in harbor.AllShips)
                {
                    List<StatusLog> DayReviewShipLogs = new();

                    foreach (StatusLog log in ship.HistoryIList)
                    {

                        if (log.PointInTime >= past24Hours && log.PointInTime <= currentTime)
                        {
                            DayReviewShipLogs.Add(log);
                        }
                    }
                    dayLoggedEventArgs.ship = ship;
                    dayLoggedEventArgs.dayReviewShipLogs = DayReviewShipLogs;
                    DayLoggedToSimulationHistory.Invoke(this, dayLoggedEventArgs);
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
        /// Anchoring ship to a dock, ship.Status is set to Anchoring.
        /// </summary>
        private void AnchoringShips()
        {
            shipAnchoredEventArgs shipAnchoredEventArgs = new shipAnchoredEventArgs();
            List<Ship> Anchorage = harbor.Anchorage.ToList();

            foreach (Ship ship in Anchorage)
            {

                Guid anchorageID = harbor.AnchorageID;
                Guid shipID = ship.ID;
                StatusLog lastStatusLog = ship.HistoryIList.Last();
                shipAnchoredEventArgs.ship = ship;
                shipAnchoredEventArgs.currentTime = currentTime;
                shipAnchoredEventArgs.anchorageID = anchorageID;
                shipAnchoredEventArgs.message = lastStatusLog.ToString();

                if (!ship.HasBeenAlteredThisHour && lastStatusLog != null && lastStatusLog.Status == Status.Anchoring)
                {

                    ship.HasBeenAlteredThisHour = true;

                    ship.AddStatusChangeToHistory(currentTime, anchorageID, Status.Anchored);

                    ShipAnchored?.Invoke(this, shipAnchoredEventArgs);

                }
            }
        }


        /// <summary>
        /// Docking ships to the harbor, the shipStatus is set to docking
        /// </summary>
        private void DockingShips()
        {

            List<Ship> Anchorage = harbor.Anchorage.ToList();
            List<Ship> ShipsInShipDock = new(harbor.shipsInShipDock.Keys);
            List<Ship> ShipsInLoadingDock = new(harbor.shipsInLoadingDock.Keys);


            foreach (Ship ship in ShipsInShipDock)
            {
                Guid shipID = ship.ID;
                StatusLog lastStatusLog = ship.HistoryIList.Last();

                if (!ship.HasBeenAlteredThisHour && lastStatusLog != null &&
                    (lastStatusLog.Status == Status.Anchored ||
                    lastStatusLog.Status == Status.DockedToShipDock ||
                    lastStatusLog.Status == Status.DockingToLoadingDock ||
                    (lastStatusLog.Status == Status.UnloadingDone && (ship.IsForASingleTrip == true && ContainsTransitStatus(ship)))))
                {
                    Guid dockID;
                    shipDockingToLoadingDockEventArgs shipDockingToLoadingDockEventArgs = new shipDockingToLoadingDockEventArgs();
                    shipDockingToLoadingDockEventArgs.ship = ship;
                    shipDockingToLoadingDockEventArgs.currentTime = currentTime;
                    if (currentTime == startTime && lastStatusLog.Status == Status.DockedToShipDock)
                    {

                        dockID = harbor.DockShipFromShipDockToLoadingDock(ship.ID, currentTime);
                        shipDockingToLoadingDockEventArgs.dockId = dockID;
                        ship.AddStatusChangeToHistory(currentTime, dockID, Status.DockingToLoadingDock);
                        ShipDockingtoLoadingDock?.Invoke(this, shipDockingToLoadingDockEventArgs); ;

                    }
                }
            }

            foreach (Ship ship in ShipsInLoadingDock)
            {
                Guid shipID = ship.ID;
                StatusLog lastStatusLog = ship.HistoryIList.Last();
                shipDockedToLoadingDockEventArgs shipDockedToLoadingDockEventArgs = new shipDockedToLoadingDockEventArgs();


                if (!ship.HasBeenAlteredThisHour && lastStatusLog != null &&
                    (lastStatusLog.Status == Status.DockedToShipDock ||
                    lastStatusLog.Status == Status.DockingToLoadingDock ||
                    (lastStatusLog.Status == Status.UnloadingDone && (ship.IsForASingleTrip == true && ContainsTransitStatus(ship)))))
                {

                    if (lastStatusLog.Status == Status.DockingToLoadingDock && (currentTime - lastStatusLog.PointInTime).TotalHours >= 1)
                    {
                        Guid dockID = lastStatusLog.SubjectLocation;

                        ship.AddStatusChangeToHistory(currentTime, dockID, Status.DockedToLoadingDock);
                        shipDockedToLoadingDockEventArgs.dockId = dockID;
                        shipDockedToLoadingDockEventArgs.ship = ship;
                        shipDockedToLoadingDockEventArgs.currentTime = currentTime;
                        ShipDockedtoLoadingDock.Invoke(this, shipDockedToLoadingDockEventArgs);
                        if (ship.IsForASingleTrip && !ContainsTransitStatus(ship))
                        {
                            ship.AddStatusChangeToHistory(currentTime, dockID, Status.Loading);
                            //shipLoadeadContainer.Invoke(ship);
                        }
                        else
                        {
                            ship.AddStatusChangeToHistory(currentTime, dockID, Status.Unloading);
                        }
                    }
                }
            }

            foreach (Ship ship in Anchorage)
            {
                shipDockingToShipDockEventArgs shipDockingToShipDockEventArgs = new shipDockingToShipDockEventArgs();
                shipDockingToLoadingDockEventArgs ShipDockingToLoadingDockEventArgs = new shipDockingToLoadingDockEventArgs();

                shipDockingToShipDockEventArgs.ship = ship;
                shipDockingToShipDockEventArgs.currentTime = currentTime;

                ShipDockingToLoadingDockEventArgs.ship = ship;
                ShipDockingToLoadingDockEventArgs.currentTime = currentTime;

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
                    if (harbor.FreeLoadingDockExists(ship.ShipSize) && lastStatusLog.Status != Status.DockedToShipDock)
                    {
                        

                        if (lastStatusLog.Status == Status.Anchored)
                        {
                            dockID = harbor.DockShipToLoadingDock(shipID, currentTime);
                            shipDockingToShipDockEventArgs.dockId = dockID;
                            ShipDockingToLoadingDockEventArgs.dockId = dockID;

                            ship.AddStatusChangeToHistory(currentTime, dockID, Status.DockingToLoadingDock);
                            ShipDockingtoLoadingDock?.Invoke(this, ShipDockingToLoadingDockEventArgs);
                        }

                        if (harbor.FreeShipDockExists(ship.ShipSize) && ship.IsForASingleTrip == true && ContainsTransitStatus(ship)
                            && ship.ContainersOnBoard.Count == 0 && currentTime != startTime
                            && lastStatusLog.Status != Status.DockingToShipDock)
                        {
                            dockID = harbor.DockShipToShipDock(shipID);
                            ShipDockingToLoadingDockEventArgs.dockId = dockID;
                            ship.AddStatusChangeToHistory(currentTime, dockID, Status.DockingToShipDock);
                            ShipDockingtoLoadingDock?.Invoke(this, ShipDockingToLoadingDockEventArgs);

                        }

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

                StatusLog? lastStatusLog = ship.HistoryIList?.Last();

                StatusLog? secondLastStatusLog = ship.HistoryIList?[ship.HistoryIList.Count - 2];

                if (!ship.HasBeenAlteredThisHour && lastStatusLog != null && secondLastStatusLog != null &&
                    (lastStatusLog.Status == Status.Unloading || lastStatusLog.Status == Status.DockedToLoadingDock && secondLastStatusLog != null))
                {

                    Guid currentPosition = lastStatusLog.SubjectLocation;

                    // Unloading container
                    if (ship.ContainersOnBoard.Count != 0 && lastStatusLog.Status == Status.DockedToLoadingDock || lastStatusLog.Status == Status.Unloading)
                    {
                        if (lastStatusLog.Status == Status.DockedToLoadingDock)
                        {
                            ship.AddStatusChangeToHistory(currentTime, currentPosition, Status.Unloading);

                            ship.ContainersLeftForTrucks = ship.GetNumberOfContainersToTrucks();
                        }

                        // ** Unloading **

                        UnloadShipForOneHour(ship);

                    }

                    // Status oppdateringer 
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

                    ship.HasBeenAlteredThisHour = true;

                }

            }
        }

        /// <summary>
        /// Simulates the unloading of one ship, for one hour.
        /// </summary>
        /// <param name="ship">The ship that is being unloaded</param>
        private void UnloadShipForOneHour(Ship ship)
        {
            int numberOfContainersForTrucks = ship.GetNumberOfContainersToTrucks();
            int numberOfContainersForStorage = ship.GetNumberOfContainersToStorage();

            Dock loadingDock = harbor.GetLoadingDockContainingShip(ship.ID);


            // Før: for (int i = 0; i < ship.ContainersLoadedPerHour && ship.ContainersOnBoard.Count > 0; i++)
            // Hver kran på LoadingDock gjør max antall avlast - simulerer da at hver kran laster av "samtidig", da hver kran gjør hvert sitt maksimum per time
            foreach (Crane crane in loadingDock.AssignedCranes)
            {

                // Regner ut maks mulige avlastinger basert på det minste tallet - kan ikke ADVer gjøre mer enn 10 i timen, men kran kan gjøre 20, så gjør vi aldri mer enn 10.
                int maxLoadsPerHour = Math.Min(harbor.AdvLoadsPerHour, crane.ContainersLoadedPerHour);

                // Gjør det til maks per time er nådd, eller skipet er tomt
                for (int i = 0; i < maxLoadsPerHour && ship.ContainersOnBoard.Count > 0; i++)
                {
                    if (numberOfContainersForStorage != 0 && (ship.ContainersOnBoard.Count - ship.ContainersLeftForTrucks) > 0) // ?? Lage advsInQueue også?
                    {
                        Container container = MoveContainerFromShipToAdv(ship, crane);
                        MoveContainerFromAdvToStorage(container);
                    }

                    if (numberOfContainersForTrucks != 0 && harbor.TrucksInQueue.Count != 0)
                    {
                        Truck? truck = MoveContainerFromShipToTruck(ship, crane);
                        harbor.SendTruckOnTransit(loadingDock, truck);
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
            shipUnloadedContainerEventArgs.currentTime = currentTime;
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

            harbor.CraneToContainerRow(crane, currentTime);

            // Returnere plasseringen her kanskje (ContainerRow) eller container igjen?

            // Event og status håndtering kommer //

            /*ShipUnloadedContainerEventArgs shipUnloadedContainerEventArgs = new();
            shipUnloadedContainerEventArgs.currentTime = currentTime;
            shipUnloadedContainerEventArgs.ship = ship;
            shipUnloadedContainerEventArgs.Container = containerToBeUnloaded;

            ShipUnloadedContainer?.Invoke(this, shipUnloadedContainerEventArgs); */
        }

        /// <summary>
        /// Moves one container from ship to crane to truck.
        /// </summary>
        /// <param name="ship">The ship the container is unloaded from</param>
        /// <param name="crane">The crane at the dock that is used for moving between ship and truck</param>
        private Truck? MoveContainerFromShipToTruck(Ship ship, Crane crane)
        {

            Dock loadingDock = harbor.GetLoadingDockContainingShip(ship.ID);

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
            
            harbor.ShipToCrane(ship, crane, currentTime);
            harbor.CraneToTruck(crane, truck, currentTime);
            ship.ContainersLeftForTrucks--;

            return truck;

            // Event og status håndtering kommer //

            /*ShipUnloadedContainerEventArgs shipUnloadedContainerEventArgs = new();
            shipUnloadedContainerEventArgs.currentTime = currentTime;
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
                StatusLog lastStatusLog = ship.HistoryIList.Last();


                if (ship.HasBeenAlteredThisHour == false && lastStatusLog != null &&
                    (lastStatusLog.Status == Status.Undocking || lastStatusLog.Status == Status.LoadingDone || lastStatusLog.Status == Status.DockedToLoadingDock ||
                    (lastStatusLog.Status == Status.UnloadingDone && ContainsTransitStatus(ship)) || lastStatusLog.Status == Status.DockingToShipDock))
                {
                    bool containsTransitStatus = ContainsTransitStatus(ship);

                    if (ship.IsForASingleTrip == true && containsTransitStatus && lastStatusLog.Status != Status.DockingToShipDock)
                    {
                        Guid dockID = lastStatusLog.SubjectLocation;
                        ship.AddStatusChangeToHistory(currentTime, dockID, Status.DockingToShipDock);

                    }

                    else if (lastStatusLog.Status == Status.DockingToShipDock && (currentTime - lastStatusLog.PointInTime).TotalHours >= 1)
                    {
                        Guid dockID = harbor.DockShipToShipDock(ship.ID);
                        ship.AddStatusChangeToHistory(currentTime, dockID, Status.DockedToShipDock);
                    }

                    else if (lastStatusLog.Status == Status.LoadingDone || lastStatusLog.Status == Status.DockedToLoadingDock)
                    {
                        ship.AddStatusChangeToHistory(currentTime, harbor.TransitLocationID, Status.Undocking);
                    }

                    else if (lastStatusLog.Status == Status.Undocking && (currentTime - lastStatusLog.PointInTime).TotalHours >= 1)
                    {

                        harbor.UnDockShipFromLoadingDockToTransit(shipID, currentTime);
                        ship.AddStatusChangeToHistory(currentTime, harbor.TransitLocationID, Status.Transit);

                        ShipInTransitEventArgs shipInTransitEventArgs = new();
                        shipInTransitEventArgs.currentTime = currentTime;
                        shipInTransitEventArgs.ship = ship;
                        shipInTransitEventArgs.transitLocationID = harbor.TransitLocationID;
                        
                        ShipInTransit?.Invoke(this, shipInTransitEventArgs);
                    }



                    ship.HasBeenAlteredThisHour = true;

                }
            }
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
                shipLoadedContainerEventArgs shipLoadedContainerEventArgs = new shipLoadedContainerEventArgs();
                ShipUndockingEventArgs shipUndockingEventArgs = new ShipUndockingEventArgs();

                shipUndockingEventArgs.ship = ship;
                shipUndockingEventArgs.currentTime = currentTime;
                
                shipLoadedContainerEventArgs.ship = ship;
                shipLoadedContainerEventArgs.currentTime = currentTime;

                if (!ship.HasBeenAlteredThisHour && lastStatusLog != null &&
                    ((lastStatusLog.Status == Status.UnloadingDone && (ship.IsForASingleTrip != true)) ||
                     (lastStatusLog.Status == Status.UnloadingDone && (ship.IsForASingleTrip == true && !ContainsTransitStatus(ship))) ||
                     (lastStatusLog.Status == Status.Loading) ||
                     (lastStatusLog.Status == Status.DockedToLoadingDock && secondLastStatusLog != null && secondLastStatusLog.Status == Status.DockedToShipDock) &&
                     (ship.IsForASingleTrip == true && !ContainsTransitStatus(ship))))
                {
                    Guid currentPosition = lastStatusLog.SubjectLocation;

                    shipUndockingEventArgs.dockId = currentPosition;

                    if (ship.ContainersOnBoard.Count < ship.ContainerCapacity && ship.CurrentWeightInTonn < ship.MaxWeightInTonn)
                    {
                        if (harbor.storedContainers.Keys.Count != 0 && ship.ContainersOnBoard.Count < ship.ContainerCapacity)
                        {
                            if (lastStatusLog.Status != Status.Loading)
                            {
                                ship.AddStatusChangeToHistory(currentTime, currentPosition, Status.Loading);
                                //shipLoadeadContainer.Invoke(ship);
                            }

                            for (int i = 0; i < ship.ContainersLoadedPerHour; i++)
                            {

                                Container? loadedContainer = LoadContainerOnShip(ship);

                                // Event and Status handling
                                if (loadedContainer == null)
                                {
                                    ship.AddStatusChangeToHistory(currentTime, currentPosition, Status.LoadingDone);
                                    ship.AddStatusChangeToHistory(currentTime, currentPosition, Status.Undocking);
                                    ShipUndocking?.Invoke(this, shipUndockingEventArgs);
                                    break;
                                }
                                else
                                {
                                    shipLoadedContainerEventArgs.Container = loadedContainer;
                                    ShipLoadedContainer?.Invoke(this, shipLoadedContainerEventArgs);
                                }   

                            }

                        }

                        else
                        {
                            ship.AddStatusChangeToHistory(currentTime, currentPosition, Status.LoadingDone);
                            ship.AddStatusChangeToHistory(currentTime, currentPosition, Status.Undocking);
                            ShipUndocking?.Invoke(this, shipUndockingEventArgs);
                        }

                    }
                    else
                    {

                        ship.AddStatusChangeToHistory(currentTime, currentPosition, Status.LoadingDone);
                        ship.AddStatusChangeToHistory(currentTime, harbor.TransitLocationID, Status.Undocking);
                        ShipUndocking?.Invoke(this, shipUndockingEventArgs);

                    }

                    ship.HasBeenAlteredThisHour = true;

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

            Container containerToBeLoaded;

            if (ship.ContainersOnBoard.Count == 0 || ship.ContainersOnBoard.Last().Size == ContainerSize.Full && harbor.GetStoredContainer(ContainerSize.Half) != null)
            {
                if (ship.CurrentWeightInTonn + (int)ContainerSize.Half <= ship.MaxWeightInTonn && ship.ContainersOnBoard.Count < ship.ContainerCapacity)
                {

                    containerToBeLoaded = harbor.GetStoredContainer(ContainerSize.Half);

                    // NYE METODEKALL HER

                    /*harbor.LoadContainer(ContainerSize.Half, ship, currentTime);*/

                    return containerToBeLoaded;
                }
            }

            else if (ship.ContainersOnBoard.Last().Size == ContainerSize.Half && harbor.GetStoredContainer(ContainerSize.Full) != null)
            {
                if (ship.CurrentWeightInTonn + (int)ContainerSize.Full <= ship.MaxWeightInTonn && ship.ContainersOnBoard.Count < ship.ContainerCapacity)
                {

                    containerToBeLoaded = harbor.GetStoredContainer(ContainerSize.Full);

                    harbor.LoadContainer(ContainerSize.Full, ship, currentTime);
                    return containerToBeLoaded;

                }
            }
            
            return null;
        }

        /// <summary>
        /// Returns ships in transit to harbor.
        /// </summary>
        private void InTransitShips()
        {
            shipAnchoringEventArgs shipAnchoringEventArgs = new shipAnchoringEventArgs();
            foreach (Ship ship in harbor.ShipsInTransit.Keys)
            {
                shipAnchoringEventArgs.ship = ship;
                StatusLog lastStatusLog = ship.HistoryIList.Last();

                if (ship.HasBeenAlteredThisHour == false && lastStatusLog != null && lastStatusLog.Status == Status.Transit)
                {

                    Guid CurrentPosition = lastStatusLog.SubjectLocation;
                    StatusLog LastHistoryStatusLog = ship.HistoryIList.Last();
                    shipAnchoringEventArgs.currentTime = currentTime;
                    double DaysSinceTransitStart = (currentTime - LastHistoryStatusLog.PointInTime).TotalDays;

                    if (DaysSinceTransitStart >= ship.RoundTripInDays)
                    {
                        harbor.AddNewShipToAnchorage(ship);
                        ship.AddStatusChangeToHistory(currentTime, CurrentPosition, Status.Anchoring);
                        shipAnchoringEventArgs.anchorageID = harbor.AnchorageID;
                        ShipAnchoring?.Invoke(this, shipAnchoringEventArgs);
                    }

                    ship.HasBeenAlteredThisHour = true;

                }
            }
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
        /// Returns a ReadOnlyCollection of DailyLog objects that together represent the history of the simulation.
        /// </summary>
        /// <returns>ReadOnlyCollection of DailyLog objects. Each one contains information about a single day of the simulation.</returns>
        public Harbor harborToBeSimulated { get; internal set; }
        /// <summary>
        /// The time the simulation will start from
        /// </summary>
        /// <returns>Datetime object representing the simulations start time</returns>
        public DateTime startDate { get; internal set; }
        /// <summary>
        /// Returns a string representing the event.
        /// </summary>
        /// <returns>String representing the event that was raised</returns>
        public String message { get; internal set; }
    }

    /// <summary>
    /// The EventArgs class for the SimulationEnded event.
    /// </summary>
    public class SimulationEndedEventArgs : EventArgs
    {
        /// <summary>
        /// Returns a ReadOnlyCollection of DailyLog objects that together represent the history of the simulation.
        /// </summary>
        /// <returns>ReadOnlyCollection of DailyLog objects. Each one contains information about a single day of the simulation.</returns>
        public ReadOnlyCollection<DailyLog> simulationHistory { get; internal set; }
        /// <summary>
        /// Returns a string representing the event.
        /// </summary>
        /// <returns>String representing the event that was raised</returns>
        public String message { get; internal set; }
    }
    /// <summary>
    /// The EventArgs class for the DayOver event.
    /// </summary>
    public class DayOverEventArgs : EventArgs
    {
        /// <summary>
        /// Returns a DailyLog object containing information about the previous day in the simulation.
        /// </summary>
        /// <returns>DailyLog object containing information about the state of the simulation at the time the object was created</returns>
        public DailyLog todaysLog { get; internal set; }
        /// <summary>
        /// The time in the simulation the event was raised.
        /// </summary>
        /// <returns>DateTime object representing the time in the simulation the event was raised</returns>
        public DateTime currentTime { get; internal set; }
        /// <summary>
        /// Returns a string representing the event.
        /// </summary>
        /// <returns>String representing the event that was raised</returns>
        public String message { get; internal set; }

    }
    /// <summary>
    /// The EventArgs class for the DayLogged event
    /// </summary>
    public class DayLoggedEventArgs : EventArgs
    {
        /// <summary>
        /// Returns a DailyLog containing information about the state of the harbor the day the event was raised
        /// </summary>
        /// <returns>DailyLog containing information about the state of the harbor the day the event was raised</returns>
        public DailyLog todaysLog { get; internal set; }
        /// <summary>
        /// The time in the simulation the event was raised.
        /// </summary>
        /// <returns>DateTime object representing the time in the simulation the event was raised</returns>
        public DateTime currentTime { get; internal set; }
        /// <summary>
        /// Returns a string representing the event.
        /// </summary>
        /// <returns>String representing the event that was raised</returns>
        public String message { get; internal set; }
        /// <summary>
        /// The ship that the dayReviewShipLogs logs come from
        /// </summary>
        /// <returns>Ship object representing the ship that logged the DayReview logs</returns>
        public Ship ship { get; internal set; }
        /// <summary>
        /// Returns a list of all logs registered by ship in the past day
        /// </summary>
        /// <returns>List with all logs registered by ship in the past day</returns>
        public IList<StatusLog>? dayReviewShipLogs { get; internal set; }
    }
    /// <summary>
    /// The EventArgs class for the ShipUndocked event.
    /// </summary>
    public class ShipUndockingEventArgs : EventArgs
    {
        /// <summary>
        /// The ship that undocked
        /// </summary>
        /// <returns>Ship object representing the ship that undocked</returns>
        public Ship ship { get; internal set; }
        /// <summary>
        /// The time in the simulation the event was raised.
        /// </summary>
        /// <returns>DateTime object representing the time in the simulation the event was raised.</returns>
        public DateTime currentTime { get; internal set; }
        /// <summary>
        /// The unique ID of the dock the ship undocked from.
        /// </summary>
        /// <returns>Guid object representing the ID of the dock the ship undocked from</returns>
        public Guid dockId { get; internal set; }
        /// <summary>
        /// Returns a string representing the event.
        /// </summary>
        /// <returns>String representing the event that was raised</returns>
        public String message { get; internal set; }
    }

    /// <summary>
    /// The EventArgs class for the ShipInTransit event.
    /// </summary>
    public class ShipInTransitEventArgs : EventArgs
    {
        /// <summary>
        /// The ship that is in transit
        /// </summary>
        /// <returns>Ship object representing the ship that undocked</returns>
        public Ship ship { get; internal set; }
        /// <summary>
        /// The time in the simulation the event was raised.
        /// </summary>
        /// <returns>DateTime object representing the time in the simulation the event was raised.</returns>
        public DateTime currentTime { get; internal set; }
        /// <summary>
        /// The unique ID of the dock the ship undocked from.
        /// </summary>
        /// <returns>Guid object representing the ID of the transit location the ship is located at.</returns>
        public Guid transitLocationID { get; internal set; }
    }

    /// <summary>
    /// The EventArgs class for the ShipDockingToShipDock event.
    /// </summary>
    public class shipDockingToShipDockEventArgs : EventArgs
    {
        /// <summary>
        /// The ship that that is docking to the ship dock
        /// </summary>
        /// <returns>Ship object representing the ship that is docking to the ship dock</returns>
        public Ship ship { get; internal set; }
        /// <summary>
        /// The time in the simulation the event was raised.
        /// </summary>
        /// <returns>DateTime object representing the time in the simulation the event was raised</returns>
        public DateTime currentTime { get; internal set; }
        /// <summary>
        /// The unique ID of the dock the ship is docking to.
        /// </summary>
        /// <returns>Guid object representing the ID of the dock the ship is docking to</returns>
        public Guid dockId { get; internal set; }
        /// <summary>
        /// Returns a string representing the event.
        /// </summary>
        /// <returns>String representing the event that was raised</returns>
        public String message { get; internal set; }
    }
    /// <summary>
    /// The EventArgs class for the shipDockedToShipDock event.
    /// </summary>
    public class shipDockedToShipDockEventArgs : EventArgs
    {
        /// <summary>
        /// The ship that that docked to the ship dock
        /// </summary>
        /// <returns>Ship object representing the ship that docked to the ship dock</returns>
        public Ship ship { get; internal set; }
        /// <summary>
        /// The time in the simulation the event was raised.
        /// </summary>
        /// <returns>DateTime object representing the time in the simulation the event was raised</returns>
        public DateTime currentTime { get; internal set; }
        /// <summary>
        /// The unique ID of the dock the ship docked to.
        /// </summary>
        /// <returns>Guid object representing the ID of the dock the ship docked to</returns>
        public Guid dockId { get; internal set; }
        /// <summary>
        /// Returns a string representing the event.
        /// </summary>
        /// <returns>String representing the event that was raised</returns>
        public String message { get; internal set; }
    }
    /// <summary>
    /// The EventArgs class for the shipDockingToLoadingDock event.
    /// </summary>
    public class shipDockingToLoadingDockEventArgs : EventArgs
    {
        /// <summary>
        /// The ship that is docking to the loading dock
        /// </summary>
        /// <returns>Ship object representing the ship that is docking to the loading dock</returns>
        public Ship ship { get; internal set; }
        /// <summary>
        /// The time in the simulation the event was raised.
        /// </summary>
        /// <returns>DateTime object representing the time in the simulation the event was raised</returns>
        public DateTime currentTime { get; internal set; }
        /// <summary>
        /// The unique ID of the dock the ship is docking to.
        /// </summary>
        /// <returns>Guid object representing the ID of the dock the ship is docking to</returns>
        public Guid dockId { get; internal set; }
        /// <summary>
        /// Returns a string representing the event.
        /// </summary>
        /// <returns>String representing the event that was raised</returns>
        public String message { get; internal set; }
    }
    /// <summary>
    /// The EventArgs class for the shipDockedToLoadingDock event.
    /// </summary>
    public class shipDockedToLoadingDockEventArgs : EventArgs
    {
        /// <summary>
        /// The ship that has docked to the loading dock.
        /// </summary>
        /// <returns>Ship object representing the ship that has docked to the loading dock</returns>
        public Ship ship { get; internal set; }
        /// <summary>
        /// The time in the simulation the event was raised.
        /// </summary>
        /// <returns>DateTime object representing the time in the simulation the event was raised</returns>
        public DateTime currentTime { get; internal set; }
        /// <summary>
        /// The unique ID of the dock the ship docked to.
        /// </summary>
        /// <returns>Guid object representing the ID of the dock the ship docked to</returns>
        public Guid dockId { get; internal set; }
        /// <summary>
        /// Returns a string representing the event.
        /// </summary>
        /// <returns>String representing the event that was raised</returns>
        public String message { get; internal set; }
    }
    /// <summary>
    /// The EventArgs class for the ShipLoadedContainer event.
    /// </summary>
    public class shipLoadedContainerEventArgs : EventArgs
    {
        /// <summary>
        /// The ship that loaded the container in to its cargo
        /// </summary>
        /// <returns>Ship object representing the ship loaded the container in to its cargo</returns>
        public Ship ship { get; internal set; }
        /// <summary>
        /// The time in the simulation the event was raised.
        /// </summary>
        /// <returns>DateTime object representing the time in the simulation the event was raised</returns>
        public DateTime currentTime { get; internal set; }
        /// <summary>
        /// The container loaded omboard the ship
        /// </summary>
        /// <returns>Container object representing the container loaded omboard the ship</returns>
        public Container Container { get; internal set; }
        /// <summary>
        /// Returns a string representing the event.
        /// </summary>
        /// <returns>String representing the event that was raised</returns>
        public String message { get; internal set; }
    }
    /// <summary>
    /// The EventArgs class for the ShipUnloadedContainer event.
    /// </summary>
    public class ShipUnloadedContainerEventArgs : EventArgs
    {
        /// <summary>
        /// The ship that unloaded the container on to the harbor
        /// </summary>
        /// <returns>Ship object representing the ship that unloaded the container on to the harbor</returns>
        public Ship ship { get; internal set; }
        /// <summary>
        /// The time in the simulation the event was raised.
        /// </summary>
        /// <returns>DateTime object representing the time in the simulation the event was raised</returns>
        public DateTime currentTime { get; internal set; }
        /// <summary>
        /// The container unloaded from the ship and on to the harbor.
        /// </summary>
        /// <returns>Container object representing the container unloaded from the ship and on to the harbor</returns>
        public Container Container { get; internal set; }
        /// <summary>
        /// Returns a string representing the event.
        /// </summary>
        /// <returns>String representing the event that was raised</returns>
        public String message { get; internal set; }
    }
    /// <summary>
    /// The EventArgs class for the ShipAnchored event.
    /// </summary>
    public class shipAnchoredEventArgs : EventArgs
    {
        /// <summary>
        /// The ship that anchored to the anchorage
        /// </summary>
        /// <returns>Ship object representing the ship that anchored to the anchorage</returns>
        public Ship ship { get; internal set; }
        /// <summary>
        /// The time in the simulation the event was raised.
        /// </summary>
        /// <returns>DateTime object representing the time in the simulation the event was raised</returns>
        public DateTime currentTime { get; internal set; }
        /// <summary>
        /// The ID of the anchorage
        /// </summary>
        /// <returns>Guid object representing the ID of the anchorage</returns>
        public Guid anchorageID { get; internal set; }
        /// <summary>
        /// Returns a string representing the event.
        /// </summary>
        /// <returns>String representing the event that just was raised</returns>
        public String message { get; internal set; }
    }
    /// <summary>
    /// The EventArgs class for the ShipAnchoring event.
    /// </summary>
    public class shipAnchoringEventArgs : EventArgs
    {
        /// <summary>
        /// The ship that is anchoring to the anchorage
        /// </summary>
        /// <returns>Ship object representing the ship that is anchoring to the anchorage</returns>
        public Ship ship { get; internal set; }
        /// <summary>
        /// The time in the simulation the event was raised.
        /// </summary>
        /// <returns>DateTime object representing the time in the simulation the event was raised</returns>
        public DateTime currentTime { get; internal set; }
        /// <summary>
        /// The ID of the anchorage
        /// </summary>
        /// <returns>Guid object representing the ID of the anchorage</returns>
        public Guid anchorageID { get; internal set; }
        /// <summary>
        /// Returns a string representing the event.
        /// </summary>
        /// <returns>String representing the event that just was raised</returns>
        public String message { get; internal set; }
    }
}

