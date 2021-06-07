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

    public class ItemDbStore : IDataReader<Ps2Item>, IItemDbStore {

        private readonly ILogger<ItemDbStore> _Logger;
        private readonly IDbHelper _DbHelper;

        public ItemDbStore(ILogger<ItemDbStore> logger,
            IDbHelper helper) {

            _Logger = logger;
            _DbHelper = helper ?? throw new ArgumentNullException(nameof(helper));
        }

        public async Task<List<Ps2Item>> GetAll() {
            using NpgsqlConnection conn = _DbHelper.Connection();
            using NpgsqlCommand cmd = await _DbHelper.Command(conn, @"
                SELECT *
                    FROM items
            ");

            List<Ps2Item> items = await ReadList(cmd);

            return items;
        }

        public async Task Upsert(Ps2Item item) {
            using NpgsqlConnection conn = _DbHelper.Connection();
            using NpgsqlCommand cmd = await _DbHelper.Command(conn, @"
                INSERT INTO items (
                    item_id, type_id, category_id, name, faction_id
                ) VALUES (
                    @ID, @TypeID, @CategoryID, @Name, @FactionID
                ) ON CONFLICT (item_id) DO
                    UPDATE SET type_id = @TypeID,
                        category_id = @CategoryID,
                        name = @Name,
                        faction_id = @FactionID
                    WHERE items.item_id = @ID
            ");

            cmd.AddParameter("@ID", item.ID);
            cmd.AddParameter("@TypeID", item.TypeID);
            cmd.AddParameter("@CategoryID", item.CategoryID);
            cmd.AddParameter("@Name", item.Name);
            cmd.AddParameter("@FactionID", item.FactionID);

            await cmd.ExecuteNonQueryAsync();
        }

        public override Ps2Item ReadEntry(NpgsqlDataReader reader) {
            Ps2Item item = new Ps2Item();

            item.ID = reader.GetInt32("item_id");
            item.TypeID = reader.GetInt32("type_id");
            item.CategoryID = reader.GetInt32("category_id");
            item.FactionID = reader.GetInt32("faction_id");
            item.Name = reader.GetString("name");

            return item;
        }

    }
}
