using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeaponStats.Models.Census {

    public class Ps2Character {

        public string ID { get; set; } = "";

        public string Name { get; set; } = "";

        public int WorldID { get; set; }

        public int FactionID { get; set; }

        public string? OutfitID { get; set; }

        public string? OutfitTag { get; set; }

        public string? OutfitName { get; set; }

    }

}
