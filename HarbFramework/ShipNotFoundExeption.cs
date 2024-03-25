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
        public ShipNotFoundExeption() { }

        public ShipNotFoundExeption(string message) : base(message)
        {
        }
        public ShipNotFoundExeption(String message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
