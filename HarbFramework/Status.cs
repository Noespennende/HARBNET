using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarbFramework
{
    /// <summary>
    /// Status for ships and containers.
    /// </summary>
    public enum Status
    {
        /// <summary>
        /// No status.
        /// </summary>
        None = 0,
        /// <summary>
        /// Ship is anchoring
        /// </summary>
        Anchoring,
        /// <summary>
        /// Ship is in anchorage
        /// </summary>
        Anchored,
        /// <summary>
        /// Docking to a free loading dock
        /// </summary>
        DockingToLoadingDock,
        /// <summary>
        /// Docking to a free ship dock
        /// </summary>
        DockingToShipDock,
        /// <summary>
        /// Docked to a loading dock
        /// </summary>
        DockedToLoadingDock,
        /// <summary>
        /// Docked to a ship dock
        /// </summary>
        DockedToShipDock,
        /// <summary>
        /// Unloading container(s) from ship to harbor.
        /// </summary>
        Unloading,
        /// <summary>
        /// Done unloading container(s) from ship to harbor.
        /// </summary>
        UnloadingDone,
        /// <summary>
        /// Loading container(s) from harbor to ship.
        /// </summary>
        Loading,
        /// <summary>
        /// Done loading container(s) from harbor to ship.
        /// </summary>
        LoadingDone,
        /// <summary>
        /// Undocking ship from harbor.
        /// </summary>
        Undocking,
        /// <summary>
        /// For ships: In transit on the sea. For containers: in transit onboard a ship.
        /// </summary>
        Transit,
        /// <summary>
        /// Container stored on land in harbour.
        /// </summary>
        InStorage


    }
}
