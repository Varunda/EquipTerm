using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WeaponStats.Services.Environment;
using WeaponStats.Services.Realtime;

namespace WeaponStats.Services.Hosted {

    public class BackgroundEventProcessHostedService : BackgroundService {

        private readonly ILogger<BackgroundEventProcessHostedService> _Logger;
        private readonly IBackgroundTaskQueue _Queue;
        private readonly IRealtimeEventHandler _EventHandler;

        public BackgroundEventProcessHostedService(ILogger<BackgroundEventProcessHostedService> logger,
            IBackgroundTaskQueue queue, IRealtimeEventHandler eventHandler) {

            _Logger = logger;
            _Queue = queue;
            _EventHandler = eventHandler;
        }

        protected override async Task ExecuteAsync(CancellationToken cancel) {
            while (cancel.IsCancellationRequested == false) {
                JToken token = await _Queue.DequeueAsync(cancel);
                try {
                    _EventHandler.Process(token);
                } catch (Exception ex) {
                    _Logger.LogError(ex, "Failed to process realtime event {token}", token);
                }
            }
        }

    }
}
