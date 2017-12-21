using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Globalization;
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

                if (part.Contains("["))
                {
                    
                    PropertyInfo info = type.GetProperty(part.Substring(0, part.Length - 3));

                    if (info == null) { return null; }

                    obj = info.GetValue(obj, null);

                    if (obj is IList)
                    {
                        var x = int.Parse(part.Substring(part.Length - 2, 1));
                        if (x > (((IList)obj).Count - 1)) { return null; }
                        obj = ((IList)obj)[x];
                    }

                }
                else
                {
                    PropertyInfo info = type.GetProperty(part);

                    if (info == null) { return null; }

                    obj = info.GetValue(obj, null);

                }

            }
            if (obj == null){
                return string.Empty;
            }
            else if (obj.GetType().IsEnum)
            {
                return ((Enum)Enum.ToObject(obj.GetType(), obj)).DisplayName();
            }
            else if (obj is Double)
            {
                return ((Double)obj).ToString("N2", CultureInfo.CurrentUICulture);
            }
            else if (obj is Boolean)
            {
                return ((Boolean)obj).AsYesNo();
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

        public static DataTable AsDataTable<T>(this IEnumerable<T> list)
        {
            Type type = typeof(T);
            var properties = type.GetProperties();
            //Table definition
            DataTable dataTable = new DataTable();
            foreach (PropertyInfo info in properties)
            {
                dataTable.Columns.Add(new DataColumn()
                {
                    ColumnName = info.Name,
                    DataType = Nullable.GetUnderlyingType(info.PropertyType) ?? info.PropertyType,
                    Caption = info.GetCustomAttribute<DisplayAttribute>() is null ? "" : info.GetCustomAttribute<DisplayAttribute>().Name
                });
            }
            //Table Data
            foreach (T entity in list)
            {
                object[] values = new object[properties.Length];
                for (int i = 0; i < properties.Length; i++)
                {
                    values[i] = properties[i].GetValue(entity);
                }

                dataTable.Rows.Add(values);
            }

            return dataTable;
        }
    }
}
