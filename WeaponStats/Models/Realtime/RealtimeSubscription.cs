using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeaponStats.Models.Realtime {

    /// <summary>
    /// Wrapper around the subscription options for the realtime events
    /// </summary>
    public class RealtimeSubscription {

        /// <summary>
        /// List of character IDs to listen to, or "all" to listen to all
        /// </summary>
        public List<string> Characters { get; set; } = new List<string>();

        /// <summary>
        /// List of events to listen for, or "all"
        /// </summary>
        public List<string> Events { get; set; } = new List<string>();

        /// <summary>
        /// List of worlds to listen to, or "all"
        /// </summary>
        public List<string> Worlds { get; set; } = new List<string>();

    }
}
