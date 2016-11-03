using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DoEko.Models
{
    public static class EnumExtensions
    {
        public static string DisplayName(this Enum value)
        {
            if (value != null)
            {
                var EnumMemberDispAttrib = value.GetType().GetMember(Enum.GetName(value.GetType(), value))[0].GetCustomAttributes(typeof(DisplayAttribute), false)[0];

                return EnumMemberDispAttrib == null ? value.ToString() : ((DisplayAttribute)EnumMemberDispAttrib).Name;
            }
            else
            {
                return "Nie ustawiono";
            }
                
            

            //((DisplayAttribute)EnumMember.GetCustomAttributes(typeof(DisplayAttribute), false)[0]).Name;

            //Type enumType = value.GetType();
            //var enumValue = Enum.GetName(enumType, value);
            //MemberInfo member = enumType.GetMember(enumValue)[0];
            //var attrs = member.GetCustomAttributes(typeof(DisplayAttribute), false);
            //if (attrs.Any()) {
            //    var displayAttr = ((DisplayAttribute)attrs[0]);

            //    outString = displayAttr.Name;

            //    if (displayAttr.ResourceType != null) {
            //        outString = displayAttr.GetName();
            //    }
            //} else {
            //    outString = value.ToString();
            //}

            //return outString;

        }
    }
}
