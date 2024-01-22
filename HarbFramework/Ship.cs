using HarbFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace harbNet
{
    internal class Ship
    {
        Guid id = Guid.NewGuid();
        ShipSize shipSize;
        DateTime startDate;
        int RoundTripInDays;
        ICollection<Event> history;
        ICollection<Container> containersOnBoard;
        int containerCapacity;
        int maxWeigh;
        int baseWeigt;
        int currentWeight;
        int baseBerthingTimeInMinutes;
        int baseDockingTimeInMinutes;
        
    }
}
