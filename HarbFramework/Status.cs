using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarbFramework
{
    public enum Status
    {
        None = 0,

        Anchoring,
        Anchored,

        DockingToLoadingDock,
        DockingToShipDock,

        DockedToLoadingDock,
        DockedToShipDock,

        Unloading,
        UnloadingDone,
        Loading,
        LoadingDone,

        Undocking,
        Transit,

        InStorage

        
    }
}
