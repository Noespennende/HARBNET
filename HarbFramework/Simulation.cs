﻿using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public EventHandler simulationEnd;
        public EventHandler simulationStart;
        public EventHandler DayEnded;
        public EventHandler DayLoggedToSimulationHistory;
        public EventHandler ShipUnDocked;
        public EventHandler shipDockingToShipDock;
        public EventHandler shipDockedShipDock;
        public EventHandler ShipDockingtoLoadingDock;
        public EventHandler ShipDockedtoLoadingDock;
        public EventHandler shipLoadedContainer;
        public EventHandler shipUnloadedContainer;
        public EventHandler shipAnchored;
        public EventHandler shipAnchoring;

        /*
        public delegate void SimulationEndedHandler(Object sender, EventArgs e);
        public event SimulationEndedHandler SimulationEnded;

        public delegate void DayOverEventHandler(String message, DateTime currentTime);
        public event DayOverEventHandler? DayEndede;

        public delegate void DayLoggedEventHandler(Ship ship, StatusLog todaysLog);
        public event DayLoggedEventHandler? DayLoggedToSimulationHistorye;

        public delegate void ShipUndockedHandler(Ship ship);
        public event ShipUndockedHandler? ShipUnDockede;

        public delegate void shipDockedToShipDockHandler(Ship ship);
        public event shipDockedToShipDockHandler? shipDockedShipDocke;

        public delegate void shipDockingToLoadingDockHandler(Ship ship);
        public event shipDockingToLoadingDockHandler? ShipDockingtoLoadingDocke;

        public delegate void shipDockedToLoadingDockHandler(Ship ship);
        public event shipDockedToLoadingDockHandler? ShipDockedtoLoadingDocke;

        public delegate void shipLoadedContainerHandler(Ship ship, Container container);
        public event shipLoadedContainerHandler? shipLoadeadContainer;

        public delegate void shipUnloadedContainerHandler(Ship ship, Container container);
        public event shipUnloadedContainerHandler? shipUnloadedContainere;

        public delegate void shipAnchoredHandler(Ship ship);
        public event shipAnchoredHandler? shipAnchoreda;

        public delegate void shipAnchoringHandler(Ship ship);
        public event shipAnchoringHandler? shipAnchoringa;
        */
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
            this.currentTime = startTime;

            shipAnchoringEventArgs ev = new shipAnchoringEventArgs();

            Console.WriteLine("Simulation starting ...");
            Thread.Sleep(2000);

            while (currentTime < endTime)
            {

                foreach (Ship ship in harbor.AllShips)
                {
                    ev.ship = ship;
                    ev.currentTime = currentTime;

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
                            ev.anchorageID = harbor.AnchorageID;
                            shipAnchoring.Invoke(this, ev);
                        }

                        History.Add(new DailyLog(currentTime, harbor.Anchorage, harbor.GetShipsInTransit(), harbor.GetContainersStoredInHarbour(),
                            harbor.GetShipsInLoadingDock(), harbor.GetShipsInShipDock()));
                    }
                }



                foreach (Ship ship in harbor.AllShips)
                {
                    ship.HasBeenAlteredThisHour = false;

                }

                UndockingShips();

                AnchoringShips();

                DockingShips();

                UnloadingShips();

                LoadingShips();

                InTransitShips();

                DayLoggedEventArgs e = new DayLoggedEventArgs();
                e.currentTime = currentTime;
                e.message = $"\nDay over\n Current Time: ";
                //e.todaysLog = 

                DateTime teset = currentTime.AddHours(-24);
                if (currentTime.Hour == 0)
                {
                    DayEnded.Invoke(this, e);


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
                                    DayLoggedToSimulationHistory.Invoke(this, e);

                                }
                                else
                                    e.message = $"ShipName: {ship.Name}| Date: {his.PointInTime}| Status: {his.Status}|\n";
                                    DayLoggedToSimulationHistory.Invoke(this, e);

                            }
                        }
                    }
                    Console.WriteLine("----------------------------");

                }

                currentTime = currentTime.AddHours(1);

                continue;
            }
            SimulationEndedEventArgs ended = new SimulationEndedEventArgs();

            ended.message = $"\n----------------\nSimulation over!\n----------------\n";
            simulationEnd.Invoke(this, ended);
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
            shipAnchoredEventArgs e = new shipAnchoredEventArgs();
            List<Ship> Anchorage = harbor.Anchorage.ToList();

            foreach (Ship ship in Anchorage)
            {


                Guid shipID = ship.ID;
                StatusLog lastStatusLog = ship.HistoryIList.Last();
                e.ship = ship;
                e.currentTime = currentTime;
                e.message = lastStatusLog.ToString();

                if (!ship.HasBeenAlteredThisHour && lastStatusLog != null && lastStatusLog.Status == Status.Anchoring)
                {

                    ship.HasBeenAlteredThisHour = true;

                    ship.AddStatusChangeToHistory(currentTime, harbor.AnchorageID, Status.Anchored);

                    shipAnchored?.Invoke(this, e);

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
                    shipDockingToShipDockEventArgs e = new shipDockingToShipDockEventArgs();
                    e.ship = ship;
                    e.currentTime = currentTime;
                    if (currentTime == startTime && lastStatusLog.Status == Status.DockedToShipDock)
                    {

                        dockID = harbor.DockShipFromShipDockToLoadingDock(ship.ID, currentTime);
                        e.dockId = dockID;
                        ship.AddStatusChangeToHistory(currentTime, dockID, Status.DockingToLoadingDock);
                        ShipDockingtoLoadingDock?.Invoke(ship, e); ;

                    }
                }
            }

            foreach (Ship ship in ShipsInLoadingDock)
            {
                Guid shipID = ship.ID;
                StatusLog lastStatusLog = ship.HistoryIList.Last();
                shipDockedToShipDockEventArgs eventArgs = new shipDockedToShipDockEventArgs();


                if (!ship.HasBeenAlteredThisHour && lastStatusLog != null &&
                    (lastStatusLog.Status == Status.DockedToShipDock ||
                    lastStatusLog.Status == Status.DockingToLoadingDock ||
                    (lastStatusLog.Status == Status.UnloadingDone && (ship.IsForASingleTrip == true && ContainsTransitStatus(ship)))))
                {

                    if (lastStatusLog.Status == Status.DockingToLoadingDock && (currentTime - lastStatusLog.PointInTime).TotalHours >= 1)
                    {
                        Guid dockID = lastStatusLog.SubjectLocation;

                        ship.AddStatusChangeToHistory(currentTime, dockID, Status.DockedToLoadingDock);
                        eventArgs.dockId = dockID;
                        eventArgs.ship = ship;
                        eventArgs.currentTime = currentTime;
                        shipDockedShipDock.Invoke(this, eventArgs);
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
                shipDockingToShipDockEventArgs shipdockevent = new shipDockingToShipDockEventArgs();
                shipDockingToLoadingDockEventArgs shipLoaddockevent = new shipDockingToLoadingDockEventArgs();

                shipdockevent.ship = ship;
                shipdockevent.currentTime = currentTime;

                shipLoaddockevent.ship = ship;
                shipLoaddockevent.currentTime = currentTime;

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
                            shipdockevent.dockId = dockID;

                            ship.AddStatusChangeToHistory(currentTime, dockID, Status.DockingToLoadingDock);
                            ShipDockingtoLoadingDock?.Invoke(this, shipdockevent);
                        }

                        if (harbor.FreeShipDockExists(ship.ShipSize) && ship.IsForASingleTrip == true && ContainsTransitStatus(ship)
                            && ship.ContainersOnBoard.Count == 0 && currentTime != startTime
                            && lastStatusLog.Status != Status.DockingToShipDock)
                        {
                            dockID = harbor.DockShipToShipDock(shipID);
                            shipLoaddockevent.dockId = dockID;
                            ship.AddStatusChangeToHistory(currentTime, dockID, Status.DockingToShipDock);
                            ShipDockingtoLoadingDock?.Invoke(this, shipLoaddockevent);

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
                shipLoadedContainerEventArgs e = new shipLoadedContainerEventArgs();
                ShipUndockedEventArgs shipUndockedEventArgs = new ShipUndockedEventArgs();

                shipUndockedEventArgs.ship = ship;
                shipUndockedEventArgs.currentTime = currentTime;
                e.ship = ship;
                e.currentTime = currentTime;
                if (!ship.HasBeenAlteredThisHour && lastStatusLog != null &&
                    ((lastStatusLog.Status == Status.UnloadingDone && (ship.IsForASingleTrip != true)) ||
                     (lastStatusLog.Status == Status.UnloadingDone && (ship.IsForASingleTrip == true && !ContainsTransitStatus(ship))) ||
                     (lastStatusLog.Status == Status.Loading) ||
                     (lastStatusLog.Status == Status.DockedToLoadingDock && secondLastStatusLog != null && secondLastStatusLog.Status == Status.DockedToShipDock) &&
                     (ship.IsForASingleTrip == true && !ContainsTransitStatus(ship))))
                {
                    Guid currentPosition = lastStatusLog.SubjectLocation;

                    if (ship.ContainersOnBoard.Count < ship.ContainerCapacity && ship.CurrentWeightInTonn < ship.MaxWeightInTonn)
                    {
                        if (harbor.storedContainers.Keys.Count != 0 && ship.ContainersOnBoard.Count < ship.ContainerCapacity)
                        {

                            for (int i = 0; i < ship.ContainersLoadedPerHour; i++)
                            {


                                if (ship.ContainersOnBoard.Count == 0 || ship.ContainersOnBoard.Last().Size == ContainerSize.Large && harbor.GetStoredContainer(ContainerSize.Small) != null)
                                {
                                    if (ship.CurrentWeightInTonn + (int)ContainerSize.Small <= ship.MaxWeightInTonn && ship.ContainersOnBoard.Count < ship.ContainerCapacity)
                                    {
                                        harbor.LoadContainer(ContainerSize.Small, ship, currentTime);

                                    }
                                }

                                else if (ship.ContainersOnBoard.Last().Size == ContainerSize.Small && harbor.GetStoredContainer(ContainerSize.Medium) != null)
                                {

                                    if (ship.CurrentWeightInTonn + (int)ContainerSize.Medium <= ship.MaxWeightInTonn && ship.ContainersOnBoard.Count < ship.ContainerCapacity)
                                    {
                                        harbor.LoadContainer(ContainerSize.Medium, ship, currentTime);
                                    }
                                }

                                else if (ship.ContainersOnBoard.Last().Size == ContainerSize.Medium && harbor.GetStoredContainer(ContainerSize.Large) != null)
                                {
                                    if (ship.CurrentWeightInTonn + (int)ContainerSize.Large <= ship.MaxWeightInTonn && ship.ContainersOnBoard.Count < ship.ContainerCapacity)
                                    {
                                        harbor.LoadContainer(ContainerSize.Large, ship, currentTime);
                                    }
                                }

                                else
                                {
                                    ship.AddStatusChangeToHistory(currentTime, currentPosition, Status.LoadingDone);
                                    ship.AddStatusChangeToHistory(currentTime, currentPosition, Status.Undocking);
                                    ShipUnDocked?.Invoke(this, shipUndockedEventArgs);
                                    break;
                                }
                            }

                            if (lastStatusLog.Status != Status.Loading)
                            {
                                ship.AddStatusChangeToHistory(currentTime, currentPosition, Status.Loading);
                                //shipLoadeadContainer.Invoke(ship);
                            }

                        }

                        else
                        {
                            ship.AddStatusChangeToHistory(currentTime, currentPosition, Status.LoadingDone);
                            ship.AddStatusChangeToHistory(currentTime, currentPosition, Status.Undocking);
                            ShipUnDocked?.Invoke(this, shipUndockedEventArgs);
                        }

                    }
                    else
                    {

                        ship.AddStatusChangeToHistory(currentTime, currentPosition, Status.LoadingDone);
                        ship.AddStatusChangeToHistory(currentTime, harbor.TransitLocationID, Status.Undocking);
                        ShipUnDocked?.Invoke(this, shipUndockedEventArgs);

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
            shipAnchoringEventArgs e = new shipAnchoringEventArgs();
            foreach (Ship ship in harbor.ShipsInTransit.Keys)
            {
                e.ship = ship;
                StatusLog lastStatusLog = ship.HistoryIList.Last();

                if (ship.HasBeenAlteredThisHour == false && lastStatusLog != null && lastStatusLog.Status == Status.Transit)
                {

                    Guid CurrentPosition = lastStatusLog.SubjectLocation;
                    StatusLog LastHistoryStatusLog = ship.HistoryIList.Last();
                    e.currentTime = currentTime;
                    double DaysSinceTransitStart = (currentTime - LastHistoryStatusLog.PointInTime).TotalDays;

                    if (DaysSinceTransitStart >= ship.RoundTripInDays)
                    {
                        harbor.AddNewShipToAnchorage(ship);
                        ship.AddStatusChangeToHistory(currentTime, CurrentPosition, Status.Anchoring);

                        shipAnchoring?.Invoke(this, e);
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
    /// The EventArgs class for the ShipUndocked event.
    /// </summary>
    public class ShipUndockedEventArgs : EventArgs
    {
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
    /// The EventArgs class for the shipDockingToShipDock event.
    /// </summary>
    public class shipDockingToShipDockEventArgs : EventArgs
    {
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
    /// The EventArgs class for the shipLoadedContainer event.
    /// </summary>
    public class shipLoadedContainerEventArgs : EventArgs
    {
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
    /// The EventArgs class for the shipUnloadedContainer event.
    /// </summary>
    public class shipUnloadedContainerEventArgs : EventArgs
    {
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
    /// The EventArgs class for the shipAnchored event.
    /// </summary>
    public class shipAnchoredEventArgs : EventArgs
    {
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
    /// The EventArgs class for the shipAnchoring event.
    /// </summary>
    public class shipAnchoringEventArgs : EventArgs
    {
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

