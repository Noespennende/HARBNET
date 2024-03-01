using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe8.HarbNet
{
    internal interface IApi
    {
        // Første utkastet til ett API. Denne klassen blir ikke brukt i den endelige løsningen.
        /*
        public void Create(); //Oppretter simuleringen
        public String SetName(String name); //returnerer navnet
        public Guid CreatePort(DockSize portSize); //returnerer Guid til porten som ble laget
        public Guid CreateShip(ShipSize shipSize); //Returnerer Guid til skipet som ble laget
        public Guid CreateShip(ShipSize shipSize, DateTime startDate); //Returnerer Guid til skipet som ble laget
        public Guid CreateShip(ShipSize shipSize, DateTime startDate, int roundTripInDays); //Returnerer Guid til skipet som ble laget
        public Boolean RemoveShip(Guid shipId); //Returnerer True hvis skipet ble fjernet, false ellers.
        public Boolean RemovePort(Guid PortId); //Returnerer True hvis havna ble fjernet, false ellers.
        public void SimulationDuration(DateTime startDate, DateTime endDate);
        public void Run(); //starte simuleringen
        public ICollection<Event> GetContasinerHistory(Guid containerID);
        public ICollection<Event> GetShipHistory(Guid shipID);
        public ICollection<Event> GetWeatherHistory();
        public ICollection<Guid> GetContainerIDs();
        public ICollection<Guid> GetShipIDs();
        public void PrintHistoryToConsole();
        public void PrintHistoryToFile(String fileName);
        public int GetShipTurnaroundTimeInHours(); //får int med tid i timer. 
        public int GetShipTurnaroundTimeInHours(DateTime startDate, DateTime endDate); //får int med tid i timer beregnet mellom start og sluttdato
        public int GetContainerTurnAroundTimeInHours();//får int med tid i timer. 
        public int GetContainerTurnAroundTimeInHours(DateTime startDate, DateTime endDate); //får int med tid i timer beregnet mellom start og sluttdato
        public int GetAverageLoadTimeInHours(); //får int med tid i timer. 
        public int GetAverageLoadTimeInHours(DateTime startDate, DateTime endDate);  //får int med tid i timer beregnet mellom start og sluttdato
        public int GetAverageQueuingTimeInHours(); //får int med tid i timer skip må vente i kø. 
        public int GetAverageQueuingTimeInHours(DateTime startDate, DateTime endDate); //får int med tid i timer skip må vente i kø. 
        public int GetContainersMoved(); //Antall containere flyttet i løpet av simuleringen
        public int GetContainersMoved(DateTime startDate, DateTime endDate);
        public int GetNumberOfDockings(); //Antall skip docket i løpet av simuleringen 
        public int GetNumDockings(DateTime startDate); //Antall skip docket i løpet av simuleringen 
        */
    }
}
