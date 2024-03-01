using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe8.HarbNet
{
    /// <summary>
    /// Event to be stored in a ships or containers history holding information about the subjects wereabouts and status.
    /// </summary>
    public class StatusLog : IStatusLog
    {
        /// <summary>
        /// Gets an unique Id of the subject
        /// </summary>
        public Guid Subject { get; internal set; }
        /// <summary>
        /// gets an unique ID of the location of a subject
        /// </summary>
        public Guid SubjectLocation { get; internal set; }
        /// <summary>
        /// gets the point in time of an event or action. 
        /// </summary>
        public DateTime PointInTime { get; internal set; }
        /// <summary>
        /// gets The status of the subject
        /// </summary>
        public Status Status { get; internal set; }

        /// <summary>
        /// Constructor for an Event
        /// </summary>
        /// <param name="subject">The subject of the event</param>
        /// <param name="subjectLocation">The location of the subject</param>
        /// <param name="pointInTime">The point in time for this event</param>
        /// <param name="status">The status of the event, f.eks Undocking, Loading, Unloading</param>
        internal StatusLog (Guid subject, Guid subjectLocation, DateTime pointInTime, Status status)
        {
            this.Subject = subject;
            this.SubjectLocation = subjectLocation;
            this.PointInTime = pointInTime;
            this.Status = status;
        }

        /// <summary>
        /// Returns a string conraining information about the subject on a given point in time
        /// </summary>
        /// <returns> a String containing information about the subject on a given point in time</returns>
        override public string ToString()
        {
            return ("Date: " + PointInTime.ToString() + ", Subject ID: " + Subject.ToString() + ", Location: " + SubjectLocation.ToString() + ", Status: " + Status.ToString());
        }
    }
}
