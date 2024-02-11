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
        public DateTime Time { get; set; }
        public IList<Ship> ShipsInAnchorage { get; internal set; }
        public IList<Ship> ShipsInTransit { get; internal set; }
        public IList<Container> ContainersInHarbour { get; internal set; }
        public IList<Ship> ShipsDockedInLoadingDocks { get; internal set; }
        public IList<Ship> ShipsDockedInShipDocks { get; internal set; }

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
                    Console.WriteLine("NAME: " + ship.ShipName + ", SIZE: " + ship.ShipSize + "STATUS: " + ship.getCurrentStatus() +
                        ", MAX WEIGHT: " + ship.MaxWeightInTonn + "tonns " + ", CURRENT WEIGHT: " + ship.CurrentWeightInTonn + " tonns" + ", CONTAINER CAPACITY: " + ship.ContainerCapacity + ", CONTAINERS ONBOARD: " + ship.ContainersOnBoard.Count + ", ID: " + ship.ID);
                }
            } else
            {
                Console.WriteLine("\n NO SHIPS IN ANCHORAGE");
            }

            if (ShipsInTransit.Count > 0) 
                {
                    Console.WriteLine("\nSHIPS IN TRANSIT:");
                    foreach (Ship ship in ShipsInTransit)
                    {
                        Console.WriteLine("NAME: " + ship.ShipName + ", SIZE: " + ship.ShipSize + "STATUS: " + ship.getCurrentStatus() +
                            ", MAX WEIGHT: " + ship.MaxWeightInTonn + "tonns " + ", CURRENT WEIGHT: " + ship.CurrentWeightInTonn + " tonns" + ", CONTAINER CAPACITY: " + ship.ContainerCapacity + ", CONTAINERS ONBOARD: " + ship.ContainersOnBoard.Count + ", ID: " + ship.ID);
                    }
                } else
            {
                Console.WriteLine("\n NO SHIPS IN TRANSIT");
            }

            if (ShipsDockedInLoadingDocks.Count > 0)
            {
                Console.WriteLine("\nSHIPS IN LOADING DOCK:");
                foreach (Ship ship in ShipsInTransit)
                {
                    Console.WriteLine("NAME: " + ship.ShipName + ", SIZE: " + ship.ShipSize + "STATUS: " + ship.getCurrentStatus() +
                        ", MAX WEIGHT: " + ship.MaxWeightInTonn + "tonns " + ", CURRENT WEIGHT: " + ship.CurrentWeightInTonn + " tonns" + ", CONTAINER CAPACITY: " + ship.ContainerCapacity + ", CONTAINERS ONBOARD: " + ship.ContainersOnBoard.Count + ", ID: " + ship.ID);
                }
            } else
            {
                Console.WriteLine("\n NO SHIPS DOCKED IN LOADING DOCKS");
            }

            
            if (ShipsDockedInShipDocks.Count > 0)
            {
                Console.WriteLine("\nSHIPS IN SHIP DOCK:");
                foreach (Ship ship in ShipsDockedInShipDocks)
                {
                    Console.WriteLine("NAME: " + ship.ShipName + ", SIZE: " + ship.ShipSize + "STATUS: " + ship.getCurrentStatus() +
                        ", MAX WEIGHT: " + ship.MaxWeightInTonn + "tonns " + ", CURRENT WEIGHT: " + ship.CurrentWeightInTonn + " tonns" + ", CONTAINER CAPACITY: " + ship.ContainerCapacity + ", CONTAINERS ONBOARD: " + ship.ContainersOnBoard.Count + ", ID: " + ship.ID);
                }
            } else
            {
                Console.WriteLine("\n NO SHIPS DOCKED IN SHIP DOCKS");
            }

        }

        public void PrintInfoForAllContainers()
        {
            Console.WriteLine("\n---------------------------------");
            Console.WriteLine("DATE:" + Time.ToString());
            Console.WriteLine("---------------------------------");


            if (ShipsInAnchorage.Count > 0)
            {
                Console.WriteLine("\n CONTAINERS ONBOARD SHIPS IN ANCHORAGE:");
                bool infoPrinted = false;
                foreach (Ship ship in ShipsInAnchorage)
                {
                    if (ship.ContainersOnBoard.Count > 0)
                    {
                        infoPrinted = true;
                        Console.WriteLine("\nSHIP NAME: " + ship.ShipName + ", SHIP ID: " + ship.ID.ToString());

                        foreach (Container container in ship.ContainersOnBoard)
                        {
                            Console.WriteLine("CONTAINER SIZE: " + container.Size + ", WEIGHT: " + container.WeightInTonn + "tonns" + ", STATUS: " + container.GetStatus() + ", ID: " + container.ID);
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
                Console.WriteLine("\n NO CONTAINERS ONBOARD SHIPS IN ANCHORAGE");
            }


            if (ShipsInTransit.Count > 0)
            {
                Console.WriteLine("\n CONTAINERS ONBOARD SHIPS IN TRANSIT:");
                bool infoPrinted = false;
                foreach (Ship ship in ShipsDockedInLoadingDocks)
                {
                    if (ship.ContainersOnBoard.Count > 0)
                    {
                        infoPrinted = true;
                        Console.WriteLine("\nSHIP NAME: " + ship.ShipName + ", SHIP ID: " + ship.ID.ToString());

                        foreach (Container container in ship.ContainersOnBoard)
                        {
                            Console.WriteLine("CONTAINER SIZE: " + container.Size + ", WEIGHT: " + container.WeightInTonn + "tonns" + ", STATUS: " + container.GetStatus() + ", ID: " + container.ID);
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
                Console.WriteLine("\n NO CONTAINERS ONBOARD SHIPS IN TRANSIT");
            }

            if (ShipsDockedInLoadingDocks.Count > 0)
            {
                Console.WriteLine("\n CONTAINERS ONBOARD SHIPS IN LOADING DOCKS:");
                bool infoPrinted = false;
                foreach (Ship ship in ShipsDockedInLoadingDocks)
                {
                    if (ship.ContainersOnBoard.Count > 0)
                    {
                        infoPrinted = true;
                        Console.WriteLine("\nSHIP NAME: " + ship.ShipName + ", SHIP ID: " + ship.ID.ToString());

                        foreach (Container container in ship.ContainersOnBoard)
                        {
                            Console.WriteLine("CONTAINER SIZE: " + container.Size + ", WEIGHT: " + container.WeightInTonn + "tonns" + ", STATUS: " + container.GetStatus() + ", ID: " + container.ID);
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
                Console.WriteLine("\n NO CONTAINERS ONBOARD SHIPS IN LOADING DOCKS");
            }

            if (ContainersInHarbour.Count > 0)
            {
                Console.WriteLine("\n CONTAINERS IN HARBOR STORAGE:");

                foreach (Container container in ContainersInHarbour)
                {
                    Console.WriteLine("CONTAINER SIZE: " + container.Size + ", WEIGHT: " + container.WeightInTonn + "tonns" + ", STATUS: " + container.GetStatus() + ", ID: " + container.ID);
                }
                        
            }
            else
            {
                Console.WriteLine("\n NO CONTAINERS IN HARBOR STORAGE");
            }
        }

    }
}
