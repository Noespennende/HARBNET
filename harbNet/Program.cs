using System.ComponentModel;
using System;
using HarbFramework;

namespace harbNet
{
    internal class Program
    {
        static void Main(string[] args)
        {


            DateTime startTime = new DateTime(2023, 2, 2, 8, 0, 0);
            DateTime endTime = startTime + TimeSpan.FromDays(40);
            List<Ship> ships = new List<Ship>();

            Ship testShip = new("SS MonkeyPaw", ShipSize.Large, startTime, false, 2, 50);
            ships.Add(testShip);


            Ship oneTripShip = new("SS OneTrip", ShipSize.Small, startTime.AddDays(5), true, 1, 2);
            ships.Add(oneTripShip);


            Harbor harbor = new Harbor(ships, 10, 10, 10, 300, 300, 300, 300, 300, 300);


            Simulation simulation = new Simulation(harbor, startTime, endTime);


            IList<Log> historyList = simulation.Run();

            oneTripShip.PrintHistory();





            Console.WriteLine("-------------------------");

            simulation.PrintShipHistory();




        }
    }
}
