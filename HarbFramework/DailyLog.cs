﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe8.HarbNet
{
    /// <summary>
    /// Daily logs to be stored in a simulations history. Holds information about the state of the harbor on a specific day of a simulation.
    /// </summary>
    public class DailyLog : IDailyLog
    {
        /// <summary>
        /// Gets the date and time the DailyLog's info were logged.
        /// </summary>
        /// <returns>Returns a DateTime object representing the date and time the info were logged</returns>
        public DateTime Time { get; internal set; }
        /// <summary>
        /// Gets all ships in anchorage at the date and time when the DailyLog object were created.
        /// </summary>
        /// <returns>Returns a an Ilist with ship object representing all the ships in anchorage when the DailyLog object was created</returns>
        public ReadOnlyCollection<Ship> ShipsInAnchorage { get; }
        /// <summary>
        /// Gets all the ships in transit when the DailyLog object were created
        /// </summary>
        /// <returns>Returns a IList with Ship object representing the ships in transit when the DailyLog object was created</returns>
        public ReadOnlyCollection<Ship> ShipsInTransit { get; }
        /// <summary>
        /// Gets all the containers stored in harbour when the DailyLog object were created
        /// </summary>
        /// <returns>Returns a IList with Container object representing the containers stored in when the DailyLog object was created</returns>
        public ReadOnlyCollection<Container> ContainersInHarbour { get; }
        /// <summary>
        /// Gets all the ships docked in a loading dock when the DailyLog object were created
        /// </summary>
        /// <returns>Returns a IList with Ship object representing the docked in a loading dock when the DailyLog object was created</returns>
        public ReadOnlyCollection<Ship> ShipsDockedInLoadingDocks { get; }
        /// <summary>
        /// Gets all the ships docked to ship docks when the DailyLog object were created
        /// </summary>
        /// <returns>Returns a IList with Ship object representing the ships docked to ship docks when the DailyLog object was created</returns>
        public ReadOnlyCollection<Ship> ShipsDockedInShipDocks { get; }

        /// <summary>
        /// Creates a Dailylog object which holds information about the state of the simulation at a specific day.
        /// </summary>
        /// <param name="time">The date and time of the information being logged</param>
        /// <param name="shipsInTransit">All the ships in transit at the time given</param>
        /// <param name="containersInHarbour">All the containers stored in harbour at the time given</param>
        /// <param name="shipsDockedInLoadingDocks">All the ships docked in loading docks at the time given</param>
        /// <param name="ShipsDockedInShipDocks">All the ships docked in ship docks at the time given</param>
        internal DailyLog(DateTime time, IList<Ship> shipsInAnchorage, IList<Ship> shipsInTransit, IList<Container> containersInHarbour, IList<Ship> shipsDockedInLoadingDocks, IList<Ship> ShipsDockedInShipDocks)
        {
            this.Time = time;

            this.ShipsInAnchorage = new ReadOnlyCollection<Ship>(DuplicateShipList(shipsInAnchorage));

            this.ShipsInTransit = new ReadOnlyCollection<Ship>(DuplicateShipList(shipsInTransit));

            this.ShipsDockedInLoadingDocks = new ReadOnlyCollection<Ship>(DuplicateShipList(shipsDockedInLoadingDocks));

            this.ShipsDockedInShipDocks = new ReadOnlyCollection<Ship>(DuplicateShipList(shipsDockedInLoadingDocks));

            this.ContainersInHarbour = new ReadOnlyCollection<Container>(DuplicateContainerList(containersInHarbour));

        }

        /// <summary>
        /// Duplicate a shipList making copies of all objects in it.
        /// </summary>
        /// <param name="shipListToDuplicate">list to duplicate</param>
        /// <returns>a Collection duplications of all objects in the List</returns>
        private Collection <Ship> DuplicateShipList(IList<Ship> shipListToDuplicate)
        {
            Collection<Ship> duplicatedList = new Collection<Ship>();

            foreach (Ship ship in shipListToDuplicate)
            {
                IList<Container> containerList = new List<Container>();
                IList<StatusLog> eventList = new List<StatusLog>();

                foreach (Container container in ship.ContainersOnBoard)
                {
                    IList<StatusLog> containersHistory = new List<StatusLog>();
                    foreach (StatusLog containerEvent in container.History)
                    {
                        containersHistory.Add(new StatusLog(containerEvent.Subject, containerEvent.SubjectLocation, containerEvent.PointInTime, containerEvent.Status));
                    }
                    containerList.Add(new Container(container.Size, container.WeightInTonn, ship.ID, container.ID, containersHistory));
                }
                foreach (StatusLog eventObject in ship.History)
                {
                    eventList.Add(new StatusLog(eventObject.Subject, eventObject.SubjectLocation, eventObject.PointInTime, eventObject.Status));
                }

                duplicatedList.Add(new Ship(ship.Name, ship.ShipSize, ship.StartDate, ship.IsForASingleTrip, ship.RoundTripInDays, ship.ID, containerList, eventList));
            }
            return duplicatedList;
        }

        /// <summary>
        /// Duplicate a container list making copies of all objects in it.
        /// </summary>
        /// <param name="containersToDuplicate">list to duplicate</param>
        /// <returns>a collection containing duplications of all objects in the List</returns>
        private Collection<Container> DuplicateContainerList(IList<Container> containersToDuplicate)
        {
            Collection<Container> duplicatedList = new Collection<Container>();

            foreach (Container container in containersToDuplicate)
            {
                IList<StatusLog> eventList = new List<StatusLog>();
                foreach (StatusLog containerEvent in container.History)
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
        public void PrintInfoForAllShips()
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
        public void PrintInfoForAllContainers()
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
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>Returns a String containing information about all ships on the given day of a simulation</returns>
        override public String ToString()
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
        /// Returns a string that represents the current object.
        /// </summary>
        /// <param name="ShipsOrContainers">"ships" returns information on all ships, "containers" return information on all containers</param>
        /// <returns>Returns a String containing information about all ships or containers on the given day of a simulation.</returns>
        public String ToString(String ShipsOrContainers)
        {
            if (ShipsOrContainers.ToLower().Equals("ships") || ShipsOrContainers.ToLower().Equals("ship"))
            {
                return ToString();
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

                return sb.ToString();
            } else {
                return "";
            }
  
        }

    }
}
