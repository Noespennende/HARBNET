using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gruppe8.HarbNet.PublicApiAbstractions;

namespace Gruppe8.HarbNet
{
    /// <summary>
    /// Daily logs to be stored in a simulations history. Holds information about the state of the harbor on a specific day of a simulation.
    /// </summary>
    public class DailyLog : HistoryRecord
    {
        /// <summary>
        /// Gets the date and time the DailyLog's info were logged.
        /// </summary>
        /// <returns>Returns a DateTime object representing the date and time the info were logged</returns>
        public override DateTime Time { get; internal set; }
        /// <summary>
        /// Gets a ReadOnlyCollection of ship objects containting information of all ships in anchorage at the date and time when the DailyLog object were created.
        /// </summary>
        /// <returns>Returns a ReadOnlyCollection with Ship object representing all the ships in anchorage when the DailyLog object was created.</returns>
        public override ReadOnlyCollection<Ship> ShipsInAnchorage { get; }
        /// <summary>
        /// Gets a ReadOnlyCollection of ship objects containing information of all the ships in transit when the DailyLog object were created.
        /// </summary>
        /// <returns>Returns a ReadOnlyCollection with Ship object representing the ships in transit when the DailyLog object was created.</returns>
        public override ReadOnlyCollection<Ship> ShipsInTransit { get; }
        /// <summary>
        /// Gets a ReadOnlyCollection of container objects containing information of all the containers stored in harbour when the DailyLog object were created.
        /// </summary>
        /// <returns>Returns a ReadOnlyCollection with Container object representing the containers stored in when the DailyLog object was created.</returns>
        public override ReadOnlyCollection<Container> ContainersInHarbour { get; }
        /// <summary>
        /// Gets a ReadOnlyCollection of ship objects containing information of all the ships docked in a loading dock when the DailyLog object were created.
        /// </summary>
        /// <returns>Returns a ReadOnlyCollection with Ship object representing the docked in a loading dock when the DailyLog object was created.</returns>
        public override ReadOnlyCollection<Ship> ShipsDockedInLoadingDocks { get; }
        /// <summary>
        /// Gets a ReadOnlyCollection of ship objects containing information of all the ships docked to ship docks when the DailyLog object were created.
        /// </summary>
        /// <returns>Returns a ReadOnlyCollection with Ship object representing the ships docked to ship docks when the DailyLog object was created.</returns>
        public override ReadOnlyCollection<Ship> ShipsDockedInShipDocks { get; }
        /// <summary>
        /// Gets a ReadOnlyCollection of container objects containing information of all the containers that have arrived to their destination during the simulation.
        /// </summary>
        /// <returns>Returns a ReadOnlyCollection of container objects containing information of all the containers that have arrived to their destination during the simulation.</returns>
        public override ReadOnlyCollection<Container> ContainersArrivedAtDestination { get; }

        /// <summary>
        /// Creates a Dailylog object which holds information about the state of the simulation at a specific day.
        /// </summary>
        /// <param name="time">The date and time the information is being logged.</param>
        /// <param name="shipsInAnchorage">An IList with ship objects containing all the ships in anchorage at the time given.</param>
        /// <param name="shipsInTransit">An IList with ship objects containing all the ships in transit at the time given.</param>
        /// <param name="containersInHarbour">An IList with container objects with all the containers stored in harbour at the time given.</param>
        /// <param name="containersAtDestination">An IList with container objects with all the containers stored in their target destination at the time given.</param>
        /// <param name="shipsDockedInLoadingDocks">An IList with ship objects containing all the ships docked in loading docks at the time given.</param>
        /// <param name="ShipsDockedInShipDocks">An IList with ship objects containing all the ships docked in ship docks at the time given.</param>
        internal DailyLog(DateTime time, IList<Ship> shipsInAnchorage, IList<Ship> shipsInTransit, IList<Container> containersInHarbour, IList<Container> containersAtDestination, IList<Ship> shipsDockedInLoadingDocks, IList<Ship> ShipsDockedInShipDocks)
        {
            this.Time = time;

            this.ShipsInAnchorage = new ReadOnlyCollection<Ship>(DuplicateShipList(shipsInAnchorage));

            this.ShipsInTransit = new ReadOnlyCollection<Ship>(DuplicateShipList(shipsInTransit));

            this.ShipsDockedInLoadingDocks = new ReadOnlyCollection<Ship>(DuplicateShipList(shipsDockedInLoadingDocks));

            this.ShipsDockedInShipDocks = new ReadOnlyCollection<Ship>(DuplicateShipList(shipsDockedInLoadingDocks));

            this.ContainersInHarbour = new ReadOnlyCollection<Container>(DuplicateContainerList(containersInHarbour));

            this.ContainersArrivedAtDestination = new ReadOnlyCollection<Container> (DuplicateContainerList(containersAtDestination));

        }

