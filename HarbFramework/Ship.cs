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
        public Guid ID { get; } = Guid.NewGuid();
        public ShipSize ShipSize { get; internal set; }
        public String ShipName { get; internal set; }
        public DateTime StartDate { get; internal set; }
        public int RoundTripInDays { get; internal set; }
        public Guid CurrentLocation { get; internal set; }
        public IList<Event> History { get; internal set; }
        public IList<Container> ContainersOnBoard {  get; set; } = new List<Container>();
        public int ContainerCapacity { get; internal set; }
        public int MaxWeightInTonn {  get; internal set; }
        public int BaseWeightInTonn { get; internal set; }
        public int CurrentWeightInTonn { get; internal set; }
        internal int ContainersLoadedPerHour { get; set; }
        internal int BaseBerthingTimeInHours { get; set; }
        internal int BaseDockingTimeInHours { get; set; }

        internal bool IsForASingleTrip { get; set; } = false;
        internal bool HasBeenAlteredThisHour = false;


        public Ship (String ShipName, ShipSize shipSize, DateTime StartDate, bool IsForASingleTrip, int roundTripInDays, int numberOfcontainersOnBoard)
        {
            this.ShipName = ShipName;
            this.ShipSize = shipSize;
            this.StartDate = StartDate;
            this.RoundTripInDays = roundTripInDays;
            this.ContainersOnBoard = new List<Container>();

            if (shipSize == ShipSize.Large)
            {
                ContainersLoadedPerHour = 10;
            }
            else if (shipSize == ShipSize.Medium)
            {
                ContainersLoadedPerHour = 8;
            }
            else
            {
                ContainersLoadedPerHour = 6;
            }
            this.History = new List<Event>();

            if(!IsForASingleTrip)
                AddContainersOnBoard(numberOfcontainersOnBoard);

            SetBaseShipInformation(shipSize);

        }


        private void SetBaseShipInformation(ShipSize shipSize)
        {
            if (shipSize == ShipSize.Small)
            {
                this.ContainerCapacity = 20;
                this.BaseWeightInTonn = 5000;
                this.MaxWeightInTonn = BaseWeightInTonn + (24 * 25);

                this.BaseDockingTimeInHours = 3;
                this.BaseBerthingTimeInHours = 6;

            }
            else if (shipSize == ShipSize.Medium)
            {

                this.ContainerCapacity = 50;
                this.BaseWeightInTonn = 50000;
                this.MaxWeightInTonn = BaseWeightInTonn + (24 * 55);

                this.BaseDockingTimeInHours = 5;
                this.BaseBerthingTimeInHours = 7;

            }
            else if (shipSize == ShipSize.Large)
            {
                this.ContainerCapacity = 100;
                this.BaseWeightInTonn = 100000;
                this.MaxWeightInTonn = BaseWeightInTonn + (24 * 150);

                this.BaseDockingTimeInHours = 7;
                this.BaseBerthingTimeInHours = 9;
            }
            else
            {
                throw new Exception("Invalid ship size given. Valid ship sizes: ShipSize.Small, ShipSize.Medium, ShipSize.Large");
            }

            int currentWeight = BaseWeightInTonn;

            foreach (Container container in ContainersOnBoard)
            {
                currentWeight += container.WeightInTonn;
            }

            if (currentWeight > MaxWeightInTonn)
            {
                throw new Exception("The ships current weight is to heavy. Max overall container weight for small ships is 600 tonns (about 55 containers), for medium ships: 1320 tonns (about 55 containers), for large ships: 5600 tonns (about 150 containers)");
            }
            else if (shipSize == ShipSize.Small && ContainersOnBoard.Count > ContainerCapacity)
            {
                throw new Exception("The ship has too many containers on board. The container capacity for small ships is max 20 containers");
            };
        }

        private void AddContainersOnBoard(int numberOfcontainersOnBoard)
        {
            for (int i = 0; i < numberOfcontainersOnBoard; i++)
            {
                if (i % 3 == 0)
                {
                    ContainersOnBoard.Add(new Container(ContainerSize.Small, 10, this.ID));
                }
                if (i % 3 == 1)
                {
                    ContainersOnBoard.Add(new Container(ContainerSize.Medium, 15, this.ID));
                }
                if (i % 3 == 2)
                {
                    ContainersOnBoard.Add(new Container(ContainerSize.Large, 15, this.ID));
                }
            }
        }


        internal Event AddHistoryEvent (DateTime currentTime, Guid currentLocation, Status status)
        {
            Event currentEvent = new Event(ID,currentLocation, currentTime, status);
            History.Add(currentEvent);
            return currentEvent;


        }

        internal Container GetContainer(ContainerSize containerSize)
        {
            foreach (Container container in ContainersOnBoard)
            {
                if (container.Size == containerSize)
                {
                    return container;
                }
            }
            return null;
        }


        internal int GetNumberOfContainersOnBoard (ContainerSize containerSize)
        {
            int count = 0;
            foreach (Container container in ContainersOnBoard)
            {
                if (container.Size == containerSize)
                {
                    count++;
                }
            }
            return count;
        }

        internal bool RemoveContainer (Guid containerID)
        {
            foreach(Container container in ContainersOnBoard)
            {
                if (container.ID == containerID)
                {
                    ContainersOnBoard.Remove(container);
                    CurrentWeightInTonn -= container.WeightInTonn;
                    return true;
                }
            }
            return false;
        }

        internal void AddContainer (Container container)
        {
            ContainersOnBoard.Add(container);
            CurrentWeightInTonn += container.WeightInTonn;
        }
        internal void SetNextStepCheckFalse()
        {
            HasBeenAlteredThisHour = false;
        }
        internal void SetNextStepCheckTrue()
        {
            HasBeenAlteredThisHour = true;
        }
        internal bool GetNextStepCheck()
        {
            return HasBeenAlteredThisHour;
        }

        public void PrintHistory()
        {
            Console.WriteLine($"ShipId: {ID}");
            foreach (Event his in History)
            {

                Console.WriteLine($"ShipId: {his.Subject}Date: {his.PointInTime}|Status: {his.Status}|\n");

            }
        }
    }
}
