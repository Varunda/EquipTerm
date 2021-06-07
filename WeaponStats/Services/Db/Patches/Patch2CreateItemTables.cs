using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeaponStats.Services.Db.Patches {

    [PatchAttritube]
    public class Patch2CreateItemTables : IDbPatch {

        public int MinVersion => 2;
        public string Name => "CreateItemTables";

        public async Task Execute(IDbHelper helper) {
            using (NpgsqlConnection conn = helper.Connection()) {
                using NpgsqlCommand cmd = await helper.Command(conn, @"
                    CREATE TABLE IF NOT EXISTS items (
                        item_id int NOT NULL PRIMARY KEY,
                        type_id int NOT NULL,
                        category_id int NOT NULL,
                        name varchar NOT NULL,
                        faction_id int NOT NULL,
                        timestamp timestamptz NOT NULL DEFAULT (NOW() at time zone 'utc')
                    );
                ");

                await cmd.ExecuteNonQueryAsync();
            }

            using (NpgsqlConnection conn = helper.Connection()) {
                using NpgsqlCommand cmd = await helper.Command(conn, @"
                    CREATE TABLE IF NOT EXISTS item_type (
                        type_id int NOT NULL PRIMARY KEY,
                        name varchar NOT NULL,
                        code varchar NOT NULL,
                        timestamp timestamptz NOT NULL DEFAULT (NOW() at time zone 'utc')
                    );
                ");

                await cmd.ExecuteNonQueryAsync();
            }

            using (NpgsqlConnection conn = helper.Connection()) {
                using NpgsqlCommand cmd = await helper.Command(conn, @"
                    CREATE TABLE IF NOT EXISTS item_category (
                        category_id int NOT NULL PRIMARY KEY,
                        name varchar NOT NULL,
                        timestamp timestamptz NOT NULL DEFAULT (NOW() at time zone 'utc')
                    );
                ");

                await cmd.ExecuteNonQueryAsync();
            }
        }

    }
}
