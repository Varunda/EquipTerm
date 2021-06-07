using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeaponStats.Models.Realtime;

namespace WeaponStats.Services.Realtime.Implementations {

    public class RealtimeEvents : IRealtimeEvents {

        private readonly ILogger<RealtimeEvents> _Logger;

        public RealtimeEvents(ILogger<RealtimeEvents> logger) {
            _Logger = logger;
        }

        public event EventHandler<PlayerLogoutEvent>? OnLogout;
        public void EmitLogout(PlayerLogoutEvent ev) {
            OnLogout?.Invoke(this, ev);
        }

    }
}
