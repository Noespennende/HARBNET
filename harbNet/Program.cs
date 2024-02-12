using System.ComponentModel;
using System;
using HarbFramework;

namespace harbNet
{
    internal class Program
    {
        static void Main(string[] args)
        {


            // Setting a time for the start time of the simulation,  which will be used in the creation of Simulation object
            DateTime startTime = new DateTime(2024, 3, 1, 8, 0, 0);

            // Setting a time for the simulation's end, which will be used in the creation of Simulation object
            DateTime endTime = startTime + TimeSpan.FromDays(30);
            List<Ship> ships = new List<Ship>();

            // Creating ships that will be simulated in the Simulation
            Ship shipHappens = new("Ship Happens", ShipSize.Large, startTime, false, 7, 50);
            Ship auroraBorealis = new("Aurora Borealis", ShipSize.Medium, startTime.AddDays(4), false, 3, 49);
            Ship skipOHoi = new("Ship O'Hoi", ShipSize.Small, startTime.AddHours(4), false, 1, 15);
            Ship ssSolitude = new("SS Solitude", ShipSize.Small, startTime, true, 14, 1);

            // Adding the ships to a list, that will be sent into the Harbor object
            ships.Add(shipHappens);
            ships.Add(ssSolitude);
            ships.Add(auroraBorealis);
            ships.Add(skipOHoi);

            // Creating the harbor which will be used in the simulation, using the ship list
            Harbor kjuttaviga = new Harbor(ships, 4, 3, 2, 2, 1, 1, 100, 200, 150);

            // Creating the simulation object, of which the simulation will run from
            Simulation simulation = new Simulation(kjuttaviga, startTime, endTime);

            // Running/starting the Simulation. Run() outputs a list of logs that is created during the simulation. This is 
            simulation.Run();


            Console.WriteLine("-------------------------");

            ssSolitude.PrintHistory();

            simulation.PrintContainerHistory();
            simulation.PrintShipHistory();

           



        }
    }
}
