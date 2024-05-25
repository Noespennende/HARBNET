using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe8.HarbNet
{
    /// <summary>
    /// Exception thrown if a failed attemt was made to load a container to an AGV's cargo. This can happen either because the AGV does not exist within the
    /// simulation or because the AGV already has a container in its storage.
    /// </summary>
    public class AgvCantBeLoadedException : Exception
    {
        /// <summary>
        /// Creates new AgvCantBeLoadedException.
        /// </summary>
        public AgvCantBeLoadedException() { }

        /// <summary>
        /// Creates new AgvCantBeLoadedException.
        /// </summary>
        /// <param name="message">Message to be thrown when exception is called.</param>
        public AgvCantBeLoadedException(string message) : base(message)
        {
        }

        /// <summary>
        /// Creates new AgvCantBeLoadedException.
        /// </summary>
        /// <param name="message">Message to be thrown when exception is called.</param>
        /// <param name="innerException">innerException to be thrown when exception is called.</param>
        public AgvCantBeLoadedException(String message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
