using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe8.HarbNet
{
    internal class Adv
    {
        /// <summary>
        /// Gets the unique ID for the Adv
        /// </summary>
        /// <return>Returns a Guid object representing the Advs unique ID</return>
        internal Guid ID { get; set; }

        /// <summary>
        /// Gets container
        /// </summary>
        /// <return>Returns a container object</return>
        internal Container Container { get; set; }

        /// <summary>
        /// Gets the status of the Adv
        /// </summary>
        /// <return>Returns the latest registered status of the Adv</return>
        internal Status Status { get; set; }

        /// <summary>
        /// Get the location of the Adv
        /// </summary>
        /// <return>Returns the latest registered location of the Adv</return>
        internal Guid Location {  get; set; }

        /// <summary>
        /// Creates a new Adv object
        /// </summary>
        /// <param name="id">Unique Id for the Adv object</param>
        /// <param name="location">Location of the Adv</param>
        internal Adv (Guid id, Guid location)
        {
            this.ID = id;
            this.Container = null;
            this.Location = location;
        }

        /// <summary>
        /// Gets the container that will be loaded from an object to another
        /// </summary>
        /// <param name="containerToBeLoaded">The specific container object</param>
        /// <return>Returns the unique Id of the specified container</return>
        internal Guid LoadContainer(Container containerToBeLoaded)
        {
            this.Container = containerToBeLoaded;
            return containerToBeLoaded.ID;
        }

        /// <summary>
        /// Get the container that will be unloaded from an object to another
        /// </summary>
        /// <return>Returns the container object</return>
        internal Container UnloadContainer()
        {
            Container containerToBeUnloaded = this.Container;
            this.Container = null;
            return containerToBeUnloaded;

        }
    }
}
