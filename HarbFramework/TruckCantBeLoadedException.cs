using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe8.HarbNet
{
    /// <summary>
    /// Exception thrown if an attempt is made to load a Truck with a container, but the attempt fail or if the Truck does not exist.
    /// </summary>
    public class TruckCantBeLoadedException : Exception
    {
        /// <summary>
        /// Creates new TruckCantBeLoadedException object.
        /// </summary>
        public TruckCantBeLoadedException() 
        { 
        }

        /// <summary>
        /// Creates new TruckCantBeLoadedException object.
        /// </summary>
        /// <param name="message">Message to be thrown when exception is called.</param>
        public TruckCantBeLoadedException(string message) : base(message) 
        { 
        }

        /// <summary>
        /// Creates new TruckCantBeLoadedException object.
        /// </summary>
        /// <param name="message">Message to be thrown when exception is called.</param>
        /// <param name="innerException">innerException to be thrown when exception is called.</param>
        public TruckCantBeLoadedException(string message, Exception innerException) : base(message, innerException)
        { 
        }
    }
}