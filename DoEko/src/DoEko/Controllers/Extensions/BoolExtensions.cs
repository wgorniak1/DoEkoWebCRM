using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Controllers.Extensions
{
    public static class BoolExtensions
    {
        public static string AsYesNo (this bool value)
        {
            return value ? "Tak" : "Nie";
        }
    }
}
