using harbNet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarbFramework
{
    internal class Log
    {
        DateTime time { get; set; }
        Hashtable dockedShips { get; set; }
        ArrayList shipsInQueue { get; set; }
        ArrayList shipsInTransit { get; set; }
        Hashtable containersInHarbour { get; set; }

        public Log(DateTime time, Hashtable dockedShips, ArrayList shipsInQueue, Hashtable containersInHarbour, ArrayList shipsInTransit ) { 
            this.time = time;
            this.containersInHarbour = containersInHarbour;
            this.shipsInQueue = shipsInQueue;
            this.shipsInTransit = shipsInTransit;
            this.dockedShips = dockedShips;
        }
    }
}
