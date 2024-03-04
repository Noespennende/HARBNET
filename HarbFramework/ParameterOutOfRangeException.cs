using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe8.HarbNet
{
    public class ParameterOutOfRangeException : Exception
    {

        public ParameterOutOfRangeException()
        {
        }

        public ParameterOutOfRangeException(string message)
            : base(message)
        {
        }

        public ParameterOutOfRangeException(string message, Exception inner)
            : base(message, inner)
        {
        }

        //Constructor is needed for serialization
        protected ParameterOutOfRangeException(SerializationInfo info, StreamingContext context)
        {
        }

    }
}
