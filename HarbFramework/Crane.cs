using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe8.HarbNet
{
    /// <summary>
    /// Cranes to be used in a simulation.
    /// </summary>
    internal class Crane
    {
        /// <summary>
        /// Gets the unique ID for the crane.
        /// </summary>
        /// <returns>Returns a Guid object representing the cranes unique ID.</returns>
        internal Guid ID { get; set; }
        /// <summary>
        /// Gets the container object
        /// </summary>
        /// <returns>Returns the container object that will be loaded or unloaded by the crane.</returns>
        internal Container Container { get; set; }
        /// <summary>
        /// Gets the containers loaded per hour.
        /// </summary>
        /// <returns>Returns the int value representing the amount of containers loaded per hour.</returns>
        internal int ContainersLoadedPerHour { get; set; }
        /// <summary>
        /// Gets the unique ID for the cranes current location
        /// </summary>
        /// <returns>Returns a Guid object representing the location of the crane</returns>
        internal Guid Location { get; set; }

        /// <summary>
        /// Creates a new crane object.
        /// </summary>
        /// <param name="containersLoadedPerHour">Amount on containers loaded per hour.</param>
        /// <param name="location">The location of the crane.</param>
        internal Crane (int containersLoadedPerHour, Guid location)
        {
            this.ID = Guid.NewGuid();
            this.ContainersLoadedPerHour = containersLoadedPerHour;
            this.Location = location;
            this.Container = null;
        }

        /// <summary>
        /// Loads container from another object to crane.
        /// </summary>
        /// <param name="containerToBeLoaded">Name of container to be loaded.</param>
        /// <returns>Returns the Guid object representing the unique ID of container to be loaded.</returns>
        internal Guid LoadContainer (Container containerToBeLoaded)
        {
            this.Container = containerToBeLoaded;
            return containerToBeLoaded.ID;
        }

        /// <summary>
        /// Unloads container from crane to another object.
        /// </summary>
        /// <returns>Returns the Container object to be unloaded.</returns>
        internal Container Unload ()
        {
            Container containerToBeUnloaded = this.Container;
            this.Container = null;
            return containerToBeUnloaded;

        }
    }
}
