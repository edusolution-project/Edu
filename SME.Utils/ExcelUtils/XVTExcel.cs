using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace SME.Utils.ExcelUtils
{
    public class XVTExcel
    {
        /// <summary>
        /// Create workbook ny file path
        /// </summary>
        /// <param name="templateFilePath"></param>
        /// <returns></returns>
        /// <author>hungnd8</author>
        public static IXVTWorkbook OpenWorkbook(string templateFilePath)
        {
            return new XVTWorkbook(new ClosedXML.Excel.XLWorkbook(templateFilePath));
        }

        public static IXVTWorkbook OpenWorkbook(byte[] byteData)
        {
            return new XVTWorkbook(new ClosedXML.Excel.XLWorkbook(new MemoryStream(byteData)));
        }

        /// <summary>
        ///  Create workbook from file stream
        /// </summary>
        /// <param name="fileStream"></param>
        /// <returns></returns>
        /// <author>hungnd8</author>
        public static IXVTWorkbook OpenWorkbook(Stream fileStream)
        {
            try
            {
                return new XVTWorkbook(new ClosedXML.Excel.XLWorkbook(fileStream));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static IXVTWorkbook OpenWorkbook(Stream fileStream, MemoryStream fs)
        {
            try
            {
                return new XVTWorkbook(new ClosedXML.Excel.XLWorkbook(fileStream));
            }
            catch (Exception e)
            {

                if (e.ToString().Contains("Invalid Hyperlink"))
                {
                    fileStream.Position = 0;
                    fileStream.CopyTo(fs);
                    fs.Position = 0;
                    UriFixer.FixInvalidUri(fs, brokenUri => FixUri(brokenUri));
                    fs.Position = 0;
                    return new XVTWorkbook(new ClosedXML.Excel.XLWorkbook(fs));
                }
                else
                {
                    throw e;
                }
            }
        }

        private static Uri FixUri(string brokenUri)
        {
            return new Uri("http://broken-link/");
        }


        public static class UriFixer
        {
            public static void FixInvalidUri(Stream fs, Func<string, Uri> invalidUriHandler)
            {
                XNamespace relNs = "http://schemas.openxmlformats.org/package/2006/relationships";
                using (ZipArchive za = new ZipArchive(fs, ZipArchiveMode.Update, true))
                {
                    foreach (var entry in za.Entries.ToList())
                    {
                        if (!entry.Name.EndsWith(".rels"))
                            continue;
                        bool replaceEntry = false;
                        XDocument entryXDoc = null;
                        using (var entryStream = entry.Open())
                        {
                            try
                            {
                                entryXDoc = XDocument.Load(entryStream);
                                if (entryXDoc.Root != null && entryXDoc.Root.Name.Namespace == relNs)
                                {
                                    var urisToCheck = entryXDoc
                                        .Descendants(relNs + "Relationship")
                                        .Where(r => r.Attribute("TargetMode") != null && (string)r.Attribute("TargetMode") == "External");
                                    foreach (var rel in urisToCheck)
                                    {
                                        var target = (string)rel.Attribute("Target");
                                        if (target != null)
                                        {
                                            try
                                            {
                                                Uri uri = new Uri(target);
                                            }
                                            catch (UriFormatException)
                                            {
                                                Uri newUri = invalidUriHandler(target);
                                                rel.Attribute("Target").Value = newUri.ToString();
                                                replaceEntry = true;
                                            }
                                        }
                                    }
                                }
                            }
                            catch (XmlException)
                            {
                                continue;
                            }
                        }
                        if (replaceEntry)
                        {
                            var fullName = entry.FullName;
                            entry.Delete();
                            var newEntry = za.CreateEntry(fullName);
                            using (StreamWriter writer = new StreamWriter(newEntry.Open()))
                            using (XmlWriter xmlWriter = XmlWriter.Create(writer))
                            {
                                entryXDoc.WriteTo(xmlWriter);
                            }
                        }
                    }
                }
            }
        }

        public void ReadingExcel(Stream fileUpload)
        {
            IXVTWorkbook oBook = XVTExcel.OpenWorkbook(fileUpload);
            IXVTWorksheet firstSheet = oBook.GetSheet(1);
            var endRowUsed = firstSheet.Worksheet.LastRowUsed();
            string A1 = firstSheet.GetValueOfCell("A1").ToString();
            string A2 = firstSheet.GetValueOfCell("A2").ToString();
            string A3 = firstSheet.GetValueOfCell("A3").ToString();
            string A4 = firstSheet.GetValueOfCell("A4").ToString();
            string A5 = firstSheet.GetValueOfCell("A5").ToString();


            A1 = firstSheet.GetValueOfCell(1, 1).ToString();
            A2 = firstSheet.GetValueOfCell(2, 1).ToString();
            A3 = firstSheet.GetValueOfCell(3, 1).ToString();
            A4 = firstSheet.GetValueOfCell(4, 1).ToString();
            A5 = firstSheet.GetValueOfCell(5, 1).ToString();
        }
    }
}
