using System;

public interface IHarbor
{
	public IHarbor()
	{
        public Harbor (ICollection<Ship> listOfShips, int numberOfSmallDocks, int numberOfMediumDocks, int numberOfLargeDocks, int numberOfSmallContainerSpaces, int numberOfMediumContainerSpaces,
            int numberOfLargeContainerSpaces);

        public string GetShipStatus (Guid ShipID);

        public string GetDockStatus(Guid dockID);

        public string GetStatusAllDocks();

        public string GetStatusAllShips();


    }
}
