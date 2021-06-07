using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeaponStats.Services.Db {

    /// <summary>
    /// Creates and updates the databases
    /// </summary>
    public interface IDbCreator {

        Task Execute();

    }
}
