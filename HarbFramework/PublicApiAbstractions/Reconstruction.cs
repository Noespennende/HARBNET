using System.Collections.ObjectModel;
using System.Text;

namespace Gruppe8.HarbNet.PublicApiAbstractions
{
    /// <summary>
    /// Abstract class defining the public API for Reconstructions such as the Simulation class.
    /// </summary>
    public abstract class Reconstruction
    {
        /// <summary>
        /// A list of DailyLog objects containing information about the status of the harbour troughout the duration of a simulation.
        /// </summary>
        /// <returns>Returns a readOnlyCollection of DailyLog objects each containing information from one day of the simulation. Together the list represent the entire history of one simulation.</returns>
        public abstract ReadOnlyCollection<DailyLog> History { get; }
        /// <summary>
        /// Starts the simulation.
        /// </summary>
        /// <returns>a IList of DailyLog objects each containing information about the state of the harbour on a given day.</returns>
        public abstract IList<DailyLog> Run();
        /// <summary>
        /// Prints the entire history for each ship in the harbor simulation to console.
        /// </summary>
        public abstract void PrintShipHistory();
        /// <summary>
        /// Prints the entire history of one ship to the console
        ///  <param name="shipToBePrinted">The ship who's history will be printed</param>
        /// </summary>
        public abstract void PrintShipHistory(Ship shipToBePrinted);
        /// <summary>
        /// Prints the entire history of one ship to the console
        /// </summary>
        /// <param name="shipID">The unique ID of the ship who's history will be printed.</param>
        public abstract void PrintShipHistory(Guid shipID);
        /// <summary>
        /// Prints the entire history of each container in the harbor simulation to console.
        /// </summary>
        public abstract void PrintContainerHistory();
        /// <summary>
        /// Returns a string that contains information about the start time, end time of the simulation and the ID of the harbour used.
        /// </summary>
        /// <returns> a string that contains information about the start time, end time of the simulation and the ID of the harbour used.</returns>
        public abstract override string ToString();
        /// <summary>
        /// Returns a string that contains information about the entire history of each ship in the previous harbor simulation.
        /// </summary>
        /// <returns> a string that contains information about all ships in the previous simulation. Returns empty string if no simulation has been run.</returns>
        public abstract string HistoryToString();
        /// <summary>
        /// Returns a string containing information about the entire history of each ship or each container in the simulation.
        /// </summary>
        /// <param name="ShipsOrContainers">Sending inn "ships" returns the history of all ships in the previous simulation. Sending inn "containers" return the history of each container in the previous simulation</param>
        /// <returns>Returns a String value containing the entire history of all ships or all containers of the simulation. Returns an empty string if wrong value is given in param or no simulation has been run.</returns>
        public abstract string HistoryToString(string ShipsOrContainers);
        /// <summary>
        /// Returns a string that represents the entire history of all ships used in the previous simulation.
        /// </summary>
        /// <param name="ship">The ship in which the history is to be retrieved.</param>
        /// <returns>Returns a String value containing information about the entire history of the given ship in the simulation.</returns>
        public abstract string HistoryToString(Ship ship);
        /// <summary>
        /// Returns a string that represents the entire history of all ships used in the previous simulation.
        /// </summary>
        /// <param name="shipID">The unique ID of the ship the history belongs to.</param>
        /// <returns>Returns a String value containing information about the entire history of the given ship in the simulation.</returns>
        public abstract string HistoryToString(Guid shipID);
    }
}