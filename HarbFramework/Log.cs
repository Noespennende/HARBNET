using harbNet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarbFramework
{
    public class Log : ILog
    {
        /// <summary>
        /// Gets the date and time the logs info were logged.
        /// </summary>
        /// <returns>Returns a DateTime object representing the date and time the info were logged</returns>
        public DateTime Time { get; internal set; }
        /// <summary>
        /// Gets all ships in anchorage when the log object were created.
        /// </summary>
        /// <returns>Returns a an Ilist with ship object representing all the ships in anchorage when the log object was created</returns>
        public IList<Ship> ShipsInAnchorage { get; internal set; }
        /// <summary>
        /// Gets all the ships in transit when the log object were created
        /// </summary>
        /// <returns>Returns a IList with Ship object representing the ships in transit when the log object was created</returns>
        public IList<Ship> ShipsInTransit { get; internal set; }
        /// <summary>
        /// Gets all the containers stored in harbour when the log object were created
        /// </summary>
        /// <returns>Returns a IList with Container object representing the containers stored in when the log object was created</returns>
        public IList<Container> ContainersInHarbour { get; internal set; }
        /// <summary>
        /// Gets all the ships docked in a loading dock when the log object were created
        /// </summary>
        /// <returns>Returns a IList with Ship object representing the docked in a loading dock when the log object was created</returns>
        public IList<Ship> ShipsDockedInLoadingDocks { get; internal set; }
        /// <summary>
        /// Gets all the ships docked to ship docks when the log object were created
        /// </summary>
        /// <returns>Returns a IList with Ship object representing the ships docked to ship docks when the log object was created</returns>
        public IList<Ship> ShipsDockedInShipDocks { get; internal set; }

        /// <summary>
        /// Creates a log object
        /// </summary>
        /// <param name="time">The date and time of the information being logged</param>
        /// <param name="shipsInTransit">All the ships in transit at the time given</param>
        /// <param name="containersInHarbour">All the containers stored in harbour at the time given</param>
        /// <param name="shipsDockedInLoadingDocks">All the ships docked in loading docks at the time given</param>
        /// <param name="ShipsDockedInShipDocks">All the ships docked in ship docks at the time given</param>
        internal Log(DateTime time, IList<Ship> shipsInAnchorage, IList<Ship> shipsInTransit, IList<Container> containersInHarbour, IList<Ship> shipsDockedInLoadingDocks, IList<Ship> ShipsDockedInShipDocks)
        {
            this.Time = time;
            this.ShipsInAnchorage = shipsInAnchorage;
            this.ShipsDockedInLoadingDocks = shipsDockedInLoadingDocks;
            this.ShipsInTransit = shipsInTransit;
            this.ContainersInHarbour = containersInHarbour;
            this.ShipsDockedInLoadingDocks = shipsDockedInLoadingDocks;
            this.ShipsDockedInShipDocks = ShipsDockedInShipDocks;
        }

        /// <summary>
        /// Prints the wereabouts and info regarding all the ships in the log.
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
        /// Prints the wereabouts and info regarding all the containers in the log.
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

    }
}
