using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarbFramework
{
    internal interface IContainer
    {
        public Guid ID { get; } 
        public IList<Event> History { get; }
        public ContainerSize Size { get; }
        public int WeightInTonn { get;  }
        public Guid CurrentPosition { get; }
    }
}
