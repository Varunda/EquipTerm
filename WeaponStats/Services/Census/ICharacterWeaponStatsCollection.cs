using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeaponStats.Models;

namespace WeaponStats.Services.Census {

    public interface ICharacterWeaponStatsCollection {

        Task<List<WeaponStatEntry>> GetByCharacterIDAsync(string charID);

    }

}
