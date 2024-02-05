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
    public class Container : IContainer
    {
        public Guid ID { get; } = Guid.NewGuid();
        public IList<Event> History {  get; internal set; } = new List<Event>();
        public ContainerSize Size { get; internal set; }
        public int WeightInTonn { get; internal set; }
        public Guid CurrentPosition { get; internal set; }

        internal Container(ContainerSize size, int WeightInKG, Guid currentPosition) {
            this.Size = size;
            this.CurrentPosition = currentPosition;
            this.WeightInTonn = WeightInKG;
        }

        internal void AddHistoryEvent (Status status, DateTime currentTime)
        {
            History.Add(new Event(ID, CurrentPosition, currentTime, status));
        }
    }
}
