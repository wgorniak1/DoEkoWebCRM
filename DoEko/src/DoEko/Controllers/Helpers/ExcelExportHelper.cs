using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Controllers.Helpers
{
    public class ExcelExportHelper
    {
        public Stream Stream { get; private set; }
        private SpreadsheetDocument spreadsheetDocument;
        private WorkbookPart workbookPart;

        public ExcelExportHelper()
        {
            // Create a spreadsheet document.
            Stream = new MemoryStream();
            spreadsheetDocument = SpreadsheetDocument.Create(stream: Stream, type: SpreadsheetDocumentType.Workbook);

            // Add a WorkbookPart to the document.
            workbookPart = spreadsheetDocument.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();
            
        }

        public ExcelExportHelper(string fileName)
        {
            // Create a spreadsheet document.
            Stream = new FileStream(path: fileName, mode: FileMode.Open, access: FileAccess.ReadWrite);
            
            spreadsheetDocument = SpreadsheetDocument.Open(stream: Stream, isEditable: true);

            // Add a WorkbookPart to the document.
            this.workbookPart = spreadsheetDocument.WorkbookPart;
            if (this.workbookPart is null)
            {
                this.workbookPart = spreadsheetDocument.AddWorkbookPart();
                this.workbookPart.Workbook = new Workbook();
            }
            
        }

        private void setStyles()
        {
        }

        public void Add(DataTable table)
        {

            if (table.Columns.Count == 0 || table.Rows.Count == 0)
            {
                return;
            }

            var sheet = this.InsertWorksheet("aa");

            //header
            var header = new Row();
            foreach (DataColumn item in table.Columns)
            {
                header.RowIndex = 1;
                header.Append(ExcelExportHelper.AddCellWithText(item.ColumnName));
            }
            sheet.AppendChild<Row>(header);
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
                        var value = ExcelExportHelper.AddValue(tr[item.ColumnName]);
                        row.Append(value);
                    }
                    catch (Exception)
                    {   
                    }
                }
                
                sheet.AppendChild<Row>(row);
            }

            Worksheet wks = (Worksheet)sheet.Parent;
            string range = "W1K1:W" + table.Rows.Count + "K" + table.Columns.Count;
            wks.AppendChild<AutoFilter>(new AutoFilter() { Reference = range });

            
        }
        // Given a WorkbookPart, inserts a new worksheet.
        public SheetData InsertWorksheet(string name)
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

        public void FinalizeDocument()
        {
            spreadsheetDocument.WorkbookPart.Workbook.Save();
            spreadsheetDocument.Close();
        }

        //public void CreateExcelFile(string name)
        //{

            
        //    int rowindex = 1;
        //    foreach (var emp in lstEmps)
        //    {
        //        Row row = new Row();
        //        row.RowIndex = (UInt32)rowindex;

        //        if (rowindex == 1) //Header 
        //        {
        //            row.AppendChild(AddCellWithText("Name"));
        //            row.AppendChild(AddCellWithText("Email"));
        //        }
        //        else //Data 
        //        {
        //            row.AppendChild(AddCellWithText(emp.Name));
        //            row.AppendChild(AddCellWithText(emp.Email));
        //        }

        //        sheetData.AppendChild(row);
        //        rowindex++;
        //    }
            
        //}
   
        public static Cell AddValue(object value)
        {
            if (value.GetType() == typeof(DateTime))
            {
                return new Cell()
                {
                    CellValue = new CellValue(((DateTime)value).ToString("s")),
                    DataType = new EnumValue<CellValues>(CellValues.Date)
                };
            }
            else if (value.GetType() == typeof(Decimal))
            {
                return new Cell()
                {
                    CellValue = new CellValue(((Decimal)value).ToString()),
                    DataType = CellValues.Number
                };
            }
            else if (value.GetType() == typeof(double))
            {
                return new Cell()
                {
                    CellValue = new CellValue(((Decimal)value).ToString()),
                    DataType = CellValues.Number
                };
            }
            else if (value.GetType() == typeof(int))
            {
                return new Cell()
                {
                    CellValue = new CellValue(((int)value).ToString()),
                    DataType = CellValues.Number
                };
            }
            else if (value.GetType() == typeof(short))
            {
                return new Cell()
                {
                    CellValue = new CellValue(((short)value).ToString()),
                    DataType = CellValues.Number
                };
            }
            else if (value.GetType() == typeof(Uri))
            {
                return new Cell()
                {
                    CellValue = new CellValue(((Uri)value).AbsoluteUri.ToString()),
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
        public static Cell AddCellWithText(string text)
        {
            Cell c1 = new Cell();
            c1.DataType = CellValues.String;

            InlineString inlineString = new InlineString();
            Text t = new Text();
            t.Text = text;
            inlineString.AppendChild(t);

            c1.AppendChild(inlineString);

            return c1;
        }

    }
}
