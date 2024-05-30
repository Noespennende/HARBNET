using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe8.HarbNet
{
    /// <summary>
    /// The EventArgs class for the SimulationStarting event. This event is raised when the simulation starts.
    /// </summary>
    public class SimulationStartingEventArgs : EventArgs
    {
        /// <summary>
        /// The harbor that is being simulated.
        /// </summary>
        /// <returns>The harbor object of the harbor being simulated.</returns>
        public Harbor HarborToBeSimulated { get; internal set; }

        /// <summary>
        /// The time the simulation will start from.
        /// </summary>
        /// <returns>Datetime object representing the simulations start time.</returns>
        public DateTime StartDate { get; internal set; }

        /// <summary>
        /// A description of the event.
        /// </summary>
        /// <returns>String describing the event.</returns>
        public string Description { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the SimulationStartingEventArgs class.
        /// </summary>
        /// <param name="harborToBeSimulated">The harbor object that is being simulated.</param>
        /// <param name="startDate">The date and time the simulation starts.</param>
        /// <param name="description">String value containing a description of the event.</param>
        public SimulationStartingEventArgs(Harbor harborToBeSimulated, DateTime startDate, string description)
        {
            HarborToBeSimulated = harborToBeSimulated;
            StartDate = startDate;
            Description = description;
        }
    }
}
