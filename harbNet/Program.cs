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
            DateTime endTime = startTime + TimeSpan.FromDays(50);
            List<Ship> ships = new List<Ship>();

            Ship testShip = new(ShipSize.Large, startTime, 10, 100);
            ships.Add(testShip);

            Ship testShip2 = new(ShipSize.Medium, startTime + TimeSpan.FromHours(1), 5, 50);
            ships.Add(testShip2);

            Ship testShip3 = new(ShipSize.Small, startTime, 7, 20);
            ships.Add(testShip3);

            Harbor harbor = new Harbor(ships, 10, 10, 10, 300, 300, 300);


            Simulation simulation = new Simulation(harbor, startTime, endTime);

            simulation.Run(startTime, endTime);
            Console.WriteLine("Hei her er jeg");
            


            /*
            harbNet.create();

            harbNet.setName(String navn);

            harbNet.createPort(PortSize portSize); //sender inn enum portsize

            // harbNet.setNumberOfShips(int LargeShips, int MediumShips, int );

            harbNet.createShip(ShipSize shipSize, DateOnly StartDate DateTime ReturnDate) // overload for funksjonen hvor man kan sette bare skip størelse, skip størelse og start dato eller skipstørrelse, start dato og returdato.

            harbNet.SimulationDuration(DateOnly StartDate.Date EndDate);

            harbNet.run(); //Få errors hvis man ikke har satt riktige variabler.

            harbNet.getContainerHistory(UUID ContainerId); // Collection med Events -- m. overloads hvor man sender inn dato for ett gitt døgn

            harbNet.getShipHistory(UUID ShipId); // Collection med Events -- m. overloads hvor man sender inn dato for ett gitt døgn

            harbNet.getContainerIDs(); // Collection med container IDs. 

            harbNet.getShipIDs(); // Collection med Ship IDs;

            harbNet.printHistoryToConsole(); // printer simulerings historien til konsoll -- m. overloads hvor man sender inn dato for ett gitt døgn

            harbNet.printHistoryToFile(String Filepath); // printer simulerings historien til fil -- m. overloads hvor man sender inn dato for ett gitt døgn

            harbNet.getShipTurnAroundTime(); //gjennomsnitts turnouround for alle skip -- m. overloads hvor man sender inn dato for ett gitt døgn

            harbNet.getContainerTurnAroundTime(); //gjennomsnitts turnouround for alle skip -- m. overloads hvor man sender inn dato for ett gitt døgn

            harbNet.getAverageLoadTime(); //gjenomsnitlig loadtime for alle skip -- m. overloads hvor man sender inn dato for ett gitt døgn

            harbNet.getAverageUnloadTime(); //gjenomsnitlig loadtime for alle skip unload time for alle skip -- m. overloads hvor man sender inn dato for ett gitt døgn

            harbNet.getAverageQueueTime(); //gjennomsnittstid skip venter i kø -- m. overloads hvor man sender inn dato for ett gitt døgn

            harbNet.getWeatherHistory(); //collection med events av vær -- m. overloads hvor man sender inn dato for ett gitt døgn

            harbNet.getContainersMoved(); //Antall containere flyttet i løpet av simuleringen  -- m. overloads hvor man sender inn dato for ett gitt døgn

            harbNet.getNumberOfDockings() //Antall skip docket i løpet av simuleringen    -- m. overloads hvor man sender inn dato for ett gitt døgn
            */

        }
    }
}
