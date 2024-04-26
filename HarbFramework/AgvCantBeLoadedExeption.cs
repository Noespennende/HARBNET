using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe8.HarbNet
{
    /// <summary>
    /// Exeption thrown if an attemt is made to load an AGV with a container, but the attempt fail.
    /// </summary>
    public class AgvCantBeLoadedExeption : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public AgvCantBeLoadedExeption() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public AgvCantBeLoadedExeption(string message) : base(message)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public AgvCantBeLoadedExeption(String message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
