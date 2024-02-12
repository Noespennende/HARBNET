using System.ComponentModel;
using System;
using HarbFramework;

namespace harbNet
{
    internal class Program
    {
        static void Main(string[] args)
        {


            DateTime startTime = new DateTime(2024, 3, 1, 8, 0, 0);
            DateTime endTime = startTime + TimeSpan.FromDays(30);
            List<Ship> ships = new List<Ship>();

            //Ship shipHappens = new("Ship Happens", ShipSize.Large, startTime, false, 7, 50);
            
            Ship auroraBorealis = new("Aurora Borealis", ShipSize.Medium, startTime.AddDays(4), true, 3, 49);
            //Ship skipOHoi = new("Ship O'Hoi", ShipSize.Small, startTime.AddHours(4), false, 1, 15);
            //Ship ssSolitude = new("SS Solitude", ShipSize.Small, startTime, false, 14, 1);

            //ships.Add(shipHappens);
            //ships.Add(ssSolitude);
            ships.Add(auroraBorealis);
            //ships.Add(skipOHoi);

            

            Harbor kjuttaviga = new Harbor(ships, 4, 3, 2, 2, 1, 1, 100, 200, 150);


            Simulation simulation = new Simulation(kjuttaviga, startTime, endTime);

            IList<Log> historyList = simulation.Run();

            Console.WriteLine(auroraBorealis.ContainersOnBoard.Count);

            


            //Console.WriteLine("-------------------------");

            //simulation.PrintContainerHistory();




        }
    }
}
