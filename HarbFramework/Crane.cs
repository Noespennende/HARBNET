using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe8.HarbNet
{
    /// <summary>
    /// Cranes to be used in a simulation
    /// </summary>
    public class Crane : ICrane
    {
        /// <summary>
        /// Gets the unique ID for the crane
        /// </summary>
        /// <returns>Returns a Guid object representing the cranes unique ID</returns>
        public Guid ID { get; internal set; }
        
        /// <summary>
        /// Gets the container object
        /// </summary>
        /// <returns>Returns the container object that will be loaded or unloaded by the crane</returns>
        public Container Container { get; internal set; }

        /// <summary>
        /// Gets the containers loaded per hour
        /// </summary>
        /// <returns>Returns the int value representing the amount of containers loaded per hour</returns>
        public int ContainersLoadedPerHour { get; internal set; }

        /// <summary>
        /// Gets the unique ID for the cranes current location
        /// </summary>
        /// <returns>Returns a Guid object representing the location of the crane</returns>
        public Guid Location { get; internal set; }

        /// <summary>
        /// Creates a new crane object
        /// </summary>
        /// <param name="containersLoadedPerHour">The int amount on containers loaded per hour</param>
        /// <param name="location">The location of the crane</param>
        internal Crane (int containersLoadedPerHour, Guid location)
        {
            this.ID = Guid.NewGuid();
            this.ContainersLoadedPerHour = containersLoadedPerHour;
            this.Location = location;
            this.Container = null;
        }

        /// <summary>
        /// Loads container from another object to crane
        /// </summary>
        /// <param name="containerToBeLoaded">Name of container to be loaded</param>
        /// <returns>Returns the Guid object representing the container</returns>
        internal Guid LoadContainer (Container containerToBeLoaded)
        {
            this.Container = containerToBeLoaded;
            return containerToBeLoaded.ID;
        }

        /// <summary>
        /// Unloads container from crane to another object
        /// </summary>
        /// <returns>Returns the Container object that is unloaded from the crane to another object</returns>
        internal Container UnloadContainer ()
        {
            Container containerToBeUnloaded = this.Container;
            this.Container = null;
            return containerToBeUnloaded;

        }
        /// <summary>
        /// Returns a string that represents the object, containing the ID, container load per hour and location ID.
        /// </summary>
        /// <returns>Returns a Guid object representing the location of the crane</returns>
        public override String ToString()
        {
            return $"ID: {ID.ToString()}, Container load per hour {ContainersLoadedPerHour}, Location ID: {Location}";
        }
       
    }
}
