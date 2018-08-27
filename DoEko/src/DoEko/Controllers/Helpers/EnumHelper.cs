using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoEko.Controllers.Extensions;

namespace DoEko.Controllers.Helpers
{
    public class EnumHelper
    {

        public static Dictionary<int, string> GetKeyValuePairs(Type enumType)
        {
            Dictionary<int,string> keyValuePairs = new Dictionary<int, string>();

            foreach (var item in Enum.GetValues(enumType))
            {
                //Enum obj = (Enum)(Enum.ToObject(enumType, item));
                keyValuePairs.Add((int)item, ((Enum)item).DisplayName());
            }

            return keyValuePairs;

        }
    }
}
