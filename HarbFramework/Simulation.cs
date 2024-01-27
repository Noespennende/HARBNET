using harbNet;
using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarbFramework
{
    internal class Simulation
    {
        //StartTime variabler 
        private DateTime startTime;
        private DateTime currentTime;
        private DateTime endTime;
        private int harborSmallCount = 0;
        private int harborMediumCount = 0;
        private int harborLargeCount = 0;
        private int containerSmallCount = 0;
        private int containerMediumCount = 0;
        private int containerLargeCount = 0;



        private ArrayList containersOnBoard = new ArrayList();
        private ArrayList allDocks = new ArrayList();
        private ArrayList freeDocks = new ArrayList();
        private Hashtable shipsInDock = new Hashtable();
        private Harbor harbor;
        private Ship shipSmall, shipMedium, shipLarge;
        //private Log log = new Log()


        // Mathilde - Jeg la det her for nå
        private ArrayList allShipsInSimulation = new ArrayList();
        public void Run()
        {
            Setup();

            while (currentTime != endTime)
            {
                // Resetter nextStepCheck for alle Ship før neste runde
                foreach (Ship ship in allShipsInSimulation)
                    ship.nextStepCheck = false; 

                UndockingShips();
                LoadingShips();


                // Slutt på "runde"

                currentTime.AddHours(6); // Får bli enige om hvor mange timer en "runde" skal ta
                continue;
            }


        }
        private void UndockingShips()
        {
            foreach (Ship ship in harbor.dockedShips())
            {

                Guid shipID = ship.id;
                Event? @event = ship.history.Count > 0 ? ship.history[^1] as Event : null; // Finner siste Event i history, så skipet siste status kan sjekkes

                if (@event != null && @event.status == Status.Undocking)
                {
                    harbor.unDockShip(shipID, currentTime);
                    ship.nextStepCheck = true;
                    // Opprette Event her ....
                }
            }
        }

        private void LoadingShips()
        {
            foreach (Ship ship in harbor.dockedShips())
            {
                Guid shipID = ship.id;
                Event? @event = ship.history.Count > 0 ? ship.history[^1] as Event : null; // Finner siste Event i history, så skipet siste status kan sjekkes

                if (ship.nextStepCheck = false && @event != null && @event.status == Status.Loading)
                {
                    ContainerSize ContainerSize;
                    if (ship.shipSize == ShipSize.Small)
                    {
                        ContainerSize = ContainerSize.Small;
                    }
                    else if (ship.shipSize == ShipSize.Medium)
                    {
                        ContainerSize = ContainerSize.Medium;
                    }
                    else if (ship.shipSize == ShipSize.Large)
                    {
                        ContainerSize = ContainerSize.Large;
                    }
                    else
                    {
                        ContainerSize = ContainerSize.Small;
                        // Håndtering hvis ingen av de andre if-ene blir kjørt ... ellers klager ContainerSize i loadContainer om at den ikke får noen verdi.
                        // ?? Noen bedre måte å håndtere dette på ??
                    }

                    if (ship.containersOnBoard.Count != ship.containerCapacity) // Hvis det er fri plass til container på skipet
                    {
                        harbor.loadContainer(ContainerSize, ship, currentTime);
                    }
                    else if (ship.containersOnBoard.Count == ship.containerCapacity)
                    {
                        Status status = Status.Undocking; // Her burde det kanskje være en mellomting? "Venter på å undocke". Den undocker jo egentlig ikke før neste "runde"
                    }
                    ship.nextStepCheck = true;

                    // Opprette Event her .... - ta i bruk status variabelen over
                }
            }
        }


        private void Setup()
        {

            // Harbor oppretter ContainerSpaces internt basert på parameterne
            Harbor harbor = new Harbor(10, 10, 10, 100, 100, 100);

            int totalSmallHarborContainerSpaces = harbor.allContainerSpaces[ContainerSize.Small].Count;
            int totalMediumHarborContainerSpaces = harbor.allContainerSpaces[ContainerSize.Medium].Count;
            int totalLargeHarborContainerSpaces = harbor.allContainerSpaces[ContainerSize.Large].Count;
            int totalContainersInSimulation = calcuateTotalContainersInSimulation(); ;

            int calcuateTotalContainersInSimulation()
            {
                int totalHarborContainerCapacity = totalSmallHarborContainerSpaces + totalMediumHarborContainerSpaces + totalLargeHarborContainerSpaces;
                int totalShipContainerCapcacity = 0;
                
               
                foreach (Ship ship in allShipsInSimulation)
                {
                    totalShipContainerCapcacity += ship.containerCapacity;
                }

                
                int totalContainersInSimulation = totalHarborContainerCapacity + totalShipContainerCapcacity;
                // Skal vi legge til et tilfeldig antall containere som ikke har plass på verken skip eller harbor?
                // Ser for meg at dette kan garantere at før eller siden så må et skip må vente med å unloade til et annet har loadet?

                return totalContainersInSimulation;
            }
            
            List<ContainerSpace> allContainerSpaces = new List<ContainerSpace>();

            ArrayList tempContainersOnBoard = new ArrayList();
          
            Ship tempShipS = new(ShipSize.Small, startTime, 10, tempContainersOnBoard);

            // Oppretter og fyller Harbor med containere
            for (int i = 0; i < totalContainersInSimulation/3; i++)
            {
                tempContainersOnBoard.Add(new Container(ContainerSize.Small, 10000, Guid.Empty));  // Legge til en Small container, på temp Ship
                tempContainersOnBoard.Add(new Container(ContainerSize.Large, 25000, Guid.Empty));  // Legge til en Medium container, på temp Ship
                tempContainersOnBoard.Add(new Container(ContainerSize.Small, 50000, Guid.Empty));  // Legge til en Large container, på temp Ship

                harbor.unloadContainer(ContainerSize.Small, tempShipS, currentTime); // Fyller så Harbor opp med containere
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

            harbor = new Harbor(10, 10, 10, 100, 100, 100);
            Dictionary<ContainerSize, List<ContainerSpace>> allContainerSpaces = harbor.allContainerSpaces;
            Dictionary<ContainerSize, List<ContainerSpace>> freeContainerSpaces = harbor.freeContainerSpaces;
            Dictionary<ContainerSize, List<ContainerSpace>> storedContainerSpaces = new();


            

            shipSmall = new Ship(ShipSize.Small, currentTime, 12, containersOnBoard);
            shipMedium = new Ship(ShipSize.Medium, currentTime, 12, containersOnBoard);
            shipLarge = new Ship(ShipSize.Large, currentTime, 12, containersOnBoard);

            //eventlogger ikke implementert
            while (endTime != currentTime)
            {
               
                foreach (Ship ship in harbor.shipsInDock) //undock
                {
                    if (ship.nextStepCheck == false)
                    {
                        if (ship.containersOnBoard.Count == 0)
                        {
                            harbor.unDockShip(ship.id, currentTime);
                            shipsInDock.Remove(ship.id);
                            harbor.NumberOfFreeDocks(ship.shipSize); //usikker på denne, om den skal legge til på den måten og om neste løkke burde endres isåfall


                            ship.nextStepCheck = true;

                        }
                        else
                        {
                            break;
                        }
                    }
                }

                if (harbor.freeDockExists(ShipSize.Small) || harbor.freeDockExists(ShipSize.Medium) || harbor.freeDockExists(ShipSize.Large))
                    //dock har ikke tatt hensyn til forskjellige størrelser
                {
                    foreach (Ship ship in harbor.harbourQueInn)
                    {
                        if (harbor.freeDockExists(ship.shipSize) && !ship.nextStepCheck)
                        {//første parameter usikker
                            foreach (Dock dock in harbor.freeDocks)
                            {
                                if (dock.size.Equals(ship.shipSize))
                                {
                                    harbor.removeShipFromQueue(ship.id);
                                    harbor.DockShip(ship.id, currentTime);
                                    harbor.removeDockFromFreeDocks(dock.id);
                                    ship.nextStepCheck = true;
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

                foreach (Ship ship in harbor.shipsInDock)//laste kontainer på skip
                {
                    if (ship.nextStepCheck)
                    {
                        break;
                    }
                    else
                    {
                        //akkurat nå er det en if test per container size, kan kanskje endres
                        if (ship.containerCapacity < ship.getNumberOfContainersOnBoard(ContainerSize.Small) && ship.currentWeightInTonn < ship.currentWeightInTonn)
                        {

                            harbor.loadContainer(ContainerSize.Small, ship, currentTime);
                            ship.nextStepCheck = true;
                        }
                        else if (ship.containerCapacity < ship.getNumberOfContainersOnBoard(ContainerSize.Medium) && ship.currentWeightInTonn < ship.currentWeightInTonn)
                        {
                            harbor.loadContainer(ContainerSize.Medium, ship, currentTime);
                            ship.nextStepCheck = true;
                        }
                        else if (ship.containerCapacity < ship.getNumberOfContainersOnBoard(ContainerSize.Large) && ship.currentWeightInTonn < ship.currentWeightInTonn)
                        {
                            // ?
                        }

                    }
                }

                foreach (Ship shipflag in harbor.shipsInDock)//akkurat nå er det bare skipene som er i dock som endrer nextstepcheck
                {
                    shipflag.nextStepCheck = true;
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