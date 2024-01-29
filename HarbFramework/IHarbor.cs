using System;
using harbNet;

public interface IHarbor
{

    // CS0526: Interfaces cannot contain constructors
    //: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/compiler-messages/constructor-errors?f1url=%3FappId%3Droslyn%26k%3Dk(CS0526)#constructor-declaration
    
    /*public Harbor (ICollection<Ship> listOfShips, int numberOfSmallDocks, int numberOfMediumDocks, int numberOfLargeDocks, int numberOfSmallContainerSpaces, int numberOfMediumContainerSpaces,
        int numberOfLargeContainerSpaces); */

    public string GetShipStatus (Guid ShipID);
    public string GetDockStatus(Guid dockID);
    public string GetStatusAllDocks();
    public string GetStatusAllShips();

}
