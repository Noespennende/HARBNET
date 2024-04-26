using System;
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


            DateTime startTime = DateTime.Now;
            DateTime endTime = DateTime.Now.AddDays(7);
            
            Ship titanic = new Ship("Titanic", ShipSize.Large, startTime.AddDays(1), true, 1, 10, 40);

            Ship denSorteDame = new Ship(
                shipName: "Den Sorte Dame",
                shipSize: ShipSize.Medium,
                startDate: startTime.AddDays(2),
                isForASingleTrip: false,
                roundTripInDays: 2,
                numberOfHalfContainersOnBoard: 20,
                numberOfFullContainersOnBoard: 20
                );

            

            IList<Ship> shipsList = new List<Ship>();

            shipsList.Add( titanic );
            shipsList.Add(denSorteDame);

            ContainerStorageRow storageRow1 = new ContainerStorageRow(100);
            
            ContainerStorageRow storageRow2 = new ContainerStorageRow(
                numberOfContainerStorageSpaces: 100
                );

            IList<ContainerStorageRow> containerStorageList = new List<ContainerStorageRow>();

            containerStorageList.Add( storageRow1 );
            containerStorageList.Add( storageRow2 );

            
            Harbor kjuttaviga = new Harbor(
                listOfShips: shipsList,
                listOfContainerStorageRows: containerStorageList,
                numberOfSmallLoadingDocks: 2,
                numberOfMediumLoadingDocks: 3,
                numberOfLargeLoadingDocks: 4,
                numberOfCranesNextToLoadingDocks: 2,
                LoadsPerCranePerHour: 2,
                numberOfCranesOnHarborStorageArea: 2,
                numberOfSmallShipDocks: 1,
                numberOfMediumShipDocks: 2,
                numberOfLargeShipDocks: 3,
                numberOfTrucksArriveToHarborPerHour: 10,
                percentageOfContainersDirectlyLoadedFromShipToTrucks: 10,
                percentageOfContainersDirectlyLoadedFromHarborStorageToTrucks: 10,
                numberOfAgvs: 5,
                loadsPerAgvPerHour: 3
                );
            

            // Tar i bruk simplifisert konstruktør overload
            //Harbor kjuttaviga = new(6, 500, 20, 6, 4, 6, 4, 10);


            Simulation sim = new Simulation(
                harbor: kjuttaviga,
                simulationStartTime: startTime,
                simulationEndTime: startTime.AddDays(7)
                );


            sim.SimulationStarting += (sender, e) =>
            {
                SimulationStartingEventArgs args = (SimulationStartingEventArgs)e;
                Console.WriteLine("Simulation starting ...");
                Console.WriteLine($"Simulating {args.HarborToBeSimulated.ID} from {args.StartDate}\n");
                Thread.Sleep(1000);
            };
            sim.ShipDockedtoLoadingDock += (sender, e) =>
            {
                ShipDockedToLoadingDockEventArgs args = (ShipDockedToLoadingDockEventArgs)e;
                Console.WriteLine($"| {args.CurrentTime} | '{args.Ship.Name}' has docked to loading dock with ID {args.DockID}\n");
            };
            sim.ShipAnchored += (sender, e) =>
            {
                ShipAnchoredEventArgs args = (ShipAnchoredEventArgs)e;
                Console.WriteLine($"| {args.CurrentTime} | '{args.Ship.Name}' anchored to anchorage with ID {args.AnchorageID}\n");
            };
            sim.ShipDockedToShipDock += (sender, e) =>
            {
                ShipDockedToShipDockEventArgs args = (ShipDockedToShipDockEventArgs)e;
                Console.WriteLine($"| {args.CurrentTime} | '{args.Ship.Name}' has docked to ship dock with ID '{args.DockID}'\n");
            };
            sim.ShipUndocking += (sender, e) =>
            {
                ShipUndockingEventArgs args = (ShipUndockingEventArgs)e;
                Console.WriteLine($"| {args.CurrentTime} | '{args.Ship.Name}' undocking from location ID '{args.LocationID}'\n");
            };
            sim.ShipInTransit += (sender, e) =>
            {
                ShipInTransitEventArgs args = (ShipInTransitEventArgs)e;
                Console.WriteLine($"| {args.CurrentTime} | ' {args.Ship.Name}' is in transit at transit ID '{args.TransitLocationID}'\n");
            };
            sim.TruckLoadingFromStorage += (sender, e) =>
            {
                TruckLoadingFromStorageEventArgs args = (TruckLoadingFromStorageEventArgs)e;
                Console.WriteLine($"| {args.CurrentTime} | A truck has loaded a container and left the harbor \n");
            };
            sim.ShipUnloadedContainer += (sender, e) =>
            {
                ShipUnloadedContainerEventArgs args = (ShipUnloadedContainerEventArgs)e;
                Console.WriteLine($"| {args.CurrentTime} | '{args.Ship.Name}' unloaded container of size '{args.Container.Size}'\n");

            };
            sim.DayEnded += (sender, e) =>
            {
                DayOverEventArgs args = (DayOverEventArgs)e;

                Console.WriteLine($"-----------------------------------");
                Console.WriteLine($"| {args.CurrentTime} | Day over! |");
                Console.WriteLine($"-----------------------------------");

            };
            sim.ShipLoadedContainer += (sender, e) =>
            {
                ShipLoadedContainerEventArgs args = (ShipLoadedContainerEventArgs)e;
                Console.WriteLine($"| {args.CurrentTime} | '{args.Ship.Name}' loaded container of size '{args.Container.Size}'\n");
            };
            sim.SimulationEnded += (sender, e) =>
            {
                SimulationEndedEventArgs args = (SimulationEndedEventArgs)e;
                Console.WriteLine($"-----------------------------------");
                Console.WriteLine($"|         Simulation over!         |");
                Console.WriteLine($"-----------------------------------");

                Thread.Sleep(1000);
            };

            sim.Run();

            /*
            
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
    
            Harbor clientHarbor = new Harbor(clientShips, storageRows, 1, 1, 1, 7, (((100 * 5) / 24)/7), 10, numberOfSmallShipDocks, 1, numberOfLargeShipDocks,30,15,10,20, 3);

            Simulation clientSim = new Simulation(clientHarbor, clientStartTime, clientEndTime);

            //subscribing to events

            // Det blir mye utskrift i konsoll med abonnering slik med på alle eventene.
            // For eksempel vil ShipLoadedContainer og ShipUnloadedContainer eventene stå for mye utskrift i dette eksempel-prosjektet. (Blir det uoversiktlig kan det være en løsning å kommentere ut akkurat disse to)
            // Som nevnt over representerer dette kun et eksempel på hvordan kunden kan opprette simuleringen og håndtere av event-kallene, og viser dermed hva som er mulig i forhold til eventer.

            clientSim.SimulationStarting += (sender, e) =>
            {
                SimulationStartingEventArgs args = (SimulationStartingEventArgs)e;
                Console.WriteLine("Simulation starting ...");
                Console.WriteLine($"Simulating {args.HarborToBeSimulated.ID} from {args.StartDate}\n");
                Thread.Sleep(1000);
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
                Console.WriteLine($"| {args.CurrentTime} | '{args.Ship.Name}' is starting the unloading process. Containers onboard: {args.Ship.ContainersOnBoard.Count}'\n");

            };

            clientSim.ShipUnloadedContainer += (sender, e) =>
            {
                ShipUnloadedContainerEventArgs args = (ShipUnloadedContainerEventArgs)e;
                Console.WriteLine($"| {args.CurrentTime} | '{args.Ship.Name}' unloaded container of size '{args.Container.Size}'\n");

            };

            clientSim.ShipDoneUnloading += (sender, e) =>
            {
                ShipDoneUnloadingEventArgs args = (ShipDoneUnloadingEventArgs)e;
                Console.WriteLine($"| {args.CurrentTime} | '{args.Ship.Name}' has finished unloading. - Containers onboard: {args.Ship.ContainersOnBoard.Count}\n");

            };

            clientSim.ShipStartingLoading += (sender, e) =>
            {
                ShipStartingLoadingEventArgs args = (ShipStartingLoadingEventArgs)e;
                Console.WriteLine($"| {args.CurrentTime} | '{args.Ship.Name}' is starting the loading process. Containers onboard: {args.Ship.ContainersOnBoard.Count}\n");

            };

            clientSim.ShipLoadedContainer += (sender, e) =>
            {
                ShipLoadedContainerEventArgs args = (ShipLoadedContainerEventArgs)e;
                Console.WriteLine($"| {args.CurrentTime} | '{args.Ship.Name}' loaded container of size '{args.Container.Size}'\n");
            };

            clientSim.ShipDoneLoading += (sender, e) =>
            {
                ShipDoneLoadingEventArgs args = (ShipDoneLoadingEventArgs)e;
                Console.WriteLine($"| {args.CurrentTime} | '{args.Ship.Name}' has finished loading. - Containers onboard: {args.Ship.ContainersOnBoard.Count}\n");

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

            clientSim.TruckLoadingFromStorage += (sender, e) =>
            {
                TruckLoadingFromStorageEventArgs args = (TruckLoadingFromStorageEventArgs)e;
                Console.WriteLine($"| {args.CurrentTime} | A truck has loaded a container and left the harbor \n");
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

            //unsubscribing from event
            clientSim.SimulationStarting -= (sender, e) =>
            {
                SimulationStartingEventArgs args = (SimulationStartingEventArgs)e;
                Console.WriteLine("Simulation starting ...");
                Console.WriteLine($"Simulating {args.HarborToBeSimulated.ID} from {args.StartDate}\n");
                Thread.Sleep(2000);
            };

            */
        }
    }
}