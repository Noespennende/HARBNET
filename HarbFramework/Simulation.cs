using harbNet;
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

namespace HarbFramework
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

        public IList<Log> History {  get; private set; } = new List<Log>();

        /// <summary>
        /// Simulation constructor.
        /// </summary>
        /// <param name="harbor">A harbor object</param>
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
        /// <returns>returns the history of the simulation</returns>
        public IList<Log> Run()
        {
            this.currentTime = startTime;
            

            Console.WriteLine("Simulation starting ...");
            Thread.Sleep(2000);

            while (currentTime < endTime)
            {

                if (currentTime == startTime)
                {

                    foreach (Ship ship in harbor.AllShips)
                    {
                        if (ship.IsForASingleTrip == true)
                        {
                            DateTime time = currentTime;
                            Guid shipDock = harbor.StartShipInShipDock(ship.ID);
                            harbor.AddContainersToHarbor(ship.ContainerCapacity, time);

                            ship.AddHistoryEvent(currentTime, shipDock, Status.DockedToShipDock);
                        }
                        else
                        {
                            ship.AddHistoryEvent(currentTime, harbor.AnchorageID, Status.Anchoring);
                        }

                        History.Add(new Log(currentTime, DuplicateShipList(harbor.Anchorage), DuplicateShipList(harbor.GetShipsInTransit()), DuplicateContainerList(harbor.GetContainersStoredInHarbour()),
                            DuplicateShipList(harbor.GetShipsInLoadingDock()), DuplicateShipList(harbor.GetShipsInShipDock())));
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
                    Console.WriteLine("\nDay over");
                    Console.WriteLine("Current time: " + currentTime);

                    

                    History.Add(new Log(currentTime, DuplicateShipList(harbor.Anchorage), DuplicateShipList(harbor.GetShipsInTransit()), DuplicateContainerList(harbor.GetContainersStoredInHarbour()),
                        DuplicateShipList(harbor.GetShipsInLoadingDock()), DuplicateShipList(harbor.GetShipsInShipDock())));
                    if (currentTime.Hour == 0)
                    {

                        foreach (Ship ship in harbor.AllShips)
                        {

                            foreach (Event his in ship.History)
                            {

                                if (his.PointInTime >= teset && his.PointInTime <= currentTime)
                                {
 
                                    if (his.Status == Status.Transit)
                                    {
                                        Console.WriteLine(ship.Name + " in transit");
                                    }
                                    else
                                        Console.WriteLine($"ShipName: {ship.Name}| Date: {his.PointInTime}| Status: {his.Status}|\n");
                                }
                            }
                        }
                        Console.WriteLine("----------------------------");
                    }
                }

                currentTime = currentTime.AddHours(1);

                continue;
            }

            Console.WriteLine("\n----------------");
            Console.WriteLine("Simulation over!");
            Console.WriteLine("----------------\n");
            Thread.Sleep(1000);

            return History;

        }

       

        /// <summary>
        /// Print history for each ship .
        /// </summary>
        public void PrintShipHistory()
        {
            foreach (Log log in History)
            {
                log.PrintInfoForAllShips();
            }
        }

        /// <summary>
        /// Printing conatiner history for each container.
        /// </summary>
        public void PrintContainerHistory()
        {
            foreach (Log log in History)
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
                Event lastEvent = ship.History.Last(); 


                if (!ship.HasBeenAlteredThisHour && lastEvent != null && lastEvent.Status == Status.Anchoring)
                {

                    ship.HasBeenAlteredThisHour = true;

                    ship.AddHistoryEvent(currentTime, harbor.AnchorageID, Status.Anchored);
                    

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
                Event lastEvent = ship.History.Last();

                if (!ship.HasBeenAlteredThisHour && lastEvent != null &&
                    (lastEvent.Status == Status.Anchored ||
                    lastEvent.Status == Status.DockedToShipDock ||
                    lastEvent.Status == Status.DockingToLoadingDock ||
                    (lastEvent.Status == Status.UnloadingDone && (ship.IsForASingleTrip == true && ContainsTransitStatus(ship)))))
                {
                        Guid dockID;

                    if (currentTime == startTime && lastEvent.Status == Status.DockedToShipDock)
                    {
                        
                        dockID = harbor.DockShipFromShipDockToLoadingDock(ship.ID, currentTime);

                        ship.AddHistoryEvent(currentTime, dockID, Status.DockingToLoadingDock);

                    }
                }
            }

            foreach (Ship ship in ShipsInLoadingDock)
            {
                Guid shipID = ship.ID;
                Event lastEvent = ship.History.Last();

                if (!ship.HasBeenAlteredThisHour && lastEvent != null &&
                    (lastEvent.Status == Status.DockedToShipDock ||
                    lastEvent.Status == Status.DockingToLoadingDock ||
                    (lastEvent.Status == Status.UnloadingDone && (ship.IsForASingleTrip == true && ContainsTransitStatus(ship)))))
                {

                    if (lastEvent.Status == Status.DockingToLoadingDock && (currentTime - lastEvent.PointInTime).TotalHours >= 1)
                    {
                        Guid dockID = lastEvent.SubjectLocation;

                        ship.AddHistoryEvent(currentTime, dockID, Status.DockedToLoadingDock);
                        if (ship.IsForASingleTrip && !ContainsTransitStatus(ship))
                        {
                            ship.AddHistoryEvent(currentTime, dockID, Status.Loading);
                        }
                        else
                        {
                            ship.AddHistoryEvent(currentTime, dockID, Status.Unloading);
                        }
                    }
                }
            }

            foreach (Ship ship in Anchorage)
            {

                Guid shipID = ship.ID;
                Event lastEvent = ship.History.Last();

                if (!ship.HasBeenAlteredThisHour && lastEvent != null && 
                    (lastEvent.Status == Status.Anchored || 
                    lastEvent.Status == Status.DockedToShipDock || 
                    lastEvent.Status == Status.DockingToLoadingDock ||
                    lastEvent.Status == Status.DockingToShipDock ||
                    (lastEvent.Status == Status.UnloadingDone && (ship.IsForASingleTrip == true && ContainsTransitStatus(ship))))) 
                {
                    Guid dockID;

                    if (harbor.FreeLoadingDockExists(ship.ShipSize) && lastEvent.Status != Status.DockedToShipDock)
                    {

                        if (lastEvent.Status == Status.Anchored)
                        {
                            dockID = harbor.DockShipToLoadingDock(shipID, currentTime);

                            ship.AddHistoryEvent(currentTime, dockID, Status.DockingToLoadingDock);
                        }

                        if (harbor.FreeShipDockExists(ship.ShipSize) && ship.IsForASingleTrip == true && ContainsTransitStatus(ship)
                            && ship.ContainersOnBoard.Count == 0 && currentTime != startTime
                            && lastEvent.Status != Status.DockingToShipDock)
                        {
                            dockID = harbor.DockShipToShipDock(shipID);
                            ship.AddHistoryEvent(currentTime, dockID, Status.DockingToShipDock);
                        
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
                Event lastEvent = ship.History.Last();


                if (!ship.HasBeenAlteredThisHour && lastEvent != null && (lastEvent.Status == Status.Unloading || lastEvent.Status == Status.DockedToLoadingDock))
                {
                    Guid currentPosition = lastEvent.SubjectLocation;

                    Event secondLastEvent = ship.History[ship.History.Count - 2];


                    if (ship.ContainersOnBoard.Count != 0 && lastEvent.Status == Status.DockedToLoadingDock || lastEvent.Status == Status.Unloading)
                    {
                        if (lastEvent.Status == Status.DockedToLoadingDock)
                            ship.AddHistoryEvent(currentTime, currentPosition, Status.Unloading);

                        for (int i = 0; i < ship.ContainersLoadedPerHour && ship.ContainersOnBoard.Count > 0; i++)
                        {
                            Container ContainerToBeUnloaded = ship.ContainersOnBoard.Last();

                            harbor.UnloadContainer(ContainerToBeUnloaded.Size, ship, currentTime);
                        }
                    }

                    if (secondLastEvent.Status == Status.DockedToShipDock)
                    {
                        ship.AddHistoryEvent(currentTime, currentPosition, Status.Loading);
                    }

                    else if (ship.ContainersOnBoard.Count == 0 && !(ship.IsForASingleTrip == true && ContainsTransitStatus(ship)))
                    {
                        ship.AddHistoryEvent(currentTime, currentPosition, Status.UnloadingDone);
                       
                        ship.AddHistoryEvent(currentTime, currentPosition, Status.Loading);
                    }
                    else if (ship.ContainersOnBoard.Count == 0 && (ship.IsForASingleTrip == true && ContainsTransitStatus(ship)))
                    {
                        ship.AddHistoryEvent(currentTime, currentPosition, Status.UnloadingDone);
                        
                        ship.AddHistoryEvent(currentTime, currentPosition, Status.DockingToShipDock);
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
                Event lastEvent = ship.History.Last();

                
                if (ship.HasBeenAlteredThisHour == false && lastEvent != null && 
                    (lastEvent.Status == Status.Undocking || lastEvent.Status == Status.LoadingDone && 
                    (lastEvent.Status == Status.UnloadingDone && ContainsTransitStatus(ship)) || lastEvent.Status == Status.DockingToShipDock))
                {
                    bool containsTransitStatus = ContainsTransitStatus(ship);

                    if (ship.IsForASingleTrip == true && containsTransitStatus && lastEvent.Status != Status.DockingToShipDock)
                    {
                        Guid dockID = lastEvent.SubjectLocation;
                        ship.AddHistoryEvent(currentTime, dockID, Status.DockingToShipDock);

                    }

                    else if (lastEvent.Status == Status.DockingToShipDock && (currentTime - lastEvent.PointInTime).TotalHours >= 1)
                    {
                        Guid dockID = harbor.DockShipToShipDock(ship.ID);
                        ship.AddHistoryEvent(currentTime, dockID, Status.DockedToShipDock);
                    }

                    else if (lastEvent.Status == Status.LoadingDone)
                    {
                        ship.AddHistoryEvent(currentTime, harbor.TransitLocationID, Status.Undocking);
                    }

                    else if (lastEvent.Status == Status.Undocking && (currentTime - lastEvent.PointInTime).TotalHours >= 1)
                    {
                        harbor.UnDockShipFromLoadingDockToTransit(shipID, currentTime);
                        ship.AddHistoryEvent(currentTime, harbor.TransitLocationID, Status.Transit);
                    }



                    ship.HasBeenAlteredThisHour = true;

                }
            }
        }
        /// <summary>
        /// check to see if the ship has the Status "Transit".
        /// </summary>
        /// <param name="ship"> A ship object</param>
        /// <returns></returns>
        private static bool ContainsTransitStatus(Ship ship)
        {
            bool containsTransitStatus = false;
            foreach (Event his in ship.History)
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
        /// Loading ship with containers 
        /// </summary>
        private void LoadingShips()
        {
            foreach (Ship ship in harbor.DockedShipsInLoadingDock())
            {

                Event lastEvent = ship.History.Last();
                Event secondLastEvent = ship.History[ship.History.Count - 2]; 

                if (!ship.HasBeenAlteredThisHour && lastEvent != null &&
                    ((lastEvent.Status == Status.UnloadingDone && (ship.IsForASingleTrip != true)) ||
                     (lastEvent.Status == Status.UnloadingDone && (ship.IsForASingleTrip == true && !ContainsTransitStatus(ship))) ||
                     (lastEvent.Status == Status.Loading) ||
                     (lastEvent.Status == Status.DockedToLoadingDock && secondLastEvent != null && secondLastEvent.Status == Status.DockedToShipDock) &&
                     (ship.IsForASingleTrip == true && !ContainsTransitStatus(ship))))
                {
                    Guid currentPosition = lastEvent.SubjectLocation;

                    if (!ContainsTransitStatus(ship) && ship.ContainersOnBoard.Count < ship.ContainerCapacity)
                    {
                        if (harbor.storedContainers.Keys.Count != 0)
                        {
                            for (int i = 0; i < ship.ContainersLoadedPerHour; i++)
                            {
                                
                                if (ship.ContainersOnBoard.Count == 0 || ship.ContainersOnBoard.Last().Size == ContainerSize.Large)
                                {
                                    if (ship.CurrentWeightInTonn + (int)ContainerSize.Small <= ship.MaxWeightInTonn)
                                    {
                                        harbor.LoadContainer(ContainerSize.Small, ship, currentTime);
                                    }
                                }
                                
                                else if (ship.ContainersOnBoard.Last().Size == ContainerSize.Small)
                                {

                                    if (ship.CurrentWeightInTonn + (int)ContainerSize.Medium <= ship.MaxWeightInTonn)
                                    {
                                        harbor.LoadContainer(ContainerSize.Medium, ship, currentTime);
                                    }
                                }

                                else if (ship.ContainersOnBoard.Last().Size == ContainerSize.Medium)
                                {
                                    if (ship.CurrentWeightInTonn + (int)ContainerSize.Large <= ship.MaxWeightInTonn)
                                    {
                                        harbor.LoadContainer(ContainerSize.Large, ship, currentTime);
                                    }
                                }
                            }

                            if (lastEvent.Status != Status.Loading)
                            {
                                ship.AddHistoryEvent(currentTime, currentPosition, Status.Loading);
                            }

                        }

                        else
                        {
                            ship.AddHistoryEvent(currentTime, currentPosition, Status.LoadingDone);
                            ship.AddHistoryEvent(currentTime, currentPosition, Status.Undocking);
                        }

                    }
                    else
                    {
                     
                        ship.AddHistoryEvent(currentTime, currentPosition, Status.LoadingDone);
                        ship.AddHistoryEvent(currentTime, harbor.TransitLocationID, Status.Undocking);
                        
                    }

                    ship.HasBeenAlteredThisHour = true;

                }
            }
        }

        /// <summary>
        /// Lets ships in transit return to harbor
        /// </summary>
        private void InTransitShips()
        {
            foreach (Ship ship in harbor.ShipsInTransit.Keys)
            {

                Event lastEvent = ship.History.Last();

                if (ship.HasBeenAlteredThisHour == false && lastEvent != null && lastEvent.Status == Status.Transit)
                {

                    Guid CurrentPosition = lastEvent.SubjectLocation;
                    Event LastHistoryEvent = ship.History.Last();

                    double DaysSinceTransitStart = (currentTime - LastHistoryEvent.PointInTime).TotalDays;

                    if (DaysSinceTransitStart >= ship.RoundTripInDays)
                    {
                        harbor.AddNewShipToAnchorage(ship);
                        ship.AddHistoryEvent(currentTime, CurrentPosition, Status.Anchoring);
                    }

                    ship.HasBeenAlteredThisHour = true;

                }
            }
        }
        /// <summary>
        /// Duplicate a shipList
        /// </summary>
        /// <param name="shipListToDuplicate">list to duplicate</param>
        /// <returns>a duplicated shiplist</returns>
        private IList<Ship> DuplicateShipList (IList<Ship> shipListToDuplicate)
        {
            IList<Ship> duplicatedList = new List<Ship>();

            foreach (Ship ship in shipListToDuplicate)
            {
                IList<Container> containerList = new List<Container>();
                IList<Event> eventList = new List<Event>();

                foreach (Container container in ship.ContainersOnBoard)
                {
                    IList<Event> containersHistory = new List<Event>();
                    foreach (Event containerEvent in container.History)
                    {
                        containersHistory.Add(new Event(containerEvent.Subject, containerEvent.SubjectLocation, containerEvent.PointInTime, containerEvent.Status));
                    }
                    containerList.Add(new Container(container.Size, container.WeightInTonn, ship.ID, container.ID, containersHistory));
                }
                foreach (Event eventObject in ship.History)
                {
                    eventList.Add(new Event(eventObject.Subject, eventObject.SubjectLocation, eventObject.PointInTime, eventObject.Status));
                }

                duplicatedList.Add(new Ship(ship.Name, ship.ShipSize, ship.StartDate, ship.IsForASingleTrip, ship.RoundTripInDays, ship.ID, containerList, eventList));
            }
            return duplicatedList;
        }

        /// <summary>
        /// Duplicate a shipList
        /// </summary>
        /// <param name="containersToDuplicate">list to duplicate</param>
        /// <returns>a duplicated containerlist</returns>
        private IList<Container> DuplicateContainerList (IList<Container> containersToDuplicate)
        {
            IList<Container> duplicatedList = new List<Container>();

            foreach (Container container in containersToDuplicate)
            {
                IList<Event> eventList = new List<Event>();
                foreach (Event containerEvent in container.History)
                {
                    eventList.Add(new Event(containerEvent.Subject, containerEvent.SubjectLocation, containerEvent.PointInTime, containerEvent.Status));
                }
                duplicatedList.Add(new Container(container.Size, container.WeightInTonn, container.CurrentPosition, container.ID, eventList));
            }
            return duplicatedList;
        }
        
    }

}