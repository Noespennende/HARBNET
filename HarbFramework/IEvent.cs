using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarbFramework
{
    public interface IEvent
    {
        Guid subject { get; }
        Guid subjectLocation { get; }
        DateTime pointInTime { get; }
        Status status { get; };
    }
}
