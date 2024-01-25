using HarbFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace harbNet
{
    internal class Container
    {
        internal Guid id = Guid.NewGuid();
        internal ICollection<Event> history {  get; set; }
        internal ContainerSize size { get; set; }
        internal int WeightInTonn { get; set; }
        internal Guid currentPoison { get; set; }

        internal Container(ContainerSize size, int WeightInKG, Guid currentPosition) {
            this.size = size;
            this.currentPoison = currentPosition;
            this.WeightInTonn = WeightInKG;
        }

        internal Guid getId() { return id; }

        internal void addHistoryEvent (Status status, DateTime currentTime)
        {
            history.Add(new Event(id, currentPoison, currentTime, status));
        }
    }
}
