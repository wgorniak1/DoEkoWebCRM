using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DoEko.Controllers.Extensions
{
    public static class ObjectExtensions
    {
        public static string GetPropValue(this Object obj, String name)
        {
            foreach (String part in name.Split('.'))
            {
                if (obj == null) { return null; }

                Type type = obj.GetType();

                if (part == type.Name) { continue; }

                PropertyInfo info = type.GetProperty(part);

                if (info == null) { return null; }

                obj = info.GetValue(obj, null);
            }

            if (obj.GetType().IsEnum)
            {
                return ((Enum)Enum.ToObject(obj.GetType(), obj)).DisplayName();
            }
            else
            {
                return obj.ToString();
            }
        }

        public static T GetPropValue<T>(this Object obj, String name)
        {
            Object retval = GetPropValue(obj, name);
            if (retval == null) { return default(T); }

            // throws InvalidCastException if types are incompatible
            return (T)retval;
        }
    }
}
