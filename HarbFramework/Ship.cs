using HarbFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace harbNet
{

    public class Ship : IShip
    {
        internal Guid id = Guid.NewGuid();
        public ShipSize shipSize { get; internal set; }
        public DateTime startDate { get; internal set; }
        public int roundTripInDays { get; internal set; }
        public Guid currentLocation { get; internal set; }
        public ICollection<Event> history { get; internal set; }
        internal ICollection<Container> containersOnBoard {  get; set; }
        public int containerCapacity { get; internal set; }
        public int maxWeightInTonn {  get; internal set; }
        public int baseWeightInTonn { get; internal set; }
        public int currentWeightInTonn { get; internal set; }
        internal int containersLoadedPerHour { get; set; }
        internal int baseBerthingTimeInHours { get; set; }
        internal int baseDockingTimeInHours { get; set; }
        internal bool nextStepCheck = false;

        internal int baseBerthingTimeInHours { get; set; }

        public Ship (ShipSize shipSize, DateTime StartDate, int roundTripInDays, int numberOfcontainersOnBoard)
        {
            this.shipSize = shipSize;
            this.startDate = StartDate;
            this.roundTripInDays = roundTripInDays;
            
            for (int i = 0; i < numberOfcontainersOnBoard; i++)
            {
                if (i%3 == 0) { 
                    containersOnBoard.Add(new Container(ContainerSize.Small, 10, this.id));
                }
                if (i%3 == 1)
                {
                    containersOnBoard.Add(new Container(ContainerSize.Medium, 15, this.id));
                }
                if (i%3 == 2)
                {
                    containersOnBoard.Add(new Container(ContainerSize.Large, 15, this.id));
                }
            } 

            if (shipSize == ShipSize.Small)
            {
                this.containerCapacity = 20;
                this.baseWeightInTonn = 5000;
                this.maxWeightInTonn = baseWeightInTonn + (24 * 25);

                this.baseDockingTimeInHours = 3;
                this.baseBerthingTimeInHours = 6;

            } else if (shipSize == ShipSize.Medium) {

                this.containerCapacity = 50;
                this.baseWeightInTonn = 50000;
                this.maxWeightInTonn = baseWeightInTonn + (24 * 55);

                this.baseDockingTimeInHours = 5;
                this.baseBerthingTimeInHours = 7;

            } else if(shipSize == ShipSize.Large)
            {
                this.containerCapacity = 100;
                this.baseWeightInTonn = 100000;
                this.maxWeightInTonn = baseWeightInTonn + (24 * 150);

                this.baseDockingTimeInHours = 7;
                this.baseBerthingTimeInHours = 9;
            } else
            {
                throw new Exception("Invalid ship size given. Valid ship sizes: ShipSize.Small, ShipSize.Medium, ShipSize.Large");
            }

            int currentWeight = baseWeightInTonn;

            foreach (Container container in containersOnBoard)
            {
                currentWeight += container.WeightInTonn;
            }
<<<<<<< Updated upstream

            try
            {
                if (currentWeight > maxWeighInTonn)
                {
                    throw new Exception("The ship's current weight is too heavy. Max overall container weight for small ships is 600 tons (about 55 containers), for medium ships: 1320 tons (about 55 containers), for large ships: 5600 tons (about 150 containers)");
                }
                else if (shipSize == ShipSize.Small && containersOnBoard.Count > containerCapacity)
                {
                    throw new Exception("The ship has too many containers on board. The container capacity for small ships is max 20 containers");
                }
                else if (shipSize == ShipSize.Medium && containersOnBoard.Count > containerCapacity)
                {
                    throw new Exception("The ship has too many containers on board. The container capacity for medium ships is max 50 containers");
                }
                else if (shipSize == ShipSize.Large && containersOnBoard.Count > containerCapacity)
                {
                    throw new Exception("The ship has too many containers on board. The container capacity for large ships is max 100 containers");
                }

=======
            
            if (currentWeight > maxWeightInTonn)
            {
                throw new Exception("The ships current weight is to heavy. Max overall container weight for small ships is 600 tonns (about 55 containers), for medium ships: 1320 tonns (about 55 containers), for large ships: 5600 tonns (about 150 containers)");
            } else if (shipSize == ShipSize.Small && containersOnBoard.Count > containerCapacity)
            {
                throw new Exception("The ship has too many containers on board. The container capacity for small ships is max 20 containers");
>>>>>>> Stashed changes
            }
            catch (Exception ex)
            {
<<<<<<< Updated upstream
                Console.WriteLine("Error: " + ex.Message);
=======
                throw new Exception("The ship has too many containers on board. The container capacity for medium ships is max 50 containers");
            }
            else if (shipSize == ShipSize.Small && containersOnBoard.Count > containerCapacity)
            {
                throw new Exception("The ship has too many containers on board. The container capacity for large ships is max 100 containers");
>>>>>>> Stashed changes
            }

        }

        public Guid GetID()
        {
            return this.id;
        }

        internal Event addHistoryEvent (DateTime currentTime, Guid currentLocation, Status status)
        {
            Event currentEvent = new Event(id,currentLocation, currentTime, status);
            history.Add(currentEvent);
            return currentEvent;
            

        }

        internal Container getContainer(ContainerSize containerSize)
        {
            foreach (Container container in containersOnBoard)
            {
                if (container.size == containerSize) 
                {
                    return container;
                }
            }
            return null;
        }

        internal int getNumberOfContainersOnBoard (ContainerSize containerSize)
        {
            int count = 0;
            foreach (Container container in containersOnBoard)
            {
                if (container.size == containerSize)
                {
                    count++;
                }
            }
            return count;
        }

        internal bool removeContainer (Guid containerID)
        {
            foreach(Container container in containersOnBoard)
            {
                if (container.id == containerID)
                {
                    containersOnBoard.Remove(container);
                    currentWeightInTonn -= container.WeightInTonn;
                    return true;
                }
            }
            return false;
        }

        internal void addContainer (Container container)
        {
            containersOnBoard.Add(container);
            currentWeightInTonn += container.WeightInTonn;
        }
        internal void setNextStepCheckFalse()
        {
            nextStepCheck = false;
        }
        internal void setNextStepCheckTrue()
        {
            nextStepCheck = true;
        }
        internal bool getNextStepCheck()
        {
            return nextStepCheck;
        }
    }
}
