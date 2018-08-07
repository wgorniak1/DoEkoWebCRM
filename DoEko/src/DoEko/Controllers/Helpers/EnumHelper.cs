using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Controllers.Helpers
{
    public class EnumHelper
    {
        public static IDictionary<int, string> GetKeyValuePairs(Type enumType)
        {
            Dictionary<int, string> keyValuePairs = new Dictionary<int, string>();

            foreach (int item in Enum.GetValues(enumType))
            {
                keyValuePairs.Add(item, Enum.GetName(enumType, item));
            }

            return keyValuePairs;

        }
    }
}
