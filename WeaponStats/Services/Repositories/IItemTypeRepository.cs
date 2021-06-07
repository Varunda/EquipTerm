using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeaponStats.Models.Census;

namespace WeaponStats.Services.Repositories {

    /// <summary>
    /// Service to interact with <see cref="Ps2ItemType"/>s
    /// </summary>
    public interface IItemTypeRepository {

        /// <summary>
        ///     Get all <see cref="Ps2ItemType"/>s stored in the repository
        /// </summary>
        Task<List<Ps2ItemType>> GetAll();

        /// <summary>
        ///     Get a specific <see cref="Ps2ItemType"/> by its ID
        /// </summary>
        /// <param name="ID">ID of the <see cref="Ps2ItemType"/> to get</param>
        /// <returns>
        ///     The <see cref="Ps2ItemType"/> with <see cref="Ps2ItemType.ID"/> of <paramref name="ID"/>,
        ///     or <c>null</c> if it doesn't exist
        /// </returns>
        Task<Ps2ItemType?> GetByID(int ID);

        /// <summary>
        /// Update/Insert a <see cref="Ps2ItemType"/>
        /// </summary>
        /// <param name="parameters">Values to use when doing the insert/update</param>
        Task Upsert(Ps2ItemType parameters);

    }
}
