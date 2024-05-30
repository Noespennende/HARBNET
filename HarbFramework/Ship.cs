using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using Gruppe8.HarbNet.Advanced;

namespace Gruppe8.HarbNet
{
    /// <summary>
    /// Ship class defining ships to be used in a simulation. A ship is a CargoVessel that can transport Containers 
    /// from A to B on the ochean.
    /// </summary>

    public class Ship : CargoVessel
    {
        /// <summary>
        /// Gets the unique ID for the ship.
        /// </summary>
        /// <returns>Returns a Guid object representing the ships unique ID.</returns>
        public override Guid ID { get; }

        /// <summary>
        /// Gets the ships size. The ship's size determins the base and max weight of the ship as well as how much cargo it can hold.
        /// </summary>
        /// <returns>Returns a ShipSize enumm representing the ships size.</returns>
        public override ShipSize ShipSize { get; internal set; }

        /// <summary>
        /// Gets the ships name. 
        /// </summary>
        /// <returns>Returns a string value representing the name of the ship.</returns>
        public override string Name { get; internal set; }

        /// <summary>
        /// Gets the date and time the ship will arrive to the harbor for the first time.
        /// </summary>
        /// <returns>Returns a DateTime object representing the date and time the ship will arrive to the harbor for the first time.</returns>
        public override DateTime StartDate { get; internal set; }

        /// <summary>
        /// Gets the number of days the ship uses to complete a roundtrip at sea before returning to harbour. The number indicates the amount of days
        /// it takes for the ship to leave the harbor, travel at sea to its delivery destination, deliver its cargo, travel back at sea and arrive to the harbor again.
        /// </summary>
        /// <returns>Returns an int value representing the number of days the ship uses to do a round trip at sea.</returns>
        public override int RoundTripInDays { get; internal set; }

        /// <summary>
        /// Gets the location ID of the ships current location.
        /// </summary>
        /// <returns>Returns a Guid object representing the location ID of the ships current location.</returns>
        public override Guid CurrentLocation { get; internal set; }

        /// <summary>
        /// Gets a ReadOnlyCollection of StatusLog objects containing information on status changes the Ship has gone through throughout a simulation.
        /// Each StatusLog object contains information about one status change that happened to the Ship. Together the list holds information about the entire
        /// history of status changes that happened to the ship.
        /// </summary>
        /// <returns>Returns an ReadOnlyCollection with StatusLog objects with information on status changes the ship has gone trough troughout a simulation.</returns>
        public override ReadOnlyCollection<StatusLog> History => HistoryIList.AsReadOnly();

        /// <summary>
        /// Gets all the containers in the ship's storage.
        /// </summary>
        /// <returns>Returns an IList with Container objects representing the containers in the ships storage.</returns>
        public override IList<Container> ContainersOnBoard { get; } = new List<Container>();

        /// <summary>
        /// The maximum amount of containers the ship can hold. Both half sized and full sized containers counts as 1 container.
        /// </summary>
        /// <returns>Returns an int value representing the maximum amount of containers a ship can hold.</returns>
        public override int ContainerCapacity { get; internal set; }

        /// <summary>
        /// Gets the max weight in tonns the ship is allowed to reach.
        /// </summary>
        /// <returns>Returns an int value representing the max weight in tonns the ship is allowed to reach.</returns>
        public override int MaxWeightInTonn { get; internal set; }

        /// <summary>
        /// Gets the weight of the ship when its storage is empty.
        /// </summary>
        /// <returns>Returns an int value representing the weight of the ship when the storage is empty.</returns>
        public override int BaseWeightInTonn { get; internal set; }

        /// <summary>
        /// Gets the current weight of the ship including the cargo weight. This includes both the base weight of the ship
        /// in addition to the weight of all its cargo.
        /// </summary>
        /// <returns>Returns an int value representing the current weight of the ship.</returns>
        public override int CurrentWeightInTonn { get; internal set; }

