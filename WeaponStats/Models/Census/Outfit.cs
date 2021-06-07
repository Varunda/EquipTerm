using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeaponStats.Models.Census {

    public class Outfit {

        public string ID { get; set; } = "";

        public string Name { get; set; } = "";

        public string? Alias { get; set; }

        public List<OutfitMember> Members { get; set; } = new List<OutfitMember>();

    }
}
