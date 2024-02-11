namespace HarbFramework
{
    public interface ISimulation
    {
        public IList<Log> History { get; }
        public IList<Log> Run();
    }
}