using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeaponStats.Models.Census;
using WeaponStats.Services.Db;
using WeaponStats.Services.Repositories;

namespace WeaponStats.Code.Commands {

    [Command]
    public class ItemCommand {

        private readonly ILogger<ItemCommand> _Logger;
        private readonly IItemRepository _ItemRepository;
        private readonly IItemStatDbStore _ItemStats;

        public ItemCommand(IServiceProvider services) {
            _Logger = services.GetRequiredService<ILogger<ItemCommand>>();
            _ItemRepository = services.GetRequiredService<IItemRepository>();
            _ItemStats = services.GetRequiredService<IItemStatDbStore>();
        }

        public async Task Get(int itemID) {
            Ps2Item? item = await _ItemRepository.GetByID(itemID);
            if (item != null) {
                _Logger.LogInformation($"{JToken.FromObject(item)}");
            } else {
                _Logger.LogInformation($"No item {itemID}");
            }
        }

        public async Task Stat(int itemID) {
            Ps2Item? item = await _ItemRepository.GetByID(itemID);
            if (item != null) {
                Quartile kd = Quartile.Create(await _ItemStats.GetKillDeathRatioByItemID(itemID));
                Quartile kpm = Quartile.Create(await _ItemStats.GetKillsPerMinuteByItemID(itemID));
                Quartile acc = Quartile.Create(await _ItemStats.GetAccuracyByItemID(itemID));
                Quartile hsr = Quartile.Create(await _ItemStats.GetHeadshotRatioByItemID(itemID));

                string s = $"\nStats for {item.Name}\n";
                s += $"\nStat\tMin\tQ1\tMedian\tQ3\tMax\n";
                s += $"kpm\t{kpm.Min}\t{kpm.QuartileOne}\t{kpm.Median}\t{kpm.QuartileThree}\t{kpm.Max}\n";
                s += $"kd\t{kd.Min}\t{kd.QuartileOne}\t{kd.Median}\t{kd.QuartileThree}\t{kd.Max}\n";
                s += $"acc\t{acc.Min}\t{acc.QuartileOne}\t{acc.Median}\t{acc.QuartileThree}\t{acc.Max}\n";
                s += $"hsr\t{hsr.Min}\t{hsr.QuartileOne}\t{hsr.Median}\t{hsr.QuartileThree}\t{hsr.Max}\n";

                _Logger.LogDebug(s);
            }
        }


    }
}
