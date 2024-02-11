namespace HarbFramework
{
    public interface ISimulation
    {
        public IList<Log> History { get; }
        public IList<Log> Run();
        public void PrintShipHistory();
        public void PrintContainerHistory();
    }
}