using harbNet;
using System.Text;

namespace HarbFramework
{
    /// <summary>
    /// Interface defining the public API of the Simulation class
    /// </summary>
    public interface ISimulation
    {   
        /// <summary>
        /// A list of log history
        /// </summary>
        public IList<DailyLog> History { get; }

        /// <summary>
        /// Starting the simulation
        /// </summary>
        /// <returns>The simulation results</returns>
        public IList<DailyLog> Run();
        /// <summary>
        /// Prints the ships history
        /// </summary>
        public void PrintShipHistory();
        /// <summary>
        /// prints the containers history
        /// </summary>
        public void PrintContainerHistory();

        /// <summary>
        /// Returns a string that contains information about all ships in the previous simulation.
        /// </summary>
        /// <returns> a string that contains information about all ships in the previous simulation. Returns empty string if no simulation has been run.</returns>
        public String ToString();

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <param name="ShipsOrContainers">"ships" returns information on all ships, "containers" return information on all containers</param>
        /// <returns>Returns a String containing information about all ships or containers of the simulation. Returns an empty string if wrong value is given in param or no simulation has been ran.</returns>
        public String ToString(String ShipsOrContainers);

        /// <summary>
        /// Returns a string that represents the information about one ship in the simulation.
        /// </summary>
        /// <param name="ship">The ship you want information on</param>
        /// <returns>Returns a String containing information about the given ship in the simulation</returns>
        public String ToString(Ship ship);
    }
}