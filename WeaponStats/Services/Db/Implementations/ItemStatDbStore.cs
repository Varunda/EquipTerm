using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeaponStats.Code;
using WeaponStats.Code.ExtensionMethods;

namespace WeaponStats.Services.Db.Implementations {

    public class ItemStatDbStore : IItemStatDbStore {

        private readonly ILogger<ItemStatDbStore> _Logger;
        private readonly IDbHelper _DbHelper;

        public ItemStatDbStore(ILogger<ItemStatDbStore> logger,
            IDbHelper helper) {

            _Logger = logger;
            _DbHelper = helper ?? throw new ArgumentNullException(nameof(helper));
        }

        public async Task<List<decimal>> GetAccuracyByItemID(int itemID) {
            using NpgsqlConnection conn = _DbHelper.Connection();
            using NpgsqlCommand cmd = await _DbHelper.Command(conn, @"
                SELECT ROUND((cast(headshots as decimal) / cast(GREATEST(kills, 1) as decimal)), 4) AS value
	                FROM weapon_stats
	                WHERE item_id = @ID
		                AND kills > 1159
	            ORDER BY value
            ");

            cmd.AddParameter("@ID", itemID);

            List<decimal> values = await ReadList(cmd);

            return values;
        }

        public async Task<List<decimal>> GetHeadshotRatioByItemID(int itemID) {
            using NpgsqlConnection conn = _DbHelper.Connection();
            using NpgsqlCommand cmd = await _DbHelper.Command(conn, @"
                SELECT ROUND((cast(shots_hit as decimal) / cast(GREATEST(shots, 1) as decimal)), 4) AS value
	                FROM weapon_stats
	                WHERE item_id = @ID
		                AND kills > 1159
	            ORDER BY value
            ");

            cmd.AddParameter("@ID", itemID);

            List<decimal> values = await ReadList(cmd);

            return values;
        }

        public async Task<List<decimal>> GetKillDeathRatioByItemID(int itemID) {
            using NpgsqlConnection conn = _DbHelper.Connection();
            using NpgsqlCommand cmd = await _DbHelper.Command(conn, @"
                SELECT ROUND((cast(kills as decimal) / cast(GREATEST(deaths, 1) as decimal)), 4) AS value
	                FROM weapon_stats
	                WHERE item_id = @ID
		                AND kills > 1159
	            ORDER BY value
            ");

            cmd.AddParameter("@ID", itemID);

            List<decimal> values = await ReadList(cmd);

            return values;
        }

        public async Task<List<decimal>> GetKillsPerMinuteByItemID(int itemID) {
            using NpgsqlConnection conn = _DbHelper.Connection();
            using NpgsqlCommand cmd = await _DbHelper.Command(conn, @"
                SELECT ROUND((cast(kills as decimal) / cast(GREATEST(seconds_with, 1) as decimal)) * cast(60 as decimal), 4) AS value
	                FROM weapon_stats
	                WHERE item_id = @ID
		                AND kills > 1159
	                ORDER BY value
            ");

            cmd.AddParameter("@ID", itemID);

            List<decimal> values = await ReadList(cmd);

            return values;
        }

        private async Task<List<decimal>> ReadList(NpgsqlCommand cmd) {
            List<decimal> entries = new List<decimal>();

            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync() == true) {
                entries.Add(reader.GetDecimal(0));
            }

            return entries;
        }

    }
}