        /// <summary>
        /// Gets the TransitStatus of the ship. The transit status indicates at witch part of the transit the ship is currently.
        /// For example wether or not it is leaving or arriving to the harbor.
        /// </summary>
        /// <returns>Returns a TransitStatus enum representing where in the transit cycle the ship is currently</returns>
        internal TransitStatus TransitStatus { get; set; }

        /// <summary>
        /// Gets a IList of StatusLog objects containing information on status changes the Ship has gone through throughout a simulation.
        /// Each StatusLog object contains information about one status change that happened to the Ship. Together the list holds information about the entire
        /// history of status changes that happened to the ship.
        /// </summary>
        /// <returns>Returns an IList with StatusLog objects with information on status changes the ship has gone trough troughout a simulation.</returns>
        internal IList<StatusLog> HistoryIList { get; }

        /// <summary>
        /// Gets the maximum number of containers the ship can load in one hour. A container load is defined as loading a container
        /// from the a harbor loading crane and in to its own storage.
        /// </summary>
        /// <returns>Returns an int value representing the number of containers the ship can load or unload in one hour.</returns>
        internal int ContainersLoadedPerHour { get; set; }

        /// <summary>
        /// Gets and sets the number of containers the ship unload from its own storage an on to a harbor loading crane.
        /// </summary>
        /// <returns>Returns an int value representing the number of containers can be unloaded from the ships cargo in one hour.</returns>
        internal int BaseBerthingTimeInHours { get; set; }

        /// <summary>
        /// Gets and sets a number indicating the number of hours it takes for the ship to dock or undock to a harbor dock.
        /// </summary>
        /// <returns>Returns an int value representing the number of hours it takes for the ship to dock or undock to harbour.</returns>
        internal int BaseDockingTimeInHours { get; set; }

        /// <summary>
        /// Gets and sets a bolean representing if the ship will only do one single trip. If this value is true it means the ship
        /// will only perform one voyage before permanently docking to an available ship dock in the harbor.
        /// </summary>
        /// <returns>Returns a boolean that is true if the ship will only do one single trip and false otherwise.</returns>
        internal bool IsForASingleTrip { get; set; } = false;

        /// <summary>
        /// Checks if the ship has performed any action this hour. 
        /// </summary>
        /// <returns>Returns a bool that is true if ship has performed an action this past hour, or false if ship has performed no action.</returns>
        internal bool HasBeenAlteredThisHour = false;

        /// <summary>
        /// Gets and sets a number indicating the percentage of Containers that will be directly unloaded from the ship's storage and on to trucks on the harbor.
        /// A value of 100 represents 100%. A value of 50 represnets 50%.
        /// </summary>
        /// <returns>Returns the int value indicating the percentage of containers in the ships storage that will be directly unloaded from the ship and on to trucks on the harbor.</returns>
        internal int ContainersLeftForTrucks { get; set; } = 0;

