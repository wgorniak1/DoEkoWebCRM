using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DoEko.Models.DoEko;
using DoEko.ViewModels.ReportsViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Controllers.Extensions
{
    public static class WordprocessingDocumentExtensions
    {
        public const string fieldDelimeter = "MERGEFIELD";

        public static WordprocessingDocument MergeFields(this WordprocessingDocument doc, object data)
        {
            var splitter = new[] { ' ', '"' };
            const string tmergefield = "MERGEFIELD";
            var tmergeFields = doc.MainDocumentPart.HeaderParts.Cast<OpenXmlPart>()
                .Concat(doc.MainDocumentPart.FooterParts.Cast<OpenXmlPart>())
                .Concat(new OpenXmlPart[] { doc.MainDocumentPart })
                .SelectMany(x => x.RootElement.Descendants<SimpleField>().Select(sf => new { text = sf.Instruction.Value, el = (OpenXmlElement)sf })
                                .Concat(x.RootElement.Descendants<FieldCode>().Select(fc => new { text = fc.Text, el = (OpenXmlElement)fc })))
                .Select(a => new { words = a.text.Split(splitter, StringSplitOptions.RemoveEmptyEntries), el = a.el })
                .Where(a => tmergefield.Equals(a.words.FirstOrDefault(), StringComparison.OrdinalIgnoreCase))
                .ToLookup(k => string.Join(" ", k.words.Skip(1).TakeWhile(i => i != "\\*")), v => v.el);
            foreach (var t in tmergeFields)
            {
                //
                var fieldValue = data.GetPropValue(t.Key) == null ? "" : data.GetPropValue(t.Key);

                foreach (var tag in t.OfType<SimpleField>())
                {
                    //insert Text right before "current tag"
                    var newText = new Run(new Text(fieldValue));
                    tag.InsertBeforeSelf(newText);

                    tag.Remove();
                }

                foreach (var tag in t.OfType<FieldCode>())
                {
                    //insert Text right before "run:begin"
                    var newText = new Run(new Text(fieldValue));

                    //
                    var runBegin = tag.Parent.PreviousSibling();

                    runBegin.InsertBeforeSelf(newText);

                    foreach (var runElement in newText.ElementsAfter().ToArray())
                    {

                        if (runElement.Descendants<FieldChar>().Where(fc => fc.FieldCharType == FieldCharValues.End).Any())
                        {
                            runElement.Remove();
                            break;
                        }
                        else
                        {
                            runElement.Remove();
                        }
                    }
                }

            }
            //foreach (var runBegin in doc.MainDocumentPart.RootElement.Descendants<Run>()
            //    .Where(r => r.Descendants<FieldChar>().Where(fc => fc.FieldCharType == FieldCharValues.Begin).Any() &&
            //                r.NextSibling<Run>().Descendants<FieldCode>().Where(fc => fc.Text.Contains(fieldDelimeter)).Any()).ToArray())
            //{
            //    Run runSeparate;
            //    //1. take all run elements between "begin" and "separate" 
            //    //   to concatenate all mergefield name parts = this wiil give us mergefield name
            //    string fieldName = "";

            //    //Run runMerge = runBegin.NextSibling<Run>();

            //    //fieldName = runMerge.Descendants<FieldCode>().Where(fc => fc.Text.Contains(fieldDelimeter)).First().Text;
            //    foreach (Run run in runBegin.ElementsAfter())
            //    {
            //        if (run.Descendants<FieldChar>().Where(fc => fc.FieldCharType == FieldCharValues.Separate).Any())
            //        {
            //            runSeparate = run;
            //            break;
            //        }
            //        fieldName = fieldName + run.Descendants<FieldCode>().First().Text;
            //    }
            //    fieldName = fieldName.Substring(fieldName.LastIndexOf(fieldDelimeter, System.StringComparison.Ordinal) + fieldDelimeter.Length).Trim().Split(' ').First();


            //    //2. Calculate data value and insert new run element with that data
            //    var fieldValue = data.GetPropValue(fieldName) == null ? "" : data.GetPropValue(fieldName);
                
            //    //insert Text right before "run:begin"
            //    var newText = new Run(new Text(fieldValue));
            //    runBegin.InsertBeforeSelf(newText);

            //    //3. Delete all mergefield configuration
            //    foreach (var runElement in newText.ElementsAfter().ToArray())
            //    {

            //        if (runElement.Descendants<FieldChar>().Where(fc => fc.FieldCharType == FieldCharValues.End).Any())
            //        {
            //            runElement.Remove();
            //            break;
            //        }
            //        else
            //        {
            //            runElement.Remove();
            //        }
            //    }
            //}

            return doc;
        }
        public static WordprocessingDocument MailMerge(this WordprocessingDocument doc, object data)
        {
            var fieldCodes = doc.MainDocumentPart.RootElement.Descendants<FieldCode>().Where(fc => fc.Text.Contains(fieldDelimeter)).ToArray();
            var simpleFields = doc.MainDocumentPart.RootElement.Descendants<SimpleField>().Where(fc => fc.Instruction.HasValue && fc.Instruction.InnerText.Contains(fieldDelimeter)).ToArray();

            for (int i = 0; i < simpleFields.Count(); i++)
            {
                var fieldCode = simpleFields[i];
                var fieldName = fieldCode.Instruction.InnerText.Substring(fieldCode.Instruction.InnerText.LastIndexOf(fieldDelimeter, System.StringComparison.Ordinal) + fieldDelimeter.Length).Trim().Split(' ').First();
                var fieldValue = data.GetPropValue(fieldName);

                foreach (Run run in doc.MainDocumentPart.Document.Descendants<Run>())
                {
                    var text = "«" + fieldName + "»";
                    foreach (Text txtFromRun in run.Descendants<Text>().Where(a => text.StartsWith(a.Text)))
                    {
                        txtFromRun.Text = fieldValue == null ? "" : fieldValue.ToString();
                    }
                }
            }

            for (int i = 0; i < fieldCodes.Count(); i++)
            {
                var fieldCode = fieldCodes[i];
                var fieldName = fieldCode.Text.Substring(fieldCode.Text.LastIndexOf(fieldDelimeter, System.StringComparison.Ordinal) + fieldDelimeter.Length).Trim().Split(' ').First();
                var fieldValue = data.GetPropValue(fieldName);

            //    if (fieldValue != null)
            //    {
            //        fieldCode.Parent.ReplaceChild();
            //    }
            //}

            //foreach (FieldCode field in doc.MainDocumentPart.RootElement.Descendants<FieldCode>().Where(fc=>fc.Text.Contains(fieldDelimeter)))
            //{
                //var fieldNameStart = field.Text.LastIndexOf(fieldDelimeter, System.StringComparison.Ordinal);
                //var fieldName = field.Text.Substring(fieldNameStart + fieldDelimeter.Length).Trim().Split(' ').First();

                //var fieldValue = data.GetPropValue(fieldName);

                foreach (Run run in doc.MainDocumentPart.Document.Descendants<Run>())
                {
                    var text = "«" + fieldName + "»";
                    foreach (Text txtFromRun in run.Descendants<Text>().Where(a => text.StartsWith(a.Text)))
                    {
                        txtFromRun.Text = fieldValue == null ? "" : fieldValue.ToString();
                    }
                }

            }

            return doc;

        }

        public static WordprocessingDocument MergePictures(this WordprocessingDocument doc, InvestmentViewModel inv)
        {
            const string imgPlaceHolder = "Picture";
            var mainDoc = doc.MainDocumentPart;
            int count = mainDoc.ExternalRelationships.Count();

            for (int i = 0; i < count; i++)
            {
                ExternalRelationship extRelation = mainDoc.ExternalRelationships.ElementAt(0);
                if (extRelation.Uri.OriginalString.StartsWith(imgPlaceHolder))
                {
                    string relationId = extRelation.Id;
                    string relationType = extRelation.RelationshipType;

                    Uri pictureUri = inv.Picture(extRelation.Uri.OriginalString);
                    if (pictureUri != null)
                    {
                        //Replace id with real url
                        mainDoc.DeleteExternalRelationship(extRelation);
                        mainDoc.AddExternalRelationship(relationType, pictureUri, relationId);
                    }
                    else
                    {
                        //Remove picture control and url if picture not uploaded to azure storage
                        try
                        {
                            mainDoc.Document.Body
                                .Descendants<DocumentFormat.OpenXml.Vml.ImageData>()
                                .Where(img => img.RelationshipId == relationId)
                                .FirstOrDefault()
                                .Parent.Parent.Remove();

                            mainDoc.DeleteExternalRelationship(extRelation);

                        }
                        catch (Exception )
                        {
                            //
                            //Console.Write(exc);
                        }
                    }
                }
            }

            return doc;
        }

        public static WordprocessingDocument MergeStream(this WordprocessingDocument doc, Stream stream)
        {
            string AltChunkId = "AltChunkId"+Guid.NewGuid().ToString();

            stream.Position = 0;

            AlternativeFormatImportPart chunk = doc.MainDocumentPart.AddAlternativeFormatImportPart(AlternativeFormatImportPartType.WordprocessingML, AltChunkId);

            chunk.FeedData(stream);

            AltChunk altChunk = new AltChunk();

            altChunk.Id = AltChunkId;

            doc.MainDocumentPart.Document.Body
                .InsertAfter(altChunk, doc.MainDocumentPart.Document.Body.LastChild);
                //.InsertAfter(altChunk, doc.MainDocumentPart.Document.Body.Elements<Paragraph>().Last());

            doc.MainDocumentPart.Document.Save();
            
            return doc;
        }
    }
}
