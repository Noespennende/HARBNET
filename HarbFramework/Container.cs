using HarbFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace harbNet
{
    internal class Container
    {
        Guid id = Guid.NewGuid();
        ICollection<Event> History;
        ContainerSize size;
        int WeightInKG;
        Guid currentPoison;
    }
}
