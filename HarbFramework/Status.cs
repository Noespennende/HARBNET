﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe8.HarbNet
{
    /// <summary>
    /// Enum representing the status for ships and containers.
    /// </summary>
    public enum Status
    {
        /// <summary>
        /// No status.
        /// </summary>
        None = 0,
        /// <summary>
        /// Ship is anchoring.
        /// </summary>
        Anchoring,
        /// <summary>
        /// Ship is in anchorage.
        /// </summary>
        Anchored,
        /// <summary>
        /// Docking to a free loading dock.
        /// </summary>
        DockingToLoadingDock,
        /// <summary>
        /// Docking to a free ship dock.
        /// </summary>
        DockingToShipDock,
        /// <summary>
        /// Docked to a loading dock.
        /// </summary>
        DockedToLoadingDock,
        /// <summary>
        /// Docked to a ship dock.
        /// </summary>
        DockedToShipDock,
        /// <summary>
        /// Unloading container(s) from ship to _harbor.
        /// </summary>
        Unloading,
        /// <summary>
        /// Done unloading container(s) from ship to _harbor.
        /// </summary>
        UnloadingDone,
        /// <summary>
        /// Loading container(s) from _harbor to ship.
        /// </summary>
        Loading,
        /// <summary>
        /// Done loading container(s) from _harbor to ship.
        /// </summary>
        LoadingDone,
        /// <summary>
        /// Undocking ship from _harbor.
        /// </summary>
        Undocking,
        /// <summary>
        /// For ships: In transit on the sea. For containers: in transit onboard a ship.
        /// </summary>
        Transit,
        /// <summary>
        /// Container stored on land in harbour.
        /// </summary>
        InStorage,
        /// <summary>
        /// Container being loaded to crane.
        /// </summary>
        LoadingToCrane,
        /// <summary>
        /// Unloading container from crane to ship.
        /// </summary>
        UnloadingFromCraneToShip,
        /// <summary>
        /// Loading Container to Truck.
        /// </summary>
        LoadingToTruck,
        /// <summary>
        /// Loading Container to AGV.
        /// </summary>
        LoadingToAgv,
        /// <summary>
        /// Truck queuing.
        /// </summary>
        Queuing,
        /// <summary>
        /// Container has arrived at its final destination
        /// </summary>
        ArrivedAtDestination,
    }
}
