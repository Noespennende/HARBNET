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
        public IList<Log> History { get; }

        /// <summary>
        /// Starting the simulation
        /// </summary>
        /// <returns>The simulation results</returns>
        public IList<Log> Run();
        /// <summary>
        /// Prints the ships history
        /// </summary>
        public void PrintShipHistory();
        /// <summary>
        /// prints the containers history
        /// </summary>
        public void PrintContainerHistory();
    }
}