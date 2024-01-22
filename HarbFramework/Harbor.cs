using HarbFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace harbNet
{
    internal class Harbor
    {
        Hashtable maxDocks = new Hashtable(); // størelse : antall
        Hashtable freeDocks = new Hashtable(); // størelse : antall ledige
        Hashtable dockedShips = new Hashtable(); // uid Ship : Dock størelse
        ArrayList<Ship> portQueInn;
        Queue portQueueOut = new Queue();
        Hashtable maxContainerSpace = new Hashtable(); // størelse : antall
        Hashtable freeContainerSpace = new Hashtable(); // størelse : antall ledige
        Hashtable storedContainers = new Hashtable(); // uid Container : Container space størelse


        private Guid dockShip (Guid shipId) { } //returnerer Guid til docken skipet docker til
        private Guid unDockShip (Guid shipId) { } //returnerer Guid til docken skipet docket fra
        private Guid getFreePortID(ShipSize shipSize) { } //returnerer Guid til den ledige porten
        private int NumberOfFreeDocingSpaces(ShipSize shipSize) { } //Returnerer antall ledige plasser av den gitte typen
        private int NumberOfFreeContainerSpaces(ContainerSize containerSize) { } //returnerer antallet ledige container plasser av den gitte typen.
        private int getNumberOfOccupiedContainerSpaces(ContainerSize containerSize) { } //returnerer antallet okuperte plasser av den gitte typen
        private Guid getFreeContainerSpaceID(ContainerSize containerSize) { } //returnerer en Guid til en ledig plass av den gitte typen
        private ArrayList<Ship> dockedShips() { }
    }
}
