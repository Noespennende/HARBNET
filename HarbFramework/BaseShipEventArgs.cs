using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe8.HarbNet
{
    /// <summary>
    /// Base EventArgs with basic args to be used as a base in more complex events where a ship is involved.
    /// </summary>
    public class BaseShipEventArgs : EventArgs
    {
        /// <summary>
        /// The ship involved in the event.
        /// </summary>
        /// <returns>Returns a ship object that is involved in the event in the simulation.</returns>
        public Ship Ship { get; internal set; }

        /// <summary>
        /// The time in the simulation the event was raised.
        /// </summary>
        /// <returns>Returns a DateTime object representing the date and time an event was raised in the simulation.</returns>
        public DateTime CurrentTime { get; internal set; }
        /// <summary>
        /// A description of the event.
        /// </summary>
        /// <returns>Returns a String value describing the event.</returns>
        public string Description { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the BaseShipEventArgs class.
        /// </summary>
        /// <param name="ship">The ship object involved in the event.</param>
        /// <param name="currentTime">The current date time in the simulation.</param>
        /// <param name="description">String value containing a description of the event.</param>
        public BaseShipEventArgs(Ship ship, DateTime currentTime, string description)
        {
            Ship = ship;
            CurrentTime = currentTime;
            Description = description;
        }
    }
}
