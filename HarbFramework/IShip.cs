using HarbFramework;

namespace harbNet
{
    public interface IShip
    {
        public  Guid ID { get;  }
        public ShipSize ShipSize { get; }
        public DateTime StartDate { get; }
        public int RoundTripInDays { get; }
        public Guid CurrentLocation { get; }
        public IList<Event> History { get; }
        public IList<Guid> GetContainersOnBoard { get; }
        public int ContainerCapacity { get; }
        public int MaxWeightInTonn { get; }
        public int BaseWeightInTonn { get; }
        public int CurrentWeightInTonn { get; }
    }
}