using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DoEko.Controllers.Extensions
{
    public static class DataTableExtensions
    {
        public static MemoryStream AsExcel(this DataTable table)
        {
            // Create a spreadsheet document.
            MemoryStream memoryStream = new MemoryStream();
            SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(stream: memoryStream, type: SpreadsheetDocumentType.Workbook);

            // Add a WorkbookPart to the document.
            WorkbookPart workbookPart = spreadsheetDocument.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();

            if (table.Columns.Count == 0 || table.Rows.Count == 0)
            {
                throw new ArgumentNullException();
            }

            SheetData sheetData = DataTableExtensions.Worksheet(workbookPart, table.TableName ?? string.Empty);

            //Header
            sheetData.AppendChild<Row>(DataTableExtensions.Header(table));
            //Data
            UInt32 index = 2;
            foreach (DataRow tr in table.Rows)
            {
                var row = new Row
                {
                    RowIndex = index++
                };

                foreach (DataColumn item in table.Columns)
                {
                    try
                    {                        
                        var value = AddCellWithValue(tr[item.ColumnName]);
                        row.Append(value);
                    }
#pragma warning disable CS0168 // Variable is declared but never used
                    catch (Exception exc)
#pragma warning restore CS0168 // Variable is declared but never used
                    {   
                    }
                }

                sheetData.AppendChild<Row>(row);
            }

            //Autofilter
            Worksheet wks = (Worksheet)(sheetData.Parent);
            string range = "A1:" + ColumnAddress(table.Columns.Count) + table.Rows.Count.ToString();
            wks.AppendChild<AutoFilter>(new AutoFilter() { Reference = range });

            //
            spreadsheetDocument.WorkbookPart.Workbook.Save();
            spreadsheetDocument.Close();

            return memoryStream;
        }
        private static string ColumnAddress(int col)
        {
            if (col <= 26)
            {
                return Convert.ToChar(col + 64).ToString();
            }
            int div = col / 26;
            int mod = col % 26;
            if (mod == 0) { mod = 26; div--; }
            return ColumnAddress(div) + ColumnAddress(mod);
        }

        private static Row Header(DataTable table)
        {
            var header = new Row();
            foreach (DataColumn item in table.Columns)
            {
                header.Append(AddCellWithValue(item.Caption ?? item.ColumnName));
            }
            return header;
        }

        // Given a WorkbookPart, inserts a new worksheet.
        private static SheetData Worksheet(WorkbookPart workbookPart, string name)
        {
            // Add a new worksheet part to the workbook.
            WorksheetPart newWorksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            newWorksheetPart.Worksheet = new Worksheet(new SheetData());
            newWorksheetPart.Worksheet.Save();

            Sheets sheets = workbookPart.Workbook.GetFirstChild<Sheets>();
            if (sheets is null)
            {
                sheets = workbookPart.Workbook.AppendChild<Sheets>(new Sheets());
            }

            string relationshipId = workbookPart.GetIdOfPart(newWorksheetPart);

            // Get a unique ID for the new sheet.
            uint sheetId = 1;
            if (sheets.Elements<Sheet>().Count() > 0)
            {
                sheetId = sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;
            }

            string sheetName = "Arkusz" + sheetId;

            // Append the new worksheet and associate it with the workbook.
            Sheet sheet = new Sheet() { Id = relationshipId, SheetId = sheetId, Name = string.IsNullOrEmpty(name) ? sheetName : name };
            sheets.Append(sheet);
            workbookPart.Workbook.Save();

            return newWorksheetPart.Worksheet.GetFirstChild<SheetData>();
        }

        private static Cell AddCellWithValue(object value)
        {
            //null value = empty string
            if (value is null)
            {
                return AddCellWithValue(string.Empty);
            }

            //match cell type with value type
            Type type = value.GetType();

            if (type == typeof(DateTime))
            {
                DateTime v = (DateTime)value;
                return new Cell()
                {
                    CellValue = new CellValue(v.ToString()),
                    DataType = CellValues.String//new EnumValue<CellValues>(CellValues.Date)
                };
            }
            else if (type == typeof(Decimal))
            {
                Decimal v = (Decimal)value;
                return new Cell()
                {
                    CellValue = new CellValue(v != 0 ? v.ToString() : ""),
                    DataType = CellValues.Number
                };
            }
            else if (type == typeof(double))
            {
                double v = (double)value;
                return new Cell()
                {
                    CellValue = new CellValue(v != 0 ? v.ToString("") : ""),
                    DataType = CellValues.Number
                };
            }
            else if (type == typeof(long))
            {
                long v = (long)value;
                return new Cell()
                {
                    CellValue = new CellValue(v != 0 ? v.ToString() : ""),
                    DataType = CellValues.Number
                };
            }
            else if (type == typeof(int))
            {
                int v = (int)value;
                return new Cell()
                {
                    CellValue = new CellValue(v != 0 ? v.ToString() : ""),
                    DataType = CellValues.Number
                };
            }
            else if (type == typeof(short))
            {
                short v = (short)value;
                return new Cell()
                {
                    CellValue = new CellValue(v != 0 ? v.ToString() : ""),
                    DataType = CellValues.Number
                };
            }
            else if (type == typeof(Uri))
            {
                string formula = "HYPERLINK("+'"'+ ((Uri)value).AbsoluteUri + '"'+", "+'"'+"Link"+'"'+")";
                return new Cell()
                {
                    CellValue = new CellValue("Link"),
                    CellFormula = new CellFormula(formula),
                    DataType = CellValues.String
                };
            }
            else
            {
                Cell c = new Cell();
                c.DataType = CellValues.InlineString;
                InlineString inlineString = new InlineString();
                Text t = new Text
                {
                    Text = (string)value.ToString()
                };
                inlineString.AppendChild(t);
                c.AppendChild(inlineString);
                return c;
            };
        }
    }
}