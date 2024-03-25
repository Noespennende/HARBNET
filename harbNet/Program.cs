﻿using System;
using Gruppe8.HarbNet;
using System.Text;
using System.Drawing;
using System.Runtime.CompilerServices;


namespace Client.HarborName
{
    internal class Program
    {
        static void Main(string[] args)
        {

            //CLIENT HARBOR 
            // Denne koden representerer ett eksempel av hvordan kunden kan opprette en simulering for havnen som er gitt i oppgaven.
            // Det finnes flere måter å gjøre dette på i APIet og koden under er bare ett eksempel på hvordan dette kan gjøres. 

            DateTime clientStartTime = new DateTime(2024, 3, 1, 8, 0, 0);
            DateTime clientEndTime = clientStartTime + TimeSpan.FromDays(20);

            ShipNames shipNames = new ShipNames();

            IList<Ship> clientShips = new List<Ship>();

            Random rand = new Random();
            int numberOfSmallShipDocks= 0;
            int numberOfMediumShipDocks = 0;
            int numberOfLargeShipDocks = 0;

            int count = 0;

            IList<Container> containerList = new List<Container>();
            IList<ContainerStorageRow> storageRows = new List<ContainerStorageRow>();

            for (int i = 0; i < 24; i++)
            {
                storageRows.Add(new ContainerStorageRow(18 * 6 * 4));
            }

            for (int i = 0; i < 7; i++)
            {
                storageRows.Add(new ContainerStorageRow(15 * 6 * 4));
            }

            // Kommentert ut for enklere testing. Fjerner kommentering når alt er OK
            /*
            for (int i = 0 ; i < 7; i++) {
                for (int j = 0 ; j < 20; j++) {
                    ShipSize shipSize;
                    bool singleTrip;
                    int randShipSize = rand.Next(1, 4);
                    int containerCount;

                

                    if (randShipSize ==  1) {
                        shipSize = ShipSize.Small;
                        containerCount = 19;

                    } else if (randShipSize == 2)
                    {
                        shipSize = ShipSize.Medium;
                        containerCount = 49;
                    } else
                    {
                        shipSize = ShipSize.Large;
                        containerCount = 99;
                    }

                    if (rand.Next(0, 2) == 0)
                    {
                        singleTrip = true;
                    } else
                    {
                        singleTrip = false;
                    }

                    if (singleTrip)
                    {
                        if (shipSize == ShipSize.Small)
                        {
                            numberOfSmallShipDocks++;
                        } else if (shipSize == ShipSize.Medium)
                        {
                            numberOfMediumShipDocks++;
                        } else
                        {
                            numberOfLargeShipDocks++;
                        }
                    }

                    DateTime startDate = clientStartTime.AddDays(rand.Next(0, 7));


                    clientShips.Add(new Ship((shipNames.Names[count]), shipSize, startDate ,singleTrip, rand.Next(0, 6), rand.Next(1, (containerCount / 2)), rand.Next(1, (containerCount / 2))));
                    count++;
                }
                
            }
            */

            Ship ship1 = new Ship(("Ship solo 1"), ShipSize.Medium, clientStartTime, true, rand.Next(0, 6), rand.Next(1, (49 / 2)), rand.Next(1, (49 / 2)));
            Ship ship2 = new Ship(("Ship solo 2"), ShipSize.Medium, clientStartTime, true, rand.Next(0, 6), rand.Next(1, (49 / 2)), rand.Next(1, (49 / 2)));
            Ship ship3 = new Ship(("Ship recurring"), ShipSize.Large, clientStartTime, false, rand.Next(0, 6), rand.Next(1, (99 / 2)), rand.Next(1, (99 / 2)));
            
            clientShips.Add(ship1);
            clientShips.Add(ship2);
            clientShips.Add(ship3);



            Harbor clientHarbor = new Harbor(clientShips, storageRows, 1, 1, 1, 7, (((100 * 5) / 24)/7), 10, numberOfSmallShipDocks, 1, numberOfLargeShipDocks,30,15,10,20, 3);

            Simulation clientSim = new Simulation(clientHarbor, clientStartTime, clientEndTime);

            //subscribing to events
            clientSim.SimulationStarting += (sender, e) =>
            {
                SimulationStartingEventArgs args = (SimulationStartingEventArgs)e;
                Console.WriteLine("Simulation starting ...");
                Console.WriteLine($"Simulating {args.HarborToBeSimulated.ID} from {args.StartDate}\n");
                Thread.Sleep(2000);
            };

            clientSim.OneHourHasPassed += (sender, e) =>
            {
                OneHourHasPassedEventArgs args = (OneHourHasPassedEventArgs)e;
                Console.WriteLine($"| New hour!: {args.CurrentTime.TimeOfDay} |\n");

            };

            clientSim.ShipAnchoring += (sender, e) =>
            {
                ShipAnchoringEventArgs args = (ShipAnchoringEventArgs)e;
                Console.WriteLine($"| {args.CurrentTime} | '{args.Ship.Name}' anchoring to anchorage with ID {args.AnchorageID}\n");
            };

            clientSim.ShipAnchored += (sender, e) =>
            {
                ShipAnchoredEventArgs args = (ShipAnchoredEventArgs)e;
                Console.WriteLine($"| {args.CurrentTime} | '{args.Ship.Name}' anchored to anchorage with ID {args.AnchorageID}\n");
            };

            clientSim.ShipDockingtoLoadingDock += (sender, e) =>
            {
                ShipDockingToLoadingDockEventArgs args = (ShipDockingToLoadingDockEventArgs)e;
                Console.WriteLine($"| {args.CurrentTime} | '{args.Ship.Name}' starting docking to loading dock with ID {args.DockID}\n");
            };

            clientSim.ShipDockedtoLoadingDock += (sender, e) =>
            {
                ShipDockedToLoadingDockEventArgs args = (ShipDockedToLoadingDockEventArgs)e;
                Console.WriteLine($"| {args.CurrentTime} | '{args.Ship.Name}' has docked to loading dock with ID {args.DockID}\n");
            };

            clientSim.ShipStartingUnloading += (sender, e) =>
            {
                ShipStartingUnloadingEventArgs args = (ShipStartingUnloadingEventArgs)e;
                Console.WriteLine($"| {args.CurrentTime} | '{args.Ship.Name}' is starting the unloading process.'\n");

            };

            // BØR FORANDRE HVA SOM SKJER PÅ "MOTTAKELSE" -> Akkurat nå blir det myyyye utskrift. Kanskje en count?
            clientSim.ShipUnloadedContainer += (sender, e) =>
            {
                ShipUnloadedContainerEventArgs args = (ShipUnloadedContainerEventArgs)e;
                //Console.WriteLine($"| {args.CurrentTime} | '{args.Ship.Name}' unloaded container of size '{args.Container.Size}'\n");

            };

            clientSim.ShipDoneUnloading += (sender, e) =>
            {
                ShipDoneUnloadingEventArgs args = (ShipDoneUnloadingEventArgs)e;
                Console.WriteLine($"| {args.CurrentTime} | '{args.Ship.Name}' has finished unloading.\n");

            };

            clientSim.ShipStartingLoading += (sender, e) =>
            {
                ShipStartingLoadingEventArgs args = (ShipStartingLoadingEventArgs)e;
                Console.WriteLine($"| {args.CurrentTime} | '{args.Ship.Name}' is starting the loading process.\n");

            };

            // BØR FORANDRE HVA SOM SKJER PÅ "MOTTAKELSE" -> Akkurat nå blir det myyyye utskrift. Kanskje en count?
            clientSim.ShipLoadedContainer += (sender, e) =>
            {

                ShipLoadedContainerEventArgs args = (ShipLoadedContainerEventArgs)e;

                //Console.WriteLine($"| {args.CurrentTime} | '{args.Ship.Name}' loaded container of size '{args.Container.Size}'\n");
            };

            clientSim.ShipDoneLoading += (sender, e) =>
            {
                ShipDoneLoadingEventArgs args = (ShipDoneLoadingEventArgs)e;
                Console.WriteLine($"| {args.CurrentTime} | '{args.Ship.Name}' has finished loading.\n");

            };

            clientSim.ShipUndocking += (sender, e) =>
            {
                ShipUndockingEventArgs args = (ShipUndockingEventArgs)e;
                Console.WriteLine($"| {args.CurrentTime} | '{args.Ship.Name}' undocking from location ID '{args.LocationID}'\n");
            };

            clientSim.ShipInTransit += (sender, e) =>
            {
                ShipInTransitEventArgs args = (ShipInTransitEventArgs)e;
                Console.WriteLine($"| {args.CurrentTime} | ' {args.Ship.Name}' is in transit at transit ID '{args.TransitLocationID}'\n");
            };

            clientSim.ShipDockedToShipDock += (sender, e) =>
            {
                ShipDockedToShipDockEventArgs args = (ShipDockedToShipDockEventArgs)e;
                Console.WriteLine($"| {args.CurrentTime} | '{args.Ship.Name}' has docked to ship dock with ID '{args.DockID}'\n");
            };

            clientSim.DayEnded += (sender, e) =>
            {
                DayOverEventArgs args = (DayOverEventArgs)e;

                Console.WriteLine($"-----------------------------------");
                Console.WriteLine($"| {args.CurrentTime} | Day over! |");
                Console.WriteLine($"-----------------------------------");

            };

            clientSim.DayLoggedToSimulationHistory += (sender, e) =>
            {
                DayLoggedEventArgs args = (DayLoggedEventArgs)e;

                bool anyLogsPrinted = false;

                Console.WriteLine($"** Here is a quick summary of {args.Ship.Name}'s movements today: **");
                if (args.DayReviewShipLogs != null && args.DayReviewShipLogs.Any())
                {
                    foreach (StatusLog log in args.DayReviewShipLogs)
                    {
                        Console.WriteLine($"| {log.PointInTime} | {args.Ship.Name} | Status: {log.Status} |");
                        anyLogsPrinted = true;
                    }
                }

                if (!anyLogsPrinted)
                {
                    if (args.Ship.History.Count != 0)
                    {
                        Console.WriteLine($"Looks like today has been a pretty quiet day for {args.Ship.Name} in the ol' Harbor-ino");
                        Console.WriteLine($"Believe it or not, {args.Ship.Name} is still in {args.Ship.History.Last().Status}!\n");
                    }
                    else
                    {
                        Console.WriteLine($"Looks like we're still waiting for {args.Ship.Name} to start!");
                        Console.WriteLine($"It's start date is {args.Ship.StartDate}. It is currently {args.CurrentTime}\n");
                    }
                }
                Console.WriteLine("-----------------------------------");
                Console.WriteLine("");
            };

            clientSim.SimulationEnded += (sender, e) =>
            {
                SimulationEndedEventArgs args = (SimulationEndedEventArgs)e;
                Console.WriteLine($"-----------------------------------");
                Console.WriteLine($"|         Simulation over!         |");
                Console.WriteLine($"-----------------------------------");

                Thread.Sleep(1000);
            };


            clientSim.Run();

            clientSim.PrintShipHistory(ship1);
            clientSim.PrintShipHistory(ship2);
            clientSim.PrintShipHistory(ship3);

            //unsubscribing to events
            clientSim.SimulationStarting -= (sender, e) =>
            {
                SimulationStartingEventArgs args = (SimulationStartingEventArgs)e;
                Console.WriteLine("Simulation starting ...");
                Console.WriteLine($"Simulating {args.HarborToBeSimulated.ID} from {args.StartDate}\n");
                Thread.Sleep(2000);
            };



            //EXAMPLE PROJECT

            /*This project sets up and runs a simulation using the public API. Several methods for printing information about the simulation
             is used simultaniously here (ShipObject.PrintHistory(), SimulationObject.PrintContainerHistory() and SimulationObject.PrintShipHistory()) 
            so the consol log will be cluttered with information. Normally only one of these methods would be enough to get information about the simulation. 

            // Setting a time for the start time of the simulation,  which will be used in the creation of Simulation object
            DateTime startTime = new DateTime(2024, 3, 1, 8, 0, 0);

            // Setting a time for the simulation's end, which will be used in the creation of Simulation object
            DateTime endTime = startTime + TimeSpan.FromDays(30);
            List<Ship> ships = new List<Ship>();



            // Creating ships that will be simulated in the Simulation
            Ship shipHappens = new("Ship Happens", ShipSize.Large, startTime, false, 7, 25, 10, 15);
            Ship auroraBorealis = new("Aurora Borealis", ShipSize.Medium, startTime.AddDays(4), false, 3, 5, 20, 15);
            Ship skipOHoi = new("Ship O'Hoi", ShipSize.Small, startTime.AddHours(4), false, 1, 5, 5, 5);
            Ship ssSolitude = new("SS Solitude", ShipSize.Small, startTime, true, 5, 1, 10, 4);
            Ship denSorteDame = new("Den Sorte Dame", ShipSize.Large, startTime, false, 4, 20, 15, 5);

            // Adding the ships to a list, that will be sent into the Harbor object
            ships.Add(shipHappens);
            ships.Add(ssSolitude);
            ships.Add(auroraBorealis);
            //ships.Add(skipOHoi);
            //ships.Add(denSorteDame);


            // Creating the harbor which will be used in the simulation, using the ship list
            Harbor kjuttaviga = new Harbor(ships, 4, 3, 2, 2, 1, 1, 100, 200, 150);

            // Creating the simulation object, of which the simulation will run from
            Simulation simulation = new Simulation(kjuttaviga, startTime, endTime);

            //subscribing to events
            simulation.SimulationStarting += (sender, e) =>
            {
                SimulationStartingEventArgs args = (SimulationStartingEventArgs)e;
                Console.WriteLine("Simulation starting ...");
                Console.WriteLine($"Simulating {args.harborToBeSimulated.ID} from {args.startDate}\n");
                Thread.Sleep(2000);
            };

            simulation.ShipAnchoring += (sender, e) =>
            {
                shipAnchoringEventArgs args = (shipAnchoringEventArgs)e;
                Console.WriteLine($"| {args.CurrentTime} | '{args.ship.Name}' anchoring to anchorage with ID {args.anchorageID}\n");
            };

            simulation.ShipAnchored += (sender, e) =>
            {
                shipAnchoredEventArgs args = (shipAnchoredEventArgs)e;
                Console.WriteLine($"| {args.CurrentTime} | '{args.ship.Name}' anchored to anchorage with ID {args.anchorageID}\n");
            };

            simulation.ShipDockingtoLoadingDock += (sender, e) =>
            {
                shipDockingToLoadingDockEventArgs args = (shipDockingToLoadingDockEventArgs)e;
                Console.WriteLine($"| {args.CurrentTime} | '{args.ship.Name}' starting docking to loading dock with ID {args.dockID}\n");
            };

            simulation.ShipDockedtoLoadingDock += (sender, e) =>
            {
                shipDockedToLoadingDockEventArgs args = (shipDockedToLoadingDockEventArgs)e;
                Console.WriteLine($"| {args.CurrentTime} | '{args.ship.Name}' has docked to loading dock with ID {args.dockID}\n");
            };


            simulation.ShipUnloadedContainer += (sender, e) =>
            {
                ShipUnloadedContainerEventArgs args = (ShipUnloadedContainerEventArgs)e;
                Console.WriteLine($"| {args.CurrentTime} | '{args.ship.Name}' unloaded container of size '{args.Container.Size}'\n");

            };

            simulation.ShipLoadedContainer += (sender, e) =>
            {
                shipLoadedContainerEventArgs args = (shipLoadedContainerEventArgs)e;

                Console.WriteLine($"| {args.CurrentTime} | '{args.ship.Name}' loaded container of size '{args.Container.Size}'\n");
            };

            simulation.ShipUndocking += (sender, e) =>
            {
                ShipUndockingEventArgs args = (ShipUndockingEventArgs)e;
                Console.WriteLine($"| {args.CurrentTime} | '{args.ship.Name}' undocking from dock ID '{args.dockID}'\n");
            };

            simulation.ShipInTransit += (sender, e) =>
            {
                ShipInTransitEventArgs args = (ShipInTransitEventArgs)e;
                Console.WriteLine($"| {args.CurrentTime} | '{args.ship.Name}' is in transit at transit ID '{args.transitLocationID}'\n");
            };

            simulation.ShipDockedToShipDock += (sender, e) =>
            {
                shipDockedToShipDockEventArgs args = (shipDockedToShipDockEventArgs)e;
                Console.WriteLine($"| {args.CurrentTime} | '{args.ship.Name}' has docked to ship dock with ID '{args.dockID}'\n");
            };

            simulation.DayEnded += (sender, e) =>
            {
                DayLoggedEventArgs args = (DayLoggedEventArgs)e;

                Console.WriteLine($"-----------------------------------");
                Console.WriteLine($"| {args.CurrentTime} | Day over! |");
                Console.WriteLine($"-----------------------------------");

            };

            simulation.DayLoggedToSimulationHistory += (sender, e) =>
            {
                DayLoggedEventArgs args = (DayLoggedEventArgs)e;

                bool anyLogsPrinted = false;

                Console.WriteLine($"** Here is a quick summary of {args.ship.Name}'s movements today: **");
                if (args.dayReviewShipLogs != null && args.dayReviewShipLogs.Any())
                {
                    foreach (StatusLog log in args.dayReviewShipLogs)
                    {
                        Console.WriteLine($"| {log.PointInTime} | {args.ship.Name} | Status: {log.Status} |");
                        anyLogsPrinted = true;
                    }
                }

                if (!anyLogsPrinted)
                {
                    if (args.ship.History.Count != 0)
                    {
                        Console.WriteLine($"Looks like today has been a pretty quiet day for {args.ship.Name} in the ol' Harbor-ino");
                        Console.WriteLine($"Believe it or not, {args.ship.Name} is still in {args.ship.History.Last().Status}!\n");
                    }
                    else
                    {
                        Console.WriteLine($"Looks like we're still waiting for {args.ship.Name} to start!");
                        Console.WriteLine($"It's start date is {args.ship.StartDate}. It is currently {args.CurrentTime}\n");
                    }
                }
                Console.WriteLine("-----------------------------------");
                Console.WriteLine("");
            };

            simulation.SimulationEnded += (sender, e) =>
            {
                SimulationEndedEventArgs args = (SimulationEndedEventArgs)e;
                Console.WriteLine($"-----------------------------------");
                Console.WriteLine($"|         Simulation over!         |");
                Console.WriteLine($"-----------------------------------");
            };


            /* Running/starting the Simulation. Run() outputs a list of logs that is created during the simulation. It's worth noting that simulation.Run() will also print
            updates on all ships during the simulation
            simulation.Run();


            //unsubscribing to events
            simulation.SimulationStarting -= (sender, e) =>
            {
                SimulationStartingEventArgs args = (SimulationStartingEventArgs)e;
                Console.WriteLine("Simulation starting ...");
                Console.WriteLine($"Simulating {args.harborToBeSimulated.ID} from {args.startDate}\n");
                Thread.Sleep(2000);
            };

            simulation.ShipAnchoring -= (sender, e) =>
            {
                shipAnchoringEventArgs args = (shipAnchoringEventArgs)e;
                Console.WriteLine($"| {args.CurrentTime} | '{args.ship.Name}' anchoring to anchorage with ID {args.anchorageID}\n");
            };

            simulation.ShipAnchored -= (sender, e) =>
            {
                shipAnchoredEventArgs args = (shipAnchoredEventArgs)e;
                Console.WriteLine($"| {args.CurrentTime} | '{args.ship.Name}' anchored to anchorage with ID {args.anchorageID}\n");
            };

            simulation.ShipDockingtoLoadingDock -= (sender, e) =>
            {
                shipDockingToLoadingDockEventArgs args = (shipDockingToLoadingDockEventArgs)e;
                Console.WriteLine($"| {args.CurrentTime} | '{args.ship.Name}' starting docking to loading dock with ID {args.dockID}\n");
            };

            simulation.ShipDockedtoLoadingDock -= (sender, e) =>
            {
                shipDockedToLoadingDockEventArgs args = (shipDockedToLoadingDockEventArgs)e;
                Console.WriteLine($"| {args.CurrentTime} | '{args.ship.Name}' has docked to loading dock with ID {args.dockID}\n");
            };


            simulation.ShipUnloadedContainer -= (sender, e) =>
            {
                ShipUnloadedContainerEventArgs args = (ShipUnloadedContainerEventArgs)e;
                Console.WriteLine($"| {args.CurrentTime} | '{args.ship.Name}' unloaded container of size '{args.Container.Size}'\n");

            };

            simulation.ShipLoadedContainer -= (sender, e) =>
            {
                shipLoadedContainerEventArgs args = (shipLoadedContainerEventArgs)e;

                Console.WriteLine($"| {args.CurrentTime} | '{args.ship.Name}' loaded container of size '{args.Container.Size}'\n");
            };

            simulation.ShipUndocking -= (sender, e) =>
            {
                ShipUndockingEventArgs args = (ShipUndockingEventArgs)e;
                Console.WriteLine($"| {args.CurrentTime} | '{args.ship.Name}' undocking from dock ID '{args.dockID}'\n");
            };

            simulation.ShipInTransit -= (sender, e) =>
            {
                ShipInTransitEventArgs args = (ShipInTransitEventArgs)e;
                Console.WriteLine($"| {args.CurrentTime} | '{args.ship.Name}' is in transit at transit ID '{args.transitLocationID}'\n");
            };

            simulation.ShipDockedToShipDock -= (sender, e) =>
            {
                shipDockedToShipDockEventArgs args = (shipDockedToShipDockEventArgs)e;
                Console.WriteLine($"| {args.CurrentTime} | '{args.ship.Name}' has docked to ship dock with ID '{args.dockID}'\n");
            };

            simulation.DayEnded -= (sender, e) =>
            {
                DayLoggedEventArgs args = (DayLoggedEventArgs)e;

                Console.WriteLine($"-----------------------------------");
                Console.WriteLine($"| {args.CurrentTime} | Day over! |");
                Console.WriteLine($"-----------------------------------");

            };

            simulation.DayLoggedToSimulationHistory -= (sender, e) =>
            {
                DayLoggedEventArgs args = (DayLoggedEventArgs)e;

                bool anyLogsPrinted = false;

                Console.WriteLine($"** Here is a quick summary of {args.ship.Name}'s movements today: **");
                if (args.dayReviewShipLogs != null && args.dayReviewShipLogs.Any())
                {
                    foreach (StatusLog log in args.dayReviewShipLogs)
                    {
                        Console.WriteLine($"| {log.PointInTime} | {args.ship.Name} | Status: {log.Status} |");
                        anyLogsPrinted = true;
                    }
                }

                if (!anyLogsPrinted)
                {
                    if (args.ship.History.Count != 0)
                    {
                        Console.WriteLine($"Looks like today has been a pretty quiet day for {args.ship.Name} in the ol' Harbor-ino");
                        Console.WriteLine($"Believe it or not, {args.ship.Name} is still in {args.ship.History.Last().Status}!\n");
                    }
                    else
                    {
                        Console.WriteLine($"Looks like we're still waiting for {args.ship.Name} to start!");
                        Console.WriteLine($"It's start date is {args.ship.StartDate}. It is currently {args.CurrentTime}\n");
                    }
                }
                Console.WriteLine("-----------------------------------");
                Console.WriteLine("");
            };

            simulation.SimulationEnded -= (sender, e) =>
            {
                SimulationEndedEventArgs args = (SimulationEndedEventArgs)e;
                Console.WriteLine($"-----------------------------------");
                Console.WriteLine($"|         Simulation over!         |");
                Console.WriteLine($"-----------------------------------");
            };



            Console.WriteLine("\n-----------PRINTING HISTORY OF A SINGLE SHIP--------------\n");

            //Prints the history for a single ship
            ssSolitude.PrintHistory();

            Console.WriteLine("\n-----------PRINTING HISTORY OF ALL CONTAINERS IN THE SIMULATION--------------\n");
            //Prints the history for all containers in the simulation
            simulation.PrintContainerHistory();

            Console.WriteLine("\n-----------PRINTING HISTORY OF ALL SHIPS IN THE SIMULATION--------------\n");
            //Prints the history for all ships in the simulation
            simulation.PrintShipHistory(); */

        }
    }
}