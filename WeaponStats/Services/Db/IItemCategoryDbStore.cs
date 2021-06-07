using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeaponStats.Models.Census;

namespace WeaponStats.Services.Db {

    /// <summary>
    /// Service to interact with the DB table that has the <see cref="Ps2ItemCategory"/> entries
    /// </summary>
    public interface IItemCategoryDbStore {

        /// <summary>
        ///     Get all entries stored in the database
        /// </summary>
        Task<List<Ps2ItemCategory>> GetAll();

        /// <summary>
        /// Insert/Update an entry
        /// </summary>
        /// <param name="parameters">Parameters of the item to use</param>
        Task Upsert(Ps2ItemCategory parameters);

    }
}
