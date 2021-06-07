using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeaponStats.Models.Realtime;

namespace WeaponStats.Services.Realtime {

    /// <summary>
    /// Broadcast event
    /// </summary>
    public interface IRealtimeEvents {

        event EventHandler<PlayerLogoutEvent> OnLogout;
        void EmitLogout(PlayerLogoutEvent ev);

    }
}
