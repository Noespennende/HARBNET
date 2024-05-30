using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gruppe8.HarbNet.Advanced;

namespace Gruppe8.HarbNet
{
    /// <summary>
    /// StatusLogs to be stored in a ships or containers history. Each object containing information about the subject at the time the subject went trough a status change.
    /// </summary>
    public class StatusLog : StatusRecord
    {
        /// <summary>
        /// Gets the unique ID for the subject.
        /// </summary>
        /// <returns>Returns a Guid object representing the subjects unique ID.</returns>
        public override Guid Subject { get; internal set; }

        /// <summary>
        /// Gets the ID of the subjects location.
        /// </summary>
        /// <returns>Returns a Guid object representing the locations unique ID.</returns>
        public override Guid SubjectLocation { get; internal set; }

        /// <summary>
        /// Gets the date and time the status change occured. 
        /// </summary>
        /// <returns>Returns a DateTime object representing the date and time the subjects status change occured.</returns>
        public override DateTime Timestamp { get; internal set; }

        /// <summary>
        /// Gets the current status of the subject.
        /// </summary>
        /// <return>Returns a Status enum representing the latest registered status of the subject.</return>
        public override Status Status { get; internal set; }

        /// <summary>
        /// Constructor for a StatusLog object. Each object holds information about its subject at the time it went trough a status change.
        /// </summary>
        /// <param name="subject">Guid of the subject that went trough the status change</param>
        /// <param name="subjectLocation">Guid of the location of the subject at the time the subject changed status</param>
        /// <param name="pointInTime">Date and time the subject changed status</param>
        /// <param name="status">Status enum representing the new status of the subject</param>
        internal StatusLog (Guid subject, Guid subjectLocation, DateTime pointInTime, Status status)
        {
            Subject = subject;
            SubjectLocation = subjectLocation;
            Timestamp = pointInTime;
            Status = status;
        }

        /// <summary>
        /// Returns a string with the date and time of status log, subjets ID, subjets location and current status.
        /// </summary>
        /// <returns> a String containing information about the subject on a given point in time.</returns>
         public override string ToString()
        {
            return 
                $"Date: + {Timestamp}" + 
                $", Subject ID: {Subject}, " +
                $"Location: {SubjectLocation}, " +
                $"Status: {Status}";
        }
    }
}
