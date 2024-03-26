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
        /// Gets a ReadOnlyCollection of ship objects containting information of all ships in anchorage at the date and time when the DailyLog object were created.
        /// </summary>
        /// <returns>Returns a ReadOnlyCollection with Ship object representing all the ships in anchorage when the DailyLog object was created.</returns>
        public ReadOnlyCollection<Ship> ShipsInAnchorage { get; }
        /// <summary>
        /// Gets a ReadOnlyCollection of ship objects containing information of all the ships in transit when the DailyLog object were created.
        /// </summary>
        /// <returns>Returns a ReadOnlyCollection with Ship object representing the ships in transit when the DailyLog object was created.</returns>
        public ReadOnlyCollection<Ship> ShipsInTransit { get; }
        /// <summary>
        /// Gets a ReadOnlyCollection of ship objects containing information of all the ships docked in a loading dock when the DailyLog object were created.
        /// </summary>
        /// <returns>Returns a ReadOnlyCollection with Ship object representing the docked in a loading dock when the DailyLog object was created.</returns>
        public ReadOnlyCollection<Ship> ShipsDockedInLoadingDocks { get; }
        /// <summary>
        /// Gets a ReadOnlyCollection of ship objects containing information of all the ships docked to ship docks when the DailyLog object were created.
        /// </summary>
        /// <returns>Returns a ReadOnlyCollection with Ship object representing the ships docked to ship docks when the DailyLog object was created.</returns>
        public ReadOnlyCollection<Ship> ShipsDockedInShipDocks { get; }
        /// <summary>
        /// Gets a ReadOnlyCollection of container objects containing information of all the containers stored in harbour when the DailyLog object were created.
        /// </summary>
        /// <returns>Returns a ReadOnlyCollection with Container object representing the containers stored in when the DailyLog object was created.</returns>
        public ReadOnlyCollection<Container> ContainersInHarbour { get; }

        /// <summary>
        /// Gets a ReadOnlyCollection of container objects containing information of all the containers that have arrived to their destination during the simulation.
        /// </summary>
        /// <returns>Returns a ReadOnlyCollection of container objects containing information of all the containers that have arrived to their destination during the simulation.</returns>
        public ReadOnlyCollection<Container> ContainersArrivedAtDestination { get; }
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
        public String HistoryToString();
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <param name="ShipsOrContainers">"ships" returns information on all ships, "containers" return information on all containers</param>
        /// <returns>Returns a String containing information about all ships or containers on the given day of a simulation.</returns>
        public String HistoryToString(String ShipsOrContainers);

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>Returns a String containing the time the DailyLog object represents and the number of ships in all locations.</returns>
        public String ToString();
    }
}
