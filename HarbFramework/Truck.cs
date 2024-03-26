using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe8.HarbNet
{
    public class Truck
    {
        /// <summary>
        /// Gets the unique ID for the truck.
        /// </summary>
        /// <returns>Returns a Guid object representing the trucks unique ID.</returns>
        public Guid ID { get; internal set; } = Guid.NewGuid();
        /// <summary>
        /// Gets the ID of the trucks location.
        /// </summary>
        /// <returns>Returns a Guid object representing the ID of the trucks location.</returns>
        public Guid Location { get; internal set; }
        /// <summary>
        /// Gets the current status of the truck.
        /// </summary>
        /// <return>Returns a Status enum representing the latest registered status of the truck.</return>
        public Status Status { get; internal set; }
        /// <summary>
        /// Gets container.
        /// </summary>
        /// <returns>Returns a container object.</returns>
        public Container? Container { get; internal set; }

        /// <summary>
        /// Creates new truck object.
        /// </summary>
        /// <param name="location">Location of the truck to be created.</param>
        public Truck (Guid location)
        {
            this.Location = location;
            this.Status = Status.Queuing;
            this.Container = null;
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
        /// Unloads container from an object to another.
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
