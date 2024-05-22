using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe8.HarbNet
{
    /// <summary>
    /// Exception thrown if an attempt is made to load a AGV with a container, but the attempt fail or if the AGV does not exist.
    /// </summary>
    public class AgvCantBeLoadedExeption : Exception
    {
        /// <summary>
        /// Creates new AgvCantBeLoadedException.
        /// </summary>
        public AgvCantBeLoadedExeption() { }

        /// <summary>
        /// Creates new AgvCantBeLoadedException.
        /// </summary>
        /// <param name="message">Message to be thrown when exception is called.</param>
        public AgvCantBeLoadedExeption(string message) : base(message)
        {
        }

        /// <summary>
        /// Creates new AgvCantBeLoadedException.
        /// </summary>
        /// <param name="message">Message to be thrown when exception is called.</param>
        /// <param name="innerException">innerException to be thrown when exception is called.</param>
        public AgvCantBeLoadedExeption(String message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
