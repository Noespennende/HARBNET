using System;
using Gruppe8.HarbNet;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe8.HarbNet
{
    public class InvalidParameterException : Exception
    {

        public InvalidParameterException() 
        { 
        }

        public InvalidParameterException(string message) 
            : base(message)
        { 
        }

        public InvalidParameterException(string message, Exception inner) 
            : base(message, inner)
        { 
        }

        //Constructor is needed for serialization
        protected InvalidParameterException(SerializationInfo info, StreamingContext context) 
        { 
        }
        
    }
}
