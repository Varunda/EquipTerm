using DaybreakGames.Census;
using DaybreakGames.Census.Operators;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WeaponStats.Code.ExtensionMethods;
using WeaponStats.Models.Census;

namespace WeaponStats.Services.Census.Implementations {

    public class ItemCollection : IItemCollection {

        private readonly ILogger<ItemCollection> _Logger;
        private readonly ICensusQueryFactory _Census;

        /// <summary>
        /// The max amount of entries that can be returned from a census call to /item
        /// </summary>
        private const int MAX_RETURN_SIZE = 5000;

        public ItemCollection(ILogger<ItemCollection> logger, ICensusQueryFactory census) {
            _Logger = logger;
            _Census = census ?? throw new ArgumentNullException(nameof(census));
        }

        public async Task<List<Ps2Item>> GetAll() {
            List<Ps2Item> items;

            CensusQuery query = _Census.Create("item");
            query.SetLanguage(CensusLanguage.English);
            query.SetLimit(10000);
            query.ShowFields("item_id", "item_type_id", "item_category_id", "name", "faction_id");

            try {
                // If it's possible there are more items to get from census as the amount that can be returned is capped
                bool possibleMore = true;

                // Ensure we don't accidentally while true
                int iterationCount = 0;

                List<JToken> result = new List<JToken>();

                Stopwatch timer = Stopwatch.StartNew();

                while (possibleMore == true) {
                    IEnumerable<JToken> results = await _GetFromCensus(MAX_RETURN_SIZE * iterationCount);
                    result.AddRange(results);

                    _Logger.LogTrace($"Got {results.Count()} results");
                    if (results.Count() < MAX_RETURN_SIZE) {
                        possibleMore = false;
                    }

                    ++iterationCount;
                    if (iterationCount > 20) {
                        _Logger.LogError($"Still had more results after {iterationCount} iterations, doing {MAX_RETURN_SIZE} per call");
                    }
                }

                items = new List<Ps2Item>(result.Count());
                
                foreach (JToken token in result) {
                    Ps2Item? item = _ParseEntry(token);
                    if (item != null) {
                        items.Add(item);
                    }
                }

                timer.Stop();

                _Logger.LogDebug($"Took {timer.ElapsedMilliseconds}ms to load {items.Count} items");
            } catch (Exception ex) {
                _Logger.LogError(ex, "Failed to get item collection");
                items = new List<Ps2Item>();
            }

            return items;
        }

        private async Task<IEnumerable<JToken>> _GetFromCensus(int offset) {
            CensusQuery query = _Census.Create("item");
            query.SetLanguage(CensusLanguage.English);
            query.SetLimit(10000);
            query.ShowFields("item_id", "item_type_id", "item_category_id", "name", "faction_id");
            query.SetStart(offset);

            try {
                IEnumerable<JToken> result = await query.GetListAsync();
                return result;
            } catch (Exception ex) {
                _Logger.LogError(ex, "Failed to get item collection");
                return new List<JToken>();
            }
        }

        private Ps2Item? _ParseEntry(JToken? token) {
            if (token == null) {
                return null;
            }

            JToken? nameToken = token.SelectToken("name");

            Ps2Item item = new Ps2Item() {
                ID = token.GetInt32("item_id", -1),
                TypeID = token.GetInt32("item_type_id", -1),
                CategoryID = token.GetInt32("item_category_id", -1),
                FactionID = token.GetInt32("faction_id", -1),
                Name = nameToken?.GetString("en", "<Missing name>") ?? "<Missing name>"
            };

            return item;
        }

    }
}
