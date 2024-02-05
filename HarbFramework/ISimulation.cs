namespace HarbFramework
{
    public interface ISimulation
    {
        public IList<Log> History { get; }
        public void Run(DateTime startTime, DateTime endTime);
    }
}