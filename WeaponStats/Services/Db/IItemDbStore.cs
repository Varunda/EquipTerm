using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeaponStats.Models.Census;

namespace WeaponStats.Services.Db {

    /// <summary>
    /// Service to interact with the DB that locally stored <see cref="Ps2Item"/>s
    /// </summary>
    public interface IItemDbStore {

        /// <summary>
        ///     Get all <see cref="Ps2Item"/>s stored in the DB
        /// </summary>
        Task<List<Ps2Item>> GetAll();

        /// <summary>
        ///     Update/Insert a <see cref="Ps2Item"/>
        /// </summary>
        /// <param name="item">Values used to insert/update</param>
        Task Upsert(Ps2Item item);

    }
}
