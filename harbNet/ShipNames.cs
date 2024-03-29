﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Client.HarborName
{

    internal class ShipNames
    {
        //Denne klassen holder navn på skip generert av ChatGDP. Den er ikke en del av verken APIet eller rammeverket og brukes bare for å gjøre det
        //lettere å opprette mange skip med unike navn som kan sendes inn i til simuleringen ved hjelp av APIet.
        //Prompten for å opprette disse navnene er:
        // "Give me a list of 200 unique ship names of famous ships from fiction or real life in the form of string values in C#. Each value in the list must be unique and not a duplicate of another value in the list."
        // Vi har også lagt til noen navn til listen som vi har funnet på selv.

        internal string[] Names { get; } = {
            "SeaLion",
            "SeaBear",
            "Flying Eagle",
            "Boaty McBoatFace",
            "SS Titanic",
            "SS OceanWave",
            "Bullrush",
            "The morrow",
            "SS Hamilton",
            "Londonferry",
            "Kittleham",
            "SS Grog",
            "SS Klanker",
            "KV Staalbass",
            "SS Infanta",
            "Messanger",
            "The Blue Whale",
            "Bebop",
            "Den sorte dame",
            "Millennium Falcon",
            "USS Enterprise",
            "Serenity",
            "Normandy",
            "Death Star",
            "Battlestar Galactica",
            "TARDIS",
            "Executor",
            "Nostromo",
            "Prometheus",
            "Heart of Gold",
            "Rocinante",
            "Voyager",
            "Event Horizon",
            "Slave I",
            "Discovery One",
            "Borg Cube",
            "Galactica",
            "Titanic",
            "Black Pearl",
            "Nautilus",
            "Flying Dutchman",
            "Red Dwarf",
            "Swordfish II",
            "SS Minnow",
            "SS Poseidon",
            "SS Venture",
            "SS Lazarus",
            "SS Bountiful",
            "SS Star Dreamer",
            "SS Horizon Explorer",
            "SS Hyperion",
            "SS Skybreaker",
            "SS Moonstrider",
            "SS Starlight",
            "SS Aurora",
            "SS Celestial",
            "SS Stargazer",
            "SS Infinity",
            "SS Phoenix",
            "SS Odyssey",
            "SS Firefly",
            "SS Titan",
            "SS Nightfall",
            "SS Falcon",
            "SS Stardust",
            "SS Solaris",
            "SS Thunderbird",
            "SS Dragonfly",
            "SS Starstrider",
            "SS Zenith",
            "SS Galactica",
            "SS Starburst",
            "SS Vortex",
            "SS Horizon's Edge",
            "SS Nebula",
            "SS Artemis",
            "SS Centauri",
            "SS Eclipse",
            "SS Orion",
            "SS Pulsar",
            "SS Triton",
            "SS Valkyrie",
            "SS Black Hole",
            "SS Odyssey",
            "SS Enigma",
            "SS Spectre",
            "SS Prometheus",
            "SS Andromeda",
            "SS Horizon's Dawn",
            "SS Aquarius",
            "SS Nebula Star",
            "SS Celestial Voyager",
            "SS Aurora Borealis",
            "SS Infinity Star",
            "SS Lunar Shadow",
            "SS Orion's Belt",
            "SS Solar Flare",
            "SS Thunderstrike",
            "SS Galaxy Explorer",
            "SS Stellar Compass",
            "SS Aurora Explorer",
            "SS Solar Eclipse",
            "SS Starlight Explorer",
            "SS Horizon's Quest",
            "SS Nebula Explorer",
            "SS Celestial Explorer",
            "SS Solar Voyager",
            "SS Star Seeker",
            "SS Eclipse Voyager",
            "SS Nova Explorer",
            "SS Pegasus",
            "SS Horizon's Venture",
            "SS Stellar Explorer",
            "SS Aurora Voyager",
            "SS Solar Voyager",
            "SS Celestial Voyager",
            "SS Horizon's Voyage",
            "SS Nebula Voyager",
            "SS Star Voyager",
            "SS Eclipse Expedition",
            "SS Nova Voyager",
            "SS Horizon's Expedition",
            "SS Stellar Voyager",
            "SS Aurora Expedition",
            "SS Solar Expedition",
            "SS Celestial Expedition",
            "Blackbeard's Revenge",
            "Bismarck",
            "Cutty Sark",
            "Mayflower",
            "Endeavour",
            "Santa Maria",
            "Golden Hind",
            "HMS Victory",
            "Lusitania",
            "USS Constitution",
            "Flying Cloud",
            "Santa Maria",
            "Endurance",
            "Queen Anne's Revenge",
            "HMS Beagle",
            "USS Monitor",
            "USS Arizona",
            "Mary Celeste",
            "USS Maine",
            "Nina",
            "Pinta",
            "Santa Maria",
            "HMS Bounty",
            "Calypso",
            "SS Great Eastern",
            "USS Indianapolis",
            "Bounty",
            "USS Missouri",
            "USS Lexington",
            "HMS Belfast",
            "USS Hornet",
            "HMS Terror",
            "HMS Discovery",
            "HMS Erebus",
            "HMS Resolution",
            "USS Hornet",
            "USS Intrepid",
            "HMS Warrior",
            "HMS Victory",
            "HMS Surprise",
            "USS Constitution",
            "USS Constellation",
            "USS Kearsarge",
            "HMS Thunder Child",
            "USS Monitor",
            "CSS Virginia",
            "USS Cairo",
            "USS Merrimac",
            "USS Monitor",
            "CSS Alabama",
            "USS Kearsarge",
            "HMS Endurance",
            "USS Chesapeake",
            "HMS Leopard",
            "USS Chesapeake",
            "HMS Leopard",
            "USS Philadelphia",
            "HMS Guerriere",
            "USS Chesapeake",
            "HMS Shannon",
            "USS United States",
            "HMS Macedonian",
            "USS Constitution",
            "CSS Virginia",
            "USS Monitor",
            "CSS Alabama",
            "USS Kearsarge",
            "CSS Alabama",
            "USS Kearsarge",
            "USS Virginia",
            "USS Monitor",
            "CSS Virginia",
            "USS Monitor",
            "USS Merrimack",
            "CSS Virginia",
            "USS Kearsarge",
            "CSS Alabama",
            "USS Chesapeake",
            "HMS Shannon",
            "USS Chesapeake",
            "HMS Leopard",
            "USS Chesapeake",
            "HMS Leopard",
            "USS Philadelphia",
            "HMS Guerriere",
            "USS Chesapeake",
            "HMS Shannon",
            "USS United States",
            "HMS Macedonian",
            "USS Constitution",
            "USS Essex",
            "HMS Phoebe",
            "HMS Phoebe"
        };

    }
}