        /// <summary>
        /// Creates a new object of the Ship class. A ship is a CargoVessel that can transport Containers 
        /// from A to B on the ochean.
        /// </summary>
        /// <param name="shipName">Name of the ship object to be created.</param>
        /// <param name="shipSize">shipSize enum representing the size of the ship to be created. The ship's size determins the base and max weight of the ship as well as how much cargo it can hold.</param>
        /// <param name="startDate">Date and time for when the ship will arrive to the harbor for the first time.</param>
        /// <param name="isForASingleTrip">A bolean representing if the ship will only do one single trip. If this value is true it means the ship
        /// will only perform one voyage before permanently docking to an available ship dock in the harbor.</param>
        /// <param name="roundTripInDays">Int value representing the number of days the ship uses to complete a roundtrip at sea before returning to harbour. The number indicates the amount of days
        /// it takes for the ship to leave the harbor, travel at sea to its delivery destination, deliver its cargo, travel back at sea and arrive to the harbor again.</param>
        /// <param name="containersToBeStoredInCargo">IList of Container objects that will be placed in the Ship's cargo when it first arrives to the harbor</param>
        public Ship(
            string shipName,
            ShipSize shipSize,
            DateTime startDate,
            bool isForASingleTrip,
            int roundTripInDays,
            IList<Container> containersToBeStoredInCargo)
        {
            ID = Guid.NewGuid();
            Name = shipName;
            ShipSize = shipSize;
            StartDate = startDate;
            RoundTripInDays = roundTripInDays;
            IsForASingleTrip = isForASingleTrip;
            HistoryIList = new List<StatusLog>();

            if (isForASingleTrip)
            {
                TransitStatus = TransitStatus.Leaving;
            }

            else
            {
                TransitStatus = TransitStatus.Arriving;
            }

            if (shipSize == ShipSize.Large)
            {
                ContainersLoadedPerHour = 8;
            }

            else if (shipSize == ShipSize.Medium)
            {
                ContainersLoadedPerHour = 6;
            }

            else
            {
                ContainersLoadedPerHour = 4;
            }

            foreach (Container container in containersToBeStoredInCargo)
            {
                ContainersOnBoard.Add((Container)container);
            }

            HistoryIList.Add(new StatusLog(ID, Guid.Empty, startDate, Status.Anchoring));

            SetBaseShipInformation(shipSize);



        }

        /// <summary>
        /// Creates a new object of the Ship class. A ship is a CargoVessel that can transport Containers 
        /// from A to B on the ochean.
        /// </summary>
        /// <param name="shipName">Name of the ship object to be created.</param>
        /// <param name="shipSize">shipSize enum representing the size of the ship to be created. The ship's size determins the base and max weight of the ship as well as how much cargo it can hold.</param>
        /// <param name="startDate">Date and time for when the ship will arrive to the harbor for the first time.</param>
        /// <param name="isForASingleTrip">A bolean representing if the ship will only do one single trip. If this value is true it means the ship
        /// will only perform one voyage before permanently docking to an available ship dock in the harbor.</param>
        /// <param name="roundTripInDays">Int value representing the number of days the ship uses to complete a roundtrip at sea before returning to harbour. The number indicates the amount of days
        /// it takes for the ship to leave the harbor, travel at sea to its delivery destination, deliver its cargo, travel back at sea and arrive to the harbor again.</param>
        /// <param name="numberOfHalfContainersOnBoard">Int value representing the amount of Small containers that will be in the ships storage when it enters the harbor for the first time.</param>
        /// <param name="numberOfFullContainersOnBoard">Int value representing the amount of Large containers that will be in the ships storage when it enters the harbor for the first time.</param>
        public Ship(
            string shipName,
            ShipSize shipSize,
            DateTime startDate,
            bool isForASingleTrip,
            int roundTripInDays,
            int numberOfHalfContainersOnBoard,
            int numberOfFullContainersOnBoard)
        {
            ID = Guid.NewGuid();
            Name = shipName;
            ShipSize = shipSize;
            StartDate = startDate;
            RoundTripInDays = roundTripInDays;
            ContainersOnBoard = new List<Container>();
            IsForASingleTrip = isForASingleTrip;
            HistoryIList = new List<StatusLog>();

            if (shipSize == ShipSize.Large)
            {
                ContainersLoadedPerHour = 8;
            }

            else if (shipSize == ShipSize.Medium)
            {
                ContainersLoadedPerHour = 6;
            }

            else
            {
                ContainersLoadedPerHour = 4;
            }

            SetBaseShipInformation(shipSize);

            AddContainersOnBoard(ContainerSize.Half, numberOfHalfContainersOnBoard);
            AddContainersOnBoard(ContainerSize.Full, numberOfFullContainersOnBoard);
        }

