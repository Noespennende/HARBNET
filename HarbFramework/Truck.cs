using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe8.HarbNet
{
    public class Truck
    {
        public Guid ID { get; internal set; } = new Guid();
        public Guid Location { get; internal set; }
        public Status Status { get; internal set; }
        public Container Container { get; internal set; }

        public Truck (Guid location)
        {
            this.Location = location;
            this.Status = Status.Queuing;
            this.Container = null;
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
