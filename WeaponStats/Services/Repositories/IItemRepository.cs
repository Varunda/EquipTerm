using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeaponStats.Models.Census;

namespace WeaponStats.Services.Repositories {

    /// <summary>
    /// Service for interacting with <see cref="Ps2Item"/>s
    /// </summary>
    public interface IItemRepository {

        /// <summary>
        ///     Get all <see cref="Ps2Item"/>s stored in the backing data layers
        /// </summary>
        Task<List<Ps2Item>> GetAll();

        /// <summary>
        ///     Get all <see cref="Ps2Item"/> that match the type ID
        /// </summary>
        /// <param name="typeID">ID of the <see cref="Ps2Item.TypeID"/> to get</param>
        /// <returns>
        ///     All <see cref="Ps2Item"/> with <see cref="Ps2Item.TypeID"/> of <paramref name="typeID"/>
        /// </returns>
        Task<List<Ps2Item>> GetByTypeID(int typeID);

        /// <summary>
        ///     Get all <see cref="Ps2Item"/> that match the category ID
        /// </summary>
        /// <param name="categoryID">ID of the <see cref="Ps2Item.TypeID"/> to get</param>
        /// <returns>
        ///     All <see cref="Ps2Item"/> with <see cref="Ps2Item.CategoryID"/> of <paramref name="categoryID"/>
        /// </returns>
        Task<List<Ps2Item>> GetByCategoryID(int categoryID);

        /// <summary>
        ///     Get a specific <see cref="Ps2Item"/> by ID
        /// </summary>
        /// <param name="itemID">ID of the <see cref="Ps2Item"/> to get</param>
        /// <returns>
        ///     The <see cref="Ps2Item"/> with <see cref="Ps2Item.ID"/> of <paramref name="itemID"/>,
        ///     or <c>null</c> if it doesn't exist
        /// </returns>
        Task<Ps2Item?> GetByID(int itemID);

        /// <summary>
        ///     Update/Insert a <see cref="Ps2Item"/>
        /// </summary>
        /// <param name="item">Values to use when performing the update/insert</param>
        Task Upsert(Ps2Item item);

    }
}
