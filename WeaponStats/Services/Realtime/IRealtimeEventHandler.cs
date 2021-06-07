using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

namespace WeaponStats.Services.Realtime {

    /// <summary>
    /// Service that handles realtime events from the realtime web socket
    /// </summary>
    public interface IRealtimeEventHandler {

        /// <summary>
        /// Process the JSON from the realtime web socket, and execute the event listeners
        /// </summary>
        /// <param name="token"></param>
        void Process(JToken token);

    }
}
