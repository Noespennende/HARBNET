using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarbFramework
{
    internal class Simulation
    {
        //StartTime variabler 
        //currentTime
        //StartOfDay
        //EndTime variabler
        //opprett Harbour objekt (opprett kontainere til havn)
        //opprett båter (opprett containere ombord)
        //ArrayList Logs = new ArrayList()

        //legg til en initiell logfil til historie som logger starttilstanden til simuleringen.

        //while(Ikke sluttdato){
            //En runde i while løkken er en time
            //Undocke båter som er ferdig med lasting  (Lag en foreach som går igjennom alle båter i havna og ser om de er ferdig med jobben. hvis de er det undock båten og sett nextStepCheck til True)
            //Sjekk hvor mange havner av de forskjellige størelsene er ledig (For løkke som går igjennom ledige havner og teller opp antallet av de forskjellige størelsene som er ledig)
            //Docke båter fra HarbourQueueInn (For løkke som looper like mange ganger som antal skip av de forskjellige størrelsene som skal dokkes og bruker harbor funksjonen for å docke skipene hvis de er det undock båten og sett nextStepCheck til True)
            //Laste av containere fra båten / Laste containere til båter. (For løkke som går gjennom alle skipene til kai og finner ut om de skal laste av eller på containere og laster av/på så mange containere som skipet har mulighet til basert på berthing time)
            //Setter alle nextStepCheck til false hos alle objekter (For løkke som går gjennom alle skip i simuleringen og setter next step check til false)
            //oppdaterer current time med +1 time}
            //Current time == Start of day + 24 hours => Lag log fil
    }
}
