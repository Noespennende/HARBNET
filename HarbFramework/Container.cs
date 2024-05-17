using Gruppe8.HarbNet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Gruppe8.HarbNet.PublicApiAbstractions;

namespace Gruppe8.HarbNet
{
    /// <summary>
    /// Container to be used in the simulation.
    /// </summary>
    public class Container : StorageUnit
    {
        /// <summary>
        /// Gets the unique ID for container.
        /// </summary>
        /// <returns>Returns a Guid object representing the containers unique ID.</returns>
        public override Guid ID { get; }
        /// <summary>
        /// Gets a ReadOnlyCollection of StatusLog objects containing information on status changes the container has gone through throughout a simulation.
        /// </summary>
        /// <returns>Returns a ReadOnlyCollection with StatusLog objects with information on status changes the container has gone through throughout a simulation.</returns>
        public override ReadOnlyCollection<StatusLog> History { get { return HistoryIList.AsReadOnly(); } }
        /// <summary>
        /// Gets a IList of StatusLog objects containing information on status changes the container has gone through throughout a simulation.
        /// </summary>
        /// <returns>Returns an Ilist with StatusLog objects with informations on the status changes the container has gone through throughout a simulation.</returns>
        internal IList<StatusLog> HistoryIList {  get; } = new List<StatusLog>();
        /// <summary>
        /// Gets the containers size
        /// </summary>
        /// <returns>Returns a ContainerSize enum representing the containers size</returns>
        public override ContainerSize Size { get; internal set; }
        /// <summary>
        /// Gets the containers weight in tonn.
        /// </summary>
        /// <returns>Returns an int value representing the containers weight in tonn.</returns>
        public override int WeightInTonn { get; internal set; }
        /// <summary>
        /// Gets the ID if the containers current position.
        /// </summary>
        /// <returns>Returns a Guid object representing the ID of the containers current position.</returns>
        public override Guid CurrentPosition { get; internal set; }
        /// <summary>
        /// Gets the number of days the container has been in storage.
        /// </summary>
        /// <returns>Returns the int value of the days container has been in storage</returns>
        internal int DaysInStorage { get; set; }

        /// <summary>
        /// Constructor for Container, creates a new container object.
        /// </summary>
        /// <param name="size">The containerSize enum representing the Size of container to be created.</param>
        /// <param name="WeightInTonn">Int value representing the weight of container to be created in tonn.</param>
        /// <param name="currentPosition">Unique Guid representing the position the Container to be created will currently be located.</param>
        internal Container(ContainerSize size, int WeightInTonn, Guid currentPosition) {
            this.ID = Guid.NewGuid();
            this.Size = size;
            this.CurrentPosition = currentPosition;
            this.WeightInTonn = WeightInTonn;
        }

        /// <summary>
        /// Constructor for Container, creates a new container object.
        /// </summary>
        /// <param name="size">The containerSize enum representing the Size of container to be created.</param>
        /// <param name="WeightInTonn">Int value representing the weight of container to be created in tonn.</param>
        /// <param name="currentPosition">Unique Guid representing the position the Container to be created will currently be located.</param>
        /// <param name="id">Unique Guid defining the container object to be created.</param>
        /// <param name="containerHistory">An IList containing the history of status changes the container to be created has been through stored in StatusLog objects.</param>
        internal Container(ContainerSize size, int WeightInTonn, Guid currentPosition, Guid id, IList<StatusLog> containerHistory)
        {
            this.ID = id;
            this.Size = size;
            this.CurrentPosition = currentPosition;
            this.WeightInTonn = WeightInTonn;
            this.HistoryIList = containerHistory;
        }

        /// <summary>
        /// Adds a StatusLog object to the containers history, which creates a status change.
        /// </summary>
        /// <param name="status">The Status enum representing the new status change that will be added to the containers history list.</param>
        /// <param name="currentTime">The date and time the status change to the container happened./param>
        internal void AddStatusChangeToHistory (Status status, DateTime currentTime)
        {
            HistoryIList.Add(new StatusLog(ID, CurrentPosition, currentTime, status));
        }

        /// <summary>
        /// Gets current status of the container, which is the most recent status change in the HistoryList with StatusLogs.
        /// </summary>
        /// <returns>Returns a Status enum with the most recent status of the container.</returns>
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
        /// Adds another day to the amount of days the container is stored in storage.
        /// </summary>
        internal void AddAnotherDayInStorage()
        {
            DaysInStorage++;
        }

        /// <summary>
        /// Prints a container's entire HistoryList to console, with the date and time the status change took place and status enum for each StatusLog change.
        /// </summary>
        public override void PrintHistory()
        {
            Console.WriteLine($"Container ID: {ID.ToString()}"); 
            foreach (StatusLog sl in HistoryIList)
            {
                Console.WriteLine($"Date: {sl.PointInTime} Status: {sl.Status}\n");
            }
        }

        /// <summary>
        /// Prints the containers entire HistoryList, with the container ID, date and time the status change took place and status enum for each StatusLog change in the form of a String. 
        /// </summary>
        /// <returns>Returns a String representing the history of a the container, printing each Statuslog object in the HistoryList.</returns>
        public override String HistoryToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append($"Container ID: {ID.ToString()}\n");
            foreach (StatusLog sl in HistoryIList) {
                sb.Append($"Container Id: {sl.Subject} Date: {sl.PointInTime} Status: {sl.Status}\n");
            }
            return sb.ToString();
        }

        /// <summary>
        /// Prints a String with the container's ID, ContainerSize enum and int value representing it's weight in tonn. 
        /// </summary>
        /// <returns>Returns a String with the container's ID, ContainerSize enum and int value representing it's weight in tonn.</returns>
        public override String ToString()
        {
            return ($"ID: {ID.ToString()}, Size: {Size}, Weight: {WeightInTonn} tonnes");
        }
    }
}
