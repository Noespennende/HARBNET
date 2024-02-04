namespace HarbFramework
{
    public interface ISimulation
    {
        public ICollection<Log> History { get; }
        public void Run(DateTime startTime, DateTime endTime);
    }
}