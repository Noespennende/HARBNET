using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarbFramework
{
    public interface IEvent
    {
        public Guid Subject { get; }
        public Guid SubjectLocation { get; }
        public DateTime PointInTime { get; }
        public Status Status { get; }
    }
}
