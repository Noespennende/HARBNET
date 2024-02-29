using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarbFramework
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
        /// gets the point in time for the event
        /// </summary>
        public DateTime PointInTime { get; }
        /// <summary>
        /// gets the status of the event IE: undocking, unloading.
        /// </summary>
        public Status Status { get; }

        /// <summary>
        /// Returns a string conraining information about the subject on a given point in time
        /// </summary>
        /// <returns> a String containing information about the subject on a given point in time.</returns>
        public String ToString();
    }
}
