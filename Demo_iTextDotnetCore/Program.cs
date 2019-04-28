using System;
using System.IO;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Barcodes;
using iText.Layout.Borders;
using iText.Layout.Properties;
using System.Diagnostics;

namespace Demo_iTextDotnetCore
{
    class Program
    {
        static void Main(string[] args)
        {
            string exportFolder = AppContext.BaseDirectory;
            string exportFile = Path.Combine(exportFolder, "Test.pdf");

            using (PdfWriter pdfWriter = new PdfWriter(exportFile))
            {
                using(PdfDocument pdf = new PdfDocument(pdfWriter))
                {
                    Document document = new Document(pdf);
                    //document.Add(new Paragraph("Hello World"));
                    Barcode128 barcode128 = new Barcode128(pdf);
                    barcode128.SetCodeType(Barcode128.CODE_C);
                    //barcode128.SetBaseline(-1);
                    //barcode128.SetSize(15);
                    barcode128.SetCode("011120");
                    Image barcodeImage = new Image(barcode128.CreateFormXObject(pdf));
                    barcodeImage.SetHorizontalAlignment(HorizontalAlignment.CENTER);
                    barcodeImage.Scale(1.5F, 1.5F);
                    Table table = new Table(5, false);
                    table.SetWidth(UnitValue.CreatePercentValue(100));
                    Cell[] cellArray = new Cell[100];
                    for (int i = 0; i < cellArray.Length; i++)
                    {
                        cellArray[i] = new Cell();
                        //cellArray[i].SetBorder(new SolidBorder(ColorConstants.WHITE, 0));
                        cellArray[i].Add(barcodeImage);
                        //cellArray[i].SetHorizontalAlignment(iText.Layout.Properties.HorizontalAlignment.CENTER);
                        table.AddCell(cellArray[i]);
                    }
                    
                    //table.SetBorder(new SolidBorder(1));
                    document.Add(table);
                    
                    //barcode128.PlaceBarcode(new PdfCanvas(pdf.GetFirstPage()), ColorConstants.BLUE, ColorConstants.GREEN);
                    //PdfCanvas pdfCanvas = new PdfCanvas(pdf.GetFirstPage());
                    //PdfFormXObject pdfFormXObject = barcode128.CreateFormXObject(pdf);
                    //float scale = 1;
                    //float x = 40;
                    //float y = 760;
                    //pdfCanvas.AddXObject(pdfFormXObject, scale, 0, 0, scale, x, y);
                }
            }
            //Process.Start(exportFile);
            Process proc = new Process();
            proc.StartInfo.FileName = exportFile;
            proc.StartInfo.UseShellExecute = true;
            proc.Start();
        }
    }
}
