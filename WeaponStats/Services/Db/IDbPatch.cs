using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeaponStats.Services.Db;

namespace WeaponStats.Services.Db {

    public interface IDbPatch {

        int MinVersion { get; }

        string Name { get; }

        Task Execute(IDbHelper helper);

    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class PatchAttritube : Attribute {
    
    }

}
