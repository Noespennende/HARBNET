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
        internal Guid ID = Guid.NewGuid();
        internal ICollection<Event> History {  get; set; } = new List<Event>();
        internal ContainerSize Size { get; set; }
        internal int WeightInTonn { get; set; }
        internal Guid CurrentPosition { get; set; }

        internal Container(ContainerSize size, int WeightInKG, Guid currentPosition) {
            this.Size = size;
            this.CurrentPosition = currentPosition;
            this.WeightInTonn = WeightInKG;
        }

        internal Guid GetId() { return ID; }

        internal void AddHistoryEvent (Status status, DateTime currentTime)
        {
            History.Add(new Event(ID, CurrentPosition, currentTime, status));
        }
    }
}
