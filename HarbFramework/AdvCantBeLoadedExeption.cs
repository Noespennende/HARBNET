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
    internal class AdvCantBeLoadedExeption : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        internal AdvCantBeLoadedExeption() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        internal AdvCantBeLoadedExeption(string message) : base(message)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        internal AdvCantBeLoadedExeption(String message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
