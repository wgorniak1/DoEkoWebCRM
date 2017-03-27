using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Controllers.Extensions
{
    public static class SpreadsheetDocumentExtensions
    {
        public static System.Data.DataTable RetrieveDataTable (this SpreadsheetDocument doc, bool withHeader = false)
        {
            System.Data.DataTable dt = new System.Data.DataTable();

            //Read the first Sheets 
            Sheet sheet = doc.WorkbookPart.Workbook.Sheets.GetFirstChild<Sheet>();
            Worksheet worksheet = (doc.WorkbookPart.GetPartById(sheet.Id.Value) as WorksheetPart).Worksheet;
            IEnumerable<Row> rows = worksheet.GetFirstChild<SheetData>().Descendants<Row>();

            //1st row - Header
            int rowsToSkip = 0;
            int columnIndex = 1;
            if (withHeader)
            {
                foreach (Cell cell in rows.First().Descendants<Cell>())
                {
                    var colunmName = cell.RetrieveValue(doc);

                    if (string.IsNullOrEmpty(colunmName)) colunmName = "Field" + columnIndex++;
                    int colNo = 1;
                    while (dt.Columns.Contains(colunmName))
                    {
                        colunmName = colunmName + '[' + colNo.ToString() + ']';
                        colNo++;
                    }

                    dt.Columns.Add(colunmName);
                }
                //
                rowsToSkip = 1;
            }
            //rest - Data
            foreach (Row row in rows.Skip(rowsToSkip))
            {
                dt.Rows.Add();
                columnIndex = 0;
                foreach (Cell cell in row.Descendants<Cell>())
                {
                    dt.Rows[dt.Rows.Count - 1][columnIndex] = cell.RetrieveValue(doc);
                    columnIndex++;
                }
            }
            
            return dt;
        }
    }
}
