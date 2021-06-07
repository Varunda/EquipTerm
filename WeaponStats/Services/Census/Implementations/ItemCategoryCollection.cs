using DaybreakGames.Census;
using DaybreakGames.Census.Operators;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeaponStats.Code.ExtensionMethods;
using WeaponStats.Models.Census;

namespace WeaponStats.Services.Census.Implementations {

    public class ItemCategoryCollection : IItemCategoryCollection {

        private readonly ILogger<ItemCategoryCollection> _Logger;
        private readonly ICensusQueryFactory _Census;

        public ItemCategoryCollection(ILogger<ItemCategoryCollection> logger,
            ICensusQueryFactory census) {

            _Logger = logger;

            _Census = census ?? throw new ArgumentNullException(nameof(census));
        }

        public async Task<List<Ps2ItemCategory>> GetAll() {
            List<Ps2ItemCategory> entries;

            CensusQuery query = _Census.Create("item_category");
            query.SetLanguage(CensusLanguage.English);
            query.SetLimit(1000);

            try {
                IEnumerable<JToken> result = await query.GetListAsync();

                entries = new List<Ps2ItemCategory>(result.Count());

                foreach (JToken token in result) {
                    Ps2ItemCategory? cat = _ParseEntry(token);
                    if (cat != null) {
                        entries.Add(cat);
                    }
                }
            } catch (Exception ex) {
                _Logger.LogError(ex, "Failed to get item_category");
                entries = new List<Ps2ItemCategory>();
            }

            return entries;
        }

        private Ps2ItemCategory? _ParseEntry(JToken token) {
            if (token == null) {
                return null;
            }

            JToken? nameToken = token.SelectToken("name");

            Ps2ItemCategory cat = new Ps2ItemCategory() {
                ID = token.GetInt32("item_category_id", 0),
                Name = nameToken?.GetString("en", "<Missing name>") ?? "<Missing name>"
            };

            return cat;
        }

    }
}
