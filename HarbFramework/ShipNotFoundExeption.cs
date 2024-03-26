using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe8.HarbNet
{
    /// <summary>
    /// Exeption class for when ships are not found by a method. 
    /// </summary>
    internal class ShipNotFoundExeption : Exception
    {

        /// <summary>
        /// Creates new ShipNotFoundException object.
        /// </summary>
        public ShipNotFoundExeption() { }

        /// <summary>
        /// Creates new ShipNotFoundException object.
        /// </summary>
        /// <param name="message">Message to be thrown when exception is called.</param>
        public ShipNotFoundExeption(string message) : base(message)
        {
        }

        /// <summary>
        /// Creates new ShipNotFoundException object.
        /// </summary>
        /// <param name="message">Message to be thrown when exception is called.</param>
        /// <param name="innerException">innerException to be thrown when exception is called.</param>
        public ShipNotFoundExeption(String message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
