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
        public const string fieldDelimeter = " MERGEFIELD ";
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
                        catch (Exception exc)
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
                .InsertAfter(altChunk, doc.MainDocumentPart.Document.Body.Elements<Paragraph>().Last());

            doc.MainDocumentPart.Document.Save();
            
            return doc;
        }
    }
}
