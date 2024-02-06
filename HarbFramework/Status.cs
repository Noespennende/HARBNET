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
        DockingToLoadingDock,
        DockingToShipDock,
        Undocking,
        Loading,
        Unloading,
        Transit,
        InStorage,
        Anchoring,
        UnloadingDone,
        LoadingDone
    }
}
