using DaybreakGames.Census;
using DaybreakGames.Census.Operators;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeaponStats.Models.Census;

namespace WeaponStats.Services.Census.Implementations {

    public class OutfitCollection : IOutfitCollection {

        private readonly ILogger<OutfitCollection> _Logger;

        private readonly ICensusQueryFactory _Census;

        public OutfitCollection(ILogger<OutfitCollection> logger,
                ICensusQueryFactory census) {

            _Logger = logger;
            _Census = census;
        }

        public async Task<Outfit?> GetByIDAsync(string ID) {
            Outfit? outfit = null;

            CensusQuery query = _Census.Create("outfit");
            query.Where("outfit_id").Equals(ID);
            query.SetLimit(1);
            query.SetLanguage(CensusLanguage.English);
            query.AddResolve("member_online_status");

            try {
                Uri uri = query.GetUri();
                JToken result = await query.GetAsync();

                outfit = _ParseOutfit(result);
            } catch (Exception ex) {
                _Logger.LogError(ex, "Failed to get outfit by ID {ID}", ID);
            }

            return outfit;
        }

        public Task<Outfit?> GetByTagAsync(string tag) {
            throw new NotImplementedException();
        }

        private Outfit? _ParseOutfit(JToken? result) {
            if (result == null) {
                return null;
            }

            Outfit outfit = new Outfit() {
                ID = result.Value<string?>("outfit_id") ?? "0",
                Name = result.Value<string?>("name") ?? "",
                Alias = result.Value<string?>("alias")
            };

            JToken? members = result.SelectToken("members");

            if (members != null) {
                JArray? arr = (JArray?)members;
                if (arr != null) {
                    List<OutfitMember> list = new List<OutfitMember>(result.Value<int?>("member_count") ?? 1);
                    foreach (JToken? iter in arr) {
                        if (iter == null) {
                            _Logger.LogDebug($"Skipping null iter");
                            continue;
                        }

                        _Logger.LogDebug($"Parsing: {iter}");

                        list.Add(_ParseMember(iter));
                    }
                    outfit.Members = list;
                } else {
                    _Logger.LogWarning($"Failed to get members");
                }
            } else {
                _Logger.LogInformation($"'members' isn't present");
            }

            return outfit;
        }

        private OutfitMember _ParseMember(JToken result) {
            return new OutfitMember() {
                ID = result.Value<string?>("character_id") ?? "0",
                Online = (result.Value<string?>("online_status") ?? "0") != "0"
            };
        }

    }
}
