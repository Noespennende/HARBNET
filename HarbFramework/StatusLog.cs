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
        /// Gets the unique ID for the subject.
        /// </summary>
        /// <returns>Returns a Guid object representing the subjects unique ID.</returns>
        public Guid Subject { get; internal set; }
        /// <summary>
        /// Gets the ID of the subjects location.
        /// </summary>
        /// <returns>Returns a Guid object representing the locations unique ID.</returns>
        public Guid SubjectLocation { get; internal set; }
        /// <summary>
        /// Gets the date and time the status change occured. 
        /// </summary>
        /// <returns>Returns a DateTime object representing the date and time the subjects status change occured.</returns>
        public DateTime PointInTime { get; internal set; }
        /// <summary>
        /// Gets the current status of the subject.
        /// </summary>
        /// <return>Returns a Status enum representing the latest registered status of the subject.</return>
        public Status Status { get; internal set; }

        /// <summary>
        /// Constructor for a StatusLog object. Each object holds information about its subject at the time it went trough a status change.
        /// </summary>
        /// <param name="subject">Guid of the subject that went trough the status change</param>
        /// <param name="subjectLocation">Guid of the location of the subject at the time the subject changed status</param>
        /// <param name="pointInTime">Date and time the subject changed status</param>
        /// <param name="status">The new status of the subject, f.eks Undocking, Loading, Unloading</param>
        internal StatusLog (Guid subject, Guid subjectLocation, DateTime pointInTime, Status status)
        {
            this.Subject = subject;
            this.SubjectLocation = subjectLocation;
            this.PointInTime = pointInTime;
            this.Status = status;
        }

        /// <summary>
        /// Returns a string conraining information about the subject on a given point in time.
        /// </summary>
        /// <returns> a String containing information about the subject on a given point in time.</returns>
        override public string ToString()
        {
            return ("Date: " + PointInTime.ToString() + ", Subject ID: " + Subject.ToString() + ", Location: " + SubjectLocation.ToString() + ", Status: " + Status.ToString());
        }
    }
}
