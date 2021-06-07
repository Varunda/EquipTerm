using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeaponStats.Models {

    public class DbOptions {

        /// <summary>
        /// URL of the PSQL server
        /// </summary>
        public string ServerUrl { get; set; } = "localhost";

        /// <summary>
        /// Database name to use
        /// </summary>
        public string DatabaseName { get; set; } = "ps2";

        /// <summary>
        /// Username to connect to the database with
        /// </summary>
        public string Username { get; set; } = "postgres";

        /// <summary>
        /// Password to connect to the database with
        /// </summary>
        public string Password { get; set; } = "password";

    }
}
