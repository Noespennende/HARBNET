using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe8.HarbNet
{
    /// <summary>
    /// Cranes used to load and unload containers to and from ships, trucks and the harbor's own storage area. 
    /// </summary>
    internal class Crane {

        /// <summary>
        /// Gets the unique ID for the crane.
        /// </summary>
        /// <returns>Returns a Guid object representing the cranes unique ID.</returns>
        internal Guid ID { get; set; }

        /// <summary>
        /// Gets the Container object representing the container currently stored in the cranes cargo.
        /// </summary>
        /// <returns>Returns a Container object representing the container currently stored in the cranes cargo.
        /// If no container exist Null is returned.</returns>
        internal Container? Container { get; set; }

        /// <summary>
        /// Gets a number representing the amount of loads the Crane can do in an hour.
        /// One load is defined by the crane loading one container on to its cargo or unload one container from its cargo.
        /// </summary>
        /// <returns>Returns the int value representing the amount of loads the container can perform in one hour of the simulation.</returns>
        internal int ContainersLoadedPerHour { get; set; }

        /// <summary>
        /// Gets the ID of the containers current location. 
        /// </summary>
        /// <returns>Returns a Guid object representing the ID of the cranes current location</returns>
        internal Guid Location { get; set; }

        /// <summary>
        /// Constructor used to create objects of the crane class. A crane can be used to load and unload containers to and from ships,
        /// trucks and the harbor's own storage area
        /// </summary>
        /// <param name="containersLoadedPerHour">Int value representing the amount of loads the Crane can do in an hour.
        /// One load is defined by the crane loading one container on to its cargo or unload one container from its cargo.</param>
        /// <param name="location">Guid containing the ID of the location the crane will be placed at.</param>
        internal Crane(int containersLoadedPerHour, Guid location)
        {
            ID = Guid.NewGuid();
            ContainersLoadedPerHour = containersLoadedPerHour;
            Location = location;
            Container = null;
        }


        /// <summary>
        /// Loads the given container on to the cranes storage.
        /// </summary>
        /// <param name="containerToBeLoaded">The container object to be loaded by the Crane.</param>
        /// <returns>Returns the Guid object representing the unique ID of the container that was loaded on to the cranes storage.</returns>
        internal Guid LoadContainer(Container containerToBeLoaded)
        {
            Container = containerToBeLoaded;
            return containerToBeLoaded.ID;
        }

        /// <summary>
        /// Unloads the currently loaded container from the cranes storage. Crane current cargo is then set to null.
        /// </summary>
        /// <returns>Returns a Container object representing the container unloaded from the cranes storage. Returns null if no container
        /// is found in the cranes storage.</returns>
        internal Container? UnloadContainer()
        {
            Container? containerToBeUnloaded = Container;
            Container = null;
            return containerToBeUnloaded;
        }


    }
}
