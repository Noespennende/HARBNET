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
        /// Gets the subject ID
        /// </summary>
        public Guid Subject { get; }
        /// <summary>
        /// Gets the subject location
        /// </summary>
        public Guid SubjectLocation { get; }
        /// <summary>
        /// gets the point in time the status change occured to the subject.
        /// </summary>
        public DateTime PointInTime { get; }
        /// <summary>
        /// gets the new status of the subject at the time the status change occured. IE: undocking, unloading.
        /// </summary>
        public Status Status { get; }

        /// <summary>
        /// Returns a string conraining information about the subject on a given point in time
        /// </summary>
        /// <returns> a String containing information about the subject on a given point in time.</returns>
        public String ToString();
    }
}
