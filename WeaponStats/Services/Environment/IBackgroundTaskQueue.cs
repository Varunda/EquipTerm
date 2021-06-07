using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WeaponStats.Services.Environment {

    public interface IBackgroundTaskQueue {

        void Queue(JToken payload);

        Task<JToken> DequeueAsync(CancellationToken cancel);

    }
}
