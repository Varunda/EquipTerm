using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeaponStats.Code.ExtensionMethods;
using WeaponStats.Models.Realtime;

namespace WeaponStats.Services.Realtime.Implementations {

    public class RealtimeEventHandler : IRealtimeEventHandler {

        private readonly ILogger<RealtimeEventHandler> _Logger;

        private readonly IRealtimeEvents _RealtimeEvents;

        /// <summary>
        /// Psuedo-queue to prevent duplicate events from being processed twice
        /// </summary>
        private readonly List<JToken> _Recent = new List<JToken>();

        public RealtimeEventHandler(ILogger<RealtimeEventHandler> logger,
            IRealtimeEvents events) {

            _Logger = logger;

            _RealtimeEvents = events ?? throw new ArgumentNullException(nameof(events));
        }

        public void Process(JToken token) {
            if (_Recent.Contains(token)) {
                _Logger.LogInformation($"Skipping duplicate event {token}");
                return;
            }

            // Add to the recent list, removing the oldest entry if the queue is at max size
            _Recent.Add(token);
            if (_Recent.Count > 10) {
                _Recent.RemoveAt(0);
            }

            string? type = token.Value<string?>("type");

            if (type == "serviceMessage") {
                JToken? payloadToken = token.SelectToken("payload");
                if (payloadToken == null) {
                    _Logger.LogWarning($"Missing 'payload' from {token}");
                    return;
                }

                string? eventName = payloadToken.Value<string?>("event_name");

                if (eventName == "PlayerLogin") {
                    _Logger.LogInformation($"login");
                } else if (eventName == "PlayerLogout") {
                    ProcessLogoutEvent(payloadToken);
                } else if (eventName == null) {
                    _Logger.LogWarning($"Missing 'event_name' from {token}");
                } else {
                    _Logger.LogWarning($"Unhandled 'event_name' {eventName} from {token}");
                }
            } else if (type == "heartbeat") {

            }
        }

        private void ProcessLogoutEvent(JToken token) {
            PlayerLogoutEvent ev = new PlayerLogoutEvent() {
                CharacterID = token.GetString("character_id", "0"),
                Timestamp = token.CensusTimestamp("timestamp")
            };

            _RealtimeEvents.EmitLogout(ev);
        }

    }
}
