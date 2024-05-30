﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe8.HarbNet
{
    /// <summary>
    /// The EventArgs class for the ShipStartingLoading event. This event is raised when a ship starts to load containers from the harbor in to the ship's cargohold.
    /// </summary>
    public class ShipStartingLoadingEventArgs : BaseShipEventArgs
    {
        /// <summary>
        /// The unique ID of the dock the ship is located at and loading from.
        /// </summary>
        /// <returns>Guid object representing the ID of the dock the ship is docking to</returns>
        public Guid DockID { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the ShipStartingUnloadingEventArgs class.
        /// </summary>
        /// <param name="ship">The ship object involved in the event.</param>
        /// <param name="currentTime">The current date and time in the simulation.</param>
        /// <param name="description">String value containing a description of the event.</param>
        /// <param name="dockID">The unique ID of the dock the ship is located at and loading from.</param>
        public ShipStartingLoadingEventArgs(Ship ship, DateTime currentTime, string description, Guid dockID)
            : base(ship, currentTime, description)
        {
            DockID = dockID;
        }
    }
}
