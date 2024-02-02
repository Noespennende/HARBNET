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

            // Setup(); <--- Ikke behov for lenger, nå når bruker setter opp Harbor og Ship og sender inn i Simulation()

            int round = 1;
            while (round < 5)
            {

                foreach (Ship ship in harbor.AllShips)
                {
                    if (round == 1)
                        ship.AddHistoryEvent(currentTime, harbor.HarbourQueInnID, Status.Docking);
                }

                // Resetter nextStepCheck for alle Ship før neste runde
                foreach (Ship ship in harbor.AllShips) {
                    ship.NextStepCheck = false;
                    
                }


                ArrayList harborQueInn = harbor.HarbourQueInn; // Lag en referanse til ArrayList

                List<Ship> copyOfHarbourQueInn = new List<Ship>(); // Opprett en ny List<Ship>

                // Kopier elementene fra ArrayList til List<Ship>
                foreach (var item in harborQueInn)
                {
                    if (item is Ship ship)
                    {
                        copyOfHarbourQueInn.Add(ship);
                    }
                }


                UndockingShips();

                DockingShips();

                UnloadingShips();

                LoadingShips();

                







                // Slutt på "runde"
                Console.WriteLine("\nRound " + round + " over");
                Console.WriteLine("----------------------------");
                round += 1; // Får bli enige om hvor mange timer en "runde" skal ta, hvordan vi skal kalkulere runder med startTime, currentTime og endTime etc.
                continue;
            }


        }

        private void DockingShips()
        {

            Console.WriteLine("\n** DockingShips **");
            ArrayList shipsToDock = new ArrayList(harbor.HarbourQueInn);

            int counter = 0;

            foreach (Ship ship in shipsToDock)
            {

                Guid shipID = ship.ID;
                Event? @event = ship.History.Count > 0 ? ship.History[^1] as Event : null; // Finner siste Event i history, så skipet siste status kan sjekkes


                if (!ship.NextStepCheck && @event != null && @event.Status == Status.Docking)
                {
                    Guid dockID = harbor.DockShip(shipID, currentTime);
                    ship.NextStepCheck = true;

                    ship.AddHistoryEvent(currentTime, dockID, Status.Unloading);

                    counter += 1;
                    
                }
            }
            Console.WriteLine("Number of dockings: " + counter);
            Console.WriteLine("Docking successful!");
        }

        private void UnloadingShips()
        {
            Console.WriteLine("\n** UnloadingShips **");
            foreach (Ship ship in harbor.ShipsInDock.Keys)
            {
                
                Guid shipID = ship.ID;
                Event? @event = ship.History.Count > 0 ? ship.History[^1] as Event : null; // Finner siste Event i history, så skipet siste status kan sjekkes

                if (!ship.NextStepCheck && @event != null && @event.Status == Status.Unloading)
                {
                    
                    Guid currentPosition = @event.SubjectLocation;

                    // !! OBS OBS !! 
                    // Mangler mer detaljert logikk for UnloadingShips() her.

                    // Akkurat prøves det å unloade en av hver størrelse - kanskje noen mer elegant måte å vite hvilke størrelser som er på skipet,
                    // og så kanskje randomisere hvilke som blir unloadet?

                        harbor.UnloadContainer(ContainerSize.Small, ship, currentTime);
                    
                        harbor.UnloadContainer(ContainerSize.Medium, ship, currentTime);
                    
                        harbor.UnloadContainer(ContainerSize.Large, ship, currentTime);


                    ship.NextStepCheck = true;

                    ship.AddHistoryEvent(currentTime, currentPosition, Status.Loading);

                    Console.WriteLine("Stored containers: " + harbor.StoredContainers.Count);
                    Console.WriteLine("Containers on ship: " + ship.ContainersOnBoard.Count);
                    Console.WriteLine("Unloading successful!");

                }
            }
        }

        private void UndockingShips()
        {
            Console.WriteLine("\n** UndockingShips **");
            int counter = 0;
            foreach (Ship ship in harbor.DockedShips())
            {
                Guid shipID = ship.ID;
                Event? @event = ship.History.Count > 0 ? ship.History[^1] as Event : null; // Finner siste Event i history, så skipet siste status kan sjekkes

                if (ship.NextStepCheck == false && @event != null && @event.Status == Status.Undocking)
                {
                    harbor.UnDockShip(shipID, currentTime);
                    ship.NextStepCheck = true;
                    // Opprette Event her ....
                    counter += 1;
                    
                }
            }
            Console.WriteLine("Number of undockings: " + counter);
            Console.WriteLine("Undocking successful!");
        }

        private void LoadingShips()
        {
            Console.WriteLine("\n** LoadingShips **");
            foreach (Ship ship in harbor.DockedShips())
            {

                Guid shipID = ship.ID;
                Event? @event = ship.History.Count > 0 ? ship.History[^1] as Event : null; // Finner siste Event i history, så skipet siste status kan sjekkes

                if (ship.NextStepCheck == false && @event != null && @event.Status == Status.Loading)
                {

                    Guid currentPosition = @event.SubjectLocation;

                    ContainerSize containerSmall = ContainerSize.Small;
                    ContainerSize containerMedium = ContainerSize.Medium;
                    ContainerSize containerLarge = ContainerSize.Large;

 
                    if (ship.ContainersOnBoard.Count != ship.ContainerCapacity) // Hvis det er fri plass til container på skipet
                    {
                        Console.WriteLine("Containers onboard ship before: " + ship.ContainersOnBoard.Count);

                        harbor.LoadContainer(containerSmall, ship, currentTime);
                        harbor.LoadContainer(containerMedium, ship, currentTime);
                        harbor.LoadContainer(containerLarge, ship, currentTime);

                        Console.WriteLine("Containers onboard ship after: " + ship.ContainersOnBoard.Count);
                    }
                    else if (ship.ContainersOnBoard.Count == ship.ContainerCapacity)
                    {
                        Status status = Status.Undocking; // Her burde det kanskje være en mellomting? "Venter på å undocke". Den undocker jo egentlig ikke før neste "runde"
                    }
                    ship.NextStepCheck = true;

                    if (ship.ContainersOnBoard.Count == ship.ContainerCapacity)
                        ship.AddHistoryEvent(currentTime, currentPosition, Status.Undocking);
                    else
                    {
                        ship.AddHistoryEvent(currentTime, currentPosition, Status.Undocking); 
                        // Obs obs ! - Status her må settes til Unloading når harbor har masse containers
                        // til å fylle opp skipet med.
                    }

                    Console.WriteLine("Loading successful!");

                    // Opprette Event her .... - ta i bruk status variabelen over
                }
            }
        }


        private void Setup()
        {

            // Harbor oppretter ContainerSpaces internt basert på parameterne

            int totalSmallHarborContainerSpaces = harbor.AllContainerSpaces[ContainerSize.Small].Count;
            int totalMediumHarborContainerSpaces = harbor.AllContainerSpaces[ContainerSize.Medium].Count;
            int totalLargeHarborContainerSpaces = harbor.AllContainerSpaces[ContainerSize.Large].Count;
            int totalContainersInSimulation = CalcuateTotalContainersInSimulation(); ;

            int CalcuateTotalContainersInSimulation()
            {
                int totalHarborContainerCapacity = totalSmallHarborContainerSpaces + totalMediumHarborContainerSpaces + totalLargeHarborContainerSpaces;
                int totalShipContainerCapcacity = 0;
                
               
                foreach (Ship ship in allShipsInSimulation)
                {
                    totalShipContainerCapcacity += ship.ContainerCapacity;
                }

                
                int totalContainersInSimulation = totalHarborContainerCapacity + totalShipContainerCapcacity;
                // Skal vi legge til et tilfeldig antall containere som ikke har plass på verken skip eller harbor?
                // Ser for meg at dette kan garantere at før eller siden så må et skip må vente med å unloade til et annet har loadet?

                return totalContainersInSimulation;
            }
            
            List<ContainerSpace> allContainerSpaces = new List<ContainerSpace>();

            ArrayList tempContainersOnBoard = new ArrayList();
          
            Ship tempShipS = new(ShipSize.Small, startTime, 10, tempContainersOnBoard.Count);

            // Oppretter og fyller Harbor med containere
            for (int i = 0; i < totalContainersInSimulation/3; i++)
            {
                tempContainersOnBoard.Add(new Container(ContainerSize.Small, 10000, Guid.Empty));  // Legge til en Small container, på temp Ship
                tempContainersOnBoard.Add(new Container(ContainerSize.Large, 25000, Guid.Empty));  // Legge til en Medium container, på temp Ship
                tempContainersOnBoard.Add(new Container(ContainerSize.Small, 50000, Guid.Empty));  // Legge til en Large container, på temp Ship

                harbor.UnloadContainer(ContainerSize.Small, tempShipS, currentTime); // Fyller så Harbor opp med containere
                // Dette kan nok gjøres på en annen og mer direkte måte, men for nå gjorde jeg det slik :) - Mathilde
            }
        }
        // Mathilde slutt



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