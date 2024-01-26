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
        internal ShipSize shipSize { get; set; }
        internal DateTime startDate { get; set; }
        internal int roundTripInDays { get; set; }
        internal Guid currentLocation { get; set; }
        internal ArrayList history { get; set; }
        internal ArrayList containersOnBoard { get; set; }
        internal int containerCapacity { get; set; }
        internal int maxWeighInTonn { get; set; }
        internal int baseWeigtInTonn { get; set; }
        internal int currentWeightInTonn { get; set; }
        internal int baseBerthingTimeInHours { get; set; }
        internal int baseDockingTimeInHours { get; set; }
        internal bool nextStepCheck = false;

        internal Ship (ShipSize shipSize, DateTime StartDate, int roundTripInDays, ArrayList containersOnBoard)
        {
            this.shipSize = shipSize;
            this.startDate = StartDate;
            this.roundTripInDays = roundTripInDays;
            this.containersOnBoard = containersOnBoard;

            if (shipSize == ShipSize.Small)
            {
                this.containerCapacity = 20;
                this.baseWeigtInTonn = 5000;
                this.maxWeighInTonn = baseWeigtInTonn + (24 * 25);

                this.baseDockingTimeInHours = 6;
                this.baseBerthingTimeInHours = 6;

            } else if (shipSize == ShipSize.Medium) {

                this.containerCapacity = 50;
                this.baseWeigtInTonn = 50000;
                this.maxWeighInTonn = baseWeigtInTonn + (24 * 55);

                this.baseDockingTimeInHours = 7;
                this.baseBerthingTimeInHours = 7;

            } else if(shipSize == ShipSize.Large)
            {
                this.containerCapacity = 100;
                this.baseWeigtInTonn = 100000;
                this.maxWeighInTonn = baseWeigtInTonn + (24 * 150);

                this.baseDockingTimeInHours = 9;
                this.baseBerthingTimeInHours = 9;
            } else
            {
                throw new Exception("Invalid ship size given. Valid ship sizes: ShipSize.Small, ShipSize.Medium, ShipSize.Large");
            }
        }

        internal Guid getID()
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
