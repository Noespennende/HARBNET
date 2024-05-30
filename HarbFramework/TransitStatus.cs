﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe8.HarbNet
{
    /// <summary>
    /// Status for subjects in transit to a destination.
    /// </summary>
    internal enum TransitStatus
    {
        /// <summary>
        /// No status.
        /// </summary>
        None = 0,
        /// <summary>
        /// Subject is arriving to _harbor.
        /// </summary>
        Arriving = 1,
        /// <summary>
        /// Subject is leaving the _harbor.
        /// </summary>
        Leaving = 2,
        /// <summary>
        /// Subject is anchoring at the anchorage.
        /// </summary>
        Anchoring = 3,
    }
}
