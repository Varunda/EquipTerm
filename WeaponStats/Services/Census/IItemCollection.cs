using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeaponStats.Models.Census;

namespace WeaponStats.Services.Census {

    /// <summary>
    /// Service to interact with the item collection
    /// </summary>
    public interface IItemCollection {

        /// <summary>
        ///     Get all <see cref="Ps2Item"/>s in the census collection
        /// </summary>
        Task<List<Ps2Item>> GetAll();

    }
}
