using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeaponStats.Services.Db {

    public interface IItemStatDbStore {

        Task<List<decimal>> GetKillDeathRatioByItemID(int itemID);

        Task<List<decimal>> GetKillsPerMinuteByItemID(int itemID);

        Task<List<decimal>> GetHeadshotRatioByItemID(int itemID);

        Task<List<decimal>> GetAccuracyByItemID(int itemID);

    }
}
