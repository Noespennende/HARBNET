using harbNet;
using System;
using System.Collections;
using System.Collections.Generic;
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
        public IList<Ship> ShipsDockedInShippingDocks { get; internal set; }

        internal Log(DateTime time, IList<Ship> shipsInAnchorage, IList<Ship> shipsInTransit, IList<Container> containersInHarbour, IList<Ship> shipsDockedInLoadingDocks, IList<Ship> ShipsDockedInShippingDocks)
        {
            this.Time = time;
            this.ShipsInAnchorage = shipsInAnchorage;
            this.ShipsDockedInLoadingDocks = shipsDockedInLoadingDocks;
            this.ShipsInTransit = shipsInTransit;
            this.ContainersInHarbour = containersInHarbour;
            this.ShipsDockedInLoadingDocks = shipsDockedInLoadingDocks;
            this.ShipsDockedInShippingDocks = ShipsDockedInShippingDocks;
        }

        public void PrintInfoForAllShips()
        {
            Console.WriteLine("---------------------------------");
            Console.WriteLine("DATE:" + Time.ToString());
            Console.WriteLine("---------------------------------");
           

            Console.WriteLine("\nSHIPS IN ANCHORAGE:");


            foreach (Ship ship in ShipsInAnchorage)
            {
                Console.WriteLine("Ship name: " + ship.ShipName + ", Size: " + ship.ShipSize + "Status: " + ship.getCurrentStatus() +
                    ", Max weight (tonns): " + ship.MaxWeightInTonn + ", Current weight (tonns): " + ship.CurrentWeightInTonn + ", Container capacity: " + ship.ContainerCapacity + ", Containers onboard: " + ship.ContainersOnBoard.Count + ", ID: " + ship.ID);
            }

        }

    }
}
