using Gruppe8.HarbNet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe8.HarbNet
{
    /// <summary>
    /// Container to be used in the simulation.
    /// </summary>
    public class Container : IContainer
    {
        /// <summary>
        /// Gets the unique ID for container.
        /// </summary>
        /// <returns>Returns a Guid object representing the containers unique ID.</returns>
        public Guid ID { get; }
        /// <summary>
        /// Gets a ReadOnlyCollection of StatusLog objects containing information on status changes the container has gone through throughout a simulation.
        /// </summary>
        /// <returns>Returns a ReadOnlyCollection with StatusLog objects with information on status changes the container has gone through throughout a simulation.</returns>
        public ReadOnlyCollection<StatusLog> History { get { return HistoryIList.AsReadOnly(); } }
        /// <summary>
        /// Gets a IList of StatusLog objects containing information on status changes the container has gone through throughout a simulation.
        /// </summary>
        /// <returns>Returns an Ilist with StatusLog objects with informations on the status changes the container has gone through throughout a simulation.</returns>
        internal IList<StatusLog> HistoryIList {  get; } = new List<StatusLog>();
        /// <summary>
        /// Gets the containers size
        /// </summary>
        /// <returns>Returns a ContainerSize enum representing the containers size</returns>
        public ContainerSize Size { get; internal set; }
        /// <summary>
        /// Gets the containers weight in tonn.
        /// </summary>
        /// <returns>Returns an int value representing the containers weight in tonn.</returns>
        public int WeightInTonn { get; internal set; }
        /// <summary>
        /// Gets the ID if the containers current position.
        /// </summary>
        /// <returns>Returns a Guid object representing the ID of the containers current position.</returns>
        public Guid CurrentPosition { get; internal set; }
        /// <summary>
        /// Gets the number of days the container has been in storage.
        /// </summary>
        /// <returns>Returns the int value of the days container has been in storage</returns>
        internal int DaysInStorage { get; set; }

        /// <summary>
        /// Constructor for Container, creates a new container object.
        /// </summary>
        /// <param name="size">Size of container to be created</param>
        /// <param name="WeightInTonn">Weight of container to be created in tonn.</param>
        /// <param name="currentPosition">Current posistion of container to be created.</param>
        public Container(ContainerSize size, int WeightInTonn, Guid currentPosition) {
            //SKAL VÆRE INTERNAL
            this.ID = Guid.NewGuid();
            this.Size = size;
            this.CurrentPosition = currentPosition;
            this.WeightInTonn = WeightInTonn;
        }

        /// <summary>
        /// Constructor for Container, creates a new container object.
        /// </summary>
        /// <param name="size">Size of container to be created</param>
        /// <param name="WeightInTonn">weight of container to be created in tonn.</param>
        /// <param name="currentPosition">Current position of container to ve created.</param>
        /// <param name="id">Unique ID defining the container to be created.</param>
        /// <param name="containerHistory">History of status changes the container has been through stored in StatusLog objects.</param>
        internal Container(ContainerSize size, int WeightInTonn, Guid currentPosition, Guid id, IList<StatusLog> containerHistory)
        {
            this.ID = id;
            this.Size = size;
            this.CurrentPosition = currentPosition;
            this.WeightInTonn = WeightInTonn;
            this.HistoryIList = containerHistory;
        }

        /// <summary>
        /// Adds a StatusLog object to the containers history.
        /// </summary>
        /// <param name="status">Current status of container to be created.</param>
        /// <param name="currentTime">The time the status change to the container be created happened</param>
        internal void AddStatusChangeToHistory (Status status, DateTime currentTime)
        {
            HistoryIList.Add(new StatusLog(ID, CurrentPosition, currentTime, status));
        }

        /// <summary>
        /// Gets current status of the container.
        /// </summary>
        /// <returns>Returns a Status enum with the current status of the container.</returns>
        public Status GetCurrentStatus()
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
        /// Adds another day the container is in storage.
        /// </summary>
        internal void AddAnotherDayInStorage()
        {
            DaysInStorage++;
        }

        /// <summary>
        /// Prints the containers entire history to console.
        /// </summary>
        public void PrintHistory()
        {
            Console.WriteLine($"Container ID: {ID.ToString()}"); 
            foreach (StatusLog sl in HistoryIList)
            {
                Console.WriteLine($"Date: {sl.PointInTime} Status: {sl.Status}\n");
            }
        }

        /// <summary>
        /// Returns the containers entire history in the form of a String. 
        /// </summary>
        /// <returns>String representing the history of a the container.</returns>
        public String HistoryToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append($"Container ID: {ID.ToString()}\n");
            foreach (StatusLog sl in HistoryIList) {
                sb.Append($"Container Id: {sl.Subject} Date: {sl.PointInTime} Status: {sl.Status}\n");
            }
            return sb.ToString();
        }

        /// <summary>
        /// Returns a String containing information about the container. 
        /// </summary>
        /// <returns>String containing information about the container.</returns>
        public override String ToString()
        {
            return ($"ID: {ID.ToString()}, Size: {Size}, Weight: {WeightInTonn} tonnes");
        }
    }
}
