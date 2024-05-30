using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe8.HarbNet
{
    /// <summary>
    /// The EventArgs class for the DayEnded event.
    /// </summary>
    public class DayEndedEventArgs : EventArgs
    {
        /// <summary>
        /// A DailyLog object containing information about the previous day in the simulation.
        /// </summary>
        /// <returns>Returns a DailyLog object containing information about the state of the simulation at the time the object was created</returns>
        public DailyLog TodaysLog { get; internal set; }

        /// <summary>
        /// A Dictionary collection containing all ships and their logs from the previous day in the simulation.
        /// </summary>
        /// <returns>Returns a Dictionary collection containing Ship-List pairs, where List is the history of the ship from the previous day.</returns>
        public Dictionary<Ship, List<StatusLog>> DayReviewAllShipLogs { get; internal set; }

        /// <summary>
        /// The time in the simulation the event was raised.
        /// </summary>
        /// <returns>Returns a DateTime object representing the time in the simulation the event was raised</returns>
        public DateTime CurrentTime { get; internal set; }

        /// <summary>
        /// A description of the event.
        /// </summary>
        /// <returns>Returns a String value describing the event.</returns>
        public string Description { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the DayEndedEventArgs class.
        /// </summary>
        /// <param name="todaysLog">A DailyLog object containing information about the previous day in the simulation.</param>
        /// <param name="dayReviewAllShipLogs">A Dictionary collection with Ship-List pair, where the List is the history of the ship from the previous day.</param>
        /// <param name="currentTime">The date and time in the simulation the event was raised.</param>
        /// <param name="description">A string value containing a description of the event.</param>
        public DayEndedEventArgs(DailyLog todaysLog, Dictionary<Ship, List<StatusLog>> dayReviewAllShipLogs, DateTime currentTime, string description)
        {
            TodaysLog = todaysLog;
            DayReviewAllShipLogs = dayReviewAllShipLogs;
            CurrentTime = currentTime;
            Description = description;
        }
    }
}
