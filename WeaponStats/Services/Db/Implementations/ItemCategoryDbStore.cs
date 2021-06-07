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

    public class ItemCategoryDbStore : IDataReader<Ps2ItemCategory>, IItemCategoryDbStore {

        private readonly ILogger<ItemCategoryDbStore> _Logger;
        private readonly IDbHelper _DbHelper;

        public ItemCategoryDbStore(ILogger<ItemCategoryDbStore> logger,
            IDbHelper helper) {

            _Logger = logger;

            _DbHelper = helper ?? throw new ArgumentNullException(nameof(helper));
        }

        public async Task<List<Ps2ItemCategory>> GetAll() {
            using NpgsqlConnection conn = _DbHelper.Connection();
            using NpgsqlCommand cmd = await _DbHelper.Command(conn, @"
                SELECT *
                    FROM item_category
            ");

            List<Ps2ItemCategory> entries = await ReadList(cmd);

            return entries;
        }

        public async Task Upsert(Ps2ItemCategory parameters) {
            using NpgsqlConnection conn = _DbHelper.Connection();
            using NpgsqlCommand cmd = await _DbHelper.Command(conn, @"
                INSERT INTO item_category (
                    category_id, name
                ) VALUES (
                    @ID, @Name
                ) ON CONFLICT (category_id) DO
                    UPDATE SET name = @Name
                    WHERE item_category.category_id = @ID
            ");

            cmd.AddParameter("@ID", parameters.ID);
            cmd.AddParameter("@Name", parameters.Name);

            await cmd.ExecuteNonQueryAsync();
        }

        public override Ps2ItemCategory ReadEntry(NpgsqlDataReader reader) {
            Ps2ItemCategory cat = new Ps2ItemCategory() {
                ID = reader.GetInt32("category_id"),
                Name = reader.GetString("name")
            };

            return cat;
        }

    }
}
