using HarbFramework;

namespace harbNet
{
    public interface IShip
    {
        public  Guid GetID();
        public ShipSize shipSize { get; }
        public DateTime startDate { get; }
        public int roundTripInDays { get; }
        public Guid currentLocation { get; }
        public ICollection<Event> history { get; }
        public ICollection<String> getContainersOnBoard { get; }
        public int containerCapacity { get; }
        public int maxWeightInTonn { get; }
        public int baseWeightInTonn { get; }
        public int currentWeightInTonn { get; }
    }
}