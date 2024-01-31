using HarbFramework;

namespace harbNet
{
    public interface IShip
    {
        Guid GetID();
        ShipSize shipSize { get; }
        DateTime startDate { get; }
        int roundTripInDays { get; }
        Guid currentLocation { get; }
        ICollection<Event> history { get; }
        ICollection<String> getContainersOnBoard { get; }
        int containerCapacity { get; }
        int maxWeightInTonn { get; }
        int baseWeightInTonn { get; }
        int currentWeightInTonn { get; }
    }
}