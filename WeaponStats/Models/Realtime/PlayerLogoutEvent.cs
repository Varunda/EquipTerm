using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeaponStats.Models.Realtime {

    /// <summary>
    /// Realtime event for when a character logs out
    /// </summary>
    public class PlayerLogoutEvent {

        /// <summary>
        /// ID of the character that logged out
        /// </summary>
        public string CharacterID { get; set; } = "";

        /// <summary>
        /// Timestamp of when the character logged out
        /// </summary>
        public DateTime Timestamp { get; set; }

    }
}
