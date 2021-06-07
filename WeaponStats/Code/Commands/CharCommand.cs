using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeaponStats.Services.Census;
using WeaponStats.Models.Census;
using WeaponStats.Models;
using WeaponStats.Services.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace WeaponStats.Code.Commands {

    [Command]
    public class CharCommand {

        private readonly ILogger<CharCommand> _Logger;
        private readonly ICharacterCollection _Characters;
        private readonly IWeaponStatRepository _Weapons;

        public CharCommand(IServiceProvider services) {
            _Logger = (ILogger<CharCommand>)services.GetService(typeof(ILogger<CharCommand>));

            _Characters = services.GetRequiredService<ICharacterCollection>();
            _Weapons = services.GetRequiredService<IWeaponStatRepository>();
        }

        public async Task Get(string name) {
            Ps2Character? c = await _Characters.GetByNameAsync(name);
            if (c != null) {
                _Logger.LogInformation($"{name} => {JToken.FromObject(c)}");
            } else {
                _Logger.LogInformation($"{name} => null");
            }
        }

        public async Task Item(string name) {
            Ps2Character? c = await _Characters.GetByNameAsync(name);
            if (c != null) {
                _Logger.LogInformation($"{name} => {JToken.FromObject(c)}");

                string s = "Weapon stats =>\n";

                List<WeaponStatEntry> stats = await _Weapons.GetByCharacterID(c.ID);
                foreach (WeaponStatEntry entry in stats) {
                    if (entry.Kills > 0 || entry.Deaths > 0) {
                        s += $"\t{entry.WeaponID}: KD:{entry.KillDeathRatio}, {entry.HeadshotRatio}, {entry.KillsPerMinute}, {entry.Accuracy}\n";
                    }
                }

                _Logger.LogInformation(s);
            } else {
                _Logger.LogInformation($"{name} => null");
            }
        }

        public async Task GetItem(string name, int itemID) {
            Ps2Character? c = await _Characters.GetByNameAsync(name);
            if (c != null) {
                string s = "";

                List<WeaponStatEntry> stats = await _Weapons.GetByCharacterID(c.ID);
                foreach (WeaponStatEntry entry in stats) {
                    if (entry.WeaponID == itemID && (entry.Kills > 0 || entry.Deaths > 0)) {
                        s += string.Format("{0}: KD:{1,2:F2}, HSR:{2,2:F2}%, KPM:{3,2:F2}, ACC:{4,2:F2}%\n",
                            entry.WeaponID, entry.KillDeathRatio, entry.HeadshotRatio * 100, entry.KillsPerMinute, entry.Accuracy * 100);
                    }
                }

                _Logger.LogInformation(s);
            }
        }

    }
}
