using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarbFramework
{
    internal class ContainerSpace
    {
        internal Guid id = Guid.NewGuid();
        internal ContainerSize size { get; set; }
        internal bool free {  get; set; }
        internal Guid storedContainer;

        internal ContainerSpace (ContainerSize size)
        {
            this.size = size;
            this.free = true;
            this.storedContainer = Guid.Empty;
        }
        
    }
}
