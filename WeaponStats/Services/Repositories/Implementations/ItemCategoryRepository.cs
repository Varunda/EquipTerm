using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeaponStats.Models.Census;
using WeaponStats.Services.Census;
using WeaponStats.Services.Db;

namespace WeaponStats.Services.Repositories.Implementations {

    public class ItemCategoryRepository : IItemCategoryRepository {

        private readonly ILogger<ItemCategoryRepository> _Logger;
        private readonly IMemoryCache _Cache;

        private readonly IItemCategoryDbStore _Db;
        private readonly IItemCategoryCollection _Census;

        private const string _CacheKeyAll = "ItemCategory.All";

        public ItemCategoryRepository(ILogger<ItemCategoryRepository> logger, IMemoryCache cache,
            IItemCategoryDbStore db, IItemCategoryCollection coll) {

            _Logger = logger;
            _Cache = cache;

            _Db = db ?? throw new ArgumentNullException(nameof(db));
            _Census = coll ?? throw new ArgumentNullException(nameof(coll));
        }

        public async Task<List<Ps2ItemCategory>> GetAll() {
            if (_Cache.TryGetValue(_CacheKeyAll, out List<Ps2ItemCategory> entries) == false) {
                _Logger.LogDebug($"Getting Ps2ItemCategory list from DB");
                entries = await _Db.GetAll();

                if (entries.Count == 0) {
                    _Logger.LogDebug($"No entries from DB, getting list from Census");
                    entries = await _Census.GetAll();

                    foreach (Ps2ItemCategory category in entries) {
                        await _Db.Upsert(category);
                    }
                }

                _Cache.Set(_CacheKeyAll, entries, new MemoryCacheEntryOptions() {
                    SlidingExpiration = TimeSpan.FromHours(4)
                });
            }

            return entries;
        }

        public async Task<Ps2ItemCategory?> GetByID(int ID) {
            return (await GetAll())
                .FirstOrDefault(iter => iter.ID == ID);
        }

        public Task Upsert(Ps2ItemCategory parameters) {
            return _Db.Upsert(parameters);
        }

    }
}
