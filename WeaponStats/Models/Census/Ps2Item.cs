using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeaponStats.Models.Census {

    public class Ps2Item {

        public int ID { get; set; }

        public int TypeID { get; set; }

        public int CategoryID { get; set; }

        public string Name { get; set; } = "";

        public int FactionID { get; set; }


    }
}
