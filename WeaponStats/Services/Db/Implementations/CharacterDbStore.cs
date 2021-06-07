using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using WeaponStats.Code.ExtensionMethods;
using WeaponStats.Models.Census;

namespace WeaponStats.Services.Db.Implementations {

    public class CharacterDbStore : IDataReader<Ps2Character>, ICharacterDbStore {

        private readonly ILogger<CharacterDbStore> _Logger;
        private readonly IDbHelper _DbHelper;

        public CharacterDbStore(ILogger<CharacterDbStore> logger, IMemoryCache cache,
                IDbHelper helper) {

            _Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _DbHelper = helper ?? throw new ArgumentNullException(nameof(helper));
        }

        public async Task<Ps2Character?> GetByID(string charID) {
            using NpgsqlConnection conn = _DbHelper.Connection();
            using NpgsqlCommand cmd = await _DbHelper.Command(conn, @"
                SELECT *
                    FROM character
                    WHERE character_id = @ID
            ");

            cmd.Parameters.AddWithValue("@ID", charID);

            Ps2Character? c = await ReadSingle(cmd);

            return c;
        }

        public async Task<Ps2Character?> GetByName(string name) {
            using NpgsqlConnection conn = _DbHelper.Connection();
            using NpgsqlCommand cmd = await _DbHelper.Command(conn, @"
                SELECT *
                    FROM character
                    WHERE name = @Name
            ");

            cmd.Parameters.AddWithValue("@Name", name);

            Ps2Character? c = await ReadSingle(cmd);

            return c;
        }

        public async Task Upsert(Ps2Character character) {
            using NpgsqlConnection conn = _DbHelper.Connection();
            using NpgsqlCommand cmd = await _DbHelper.Command(conn, @"
                INSERT INTO character (
                    character_id, name, server_id, faction_id, outfit_id
                ) VALUES (
                    @charID, @name, @server, @faction, @outfit
                ) ON CONFLICT (character_id) DO
                    UPDATE SET name = @name,
                        server_id = @server,
                        faction_id = @faction,
                        outfit_id = @outfit
            ");

            cmd.AddParameter("@charID", character.ID);
            cmd.AddParameter("@name", character.Name);
            cmd.AddParameter("@server", character.WorldID);
            cmd.AddParameter("@faction", character.FactionID);
            cmd.AddParameter("@outfit", character.OutfitID);

            await cmd.ExecuteNonQueryAsync();
        }

        public override Ps2Character ReadEntry(NpgsqlDataReader reader) {
            Ps2Character c = new Ps2Character();

            // Why keep the reading separate? So if there is an error reading a column,
            // the exception contains the line, which contains the bad column
            c.ID = reader.GetString("character_id");
            c.Name = reader.GetString("name");
            c.FactionID = reader.GetInt32("faction_id");
            c.WorldID = reader.GetInt32("server_id");

            if (reader.IsDBNull("outfit_id") == true) {
                c.OutfitID = null;
            } else {
                c.OutfitID = reader.GetString("outfit_id");
            }

            return c;
        }

    }
}
