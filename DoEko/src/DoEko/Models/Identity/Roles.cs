using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Models.Identity
{
    public static class Roles
    {
        private static readonly string[] roles = { "Admin", "Inspector", "ContractManager", "Reader", "Writer" };
        public static string Admin { get { return roles[0]; } }
        public static string Inspector { get { return roles[1]; } }

        public static IEnumerable<string> All { get { return roles; } }
    }
}
