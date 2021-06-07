using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeaponStats.Models.Census;

namespace WeaponStats.Services.Census {

    /// <summary>
    /// Interface for using the Census item category collection
    /// </summary>
    public interface IItemCategoryCollection {

        /// <summary>
        ///     Get all <see cref="Ps2ItemCategory"/>s there are
        /// </summary>
        /// <remarks>
        ///     No method to get a single <see cref="Ps2ItemCategory"/> is supported, as you're supposed to be using
        ///     the repo, this is meant more for internal use
        /// </remarks>
        Task<List<Ps2ItemCategory>> GetAll();

    }
}
