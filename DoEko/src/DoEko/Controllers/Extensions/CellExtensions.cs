using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Controllers.Extensions
{
    public static class CellExtensions
    {
        public static string RetrieveValue (this Cell cell, SpreadsheetDocument doc)
        {
            string value = cell.CellValue != null ? cell.CellValue.InnerText : "";

            return (cell.DataType != null && cell.DataType.Value == CellValues.SharedString) ?
                doc.WorkbookPart.SharedStringTablePart.SharedStringTable.ChildElements.GetItem(int.Parse(value)).InnerText : 
                value;
        }
    }
}
