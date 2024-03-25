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
        /// <param name="id">Unique Id for the Adv to be created.</param>
        /// <param name="location">Location of the Adv to be created.</param>
        internal Adv (Guid location)
        {
            this.Container = null;
            this.Location = location;
        }

        /// <summary>
        /// Loads container from a object to another.
        /// </summary>
        /// <param name="containerToBeLoaded">Container to be loaded.</param>
        /// <return>Returns a Guid object representing the container to be loaded.</return>
        internal Guid LoadContainer(Container containerToBeLoaded)
        {
            this.Container = containerToBeLoaded;
            return containerToBeLoaded.ID;
        }

        /// <summary>
        /// Unloads container from a object to another.
        /// </summary>
        /// <return>Returns the container object to be unloaded.</return>
        internal Container UnloadContainer()
        {
            Container containerToBeUnloaded = this.Container;
            this.Container = null;
            return containerToBeUnloaded;

        }
    }
}
