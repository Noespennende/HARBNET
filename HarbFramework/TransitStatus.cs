using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe8.HarbNet
{
    /// <summary>
    /// Status for subjects in transit.
    /// </summary>
    internal enum TransitStatus
    {
        /// <summary>
        /// No status.
        /// </summary>
        none = 0,
        /// <summary>
        /// Subject is arriving.
        /// </summary>
        Arriving = 1,
        /// <summary>
        /// Subject is leaving.
        /// </summary>
        Leaving = 2,
        /// <summary>
        /// Subject is anchoring.
        /// </summary>
        Anchoring = 3
    }
}
