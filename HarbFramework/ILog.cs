using harbNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarbFramework
{
    public interface ILog
    {
        /// <summary>
        /// Gets the date and time the logs info were logged.
        /// </summary>
        /// <returns>Returns a DateTime object representing the date and time the info were logged</returns>
        public DateTime Time { get; }
        /// <summary>
        /// Gets all ships in anchorage when the log object were created.
        /// </summary>
        /// <returns>Returns a an Ilist with ship object representing all the ships in anchorage when the log object was created</returns>
        public IList<Ship> ShipsInAnchorage { get; }
        /// <summary>
        /// Gets all the ships in transit when the log object were created
        /// </summary>
        /// <returns>Returns a IList with Ship object representing the ships in transit when the log object was created</returns>
        public IList<Ship> ShipsInTransit { get; }
        /// <summary>
        /// Gets all the ships docked in a loading dock when the log object were created
        /// </summary>
        /// <returns>Returns a IList with Ship object representing the docked in a loading dock when the log object was created</returns>
        public IList<Ship> ShipsDockedInLoadingDocks { get; }
        /// <summary>
        /// Gets all the ships docked to ship docks when the log object were created
        /// </summary>
        /// <returns>Returns a IList with Ship object representing the ships docked to ship docks when the log object was created</returns>
        public IList<Ship> ShipsDockedInShipDocks { get; }
        /// <summary>
        /// Gets all the containers stored in harbour when the log object were created
        /// </summary>
        /// <returns>Returns a IList with Container object representing the containers stored in when the log object was created</returns>
        public IList<Container> ContainersInHarbour { get; }
        /// <summary>
        /// Prints the wereabouts and info regarding all the ships in the log.
        /// </summary>
        public void PrintInfoForAllShips();
        /// <summary>
        /// Prints the wereabouts and info regarding all the containers in the log.
        /// </summary>
        public void PrintInfoForAllContainers();

    }
}
