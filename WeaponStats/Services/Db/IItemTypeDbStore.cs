using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeaponStats.Models.Census;

namespace WeaponStats.Services.Db {

    /// <summary>
    /// Service to interact with the DB store of <see cref="Ps2ItemType"/>s
    /// </summary>
    public interface IItemTypeDbStore {

        /// <summary>
        ///     Get all <see cref="Ps2ItemType"/>s stored in the DB
        /// </summary>
        Task<List<Ps2ItemType>> GetAll();

        /// <summary>
        ///     Update/Insert a <see cref="Ps2ItemType"/> using the values in <paramref name="parameters"/>
        /// </summary>
        /// <param name="parameters">Values used to insert</param>
        Task Upsert(Ps2ItemType parameters);

    }
}
