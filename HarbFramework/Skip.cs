using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace harbNet
{
    internal class Skip
    {
        Guid id = Guid.NewGuid();
        String size;
        Hashtable historyEvent = new Hashtable();
        Hashtable cargoHistoryEvent = new Hashtable();
        List<Hashtable> history = new List<Hashtable>();
        List<Container> containersOnboard = new List<Container>();
        List<Hashtable> cargoHistory = new List<Hashtable>();
        int containerCapacity;
        int maxWeigh;
        int baseWeigt;
        int baseBerthingTimeInMinutes;
        int baseDockingTimeInMinutes;
        int baseRoundtripTimeInHours;
        
    }
}
