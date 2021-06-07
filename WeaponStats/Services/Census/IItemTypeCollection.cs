using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeaponStats.Models.Census;

namespace WeaponStats.Services.Census {

    /// <summary>
    /// Service to get the <see cref="Ps2ItemType"/>s from the census collection
    /// </summary>
    public interface IItemTypeCollection {

        /// <summary>
        ///     Get all <see cref="Ps2ItemType"/>s in census collection
        /// </summary>
        Task<List<Ps2ItemType>> GetAll();

    }
}
