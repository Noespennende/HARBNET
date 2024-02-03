using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarbFramework
{
    public class Event : IEvent
    {
        public Guid Subject {  get; internal set; }
        public Guid SubjectLocation { get; internal set; }
        public DateTime PointInTime { get; internal set; }
        public Status Status { get; internal set; }

        internal Event (Guid subject, Guid subjectLocation, DateTime pointInTime, Status status)
        {
            this.Subject = subject;
            this.SubjectLocation = subjectLocation;
            this.PointInTime = pointInTime;
            this.Status = status;
        }
    }
}
