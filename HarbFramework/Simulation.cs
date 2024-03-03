using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

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

        public delegate void SimulationEndHandler(String message);
        public event SimulationEndHandler SimulationEnd;

        public delegate void DayOverEventHandler(String message, DateTime currentTime);
        public event DayOverEventHandler? DayOverEvent;

        public delegate void ShipStatusEventHandler(string message);
        public event ShipStatusEventHandler? ShipStatusEvent;

        public delegate void ShipUndockedHandler(Ship ship);
        public event ShipUndockedHandler? ShipUnDocked;

        public delegate void shipDockedToShipDockHandler(Ship ship);
        public event shipDockedToShipDockHandler? shipDockedShipDock;

        public delegate void shipDockingToLoadingDockHandler(Ship ship);
        public event shipDockingToLoadingDockHandler? ShipDockingtoLoadingDock;

        public delegate void shipDockedToLoadingDockHandler(Ship ship);
        public event shipDockedToLoadingDockHandler? ShipDockedtoLoadingDock;

        public delegate void shipLoadingContainerHandler(Ship ship);
        public event shipLoadingContainerHandler? shipLoadingContainer;

        public delegate void shipUnloadingContainerHandler(Ship ship);
        public event shipUnloadingContainerHandler? shipUnloadingContainer;

        public delegate void shipAnchoredHandler(Ship ship);
        public event shipAnchoredHandler? shipAnchored;

        public delegate void shipAnchoringHandler(Ship ship);
        public event shipAnchoringHandler? shipAnchoring;

        /// <summary>
        /// History for all ships and containers in the simulation in the form of Log objects. Each Log object stores information for one day in the simulation and contains information about the location and status of all ships and containers that day.
        /// </summary>
        /// <returns>returns a list of log objects each representing one day of the simulation. Together the list represent the entire history of one simulation.</returns>
        public IList<DailyLog> History {  get; } = new List<DailyLog>();

        /// <summary>
        /// Simulation constructor.
        /// </summary>
        /// <param name="harbor">The harbor which will be used in the simulation</param>
        /// <param name="simulationStartTime">The start time of the simulation</param>
        /// <param name="simulationEndTime">The end time of simulation</param>
        public Simulation (Harbor harbor, DateTime simulationStartTime, DateTime simulationEndTime)
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
            

            Console.WriteLine("Simulation starting ...");
            Thread.Sleep(2000);

            while (currentTime < endTime)
            {

              
                foreach (Ship ship in harbor.AllShips)
                {
                    if (ship.StartDate == currentTime) { 

                        harbor.AddNewShipToAnchorage(ship);

                            if (ship.IsForASingleTrip == true)
                            {
                                Guid shipDock = harbor.StartShipInShipDock(ship.ID);
                                harbor.AddContainersToHarbor(ship.ContainerCapacity, currentTime);

                                ship.AddStatusChangeToHistory(currentTime, shipDock, Status.DockedToShipDock);
                            }
                            else
                            {
                                ship.AddStatusChangeToHistory(currentTime, harbor.AnchorageID, Status.Anchoring);
                                shipAnchoring?.Invoke(ship);
                            }

                            History.Add(new DailyLog(currentTime, harbor.Anchorage, harbor.GetShipsInTransit(), harbor.GetContainersStoredInHarbour(), 
                                harbor.GetShipsInLoadingDock(), harbor.GetShipsInShipDock()));
                    }
                }



                foreach (Ship ship in harbor.AllShips) {
                    ship.HasBeenAlteredThisHour = false;
                    
                }

                UndockingShips();

                AnchoringShips();

                DockingShips();

                UnloadingShips();

                LoadingShips();

                InTransitShips();

                DateTime teset = currentTime.AddHours(-24);
                if (currentTime.Hour == 0)
                {
                    DayOverEvent.Invoke($"\nDay over\n Current Time: ", teset);
                    
                                                      
                    History.Add(new DailyLog(currentTime, harbor.Anchorage, harbor.GetShipsInTransit(), harbor.GetContainersStoredInHarbour(),
                        harbor.GetShipsInLoadingDock(), harbor.GetShipsInShipDock()));
                   

                        foreach (Ship ship in harbor.AllShips)
                        {
                            
                            foreach (StatusLog his in ship.HistoryIList)
                            {

                                if (his.PointInTime >= teset && his.PointInTime <= currentTime)
                                {

                                    if (his.Status == Status.Transit)
                                    {
                                        ShipStatusEvent?.Invoke($"{ship.Name} in transit");
                                    }
                                    else
                                        
                                        ShipStatusEvent?.Invoke($"ShipName: {ship.Name}| Date: {his.PointInTime}| Status: {his.Status}|\n");
                                    
                                }
                            }
                        }
                        Console.WriteLine("----------------------------");
                    
                }

                currentTime = currentTime.AddHours(1);

                continue;
            }
            SimulationEnd?.Invoke($"\n----------------\nSimulation over!\n----------------\n");
            Thread.Sleep(1000);

            return History;

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
            List<Ship> Anchorage = harbor.Anchorage.ToList();

            foreach (Ship ship in Anchorage)
            {
                
                Guid shipID = ship.ID;
                StatusLog lastStatusLog = ship.HistoryIList.Last(); 


                if (!ship.HasBeenAlteredThisHour && lastStatusLog != null && lastStatusLog.Status == Status.Anchoring)
                {

                    ship.HasBeenAlteredThisHour = true;

                    ship.AddStatusChangeToHistory(currentTime, harbor.AnchorageID, Status.Anchored);
                    
                    shipAnchored?.Invoke(ship);

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

                    if (currentTime == startTime && lastStatusLog.Status == Status.DockedToShipDock)
                    {
                        
                        dockID = harbor.DockShipFromShipDockToLoadingDock(ship.ID, currentTime);

                        ship.AddStatusChangeToHistory(currentTime, dockID, Status.DockingToLoadingDock);
                        ShipDockingtoLoadingDock?.Invoke(ship);

                    }
                }
            }

            foreach (Ship ship in ShipsInLoadingDock)
            {
                Guid shipID = ship.ID;
                StatusLog lastStatusLog = ship.HistoryIList.Last();

                if (!ship.HasBeenAlteredThisHour && lastStatusLog != null &&
                    (lastStatusLog.Status == Status.DockedToShipDock ||
                    lastStatusLog.Status == Status.DockingToLoadingDock ||
                    (lastStatusLog.Status == Status.UnloadingDone && (ship.IsForASingleTrip == true && ContainsTransitStatus(ship)))))
                {

                    if (lastStatusLog.Status == Status.DockingToLoadingDock && (currentTime - lastStatusLog.PointInTime).TotalHours >= 1)
                    {
                        Guid dockID = lastStatusLog.SubjectLocation;

                        ship.AddStatusChangeToHistory(currentTime, dockID, Status.DockedToLoadingDock);
                        shipDockedShipDock?.Invoke(ship);
                        if (ship.IsForASingleTrip && !ContainsTransitStatus(ship))
                        {
                            ship.AddStatusChangeToHistory(currentTime, dockID, Status.Loading);
                            shipLoadingContainer.Invoke(ship);
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

                            ship.AddStatusChangeToHistory(currentTime, dockID, Status.DockingToLoadingDock);
                            ShipDockingtoLoadingDock?.Invoke(ship);
                        }

                        if (harbor.FreeShipDockExists(ship.ShipSize) && ship.IsForASingleTrip == true && ContainsTransitStatus(ship)
                            && ship.ContainersOnBoard.Count == 0 && currentTime != startTime
                            && lastStatusLog.Status != Status.DockingToShipDock)
                        {
                            dockID = harbor.DockShipToShipDock(shipID);
                            ship.AddStatusChangeToHistory(currentTime, dockID, Status.DockingToShipDock);
                            ShipDockingtoLoadingDock?.Invoke(ship);

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
            foreach (Ship ship in harbor.shipsInLoadingDock.Keys)
            {
                StatusLog lastStatusLog = ship.HistoryIList.Last();


                if (!ship.HasBeenAlteredThisHour && lastStatusLog != null && (lastStatusLog.Status == Status.Unloading || lastStatusLog.Status == Status.DockedToLoadingDock))
                {
                    
                    Guid currentPosition = lastStatusLog.SubjectLocation;

                    StatusLog secondLastStatusLog = ship.HistoryIList[ship.HistoryIList.Count - 2];


                    if (ship.ContainersOnBoard.Count != 0 && lastStatusLog.Status == Status.DockedToLoadingDock || lastStatusLog.Status == Status.Unloading)
                    {
                        if (lastStatusLog.Status == Status.DockedToLoadingDock)
                            ship.AddStatusChangeToHistory(currentTime, currentPosition, Status.Unloading);
                            
                        for (int i = 0; i < ship.ContainersLoadedPerHour && ship.ContainersOnBoard.Count > 0; i++)
                        {
                            Container ContainerToBeUnloaded = ship.ContainersOnBoard.Last();

                            harbor.UnloadContainer(ContainerToBeUnloaded.Size, ship, currentTime);
                        }
                    }

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
        /// undock ship from harbor, and set status to Transit
        /// </summary>
        private void UndockingShips()
        {
            foreach (Ship ship in harbor.DockedShipsInLoadingDock())
            {
                Guid shipID = ship.ID;
                StatusLog lastStatusLog = ship.HistoryIList.Last();

                
                if (ship.HasBeenAlteredThisHour == false && lastStatusLog != null && 
                    (lastStatusLog.Status == Status.Undocking || lastStatusLog.Status == Status.LoadingDone && 
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

                    else if (lastStatusLog.Status == Status.LoadingDone)
                    {
                        ship.AddStatusChangeToHistory(currentTime, harbor.TransitLocationID, Status.Undocking);
                    }

                    else if (lastStatusLog.Status == Status.Undocking && (currentTime - lastStatusLog.PointInTime).TotalHours >= 1)
                    {
                        harbor.UnDockShipFromLoadingDockToTransit(shipID, currentTime);
                        ship.AddStatusChangeToHistory(currentTime, harbor.TransitLocationID, Status.Transit);
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

                if (!ship.HasBeenAlteredThisHour && lastStatusLog != null &&
                    ((lastStatusLog.Status == Status.UnloadingDone && (ship.IsForASingleTrip != true)) ||
                     (lastStatusLog.Status == Status.UnloadingDone && (ship.IsForASingleTrip == true && !ContainsTransitStatus(ship))) ||
                     (lastStatusLog.Status == Status.Loading) ||
                     (lastStatusLog.Status == Status.DockedToLoadingDock && secondLastStatusLog != null && secondLastStatusLog.Status == Status.DockedToShipDock) &&
                     (ship.IsForASingleTrip == true && !ContainsTransitStatus(ship))))
                {
                    Guid currentPosition = lastStatusLog.SubjectLocation;

                    if (!ContainsTransitStatus(ship) && ship.ContainersOnBoard.Count < ship.ContainerCapacity)
                    {
                        if (harbor.storedContainers.Keys.Count != 0 && ship.ContainersOnBoard.Count < ship.ContainerCapacity)
                        {
                           
                            for (int i = 0; i < ship.ContainersLoadedPerHour; i++)
                            {
                                
                                
                                if (ship.ContainersOnBoard.Count == 0 || ship.ContainersOnBoard.Last().Size == ContainerSize.Large)
                                {
                                    if (ship.CurrentWeightInTonn + (int)ContainerSize.Small <= ship.MaxWeightInTonn && ship.ContainersOnBoard.Count < ship.ContainerCapacity)
                                    {
                                        harbor.LoadContainer(ContainerSize.Small, ship, currentTime);
                                    }
                                }
                                
                                else if (ship.ContainersOnBoard.Last().Size == ContainerSize.Small)
                                {

                                    if (ship.CurrentWeightInTonn + (int)ContainerSize.Medium <= ship.MaxWeightInTonn && ship.ContainersOnBoard.Count < ship.ContainerCapacity)
                                    {
                                        harbor.LoadContainer(ContainerSize.Medium, ship, currentTime);
                                    }
                                }

                                else if (ship.ContainersOnBoard.Last().Size == ContainerSize.Medium)
                                {
                                    if (ship.CurrentWeightInTonn + (int)ContainerSize.Large <= ship.MaxWeightInTonn && ship.ContainersOnBoard.Count < ship.ContainerCapacity)
                                    {
                                        harbor.LoadContainer(ContainerSize.Large, ship, currentTime);
                                    }
                                }
                            }

                            if (lastStatusLog.Status != Status.Loading)
                            {
                                ship.AddStatusChangeToHistory(currentTime, currentPosition, Status.Loading);
                                shipLoadingContainer.Invoke(ship);
                            }

                        }

                        else
                        {
                            ship.AddStatusChangeToHistory(currentTime, currentPosition, Status.LoadingDone);
                            ship.AddStatusChangeToHistory(currentTime, currentPosition, Status.Undocking);
                            ShipUnDocked.Invoke(ship);
                        }

                    }
                    else
                    {
                     
                        ship.AddStatusChangeToHistory(currentTime, currentPosition, Status.LoadingDone);
                        ship.AddStatusChangeToHistory(currentTime, harbor.TransitLocationID, Status.Undocking);
                        ShipUnDocked.Invoke(ship);

                    }

                    ship.HasBeenAlteredThisHour = true;

                }
            }
        }

        /// <summary>
        /// Returns ships in transit to harbor.
        /// </summary>
        private void InTransitShips()
        {
            foreach (Ship ship in harbor.ShipsInTransit.Keys)
            {

                StatusLog lastStatusLog = ship.HistoryIList.Last();

                if (ship.HasBeenAlteredThisHour == false && lastStatusLog != null && lastStatusLog.Status == Status.Transit)
                {

                    Guid CurrentPosition = lastStatusLog.SubjectLocation;
                    StatusLog LastHistoryStatusLog = ship.HistoryIList.Last();

                    double DaysSinceTransitStart = (currentTime - LastHistoryStatusLog.PointInTime).TotalDays;

                    if (DaysSinceTransitStart >= ship.RoundTripInDays)
                    {
                        harbor.AddNewShipToAnchorage(ship);
                        ship.AddStatusChangeToHistory(currentTime, CurrentPosition, Status.Anchoring);
                        shipAnchoring?.Invoke(ship);
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
            } else { return "";  }
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

}