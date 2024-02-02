using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarbFramework
{
    public interface IEvent
    {
        public Guid subject { get; }
        public Guid subjectLocation { get; }
        public DateTime pointInTime { get; }
        public Status status { get; }
    }
}
