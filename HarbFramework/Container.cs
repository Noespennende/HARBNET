using Gruppe8.HarbNet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe8.HarbNet
{
    /// <summary>
    /// Container to be used in the simulation
    /// </summary>
    public class Container : IContainer
    {
        /// <summary>
        /// Unique ID for container
        /// </summary>
        /// <return>Returns the unique ID defining a specific container</return>
        public Guid ID { get; }

        /// <summary>
        /// Gets the history of the container
        /// </summary>
        /// <return>Returns a list of StatusLog objects that contains informations on the status changes the container has been through</return>
        public IList<StatusLog> History {  get; } = new List<StatusLog>();
        /// <summary>
        /// Gets the size of the container
        /// </summary>
        /// <return>Returns the size of the container</return>
        public ContainerSize Size { get; internal set; }

        /// <summary>
        /// Gets the containers weight in tonn
        /// </summary>
        /// <return>Returns the int value of the containers weight in tonn</return>
        public int WeightInTonn { get; internal set; }

        /// <summary>
        /// Unique ID for the current position
        /// </summary>
        /// <return>Returns the Guid for the current position of container</return>
        public Guid CurrentPosition { get; internal set; }

        /// <summary>
        /// Constructor for Container, creates a new container object
        /// </summary>
        /// <param name="size">Size of container</param>
        /// <param name="WeightInTonn">weight of container in tonn</param>
        /// <param name="currentPosition">Current posistion of container</param>
        internal Container(ContainerSize size, int WeightInTonn, Guid currentPosition) {
            this.ID = Guid.NewGuid();
            this.Size = size;
            this.CurrentPosition = currentPosition;
            this.WeightInTonn = WeightInTonn;
        }

        /// <summary>
        /// Constructor for Container, creates a new container object
        /// </summary>
        /// <param name="size">Size of container</param>
        /// <param name="WeightInTonn">weight of container in tonn</param>
        /// <param name="currentPosition">Current position of container</param>
        /// <param name="id">Unique ID defining the container</param>
        /// <param name="containerHistory">History of status changes the container has been through stored in StatusLog objects.</param>
        internal Container(ContainerSize size, int WeightInTonn, Guid currentPosition, Guid id, IList<StatusLog> containerHistory)
        {
            this.ID = id;
            this.Size = size;
            this.CurrentPosition = currentPosition;
            this.WeightInTonn = WeightInTonn;
            this.History = containerHistory;
        }

        /// <summary>
        /// Adds a StatusLog object to the containers history.
        /// </summary>
        /// <param name="status">current status of container</param>
        /// <param name="currentTime">The time the status change to the container happened</param>
        internal void AddStatusChangeToHistory (Status status, DateTime currentTime)
        {
            History.Add(new StatusLog(ID, CurrentPosition, currentTime, status));
        }

        /// <summary>
        /// Gets current status of container
        /// </summary>
        /// <returns>StatusLog with information about the latest status change of container. Returns none if there is no history registered</returns>
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

        /// <summary>
        /// Prints the containers entire history to console 
        /// </summary>
        public void PrintHistory()
        {
            Console.WriteLine($"Container ID: {ID.ToString()}"); 
            foreach (StatusLog sl in History)
            {
                Console.WriteLine($"Date: {sl.PointInTime} Status: {sl.Status}\n");
            }
        }

        /// <summary>
        /// Returns a String representing the history of the container. 
        /// </summary>
        /// <returns>String representing the history of a the container</returns>
        public String HistoryToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append($"Container ID: {ID.ToString()}\n");
            foreach (StatusLog sl in History) {
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
