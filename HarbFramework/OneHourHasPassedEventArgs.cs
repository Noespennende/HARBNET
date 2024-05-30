using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe8.HarbNet
{
    /// <summary>
    /// The EventArgs class for the OneHourHasPassed event. This event is raised every hour of the simulation.
    /// </summary>
    public class OneHourHasPassedEventArgs : EventArgs
    {
        /// <summary>
        /// The time in the simulation the event was raised.
        /// </summary>
        /// <returns>Returns a DateTime object representing the date and time an event was raised in the simulation.</returns>
        public DateTime CurrentTime { get; internal set; }

        /// <summary>
        /// A description of the event.
        /// </summary>
        /// <returns>Returns a string value containing a description of the event.</returns>
        public string Description { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the OneHourHasPassedEventArgs class.
        /// </summary>
        /// <param name="currentTime">The date and time in the simulation the event was raised.</param>
        /// <param name="description">A string value containing a description of the event.</param>
        public OneHourHasPassedEventArgs(DateTime currentTime, string description)
        {
            CurrentTime = currentTime;
            Description = description;
        }
    }
}
