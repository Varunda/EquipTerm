using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WeaponStats.Models;
using WeaponStats.Models.Census;
using WeaponStats.Models.Realtime;
using WeaponStats.Services.Census;
using WeaponStats.Services.Db;
using WeaponStats.Services.Environment;
using WeaponStats.Services.Realtime;
using WeaponStats.Services.Repositories;

namespace WeaponStats.Services.Hosted {

    /// <summary>
    /// Hosted service that performs character updates in the background
    /// </summary>
    public class BackgroundCharacterUpdateHostedService : BackgroundService {

        private readonly ILogger<BackgroundCharacterUpdateHostedService> _Logger;

        private readonly IBackgroundCharacterQueue _Queue;
        private readonly ICharacterCollection _CharacterCollection;
        private readonly ICharacterRepository _CharacterRepository;
        private readonly ICharacterWeaponStatsCollection _WeaponStatCollection;
        private readonly IWeaponStatDbStore _WeaponStatDb;

        private readonly IRealtimeMonitor _RealtimeMonitor;
        private readonly IRealtimeEvents _RealtimeEvents;

        // How many seconds to wait in between each queue update
        private const int _IntervalDelay = 10;

        public BackgroundCharacterUpdateHostedService(ILogger<BackgroundCharacterUpdateHostedService> logger,
            IBackgroundCharacterQueue queue, ICharacterCollection charColl,
            ICharacterWeaponStatsCollection weaponColl, IRealtimeMonitor monitor,
            IRealtimeEvents realtimeEvents, ICharacterRepository charRepo,
            IWeaponStatDbStore weaponStatDb) {

            _Logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _Queue = queue ?? throw new ArgumentNullException(nameof(queue));

            _CharacterCollection = charColl ?? throw new ArgumentNullException(nameof(charColl));
            _CharacterRepository = charRepo ?? throw new ArgumentNullException(nameof(charRepo));
            _WeaponStatCollection = weaponColl ?? throw new ArgumentNullException(nameof(weaponColl));
            _WeaponStatDb = weaponStatDb ?? throw new ArgumentNullException(nameof(weaponStatDb));

            _RealtimeMonitor = monitor ?? throw new ArgumentNullException(nameof(monitor));
            _RealtimeEvents = realtimeEvents ?? throw new ArgumentNullException(nameof(realtimeEvents));
        }

        public override async Task StartAsync(CancellationToken cancel) {
            await base.StartAsync(cancel);

            _RealtimeEvents.OnLogout += OnPlayerLogout;

            try {
                _RealtimeMonitor.Subscribe(new RealtimeSubscription() {
                    Events = { "PlayerLogout" },
                    Characters = { "all" },
                    Worlds = { "all" }
                });
            } catch (Exception ex) {
                _Logger.LogError(ex, "Failed to subscribe to logout events");
            }

            _Logger.LogInformation($"Started service: {nameof(BackgroundCharacterUpdateHostedService)}");
        }

        protected override async Task ExecuteAsync(CancellationToken cancel) {
            _Logger.LogInformation($"{nameof(BackgroundCharacterUpdateHostedService)} started");

            while (cancel.IsCancellationRequested == false) {

                int amount = _Queue.Length();
                _Logger.LogInformation($"Performing updates on {amount} characters");

                int updateCount = 0;
                List<string> updated = new List<string>();

                Stopwatch timer = Stopwatch.StartNew();

                while (_Queue.IsEmpty() == false) {
                    CharacterUpdateParameters param = await _Queue.Dequeue(cancel);
                    ++updateCount;

                    try {
                        //_Logger.LogTrace($"Performing background update on {param.CharacterID}");

                        Ps2Character? character = await _CharacterRepository.GetByID(param.CharacterID);
                        if (character == null) {
                            _Logger.LogWarning($"Failed to find character {param.CharacterID}");
                            continue;
                        }

                        updated.Add(character.Name);

                        List<WeaponStatEntry> entries = await _WeaponStatCollection.GetByCharacterIDAsync(character.ID);
                        foreach (WeaponStatEntry entry in entries) {
                            await _WeaponStatDb.Upsert(entry);
                        }
                    } catch (Exception ex) {
                        _Logger.LogError(ex, "Failed to process background character update on {charID}", param.CharacterID);
                    }

                    if (updateCount > 99) {
                        _Logger.LogWarning($"Stopping at 100 updates per iteration");
                        break;
                    }
                }

                timer.Stop();

                _Logger.LogInformation($"Took {timer.ElapsedMilliseconds}ms to update {updateCount} characters: {string.Join(", ", updated)}");

                await Task.Delay(1000 * _IntervalDelay, cancel);
            }
        }

        private void OnPlayerLogout(object? sender, PlayerLogoutEvent ev) {
            _Queue.Queue(new CharacterUpdateParameters() {
                CharacterID = ev.CharacterID,
                LogoutTime = DateTime.UtcNow + TimeSpan.FromMinutes(5)
            });
        }

        public override async Task StopAsync(CancellationToken cancellationToken) {
            await base.StopAsync(cancellationToken);

            _RealtimeEvents.OnLogout -= OnPlayerLogout;

            _Logger.LogInformation($"Stopped hosted service: {nameof(BackgroundCharacterUpdateHostedService)}");
        }

    }
}
