using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeaponStats.Models {

    /// <summary>
    /// Parameters about when to update a character in a background service
    /// </summary>
    public class CharacterUpdateParameters {

        /// <summary>
        /// ID of the character to update
        /// </summary>
        public string CharacterID { get; set; } = "";

        /// <summary>
        /// When the character logged out
        /// </summary>
        public DateTime LogoutTime = DateTime.UtcNow;

    }
}
