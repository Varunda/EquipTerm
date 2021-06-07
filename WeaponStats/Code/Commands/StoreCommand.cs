using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeaponStats.Services.Census;
using WeaponStats.Code.Constants;
using WeaponStats.Models.Census;
using WeaponStats.Services.Environment;

namespace WeaponStats.Code.Commands {

    [Command]
    public class StoreCommand {

        private readonly ILogger<StoreCommand> _Logger;
        private readonly ICharacterCollection _Characters;

        public StoreCommand(IServiceProvider services) {
            _Logger = (ILogger<StoreCommand>)services.GetService(typeof(ILogger<StoreCommand>));

            _Characters = (ICharacterCollection)services.GetService(typeof(ICharacterCollection));
        }

        public async Task Print(string nameOrId) {
            Ps2Character? c;
            if (nameOrId.Length == 19) {
                c = await _Characters.GetByIDAsync(nameOrId);
            } else {
                c = await _Characters.GetByNameAsync(nameOrId);
            }

            if (c == null) {
                _Logger.LogWarning($"Failed to find {nameOrId}");
                return;
            }
        }

    }
}
