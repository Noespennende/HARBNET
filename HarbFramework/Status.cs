﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarbFramework
{
    public enum Status
    {
        None,
        DockingToLoadingDock,
        DockingToShipDock,
        Undocking,
        Loading,
        Unloading,
        Transit,
        InStorage,
        Queuing,

        UnloadingDone,
        LoadingDone
    }
}
