using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe8.HarbNet
{

    public class Crane : ICrane
    {
        public Guid ID { get; internal set; }
        public Container Container { get; internal set; }
        public int ContainersLoadedPerHour { get; internal set; }
        public Status Status { get; internal set; }
        public Guid location { get; internal set; }
        public ReadOnlyCollection<StatusLog> History { get { return HistoryIList.AsReadOnly(); } }
        internal IList<StatusLog> HistoryIList { get; set; }

        internal Crane (int containersLoadedPerHour, Guid location)
        {
            this.ID = Guid.NewGuid();
            this.ContainersLoadedPerHour = containersLoadedPerHour;
            this.location = location;
            this.Container = null;
        }

        internal Guid LoadContainer (Container containerToBeLoaded)
        {
            this.Container = containerToBeLoaded;
            return containerToBeLoaded.ID;
        }

        internal Container UnloadContainer ()
        {
            Container containerToBeUnloaded = this.Container;
            this.Container = null;
            return containerToBeUnloaded;

        }
    }
}
