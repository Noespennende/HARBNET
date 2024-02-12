using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarbFramework
{
    public interface IEvent
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
    }
}
