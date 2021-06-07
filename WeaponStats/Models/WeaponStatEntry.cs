using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeaponStats.Models {

    // characters_weapon_stat_by_faction>   weapon_headshots, weapon_kills
    // characters_weapon-stats>             weapon_play_time, weapon_hit_count, weapon_fire_count, weapon_deaths

    public class WeaponStatEntry {

        public Int64 WeaponID { get; set; }

        public string CharacterID { get; set; } = "";

        public int Kills { get; set; }

        public int Deaths { get; set; }

        public int Headshots { get; set; }

        public int Shots { get; set; }

        public int ShotsHit { get; set; }

        public int SecondsWith { get; set; }

        /// <summary>
        /// When the entry was created
        /// </summary>
        public DateTime Timestamp { get; set; }

        public decimal Accuracy => ShotsHit / Math.Max(1m, Shots);

        public decimal HeadshotRatio => Headshots / Math.Max(1m, Kills);

        public decimal KillDeathRatio => Kills / Math.Max(Deaths, 1m);

        public decimal KillsPerMinute => Kills / (Math.Max(SecondsWith, 1m) / 60m);

    }
}
