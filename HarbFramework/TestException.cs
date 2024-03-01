using System;
using Gruppe8.HarbNet;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe8.HarbNet
{
    public class TestException : Exception
    {

        public TestException() 
        { 
        }

        public TestException(string message) 
        { 
        }

        public TestException(string message, Exception inner) 
        { 
        }


        protected TestException(SerializationInfo info, StreamingContext context) 
        { 
        }
        
    }
}
