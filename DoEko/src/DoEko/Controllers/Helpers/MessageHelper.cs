using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Controllers.Helpers
{
    public enum ProjectNumber
    {
        ParentProjectNotFound = 1,
        Other = 999
    }

    public static class MessageHelper
    {
        
        public static string Text(int Id)
        {
            switch (Id)
            {
                case (int)ProjectNumber.ParentProjectNotFound:
                    return "Nie znaleziono projektu o numerze ";
                default:
                    return "";
            }
        }
    }
}
