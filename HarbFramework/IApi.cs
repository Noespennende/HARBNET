using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HarbFramework
{
    internal interface IApi
    {
        public void create(); //Oppretter simuleringen
        public String setName(String name); //returnerer navnet
        public Guid createPort(DockSize portSize); //returnerer Guid til porten som ble laget
        public Guid createShip(ShipSize shipSize); //Returnerer Guid til skipet som ble laget
        public Guid createShip(ShipSize shipSize, DateTime startDate); //Returnerer Guid til skipet som ble laget
        public Guid createShip(ShipSize shipSize, DateTime startDate, int roundTripInDays); //Returnerer Guid til skipet som ble laget
        public Boolean removeShip(Guid shipId); //Returnerer True hvis skipet ble fjernet, false ellers.
        public Boolean removePort(Guid PortId); //Returnerer True hvis havna ble fjernet, false ellers.
        public void simulationDuration(DateTime startDate, DateTime endDate);
        public void run(); //starte simuleringen
        public ICollection<Event> getContasinerHistory(Guid containerID);
        public ICollection<Event> getShipHistory(Guid shipID);
        public ICollection<Event> getWeatherHistory();
        public ICollection<Guid> getContainerIDs();
        public ICollection<Guid> getShipIDs();
        public void printHistoryToConsole();
        public void printHistoryToFile(String fileName);
        public int getShipTurnaroundTimeInHours(); //får int med tid i timer. 
        public int getShipTurnaroundTimeInHours(DateTime startDate, DateTime endDate); //får int med tid i timer beregnet mellom start og sluttdato
        public int getContainerTurnAroundTimeInHours();//får int med tid i timer. 
        public int getContainerTurnAroundTimeInHours(DateTime startDate, DateTime endDate); //får int med tid i timer beregnet mellom start og sluttdato
        public int getAverageLoadTimeInHours(); //får int med tid i timer. 
        public int getAverageLoadTimeInHours(DateTime startDate, DateTime endDate);  //får int med tid i timer beregnet mellom start og sluttdato
        public int getAverageQueuingTimeInHours(); //får int med tid i timer skip må vente i kø. 
        public int getAverageQueuingTimeInHours(DateTime startDate, DateTime endDate); //får int med tid i timer skip må vente i kø. 
        public int getContainersMoved(); //Antall containere flyttet i løpet av simuleringen
        public int getContainersMoved(DateTime startDate, DateTime endDate);
        public int getNumberOfDockings(); //Antall skip docket i løpet av simuleringen 
        public int getNumDockings(DateTime startDate); //Antall skip docket i løpet av simuleringen 
    }
}
