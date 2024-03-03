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
            Ship ssSolitude = new("SS Solitude", ShipSize.Small, startTime, true, 1, 1, 10, 4);
            Ship denSorteDame = new("Den Sorte Dame", ShipSize.Large, startTime, false, 4, 20, 15, 5);

            // Adding the ships to a list, that will be sent into the Harbor object
            ships.Add(shipHappens);
            ships.Add(ssSolitude);
            ships.Add(auroraBorealis);
            ships.Add(skipOHoi);
            ships.Add(denSorteDame);

        
            // Creating the harbor which will be used in the simulation, using the ship list
            Harbor kjuttaviga = new Harbor(ships, 4, 3, 2, 2, 1, 1, 100, 200, 150);
            
            // Creating the simulation object, of which the simulation will run from
            Simulation simulation = new Simulation(kjuttaviga, startTime, endTime);
            simulation.ShipUnDocked += onShipUndock;
            simulation.shipDockedShipDock += onShipDockedToDock;
            simulation.shipLoadingContainer += onshipLoadingContainer;
            simulation.DayOverEvent += (message, currentTime) =>
            {
                Console.WriteLine("\nDay over");
                Console.WriteLine("Current time: " + currentTime);
            };

            simulation.SimulationEnd += (message) =>
            {
                Console.WriteLine(message);
            };

            simulation.ShipStatusEvent += (message) =>
            {
                Console.WriteLine(message);
            };


            static void onShipUndock(Ship ship)
            {
                Console.WriteLine($"Ship {ship.Name} undocked");
            }

            static void onShipDockedToDock(Ship ship)
            {
                Console.WriteLine($"Ship {ship.Name} docked to Loading dock");
            }
            static void onshipLoadingContainer(Ship ship)
            {
                Console.WriteLine($"Ship {ship.Name} Loading Containers");
            }

            /* Running/starting the Simulation. Run() outputs a list of logs that is created during the simulation. It's worth noting that simulation.Run() will also print
            updates on all ships during the simulation*/
            simulation.Run();

          
            /*
            Console.WriteLine("\n-------------------------\n");

            //Prints the history for a single ship
            ssSolitude.PrintHistory();

            Console.WriteLine("\n-------------------------\n");
            //Prints the history for all containers in the simulation
            //simulation.PrintContainerHistory();

            Console.WriteLine("\n-------------------------\n");
            //Prints the history for all ships in the simulation
            //simulation.PrintShipHistory();

            StringBuilder sb = new StringBuilder();

            Console.WriteLine($"{sb.ToString()}");*/

        }

        
    }
}