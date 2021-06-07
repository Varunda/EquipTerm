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

    public class CharacterRepository : ICharacterRepository {

        private readonly ILogger<CharacterRepository> _Logger;
        private readonly IMemoryCache _Cache;

        private readonly ICharacterDbStore _Db;
        private readonly ICharacterCollection _Census;

        private const string _CacheKeyID = "Character.ID.{0}"; // {0} => char ID
        private const string _CacheKeyName = "Character.Name.{0}"; // {0} => char ID

        public CharacterRepository(ILogger<CharacterRepository> logger, IMemoryCache cache,
                ICharacterDbStore db, ICharacterCollection census) {

            _Logger = logger;
            _Cache = cache;

            _Db = db;
            _Census = census;
        }

        public async Task<Ps2Character?> GetByID(string charID) {
            string key = string.Format(_CacheKeyID, charID);

            if (_Cache.TryGetValue(key, out Ps2Character? character) == false) {
                character = await _Db.GetByID(charID);

                // Only update the character if it's expired
                if (character == null || await HasExpired(character) == true) {
                    character = await _Census.GetByIDAsync(charID);

                    if (character != null) {
                        await _Db.Upsert(character);
                    }
                }

                _Cache.Set(key, character, new MemoryCacheEntryOptions() {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20)
                });

                string nameKey = string.Format(_CacheKeyName, charID);
                _Cache.Set(nameKey, character?.Name, new MemoryCacheEntryOptions() {
                    SlidingExpiration = TimeSpan.FromHours(2)
                });
            }

            return character;
        }

        public async Task<Ps2Character?> GetByName(string name) {
            Ps2Character? character;

            string key = string.Format(_CacheKeyName, name);

            if (_Cache.TryGetValue(key, out string charID) == false) {
                character = await _Db.GetByName(name);

                if (character == null || await HasExpired(character) == true) {
                    character = await _Census.GetByNameAsync(name);
                }

                _Cache.Set(key, character?.Name, new MemoryCacheEntryOptions() {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20)
                });

                if (character != null) {
                    string idKey = string.Format(_CacheKeyID, character.ID);
                    _Cache.Set(idKey, character, new MemoryCacheEntryOptions() {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20)
                    });
                }
            } else {
                character = await GetByID(charID);
            }

            return character;
        }

        public Task Upsert(Ps2Character character) {
            throw new NotImplementedException();
        }
        
        private Task<bool> HasExpired(Ps2Character character) {
            return Task.FromResult(true);
        }

    }
}
