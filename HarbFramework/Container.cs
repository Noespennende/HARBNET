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
        public Guid ID { get; }
        public IList<Event> History {  get; internal set; } = new List<Event>();
        public ContainerSize Size { get; internal set; }
        public int WeightInTonn { get; internal set; }
        public Guid CurrentPosition { get; internal set; }

        internal Container(ContainerSize size, int WeightInTonn, Guid currentPosition) {
            this.ID = Guid.NewGuid();
            this.Size = size;
            this.CurrentPosition = currentPosition;
            this.WeightInTonn = WeightInTonn;
        }

        internal Container(ContainerSize size, int WeightInTonn, Guid currentPosition, Guid id, IList<Event> containerHistory)
        {
            this.ID = id;
            this.Size = size;
            this.CurrentPosition = currentPosition;
            this.WeightInTonn = WeightInTonn;
            this.History = containerHistory;
        }

        internal void AddHistoryEvent (Status status, DateTime currentTime)
        {
            History.Add(new Event(ID, CurrentPosition, currentTime, status));
        }

        public Status GetCurrentStatus()
        {
            if (History.Count > 0)
            {
                return History.Last().Status;
            }
            else
            {
                return Status.None;
            }
            
        }
    }
}
