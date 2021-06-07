using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeaponStats.Models.Census;

namespace WeaponStats.Services.Repositories {

    /// <summary>
    /// Service to get and update <see cref="Ps2Character"/>s, both from census and the backing DB
    /// </summary>
    public interface ICharacterRepository {

        /// <summary>
        ///     Get a <see cref="Ps2Character"/>
        /// </summary>
        /// <param name="charID">ID of the <see cref="Ps2Character"/> to get</param>
        /// <returns>
        ///     The <see cref="Ps2Character"/> with <see cref="Ps2Character.ID"/> of <paramref name="charID"/>,
        ///     or <c>null</c> if it doesn't exist
        /// </returns>
        Task<Ps2Character?> GetByID(string charID);

        Task<Ps2Character?> GetByName(string name);

        Task Upsert(Ps2Character character);

    }
}
