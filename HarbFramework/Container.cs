using Gruppe8.HarbNet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Gruppe8.HarbNet.Advanced;

namespace Gruppe8.HarbNet
{
    /// <summary>
    /// Class representing storage containers to be used in a _harbor simulation.
    /// </summary>
    public class Container : StorageUnit
    {
        /// <summary>
        /// Gets the unique ID for container.
        /// </summary>
        /// <returns>Returns a Guid object representing the unique ID of the container.</returns>
        public override Guid ID { get; } = Guid.NewGuid();
        
        /// <summary>
        /// Gets a ReadOnlyCollection of StatusLog objects containing information on status changes the container has gone through throughout a simulation.
        /// Each StatusLog object contains information about one status change that happened to the container. Together the list holds information about the entire
        /// history of status changes that happened to the Container.
        /// </summary>
        /// <returns>Returns a ReadOnlyCollection with StatusLog objects with information on status changes the container has gone through throughout a simulation.</returns>
        public override ReadOnlyCollection<StatusLog> History => HistoryIList.AsReadOnly();
        
        /// <summary>
        /// Gets a IList of StatusLog objects containing information on status changes the container has gone through throughout a simulation.
        /// Each StatusLog object contains information about one status change that happened to the container. 
        /// </summary>
        /// <returns>Returns an Ilist with StatusLog objects with informations on the status changes the container has gone through throughout a simulation.</returns>
        internal IList<StatusLog> HistoryIList {  get; } = new List<StatusLog>();
        
        /// <summary>
        /// Gets the container's size
        /// </summary>
        /// <returns>Returns a ContainerSize enum representing the container's size</returns>
        public override ContainerSize Size { get; internal set; }
        
        /// <summary>
        /// Gets the containers weight in tonns.
        /// </summary>
        /// <returns>Returns an int value representing the containers weight in tonns.</returns>
        public override int WeightInTonn { get; internal set; }
        
        /// <summary>
        /// Gets the location ID of the container's current location.
        /// </summary>
        /// <returns>Returns a Guid object representing the ID of the containers current location.</returns>
        public override Guid CurrentLocation { get; internal set; }
        
        /// <summary>
        /// Gets a number representing how many days the Container has been stored in the _harbor's storage.
        /// </summary>
        /// <returns>Returns the int value representing the amount of days the Container has been stored in the _harbor's storage</returns>
        internal int DaysInStorage { get; set; }

        /// <summary>
        /// Constructor used to create objects of the Container class.
        /// </summary>
        /// <param name="size">A ContainerSize enum representing the Size of container to be created.</param>
        /// <param name="WeightInTonn">Int value representing the weight in tonns of container to be created.</param>
        /// <param name="currentPosition">Guid object representing the ID of the current position of the Container to be created.</param>
        internal Container(ContainerSize size, int weightInTonn, Guid currentPosition) {
            Size = size;
            CurrentLocation = currentPosition;
            WeightInTonn = weightInTonn;
        }

        /// <summary>
        /// Constructor used to create objects of the Container class.
        /// </summary>
        /// <param name="size">A ContainerSize enum representing the Size of container to be created.</param>
        /// <param name="WeightInTonn">Int value representing the weight in tonns of container to be created.</param>
        /// <param name="currentPosition">Guid object representing the ID of the current position of the Container to be created.</param>
        /// <param name="id">Guid representing the ID of the container to be created</param>
        /// <param name="containerHistory">An IList of StatusLog objects representing the history of the container to be created</param>
        internal Container(
            ContainerSize size, 
            int weightInTonn, 
            Guid currentPosition, 
            Guid id, 
            IList<StatusLog> containerHistory)
        {
            ID = id;
            Size = size;
            CurrentLocation = currentPosition;
            WeightInTonn = weightInTonn;
            HistoryIList = containerHistory;
        }

        /// <summary>
        /// Adds a StatusLog object to the Containers history list. This method is used to record information about a single
        /// status change that happened to the container.
        /// </summary>
        /// <param name="status">Status enum representing the new status of the container.</param>
        /// <param name="currentTime">DateTime object representing the date and time the status change occured./param>
        internal void AddStatusChangeToHistory (Status status, DateTime currentTime)
        {
            HistoryIList.Add(new StatusLog(ID, CurrentLocation, currentTime, status));
        }

        /// <summary>
        /// Gets current status of the container, which is the most recent status change in the HistoryList with StatusLogs.
        /// </summary>
        /// <returns>Returns a Status enum with the current status of the container.</returns>
        public override Status GetCurrentStatus()
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
        /// Adds another day to the total amount of days the container has been in the _harbor's storage.
        /// </summary>
        internal void AddAnotherDayInStorage()
        {
            DaysInStorage++;
        }

        /// <summary>
        /// Prints a container's entire HistoryList to console. Information printed includes the date and time and status of the container for all status changes in the containers entire History.
        /// </summary>
        public override void PrintHistory()
        {
            Console.WriteLine($"Container ID: {ID.ToString()}"); 
            foreach (StatusLog sl in HistoryIList)
            {
                Console.WriteLine($"Date: {sl.Timestamp} Status: {sl.Status}\n");
            }
        }

        /// <summary>
        /// Gets a string containing information about the container's entire History. Information in the string includes the date and time and status of the container for all status changes in the containers entire History. 
        /// </summary>
        /// <returns>Returns a String representing the history of a the container</returns>
        public override string HistoryToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append($"Container ID: {ID.ToString()}\n");
            foreach (StatusLog sl in HistoryIList) {
                sb.Append($"Container Id: {sl.Subject} Date: {sl.Timestamp} Status: {sl.Status}\n");
            }
            return sb.ToString();
        }

        /// <summary>
        /// Gets a String with containing the container's ID, Container's size and int value representing it's weight in tonn. 
        /// </summary>
        /// <returns>Returns a String with containing the container's ID, Container's size and int value representing it's weight in tonn. </returns>
        public override string ToString()
        {
            return $"ID: {ID}, Size: {Size}, Weight: {WeightInTonn} tonnes";
        }
    }
}
