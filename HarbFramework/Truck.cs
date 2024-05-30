using Gruppe8.HarbNet.Advanced;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe8.HarbNet
{
    /// <summary>
    /// Trucks are viechles that can be used to transport containers to and from the _harbor. A truck can hold a single container.
    /// </summary>
    public class Truck
    {
        /// <summary>
        /// Gets the unique ID for the truck.
        /// </summary>
        /// <returns>Returns a Guid object representing the trucks unique ID.</returns>
        public Guid ID { get; internal set; } = Guid.NewGuid();
        
        /// <summary>
        /// Gets the location ID of the trucks current location.
        /// </summary>
        /// <returns>Returns a Guid object representing the ID of the trucks location.</returns>
        public Guid Location { get; internal set; }
        
        /// <summary>
        /// Gets the current status of the truck.
        /// </summary>
        /// <return>Returns a Status enum representing the latest registered status of the truck.</return>
        public Status Status { get; internal set; }
        
        /// <summary>
        /// Gets the container in the truck's storage.
        /// </summary>
        /// <returns>Returns a container object representing the container in the truck's storage.</returns>
        public Container? Container { get; internal set; }

        /// <summary>
        /// Creates a new object of the Truck class.
        /// </summary>
        /// <param name="location">Guid object representing the current location of the truck to be created.</param>
        internal Truck (Guid location)
        {
            Location = location;
            Status = Status.Queuing;
            Container = null;
        }

        /// <summary>
        /// Loads the given container on to the trucks storage.
        /// </summary>
        /// <param name="containerToBeLoaded">Container object to be loaded to the truck's storage.</param>
        /// <return>Returns a Guid object representing the container that were loaded on to the trucks storage.</return>
        internal Guid LoadContainer(Container containerToBeLoaded)
        {
            Container = containerToBeLoaded;

            return containerToBeLoaded.ID;
        }

        /// <summary>
        /// Unloads a container from the truck's storage. Trucks storage is then set to null.
        /// </summary>
        /// <return>Returns the container object unloaded from the trucks storage.</return>
        internal Container? UnloadContainer()
        {
            Container? containerToBeUnloaded = Container;
            Container = null;

            return containerToBeUnloaded;
        }
    }
}
