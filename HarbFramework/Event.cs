using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarbFramework
{
    internal class Event
    {
        Guid subject;
        Guid subjectLocation;
        DateTime pointInTime;
        Status status;
    }
}
