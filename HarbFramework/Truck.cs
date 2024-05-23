using Gruppe8.HarbNet.PublicApiAbstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe8.HarbNet
{
    public class Truck : CargoVehicle
    {
        /// <summary>
        /// Gets the unique ID for the truck.
        /// </summary>
        /// <returns>Returns a Guid object representing the trucks unique ID.</returns>
        public override Guid ID { get; internal set; } = Guid.NewGuid();
        /// <summary>
        /// Gets the ID of the trucks location.
        /// </summary>
        /// <returns>Returns a Guid object representing the ID of the trucks location.</returns>
        public override Guid Location { get; internal set; }
        /// <summary>
        /// Gets the current status of the truck.
        /// </summary>
        /// <return>Returns a Status enum representing the latest registered status of the truck.</return>
        public override Status Status { get; internal set; }
        /// <summary>
        /// Gets the container in the viechles storage.
        /// </summary>
        /// <returns>Returns a container object.</returns>
        public override Container? Container { get; internal set; }

        /// <summary>
        /// Creates new truck object.
        /// </summary>
        /// <param name="location">Guid object representing the location of the truck to be created.</param>
        internal Truck (Guid location)
        {
            this.Location = location;
            this.Status = Status.Queuing;
            this.Container = null;
        }

        /// <summary>
        /// Loads container from a object to truck.
        /// </summary>
        /// <param name="containerToBeLoaded">Container object to be loaded to truck.</param>
        /// <return>Returns a Guid object representing the container to be loaded.</return>
        internal Guid LoadContainer(Container containerToBeLoaded)
        {
            this.Container = containerToBeLoaded;
            return containerToBeLoaded.ID;
        }
        /// <summary>
        /// Unloads container from an truck to another object.
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