        /// <summary>
        /// Duplicate a shipList making copies of all objects in it in a new list.
        /// </summary>
        /// <param name="shipListToDuplicate">The IList containing Ship objects to be duplicated.</param>
        /// <returns>Returns a Collection containing Ship objects, which are duplications of all objects in the shipListToDuplicate.</returns>
        private Collection <Ship> DuplicateShipList(IList<Ship> shipListToDuplicate)
        {
            Collection<Ship> duplicatedList = new Collection<Ship>();

            foreach (Ship ship in shipListToDuplicate)
            {
                IList<Container> containerList = new List<Container>();
                IList<StatusRecord> eventList = new List<StatusRecord>();

                foreach (Container container in ship.ContainersOnBoard)
                {
                    IList<StatusLog> containersHistory = new List<StatusLog>();
                    foreach (StatusLog containerEvent in container.HistoryIList)
                    {
                        containersHistory.Add(new StatusLog(containerEvent.Subject, containerEvent.SubjectLocation, containerEvent.PointInTime, containerEvent.Status));
                    }
                    containerList.Add(new Container(container.Size, container.WeightInTonn, ship.ID, container.ID, containersHistory));
                }
                foreach (StatusLog eventObject in ship.HistoryIList)
                {
                    eventList.Add(new StatusLog(eventObject.Subject, eventObject.SubjectLocation, eventObject.PointInTime, eventObject.Status));
                }

                duplicatedList.Add(new Ship(ship.Name, ship.ShipSize, ship.StartDate, ship.IsForASingleTrip, ship.RoundTripInDays, ship.ID, containerList, eventList));
            }
            return duplicatedList;
        }

        /// <summary>
        /// Duplicate a container list making copies of all objects in it in a new list.
        /// </summary>
        /// <param name="containersToDuplicate">The IList containing Container objects to be duplicated.</param>
        /// <returns>Returns a Collection containing Container objects, which are duplications of all objects in the containersToDuplicate.</returns>
        private Collection<Container> DuplicateContainerList(IList<Container> containersToDuplicate)
        {
            Collection<Container> duplicatedList = new Collection<Container>();

            foreach (Container container in containersToDuplicate)
            {
                IList<StatusLog> eventList = new List<StatusLog>();
                foreach (StatusLog containerEvent in container.HistoryIList)
                {
                    eventList.Add(new StatusLog(containerEvent.Subject, containerEvent.SubjectLocation, containerEvent.PointInTime, containerEvent.Status));
                }
                duplicatedList.Add(new Container(container.Size, container.WeightInTonn, container.CurrentPosition, container.ID, eventList));
            }
            return duplicatedList;
        }

