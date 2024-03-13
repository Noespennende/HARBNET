using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe8.HarbNet
{
    public class Adv : IAdv
    {
        public Guid ID { get; internal set; }
        public Container Container { get; internal set; }
        public Status Status { get; internal set; }
        public Guid Location {  get; internal set; }
        public ReadOnlyCollection<StatusLog> History { get { return HistoryIList.AsReadOnly(); } }
        internal IList<StatusLog> HistoryIList { get; set; }

        public Adv (Guid id, Guid location)
        {
            this.ID = id;
            this.Container = null;
            this.Location = location;
            this.HistoryIList = new List<StatusLog>();
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
