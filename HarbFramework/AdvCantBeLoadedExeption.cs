﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe8.HarbNet
{
    internal class AdvCantBeLoadedExeption : Exception
    {
        public AdvCantBeLoadedExeption() { }

        public AdvCantBeLoadedExeption(string message) : base(message)
        {
        }
        public AdvCantBeLoadedExeption(String message, Exception innerException) : base(message, innerException)
        {
        }
    }
}