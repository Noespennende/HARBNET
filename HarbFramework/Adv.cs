using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe8.HarbNet
{
    internal class Adv
    {
        internal Guid ID { get; set; }
        internal Container Container { get; set; }
        internal Status Status { get; set; }
        internal Guid Location {  get; set; }

        internal Adv (Guid id, Guid location)
        {
            this.ID = id;
            this.Container = null;
            this.Location = location;
        }

        internal Guid LoadContainer(Container containerToBeLoaded)
        {
            this.Container = containerToBeLoaded;
            return containerToBeLoaded.ID;
        }

        internal Container UnloadContainer()
        {
            Container containerToBeUnloaded = this.Container;
            this.Container = null;
            return containerToBeUnloaded;

        }
    }
}