        /// <summary>
        /// Creates a new object of the Ship class. A ship is a CargoVessel that can transport Containers 
        /// from A to B on the ochean.
        /// </summary>
        /// <param name="shipName">Name of the ship object to be created.</param>
        /// <param name="shipSize">shipSize enum representing the size of the ship to be created. The ship's size determins the base and max weight of the ship as well as how much cargo it can hold.</param>
        /// <param name="startDate">Date and time for when the ship will arrive to the harbor for the first time.</param>
        /// <param name="isForASingleTrip">A bolean representing if the ship will only do one single trip. If this value is true it means the ship
        /// will only perform one voyage before permanently docking to an available ship dock in the harbor.</param>
        /// <param name="roundTripInDays">Int value representing the number of days the ship uses to complete a roundtrip at sea before returning to harbour. The number indicates the amount of days
        /// it takes for the ship to leave the harbor, travel at sea to its delivery destination, deliver its cargo, travel back at sea and arrive to the harbor again.</param>
        /// <param name="id">Unique Guid that will be set as the ship's ID</param>
        /// <param name="containersOnboard">An IList containing Containers objects representing the containers in the ships cargo.</param>
        /// <param name="currentHistory">An IList containing StatusRecord objects representing the ships history so far in the simulation.</param>
        internal Ship(
            string shipName,
            ShipSize shipSize,
            DateTime startDate,
            bool isForASingleTrip,
            int roundTripInDays,
            Guid id,
            IList<Container> containersOnboard,
            IList<StatusLog> currentHistory)
        {
            Name = shipName;
            ShipSize = shipSize;
            StartDate = startDate;
            RoundTripInDays = roundTripInDays;
            IsForASingleTrip = isForASingleTrip;
            ID = id;
            HistoryIList = currentHistory;
            ContainersOnBoard = containersOnboard;

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

            SetBaseShipInformation(shipSize);
        }

        /// <summary>
        /// Prints the ships entire history to console. Information printed includes the ship's name, ID, Date and Time of all status changes and the coresponding status the ship had at those times.
        /// </summary>
        public override void PrintHistory()
        {
            Console.WriteLine($"Ship name: {Name}, Ship ID {ID}");
            Console.WriteLine("------------------------------------");

            foreach (StatusLog his in HistoryIList)
            {
                Console.WriteLine($"Date: {his.Timestamp} Status: {his.Status}|\n");
            }
        }

