using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Controllers.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static IEnumerable<SelectListItem> GetEnumSelectListO(this IHtmlHelper htmlHelper, Type enumType)
        {
            return htmlHelper.GetEnumSelectList(enumType).OrderBy(item => item.Text);
        }
    }
}
