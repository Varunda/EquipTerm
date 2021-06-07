using DaybreakGames.Census;
using DaybreakGames.Census.Exceptions;
using DaybreakGames.Census.Operators;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeaponStats.Models.Census;

namespace WeaponStats.Services.Census.Implementations {

    public class CharacterCollection : ICharacterCollection {

        private readonly ILogger<CharacterCollection> _Logger;

        private readonly ICensusQueryFactory _Census;

        public CharacterCollection(ILogger<CharacterCollection> logger,
                ICensusQueryFactory factory) {

            _Logger = logger;
            _Census = factory;
        }

        public async Task<Ps2Character?> GetByNameAsync(string name) {
            Ps2Character? c = await _GetCharacterFromCensusByName(name, true);

            return c;
        }

        public async Task<Ps2Character?> GetByIDAsync(string ID) {
            Ps2Character? c = await _GetCharacterFromCensus(ID, true);

            return c;
        }

        private async Task<Ps2Character?> _GetCharacterFromCensus(string ID, bool retry) {
            CensusQuery query = _Census.Create("character");

            query.Where("character_id").Equals(ID);
            query.AddResolve("outfit", "online_status");
            query.ShowFields("character_id", "name", "faction_id", "outfit", "online_status");

            try {
                JToken result = await query.GetAsync();

                Ps2Character? player = _ParseCharacter(result);

                return player;
            } catch (CensusConnectionException ex) {
                if (retry == true) {
                    _Logger.LogWarning("Retrying {Char} from API", ID);
                    return await _GetCharacterFromCensus(ID, false); 
                } else {
                    _Logger.LogError(ex, "Failed to get {0} from API", ID);
                    throw ex;
                }
            }
        }

        private async Task<Ps2Character?> _GetCharacterFromCensusByName(string name, bool retry) {
            CensusQuery query = _Census.Create("character");

            query.Where("name.first_lower").Equals(name.ToLower());
            query.AddResolve("outfit");

            try {
                JToken result = await query.GetAsync();

                Ps2Character? player = _ParseCharacter(result);

                return player;
            } catch (CensusConnectionException ex) {
                if (retry == true) {
                    _Logger.LogWarning("Retrying {Char} from API", name);
                    return await _GetCharacterFromCensusByName(name, false); 
                } else {
                    _Logger.LogError(ex, "Failed to get {0} from API", name);
                    throw ex;
                }
            }
        }

        private Ps2Character? _ParseCharacter(JToken result) {
            if (result == null) {
                return null;
            }

            Ps2Character player = new Ps2Character {
                ID = result.Value<string?>("character_id") ?? "0",
                FactionID = result.Value<int?>("faction_id") ?? -1
            };

            JToken? nameToken = result.SelectToken("name");
            if (nameToken == null) {
                _Logger.LogWarning($"Missing name field from {result}");
            } else {
                player.Name = nameToken.Value<string?>("first") ?? "BAD NAME";
            }

            JToken? outfitToken = result.SelectToken("outfit");
            if (outfitToken != null) {
                player.OutfitID = outfitToken.Value<string?>("outfit_id");
                player.OutfitTag = outfitToken.Value<string?>("alias");
                player.OutfitName = outfitToken.Value<string?>("name");
            }

            return player;
        }

    }
}
