﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeaponStats.Models.Census {

    public class WeaponStatByFactionEntry {

        public string CharacterID { get; set; } = "";

        public string StatName { get; set; } = "";

        public string ItemID { get; set; } = "";

        public string VehicleID { get; set; } = "";

        public int ValueVS { get; set; }

        public int ValueNC { get; set; }

        public int ValueTR { get; set; }

    }
}
