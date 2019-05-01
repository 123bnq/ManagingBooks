using System;
using System.Windows;
using System.Windows.Input;
using ManagingBooks.Model;
using ManagingBooks.Windows;
using Microsoft.Data.Sqlite;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.ComponentModel;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Reflection;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Barcodes;
using Microsoft.Win32;
using System.Diagnostics;
using System.Linq;
using WPFCustomMessageBox;

namespace ManagingBooks
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BackgroundWorker Search = new BackgroundWorker();
        BackgroundWorker Delete = new BackgroundWorker();
        int LastIndex = -1;

        Uri English = new Uri(".\\Resources\\Resources.xaml", UriKind.Relative);
        Uri German = new Uri(".\\Resources\\Resources.de.xaml", UriKind.Relative);

        //SearchBook DeleteBook;

        public MainWindow()
        {
            InitializeComponent();
            SearchBookModel context = new SearchBookModel();
            this.DataContext = context;
            context.DisplayBooks = new ObservableCollection<SearchBook>();
            context.ListBookPrint = new ObservableCollection<int>();
            SearchList.ItemsSource = context.DisplayBooks;
            context.DisplayBooks.Clear();
            Search.WorkerReportsProgress = true;
            Search.DoWork += Search_DoWork;
            Search.ProgressChanged += Search_ProgressChanged;
            Search.RunWorkerCompleted += Search_RunWorkerCompleted;
            Search.RunWorkerAsync(NumberOfBooks());
            CollectionView view = CollectionViewSource.GetDefaultView(SearchList.ItemsSource) as CollectionView;
            view.Filter = UserFilter;
            Delete.WorkerReportsProgress = true;
            Delete.DoWork += Delete_DoWork;
            Delete.ProgressChanged += Delete_ProgressChanged;
            Delete.RunWorkerCompleted += Delete_RunWorkerCompleted;
            ClearEntries(context);
        }


        void Search_DoWork(object sender, DoWorkEventArgs e)
        {
            int max = (int)e.Argument;
            int i = 0;
            int progressPercentage;

            SqliteConnection con;
            SqlMethods.SqlConnect(out con);
            var selectCommand = con.CreateCommand();
            selectCommand.CommandText = "SELECT b.BookId,b.Number,b.Title,b.Version,b.Medium,a.AuthorId,a.Name,s.Signature,b.Publisher,b.Place,b.Year,b.DayBought,b.Pages,b.Price " +
                "FROM Books b " +
                "LEFT JOIN Books_Authors ba ON (b.BookId = ba.BookId) " +
                "LEFT JOIN Authors a ON (ba.AuthorId = a.AuthorId) " +
                "LEFT JOIN Books_Signatures bs ON (bs.BookId = b.BookId) " +
                "LEFT JOIN Signatures s ON (bs.SignatureId = s.SignatureId) ORDER BY b.BookId,ba.Priority,bs.Priority";
            SqliteDataReader r = selectCommand.ExecuteReader();
            int lastBookId = -1;
            int lastAuthorId = -1;
            bool finishedBook = false;
            SearchBook tempBook = new SearchBook();
            while (r.Read())
            {
                int result;
                int.TryParse(Convert.ToString(r["BookId"]), out result);
                if (result == lastBookId)
                {
                    int result1;
                    int.TryParse(Convert.ToString(r["AuthorId"]), out result1);
                    if (result1 == lastAuthorId)
                    {
                        string temp = Convert.ToString(r["Signature"]);
                        if (!tempBook.Signatures.Contains(temp))
                        {
                            tempBook.Signatures += " " + Convert.ToString(r["Signature"]);
                        }
                    }
                    if (result1 != lastAuthorId)
                    {
                        tempBook.Authors += ", " + Convert.ToString(r["Name"]);
                        lastAuthorId = result1;
                    }
                }
                else
                {
                    finishedBook = true;
                    if (finishedBook && (!string.IsNullOrEmpty(tempBook.Title)))
                    {
                        // report progress change
                        progressPercentage = Convert.ToInt32(((double)i / max) * 100);
                        (sender as BackgroundWorker).ReportProgress(progressPercentage, tempBook);

                        finishedBook = false;
                    }
                    tempBook = new SearchBook();
                    tempBook.BookId = result;
                    lastBookId = result;
                    int.TryParse(Convert.ToString(r["Number"]), out result);
                    tempBook.Number = result;
                    tempBook.Title = Convert.ToString(r["Title"]);
                    tempBook.Publishers = Convert.ToString(r["Publisher"]);
                    int.TryParse(Convert.ToString(r["Year"]), out result);
                    tempBook.Year = result;
                    int.TryParse(Convert.ToString(r["Version"]), out result);
                    tempBook.Version = result;
                    tempBook.Medium = Convert.ToString(r["Medium"]);
                    tempBook.Place = Convert.ToString(r["Place"]);
                    tempBook.Date = Convert.ToString(r["DayBought"]);
                    int.TryParse(Convert.ToString(r["Pages"]), out result);
                    tempBook.Pages = result;
                    double.TryParse(Convert.ToString(r["Price"]), out double result_d);
                    tempBook.Price = result_d;
                    int.TryParse(Convert.ToString(r["AuthorId"]), out result);
                    lastAuthorId = result;
                    tempBook.Authors = Convert.ToString(r["Name"]);
                    tempBook.Signatures = Convert.ToString(r["Signature"]);
                    i++;
                }
                Thread.Sleep(TimeSpan.FromTicks(500));
            }
            if (i != 0)
            {
                progressPercentage = Convert.ToInt32(((double)i / max) * 100);
                (sender as BackgroundWorker).ReportProgress(progressPercentage, tempBook);
            }
            e.Result = i;
            con.Close();

        }

        void Search_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            SearchBookModel temp = this.DataContext as SearchBookModel;
            temp.Progress = e.ProgressPercentage;
            temp.Status = Application.Current.FindResource("MainWindow.CodeBehind.Status.Running").ToString();
            if (e.UserState != null)
            {
                temp.DisplayBooks.Add(e.UserState as SearchBook);
            }
        }

        void Search_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SearchBookModel context = this.DataContext as SearchBookModel;
            context.Status = Application.Current.FindResource("MainWindow.CodeBehind.Status.Completed").ToString();
            if (LastIndex != -1)
            {
                SearchList.SelectedIndex = LastIndex;
                LastIndex = -1;
                SearchList.ScrollIntoView(SearchList.SelectedItem);
            }
        }

        private bool UserFilter(object item)
        {
            var context = this.DataContext as SearchBookModel;
            if (string.IsNullOrWhiteSpace(context.SearchText))
            {
                return true;
            }
            else
            {
                //switch (context.SearchBy)
                //{
                //    case "Number":
                //        return (item as SearchBook).Number.ToString().IndexOf(context.SearchText, StringComparison.OrdinalIgnoreCase) >= 0;
                //    case "Signature":
                //        return (item as SearchBook).Signatures.StartsWith(context.SearchText, StringComparison.OrdinalIgnoreCase);
                //    case "Title":
                //        return (item as SearchBook).Title.IndexOf(context.SearchText, StringComparison.OrdinalIgnoreCase) >= 0;
                //    case "Authors":
                //        return (item as SearchBook).Authors.IndexOf(context.SearchText, StringComparison.OrdinalIgnoreCase) >= 0;
                //    case "Place":
                //        return (item as SearchBook).Place.IndexOf(context.SearchText, StringComparison.OrdinalIgnoreCase) >= 0;
                //    case "Medium":
                //        return (item as SearchBook).Medium.StartsWith(context.SearchText, StringComparison.OrdinalIgnoreCase);
                //    default:
                //        return true;
                //}
                if (context.SearchBy.Equals(Application.Current.FindResource("MainWindow.SearchBy.Number")))
                    return (item as SearchBook).Number.ToString().IndexOf(context.SearchText, StringComparison.OrdinalIgnoreCase) >= 0;
                else if (context.SearchBy.Equals(Application.Current.FindResource("MainWindow.SearchBy.Signatures")))
                    return (item as SearchBook).Signatures.StartsWith(context.SearchText, StringComparison.OrdinalIgnoreCase);
                else if (context.SearchBy.Equals(Application.Current.FindResource("MainWindow.SearchBy.Title")))
                    return (item as SearchBook).Title.IndexOf(context.SearchText, StringComparison.OrdinalIgnoreCase) >= 0;
                else if (context.SearchBy.Equals(Application.Current.FindResource("MainWindow.SearchBy.Authors")))
                    return (item as SearchBook).Authors.IndexOf(context.SearchText, StringComparison.OrdinalIgnoreCase) >= 0;
                else if (context.SearchBy.Equals(Application.Current.FindResource("MainWindow.SearchBy.Place")))
                    return (item as SearchBook).Place.IndexOf(context.SearchText, StringComparison.OrdinalIgnoreCase) >= 0;
                else if (context.SearchBy.Equals(Application.Current.FindResource("MainWindow.SearchBy.Medium")))
                    return (item as SearchBook).Medium.StartsWith(context.SearchText, StringComparison.OrdinalIgnoreCase);
                else
                    return true;
            }
        }

        private void AddWindow_Click(object sender, RoutedEventArgs e)
        {
            if (!new AddBook() { Owner = this }.ShowDialog().Value)
            {
                (DataContext as SearchBookModel).DisplayBooks.Clear();
                Search.RunWorkerAsync(NumberOfBooks());
            }
        }

        private void ExitCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ExitCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void SearchCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (!string.IsNullOrWhiteSpace(BoxSearchText.Text) && !string.IsNullOrWhiteSpace(BoxSearchBy.Text));
        }

        private void SearchCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SearchBookModel context = (this.DataContext as SearchBookModel);
            CollectionViewSource.GetDefaultView(SearchList.ItemsSource).Refresh();
            if (context.SearchBy.Equals("Author"))
            {
            }

            MessageBox.Show($"Search is executed\nSearch for: {context.SearchText}\nSearch By: {context.SearchBy}", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            context.SearchText = string.Empty;
            BoxSearchBy.SelectedItem = null;
        }

        private void SearchAll(SearchBookModel context)
        {
            SqliteConnection con;
            SqlMethods.SqlConnect(out con);
            var selectCommand = con.CreateCommand();
            selectCommand.CommandText = "SELECT b.BookId,b.Number,b.Title,b.Version,b.Medium,a.AuthorId,a.Name,s.Signature,b.Publisher,b.Place,b.Year,b.DayBought,b.Pages,b.Price " +
                "FROM Books b " +
                "LEFT JOIN Books_Authors ba ON (b.BookId = ba.BookId) " +
                "LEFT JOIN Authors a ON (ba.AuthorId = a.AuthorId) " +
                "LEFT JOIN Books_Signatures bs ON (b.BookId = bs.BookId) " +
                "LEFT JOIN Signatures s ON (bs.SignatureId = s.SignatureId) ORDER BY b.BookId";
            SqliteDataReader r = selectCommand.ExecuteReader();
            int lastBookId = -1;
            int lastAuthorId = -1;
            SearchBook tempBook = new SearchBook();
            while (r.Read())
            {

                int result;
                int.TryParse(Convert.ToString(r["BookId"]), out result);
                if (result == lastBookId)
                {
                    int result1;
                    int.TryParse(Convert.ToString(r["AuthorId"]), out result1);
                    if (result1 == lastAuthorId)
                    {
                        string temp = Convert.ToString(r["Signature"]);
                        if (!tempBook.Signatures.Contains(temp))
                        {
                            tempBook.Signatures += " " + Convert.ToString(r["Signature"]);
                        }
                    }
                    if (result1 != lastAuthorId)
                    {
                        tempBook.Authors += ", " + Convert.ToString(r["Name"]);
                        lastAuthorId = result1;
                    }
                }
                if (result != lastBookId)
                {
                    tempBook = new SearchBook();
                    tempBook.BookId = result;
                    lastBookId = result;
                    int.TryParse(Convert.ToString(r["Number"]), out result);
                    tempBook.Number = result;
                    tempBook.Title = Convert.ToString(r["Title"]);
                    tempBook.Publishers = Convert.ToString(r["Publisher"]);
                    int.TryParse(Convert.ToString(r["Year"]), out result);
                    tempBook.Year = result;
                    int.TryParse(Convert.ToString(r["Version"]), out result);
                    tempBook.Version = result;
                    tempBook.Medium = Convert.ToString(r["Medium"]);
                    tempBook.Place = Convert.ToString(r["Place"]);
                    tempBook.Date = Convert.ToString(r["DayBought"]);
                    int.TryParse(Convert.ToString(r["Pages"]), out result);
                    tempBook.Pages = result;
                    double.TryParse(Convert.ToString(r["Price"]), out double result_d);
                    tempBook.Price = result_d;
                    int.TryParse(Convert.ToString(r["AuthorId"]), out result);
                    lastAuthorId = result;
                    tempBook.Authors = Convert.ToString(r["Name"]);
                    tempBook.Signatures = Convert.ToString(r["Signature"]);
                    context.DisplayBooks.Add(tempBook);
                }
            }
            con.Close();
        }

        private void BoxSearchText_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(SearchList.ItemsSource).Refresh();
        }

        private void EditCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (SearchList.SelectedItem != null);
        }

        private void EditCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            int bookId = (SearchList.SelectedItem as SearchBook).BookId;
            LastIndex = SearchList.SelectedIndex;
            EditBook editWindow = new EditBook(bookId) { Owner = this };
            if (!editWindow.ShowDialog().Value)
            {
                if (editWindow.IsNew)
                {
                    LastIndex = SearchList.Items.Count;
                }
                (DataContext as SearchBookModel).DisplayBooks.Clear();
                Search.RunWorkerAsync(NumberOfBooks());
            }
        }

        private void PrintCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //new BarcodeDisplay((SearchList.SelectedItem as SearchBook).Number.ToString().PadLeft(6, '0')) { Owner = this }.Show();
            SearchBookModel context = this.DataContext as SearchBookModel;
            string pdfPath;
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "PDF (*.pdf)|*.pdf";
            dialog.InitialDirectory = AppContext.BaseDirectory;
            if (dialog.ShowDialog(this) == true)
            {
                pdfPath = dialog.FileName;
                using (PdfWriter writer = new PdfWriter(pdfPath))
                {
                    using (PdfDocument pdf = new PdfDocument(writer))
                    {
                        Document document = new Document(pdf);
                        Barcode128 barcode = new Barcode128(pdf);
                        barcode.SetCodeType(Barcode128.CODE_C);
                        Table table = new Table(5, false);
                        table.SetWidth(iText.Layout.Properties.UnitValue.CreatePercentValue(100));
                        foreach (var book in context.ListBookPrint)
                        {
                            string codeNr = book.ToString();
                            int length = codeNr.Length;
                            for (int i = 6; i > length; i--)
                            {
                                codeNr = String.Concat("0", codeNr);
                            }
                            barcode.SetCode(codeNr);
                            Image barcodeImage = new Image(barcode.CreateFormXObject(pdf));
                            barcodeImage.SetHorizontalAlignment(iText.Layout.Properties.HorizontalAlignment.CENTER);
                            //barcodeImage.Scale(1.5F, 1.5F);
                            Cell cell = new Cell();
                            cell.Add(barcodeImage);
                            table.AddCell(cell);
                        }
                        document.Add(table);
                    }
                }
                Process proc = new Process();
                proc.StartInfo.FileName = pdfPath;
                proc.StartInfo.UseShellExecute = true;
                proc.Start();
            }

        }

        private int NumberOfBooks()
        {
            int numBook = 0;
            SqlMethods.SqlConnect(out SqliteConnection con);
            var selectCommand = con.CreateCommand();
            selectCommand.CommandText = "SELECT BookId FROM Books";
            SqliteDataReader r = selectCommand.ExecuteReader();
            while (r.Read())
            {
                numBook++;
            }
            con.Close();
            return numBook;
        }

        private void DeleteCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (SearchList.SelectedItem != null);
        }

        private async void DeleteCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SearchBookModel context = this.DataContext as SearchBookModel;
            var deleteList = SearchList.SelectedItems;
            string msg = Application.Current.FindResource("MainWindow.CodeBehind.DeleteNotify.Message").ToString();
            string caption = Application.Current.FindResource("MainWindow.CodeBehind.DeleteNotify.Caption").ToString();
            MessageBoxResult result = CustomMessageBox.ShowYesNo(msg, caption, Application.Current.FindResource("MessageBox.YesBtn").ToString(), Application.Current.FindResource("MessageBox.NoBtn").ToString(), MessageBoxImage.Warning);
            //MessageBoxResult result = MessageBox.Show(msg, caption, MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);

            var max = deleteList.Count;
            var progressHandler = new Progress<int>(value =>
            {
                context.Progress = value;
            });
            IProgress<int> progress = progressHandler as IProgress<int>;
            if (result == MessageBoxResult.Yes)
            {
                SearchList.IsEnabled = false;
                BoxSearchText.IsEnabled = false;
                if (SearchList.SelectedIndex != -1)
                {
                    await Task.Run(() =>
                    {
                        for (int i = deleteList.Count - 1; i >= 0; i--)
                        {
                            var tempBook = deleteList[i] as SearchBook;
                            int bookId = tempBook.BookId;
                            SqlMethods.SqlConnect(out SqliteConnection con);
                            var tr = con.BeginTransaction();
                            var deleteCommand = con.CreateCommand();
                            deleteCommand.Transaction = tr;
                            deleteCommand.CommandText = $"DELETE FROM Books_Authors WHERE BookId={bookId}";
                            deleteCommand.ExecuteNonQuery();
                            deleteCommand = con.CreateCommand();
                            deleteCommand.CommandText = $"DELETE FROM Books_Signatures WHERE BookId={bookId}";
                            deleteCommand.ExecuteNonQuery();
                            deleteCommand = con.CreateCommand();
                            deleteCommand.CommandText = $"DELETE FROM Books WHERE BookId={bookId}";
                            deleteCommand.ExecuteNonQuery();
                            tr.Commit();
                            con.Close();


                            progress.Report(Convert.ToInt32((double)(max - i) / max * 100));
                            context.Status = Application.Current.FindResource("MainWindow.CodeBehind.Status.Deleting").ToString();

                            App.Current.Dispatcher.Invoke((Action)delegate
                            {
                                context.DisplayBooks.Remove(tempBook);
                            });

                            Thread.Sleep(TimeSpan.FromTicks(5));
                        }
                    });
                    context.Status = Application.Current.FindResource("MainWindow.CodeBehind.Status.DeleteCompleted").ToString();
                }
            }
            SearchList.IsEnabled = true;
            BoxSearchText.IsEnabled = true;
        }

        private void Delete_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SearchBookModel context = this.DataContext as SearchBookModel;
            context.Status = Application.Current.FindResource("MainWindow.CodeBehind.Status.DeleteCompleted").ToString();
        }

        private void Delete_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            SearchBookModel context = this.DataContext as SearchBookModel;
            context.DisplayBooks.Remove(e.UserState as SearchBook);
            context.Progress = e.ProgressPercentage;
            context.Status = Application.Current.FindResource("MainWindow.CodeBehind.Status.Deleting").ToString();
            ClearEntries(context);
        }

        private void Delete_DoWork(object sender, DoWorkEventArgs e)
        {
            {
                this.Dispatcher.Invoke(() =>
                {
                    var deleteList = SearchList.SelectedItems;
                    //var deleteList = e.Argument as List;
                    int progressPercentage;
                    if (SearchList.SelectedIndex != -1)
                    {
                        for (int i = deleteList.Count - 1; i >= 0; i--)
                        {
                            var tempBook = deleteList[i] as SearchBook;
                            int bookId = tempBook.BookId;
                            SqlMethods.SqlConnect(out SqliteConnection con);
                            var tr = con.BeginTransaction();
                            var deleteCommand = con.CreateCommand();
                            deleteCommand.Transaction = tr;
                            deleteCommand.CommandText = $"DELETE FROM Books_Authors WHERE BookId={bookId}";
                            deleteCommand.ExecuteNonQuery();
                            deleteCommand = con.CreateCommand();
                            deleteCommand.CommandText = $"DELETE FROM Books_Signatures WHERE BookId={bookId}";
                            deleteCommand.ExecuteNonQuery();
                            deleteCommand = con.CreateCommand();
                            deleteCommand.CommandText = $"DELETE FROM Books WHERE BookId={bookId}";
                            deleteCommand.ExecuteNonQuery();
                            tr.Commit();
                            con.Close();
                            Thread.Sleep(1);
                            progressPercentage = Convert.ToInt32((double)(deleteList.Count - i) / deleteList.Count * 100);
                            (sender as BackgroundWorker).ReportProgress(progressPercentage, tempBook);


                            //context.DisplayBooks.Remove(tempBook);
                        }
                    }
                });
            }
        }

        private void EditPublisher_Click(object sender, RoutedEventArgs e)
        {
            new EditPublisher() { Owner = this }.ShowDialog();
        }

        private void EditPlace_Click(object sender, RoutedEventArgs e)
        {
            new EditPlace() { Owner = this }.ShowDialog();
        }

        private void EditMedium_Click(object sender, RoutedEventArgs e)
        {
            new EditMedium() { Owner = this }.ShowDialog();
        }

        private void SearchList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            SearchBookModel context = this.DataContext as SearchBookModel;
            SearchBook viewBook = SearchList.SelectedItem as SearchBook;
            if (viewBook != null)
            {
                context.ViewNumber = viewBook.Number.ToString();
                context.ViewSignatures = viewBook.Signatures;
                context.ViewTitle = viewBook.Title;
                context.ViewAuthors = viewBook.Authors;
                context.ViewPublisher = viewBook.Publishers;
                context.ViewYear = viewBook.Year.ToString();
                context.ViewVersion = viewBook.Version.ToString();
                context.ViewMedium = viewBook.Medium;
                context.ViewPlace = viewBook.Place;
                context.ViewDate = viewBook.Date;
                context.ViewPages = viewBook.Pages.ToString();
                context.ViewPrice = viewBook.Price.ToString();
            }
        }

        private void ClearEntries(SearchBookModel context)
        {
            context.ViewNumber = string.Empty;
            context.ViewSignatures = string.Empty;
            context.ViewTitle = string.Empty;
            context.ViewAuthors = string.Empty;
            context.ViewPublisher = string.Empty;
            context.ViewYear = string.Empty;
            context.ViewVersion = string.Empty;
            context.ViewMedium = string.Empty;
            context.ViewPlace = string.Empty;
            context.ViewDate = string.Empty;
            context.ViewPages = string.Empty;
            context.ViewPrice = string.Empty;
        }

        private void ClearBookInfoCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SearchList.SelectedIndex = -1;
            ClearEntries(this.DataContext as SearchBookModel);
        }

        private void EnglishCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !Application.Current.Resources.MergedDictionaries[0].Source.ToString().Equals(English.OriginalString);
        }

        private void EnglishCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ResourceDictionary dict = new ResourceDictionary();
            dict.Source = English;
            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(dict);
        }

        private void GermanCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !Application.Current.Resources.MergedDictionaries[0].Source.ToString().Equals(German.OriginalString);
        }

        private void GermanCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ResourceDictionary dict = new ResourceDictionary();
            dict.Source = German;
            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(dict);
        }

        private void PrintCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ListPrint.Items.Count != 0;
        }

        private void RemoveFromPrintCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ListPrint.SelectedItem != null;
        }

        private void RemoveFromPrint_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SearchBookModel context = this.DataContext as SearchBookModel;
            int bookToRemove = (int)ListPrint.SelectedItem;
            context.ListBookPrint.Remove(bookToRemove);
        }

        private void ClearPrintListCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SearchBookModel context = this.DataContext as SearchBookModel;
            context.ListBookPrint.Clear();
        }

        private async void AddToPrintCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SearchBookModel context = this.DataContext as SearchBookModel;
            List<SearchBook> addList = SearchList.SelectedItems.Cast<SearchBook>().ToList();
            addList.Sort((x, y) => x.BookId.CompareTo(y.BookId));
            await Task.Run(() =>
            {
                foreach (var book in addList)
                {
                    if (!context.ListBookPrint.Contains((book as SearchBook).Number))
                    {
                        App.Current.Dispatcher.Invoke((Action)delegate
                        {
                            context.ListBookPrint.Add((book as SearchBook).Number);
                        });
                        Thread.Sleep(TimeSpan.FromTicks(5));
                    }

                    //else
                    //{
                    //    MessageBox.Show("Already added");
                    //}
                }
            });

            //SearchBook book = SearchList.SelectedItem as SearchBook;


        }
    }

    public static class CustomCommands
    {
        public static readonly RoutedUICommand Exit = new RoutedUICommand("Exit", "Exit", typeof(CustomCommands), new InputGestureCollection()
        {
            new KeyGesture(Key.Q, ModifierKeys.Control)
        });
        public static readonly RoutedUICommand Undelete = new RoutedUICommand("Undelete", "Undelete", typeof(CustomCommands), new InputGestureCollection()
        {
            new KeyGesture(Key.Z, ModifierKeys.Control)
        });

        public static readonly RoutedUICommand Search = new RoutedUICommand("Search", "Search", typeof(CustomCommands));
        public static readonly RoutedUICommand Edit = new RoutedUICommand("Edit", "Edit", typeof(CustomCommands));
        public static readonly RoutedUICommand Delete = new RoutedUICommand("Delete", "Delete", typeof(CustomCommands));
        public static readonly RoutedUICommand Print = new RoutedUICommand("Print", "Print", typeof(CustomCommands));
        public static readonly RoutedUICommand AddToPrint = new RoutedUICommand("AddToPrint", "AddToPrint", typeof(CustomCommands));
        public static readonly RoutedUICommand RemoveFromPrint = new RoutedUICommand("RemoveFromPrint", "RemoveFromPrint", typeof(CustomCommands));
        public static readonly RoutedUICommand ClearPrintList = new RoutedUICommand("ClearPrintList", "ClearPrintList", typeof(CustomCommands));

        public static readonly RoutedUICommand ClearBookInfo = new RoutedUICommand("ClearBookInfo", "ClearBookInfo", typeof(CustomCommands));

        public static readonly RoutedUICommand English = new RoutedUICommand("English", "English", typeof(CustomCommands));
        public static readonly RoutedUICommand German = new RoutedUICommand("German", "German", typeof(CustomCommands));

        public static readonly RoutedUICommand RemovePub = new RoutedUICommand("RemovePublisher", "RemovePublisher", typeof(CustomCommands));
        public static readonly RoutedUICommand SavePub = new RoutedUICommand("SavePublisher", "SavePublisher", typeof(CustomCommands));
        public static readonly RoutedUICommand RemoveMedium = new RoutedUICommand("RemoveMedium", "RemoveMedium", typeof(CustomCommands));
        public static readonly RoutedUICommand SaveMedium = new RoutedUICommand("SaveMedium", "SaveMedium", typeof(CustomCommands));
        public static readonly RoutedUICommand RemovePlace = new RoutedUICommand("RemovePlace", "RemovePlace", typeof(CustomCommands));
        public static readonly RoutedUICommand SavePlace = new RoutedUICommand("SavePlace", "SavePlace", typeof(CustomCommands));
    }
}