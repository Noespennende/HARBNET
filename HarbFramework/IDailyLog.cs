using harbNet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarbFramework
{
    /// <summary>
    /// Interface defining the public API for the Log class
    /// </summary>
    public interface IDailyLog
    {
        /// <summary>
        /// Gets the date and time the logs info were logged.
        /// </summary>
        /// <returns>Returns a DateTime object representing the date and time the info were logged</returns>
        public DateTime Time { get; }
        /// <summary>
        /// Gets all ships in anchorage when the log object were created.
        /// </summary>
        /// <returns>Returns a an ReadOnlyCollection with ship object representing all the ships in anchorage when the log object was created</returns>
        public ReadOnlyCollection<Ship> ShipsInAnchorage { get; }
        /// <summary>
        /// Gets all the ships in transit when the log object were created
        /// </summary>
        /// <returns>Returns a ReadOnlyCollection with Ship object representing the ships in transit when the log object was created</returns>
        public ReadOnlyCollection<Ship> ShipsInTransit { get; }
        /// <summary>
        /// Gets all the ships docked in a loading dock when the log object were created
        /// </summary>
        /// <returns>Returns a ReadOnlyCollection with Ship object representing the docked in a loading dock when the log object was created</returns>
        public ReadOnlyCollection<Ship> ShipsDockedInLoadingDocks { get; }
        /// <summary>
        /// Gets all the ships docked to ship docks when the log object were created
        /// </summary>
        /// <returns>Returns a ReadOnlyCollection with Ship object representing the ships docked to ship docks when the log object was created</returns>
        public ReadOnlyCollection<Ship> ShipsDockedInShipDocks { get; }
        /// <summary>
        /// Gets all the containers stored in harbour when the log object were created
        /// </summary>
        /// <returns>Returns a ReadOnlyCollection with Container object representing the containers stored in when the log object was created</returns>
        public ReadOnlyCollection<Container> ContainersInHarbour { get; }
        /// <summary>
        /// Prints the wereabouts and info regarding all the ships in the log.
        /// </summary>
        public void PrintInfoForAllShips();
        /// <summary>
        /// Prints the wereabouts and info regarding all the containers in the log.
        /// </summary>
        public void PrintInfoForAllContainers();

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>Returns a String containing information about all ships on the given day of a simulation</returns>
        public String ToString();

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <param name="ShipsOrContainers">"ships" returns information on all ships, "containers" return information on all containers</param>
        /// <returns>Returns a String containing information about all ships or containers on the given day of a simulation.</returns>
        public String ToString(String ShipsOrContainers);
        

    }
}
