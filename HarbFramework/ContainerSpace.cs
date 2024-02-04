using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarbFramework
{
    internal class ContainerSpace
    {
        internal Guid ID = Guid.NewGuid();
        internal ContainerSize Size { get; set; }
        internal bool Free {  get; set; }
        internal Guid storedContainer;

        internal ContainerSpace (ContainerSize size)
        {
            this.Size = size;
            this.Free = true;
            this.storedContainer = Guid.Empty;
        }
        
    }
}
