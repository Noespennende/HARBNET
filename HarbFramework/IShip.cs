using HarbFramework;

namespace harbNet
{
    public interface IShip
    {
        Guid GetID();
        ShipSize ShipSize { get; }
        DateTime StartDate { get; }
        int RoundTripInDays { get; }
        Guid CurrentLocation { get; }
        ICollection<Event> History { get; }
        ICollection<String> GetContainersOnBoard { get; }
        int ContainerCapacity { get; }
        int MaxWeightInTonn { get; }
        int BaseWeightInTonn { get; }
        int CurrentWeightInTonn { get; }
    }
}