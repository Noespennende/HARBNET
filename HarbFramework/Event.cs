using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarbFramework
{
    public class Event : IEvent
    {
<<<<<<< Updated upstream
        Guid subject {  get; set; }
        internal Guid subjectLocation { get; set; }
        DateTime pointInTime { get; set; }
        internal Status status { get; set; }
=======
        public Guid subject {  get; internal set; }
        public Guid subjectLocation { get; internal set; }
        public DateTime pointInTime { get; internal set; }
        public Status status { get; internal set; }
>>>>>>> Stashed changes

        internal Event (Guid subject, Guid subjectLocation, DateTime pointInTime, Status status)
        {
            this.subject = subject;
            this.subjectLocation = subjectLocation;
            this.pointInTime = pointInTime;
            this.status = status;
        }
    }
}
