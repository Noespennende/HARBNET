using harbNet;
using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
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


        public void Run()
        {
            Log harborLog = new Log();
            this.endTime = endTime;
            this.currentTime = startTime;

            while (currentTime < endTime)
            {

                foreach (Ship ship in harbor.AllShips)
                {
                    if (currentTime == startTime)
                        ship.AddHistoryEvent(currentTime, harbor.AnchorageID, Status.DockingToLoadingDock);
                }


                // Resetter nextStepCheck for alle Ship før neste runde
                foreach (Ship ship in harbor.AllShips) {
                    ship.NextStepCheck = false;
                    
                }

                UndockingShips();

                DockingShips();

                UnloadingShips();

                LoadingShips();

                InTransitShips();





                /* !! Skriv ut dagslogger her !! */

                // Dette kan reworkers/slettes
                if (currentTime.Hour == 0){
                    Console.WriteLine("\nDay over");
                    Console.WriteLine("Current time: " + currentTime);
                }
                    if (currentTime.Hour == 0)
                    {

                        foreach (Ship ship in harbor.AllShips)
                        {
                            Console.WriteLine("-------------------------------------------------------------------------------");
                            Console.WriteLine($"History ShipId: {ship.ID} Shipname: {ship.ShipName}");
                            Console.WriteLine("-------------------------------------------------------------------------------");

                        foreach (Event his in ship.History)
                            {
                            
                                Console.WriteLine($"ShipName: {ship.ShipName} Date: {his.PointInTime} Status: {his.Status} ---{his.SubjectLocation}\n");
                            
                            }
                           
                        }
                        Console.WriteLine("----------------------------");
                    }

                        currentTime = currentTime.AddHours(4);

                        continue;
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


                if (!ship.NextStepCheck && lastEvent != null && (lastEvent.Status == Status.DockingToLoadingDock || lastEvent.Status == Status.Anchoring)) 
                {
                    Guid dockID = harbor.DockShipToLoadingDock(shipID, currentTime);
                    ship.NextStepCheck = true;
                    

                    ship.AddHistoryEvent(currentTime, dockID, Status.Unloading);
                    Console.WriteLine("\n" + ship.ID + " docked");
                    Console.WriteLine("Docking successful!");

                }
            }
        }

        private void UnloadingShips()
        {
            foreach (Ship ship in harbor.shipsInLoadingDock.Keys)
            {
                Event lastEvent = ship.History.Last(); // Finner siste Event i history, så skipet siste status kan sjekkes

                if (!ship.NextStepCheck && lastEvent != null && lastEvent.Status == Status.Unloading)
                {
                    Guid currentPosition = lastEvent.SubjectLocation;

                    if (ship.ContainersOnBoard.Count != 0)
                    {
                        for (int i = 0; i < ship.ContainersLoadedPerHour && ship.ContainersOnBoard.Count > 0; i++)
                        {
                            Container ContainerToBeUnloaded = ship.ContainersOnBoard.Last(); // Logisk riktig rekkefølge

                            harbor.UnloadContainer(ContainerToBeUnloaded.Size, ship, currentTime);
                            // Console.WriteLine("Stored containers: " + harbor.storedContainers.Count);
                            // Console.WriteLine("Containers on ship: " + ship.containersOnBoard.Count);
                        }
                    }

                    if (ship.ContainersOnBoard.Count == 0)
                    {
                        foreach(Container container in ship.ContainersOnBoard)
                        {
                            currentTime = currentTime.AddHours(10);
                        }
                        ship.AddHistoryEvent(currentTime, currentPosition, Status.UnloadingDone);
                        Console.WriteLine("Unloading successful for " + ship.ID + "!");
                    }

                    ship.NextStepCheck = true;

                }
            }
        }

        private void UndockingShips()
        {
            foreach (Ship ship in harbor.DockedShipsInLoadingDock())
            {
                Guid shipID = ship.ID;
                Event lastEvent = ship.History.Last(); // Finner siste Event i history, så skipet siste status kan sjekkes

                
                if (ship.NextStepCheck == false && lastEvent != null && (lastEvent.Status == Status.Undocking || lastEvent.Status == Status.LoadingDone))
                {

                    harbor.UnDockShipFromLoadingDockToTransit(shipID, currentTime);

                    ship.AddHistoryEvent(currentTime, harbor.TransitLocationID, Status.Transit);

                    Console.WriteLine("\nUndocking successful!");
                    Console.WriteLine(ship.ID + " in transit!");


                    ship.NextStepCheck = true;

                }
            }
        }

        private void LoadingShips()
        {
            foreach (Ship ship in harbor.DockedShipsInLoadingDock())
            {

                Event lastEvent = ship.History.Last(); // Finner siste Event i history, så skipet siste status kan sjekkes

                if (ship.NextStepCheck == false && lastEvent != null && (lastEvent.Status == Status.Loading || lastEvent.Status == Status.UnloadingDone))
                {

                    Guid currentPosition = lastEvent.SubjectLocation;


                    // Try loading containers
                    if (ship.ContainersOnBoard.Count < ship.ContainerCapacity)
                    {

                        if (ship.ContainersOnBoard.Count != ship.ContainerCapacity
                        || ship.CurrentWeightInTonn != ship.MaxWeightInTonn
                        || harbor.storedContainers.Keys.Count != 0)
                        {
                            for (int i = 0; i < 5; i++)
                            {
                                // Try loading small container
                                if (ship.ContainersOnBoard.Count == 0 || ship.ContainersOnBoard.Last().Size == ContainerSize.Large)
                                {
                                    if (ship.CurrentWeightInTonn + (int)ContainerSize.Small <= ship.MaxWeightInTonn)
                                    {
                                        harbor.LoadContainer(ContainerSize.Small, ship, currentTime);
                                        // Console.WriteLine("Loading Small");
                                    }
                                }

                                // Try loading medium container
                                else if (ship.ContainersOnBoard.Last().Size == ContainerSize.Small)
                                {

                                    if (ship.CurrentWeightInTonn + (int)ContainerSize.Medium <= ship.MaxWeightInTonn)
                                    {
                                        harbor.LoadContainer(ContainerSize.Medium, ship, currentTime);
                                        // Console.WriteLine("Loading Medium");
                                    }
                                }

                                // Try loading large container
                                else if (ship.ContainersOnBoard.Last().Size == ContainerSize.Medium)
                                {
                                    if (ship.CurrentWeightInTonn + (int)ContainerSize.Large <= ship.MaxWeightInTonn)
                                    {
                                        harbor.LoadContainer(ContainerSize.Large, ship, currentTime);
                                        // Console.WriteLine("Loading Large");
                                    }
                                }
                            }

                            if (lastEvent.Status != Status.Loading)
                            {
                                ship.AddHistoryEvent(currentTime, currentPosition, Status.Loading);
                            }


                        }
                        else if (ship.ContainersOnBoard.Count == ship.ContainerCapacity
                            || ship.CurrentWeightInTonn == ship.MaxWeightInTonn
                            || harbor.storedContainers.Keys.Count == 0)
                        {
                            ship.AddHistoryEvent(currentTime, currentPosition, Status.LoadingDone);
                            Console.WriteLine("\nLoading successful for " + ship.ID + "!");
                        }


                    }
                    else
                    {

                        // Hvis ingen container passer til max vekten, sier seg ferdig
                        ship.AddHistoryEvent(currentTime, currentPosition, Status.LoadingDone);
                        Console.WriteLine("\nLoading successful for " + ship.ID + "!");
                        Console.WriteLine("Max weight reached, finishing loading");
                    }

                    ship.NextStepCheck = true;




                }
            }
        }






        private void InTransitShips()
        {
            foreach (Ship ship in harbor.shipsInTransit.Keys)
            {

                Event lastEvent = ship.History.Last(); // Finner siste Event i history, så skipet siste status kan sjekkes

                // Console.WriteLine("Test " + lastEvent.status);

                if (ship.NextStepCheck == false && lastEvent != null && lastEvent.Status == Status.Transit)
                {

                    Guid CurrentPosition = lastEvent.SubjectLocation;
                    Event LastHistoryEvent = ship.History.Last();

                    double DaysSinceTransitStart = (currentTime - LastHistoryEvent.PointInTime).TotalDays;

                    // Hvis roundTripInDays er større eller lik dager siden transit begynte (dagens dato - når eventet skjedde = dager siden eventet skjedde)
                    if (DaysSinceTransitStart >= ship.RoundTripInDays)
                    {
                        ship.AddHistoryEvent(currentTime, CurrentPosition, Status.Anchoring);
                        harbor.AddNewShipToHarbourQueue(ship);
                        Console.WriteLine("\nTransit done for " + ship.ID);
                        Console.WriteLine("Now in Queue for docking");
                    }

                    ship.NextStepCheck = true;

                }
            }
        }


        /*
         public Simulation()
         {

             Container containerSmall = new Container(ContainerSize.Small, 1000, Guid.Empty);
             Container containerMedium = new Container(ContainerSize.Medium, 2500, Guid.Empty);
             Container containerLarge = new Container(ContainerSize.Large, 5000, Guid.Empty);

             for (int i = 0; i < 50; i++)
             {
                 containersOnBoard.Add(containerSmall);
                 containersOnBoard.Add(containerMedium);
                 containersOnBoard.Add(containerLarge);
             }

             //harbor = new Harbor(10, 10, 10, 100, 100, 100);
             Dictionary<ContainerSize, List<ContainerSpace>> allContainerSpaces = harbor.allContainerSpaces;
             Dictionary<ContainerSize, List<ContainerSpace>> freeContainerSpaces = harbor.freeContainerSpaces;
             Dictionary<ContainerSize, List<ContainerSpace>> storedContainerSpaces = new();




             shipSmall = new Ship(ShipSize.Small, currentTime, 12, containersOnBoard.Count);
             shipMedium = new Ship(ShipSize.Medium, currentTime, 12, containersOnBoard.Count);
             shipLarge = new Ship(ShipSize.Large, currentTime, 12, containersOnBoard.Count);

             //eventlogger ikke implementert
            
         }*/
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