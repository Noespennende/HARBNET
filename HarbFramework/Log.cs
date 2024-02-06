﻿using harbNet;
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
        public IList<Ship> DockedShips { get; set; }
        public IList<Ship> ShipsInAnchorage { get; internal set; }
        public IList<Ship> ShipsInTransit { get; internal set; }
        public IList<Guid> ContainersInHarbour { get; set;}
        //la de til for kompilering
        public IList<Ship> ShipsDockedInLoadingDocks => throw new NotImplementedException();

        public IList<Ship> ShipsDockedInShippingDocks => throw new NotImplementedException();

        IList<Container> ILog.ContainersInHarbour => throw new NotImplementedException();

        /*
        IList<Ship> DockedShips()
        {
            List<Ship> dockedShipsList = new List<Ship>();
            foreach (var ship in DockedShips.Values)
            {
                dockedShipsList.Add((Ship)ship);
            }
            return dockedShipsList;
        }

        IList<Guid> ILog.ContainersInHarbour()
        {
            List<Guid> containersList = new List<Guid>();
            foreach(Container container in ContainersInHarbour.Values)
            {
                containersList.Add(container.ID);
            }
            return containersList;
        }
        */
    }
}
