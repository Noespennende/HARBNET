namespace HarbFramework
{
    public interface ISimulation
    {
        public IList<Log> History { get; }
        public void Run();
    }
}