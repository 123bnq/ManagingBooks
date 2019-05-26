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

            //Document doc = new Document(PageSize.A4, 10, 10, 10, 10);
            //PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(exportFile, FileMode.Create));
            //Paragraph elements = new Paragraph("bla :)");
            //doc.Open();
            //doc.Add(elements);
            //doc.Close();
            //Barcode128 barcode128 = new Barcode128();
            //barcode128.CodeType = Barcode.CODE128;
            //barcode128.Code = "012345";

            //barcode128.TextAlignment = Element.ALIGN_CENTER;
            //barcode128.ChecksumText = true;
            //barcode128.GenerateChecksum = true;
            //barcode128.Font = new Font(Font.FontFamily.COURIER, 20f).BaseFont;
            //System.Drawing.Bitmap bm = new System.Drawing.Bitmap(barcode128.CreateDrawingImage(System.Drawing.Color.Black, System.Drawing.Color.White));

            //bm.Save(exportImage);


            using (PdfWriter pdfWriter = new PdfWriter(exportFile))
            {
                using (iText.Kernel.Pdf.PdfDocument pdf = new iText.Kernel.Pdf.PdfDocument(pdfWriter))
                {
                    Rectangle envelope = new Rectangle(3000, 1000);
                    PageSize ps = new PageSize(envelope);
                    //ps.ApplyMargins(0, 0, 0, 0, true);
                    Document document = new Document(pdf, ps);
                    document.SetMargins(0, 0, 0, 0);
                    Paragraph text = new Paragraph("Signature").SetTextAlignment(TextAlignment.CENTER).SetFontSize(150);
                    Barcode128 barcode128 = new Barcode128(pdf);
                    barcode128.SetCodeType(Barcode128.CODE_C);
                    barcode128.SetCode("011120");
                    Image barcodeImage = new Image(barcode128.CreateFormXObject(pdf));
                    barcodeImage.SetHorizontalAlignment(HorizontalAlignment.CENTER);
                    barcodeImage.Scale(25F, 22F);
                    document.Add(text);
                    document.Add(barcodeImage);
                    //Table table = new Table(5, false);
                    //table.SetWidth(UnitValue.CreatePercentValue(100));
                    //Cell[] cellArray = new Cell[100];
                    //for (int i = 0; i < cellArray.Length; i++)
                    //{
                    //    cellArray[i] = new Cell();
                    //    //cellArray[i].SetBorder(new SolidBorder(ColorConstants.WHITE, 0));
                    //    cellArray[i].Add(barcodeImage);
                    //    //cellArray[i].SetHorizontalAlignment(iText.Layout.Properties.HorizontalAlignment.CENTER);
                    //    table.AddCell(cellArray[i]);
                    //}

                    ////table.SetBorder(new SolidBorder(1));
                    //document.Add(table);

                    //barcode128.PlaceBarcode(new PdfCanvas(pdf.GetFirstPage()), ColorConstants.BLUE, ColorConstants.GREEN);
                    //PdfCanvas pdfCanvas = new PdfCanvas(pdf.GetFirstPage());
                    //PdfFormXObject pdfFormXObject = barcode128.CreateFormXObject(pdf);
                    //float scale = 1;
                    //float x = 40;
                    //float y = 760;
                    //pdfCanvas.AddXObject(pdfFormXObject, scale, 0, 0, scale, x, y);

                }
            }
            Spire.Pdf.PdfDocument document1 = new Spire.Pdf.PdfDocument();
            document1.LoadFromFile(exportFile);
            System.Drawing.Image img = document1.SaveAsImage(0);
            img.Save(exportImage);
            document1.SaveToFile(System.IO.Path.Combine(exportFolder, "bc.svg"), FileFormat.SVG);
            //Process.Start(exportFile);
            Process proc = new Process();
            proc.StartInfo.FileName = exportImage;
            proc.StartInfo.UseShellExecute = true;
            proc.Start();
        }
    }
}
