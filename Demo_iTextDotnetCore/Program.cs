using System;
using System.IO;

using System.Diagnostics;
using iText.Barcodes;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Geom;
using Spire.Pdf;

namespace Demo_iTextDotnetCore
{
    class Program
    {
        static void Main(string[] args)
        {
            string exportFolder = AppContext.BaseDirectory;
            string exportFile = System.IO.Path.Combine(exportFolder, "Test.pdf");
            string exportImage = System.IO.Path.Combine(exportFolder, "bc.png");

            using (PdfWriter pdfWriter = new PdfWriter(exportFile))
            {
                using (iText.Kernel.Pdf.PdfDocument pdf = new iText.Kernel.Pdf.PdfDocument(pdfWriter))
                {
                    Rectangle envelope = new Rectangle(3016, 1327);
                    PageSize ps = new PageSize(envelope);
                    //ps.ApplyMargins(0, 0, 0, 0, true);
                    Document document = new Document(pdf, ps);
                    document.SetMargins(0, 0, 0, 0);
                    Paragraph text = new Paragraph("Signature").SetTextAlignment(TextAlignment.CENTER).SetFontSize(200);
                    Paragraph num = new Paragraph("011120").SetTextAlignment(TextAlignment.CENTER).SetFontSize(200);
                    Barcode128 barcode128 = new Barcode128(pdf);
                    barcode128.SetCodeType(Barcode128.CODE_C);
                    barcode128.SetCode("011120");
                    barcode128.FitWidth(2800);
                    barcode128.SetBarHeight(700);
                    barcode128.SetAltText("");

                    Image barcodeImage = new Image(barcode128.CreateFormXObject(pdf));
                    barcodeImage.SetHorizontalAlignment(HorizontalAlignment.CENTER);
                    document.Add(text);
                    document.Add(barcodeImage);
                    document.Add(num);
                }
            }
            Spire.Pdf.PdfDocument document1 = new Spire.Pdf.PdfDocument();
            document1.LoadFromFile(exportFile);
            System.Drawing.Image img = document1.SaveAsImage(0);
            img.Save(exportImage);
            document1.SaveToFile(System.IO.Path.Combine(exportFolder, "bc.svg"), FileFormat.SVG);

            Process proc = new Process();
            proc.StartInfo.FileName = exportImage;
            proc.StartInfo.UseShellExecute = true;
            proc.Start();
            Process.Start("explorer.exe", "/select, " + exportImage);
        }
    }
}
