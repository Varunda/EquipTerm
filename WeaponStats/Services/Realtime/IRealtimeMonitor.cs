using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WeaponStats.Models.Realtime;

namespace WeaponStats.Services.Realtime {

    /// <summary>
    /// Realtime monitor, manages the realtime handler, and makes sure the realtime events stay connected
    /// </summary>
    public interface IRealtimeMonitor {

        Task OnStart(CancellationToken cancel);

        Task OnShutdown(CancellationToken cancel);

        /// <summary>
        /// Subscribe to events on the realtime web sockets
        /// </summary>
        /// <param name="sub">Subscription to listen for</param>
        void Subscribe(RealtimeSubscription sub);

    }
}
