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
        /// <param name="containersLoadedPerHour">Int value representing the amount on containers loaded by the crane per hour.</param>
        /// <param name="location">Unique Guid representing the location the Container to be created will currently be located.</param>
        internal Crane (int containersLoadedPerHour, Guid location)
        {
            this.ID = Guid.NewGuid();
            this.ContainersLoadedPerHour = containersLoadedPerHour;
            this.Location = location;
            this.Container = null;
        }

        /// <summary>
        /// Container loaded by Crane.
        /// </summary>
        /// <param name="containerToBeLoaded">The container object to be loaded by the Crane.</param>
        /// <returns>Returns the Guid object representing the unique ID of container to be loaded.</returns>
        internal Guid LoadContainer (Container containerToBeLoaded)
        {
            this.Container = containerToBeLoaded;
            return containerToBeLoaded.ID;
        }

        /// <summary>
        /// Container unloaded from Crane.
        /// </summary>
        /// <returns>Returns the Container object to be unloaded.</returns>
        internal Container UnloadContainer ()
        {
            Container containerToBeUnloaded = this.Container;
            this.Container = null;
            return containerToBeUnloaded;

        }
    }
}
