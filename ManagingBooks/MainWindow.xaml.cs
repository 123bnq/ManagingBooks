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

        //SearchBook DeleteBook;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new SearchBookModel();
            (DataContext as SearchBookModel).DisplayBooks = new ObservableCollection<SearchBook>();
            SearchList.ItemsSource = (DataContext as SearchBookModel).DisplayBooks;
            (DataContext as SearchBookModel).DisplayBooks.Clear();
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
            temp.Status = "Running";
            if (e.UserState != null)
            {
                temp.DisplayBooks.Add(e.UserState as SearchBook);
            }
        }

        void Search_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SearchBookModel context = this.DataContext as SearchBookModel;
            context.Status = "Load Finished";
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
                switch (context.SearchBy)
                {
                    case "Number":
                        return (item as SearchBook).Number.ToString().IndexOf(context.SearchText, StringComparison.OrdinalIgnoreCase) >= 0;
                    case "Signature":
                        return (item as SearchBook).Signatures.StartsWith(context.SearchText, StringComparison.OrdinalIgnoreCase);
                    case "Title":
                        return (item as SearchBook).Title.IndexOf(context.SearchText, StringComparison.OrdinalIgnoreCase) >= 0;
                    case "Authors":
                        return (item as SearchBook).Authors.IndexOf(context.SearchText, StringComparison.OrdinalIgnoreCase) >= 0;
                    case "Place":
                        return (item as SearchBook).Place.IndexOf(context.SearchText, StringComparison.OrdinalIgnoreCase) >= 0;
                    default:
                        return true;
                }
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

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            if (SearchList.SelectedItem != null)
            {
                new BarcodeDisplay((SearchList.SelectedItem as SearchBook).Number.ToString().PadLeft(6, '0')) { Owner = this }.Show();
            }
            else
            {
                MessageBox.Show("No book is selected", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
            if (SearchList.SelectedItem != null)
            {
                new BarcodeDisplay((SearchList.SelectedItem as SearchBook).Number.ToString().PadLeft(6, '0')) { Owner = this }.Show();
            }
            else
            {
                MessageBox.Show("No book is selected", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
            MessageBoxResult result = MessageBox.Show("Do you want to delete selected book(s)?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);

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
                            context.Status = "Deleting";

                            App.Current.Dispatcher.Invoke((Action)delegate
                            {
                                context.DisplayBooks.Remove(tempBook);
                            });

                            Thread.Sleep(TimeSpan.FromTicks(5));
                        }
                    });
                    context.Status = "Finished delete";
                }
            }
            SearchList.IsEnabled = true;
            BoxSearchText.IsEnabled = true;
        }

        private void Delete_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SearchBookModel context = this.DataContext as SearchBookModel;
            context.Status = "Finished delete";
        }

        private void Delete_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            SearchBookModel context = this.DataContext as SearchBookModel;
            context.DisplayBooks.Remove(e.UserState as SearchBook);
            context.Progress = e.ProgressPercentage;
            context.Status = "Deleting";
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

        public static readonly RoutedUICommand RemovePub = new RoutedUICommand("RemovePublisher", "RemovePublisher", typeof(CustomCommands));
        public static readonly RoutedUICommand SavePub = new RoutedUICommand("SavePublisher", "SavePublisher", typeof(CustomCommands));
        public static readonly RoutedUICommand RemoveMedium = new RoutedUICommand("RemoveMedium", "RemoveMedium", typeof(CustomCommands));
        public static readonly RoutedUICommand SaveMedium = new RoutedUICommand("SaveMedium", "SaveMedium", typeof(CustomCommands));
        public static readonly RoutedUICommand RemovePlace = new RoutedUICommand("RemovePlace", "RemovePlace", typeof(CustomCommands));
        public static readonly RoutedUICommand SavePlace = new RoutedUICommand("SavePlace", "SavePlace", typeof(CustomCommands));
    }
}