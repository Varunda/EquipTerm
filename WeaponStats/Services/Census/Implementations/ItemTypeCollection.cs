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

    public class ItemTypeCollection : IItemTypeCollection {

        private readonly ILogger<ItemTypeCollection> _Logger;
        private readonly ICensusQueryFactory _Census;

        public ItemTypeCollection(ILogger<ItemTypeCollection> logger,
            ICensusQueryFactory census) {

            _Logger = logger;
            _Census = census ?? throw new ArgumentNullException(nameof(census));
        }

        public async Task<List<Ps2ItemType>> GetAll() {
            List<Ps2ItemType> types;

            CensusQuery query = _Census.Create("item_type");
            query.SetLanguage(CensusLanguage.English);
            query.SetLimit(1000);

            try {
                IEnumerable<JToken> result = await query.GetListAsync();

                types = new List<Ps2ItemType>(result.Count());
                foreach (JToken token in result) {
                    Ps2ItemType? type = _ParseEntry(token);
                    if (type != null) {
                        types.Add(type);
                    }
                }
            } catch (Exception ex) {
                _Logger.LogError(ex, "Failed to get item_type from census");
                types = new List<Ps2ItemType>();
            }

            return types;
        }

        private Ps2ItemType? _ParseEntry(JToken? token) {
            if (token == null) {
                return null;
            }

            Ps2ItemType type = new Ps2ItemType() {
                ID = token.GetInt32("item_type_id", -1),
                Name = token.GetString("name", "<Missing name>"),
                Code = token.GetString("code", "<Missing code>")
            };

            return type;
        }

    }
}
