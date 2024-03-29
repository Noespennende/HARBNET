﻿using Gruppe8.HarbNet;
using System.Collections.ObjectModel;
using System.Text;

namespace Gruppe8.HarbNet
{
    /// <summary>
    /// Interface defining the public API of the Simulation class.
    /// </summary>
    public interface ISimulation
    {   
        /// <summary>
        /// A list of DailyLog objects containing information about the status of the harbour troughout the duration of a simulation.
        /// </summary>
        public ReadOnlyCollection<DailyLog> History { get; }
        /// <summary>
        /// Starting the simulation
        /// </summary>
        /// <returns>a IList of DailyLog objects each containing information about the state of the harbour on a given day.</returns>
        public IList<DailyLog> Run();
        /// <summary>
        /// Prints the ships history to console
        /// </summary>
        public void PrintShipHistory();
        /// <summary>
        /// Prints the ships history to console
        ///  <param name="shipToBePrinted">The ship who's history will be printed</param>
        /// </summary>
        public void PrintShipHistory(Ship shipToBePrinted);
        /// <summary>
        /// Prints the ships history to console
        ///  <param name="shipName">Name of the ship who's history will be printed</param>
        /// </summary>
        public void PrintShipHistory(Guid shipID);
        /// <summary>
        /// prints the containers history to console
        /// </summary>
        public void PrintContainerHistory();
        /// <summary>
        /// Returns a string that contains information about the start time, end time of the simulation and the ID of the harbour used.
        /// </summary>
        /// <returns> a string that contains information about the start time, end time of the simulation and the ID of the harbour used.</returns>
        public String ToString();
        /// <summary>
        /// Returns a string that contains information about all ships in the previous simulation.
        /// </summary>
        /// <returns> a string that contains information about all ships in the previous simulation. Returns empty string if no simulation has been run.</returns>
        public String HistoryToString();
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <param name="ShipsOrContainers">Sending inn "ships" returns information on all ships, sending inn "containers" return information on all containers</param>
        /// <returns>Returns a String containing information about all ships or containers of the simulation. Returns an empty string if wrong value is given in param or no simulation has been ran.</returns>
        public String HistoryToString(String ShipsOrContainers);
        /// <summary>
        /// Returns a string that represents the information about one ship in the simulation.
        /// </summary>
        /// <param name="ship">The ship you want information on</param>
        /// <returns>Returns a String containing information about the given ship in the simulation</returns>
        public String HistoryToString(Ship ship);
        /// <summary>
        /// Returns a string that represents the information about one ship in the simulation.
        /// </summary>
        /// <param name="shipID">Name of the ship you want information on</param>
        /// <returns>Returns a String containing information about the given ship in the simulation</returns>
        public String HistoryToString(Guid shipID);
    }
}