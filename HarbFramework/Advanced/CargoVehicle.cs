using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe8.HarbNet.Advanced
{

    /// <summary>
    /// Abstract class defining the public API contract for cargo viechles such as the Truck class.
    /// </summary>
    public abstract class CargoVehicle
    {
        /// <summary>
        /// Gets the unique ID for the viechle.
        /// </summary>
        /// <returns>Returns a Guid object representing the trucks unique ID.</returns>
        public abstract Guid ID { get; internal set; }
        /// <summary>
        /// Gets the ID of the viechle's current location.
        /// </summary>
        /// <returns>Returns a Guid object representing the ID of the viechle's location.</returns>
        public abstract Guid Location { get; internal set; }
        /// <summary>
        /// Gets the current status of the viechle.
        /// </summary>
        /// <return>Returns a Status enum representing the latest registered status of the viechle.</return>
        public abstract Status Status { get; internal set; }
        /// <summary>
        /// Gets the container in the viechles storage.
        /// </summary>
        /// <returns>Returns a container object representing the container in the cargo viechle's storage.</returns>
        public abstract Container? Container { get; internal set; }

        /// <summary>
        /// Creates a new CargoViechle object.
        /// </summary>
        internal CargoVehicle() { }
    }
}
