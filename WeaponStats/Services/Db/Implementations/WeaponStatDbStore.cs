using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using WeaponStats.Code.ExtensionMethods;
using WeaponStats.Models;

namespace WeaponStats.Services.Db.Implementations {

    public class WeaponStatDbStore : IDataReader<WeaponStatEntry>, IWeaponStatDbStore {

        private readonly ILogger<WeaponStatDbStore> _Logger;
        private readonly IDbHelper _DbHelper;

        public WeaponStatDbStore(ILogger<WeaponStatDbStore> logger,
                IDbHelper helper) {

            _Logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _DbHelper = helper ?? throw new ArgumentNullException(nameof(helper));
        }

        public async Task<List<WeaponStatEntry>> GetByCharacterID(string charID) {
            using NpgsqlConnection conn = _DbHelper.Connection();
            using NpgsqlCommand cmd = await _DbHelper.Command(conn, @"
                SELECT *
                    FROM weapon_stats
                    WHERE character_id = @ID
            ");
            cmd.Parameters.AddWithValue("ID", charID);

            List<WeaponStatEntry> entry = await ReadList(cmd);

            return entry;
        }

        public Task<List<WeaponStatEntry>> GetByItemID(long itemID) {
            throw new NotImplementedException();
        }

        public async Task Upsert(WeaponStatEntry entry) {
            using NpgsqlConnection conn = _DbHelper.Connection();
            using NpgsqlCommand cmd = await _DbHelper.Command(conn, @"
                INSERT INTO weapon_stats (
                    character_id, item_id, kills, deaths, shots, shots_hit, headshots, seconds_with
                ) VALUES (
                    @CharID, @ItemID, @Kills, @Deaths, @Shots, @ShotsHit, @Headshots, @SecondsWith
                ) ON CONFLICT (character_id, item_id) DO
                    UPDATE SET kills = @Kills,
                        deaths = @Deaths,
                        shots = @Shots,
                        shots_hit = @ShotsHit,
                        headshots = @Headshots,
                        seconds_with = @SecondsWith,
                        timestamp = (NOW() at time zone 'utc')
                    WHERE
                        weapon_stats.character_id = @CharID
                        AND weapon_stats.item_id = @ItemID
            ");

            cmd.AddParameter("@CharID", entry.CharacterID);
            cmd.AddParameter("@ItemID", entry.WeaponID);
            cmd.AddParameter("@Kills", entry.Kills);
            cmd.AddParameter("@Deaths", entry.Deaths);
            cmd.AddParameter("@Shots", entry.Shots);
            cmd.AddParameter("@ShotsHit", entry.ShotsHit);
            cmd.AddParameter("@Headshots", entry.Headshots);
            cmd.AddParameter("@SecondsWith", entry.SecondsWith);

            await cmd.ExecuteNonQueryAsync();
        }

        public override WeaponStatEntry ReadEntry(NpgsqlDataReader reader) {
            WeaponStatEntry entry = new WeaponStatEntry {
                CharacterID = reader.GetString("character_id"),
                WeaponID = reader.GetInt64("item_id"),
                Kills = reader.GetInt32("kills"),
                Deaths = reader.GetInt32("deaths"),
                Shots = reader.GetInt32("shots"),
                ShotsHit = reader.GetInt32("shots_hit"),
                Headshots = reader.GetInt32("headshots"),
                SecondsWith = reader.GetInt32("seconds_with")
            };

            return entry;
        }

    }
}
