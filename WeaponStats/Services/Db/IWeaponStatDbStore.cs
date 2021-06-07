using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeaponStats.Models;

namespace WeaponStats.Services.Db {

    public interface IWeaponStatDbStore {

        Task<List<WeaponStatEntry>> GetByCharacterID(string charID);

        Task<List<WeaponStatEntry>> GetByItemID(long itemID);

        Task Upsert(WeaponStatEntry entry);

    }
}
