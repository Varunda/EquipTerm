using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeaponStats.Services.Environment {

    public interface ICommandBus {

        Task Execute(string command);

    }
}
