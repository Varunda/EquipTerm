using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeaponStats.Code.Commands {

    [Command]
    public class PingCommand {

        private readonly ILogger<PingCommand> _Logger;

        public PingCommand(IServiceProvider services) {
            _Logger = (ILogger<PingCommand>)services.GetService(typeof(ILogger<PingCommand>));
        }

        public void Ping() {
            _Logger.LogInformation($"Pong");
        }

    }
}
