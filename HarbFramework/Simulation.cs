using harbNet;
using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HarbFramework
{
    public class Simulation
    {
        //StartTime variabler 
        private DateTime startTime;
        private DateTime currentTime;
        private DateTime endTime;

        private ArrayList containersOnBoard = new ArrayList();
        private ArrayList allDocks = new ArrayList();
        private ArrayList freeDocks = new ArrayList();
        private Hashtable shipsInDock = new Hashtable();
        private Harbor harbor;
        private Ship shipSmall, shipMedium, shipLarge;
        public ICollection<Log> history {  get; private set; }
        //private Log log = new Log()

        // Mathilde - Jeg la det her for nå
        private ArrayList allShipsInSimulation = new ArrayList();

        public Simulation (Harbor harbor, DateTime simulationStartTime, DateTime simulationEndTime)
        {
            this.harbor = harbor;
            this.startTime = simulationStartTime;
            this.endTime = simulationEndTime;
        }


        public void Run(DateTime startTime, DateTime endTime)
        {
            this.endTime = endTime;
            this.currentTime = startTime;

            while (currentTime < endTime)
            {

                foreach (Ship ship in harbor.AllShips)
                {
                    if (currentTime == startTime)
                        ship.AddHistoryEvent(currentTime, harbor.HarbourQueInnID, Status.Docking);
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


                if (currentTime.Hour >= 0 && currentTime.Hour <= 3)
                {
                    /* !! Skriv ut dagslogger her !! */

                    // Dette kan reworkers/slettes
                    Console.WriteLine("\nDay over");
                    Console.WriteLine("Current time: " + currentTime);
                    Console.WriteLine("----------------------------");
                }

                currentTime = currentTime.AddHours(4);

                continue;
            }



        }

        private void DockingShips()
        {

            // Uten lokal HarbourQueInn fungerer ikke foreach, fordi endringer blir gjort i ShipsInHarbourQueInn
            // mens man går gjennom den (skip blir tatt ut mens foreach går gjennom)
            ArrayList ShipsInHarbourQueInn = new ArrayList(harbor.HarbourQueInn);

            foreach (Ship ship in ShipsInHarbourQueInn)
            {

                Guid shipID = ship.ID;
                Event lastEvent = ship.History.Last(); // Finner siste Event i history, så skipet siste status kan sjekkes


                if (!ship.NextStepCheck && lastEvent != null && (lastEvent.Status == Status.Docking || lastEvent.Status == Status.Queuing))
                {
                    Guid dockID = harbor.DockShip(shipID, currentTime);
                    ship.NextStepCheck = true;

                    ship.AddHistoryEvent(currentTime, dockID, Status.Unloading);
                    Console.WriteLine("\n" + ship.ID + " docked");
                    Console.WriteLine("Docking successful!");

                }
            }
        }

        private void UnloadingShips()
        {
            foreach (Ship ship in harbor.ShipsInDock.Keys)
            {
                Event lastEvent = ship.History.Last(); // Finner siste Event i history, så skipet siste status kan sjekkes

                if (!ship.NextStepCheck && lastEvent != null && lastEvent.Status == Status.Unloading)
                {
                    Guid currentPosition = lastEvent.SubjectLocation;

                    if (ship.ContainersOnBoard.Count != 0)
                    {
                        Container ContainerToBeUnloaded = ship.ContainersOnBoard.Last(); // Logisk riktig rekkefølge

                        harbor.UnloadContainer(ContainerToBeUnloaded.Size, ship, currentTime);
                        // Console.WriteLine("Stored containers: " + harbor.storedContainers.Count);
                        // Console.WriteLine("Containers on ship: " + ship.containersOnBoard.Count);
                    }

                    if (ship.ContainersOnBoard.Count == 0)
                    {
                        ship.AddHistoryEvent(currentTime, currentPosition, Status.UnloadingDone);
                        Console.WriteLine("Unloading successful for " + ship.ID + "!");
                    }

                    ship.NextStepCheck = true;

                }
            }
        }

        private void UndockingShips()
        {
            foreach (Ship ship in harbor.DockedShips())
            {
                Guid shipID = ship.ID;
                Event lastEvent = ship.History.Last(); // Finner siste Event i history, så skipet siste status kan sjekkes


                if (ship.NextStepCheck == false && lastEvent != null && (lastEvent.Status == Status.Undocking || lastEvent.Status == Status.LoadingDone))
                {
                    harbor.UnDockShip(shipID, currentTime);

                    ship.AddHistoryEvent(currentTime, harbor.TransitLocationID, Status.Transit);

                    Console.WriteLine("\nUndocking successful!");
                    Console.WriteLine(ship.ID + " in transit!");


                    ship.NextStepCheck = true;

                }
            }
        }

        private void LoadingShips()
        {
            foreach (Ship ship in harbor.DockedShips())
            {

                Event lastEvent = ship.History.Last(); // Finner siste Event i history, så skipet siste status kan sjekkes

                if (ship.NextStepCheck == false && lastEvent != null && (lastEvent.Status == Status.Loading || lastEvent.Status == Status.UnloadingDone))
                {
                    Guid currentPosition = lastEvent.SubjectLocation;

                    // Try loading containers
                    if (ship.ContainersOnBoard.Count < ship.ContainerCapacity)
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
                        // Hvis ingen container passer til max vekten, sier seg ferdig
                        else
                        {
                            ship.AddHistoryEvent(currentTime, currentPosition, Status.LoadingDone);
                            Console.WriteLine("\nLoading successful for " + ship.ID + "!");
                            Console.WriteLine("Max weight reached, finishing loading");
                        }
                    }

                    else if (ship.ContainersOnBoard.Count == ship.ContainerCapacity
                        || ship.CurrentWeightInTonn == ship.MaxWeightInTonn
                        || harbor.StoredContainers.Keys.Count == 0)
                    {
                        ship.AddHistoryEvent(currentTime, currentPosition, Status.LoadingDone);
                        Console.WriteLine("\nLoading successful for " + ship.ID + "!");
                    }

                    ship.NextStepCheck = true;

                }
            }
        }

        private void InTransitShips()
        {
            foreach (Ship ship in harbor.ShipsInTransit.Keys)
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
                        ship.AddHistoryEvent(currentTime, CurrentPosition, Status.Queuing);
                        harbor.AddNewShipToHarbourQueue(ship);
                        Console.WriteLine("\nTransit done for " + ship.ID);
                        Console.WriteLine("Now in Queue for docking");
                    }

                    ship.NextStepCheck = true;

                }
            }
        }



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
             Dictionary<ContainerSize, List<ContainerSpace>> allContainerSpaces = harbor.AllContainerSpaces;
             Dictionary<ContainerSize, List<ContainerSpace>> freeContainerSpaces = harbor.FreeContainerSpaces;
             Dictionary<ContainerSize, List<ContainerSpace>> storedContainerSpaces = new();




             shipSmall = new Ship(ShipSize.Small, currentTime, 12, containersOnBoard.Count);
             shipMedium = new Ship(ShipSize.Medium, currentTime, 12, containersOnBoard.Count);
             shipLarge = new Ship(ShipSize.Large, currentTime, 12, containersOnBoard.Count);

             //eventlogger ikke implementert
             while (endTime != currentTime)
             {

                 foreach (Ship ship in harbor.ShipsInDock) //undock
                 {
                     if (ship.NextStepCheck == false)
                     {
                         if (ship.ContainersOnBoard.Count == 0)
                         {
                             harbor.UnDockShip(ship.ID, currentTime);
                             shipsInDock.Remove(ship.ID);
                             harbor.NumberOfFreeDocks(ship.ShipSize); //usikker på denne, om den skal legge til på den måten og om neste løkke burde endres isåfall


                             ship.NextStepCheck = true;

                         }
                         else
                         {
                             break;
                         }
                     }
                 }

                 if (harbor.FreeDockExists(ShipSize.Small) || harbor.FreeDockExists(ShipSize.Medium) || harbor.FreeDockExists(ShipSize.Large))
                     //dock har ikke tatt hensyn til forskjellige størrelser
                 {
                     foreach (Ship ship in harbor.HarbourQueInn)
                     {
                         if (harbor.FreeDockExists(ship.ShipSize) && !ship.NextStepCheck)
                         {//første parameter usikker
                             foreach (Dock dock in harbor.FreeDocks)
                             {
                                 if (dock.Size.Equals(ship.ShipSize))
                                 {
                                     harbor.RemoveShipFromQueue(ship.ID);
                                     harbor.DockShip(ship.ID, currentTime);
                                     harbor.RemoveDockFromFreeDocks(dock.ID);
                                     ship.NextStepCheck = true;
                                     break;
                                 }
                                 else
                                 {
                                     break;
                                 }
                             }
                         }
                     }
                 }

                 foreach (Ship ship in harbor.ShipsInDock)//laste kontainer på skip
                 {
                     if (ship.NextStepCheck)
                     {
                         break;
                     }
                     else
                     {
                         //akkurat nå er det en if test per container size, kan kanskje endres
                         if (ship.ContainerCapacity < ship.GetNumberOfContainersOnBoard(ContainerSize.Small) && ship.CurrentWeightInTonn < ship.CurrentWeightInTonn)
                         {

                             harbor.LoadContainer(ContainerSize.Small, ship, currentTime);
                             ship.NextStepCheck = true;
                         }
                         else if (ship.ContainerCapacity < ship.GetNumberOfContainersOnBoard(ContainerSize.Medium) && ship.CurrentWeightInTonn < ship.CurrentWeightInTonn)
                         {
                             harbor.LoadContainer(ContainerSize.Medium, ship, currentTime);
                             ship.NextStepCheck = true;
                         }
                         else if (ship.ContainerCapacity < ship.GetNumberOfContainersOnBoard(ContainerSize.Large) && ship.CurrentWeightInTonn < ship.CurrentWeightInTonn)
                         {
                             // ?
                         }

                     }
                 }

                 foreach (Ship shipflag in harbor.ShipsInDock)//akkurat nå er det bare skipene som er i dock som endrer nextstepcheck
                 {
                     shipflag.NextStepCheck = true;
                 }

                 currentTime.AddHours(6); // Får bli enige om hvor mange timer en "runde" skal ta
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