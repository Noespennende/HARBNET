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
    internal class Adv
    {
        /// <summary>
        /// Gets the unique ID for the Adv.
        /// </summary>
        /// <return>Returns a Guid object representing the Advs unique ID.</return>
        internal Guid ID { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Gets a container.
        /// </summary>
        /// <return>Returns a container object.</return>
        internal Container Container { get; set; }

        /// <summary>
        /// Gets the current status of the Adv.
        /// </summary>
        /// <return>Returns a Status enum representing the latest registered status of the Adv.</return>
        internal Status Status { get; set; }

        /// <summary>
        /// Get the current location of the Adv.
        /// </summary>
        /// <return>Returns a Guid object representing the latest registered location of the Adv.</return>
        internal Guid Location {  get; set; }

        /// <summary>
        /// Creates a new Adv object.
        /// </summary>
        /// <param name="location">Unique ID representing the location the Adv to be created will be located.</param>
        internal Adv (Guid location)
        {
            this.Container = null;
            this.Location = location;
        }

        /// <summary>
        /// Loads container from an object to an Adv.
        /// </summary>
        /// <param name="containerToBeLoaded">Container object to be loaded to an Adv.</param>
        /// <return>Returns a Guid object representing the container to be loaded to an Adv.</return>
        internal Guid LoadContainer(Container containerToBeLoaded)
        {
            this.Container = containerToBeLoaded;
            return containerToBeLoaded.ID;
        }

        /// <summary>
        /// Unloads container from an Adv to another object.
        /// </summary>
        /// <return>Returns a container object representing the container to be unloaded from an Adv.</return>
        internal Container UnloadContainer()
        {
            Container containerToBeUnloaded = this.Container;
            this.Container = null;
            return containerToBeUnloaded;

        }
    }
}
