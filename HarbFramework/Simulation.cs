using harbNet;
using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace HarbFramework
{
    public class Simulation : ISimulation
    {
        //StartTime variabler 
        private DateTime startTime;
        private DateTime currentTime;
        private DateTime endTime;

        private Harbor harbor;

        public IList<Log> History {  get; private set; } = new List<Log>();


        public Simulation (Harbor harbor, DateTime simulationStartTime, DateTime simulationEndTime)
        {
            this.harbor = harbor;
            this.startTime = simulationStartTime;
            this.endTime = simulationEndTime;
        }


        public IList<Log> Run()
        {
            this.currentTime = startTime;
            History.Add(new Log(currentTime, harbor.Anchorage, harbor.GetShipsInTransit(), harbor.GetContainersStoredInHarbour(), harbor.GetShipsInLoadingDock(),harbor.GetShipsInShipDock()));

            Console.WriteLine("Simulation starting ...");
            Thread.Sleep(2000);

            while (currentTime < endTime)
            {

                foreach (Ship ship in harbor.AllShips)
                {
                    if (currentTime == startTime)
                        if (ship.IsForASingleTrip == true)
                        {
                            DateTime time = currentTime;
                            Guid shipDock = harbor.StartShipInShipDock(ship.ID);
                            harbor.AddContainersToHarbor(ship.ContainerCapacity, time);

                            ship.AddHistoryEvent(currentTime, shipDock, Status.DockedToShipDock);
                        }

                        else
                            ship.AddHistoryEvent(currentTime, harbor.AnchorageID, Status.Anchoring);
                }


                // Resetter nextStepCheck for alle Ship før neste runde
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
                    History.Add(new Log(currentTime, harbor.Anchorage, harbor.GetShipsInTransit(), harbor.GetContainersStoredInHarbour(), harbor.GetShipsInLoadingDock(), harbor.GetShipsInShipDock()));
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
                                        Console.WriteLine(ship.ShipName + " in transit");
                                    }
                                    else

                                        Console.WriteLine($"ShipName: {ship.ShipName}| Date: {his.PointInTime}| Status: {his.Status}|\n");
                                }



                            }

                        }
                        Console.WriteLine("----------------------------");
                    }
                }

                // Runden er over
                currentTime = currentTime.AddHours(1);

                continue;
            }

            Console.WriteLine("\n----------------");
            Console.WriteLine("Simulation over!");
            Console.WriteLine("----------------\n");
            Thread.Sleep(1000);

            return History;

        }

        private void AnchoringShips()
        {

            // Uten lokal Anchorage fungerer ikke foreach, fordi endringer blir gjort i ShipsInHarbourQueInn
            // mens man går gjennom den (skip blir tatt ut mens foreach går gjennom)
            List<Ship> Anchorage = harbor.Anchorage.ToList();

            foreach (Ship ship in Anchorage)
            {

                Guid shipID = ship.ID;
                Event lastEvent = ship.History.Last(); // Finner siste Event i history, så skipet siste status kan sjekkes


                if (!ship.HasBeenAlteredThisHour && lastEvent != null && lastEvent.Status == Status.Anchoring)
                {

                    harbor.AddNewShipToAnchorage(ship);
                    ship.HasBeenAlteredThisHour = true;

                    ship.AddHistoryEvent(currentTime, harbor.AnchorageID, Status.Anchored);
                    

                }
            }
        }

        private void DockingShips()
        {

            // Uten lokal Anchorage fungerer ikke foreach, fordi endringer blir gjort i ShipsInHarbourQueInn
            // mens man går gjennom den (skip blir tatt ut mens foreach går gjennom)
            List<Ship> Anchorage = harbor.Anchorage.ToList();

            foreach (Ship ship in Anchorage)
            {

                Guid shipID = ship.ID;
                Event lastEvent = ship.History.Last(); // Finner siste Event i history, så skipet siste status kan sjekkes


                if (!ship.HasBeenAlteredThisHour && lastEvent != null && 
                    (lastEvent.Status == Status.Anchored || 
                    lastEvent.Status == Status.DockedToShipDock || 
                    lastEvent.Status == Status.DockingToLoadingDock ||
                    lastEvent.Status == Status.DockingToShipDock ||
                    (lastEvent.Status == Status.UnloadingDone && (ship.IsForASingleTrip == true && ContainsTransitStatus(ship))))) 
                {

                    Guid dockID;

                    // Hvis dette er første runde, og skipet er docket på ShipDock som startposisjon
                    if (currentTime == startTime && lastEvent.Status == Status.DockedToShipDock)
                    {
                        dockID = harbor.DockShipFromShipDockToLoadingDock(ship.ID, currentTime);

                        ship.AddHistoryEvent(currentTime, dockID, Status.DockingToLoadingDock);

                    }

                    

                    if (harbor.FreeLoadingDockExists(ship.ShipSize) && lastEvent.Status != Status.DockedToShipDock)
                    {


                        // Alle skip kommer fra Anchored og begynner docking til loading dock
                        if (lastEvent.Status == Status.Anchored)
                        {
                            dockID = harbor.DockShipToLoadingDock(shipID, currentTime);
                            
                            ship.AddHistoryEvent(currentTime, dockID, Status.DockingToLoadingDock);
                        } 

                        // Alle skip er kommet til Loading dock, ferdig med docking
                        if (lastEvent.Status == Status.DockingToLoadingDock && (currentTime - lastEvent.PointInTime).TotalHours >= 1)
                        {
                            dockID = lastEvent.SubjectLocation;

                            ship.AddHistoryEvent(currentTime, dockID, Status.DockedToLoadingDock);
                            ship.AddHistoryEvent(currentTime, dockID, Status.Unloading);
                        }

                        //OneTime skip etter 
                        if (harbor.FreeShipDockExists(ship.ShipSize) && ship.IsForASingleTrip == true && ContainsTransitStatus(ship)
                            && ship.ContainersOnBoard.Count == 0 && currentTime != startTime
                            && lastEvent.Status != Status.DockingToShipDock)
                        {
                            dockID = harbor.DockShipToShipDock(shipID);
                            ship.AddHistoryEvent(currentTime, dockID, Status.DockingToShipDock);
                        
                        }

                        if (lastEvent.Status == Status.DockingToShipDock && (currentTime - lastEvent.PointInTime).TotalHours >= 1)
                        {
                            dockID = lastEvent.SubjectLocation;
                            ship.AddHistoryEvent(currentTime, dockID, Status.DockedToShipDock);
                        }

                    }

                ship.HasBeenAlteredThisHour = true;


                }
            }
        }

        private void UnloadingShips()
        {
            foreach (Ship ship in harbor.shipsInLoadingDock.Keys)
            {
                Event lastEvent = ship.History.Last(); // Finner siste Event i history, så skipet siste status kan sjekkes


                if (!ship.HasBeenAlteredThisHour && lastEvent != null && (lastEvent.Status == Status.Unloading || lastEvent.Status == Status.DockedToLoadingDock))
                {
                    Guid currentPosition = lastEvent.SubjectLocation;

                    Event secondLastEvent = ship.History[ship.History.Count - 2]; // event før lastEvent

                    if (ship.ContainersOnBoard.Count != 0)
                    {
                        for (int i = 0; i < ship.ContainersLoadedPerHour && ship.ContainersOnBoard.Count > 0; i++)
                        {
                            Container ContainerToBeUnloaded = ship.ContainersOnBoard.Last(); // Logisk riktig rekkefølge

                            harbor.UnloadContainer(ContainerToBeUnloaded.Size, ship, currentTime);
                        }
                    }

                    if (secondLastEvent.Status == Status.DockedToShipDock)
                    {
                        ship.AddHistoryEvent(currentTime, currentPosition, Status.Loading);
                    }

                    else if (lastEvent.Status == Status.DockedToLoadingDock)
                    {
                        ship.AddHistoryEvent(currentTime, currentPosition, Status.Unloading);
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

        private void UndockingShips()
        {
            foreach (Ship ship in harbor.DockedShipsInLoadingDock())
            {
                Guid shipID = ship.ID;
                Event lastEvent = ship.History.Last(); // Finner siste Event i history, så skipet siste status kan sjekkes

                
                if (ship.HasBeenAlteredThisHour == false && lastEvent != null && (lastEvent.Status == Status.Undocking || lastEvent.Status == Status.LoadingDone && (lastEvent.Status == Status.UnloadingDone && ContainsTransitStatus(ship))))
                {
                    bool containsTransitStatus = ContainsTransitStatus(ship);

                    if (ship.IsForASingleTrip == true && containsTransitStatus)
                    {
                        Guid dockID = harbor.DockShipToShipDock(ship.ID);

                        ship.AddHistoryEvent(currentTime, dockID, Status.DockingToShipDock);

                    }

                    harbor.UnDockShipFromLoadingDockToTransit(shipID, currentTime);



                    if (lastEvent.Status == Status.LoadingDone)
                    {
                        ship.AddHistoryEvent(currentTime, harbor.TransitLocationID, Status.Undocking);
                    }

                    ship.AddHistoryEvent(currentTime, harbor.TransitLocationID, Status.Transit);

                    


                    ship.HasBeenAlteredThisHour = true;

                }
            }
        }

        private static bool ContainsTransitStatus(Ship ship)
        {
            bool containsTransitStatus = false;
            foreach (Event his in ship.History)
            {
                if (his.Status == Status.Transit)
                {
                    containsTransitStatus = true;
                    break; // Avslutter løkken tidlig siden vi har funnet det vi leter etter
                }
            }

            return containsTransitStatus;
        }

        private void LoadingShips()
        {
            foreach (Ship ship in harbor.DockedShipsInLoadingDock())
            {

                Event lastEvent = ship.History.Last(); // Finner siste Event i history, så skipet siste status kan sjekkes
                Event secondLastEvent = ship.History[ship.History.Count - 2]; // event før lastEvent

                if (!ship.HasBeenAlteredThisHour && lastEvent != null &&
                    ((lastEvent.Status == Status.UnloadingDone && (ship.IsForASingleTrip != true)) ||
                     (lastEvent.Status == Status.UnloadingDone && (ship.IsForASingleTrip == true && !ContainsTransitStatus(ship))) ||
                     (lastEvent.Status == Status.Loading) ||
                     (lastEvent.Status == Status.DockingToLoadingDock && secondLastEvent != null && secondLastEvent.Status == Status.DockedToShipDock) &&
                     (ship.IsForASingleTrip != true || !ContainsTransitStatus(ship))))
                {
                    Guid currentPosition = lastEvent.SubjectLocation;


                    // Try loading containers
                    if (!ContainsTransitStatus(ship) && ship.ContainersOnBoard.Count < ship.ContainerCapacity)
                    {
                            if (harbor.storedContainers.Keys.Count != 0)
                            { 
                                for (int i = 0; i < ship.ContainersLoadedPerHour; i++)
                                {
                                    // Try loading small container
                                    if (ship.ContainersOnBoard.Count == 0 || ship.ContainersOnBoard.Last().Size == ContainerSize.Large)
                                    {
                                        if (ship.CurrentWeightInTonn + (int)ContainerSize.Small <= ship.MaxWeightInTonn)
                                        {
                                            harbor.LoadContainer(ContainerSize.Small, ship, currentTime);
                                        }
                                    }

                                    // Try loading medium container
                                    else if (ship.ContainersOnBoard.Last().Size == ContainerSize.Small)
                                    {

                                        if (ship.CurrentWeightInTonn + (int)ContainerSize.Medium <= ship.MaxWeightInTonn)
                                        {
                                            harbor.LoadContainer(ContainerSize.Medium, ship, currentTime);
                                        }
                                    }

                                    // Try loading large container
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
                            ship.AddHistoryEvent(currentTime, harbor.TransitLocationID, Status.Undocking);
                        }


                    }
                    else
                    {
                        // Hvis ingen container passer til max vekten, sier seg ferdig
                        ship.AddHistoryEvent(currentTime, currentPosition, Status.LoadingDone);
                        ship.AddHistoryEvent(currentTime, harbor.TransitLocationID, Status.Undocking);
                        
                    }

                    ship.HasBeenAlteredThisHour = true;

                }
            }
        }



        private void InTransitShips()
        {
            foreach (Ship ship in harbor.ShipsInTransit.Keys)
            {

                Event lastEvent = ship.History.Last(); // Finner siste Event i history, så skipet siste status kan sjekkes

                if (ship.HasBeenAlteredThisHour == false && lastEvent != null && lastEvent.Status == Status.Transit)
                {

                    Guid CurrentPosition = lastEvent.SubjectLocation;
                    Event LastHistoryEvent = ship.History.Last();

                    double DaysSinceTransitStart = (currentTime - LastHistoryEvent.PointInTime).TotalDays;

                    // Hvis roundTripInDays er større eller lik dager siden transit begynte (dagens dato - når eventet skjedde = dager siden eventet skjedde)
                    if (DaysSinceTransitStart >= ship.RoundTripInDays)
                    {
                        ship.AddHistoryEvent(currentTime, CurrentPosition, Status.Anchoring);
                        harbor.AddNewShipToAnchorage(ship);
                        
                    }

                    ship.HasBeenAlteredThisHour = true;

                }
            }
        }
    }

}
    //currentTime
    //StartOfDay
    //EndTime variabler
    //opprett Harbour objekt (opprett kontainere til havn)
    //opprett båter (opprett containere ombord)
    //ArrayList Logs = new ArrayList()

    //legg til en initiell logfil til historie som logger starttilstanden til simuleringen.

    //while(Ikke sluttdato){
    //En runde i while løkken er en time
    //Undocke båter som er ferdig med lasting  (Lag en foreach som går igjennom alle båter i havna og ser om de er ferdig med jobben. hvis de er det undock båten og sett nextStepCheck til True)
    //Sjekk hvor mange havner av de forskjellige størelsene er ledig (For løkke som går igjennom ledige havner og teller opp antallet av de forskjellige størelsene som er ledig)
    //Docke båter fra HarbourQueueInn (For løkke som looper like mange ganger som antal skip av de forskjellige størrelsene som skal dokkes og bruker harbor funksjonen for å docke skipene hvis de er det undock båten og sett nextStepCheck til True)
    //Laste av containere fra båten / Laste containere til båter. (For løkke som går gjennom alle skipene til kai og finner ut om de skal laste av eller på containere og laster av/på så mange containere som skipet har mulighet til basert på berthing time)
    //Setter alle nextStepCheck til false hos alle objekter (For løkke som går gjennom alle skip i simuleringen og setter next step check til false)
    //oppdaterer current time med +1 time}
    //Current time == Start of day + 24 hours => Lag log fil