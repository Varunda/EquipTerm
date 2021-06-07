using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeaponStats.Models.Census;

namespace WeaponStats.Services.Census {

    /// <summary>
    /// Manages <see cref="Ps2Character"/>s
    /// </summary>
    public interface ICharacterCollection {

        /// <summary>
        ///     Get a <see cref="Ps2Character"/> by ID
        /// </summary>
        /// <param name="ID">ID of the character to get</param>
        /// <returns>
        ///     The <see cref="Ps2Character"/> with <see cref="Ps2Character.ID"/> of <paramref name="ID"/>,
        ///     or <c>null</c> if it doesn't exist
        /// </returns>
        Task<Ps2Character?> GetByIDAsync(string ID);

        /// <summary>
        ///     Get a <see cref="Ps2Character"/> by name
        /// </summary>
        /// <param name="name">Name of the <see cref="Ps2Character"/> to get</param>
        /// <returns>
        ///     The <see cref="Ps2Character"/> with <see cref="Ps2Character.Name"/> of <paramref name="name"/>,
        ///     or <c>null</c> if it doesn't exist
        /// </returns>
        Task<Ps2Character?> GetByNameAsync(string name);

    }
}
