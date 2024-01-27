using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarbFramework
{
    internal class Event
    {
        Guid subject {  get; set; }
        Guid subjectLocation { get; set; }
        DateTime pointInTime { get; set; }
        internal Status status { get; set; }

        internal Event (Guid subject, Guid subjectLocation, DateTime pointInTime, Status status)
        {
            this.subject = subject;
            this.subjectLocation = subjectLocation;
            this.pointInTime = pointInTime;
            this.status = status;
        }
    }
}
