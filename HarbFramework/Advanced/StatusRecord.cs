namespace Gruppe8.HarbNet.Advanced
{
    /// <summary>
    /// Abstract class defining the public API for StatusRecords such as the StatusLog class.
    /// StatusRecords to be stored in a ships or containers history. Each object containing information about the subject at the time the subject went trough a status change.
    /// This abstract class can be used to make fakes to be used in testing of the API. 
    /// </summary>
    public abstract class StatusRecord
    {
        /// <summary>
        /// Gets the unique ID for the subject.
        /// </summary>
        /// <returns>Returns a Guid object representing the subjects unique ID.</returns>
        public abstract Guid Subject { get; internal set; }

        /// <summary>
        /// Gets the ID of the subjects location.
        /// </summary>
        /// <returns>Returns a Guid object representing the locations unique ID.</returns>
        public abstract Guid SubjectLocation { get; internal set; }

        /// <summary>
        /// Gets the date and time the status change occured. 
        /// </summary>
        /// <returns>Returns a DateTime object representing the date and time the subjects status change occured.</returns>
        public abstract DateTime Timestamp { get; internal set; }

        /// <summary>
        /// Gets the current status of the subject.
        /// </summary>
        /// <return>Returns a Status enum representing the latest registered status of the subject.</return>
        public abstract Status Status { get; internal set; }

        /// <summary>
        /// Returns a string with the date and time of StatusRecord, subjets ID, subjets location and current status.
        /// </summary>
        /// <returns> a String containing information about the subject on a given point in time.</returns>
        public abstract string ToString();

    }
}
