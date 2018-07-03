using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DoEko.Controllers.Extensions
{
    public static class EnumExtensions
    {
        public static string DisplayShortName(this Enum value)
        {
            if (value != null && Enum.GetName(value.GetType(), value) != null)
            {
                var EnumMemberDispAttrib = value.GetType().GetMember(Enum.GetName(value.GetType(), value))[0].GetCustomAttributes(typeof(DisplayAttribute), false)[0];

                return EnumMemberDispAttrib == null ? value.ToString() : ((DisplayAttribute)EnumMemberDispAttrib).ShortName;
            }
            else
            {
                return "";
            }
        }

        public static string DisplayName(this Enum value)
        {
            if (value != null && Enum.GetName(value.GetType(), value) != null)
            {
                var EnumMemberDispAttrib = value.GetType().GetMember(Enum.GetName(value.GetType(), value))[0].GetCustomAttributes(typeof(DisplayAttribute), false)[0];

                return EnumMemberDispAttrib == null ? value.ToString() : ((DisplayAttribute)EnumMemberDispAttrib).Name;
            }
            else
            {
                return "";
            }
                
        }
    }
}
