using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe8.HarbNet
{
    internal class CraneCantBeLoadedExeption : Exception
    {
        public CraneCantBeLoadedExeption() { }

        public CraneCantBeLoadedExeption(string message) : base(message)
        {

        }
        public CraneCantBeLoadedExeption(String message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
