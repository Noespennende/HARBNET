using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace harbNet
{
    internal class Container
    {
        Guid id = Guid.NewGuid();
        List<Hashtable> history = new List<Hashtable>();
        Hashtable historyEvent = new Hashtable();
        //Enum containerClass;
        int Weight;
        Guid currentPoison;
    }
}
