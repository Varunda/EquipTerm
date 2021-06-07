using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeaponStats.Code.Commands {

    [Command]
    public class CacheCommand {

        private readonly ILogger<CacheCommand> _Logger;
        private readonly IMemoryCache _Cache;

        private DateTime? _ConfirmTime;

        public CacheCommand(IServiceProvider services) {
            _Logger = services.GetRequiredService<ILogger<CacheCommand>>();
            _Cache = services.GetRequiredService<IMemoryCache>();
        }

        public void Delete(string key) {
            _Cache.Remove(key);
        }

        public void Clear() {
            if (_ConfirmTime == null || (_ConfirmTime - TimeSpan.FromSeconds(10)) < DateTime.UtcNow) {
                _Logger.LogWarning($"This is a dangerous command. Please use 'cache confirm' before");
                return;
            }
        }

        public void Confirm() {
            _ConfirmTime = DateTime.UtcNow;
        }

    }
}
