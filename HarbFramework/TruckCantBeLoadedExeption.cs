using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe8.HarbNet
{
    /// <summary>
    /// Exception used to check if a Adv can't be loaded.
    /// </summary>
    internal class TruckCantBeLoadedExeption : Exception
    {
        /// <summary>
        /// Creates new TruckCantBeLoadedException object.
        /// </summary>
        internal TruckCantBeLoadedExeption() { }


        /// <summary>
        /// Creates new TruckCantBeLoadedException object.
        /// </summary>
        /// <param name="message">Message to be thrown when exception is called.</param>
        internal TruckCantBeLoadedExeption(string message) : base(message) {
            
        }

        /// <summary>
        /// Creates new TruckCantBeLoadedException object.
        /// </summary>
        /// <param name="message">Message to be thrown when exception is called.</param>
        /// <param name="innerException">innerException to be thrown when exception is called.</param>
        internal TruckCantBeLoadedExeption(String message, Exception innerException) : base(message, innerException)
        { 
        }
    }
}
