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
        internal AdvCantBeLoadedExeption() { }

        internal AdvCantBeLoadedExeption(string message) : base(message)
        {
        }
        internal AdvCantBeLoadedExeption(String message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
