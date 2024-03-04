using System.ComponentModel;
using System;
using Gruppe8.HarbNet;
using System.Text;


namespace TestProgram
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //EXAMPLE PROJECT

            /*This project sets up and runs a simulation using the public API. Several methods for printing information about the simulation
             is used simultaniously here (ShipObject.PrintHistory(), SimulationObject.PrintContainerHistory() and SimulationObject.PrintShipHistory()) 
            so the consol log will be cluttered with information. Normally only one of these methods would be enough to get information about the simulation. */

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
                Console.WriteLine($"| {args.currentTime} | '{args.ship.Name}' anchoring to anchorage with ID {args.anchorageID}\n");
            };

            simulation.ShipAnchored += (sender, e) =>
            {
                shipAnchoredEventArgs args = (shipAnchoredEventArgs)e;
                Console.WriteLine($"| {args.currentTime} | '{args.ship.Name}' anchored to anchorage with ID {args.anchorageID}\n");
            };

            simulation.ShipDockingtoLoadingDock += (sender, e) =>
            {
                shipDockingToLoadingDockEventArgs args = (shipDockingToLoadingDockEventArgs)e;
                Console.WriteLine($"| {args.currentTime} | '{args.ship.Name}' starting docking to loading dock with ID {args.dockId}\n");
            };

            simulation.ShipDockedtoLoadingDock += (sender, e) =>
            {
                shipDockedToLoadingDockEventArgs args = (shipDockedToLoadingDockEventArgs)e;
                Console.WriteLine($"| {args.currentTime} | '{args.ship.Name}' has docked to loading dock with ID {args.dockId}\n");
            };


            simulation.ShipUnloadedContainer += (sender, e) =>
            {
                ShipUnloadedContainerEventArgs args = (ShipUnloadedContainerEventArgs)e;
                Console.WriteLine($"| {args.currentTime} | '{args.ship.Name}' unloaded container of size '{args.Container.Size}'\n");

            };

            simulation.ShipLoadedContainer += (sender, e) =>
            {
                shipLoadedContainerEventArgs args = (shipLoadedContainerEventArgs)e;

                Console.WriteLine($"| {args.currentTime} | '{args.ship.Name}' loaded container of size '{args.Container.Size}'\n");
            };

            simulation.ShipUndocking += (sender, e) =>
            {
                ShipUndockingEventArgs args = (ShipUndockingEventArgs)e;
                Console.WriteLine($"| {args.currentTime} | '{args.ship.Name}' undocking from dock ID '{args.dockId}'\n");
            };

            simulation.ShipInTransit += (sender, e) =>
            {
                ShipInTransitEventArgs args = (ShipInTransitEventArgs)e;
                Console.WriteLine($"| {args.currentTime} | '{args.ship.Name}' is in transit at transit ID '{args.transitLocationID}'\n");
            };

            simulation.ShipDockedShipDock += (sender, e) =>
            {
                shipDockedToShipDockEventArgs args = (shipDockedToShipDockEventArgs)e;
                Console.WriteLine($"| {args.currentTime} | '{args.ship.Name}' has docked to ship dock with ID '{args.dockId}'\n");
            };

            simulation.DayEnded += (sender, e) =>
            {
                DayLoggedEventArgs args = (DayLoggedEventArgs)e;

                Console.WriteLine($"-----------------------------------");
                Console.WriteLine($"| {args.currentTime} | Day over! |");
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
                        Console.WriteLine($"It's start date is {args.ship.StartDate}. It is currently {args.currentTime}\n");
                    }
                }
                Console.WriteLine("-----------------------------------");
                Console.WriteLine("");
            };

            simulation.SimulationEnded += (sender, e) =>
            {
                SimulationEndedEventArgs args = (SimulationEndedEventArgs)e;
                Console.WriteLine($"{args.message}, {args.simulationHistory}");
            };
           

            /* Running/starting the Simulation. Run() outputs a list of logs that is created during the simulation. It's worth noting that simulation.Run() will also print
            updates on all ships during the simulation*/
            simulation.Run();

          
            
            Console.WriteLine("\n-----------PRINTING HISTORY OF A SINGLE SHIP--------------\n");
            
            //Prints the history for a single ship
            //ssSolitude.PrintHistory();

            Console.WriteLine("\n-----------PRINTING HISTORY OF ALL CONTAINERS IN THE SIMULATION--------------\n");
            //Prints the history for all containers in the simulation
            //simulation.PrintContainerHistory();

            Console.WriteLine("\n-----------PRINTING HISTORY OF ALL SHIPS IN THE SIMULATION--------------\n");
            //Prints the history for all ships in the simulation
            //simulation.PrintShipHistory();

        }

        
    }
}