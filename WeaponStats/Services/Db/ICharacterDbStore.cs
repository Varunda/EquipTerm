using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeaponStats.Models.Census;

namespace WeaponStats.Services.Db {

    /// <summary>
    /// Database service to interact with <see cref="Ps2Character"/>s
    /// </summary>
    public interface ICharacterDbStore {

        /// <summary>
        ///     Get a character from the backing database by ID
        /// </summary>
        /// <param name="charID">ID of the <see cref="Ps2Character"/> to get</param>
        /// <returns>
        ///     The <see cref="Ps2Character"/> with <see cref="Ps2Character.ID"/> of <paramref name="charID"/>,
        ///     or <c>null</c> if it doesn't exist in the database, but it may exist in Census
        /// </returns>
        Task<Ps2Character?> GetByID(string charID);

        /// <summary>
        ///     Get a character from the backing database by name
        /// </summary>
        /// <param name="name">Name of the <see cref="Ps2Character"/> to get</param>
        /// <returns>
        ///     The <see cref="Ps2Character"/> with <see cref="Ps2Character.Name"/> of <paramref name="name"/>,
        ///     or <c>null</c> if it doesn't exist in the database, but it may exist in Census
        /// </returns>
        Task<Ps2Character?> GetByName(string name);

        /// <summary>
        ///     Insert or update (Upsert) a character in the backing database
        /// </summary>
        /// <param name="character">Character parameters to pass</param>
        /// <returns>
        ///     A task for when the operation is complete
        /// </returns>
        Task Upsert(Ps2Character character);

    }
}
