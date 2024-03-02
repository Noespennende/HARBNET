using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe8.HarbNet
{
    /// <summary>
    /// StatusLogs to be stored in a ships or containers history. Each object containing information about the subject at the time the subject went trough a status change.
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
        /// gets the point in time the status change occured. 
        /// </summary>
        public DateTime PointInTime { get; internal set; }
        /// <summary>
        /// gets the new status of the subject after the status change
        /// </summary>
        public Status Status { get; internal set; }

        /// <summary>
        /// Constructor for a StatusLog object. Each object holds information about its subject at the time it went trough a status change.
        /// </summary>
        /// <param name="subject">The subject that went trough the status change</param>
        /// <param name="subjectLocation">The location of the subject at the time the subject changed status</param>
        /// <param name="pointInTime">The point in time the subject changed status</param>
        /// <param name="status">The new status of the subject, f.eks Undocking, Loading, Unloading</param>
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
