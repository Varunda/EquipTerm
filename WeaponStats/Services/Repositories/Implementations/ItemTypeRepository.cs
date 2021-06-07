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

    public class ItemTypeRepository : IItemTypeRepository {

        private readonly ILogger<ItemTypeRepository> _Logger;
        private readonly IMemoryCache _Cache;

        private readonly IItemTypeDbStore _Db;
        private readonly IItemTypeCollection _Census;

        private const string _CacheKeyAll = "ItemType.All";

        public ItemTypeRepository(ILogger<ItemTypeRepository> logger, IMemoryCache cache,
            IItemTypeDbStore db, IItemTypeCollection census) {

            _Logger = logger;
            _Cache = cache;

            _Db = db ?? throw new ArgumentNullException(nameof(db));
            _Census = census ?? throw new ArgumentNullException(nameof(census));
        }

        public async Task<List<Ps2ItemType>> GetAll() {
            if (_Cache.TryGetValue(_CacheKeyAll, out List<Ps2ItemType> types) == false) {
                _Logger.LogDebug($"Getting types from DB");
                types = await _Db.GetAll();

                if (types.Count == 0) {
                    _Logger.LogDebug($"Getting types from Census, DB was empty");
                    types = await _Census.GetAll();

                    foreach (Ps2ItemType type in types) {
                        await _Db.Upsert(type);
                    }
                }

                _Cache.Set(_CacheKeyAll, types, new MemoryCacheEntryOptions() {
                    SlidingExpiration = TimeSpan.FromHours(4)
                });
            }

            return types;
        }

        public async Task<Ps2ItemType?> GetByID(int ID) {
            return (await GetAll())
                .FirstOrDefault(iter => iter.ID == ID);
        }

        public Task Upsert(Ps2ItemType parameters) {
            return _Db.Upsert(parameters);
        }

    }
}
