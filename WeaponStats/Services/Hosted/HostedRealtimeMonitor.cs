using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WeaponStats.Services.Realtime;

namespace WeaponStats.Services.Hosted {

    public class HostedRealtimeMonitor : IHostedService {

        private readonly ILogger<HostedRealtimeMonitor> _Logger;
        private readonly IRealtimeMonitor _Monitor;

        public HostedRealtimeMonitor(ILogger<HostedRealtimeMonitor> logger,
            IRealtimeMonitor monitor) {

            _Logger = logger;

            _Monitor = monitor ?? throw new ArgumentNullException(nameof(monitor));
        }

        public async Task StartAsync(CancellationToken cancellationToken) {
            try {
                await _Monitor.OnStart(cancellationToken);
            } catch (Exception ex) {
                _Logger.LogError(ex, "Failed to start realtime monitor");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) {
            return _Monitor.OnShutdown(cancellationToken);
        }
    }
}
