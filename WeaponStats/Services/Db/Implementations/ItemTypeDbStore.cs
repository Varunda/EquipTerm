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

    public class ItemTypeDbStore : IDataReader<Ps2ItemType>, IItemTypeDbStore {

        private readonly ILogger<ItemTypeDbStore> _Logger;
        private readonly IDbHelper _DbHelper;

        public ItemTypeDbStore(ILogger<ItemTypeDbStore> logger,
            IDbHelper helper) {

            _Logger = logger;
            _DbHelper = helper ?? throw new ArgumentNullException(nameof(helper));
        }

        public async Task<List<Ps2ItemType>> GetAll() {
            using NpgsqlConnection conn = _DbHelper.Connection();
            using NpgsqlCommand cmd = await _DbHelper.Command(conn, @"
                SELECT *
                    FROM item_type
            ");

            List<Ps2ItemType> types = await ReadList(cmd);

            return types;
        }

        public async Task Upsert(Ps2ItemType parameters) {
            using NpgsqlConnection conn = _DbHelper.Connection();
            using NpgsqlCommand cmd = await _DbHelper.Command(conn, @"
                INSERT INTO item_type (
                    type_id, name, code
                ) VALUES (
                    @ID, @Name, @Code
                ) ON CONFLICT (type_id) DO
                    UPDATE SET name = @Name,
                        code = @Code
                    WHERE item_type.type_id = @ID
            ");

            cmd.AddParameter("@ID", parameters.ID);
            cmd.AddParameter("@Name", parameters.Name);
            cmd.AddParameter("@Code", parameters.Code);

            await cmd.ExecuteNonQueryAsync();
        }

        public override Ps2ItemType ReadEntry(NpgsqlDataReader reader) {
            Ps2ItemType type = new Ps2ItemType();

            type.ID = reader.GetInt32("type_id");
            type.Name = reader.GetString("name");
            type.Code = reader.GetString("code");

            return type;
        }

    }
}