        /// <summary>
        /// Prints the wereabouts and info regarding all the ships in the DailyLog.
        /// </summary>
        public override void PrintInfoForAllShips()
        {
            Console.WriteLine("\n---------------------------------");
            Console.WriteLine("DATE:" + Time.ToString());
            Console.WriteLine("---------------------------------");


            Console.WriteLine("\nSHIPS IN ANCHORAGE:");

            if (ShipsInAnchorage.Count > 0)
            {
                foreach (Ship ship in ShipsInAnchorage)
                {
                    Console.WriteLine("NAME: " + ship.Name + ", SIZE: " + ship.ShipSize + ", STATUS: " + ship.GetCurrentStatus() +
                        ", MAX WEIGHT: " + ship.MaxWeightInTonn + "tonns " + ", CURRENT WEIGHT: " + ship.CurrentWeightInTonn + " tonns" + ", CONTAINER CAPACITY: " + ship.ContainerCapacity + ", CONTAINERS ONBOARD: " + ship.ContainersOnBoard.Count + ", ID: " + ship.ID);
                }
            } else
            {
                Console.WriteLine("\nNO SHIPS IN ANCHORAGE");
            }

            if (ShipsInTransit.Count > 0) 
                {
                    Console.WriteLine("\nSHIPS IN TRANSIT:");
                    foreach (Ship ship in ShipsInTransit)
                    {
                        Console.WriteLine("NAME: " + ship.Name + ", SIZE: " + ship.ShipSize + ", STATUS: " + ship.GetCurrentStatus() +
                            ", MAX WEIGHT: " + ship.MaxWeightInTonn + "tonns " + ", CURRENT WEIGHT: " + ship.CurrentWeightInTonn + " tonns" + ", CONTAINER CAPACITY: " + ship.ContainerCapacity + ", CONTAINERS ONBOARD: " + ship.ContainersOnBoard.Count + ", ID: " + ship.ID);
                    }
                } else
            {
                Console.WriteLine("\nNO SHIPS IN TRANSIT");
            }

            if (ShipsDockedInLoadingDocks.Count > 0)
            {
                Console.WriteLine("\nSHIPS IN LOADING DOCK:");
                foreach (Ship ship in ShipsDockedInLoadingDocks)
                {
                    Console.WriteLine("NAME: " + ship.Name + ", SIZE: " + ship.ShipSize + ", STATUS: " + ship.GetCurrentStatus() +
                        ", MAX WEIGHT: " + ship.MaxWeightInTonn + "tonns " + ", CURRENT WEIGHT: " + ship.CurrentWeightInTonn + " tonns" + ", CONTAINER CAPACITY: " + ship.ContainerCapacity + ", CONTAINERS ONBOARD: " + ship.ContainersOnBoard.Count + ", ID: " + ship.ID);
                }
            } else
            {
                Console.WriteLine("\nNO SHIPS DOCKED IN LOADING DOCKS");
            }

            
            if (ShipsDockedInShipDocks.Count > 0)
            {
                Console.WriteLine("\nSHIPS IN SHIP DOCK:");
                foreach (Ship ship in ShipsDockedInShipDocks)
                {
                    Console.WriteLine("NAME: " + ship.Name + ", SIZE: " + ship.ShipSize + ", STATUS: " + ship.GetCurrentStatus() +
                        ", MAX WEIGHT: " + ship.MaxWeightInTonn + "tonns " + ", CURRENT WEIGHT: " + ship.CurrentWeightInTonn + " tonns" + ", CONTAINER CAPACITY: " + ship.ContainerCapacity + ", CONTAINERS ONBOARD: " + ship.ContainersOnBoard.Count + ", ID: " + ship.ID);
                }
            } else
            {
                Console.WriteLine("\nNO SHIPS DOCKED IN SHIP DOCKS");
            }

        }
        /// <summary>
        /// Prints the wereabouts and info regarding all the containers in the DailyLog.
        /// </summary>
        public override void PrintInfoForAllContainers()
        {
            Console.WriteLine("\n---------------------------------");
            Console.WriteLine("DATE:" + Time.ToString());
            Console.WriteLine("---------------------------------");


            if (ShipsInAnchorage.Count > 0)
            {
                Console.WriteLine("\nCONTAINERS ONBOARD SHIPS IN ANCHORAGE:");
                bool infoPrinted = false;
                foreach (Ship ship in ShipsInAnchorage)
                {
                    if (ship.ContainersOnBoard.Count > 0)
                    {
                        infoPrinted = true;
                        Console.WriteLine("\nSHIP NAME: " + ship.Name + ", SHIP ID: " + ship.ID.ToString());

                        foreach (Container container in ship.ContainersOnBoard)
                        {
                            Console.WriteLine("CONTAINER SIZE: " + container.Size + ", WEIGHT: " + container.WeightInTonn + "tonns" + ", STATUS: " + container.GetCurrentStatus() + ", ID: " + container.ID);
                        }
                    }

                }
                if (!infoPrinted)
                {
                    Console.WriteLine("NONE");
                }
            }
            else
            {
                Console.WriteLine("\nNO CONTAINERS ONBOARD SHIPS IN ANCHORAGE");
            }


            if (ShipsInTransit.Count > 0)
            {
                Console.WriteLine("\nCONTAINERS ONBOARD SHIPS IN TRANSIT:");
                bool infoPrinted = false;
                foreach (Ship ship in ShipsDockedInLoadingDocks)
                {
                    if (ship.ContainersOnBoard.Count > 0)
                    {
                        infoPrinted = true;
                        Console.WriteLine("\nSHIP NAME: " + ship.Name + ", SHIP ID: " + ship.ID.ToString());

                        foreach (Container container in ship.ContainersOnBoard)
                        {
                            Console.WriteLine("CONTAINER SIZE: " + container.Size + ", WEIGHT: " + container.WeightInTonn + "tonns" + ", STATUS: " + container.GetCurrentStatus() + ", ID: " + container.ID);
                        }
                    }

                }
                if (!infoPrinted)
                {
                    Console.WriteLine("NONE");
                }
            }
            else
            {
                Console.WriteLine("\nNO CONTAINERS ONBOARD SHIPS IN TRANSIT");
            }

            if (ShipsDockedInLoadingDocks.Count > 0)
            {
                Console.WriteLine("\nCONTAINERS ONBOARD SHIPS IN LOADING DOCKS:");
                bool infoPrinted = false;
                foreach (Ship ship in ShipsDockedInLoadingDocks)
                {
                    if (ship.ContainersOnBoard.Count > 0)
                    {
                        infoPrinted = true;
                        Console.WriteLine("\nSHIP NAME: " + ship.Name + ", SHIP ID: " + ship.ID.ToString());

                        foreach (Container container in ship.ContainersOnBoard)
                        {
                            Console.WriteLine("CONTAINER SIZE: " + container.Size + ", WEIGHT: " + container.WeightInTonn + "tonns" + ", STATUS: " + container.GetCurrentStatus() + ", ID: " + container.ID);
                        }
                    }

                }
                if (!infoPrinted)
                {
                    Console.WriteLine("NONE");
                }
            }
            else
            {
                Console.WriteLine("\nNO CONTAINERS ONBOARD SHIPS IN LOADING DOCKS");
            }

            if (ContainersInHarbour.Count > 0)
            {
                Console.WriteLine("\nCONTAINERS IN HARBOR STORAGE:");

                foreach (Container container in ContainersInHarbour)
                {
                    Console.WriteLine("CONTAINER SIZE: " + container.Size + ", WEIGHT: " + container.WeightInTonn + "tonns" + ", STATUS: " + container.GetCurrentStatus() + ", ID: " + container.ID);
                }
                        
            }
            else
            {
                Console.WriteLine("\nNO CONTAINERS IN HARBOR STORAGE");
            }

            if (ContainersArrivedAtDestination.Count > 0)
            {
                Console.WriteLine("\nCONTAINERS ARRIVED AT THEIR DESTINATION:" + "\n");

                foreach (Container container in ContainersArrivedAtDestination)
                {
                    Console.WriteLine("CONTAINER SIZE: " + container.Size + ", WEIGHT: " + container.WeightInTonn + "tonns" + ", STATUS: " + container.GetCurrentStatus() + ", ID: " + container.ID + "\n");
                }

            }
            else
            {
                Console.WriteLine("\nNO CONTAINERS ARRIVED TO THEIR DESTINATION");
            }
        }

