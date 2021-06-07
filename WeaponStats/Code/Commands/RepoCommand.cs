using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeaponStats.Models.Census;
using WeaponStats.Services.Repositories;

namespace WeaponStats.Code.Commands {

    [Command]
    public class RepoCommand {

        private readonly ILogger<RepoCommand> _Logger;
        private readonly IItemCategoryRepository _ItemCategoryRepository;
        private readonly IItemTypeRepository _ItemTypeRepository;

        public RepoCommand(IServiceProvider services) {
            _Logger = services.GetRequiredService<ILogger<RepoCommand>>();

            _ItemCategoryRepository = services.GetRequiredService<IItemCategoryRepository>();
            _ItemTypeRepository = services.GetRequiredService<IItemTypeRepository>();
        }

        public async Task CategoryList() {
            List<Ps2ItemCategory> entries = await _ItemCategoryRepository.GetAll();

            _Logger.LogInformation($"Loaded {entries.Count} categories: {string.Join(", ", entries.Select(iter => $"{iter.ID}/{iter.Name}"))}");
        }

        public async Task TypeList() {
            List<Ps2ItemType> types = await _ItemTypeRepository.GetAll();

            _Logger.LogInformation($"Loaded {types.Count} types: {string.Join(", ", types.Select(iter => $"{iter.ID}/{iter.Name}"))}");
        }

    }
}
