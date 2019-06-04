using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using ManagingBooks.Model;
using Microsoft.Data.Sqlite;
using WPFCustomMessageBox;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using Microsoft.Win32;
using System.Diagnostics;
using iText.Layout.Borders;
using System.Data.SQLite;

namespace ManagingBooks.Windows
{
    /// <summary>
    /// Interaction logic for TransferBook.xaml
    /// </summary>
    public partial class TransferBook : Window
    {
        public TransferBook()
        {
            TransferBookModel context = new TransferBookModel();
            context.TransferList = new ObservableCollection<TransferingBook>();
            this.DataContext = context;
            InitializeComponent();
        }

        private void AddToTransferCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            TransferBookModel context = this.DataContext as TransferBookModel;
            e.CanExecute = !string.IsNullOrWhiteSpace(context.BookNumber);
        }

        private void AddToTransferCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            TransferBookModel context = this.DataContext as TransferBookModel;
            string number = context.BookNumber.PadLeft(6, '0');
            int.TryParse(context.BookNumber, out int bookNumber);

            if (context.TransferList.Any(n => n.Number == number))
            {
                //error
                string msg = Application.Current.FindResource("TransferBook.CodeBehind.WarningAdd.Exist.Message").ToString();
                string caption = Application.Current.FindResource("TransferBook.CodeBehind.WarningAdd.Exist.Caption").ToString();
                CustomMessageBox.ShowOK(msg, caption, CustomMessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                SqlMethods.SqlConnect(out SQLiteConnection con);
                SQLiteCommand selectCommand = con.CreateCommand();
                selectCommand.CommandText = $"SELECT BookId,Number,Title FROM Books WHERE Number={bookNumber}";
                SQLiteDataReader r = selectCommand.ExecuteReader();
                if (r.Read())
                {
                    TransferingBook book = new TransferingBook();
                    int.TryParse(Convert.ToString(r["BookId"]), out int result);
                    book.BookId = result;
                    book.Number = Convert.ToString(r["Number"]);
                    book.Title = Convert.ToString(r["Title"]);
                    context.TransferList.Add(book);
                }
                else
                {
                    // No book is found
                    string msg = Application.Current.FindResource("TransferBook.CodeBehind.ErrorAdd.NotFound.Message").ToString();
                    string caption = Application.Current.FindResource("TransferBook.CodeBehind.ErrorAdd.NotFound.Caption").ToString();
                    CustomMessageBox.ShowOK(msg, caption, CustomMessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            context.BookNumber = string.Empty;
        }

        private void IntNumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void ExportTransferListCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            TransferBookModel context = this.DataContext as TransferBookModel;
            e.CanExecute = context.TransferList.Count != 0;
        }

        private void ExportTransferListCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            TransferBookModel context = this.DataContext as TransferBookModel;
            string pdfPath;
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.FileName = "Transfer List";
            dialog.Filter = "PDF (*.pdf)|*.pdf";
            dialog.InitialDirectory = AppContext.BaseDirectory;
            if (dialog.ShowDialog(this) == true)
            {
                string pdfTitle = Application.Current.FindResource("TransferBook.Title").ToString();
                string pdfNumberCol = Application.Current.FindResource("TransferBook.ListTransfer.Number").ToString();
                string pdfTitleCol = Application.Current.FindResource("TransferBook.ListTransfer.BookTitle").ToString();
                pdfPath = dialog.FileName;
                using (PdfWriter writer = new PdfWriter(pdfPath))
                {
                    using (PdfDocument pdf = new PdfDocument(writer))
                    {
                        Document document = new Document(pdf);
                        Paragraph paragraph = new Paragraph(pdfTitle)
                            .SetFontSize(20.0f)
                            .SetBold()
                            .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);

                        float[] columnWidths = { 1, 3, 10 };
                        Table table = new Table(columnWidths);
                        table.SetWidth(iText.Layout.Properties.UnitValue.CreatePercentValue(100));

                        Cell[] cells = new Cell[3];
                        for (int i = 0; i < cells.Length; i++)
                        {
                            cells[i] = new Cell();
                        }
                        Paragraph para = new Paragraph().Add("ID").SetBold();
                        cells[0].Add(para)
                            .SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT);
                        para = new Paragraph().Add(pdfNumberCol).SetBold();
                        cells[1].Add(para).SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT);
                        para = new Paragraph().Add(pdfTitleCol).SetBold();
                        cells[2].Add(para).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                        foreach (var cell in cells)
                        {
                            table.AddHeaderCell(cell);
                        }
                        for (int i = 0; i < context.TransferList.Count; i++)
                        {
                            for (int j = 0; j < cells.Length; j++)
                            {
                                cells[j] = new Cell();
                            }
                            cells[0].Add(new Paragraph((i + 1).ToString()).SetFixedLeading(15)).SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT);
                            cells[1].Add(new Paragraph(context.TransferList[i].Number).SetFixedLeading(15)).SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT);
                            cells[2].Add(new Paragraph(context.TransferList[i].Title).SetFixedLeading(15)).SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT);
                            foreach (var cell in cells)
                            {
                                table.AddCell(cell);
                            }
                        }

                        document.Add(paragraph);
                        document.Add(table);
                    }
                }
                Process proc = new Process();
                proc.StartInfo.FileName = pdfPath;
                proc.StartInfo.UseShellExecute = true;
                proc.Start();
            }
        }
    }
}
