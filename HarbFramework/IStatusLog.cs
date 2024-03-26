using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe8.HarbNet
{
    /// <summary>
    /// Interface defining the contract for the public API of the StatusLog class.
    /// </summary>
    public interface IStatusLog
    {
        /// <summary>
        /// Gets the unique ID for the Subject.
        /// </summary>
        /// <returns>Returns a Guid object representing the subjects unique ID.</returns>
        public Guid Subject { get; }
        /// <summary>
        /// Gets the unique ID for the subjects location.
        /// </summary>
        /// <returns>Returns a Guid object representing the subjects location unique ID.</returns>
        public Guid SubjectLocation { get; }
        /// <summary>
        /// Gets the point in time the status change occured to the subject.
        /// </summary>
        /// <returns>Returns a DateTime object representing the date and time status change occured to the subject.</returns>
        public DateTime PointInTime { get; }
        /// <summary>
        /// Gets the new status of the subject at the time the status change occured. IE: undocking, unloading.
        /// </summary>
        /// <return>Returns a Status enum representing the latest registered status of the subject.</return>
        public Status Status { get; }
        /// <summary>
        /// Returns a string containing information about the subject on a given point in time.
        /// </summary>
        /// <returns> a String containing information about the subject on a given point in time.</returns>
        public String ToString();
    }
}
