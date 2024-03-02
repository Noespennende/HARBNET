using Gruppe8.HarbNet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe8.HarbNet
{
    /// <summary>
    /// Interface defining the public API for the DailyLog class. The DailyLog contains information about the state of a harbour on a given date and time.
    /// </summary>
    public interface IDailyLog
    {
        /// <summary>
        /// Gets the date and time the DailyLog's info were logged.
        /// </summary>
        /// <returns>Returns a DateTime object representing the date and time the info were logged</returns>
        public DateTime Time { get; }
        /// <summary>
        /// Gets all ships in anchorage when the DailyLog object were created.
        /// </summary>
        /// <returns>Returns a an ReadOnlyCollection with ship object representing all the ships in anchorage when the DailyLog object was created</returns>
        public ReadOnlyCollection<Ship> ShipsInAnchorage { get; }
        /// <summary>
        /// Gets all the ships in transit when the DailyLog object were created
        /// </summary>
        /// <returns>Returns a ReadOnlyCollection with Ship object representing the ships in transit when the DailyLog object was created</returns>
        public ReadOnlyCollection<Ship> ShipsInTransit { get; }
        /// <summary>
        /// Gets all the ships docked in a loading dock when the DailyLog object were created
        /// </summary>
        /// <returns>Returns a ReadOnlyCollection with Ship object representing the docked in a loading dock when the DailyLog object was created</returns>
        public ReadOnlyCollection<Ship> ShipsDockedInLoadingDocks { get; }
        /// <summary>
        /// Gets all the ships docked to ship docks when the DailyLog object were created
        /// </summary>
        /// <returns>Returns a ReadOnlyCollection with Ship object representing the ships docked to ship docks when the DailyLog object was created</returns>
        public ReadOnlyCollection<Ship> ShipsDockedInShipDocks { get; }
        /// <summary>
        /// Gets all the containers stored in harbour when the DailyLog object were created
        /// </summary>
        /// <returns>Returns a ReadOnlyCollection with Container object representing the containers stored in when the DailyLog object was created</returns>
        public ReadOnlyCollection<Container> ContainersInHarbour { get; }
        /// <summary>
        /// Prints the wereabouts and info regarding all the ships in the DailyLog.
        /// </summary>
        public void PrintInfoForAllShips();
        /// <summary>
        /// Prints the wereabouts and info regarding all the containers in the DailyLog.
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
