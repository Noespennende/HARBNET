using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe8.HarbNet.PublicApiAbstractions
{
    /// <summary>
    /// Abstract class defining the public API of StatusRecords such as the StatusLog class. Each object of this class represents one status change that happened to the subject
    /// the StatusRecord is storing information about.
    /// </summary>
    public abstract class StatusRecord
    {
        /// <summary>
        /// Gets the unique ID for the Subject of the StatusRecord. The ID is typically the ID of the container or ship that the StatusRecord object
        /// is storing information of.
        /// </summary>
        /// <returns>Returns a Guid object representing the subjects unique ID.</returns>
        public abstract Guid Subject { get; internal set; }
        /// <summary>
        /// Gets the unique ID for the subjects location.
        /// </summary>
        /// <returns>Returns a Guid object representing the subjects location unique ID.</returns>
        public abstract Guid SubjectLocation { get; internal set; }
        /// <summary>
        /// Gets the point in time the status change occured to the subject.
        /// </summary>
        /// <returns>Returns a DateTime object representing the date and time status change occured to the subject.</returns>
        public abstract DateTime PointInTime { get; internal set; }
        /// <summary>
        /// Gets the new status of the subject at the time the status change occured. IE: if the status change went from "Anchoring" to "Anchored", the latter is returned.
        /// </summary>
        /// <return>Returns a Status enum representing the latest registered status of the subject.</return>
        public abstract Status Status { get; internal set; }
        /// <summary>
        /// Returns a string containing information about the date and time of the status change, subjets ID, subjets location and current status.
        /// </summary>
        /// <returns> a String containing information about the subject on a given point in time.</returns>
        public override abstract string ToString();
    }
}
