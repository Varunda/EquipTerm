using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeaponStats.Services.Census;
using WeaponStats.Models;
using WeaponStats.Services.Db;
using WeaponStats.Services.Repositories;

namespace WeaponStats.Services.Repositories.Implementations {

    public class WeaponStatRepository : IWeaponStatRepository {

        private readonly ILogger<WeaponStatRepository> _Logger;
        private readonly IMemoryCache _Cache;
        private readonly IWeaponStatDbStore _Db;
        private readonly ICharacterWeaponStatsCollection _Census;

        private const string _CacheKeyChar = "WeaponStat.Character.{0}"; // {0} => CharID
        private const string _CacheKeyItem = "WeaponStat.Item.{0}"; // {0} => ItemID

        public WeaponStatRepository(ILogger<WeaponStatRepository> logger, IMemoryCache cache,
                IWeaponStatDbStore db, ICharacterWeaponStatsCollection census) {

            _Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _Cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _Db = db ?? throw new ArgumentNullException(nameof(db));
            _Census = census ?? throw new ArgumentNullException(nameof(census));
        }

        public async Task<List<WeaponStatEntry>> GetByCharacterID(string charID) {
            string key = string.Format(_CacheKeyChar, charID);
            if (_Cache.TryGetValue(key, out List<WeaponStatEntry> entries) == false) {
                _Logger.LogInformation($"Getting {key} from Db/Census");
                entries = await _Db.GetByCharacterID(charID);

                bool repull = false;

                if (entries.Count == 0) {
                    repull = true;
                }

                if (repull == false) {
                    foreach (WeaponStatEntry entry in entries) {
                        if (await HasExpired(entry) == true) {
                            repull = true;
                            break;
                        }
                    }
                }

                if (repull == true) {
                    entries = await _Census.GetByCharacterIDAsync(charID);

                    foreach (WeaponStatEntry entry in entries) {
                        try {
                            await Upsert(entry);
                        } catch (Exception ex) {
                            _Logger.LogError(ex, $"Failed to insert {entry.CharacterID} {entry.WeaponID}");
                        }
                    }
                }

                _Cache.Set(key, entries, new MemoryCacheEntryOptions() {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                });
            }

            return entries;
        }

        public async Task<List<WeaponStatEntry>> GetByItemID(long itemID) {
            string key = string.Format(_CacheKeyItem, itemID);
            if (_Cache.TryGetValue(key, out List<WeaponStatEntry> entries) == false) {
                _Logger.LogInformation($"Getting {key} from Db/Census");

                entries = await _Db.GetByItemID(itemID);

                _Cache.Set(key, entries, new MemoryCacheEntryOptions() {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                });
            }

            return entries;
        }

        public async Task Upsert(WeaponStatEntry entry) {
            await _Db.Upsert(entry);
        }

        private Task<bool> HasExpired(WeaponStatEntry entry) {
            return Task.FromResult(true);
        }

    }
}
