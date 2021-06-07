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

    public class ItemRepository : IItemRepository {

        private readonly ILogger<ItemRepository> _Logger;
        private readonly IMemoryCache _Cache;

        private readonly IItemDbStore _Db;
        private readonly IItemCollection _Census;

        private const string _CacheKey = "Item.All";

        public ItemRepository(ILogger<ItemRepository> logger, IMemoryCache cache,
            IItemDbStore db, IItemCollection census) {

            _Logger = logger;
            _Cache = cache;

            _Db = db ?? throw new ArgumentNullException(nameof(db));
            _Census = census ?? throw new ArgumentNullException(nameof(census));
        }

        public async Task<List<Ps2Item>> GetAll() {
            if (_Cache.TryGetValue(_CacheKey, out List<Ps2Item> items) == false) {
                _Logger.LogDebug($"Getting item list from DB");
                items = await _Db.GetAll();

                if (items.Count == 0) {
                    _Logger.LogDebug($"Getting item list from Census, DB was empty");
                    items = await _Census.GetAll();

                    foreach (Ps2Item item in items) {
                        await _Db.Upsert(item);
                    }
                }

                _Cache.Set(_CacheKey, items, new MemoryCacheEntryOptions() {
                    SlidingExpiration = TimeSpan.FromMinutes(10)
                });
            }

            return items;
        }

        public async Task<List<Ps2Item>> GetByTypeID(int typeID) {
            return (await GetAll())
                .Where(iter => iter.TypeID == typeID).ToList();
        }

        public async Task<List<Ps2Item>> GetByCategoryID(int categoryID) {
            return (await GetAll())
                .Where(iter => iter.CategoryID == categoryID).ToList();
        }

        public async Task<Ps2Item?> GetByID(int itemID) {
            return (await GetAll())
                .FirstOrDefault(iter => iter.ID == itemID);
        }

        public Task Upsert(Ps2Item item) {
            return _Db.Upsert(item);
        }

    }
}
