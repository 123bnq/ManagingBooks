using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ManagingBooks.Model;
using ManagingBooks.Windows;
using Microsoft.Data.Sqlite;
using System.Collections.ObjectModel;

namespace ManagingBooks
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new SearchBookModel();
            (DataContext as SearchBookModel).DisplayBooks = new ObservableCollection<SearchBook>();
            SearchList.ItemsSource = (DataContext as SearchBookModel).DisplayBooks;
            SearchAll(this.DataContext as SearchBookModel);
        }

        private void AddWindow_Click(object sender, RoutedEventArgs e)
        {
            new AddBook() { Owner = this }.ShowDialog();
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
            if (context.SearchBy.Equals("All"))
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
            selectCommand.CommandText = "SELECT b.BookId,b.Number,b.Title,b.Version,b.Medium,a.AuthorId,a.FirstName,s.Signature FROM Books b LEFT JOIN Books_Authors ba ON (b.BookId = ba.BookId) LEFT JOIN Authors a ON (ba.AuthorId = a.AuthorId) LEFT JOIN Signatures s ON (s.BookId = b.BookId)";
            selectCommand.CommandType = System.Data.CommandType.Text;
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
                        tempBook.Authors += ", " + Convert.ToString(r["FirstName"]);
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
                    int.TryParse(Convert.ToString(r["Version"]), out result);
                    tempBook.Version = result;
                    tempBook.Medium = Convert.ToString(r["Medium"]);
                    int.TryParse(Convert.ToString(r["AuthorId"]), out result);
                    lastAuthorId = result;
                    tempBook.Authors = Convert.ToString(r["FirstName"]);
                    tempBook.Signatures = Convert.ToString(r["Signature"]);
                    context.DisplayBooks.Add(tempBook);
                }
            }
            con.Close();
        }
    }

    public static class CustomCommands
    {
        public static readonly RoutedUICommand Exit = new RoutedUICommand("Exit", "Exit", typeof(CustomCommands), new InputGestureCollection()
        {
            new KeyGesture(Key.Q, ModifierKeys.Control)
        });

        public static readonly RoutedUICommand Search = new RoutedUICommand("Search", "Search", typeof(CustomCommands));
    }
}
