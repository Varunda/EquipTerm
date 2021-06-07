using DaybreakGames.Census.Stream;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WeaponStats.Models.Realtime;
using WeaponStats.Services.Environment;
using Websocket.Client;

namespace WeaponStats.Services.Realtime.Implementations {

    public class RealtimeMonitor : IRealtimeMonitor {

        private readonly ILogger<RealtimeMonitor> _Logger;
        private readonly ICensusStreamClient _Stream;
        private readonly IBackgroundTaskQueue _Queue;

        private List<RealtimeSubscription> _Subscriptions = new List<RealtimeSubscription>();

        public RealtimeMonitor(ILogger<RealtimeMonitor> logger,
            ICensusStreamClient stream,
            IBackgroundTaskQueue queue) {

            _Logger = logger;

            _Stream = stream ?? throw new ArgumentNullException(nameof(stream));
            _Queue = queue ?? throw new ArgumentNullException(nameof(queue));

            _Stream.OnConnect(_OnConnectAsync)
                .OnMessage(_OnMessageAsync)
                .OnDisconnect(_OnDisconnectAsync);
        }

        public void Subscribe(RealtimeSubscription sub) {
            CensusStreamSubscription subscription = new CensusStreamSubscription() {
                Characters = sub.Characters,
                EventNames = sub.Events,
                Worlds = sub.Worlds
            };

            _Logger.LogInformation($"New subscription: {subscription}");

            _Stream.Subscribe(subscription);
        }

        private Task _OnMessageAsync(string msg) {
            if (msg == null) {
                return Task.CompletedTask;
            }

            try {
                JToken token = JToken.Parse(msg);
                _Queue.Queue(token);
            } catch (Exception ex) {
                _Logger.LogError(ex, "Failed to parse message: {json}", msg);
            }

            return Task.CompletedTask;
        }

        public Task OnStart(CancellationToken cancel) {
            return _Stream.ConnectAsync();
        }

        public Task OnShutdown(CancellationToken cancel) {
            return _Stream.DisconnectAsync();
        }

        private Task _OnConnectAsync(ReconnectionType type) {
            if (type == ReconnectionType.Initial) {
                _Logger.LogInformation($"Stream connected");
            } else {
                _Logger.LogInformation($"{type}, reconnecting");
                _Resubscribe();
            }

            return Task.CompletedTask;
        }

        private Task _OnDisconnectAsync(DisconnectionInfo info) {
            _Logger.LogInformation($"Stream disconnected: {info.Type}");
            return Task.CompletedTask;
        }

        public void Dispose() {
            _Stream?.Dispose();
        }

        private void _Resubscribe() {
            foreach (RealtimeSubscription sub in _Subscriptions) {
                Subscribe(sub);
            }
        }

    }
}
