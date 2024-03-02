using Gruppe8.HarbNet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe8.HarbNet
{
    /// <summary>
    /// Ships to be used in a simulation.
    /// </summary>

    public class Ship : IShip
    {
        /// <summary>
        /// Gets the unique ID for the ship
        /// </summary>
        /// <returns>Returns a Guid object representing the ships unique ID</returns>
        public Guid ID { get; }
        /// <summary>
        /// Gets the ships size. 
        /// </summary>
        /// <returns>Returns a ShipSize enumm representing the ships size</returns>
        public ShipSize ShipSize { get; internal set; }
        /// <summary>
        /// Gets the ships name. 
        /// </summary>
        /// <returns>Returns a string value representing the ships name</returns>
        public String Name { get; internal set; }
        /// <summary>
        /// Gets the date and time the ship first started its voyage.
        /// </summary>
        /// <returns>Returns a DateTime object representing the date and time the ship first started its voyage</returns>
        public DateTime StartDate { get; internal set; }
        /// <summary>
        /// Gets the number of days the ship uses to complete a roundtrip at sea before returning to harbour.
        /// </summary>
        /// <returns>Returns an int value representing the number of days the ship uses to do a round trip at sea.</returns>
        public int RoundTripInDays { get; internal set; }
        /// <summary>
        /// Gets the ID of the ships current location
        /// </summary>
        /// <returns>Returns a Guid object representing the ID of the ships current location</returns>
        public Guid CurrentLocation { get; internal set; }
        /// <summary>
        /// Gets a IList of StatusLog objects containing information on status changes the ship has gone trough troughout a simulation.
        /// </summary>
        /// <returns>Returns an IList with StatusLog objects with information on status changes the ship has gone trough troughout a simulation.</returns>
        public IList<StatusLog> History { get; }
        /// <summary>
        /// Gets all the containers in the ships storage.
        /// </summary>
        /// <returns>Returns an IList with Container objects representing the containers in the ships storage.</returns>
        public IList<Container> ContainersOnBoard {  get; } = new List<Container>();
        /// <summary>
        /// Gets the container capacity of the ship.
        /// </summary>
        /// <returns>Returns an int value representing the max number of containers the ship can store.</returns>
        public int numberOfSmallContainersOnBoard { get; internal set; }
        /// <summary>
        /// Gets the number of small containers on board
        /// </summary>
        public int numberOfMediumContainersOnBoard { get; internal set; }
        /// <summary>
        /// gets the number of medium containers on boar
        /// </summary>
        public int numberOfLargeContainersOnBoard { get; internal set; }
        /// <summary>
        /// gets the number of Large containers on board
        /// </summary>

        public int ContainerCapacity { get; internal set; }
        /// <summary>
        /// Gets the ships max weight the ship in tonns can be before it sinks
        /// </summary>
        /// <returns>Returns an int value representing the max weight the ship can be in tonns.</returns>
        public int MaxWeightInTonn {  get; internal set; }
        /// <summary>
        /// Gets the weight of the ship when its storage is empty
        /// </summary>
        /// <returns>Returns an int value representing the weight of the ship when the storage is empty.</returns>
        public int BaseWeightInTonn { get; internal set; }
        /// <summary>
        /// Gets the current weight of the ship including the cargo weight. 
        /// </summary>
        /// <returns>Returns an int value representing the current weight of the ship</returns>
        public int CurrentWeightInTonn { get; internal set; }
        /// <summary>
        /// Gets and sets the number of containers the ship can load onboard in one hour.
        /// </summary>
        /// <returns>Returns an int value representing the number of containers the ship can load onboard in one hour</returns>
        internal int ContainersLoadedPerHour { get; set; }
        /// <summary>
        /// Gets and sets the number of containers the ship can take out of its own storage and load to harbour in one hour.
        /// </summary>
        /// <returns>Returns an int value representing the number of containers can berth in one hour</returns>
        internal int BaseBerthingTimeInHours { get; set; }
        /// <summary>
        /// Gets and sets the number of hours it takes for the ship to dock or undock to harbour.
        /// </summary>
        /// <returns>Returns an int value representing the number of hours it takes for the ship to dock or undock to harbour</returns>
        internal int BaseDockingTimeInHours { get; set; }
        /// <summary>
        /// Gets and sets a bolean representing if the ship will only do one single trip.
        /// </summary>
        /// <returns>Returns a boolean that is true if the ship will only do one single trip and false otherwise</returns>
        internal bool IsForASingleTrip { get; set; } = false;
        internal bool HasBeenAlteredThisHour = false;

        /// <summary>
        /// Creates a new ship object.
        /// </summary>
        /// <param name="shipName">Name of the ship to be created</param>
        /// <param name="shipSize">Size of the ship to be created</param>
        /// <param name="startDate">Date and time for when the ship will start its first voyage</param>
        /// <param name="isForASingleTrip">True if the ship should only do one trip, false otherwise.</param>
        /// <param name="roundTripInDays">Number of days the ship uses to complete a roundtrip at sea before returning to harbour.</param>
        /// <param name="numberOfContainersOnBoard">How many containers will be in the ships storage when it enters the harbor for the first time.</param>
        public Ship (String shipName, ShipSize shipSize, DateTime startDate, bool isForASingleTrip, int roundTripInDays, int numberOfContainersOnBoard)
        {
            this.ID = Guid.NewGuid();
            this.Name = shipName;
            this.ShipSize = shipSize;
            this.StartDate = startDate;
            this.RoundTripInDays = roundTripInDays;
            this.ContainersOnBoard = new List<Container>();
            
            this.IsForASingleTrip = isForASingleTrip;
            this.History = new List<StatusLog>();

            if (shipSize == ShipSize.Large)
            {
                this.ContainersLoadedPerHour = 8;
            }
            else if (shipSize == ShipSize.Medium)
            {
                this.ContainersLoadedPerHour = 6;
            }
            else
            {
                this.ContainersLoadedPerHour = 4;
            }

            History.Add(new StatusLog(this.ID, Guid.Empty, startDate, Status.Anchoring));
            
            SetBaseShipInformation(shipSize);

            if (!isForASingleTrip) {
                AddContainersOnBoard(numberOfContainersOnBoard);
            }
        }

        public Ship(string shipName, ShipSize shipSize, DateTime startDate, bool isForASingleTrip, int roundTripInDays,
             int numberOfSmallContainersOnBoard, int numberOfMediumContainersOnBoard,
             int numberOfLargeContainersOnBoard)
        {
            this.ID = Guid.NewGuid();
            this.Name = shipName;
            this.ShipSize = shipSize;
            this.StartDate = startDate;
            this.RoundTripInDays = roundTripInDays;
            this.ContainersOnBoard = new List<Container>();
            this.IsForASingleTrip = isForASingleTrip;
            this.History = new List<StatusLog>();

            if (shipSize == ShipSize.Large)
            {
                this.ContainersLoadedPerHour = 8;
            }
            else if (shipSize == ShipSize.Medium)
            {
                this.ContainersLoadedPerHour = 6;
            }
            else
            {
                this.ContainersLoadedPerHour = 4;
            }

            History.Add(new StatusLog(this.ID, Guid.Empty, startDate, Status.Anchoring));
            

            SetBaseShipInformation(shipSize);

            if (!isForASingleTrip)
            {
                AddContainersOnBoard(ContainerSize.Small, numberOfSmallContainersOnBoard);
                AddContainersOnBoard(ContainerSize.Medium, numberOfMediumContainersOnBoard);
                AddContainersOnBoard(ContainerSize.Large, numberOfLargeContainersOnBoard);
            }
        }
        /// <summary>
        /// Creates a new ship object.
        /// </summary>
        /// <param name="shipName">Name of the ship to be created</param>
        /// <param name="shipSize">Size of the ship to be created</param>
        /// <param name="startDate">Date and time for when the ship will start its first voyage</param>
        /// <param name="isForASingleTrip">True if the ship should only do one trip, false otherwise.</param>
        /// <param name="roundTripInDays">Number of days the ship uses to complete a roundtrip at sea before returning to harbour.</param>
        /// <param name="containersOnboard">Containers in the ships cargo.</param>
        /// <param name="currentHistory">The ships history so far.</param>
        internal Ship(String shipName, ShipSize shipSize, DateTime startDate, bool isForASingleTrip, int roundTripInDays, Guid id, IList<Container> containersOnboard, IList<StatusLog> currentHistory)
        {
            this.Name = shipName;
            this.ShipSize = shipSize;
            this.StartDate = startDate;
            this.RoundTripInDays = roundTripInDays;
            this.IsForASingleTrip = isForASingleTrip;
            this.ID = id;
            this.History = currentHistory;
            this.ContainersOnBoard = containersOnboard;

            if (shipSize == ShipSize.Large)
            {
                this.ContainersLoadedPerHour = 10;
            }
            else if (shipSize == ShipSize.Medium)
            {
                this.ContainersLoadedPerHour = 8;
            }
            else
            {
                this.ContainersLoadedPerHour = 6;
            }

            SetBaseShipInformation(shipSize);
        }
        /// <summary>
        /// Sets container capacity, Base Weight (in tonn) and Max weight based on the ships size.
        /// </summary>
        /// <param name="shipSize">Size of the ship</param>
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

            this.CurrentWeightInTonn = BaseWeightInTonn;

            foreach (Container container in ContainersOnBoard)
            {
                this.CurrentWeightInTonn += container.WeightInTonn;
            }

            CheckForValidWeight();
        }

        /// <summary>
        /// Generate new containers and adds them to the ship storage.
        /// </summary>
        /// <param name="numberOfcontainersOnBoard">The number of containers to be added to the ships storage</param>
        private void AddContainersOnBoard(int numberOfcontainersOnBoard)
        {
            for (int i = 0; i < numberOfcontainersOnBoard; i++)
            {
                if (i % 3 == 0)
                {
                    CheckForValidWeight();
                    Container smallContainer = new Container(ContainerSize.Small, 10, this.ID);
                    smallContainer.History.Add(new StatusLog(smallContainer.ID, this.ID, StartDate, Status.Transit));
                    ContainersOnBoard.Add(smallContainer);
                    CurrentWeightInTonn += smallContainer.WeightInTonn;
                    

                }
                if (i % 3 == 1)
                {
                    CheckForValidWeight();
                    Container mediumContainer = new Container(ContainerSize.Medium, 15, this.ID);
                    mediumContainer.History.Add(new StatusLog(mediumContainer.ID, this.ID, StartDate, Status.Transit));
                    ContainersOnBoard.Add(mediumContainer);
                    CurrentWeightInTonn += mediumContainer.WeightInTonn;
                }
                if (i % 3 == 2)
                {
                    CheckForValidWeight();
                    Container largeContainer = new Container(ContainerSize.Large, 15, this.ID);
                    largeContainer.History.Add(new StatusLog(largeContainer.ID, this.ID, StartDate, Status.Transit));
                    ContainersOnBoard.Add(largeContainer);
                    CurrentWeightInTonn += largeContainer.WeightInTonn;
                }
            }
        }
        /// <summary>
        /// Adds numbers of different size containers on board
        /// </summary>
        /// <param name="containerSize">Size of the container</param>
        /// <param name="numberOfcontainersOnBoard">Number of containers</param>
        private void AddContainersOnBoard(ContainerSize containerSize, int numberOfcontainersOnBoard)
        {
            for (int i = 0; i < numberOfcontainersOnBoard; i++)
            { 
                CheckForValidWeight();
                Container ContainertoAdd = null;
            
                if(containerSize == ContainerSize.Small) { 
                   ContainertoAdd = new Container(ContainerSize.Small, 10, this.ID);
                }
                if (containerSize == ContainerSize.Medium)
                {
                    ContainertoAdd = new Container(ContainerSize.Medium, 15, this.ID);
                }
                if (containerSize == ContainerSize.Large)
                {
                    ContainertoAdd = new Container(ContainerSize.Large, 20, this.ID);
                }
                if (ContainertoAdd != null)
                {
                    ContainertoAdd.History.Add(new StatusLog(ContainertoAdd.ID, this.ID, StartDate, Status.Transit));
                    ContainersOnBoard.Add(ContainertoAdd);
                    CurrentWeightInTonn += ContainertoAdd.WeightInTonn;
                }
            }
        }

        /// <summary>
        /// Checks if the ships current weight does not exeede its maxweight and that the ships container capacity is not exeeded. Throws exeptions if they are.
        /// </summary>
        private void CheckForValidWeight()
        {
            if (CurrentWeightInTonn > MaxWeightInTonn)
            {
                throw new Exception("The ships current weight is to heavy. Max overall container weight for small ships is 600 tonns (about 55 containers), for medium ships: 1320 tonns (about 55 containers), for large ships: 5600 tonns (about 150 containers)");
            }
            else if (this.ShipSize == ShipSize.Small && ContainersOnBoard.Count > ContainerCapacity)
            {
                throw new Exception("The ship has too many containers on board. The container capacity for small ships is max 20 containers");
            }
            else if (this.ShipSize == ShipSize.Medium && ContainersOnBoard.Count > ContainerCapacity)
            {
                throw new Exception("The ship has too many containers on board. The container capacity for small ships is max 50 containers");
            }
            else if (this.ShipSize == ShipSize.Large && ContainersOnBoard.Count > ContainerCapacity)
            {
                throw new Exception("The ship has too many containers on board. The container capacity for small ships is max 100 containers");
            }
        }

        /// <summary>
        /// Generates a new StatusLog object and adds it to the ships history.
        /// </summary>
        /// <param name="currentTime">Date and time the status change occured.</param>
        /// <param name="currentLocation">ID for the location the ship is located when the status change occured</param>
        /// <param name="status">Status the ship had when the status change occured</param>
        /// <returns>Returns StatusLog object containing information about the ship at the time the StatusLog were created</returns>
        internal StatusLog AddStatusChangeToHistory (DateTime currentTime, Guid currentLocation, Status status)
        {
            StatusLog currentStatusChange = new StatusLog(ID,currentLocation, currentTime, status);
            History.Add(currentStatusChange);
            return currentStatusChange;
        }

        /// <summary>
        /// Gets a container of the given size from the ships storage
        /// </summary>
        /// <param name="containerSize">Size of the container to be returned.</param>
        /// <returns>Returns a container of the given size from the ships storage if one exists. Else returns Null</returns>
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

        /// <summary>
        /// Gets the count of the number of containers of the given size in the ships storage.
        /// </summary>
        /// <param name="containerSize">Size of the containers to be counted.</param>
        /// <returns>Returns an int representing the containers of the given size in the ships storage</returns>
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

        /// <summary>
        /// Removes the container with the given ID from the Ships storage
        /// </summary>
        /// <param name="containerID">Unique ID for the container to be removed.</param>
        /// <returns>Returns true if the container was found and removed, false if not.</returns>
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

        /// <summary>
        /// Adds the given container to the ships storage.
        /// </summary>
        /// <param name="container">Container to be added to the ships storage.</param>
        internal void AddContainer (Container container)
        {
            ContainersOnBoard.Add(container);
            CurrentWeightInTonn += container.WeightInTonn;
        }

        /// <summary>
        /// Sets the HasBeenAlteredThisHour variable to false.
        /// </summary>
        internal void SetHasBeenAlteredThisHourToFalse()
        {
            HasBeenAlteredThisHour = false;
        }
        /// <summary>
        /// Sets the HasBeenAlteredThisHour variable to true.
        /// </summary>
        internal void SetHasBeenAlteredThisHourToTrue()
        {
            HasBeenAlteredThisHour = true;
        }
        /// <summary>
        /// Gets the value of the HasBeenAlteredThisHour variable.
        /// </summary>
        /// <returns>Returns the boolean value of the HasBeenAlteredThisHour variable.</returns>
        internal bool GetHasBeenAlteredThisHour()
        {
            return HasBeenAlteredThisHour;
        }

        /// <summary>
        /// Gets the current status of the ship
        /// </summary>
        /// <returns>Returns a status enum with the current status of the ship</returns>
        internal Status GetCurrentStatus()
        {
            if(History.Count > 0)
            {
                return History.Last().Status;
            } else
            {
                return Status.None;
            }
        }

        /// <summary>
        /// Checks the status the ship had for the given date and Time
        /// </summary>
        ///  <param name="time">Date and time to be checked</param>
        /// <returns>Returns a status enum with the status of the ship had at the given date time</returns>
        internal Status GetStatusAtPointInTime(DateTime time)
        {
            Status shipStatus = new Status();
            foreach (StatusLog statusLogObject in History)
            {
                if (statusLogObject.PointInTime < time)
                {
                    shipStatus = statusLogObject.Status;
                }
                else if (statusLogObject.PointInTime > time)
                {
                    break;
                }
            }
            return shipStatus;
        }

        /// <summary>
        /// Prints the ships entire history to consol.
        /// </summary>
        public void PrintHistory()
        {
            Console.WriteLine($"Ship ID: {ID}");
            foreach (StatusLog his in History)
            {

                Console.WriteLine($"ShipId: {his.Subject} Date: {his.PointInTime} Status: {his.Status}|\n");

            }
        }

        /// <summary>
        /// Returns the ships entire history in the form of a string.
        /// </summary>
        /// <returns> a String containing the ships entire history </returns>

        public String HistoryToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append($"Ship ID: {ID}" + "\n");

            foreach (StatusLog his in History)
            {

                sb.Append($"ShipId: {his.Subject} Date: {his.PointInTime} Status: {his.Status}\n");

            }
            return sb.ToString();
        }

        /// <summary>
        /// Returns a string containing information about the ship
        /// </summary>
        /// <returns> a String containing information about the ship </returns>
        public override string ToString()
        {
            int large = 0;
            int medium = 0;
            int small = 0;

            foreach (Container container in ContainersOnBoard)
            {
                if (container.Size == ContainerSize.Small)
                {
                    small++;
                } else if (container.Size == ContainerSize.Medium)
                {
                    medium++;
                } else
                {
                    large++;
                }
            }

            return ($"ID: {ID}, Navn: {Name}, Size: {ShipSize}, Start date: {StartDate.ToString()}, Round trip time: {RoundTripInDays} days, Containers on board: {small} small," +
                $" {medium} medium, {large} large, Base weight: {BaseWeightInTonn} tonnes, Current weight: {CurrentWeightInTonn} tonnes, Max weight: {MaxWeightInTonn} tonnes.");
        }
    }
}