        /// <summary>
        /// Returns a string that contains information about all ships on the given day of a simulation.
        /// </summary>
        /// <returns>Returns a String containing information about all ships on the given day of a simulation.</returns>
        public override String HistoryToString()
        {
            StringBuilder sb = new StringBuilder();


            sb.Append("---------------------------------\n");
            sb.Append("DATE:" + Time.ToString() + "\n");
            sb.Append("---------------------------------\n");


            sb.Append("\nSHIPS IN ANCHORAGE:\n");

            if (ShipsInAnchorage.Count > 0)
            {
                foreach (Ship ship in ShipsInAnchorage)
                {
                    sb.Append("NAME: " + ship.Name + ", SIZE: " + ship.ShipSize + ", STATUS: " + ship.GetCurrentStatus() +
                        ", MAX WEIGHT: " + ship.MaxWeightInTonn + "tonns " + ", CURRENT WEIGHT: " + ship.CurrentWeightInTonn + " tonns" + ", CONTAINER CAPACITY: " + ship.ContainerCapacity + ", CONTAINERS ONBOARD: " + ship.ContainersOnBoard.Count + ", ID: " + ship.ID + "\n");
                }
            }
            else
            {
                sb.Append("\nNO SHIPS IN ANCHORAGE" + "\n");
            }

            if (ShipsInTransit.Count > 0)
            {
                sb.Append("\nSHIPS IN TRANSIT:" + "\n");
                foreach (Ship ship in ShipsInTransit)
                {
                    sb.Append("\nNAME: " + ship.Name + ", SIZE: " + ship.ShipSize + ", STATUS: " + ship.GetCurrentStatus() +
                        ", MAX WEIGHT: " + ship.MaxWeightInTonn + "tonns " + ", CURRENT WEIGHT: " + ship.CurrentWeightInTonn + " tonns" + ", CONTAINER CAPACITY: " + ship.ContainerCapacity + ", CONTAINERS ONBOARD: " + ship.ContainersOnBoard.Count + ", ID: " + ship.ID + "\n");
                }
            }
            else
            {
                sb.Append("\nNO SHIPS IN TRANSIT" + "\n");
            }

            if (ShipsDockedInLoadingDocks.Count > 0)
            {
                sb.Append("\nSHIPS IN LOADING DOCK:");
                foreach (Ship ship in ShipsDockedInLoadingDocks)
                {
                    sb.Append("\nNAME: " + ship.Name + ", SIZE: " + ship.ShipSize + ", STATUS: " + ship.GetCurrentStatus() +
                        ", MAX WEIGHT: " + ship.MaxWeightInTonn + "tonns " + ", CURRENT WEIGHT: " + ship.CurrentWeightInTonn + " tonns" + ", CONTAINER CAPACITY: " + ship.ContainerCapacity + ", CONTAINERS ONBOARD: " + ship.ContainersOnBoard.Count + ", ID: " + ship.ID + "\n");
                }
            }
            else
            {
                sb.Append("\nNO SHIPS DOCKED IN LOADING DOCKS" + "\n");
            }


            if (ShipsDockedInShipDocks.Count > 0)
            {
                sb.Append("\nSHIPS IN SHIP DOCK:" + "\n");
                foreach (Ship ship in ShipsDockedInShipDocks)
                {
                    sb.Append("\nNAME: " + ship.Name + ", SIZE: " + ship.ShipSize + ", STATUS: " + ship.GetCurrentStatus() +
                        ", MAX WEIGHT: " + ship.MaxWeightInTonn + "tonns " + ", CURRENT WEIGHT: " + ship.CurrentWeightInTonn + " tonns" + ", CONTAINER CAPACITY: " + ship.ContainerCapacity + ", CONTAINERS ONBOARD: " + ship.ContainersOnBoard.Count + ", ID: " + ship.ID + "\n");
                }
            }
            else
            {
                sb.Append("\nNO SHIPS DOCKED IN SHIP DOCKS" + "\n");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Returns a string that contains information about all ships or containers on the given day of a simulation.
        /// </summary>
        /// <param name="ShipsOrContainers">User can choose to write either "ships" or "containers" as input. "ships" returns information on all ships, "containers" return information on all containers</param>
        /// <returns>Returns a String containing information about all ships or containers on the given day of a simulation.</returns>
        public override String HistoryToString(String ShipsOrContainers) 
        {
            if (ShipsOrContainers.ToLower().Equals("ships") || ShipsOrContainers.ToLower().Equals("ship"))
            {
                return HistoryToString();
            }
            else if (ShipsOrContainers.ToLower().Equals("containers") || ShipsOrContainers.ToLower().Equals("container"))
            {

                StringBuilder sb = new StringBuilder();

                sb.Append("\n---------------------------------");
                sb.Append("DATE:" + Time.ToString());
                sb.Append("---------------------------------");


                if (ShipsInAnchorage.Count > 0)
                {
                    sb.Append("\nCONTAINERS ONBOARD SHIPS IN ANCHORAGE:" + "\n");
                    bool infoPrinted = false;
                    foreach (Ship ship in ShipsInAnchorage)
                    {
                        if (ship.ContainersOnBoard.Count > 0)
                        {
                            infoPrinted = true;
                            sb.Append("\nSHIP NAME: " + ship.Name + ", SHIP ID: " + ship.ID.ToString() + "\n");

                            foreach (Container container in ship.ContainersOnBoard)
                            {
                                sb.Append("CONTAINER SIZE: " + container.Size + ", WEIGHT: " + container.WeightInTonn + "tonns" + ", STATUS: " + container.GetCurrentStatus() + ", ID: " + container.ID + "\n");
                            }
                        }

                    }
                    if (!infoPrinted)
                    {
                        sb.Append("NONE" + "\n");
                    }
                }
                else
                {
                    sb.Append("\nNO CONTAINERS ONBOARD SHIPS IN ANCHORAGE" + "\n");
                }


                if (ShipsInTransit.Count > 0)
                {
                    sb.Append("\nCONTAINERS ONBOARD SHIPS IN TRANSIT:" + "\n");
                    bool infoPrinted = false;
                    foreach (Ship ship in ShipsDockedInLoadingDocks)
                    {
                        if (ship.ContainersOnBoard.Count > 0)
                        {
                            infoPrinted = true;
                            sb.Append("\nSHIP NAME: " + ship.Name + ", SHIP ID: " + ship.ID.ToString() + "\n");

                            foreach (Container container in ship.ContainersOnBoard)
                            {
                                sb.Append("CONTAINER SIZE: " + container.Size + ", WEIGHT: " + container.WeightInTonn + "tonns" + ", STATUS: " + container.GetCurrentStatus() + ", ID: " + container.ID + "\n");
                            }
                        }

                    }
                    if (!infoPrinted)
                    {
                        sb.Append("NONE" + "\n");
                    }
                }
                else
                {
                    sb.Append("\nNO CONTAINERS ONBOARD SHIPS IN TRANSIT" + "\n");
                }

                if (ShipsDockedInLoadingDocks.Count > 0)
                {
                    sb.Append("\nCONTAINERS ONBOARD SHIPS IN LOADING DOCKS:" + "\n");
                    bool infoPrinted = false;
                    foreach (Ship ship in ShipsDockedInLoadingDocks)
                    {
                        if (ship.ContainersOnBoard.Count > 0)
                        {
                            infoPrinted = true;
                            sb.Append("\nSHIP NAME: " + ship.Name + ", SHIP ID: " + ship.ID.ToString() + "\n");

                            foreach (Container container in ship.ContainersOnBoard)
                            {
                                sb.Append("CONTAINER SIZE: " + container.Size + ", WEIGHT: " + container.WeightInTonn + "tonns" + ", STATUS: " + container.GetCurrentStatus() + ", ID: " + container.ID + "\n");
                            }
                        }

                    }
                    if (!infoPrinted)
                    {
                        sb.Append("NONE" + "\n");
                    }
                }
                else
                {
                    sb.Append("\nNO CONTAINERS ONBOARD SHIPS IN LOADING DOCKS" + "\n");
                }

                if (ContainersInHarbour.Count > 0)
                {
                    sb.Append("\nCONTAINERS IN HARBOR STORAGE:" + "\n");

                    foreach (Container container in ContainersInHarbour)
                    {
                        sb.Append("CONTAINER SIZE: " + container.Size + ", WEIGHT: " + container.WeightInTonn + "tonns" + ", STATUS: " + container.GetCurrentStatus() + ", ID: " + container.ID + "\n");
                    }

                }
                else
                {
                    sb.Append("\nNO CONTAINERS IN HARBOR STORAGE");
                }

                if (ContainersArrivedAtDestination.Count > 0)
                {
                    sb.Append("\nCONTAINERS ARRIVED AT THEIR DESTINATION:" + "\n");

                    foreach (Container container in ContainersArrivedAtDestination)
                    {
                        sb.Append("CONTAINER SIZE: " + container.Size + ", WEIGHT: " + container.WeightInTonn + "tonns" + ", STATUS: " + container.GetCurrentStatus() + ", ID: " + container.ID + "\n");
                    }

                }
                else
                {
                    sb.Append("\nNO CONTAINERS ARRIVED TO THEIR DESTINATION");
                }

                return sb.ToString();
            } else {
                throw new ArgumentException("Invalid input. Valid input is 'ships' or 'container'.", nameof(ShipsOrContainers));
            }
  
        }

        /// <summary>
        /// Returns a string with the date and time, amount of ships in anchorage, amount of ships docked in loading docks, amount of ships docked in ship dock, amount of ships in transit, amount of containers in harbor and amount of containers that have arrived at their destination.
        /// </summary>
        /// <returns>Returns a String containing the time the DailyLog object represents and the number of ships in all locations.</returns>
        public override string ToString()
        {
            return ($"Time: {Time.ToString()}, Ships in anchorage {ShipsInAnchorage.Count}, Ships in loading docks: {ShipsDockedInLoadingDocks.Count}, Ships in ship dock: {ShipsDockedInShipDocks.Count}, Ships in transit: " +
                $"{ShipsInTransit.Count}, Containers in harbor: {ContainersInHarbour.Count}, Containers arrived at their destination: {ContainersArrivedAtDestination.Count}");
        }

    }
}
