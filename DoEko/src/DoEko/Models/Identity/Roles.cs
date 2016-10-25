using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Models.Identity
{
    public static class Roles
    {
        private static readonly string[] roles = { "Administrator", "Użytkownik", "Inspektor" };
        public const string Admin ="Administrator";
        public const string User = "Użytkownik";
        public const string Inspector = "Inspektor";

        public static IEnumerable<string> All { get { return roles; } }
    }
}
