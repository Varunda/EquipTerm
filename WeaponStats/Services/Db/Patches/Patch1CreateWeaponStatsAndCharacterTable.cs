using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeaponStats.Services.Db.Patches {

    [PatchAttritube]
    public class Patch1CreateWeaponStatsAndCharacterTable : IDbPatch {

        public async Task Execute(IDbHelper helper) {
            using (NpgsqlConnection conn = helper.Connection()) {
                using NpgsqlCommand cmd = await helper.Command(conn, @"
                    CREATE TABLE IF NOT EXISTS weapon_stats (
                        character_id varchar NOT NULL,
                        item_id bigint NOT NULL,

                        timestamp timestamptz NOT NULL DEFAULT (NOW() at time zone 'utc'),

                        kills int NOT NULL,
                        deaths int NOT NULL,
                        headshots int NOT NULL,
                        shots int NOT NULL,
                        shots_hit int NOT NULL,
                        seconds_with int NOT NULL,

                        PRIMARY KEY (character_id, item_id)
                    );
                ");

                await cmd.ExecuteNonQueryAsync();
            }

            using (NpgsqlConnection conn = helper.Connection()) {
                using NpgsqlCommand cmd = await helper.Command(conn, @"
                    CREATE TABLE IF NOT EXISTS character (
                        character_id varchar NOT NULL PRIMARY KEY,
                        name varchar NOT NULL,
                        server_id int NOT NULL,
                        faction_id int NOT NULL,
                        outfit_id varchar NULL
                    );
                ");

                await cmd.ExecuteNonQueryAsync();
            }

        }

        public int MinVersion => 1;

        public string Name => $"CreateWeaponStatsAndCharacterTable";

    }
}
