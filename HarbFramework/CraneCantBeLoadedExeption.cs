using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe8.HarbNet
{
    /// <summary>
    /// Exception used to check if a Crane can't be loaded.
    /// </summary>
    public class CraneCantBeLoadedExeption : Exception
    {
        /// <summary>
        /// Creates new CraneCantBeLoadedException object.
        /// </summary>
        public CraneCantBeLoadedExeption() { }


        /// <summary>
        /// Creates new CraneCantBeLoadedException object.
        /// </summary>
        /// <param name="message">Message to be thrown when exception is called.</param>
        public CraneCantBeLoadedExeption(string message) : base(message)
        {

        }

        /// <summary>
        /// Creates new CraneCantBeLoadedException object.
        /// </summary>
        /// <param name="message">Message to be thrown when exception is called.</param>
        /// <param name="innerException">innerException to be thrown when exception is called.</param>
        public CraneCantBeLoadedExeption(String message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
