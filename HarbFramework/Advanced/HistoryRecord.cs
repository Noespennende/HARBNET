using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe8.HarbNet.Advanced
{
    /// <summary>
    /// Abstract class defining the public API for HistoryRecords such as the DailyLog class.
    /// HistoryRecords contains information about the state of a harbour on a given date and time.
    /// </summary>
    public abstract class HistoryRecord
    {
        /// <summary>
        /// Gets the date and time the HistoryRecord's information were recorded.
        /// </summary>
        /// <returns>Returns a DateTime object representing the date and time the information of the HistoryRecord were recorded</returns>
        public abstract DateTime Timestamp { get; internal set; }
        /// <summary>
        /// Gets a ReadOnlyCollection of ship objects containting information of all ships in anchorage at the date and time when the HistoryRecord object were created.
        /// </summary>
        /// <returns>Returns a ReadOnlyCollection with Ship object representing all the ships in anchorage when the HistoryRecord object was created.</returns>
        public abstract ReadOnlyCollection<Ship> ShipsInAnchorage { get; }
        /// <summary>
        /// Gets a ReadOnlyCollection of ship objects containing information of all the ships that were in transit when the HistoryRecord object were created.
        /// </summary>
        /// <returns>Returns a ReadOnlyCollection with Ship object representing all ships that were in transit when the HistoryRecord object was created.</returns>
        public abstract ReadOnlyCollection<Ship> ShipsInTransit { get; }
        /// <summary>
        /// Gets a ReadOnlyCollection of ship objects containing information of all the ships that were docked in a loading dock when the HistoryRecord object were created.
        /// </summary>
        /// <returns>Returns a ReadOnlyCollection with Ship object representing the all the ships docked in a loading dock when the HistoryRecord object were created.</returns>
        public abstract ReadOnlyCollection<Ship> ShipsDockedInLoadingDocks { get; }
        /// <summary>
        /// Gets a ReadOnlyCollection of ship objects containing information of all the ships docked to ship docks when the HistoryRecord object were created.
        /// </summary>
        /// <returns>Returns a ReadOnlyCollection with Ship object representing all the ships docked to ship docks when the HistoryRecord object was created.</returns>
        public abstract ReadOnlyCollection<Ship> ShipsDockedInShipDocks { get; }
        /// <summary>
        /// Gets a ReadOnlyCollection of container objects containing information of all the containers stored in harbour when the HistoryRecord object were created.
        /// </summary>
        /// <returns>Returns a ReadOnlyCollection with Container object representing all the containers stored in when the HistoryRecord object was created.</returns>
        public abstract ReadOnlyCollection<Container> ContainersInHarbor { get; }
        /// <summary>
        /// Gets a ReadOnlyCollection of container objects containing information of all the containers that has arrived to their destination during the simulation.
        /// </summary>
        /// <returns>Returns a ReadOnlyCollection of container objects containing information of all the containers that has arrived to their destination during the simulation.</returns>
        public abstract ReadOnlyCollection<Container> ContainersArrivedAtDestination { get; }
        /// <summary>
        /// Prints the wereabouts and information regarding all the ships in the HistoryRecord.
        /// </summary>
        public abstract void PrintInfoForAllShips();
        /// <summary>
        /// Prints the wereabouts and information regarding all the containers in the HistoryRecord.
        /// </summary>
        public abstract void PrintInfoForAllContainers();
        /// <summary>
        /// Returns a string with information regarding the wereabouts and information regarding all ships in the HistoryRecord.
        /// </summary>
        /// <returns>Returns a String containing information about all ships on the given day of a simulation</returns>
        public abstract string HistoryToString();
        /// <summary>
        /// Returns a string that contains information about all ships or containers on the given day of a simulation.
        /// </summary>
        /// <param name="ShipsOrContainers">User can choose to write either "ships" or "containers" as input. "ships" returns information on all ships stored in the HistoryRecord, "containers" return information on all containers stored in the HistoryRecord</param>
        /// <returns>Returns a String containing information about all ships or all containers on the given day of a simulation.</returns>
        public abstract string HistoryToString(string ShipsOrContainers);

        /// <summary>
        /// Returns a string containing information stored in the HistoryRecord.
        /// </summary>
        /// <returns>Returns a String containing information stored in the HistoryRecord.</returns>
        public abstract override string ToString();

        /// <summary>
        /// Constructor for the HistoryRecord class.
        /// </summary>
        internal HistoryRecord() { }
    }
}
