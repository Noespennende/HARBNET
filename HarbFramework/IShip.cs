using HarbFramework;

namespace harbNet
{
    public interface IShip
    {
        public  Guid GetID();
        public ShipSize ShipSize { get; }
        public DateTime StartDate { get; }
        public int RoundTripInDays { get; }
        public Guid CurrentLocation { get; }
        public ICollection<Event> History { get; }
        public ICollection<String> GetContainersOnBoard { get; }
        public int ContainerCapacity { get; }
        public int MaxWeightInTonn { get; }
        public int BaseWeightInTonn { get; }
        public int CurrentWeightInTonn { get; }
    }
}