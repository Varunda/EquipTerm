using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeaponStats.Models.Census;

namespace WeaponStats.Services.Census {

    public interface IOutfitCollection {

        Task<Outfit?> GetByIDAsync(string ID);

        Task<Outfit?> GetByTagAsync(string tag);

    }
}