        /// <summary>
        /// Gives a string containing information of the ship's entire history. Information in the String includes the ship's name, ID, Date and Time of all status changes and the coresponding status the ship had at those times.
        /// </summary>
        /// <returns> a String containing information about the ship's entire history.</returns>
        public override string HistoryToString()
        {
            StringBuilder sb = new();

            sb.Append($"Ship name: {Name}, Ship ID {ID}" + "\n");
            sb.Append("------------------------------------\n");

            foreach (StatusLog his in HistoryIList)
            {
                sb.Append($"Date: {his.Timestamp} Status: {his.Status}\n");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Returns a string with the Ships ID, name, size, startdate, round trip time, amount on containers of the differenct containerSizes on board, base weight in tonn, current weight in tonn and max weigth in tonn the ship can handle.
        /// </summary>
        /// <returns> a String containing information about the ship.</returns>
        /// 
        public override string ToString()
        {
            int full = 0;
            int half = 0;

            foreach (Container container in ContainersOnBoard)
            {
                if (container.Size == ContainerSize.Half)
                {
                    half++;
                }

                else
                {
                    full++;
                }
            }

            return
                $"ID: {ID}, " +
                $"Name: {Name}, " +
                $"Size: {ShipSize}," +
                $" Start date: {StartDate}, " +
                $"Round trip time: {RoundTripInDays} days, " +
                $"Containers on board: {half} small, {full} large, " +
                $"Base weight: {BaseWeightInTonn} tonnes, " +
                $"Current weight: {CurrentWeightInTonn} tonnes, " +
                $"Max weight: {MaxWeightInTonn} tonnes.";
        }

        /// <summary>
        /// Creates a new Container object and adds it to the ship's storage.
        /// </summary>
        /// <param name="time">DateTime object representing the Date and time of the simulation the Container is created.</param>
        internal void GenerateContainer(DateTime time)
        {
            if (ContainersOnBoard.Count < ContainerCapacity)
            {
                Random rand = new();
                ContainerSize size;

                if (rand.Next(0, 2) == 0)
                {
                    size = ContainerSize.Full;
                }

                else
                {
                    size = ContainerSize.Half;
                }

                Container container = new(size, (int)size, ID);

                container.AddStatusChangeToHistory(Status.Transit, time);
                ContainersOnBoard.Add(container);
                CurrentWeightInTonn += container.WeightInTonn;
            }
        }

        /// <summary>
        /// Gets a number indicating the amount of containers that will be directly loaded from the ship's cargo on to trucks in the harbor.
        /// The number is calculated based on the percentage value given to the method.
        /// </summary>
        /// <param name="percentTrucks">Double value representing the percentage of containers that is to be loaded directly on trucks from the ships cargo.
        /// a value of 1 indicates 100%, a value of 0.5 indicates 50%.</param>
        /// <returns>Returns the int value representing the the total amount of containers for trucks to load.</returns>
        internal int GetNumberOfContainersToTrucks(double percentTrucks)
        {
            double decimalNumberOfContainers = ContainersOnBoard.Count * percentTrucks;

            int numberOfContainersToTrucks;
            double decimalPart = decimalNumberOfContainers - Math.Floor(decimalNumberOfContainers);

            if (decimalPart < 0.5)
            {
                numberOfContainersToTrucks = (int)Math.Floor(decimalNumberOfContainers);
            }

            else
            {
                numberOfContainersToTrucks = (int)Math.Ceiling(decimalNumberOfContainers);
            }

            return numberOfContainersToTrucks;
        }

        /// <summary>
        /// Gets a value indicating the number of containers that will be loaded to the Harbors storage area from the ships cargo. The number is calculated
        /// based on the percentage value given to the method.
        /// </summary>
        /// <param name="percentTrucks">Double value representing the percentage of containers that is to be loaded directly on trucks from the ships cargo.
        /// a value of 1 indicates 100%, a value of 0.5 indicates 50%.</param>
        /// <returns>Returns the int value of the total amount of containers in the ship's storage that will be loaded to the Harbors Storage area.</returns>
        internal int GetNumberOfContainersToStorage(double percentTrucks)
        {
            int numberOfContainersToStorage = ContainersOnBoard.Count - GetNumberOfContainersToTrucks(percentTrucks);

            return numberOfContainersToStorage;
        }

        /// <summary>
        /// Generates a new StatusLog object and adds it to the ships history. This method is used to record information about a single
        /// status change that happened to the Ship.
        /// </summary>
        /// <param name="currentTime">Date and time the status change occured.</param>
        /// <param name="currentLocation">Guid object representing the location the ship is located when the status change occured.</param>
        /// <param name="status">Status enum representing the new status of the ship.</param>
        /// <returns>Returns StatusLog object containing information about the ship at the time the StatusLog were created.</returns>
        internal StatusLog AddStatusChangeToHistory(DateTime currentTime, Guid currentLocation, Status status)
        {
            StatusLog currentStatusChange = new(ID, currentLocation, currentTime, status);
            HistoryIList.Add(currentStatusChange);

            return currentStatusChange;
        }

        /// <summary>
        /// Gets a container of the given size from the ships storage.
        /// </summary>
        /// <param name="containerSize">ContainerSize enum representing the size of the container to be retrieved.</param>
        /// <returns>Returns a container object of the given size from the ships storage if one exists, if None of the gived containerSize is found null is returned.</returns>
        /// <exception cref="ArgumentException">Exception thrown if given a ContainerSize enum value that is not valid for concrete implementation.</exception>
        internal Container? GetContainer(ContainerSize containerSize)
        {
            if (containerSize == ContainerSize.None)
            {
                throw new ArgumentException("Invalid input. Container's of that size is not meant for concrete implementation. Please use ContainerSize 'Half' or 'Full' instead.", nameof(containerSize));
            }

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
        /// Gets a number indicating the amount of containers of the given size that exists in the Ship's storage.
        /// </summary>
        /// <param name="containerSize">ContainerSize enum representing the size of the container's to be counted.</param>
        /// <returns>Returns an int value representing the amount of the given size containers that exists in the ships storage.</returns>
        internal int GetNumberOfContainersOnBoard(ContainerSize containerSize)
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
        /// Removes the container with the given ID from the Ship's storage.
        /// </summary>
        /// <param name="containerID">Unique ID for the container object to be removed from the ship's storage.</param>
        /// <returns>Returns true if the container was found and removed from ship's storage, false if not.</returns>
        internal bool RemoveContainer(Guid containerID)
        {
            foreach (Container container in ContainersOnBoard)
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
        /// Adds the given container to the ship's storage.
        /// </summary>
        /// <param name="container">Container object to be added to the ships storage.</param>
        internal void AddContainer(Container container)
        {
            ContainersOnBoard.Add(container);
            CurrentWeightInTonn += container.WeightInTonn;
        }

        /// <summary>
        /// Gets the current status of the ship.
        /// </summary>
        /// <returns>Returns a status enum representing the current status of the ship.</returns>
        internal Status GetCurrentStatus()
        {
            if (HistoryIList.Count > 0)
            {
                return HistoryIList.Last().Status;
            }

            else
            {
                return Status.None;
            }
        }

        /// <summary>
        /// Checks witch status the ship had at the given date and time.
        /// </summary>
        ///  <param name="time">Date and time of witch the status of the ship is to be returned.</param>
        /// <returns>Returns a status enum with the status the ship had at the given DateTime.</returns>
        internal Status GetStatusAtPointInTime(DateTime time)
        {
            Status shipStatus = Status.None;
            foreach (StatusLog statusLogObject in HistoryIList)
            {
                if (statusLogObject.Timestamp < time)
                {
                    shipStatus = statusLogObject.Status;
                }

                else if (statusLogObject.Timestamp > time)
                {
                    break;
                }
            }

            return shipStatus;
        }

        /// <summary>
        /// Unloads container from ship's cargo.
        /// </summary>
        /// <returns>Returns null if there is zero containers on board, otherwise returns the Container object that is unloaded from the ship's cargo.</returns>
        internal Container? UnloadContainer()
        {
            if (ContainersOnBoard.Count <= 0)
            {
                return null;
            }

            Container containertoUnload = ContainersOnBoard[0];
            ContainersOnBoard.RemoveAt(0);
            CurrentWeightInTonn -= containertoUnload.WeightInTonn;

            return containertoUnload;
        }

        /// <summary>
        /// Sets basic infomation for the ship based on the ship's size. The information set includes
        /// container capacity, Base Weight (in tonn) and Max weight based on the ships size.
        /// </summary>
        /// <param name="shipSize">ShipSize enum representing the size of the ship</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if shipSize is not found.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Throws exception if the weight set for the ship is too high or if the ship contains more Containers than the ship of this size can handle.</exception>
        private void SetBaseShipInformation(ShipSize shipSize)
        {
            if (shipSize == ShipSize.Small)
            {
                ContainerCapacity = 20;
                BaseWeightInTonn = 5000;
                MaxWeightInTonn = BaseWeightInTonn + (24 * 25);

                BaseDockingTimeInHours = 3;
                BaseBerthingTimeInHours = 6;

            }

            else if (shipSize == ShipSize.Medium)
            {

                ContainerCapacity = 50;
                BaseWeightInTonn = 50000;
                MaxWeightInTonn = BaseWeightInTonn + (24 * 55);

                BaseDockingTimeInHours = 5;
                BaseBerthingTimeInHours = 7;

            }

            else if (shipSize == ShipSize.Large)
            {
                ContainerCapacity = 100;
                BaseWeightInTonn = 100000;
                MaxWeightInTonn = BaseWeightInTonn + (24 * 150);

                BaseDockingTimeInHours = 7;
                BaseBerthingTimeInHours = 9;
            }

            else
            {
                throw new ArgumentException("Invalid ship size given. Valid ship sizes: ShipSize.Small, ShipSize.Medium, ShipSize.Large", nameof(shipSize));
            }

            CurrentWeightInTonn = BaseWeightInTonn;

            foreach (Container container in ContainersOnBoard)
            {
                CurrentWeightInTonn += container.WeightInTonn;
            }
            try
            {
                CheckForValidWeight();
            }
            catch (ArgumentOutOfRangeException)
            {
                throw;
            }

        }

        /// <summary>
        /// Generate new containers of the given size and adds them to the ship's storage.
        /// </summary>
        /// <param name="containerSize">ContainerSize enum representing the size of the container's to be added to the ship's storage.</param>
        /// <param name="numberOfContainersToBeAddedToStorage">Int value representing the number of containers to be added to the ship's storage.</param>
        /// <exception cref="ArgumentOutOfRangeException">Throws exception if the weight set for the ship is too high or if the ship contains more Containers than the ship of this size can handle.</exception>
        private void AddContainersOnBoard(ContainerSize containerSize, int numberOfContainersToBeAddedToStorage)
        {
            for (int i = 0; i < numberOfContainersToBeAddedToStorage; i++)
            {
                try
                {
                    CheckForValidWeight();
                }
                catch (ArgumentOutOfRangeException)
                {
                    throw;
                }

                Container? ContainertoAdd = null;

                if (containerSize == ContainerSize.Half)
                {
                    ContainertoAdd = new(ContainerSize.Half, 10, ID);
                }

                if (containerSize == ContainerSize.Full)
                {
                    ContainertoAdd = new Container(ContainerSize.Full, 20, ID);
                }

                if (ContainertoAdd != null)
                {
                    ContainertoAdd.HistoryIList.Add(new StatusLog(ContainertoAdd.ID, ID, StartDate, Status.Transit));
                    ContainersOnBoard.Add(ContainertoAdd);
                    CurrentWeightInTonn += ContainertoAdd.WeightInTonn;
                }
            }
        }

        /// <summary>
        /// Checks if the ships current weight does not exeede its maxweight and that the ships container capacity is not exeeded. Throws ArgumentOutOfRangeException if they are.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Throws exception if the weight set for the ship is too high or if the ship contains more Containers than the ship of this size can handle.</exception>
        private void CheckForValidWeight()
        {
            if (CurrentWeightInTonn > MaxWeightInTonn)
            {
                throw new ArgumentOutOfRangeException("The ships current weight is to heavy. Max overall container weight for small ships is 600 tonns (about 55 containers), for medium ships: 1320 tonns (about 55 containers), for large ships: 5600 tonns (about 150 containers)");
            }

            else if (ShipSize == ShipSize.Small && ContainersOnBoard.Count > ContainerCapacity)
            {
                throw new ArgumentOutOfRangeException("The ship has too many containers on board. The container capacity for small ships is max 20 containers");
            }

            else if (ShipSize == ShipSize.Medium && ContainersOnBoard.Count > ContainerCapacity)
            {
                throw new ArgumentOutOfRangeException("The ship has too many containers on board. The container capacity for medium ships is max 50 containers");
            }

            else if (ShipSize == ShipSize.Large && ContainersOnBoard.Count > ContainerCapacity)
            {
                throw new ArgumentOutOfRangeException("The ship has too many containers on board. The container capacity for large ships is max 100 containers");
            }
        }
    }
}
