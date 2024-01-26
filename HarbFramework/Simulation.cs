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
        private int startTime = 0;
        private int currentTime = 0;
        private int endTime = 1;
        private int harborSmallCount = 0;
        private int harborMediumCount = 0;
        private int harborLargeCount = 0;
        private int containerSmallCount = 0;
        private int containerMediumCount = 0;
        private int containerLargeCount = 0;



        private ArrayList ContainersOnBoard = new ArrayList();
        private ArrayList allDocks = new ArrayList();
        private ArrayList freeDocks = new ArrayList();
        private Hashtable shipsInDock = new Hashtable();
        private Harbor harbor;
        private Ship shipSmall, shipMedium, shipLarge;
        nextStepCheck = false;
        //private Log log = new Log()



        public Simulation()
        {
            Container containerSmall = new Container(ContainerSize.Small, 1000, 1);
            Container containerMedium = new Container(ContainerSize.Medium, 2500, 1);
            Container containerLarge = new Container(ContainerSize.Large, 5000, 1);
            


            for (int i = 0; i < 50; i++)
            {
                ContainersOnBoard.add(containerSmall)
                ContainersOnBoard.add(containerMedium);
                ContainersOnBoard.add(containerLarge);
            }

            harbor = new Harbor(10, 10, 10, 100, 100, 100);
            Hashtable allContainerSpaces = harbor.allContainerSpaces;
            Hashtable freeContainerSpaces = harbor.freeContainerSpaces;
            Hashtable storedContainerSpaces = new Hashtable();


            

            shipSmall = new Ship(ShipSize.Small, 1, 12, containersOnBoard);
            shipMedium = new Ship(ShipSize.Medium, 1, 12, ContainersOnBoard);
            shipLarge = new Ship(ShipSize.Large, 1, 12, ContainersOnBoard);

            //eventlogger ikke implementert
            while (!endTime)
            {
                nextStepCheck = false;
                foreach (Ship ship in harbor.shipsInDock) //undock
                {
                    if (ship.containersOnBoard.Count == 0)
                    {
                        harbor.unDockShip(ship.getID, currentTime);
                        shipsInDock.Remove(ship.getID);
                        harbor.NumberOfFreeDocks(ship.shipSize) //usikker på denne, om den skal legge til på den måten og om neste løkke burde endres isåfall


                        nextStepCheck = true;

                    }
                    else
                    {
                        break;
                    }
                }

                if (harbor.freeDockExists(ShipSize.Small) || harbor.freeDockExists(ShipSize.Medium) || harbor.freeDockExists(ShipSize.Large)
                    //dock har ikke tatt hensyn til forskjellige størrelser
                {
                    foreach (Ship ship in harbor.harbourQueInn)
                    {
                        if (harbor.freeDockExists(ship.shipSize) && !ship.nextStepCheck)
                        {//første parameter usikker
                            foreach (Dock dock in harbor.freeDocks)
                            {
                                if (dock.size.Equals(ship.shipSize)
                                {
                                    harbor.removeShipFromQueue(ship.getID)
                                    harbor.dockedShips(ship.id, currentTime);
                                    harbor.removeDockFromFreeDocks(dock.getID)
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

                    }
                }

                foreach (Ship shipflag in harbor.shipsInDock)//akkurat nå er det bare skipene som er i dock som endrer nextstepcheck
                {
                    shipflag.nextStepCheck = true;
                }

                currentTime += 1;
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
}
}
