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
        /// Gets a status object containing information on the last registered status for the crane
        /// </summary>
        /// <returns>Returns a status object containing the information on the latest status change the crane has gone through</returns>
        public Status Status { get; internal set; }

        /// <summary>
        /// Gets the unique ID for the cranes current location
        /// </summary>
        /// <returns>Returns a Guid object representing the location of the crane</returns>
        public Guid location { get; internal set; }

        /// <summary>
        /// Gets a ReadOnlyCollection of StatusLog objects containing information on status changes the crane has gone through throughout a simulation
        /// </summary>
        /// <returns>Returns a ReadOnlyCollection with StatusLog objects with information on status changes the crane has gone through throughout a simulation</returns>
        public ReadOnlyCollection<StatusLog> History { get { return HistoryIList.AsReadOnly(); } }

        /// <summary>
        /// Gets a IList of StatusLog objects containing information on status changes the crane has gone through throughout a simulation
        /// </summary>
        /// <returns>Returns an IList with StatusLog objects with information on status changes the crane has gone through throughout a simulation</returns>
        internal IList<StatusLog> HistoryIList { get; set; }

        /// <summary>
        /// Creates a new crane object
        /// </summary>
        /// <param name="containersLoadedPerHour">The int amount on containers loaded per hour</param>
        /// <param name="location">The location of the crane</param>
        internal Crane (int containersLoadedPerHour, Guid location)
        {
            this.ID = Guid.NewGuid();
            this.ContainersLoadedPerHour = containersLoadedPerHour;
            this.location = location;
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
       
    }
}
