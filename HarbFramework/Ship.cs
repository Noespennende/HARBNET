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
    internal class Ship
    {
        internal Guid id = Guid.NewGuid();
        internal ShipSize shipSize { public get; set; }
        internal DateTime startDate { public get; set; }
        internal int roundTripInDays {public get; set; }
        internal Guid currentLocation { public get; set; }
        internal ArrayList history {public get; set; }
        internal ArrayList containersOnBoard { public get; set; }
        internal int containerCapacity { public get; set; }
        internal int maxWeighInTonn { public get; set; }
        internal int baseWeigtInTonn {public get; set; }
        internal int currentWeightInTonn {public get; set; }
        internal int containersLoadedPerHour { get; set; }
        internal int baseDockingTimeInHours { get; set; }
        internal bool nextStepCheck = false;

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
                    containersOnBoard.Add(new Container(ContainerSize.Large, 15, this.id))
                }
            } 

            if (shipSize == ShipSize.Small)
            {
                this.containerCapacity = 20;
                this.baseWeigtInTonn = 5000;
                this.maxWeighInTonn = baseWeigtInTonn + (24 * 25);

                this.baseDockingTimeInHours = 3;
                this.baseBerthingTimeInHours = 6;

            } else if (shipSize == ShipSize.Medium) {

                this.containerCapacity = 50;
                this.baseWeigtInTonn = 50000;
                this.maxWeighInTonn = baseWeigtInTonn + (24 * 55);

                this.baseDockingTimeInHours = 5;
                this.baseBerthingTimeInHours = 7;

            } else if(shipSize == ShipSize.Large)
            {
                this.containerCapacity = 100;
                this.baseWeigtInTonn = 100000;
                this.maxWeighInTonn = baseWeigtInTonn + (24 * 150);

                this.baseDockingTimeInHours = 7;
                this.baseBerthingTimeInHours = 9;
            } else
            {
                throw new Exception("Invalid ship size given. Valid ship sizes: ShipSize.Small, ShipSize.Medium, ShipSize.Large");
            }

            int currentWeight = baseWeigtInTonn;

            for (Container container in containersOnBoard)
            {
                currentWeight += container.WeightInTonn;
            }
            
            if (currentWeight > maxWeighInTonn)
            {
                throw new Exception("The ships current weight is to heavy. Max overall container weight for small ships is 600 tonns (about 55 containers), for medium ships: 1320 tonns (about 55 containers), for large ships: 5600 tonns (about 150 containers)")
            } else if (shipSize == ShipSize.Small && containersOnBoard.Count > containerCapacity)
            {
                throw new Exception("The ship has too many containers on board. The container capacity for small ships is max 20 containers")
            }
            else if (shipSize == ShipSize.Medium && containersOnBoard.Count > containerCapacity)
            {
                throw new Exception("The ship has too many containers on board. The container capacity for medium ships is max 50 containers")
            }
            else if (shipSize == ShipSize.Small && containersOnBoard.Count > containerCapacity)
            {
                throw new Exception("The ship has too many containers on board. The container capacity for large ships is max 100 containers")
            }
        }

        public Guid getID()
        {
            return this.id;
        }

        internal Event addHistoryEvent (DateTime currentTime, Guid currentLocation, Status status)
        {
            Event currentEvent = new Event(id, currentLocation, currentTime, status);
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
