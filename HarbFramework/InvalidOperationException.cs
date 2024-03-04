using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe8.HarbNet
{
    public class InvalidOperationException : Exception
    {
        public InvalidOperationException()
        {
        }

        public InvalidOperationException(string message)
            : base(message)
        {
        }

        public InvalidOperationException(string message, Exception inner)
            : base(message, inner)
        {
        }

        //Constructor is needed for serialization
        protected InvalidOperationException(SerializationInfo info, StreamingContext context)
        {
        }
    }
}
