using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe8.HarbNet
{
    internal class TruckCantBeLoadedExeption : Exception
    {
        public TruckCantBeLoadedExeption() { }

        public TruckCantBeLoadedExeption(string message) : base(message) {
            
        }

        public TruckCantBeLoadedExeption(String message, Exception innerException) : base(message, innerException)
        { 
        }
    }
}
