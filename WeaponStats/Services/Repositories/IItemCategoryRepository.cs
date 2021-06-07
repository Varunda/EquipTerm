using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeaponStats.Models.Census;

namespace WeaponStats.Services.Repositories {

    /// <summary>
    /// Repository to interact with <see cref="Ps2ItemCategory"/>s
    /// </summary>
    public interface IItemCategoryRepository {

        /// <summary>
        ///     Get a specific <see cref="Ps2ItemCategory"/> by its ID
        /// </summary>
        /// <param name="ID">ID of the <see cref="Ps2ItemCategory"/> to get</param>
        /// <returns>
        ///     The <see cref="Ps2ItemCategory"/> with <see cref="Ps2ItemCategory.ID"/> of <paramref name="ID"/>,
        ///     or <c>null</c> if it doesn't exist
        /// </returns>
        Task<Ps2ItemCategory?> GetByID(int ID);

        /// <summary>
        ///     Get all <see cref="Ps2ItemCategory"/> available
        /// </summary>
        Task<List<Ps2ItemCategory>> GetAll();

        /// <summary>
        ///     Update/Insert a <see cref="Ps2ItemCategory"/>
        /// </summary>
        /// <param name="parameters">Parameters used to upsert</param>
        Task Upsert(Ps2ItemCategory parameters);

    }
}
