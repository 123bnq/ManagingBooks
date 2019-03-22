using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.Sqlite;
using ManagingBooks.Model;
using System.Globalization;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace ManagingBooks.Windows
{
    /// <summary>
    /// Interaction logic for AddBook.xaml
    /// </summary>
    public partial class AddBook : Window
    {
        ObservableCollection<string> ListPublisher = new ObservableCollection<string>();
        ObservableCollection<string> ListMedium = new ObservableCollection<string>();
        ObservableCollection<string> ListPlace = new ObservableCollection<string>();
        public AddBook()
        {
            this.DataContext = new AddBookModel();
            (this.DataContext as AddBookModel).ListBook = new ObservableCollection<int>();
            InitializeComponent();
            BoxPublisher.ItemsSource = ListPublisher;
            BoxMedium.ItemsSource = ListMedium;
            BoxPlace.ItemsSource = ListPlace;
            ReadPublisherMediumPlace();
            UpdateListBook(this.DataContext as AddBookModel);
            ClearEntries();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearEntries();
        }

        /// <summary>
        /// Clear all entries' input
        /// </summary>
        private void ClearEntries()
        {
            foreach (var ctrl in WindowToBeClear.Children)
            {
                if (ctrl.GetType() == typeof(TextBox))
                {
                    (ctrl as TextBox).Clear();
                }
                if (ctrl.GetType() == typeof(ComboBox))
                {
                    (ctrl as ComboBox).SelectedItem = null;
                }
                if (ctrl.GetType().IsSubclassOf(typeof(Panel)))
                {
                    foreach (var childCtrl in (ctrl as Panel).Children)
                    {
                        if (childCtrl.GetType() == typeof(TextBox))
                        {
                            (childCtrl as TextBox).Clear();
                        }
                        if (childCtrl.GetType() == typeof(ComboBox))
                        {
                            (childCtrl as ComboBox).SelectedItem = null;
                        }
                        if (childCtrl.GetType() == typeof(DatePicker))
                        {
                            (childCtrl as DatePicker).SelectedDate = null;
                        }
                    }
                }
            }
        }

        private void AddBook_Closing(object sender, CancelEventArgs e)
        {
            bool containData = false;
            foreach (var ctrl in WindowToBeClear.Children)
            {
                if (ctrl.GetType() == typeof(TextBox))
                {
                    if (!string.IsNullOrWhiteSpace((ctrl as TextBox).Text))
                    {
                        containData |= true;
                        break;
                    }
                }
                if (ctrl.GetType() == typeof(ComboBox))
                {
                    if ((ctrl as ComboBox).SelectedItem != null)
                    {
                        containData |= true;
                        break;
                    }
                }
                if (ctrl.GetType().IsSubclassOf(typeof(Panel)))
                {
                    foreach (var childCtrl in (ctrl as Panel).Children)
                    {
                        if (childCtrl.GetType() == typeof(TextBox))
                        {
                            if (!string.IsNullOrWhiteSpace((childCtrl as TextBox).Text))
                            {
                                containData |= true;
                                break;
                            }
                        }
                        if (childCtrl.GetType() == typeof(ComboBox))
                        {
                            if ((childCtrl as ComboBox).SelectedItem != null)
                            {
                                containData |= true;
                                break;
                            }
                        }
                        if (childCtrl.GetType() == typeof(DatePicker))
                        {
                            if ((childCtrl as DatePicker).SelectedDate != null)
                            {
                                containData |= true;
                                break;
                            }
                        }
                    }
                    if (containData)
                    {
                        break;
                    }
                }
            }

            if (containData)
            {
                string msg = "Book is not completely added. Discard?";
                MessageBoxResult result = MessageBox.Show(msg, "Cancel?", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        private async void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            string message;
            string caption;
            AddBookModel context = this.DataContext as AddBookModel;
            if (IsDataToAdd(context))
            {
                int noAuthor = NumberOfAuthor(context);
                int noSignature = NumberOfSignature(context);
                if (noAuthor != 0 && noSignature != 0)
                {
                    Book book = new Book(noAuthor, noSignature, context.Number, context.Title, context.Publisher, context.Version,
                        context.Year, context.Medium, context.Date, context.Place, context.Pages, context.Price);
                    book.Authors = new Author[book.NoAuthor];
                    for (int i = 0; i < book.NoAuthor; i++)
                    {
                        book.Authors[i] = new Author();
                    }
                    book.Signatures = new string[book.NoSignature];
                    if (book.NoAuthor > 0)
                    {
                        book.Authors[0].Name = context.Author1;
                    }
                    if (book.NoAuthor > 1)
                    {
                        book.Authors[1].Name = context.Author2;
                    }
                    if (book.NoAuthor > 2)
                    {
                        book.Authors[2].Name = context.Author3;
                    }
                    if (book.NoSignature > 0)
                    {
                        book.Signatures[0] = context.Signature1;
                    }
                    if (book.NoSignature > 1)
                    {
                        book.Signatures[1] = context.Signature2;
                    }
                    if (book.NoSignature > 2)
                    {
                        book.Signatures[2] = context.Signature3;
                    }

                    SqliteConnection con;
                    SqlMethods.SqlConnect(out con);
                    var selectCommand = con.CreateCommand();
                    selectCommand.CommandText = $"SELECT * FROM Books WHERE Title = '{book.Title}' AND Version = {book.Version} AND Medium = '{book.Medium}' AND DayBought = '{book.DayBought}'";
                    SqliteDataReader r = selectCommand.ExecuteReader();

                    if (r.Read())
                    {
                        message = "Book is already added! :) :)";
                        caption = "Warning";
                        MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        await Task.Run(() =>
                        {
                            var tr = con.BeginTransaction();
                            AddBookToDatabase(ref con, ref tr, book);
                            tr.Commit();
                            con.Close();
                        });
                        message = "Book is successfully added";
                        caption = "Information";
                        MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Information);
                        UpdateListBook(context);
                        ClearEntries();
                    }
                }
                else
                {
                    message = "Next author or next signature should be put correctly :) :)";
                    caption = "Error";
                    MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                message = "Book does not have enough information!";
                caption = "Error";
                MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        /// <summary>
        /// Check if all neccessary entries are filled
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private bool IsDataToAdd(AddBookModel context)
        {
            return !(context.Number == 0)
                && !string.IsNullOrWhiteSpace(context.Signature1)
                && !string.IsNullOrWhiteSpace(context.Author1)
                && !string.IsNullOrWhiteSpace(context.Title)
                && !string.IsNullOrWhiteSpace(context.Publisher)
                && !(context.Version == 0)
                && !(context.Year == 0)
                && !string.IsNullOrWhiteSpace(context.Medium)
                && !string.IsNullOrWhiteSpace(context.Place)
                && !string.IsNullOrWhiteSpace(context.Date)
                && !(context.Pages == 0)
                && !(context.Price == 0.00);
        }

        private int NumberOfAuthor(AddBookModel context)
        {
            if (!string.IsNullOrWhiteSpace(context.Author3) && !string.IsNullOrWhiteSpace(context.Author2))
            {
                return 3;
            }
            else if (!string.IsNullOrWhiteSpace(context.Author2))
            {
                return 2;
            }
            else if (string.IsNullOrWhiteSpace(context.Author3) && string.IsNullOrWhiteSpace(context.Author2))
            {
                return 1;
            }
            return 0;
        }

        private int NumberOfSignature(AddBookModel context)
        {
            if (!string.IsNullOrWhiteSpace(context.Signature3) && !string.IsNullOrWhiteSpace(context.Signature2))
            {
                return 3;
            }
            else if (!string.IsNullOrWhiteSpace(context.Signature2))
            {
                return 2;
            }
            else if (string.IsNullOrWhiteSpace(context.Signature3) && string.IsNullOrWhiteSpace(context.Signature2))
            {
                return 1;
            }
            return 0;
        }

        private void BoxDates_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTimeFormatInfo dtfi = CultureInfo.CreateSpecificCulture("fr-FR").DateTimeFormat;
            //BoxDate.Text = BoxDates.SelectedDate.Value.ToString("d", dtfi);
            if (BoxDates.SelectedDate != null)
            {
                (this.DataContext as AddBookModel).Date = BoxDates.SelectedDate.Value.ToString("d", dtfi);
            }
        }

        private void UpdateListBook(AddBookModel context)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                context.ListBook.Clear();
            });
            SqliteConnection con;
            SqlMethods.SqlConnect(out con);
            var updateCommand = con.CreateCommand();
            updateCommand.CommandText = "SELECT Number FROM Books ORDER BY BookId DESC";
            SqliteDataReader r = updateCommand.ExecuteReader();
            int i = 20;
            while (r.Read() && i-- > 0)
            {
                int result;
                int.TryParse(Convert.ToString(r["Number"]), out result);
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    context.ListBook.Add(result);
                });
            }
            con.Close();
        }

        private async void BtnAddTest_Click(object sender, RoutedEventArgs e)
        {
            AddBookModel context = this.DataContext as AddBookModel;
            string[] place = { "Frankfurt", "Darmstadt", "Muenchen", "Berlin" };
            string date = "01/01/2019";
            string[] medium = { "Text book", "CD", "DVD", "E-book" };
            Random rnd = new Random();
            Book book;
            await Task.Run(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    book = new Book() { Number = i };
                    book.NoAuthor = rnd.Next(1, 4);
                    book.NoSignature = rnd.Next(1, 4);
                    book.Authors = new Author[book.NoAuthor];
                    for (int j = 0; j < book.NoAuthor; j++)
                    {
                        book.Authors[j] = new Author() { Name = $"Author {rnd.Next(1, 100)}" };
                    }
                    book.Signatures = new string[book.NoSignature];
                    for (int j = 0; j < book.NoSignature; j++)
                    {
                        char c = (char)rnd.Next(65, 91);
                        char[] str = { c, c, c, c };
                        book.Signatures[j] = new string(str);
                    }

                    book.Title = $"Book {i}";
                    book.Publisher = $"Publisher {rnd.Next(1, 100)}";
                    book.Version = rnd.Next(1, 11);
                    book.Year = rnd.Next(1900, 2020);
                    book.Medium = medium[rnd.Next(0, 4)];
                    book.Place = place[rnd.Next(0, 4)];
                    book.DayBought = date;
                    book.Pages = rnd.Next(10, 501);
                    book.Price = Math.Round(rnd.NextDouble() * 100, 2);

                    SqliteConnection con;
                    SqlMethods.SqlConnect(out con);
                    var tr = con.BeginTransaction();
                    AddBookToDatabase(ref con, ref tr, book);
                    tr.Commit();
                    con.Close();
                    UpdateListBook(context);
                }
            });
            string message = "100 Books are successfully added";
            string caption = "Information";
            MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Information);
            
        }

        public static void AddBookToDatabase(ref SqliteConnection con, ref SqliteTransaction tr, Book book)
        {
            var insertCommand = con.CreateCommand();
            insertCommand.Transaction = tr;
            insertCommand.CommandText = "INSERT INTO Books (Number, Title, Publisher, Version, Year, Medium, Place, DayBought, Pages, Price) " +
                "VALUES (@Number,@Title,@Publisher,@Version,@Year,@Medium,@Place,@Date,@Pages,@Price)";
            insertCommand.Parameters.AddWithValue("Number", book.Number);
            insertCommand.Parameters.AddWithValue("Title", book.Title);
            insertCommand.Parameters.AddWithValue("Publisher", book.Publisher);
            insertCommand.Parameters.AddWithValue("Version", book.Version);
            insertCommand.Parameters.AddWithValue("Year", book.Year);
            insertCommand.Parameters.AddWithValue("Medium", book.Medium);
            insertCommand.Parameters.AddWithValue("Place", book.Place);
            insertCommand.Parameters.AddWithValue("Date", book.DayBought);
            insertCommand.Parameters.AddWithValue("Pages", book.Pages);
            insertCommand.Parameters.AddWithValue("Price", book.Price);
            insertCommand.ExecuteNonQuery();
            insertCommand.Parameters.Clear();
            Thread.Sleep(TimeSpan.FromTicks(5));
            SqliteCommand selectCommand;
            SqliteDataReader r;
            for (int i = 0; i < book.NoAuthor; i++)
            {
                selectCommand = con.CreateCommand();
                selectCommand.CommandText = $"SELECT * FROM Authors WHERE Name = '{book.Authors[i].Name}'";
                r = selectCommand.ExecuteReader();
                if (!r.Read())
                {
                    insertCommand.CommandText = "INSERT INTO Authors (Name) VALUES (@Name)";
                    insertCommand.Parameters.AddWithValue("Name", book.Authors[i].Name);
                    insertCommand.ExecuteNonQuery();
                    insertCommand.Parameters.Clear();
                }
                insertCommand.CommandText = $"INSERT INTO Books_Authors (BookId, AuthorId, Priority) VALUES ((SELECT BookId FROM Books " +
                    $"WHERE Title = '{book.Title}' AND Version = '{book.Version}' AND Medium = '{book.Medium}' " +
                    $"AND Publisher = '{book.Publisher}' AND Year = '{book.Year}' AND Pages = {book.Pages})," +
                    $"(SELECT AuthorId FROM Authors WHERE Name = '{book.Authors[i].Name}'),{i + 1})";
                insertCommand.ExecuteNonQuery();
                Thread.Sleep(TimeSpan.FromTicks(50));
            }

            for (int i = 0; i < book.NoSignature; i++)
            {
                selectCommand = con.CreateCommand();
                selectCommand.CommandText = $"SELECT * FROM Signatures WHERE Signature = '{book.Signatures[i]}'";
                r = selectCommand.ExecuteReader();
                if (!r.Read())
                {
                    insertCommand.CommandText = "INSERT INTO Signatures (Signature) VALUES (@Signature)";
                    insertCommand.Parameters.AddWithValue("Signature", book.Signatures[i]);
                    insertCommand.ExecuteNonQuery();
                    insertCommand.Parameters.Clear();
                }
                insertCommand.CommandText = $"INSERT INTO Books_Signatures (BookId, SignatureId, Priority) VALUES ((SELECT BookId FROM Books " +
                    $"WHERE Title = '{book.Title}' AND Version = '{book.Version}' AND Medium = '{book.Medium}' " +
                    $"AND Publisher = '{book.Publisher}' AND Year = '{book.Year}' AND Pages = {book.Pages})," +
                    $"(SELECT SignatureId FROM Signatures WHERE Signature = '{book.Signatures[i]}'), {i + 1})";
                insertCommand.ExecuteNonQuery();
                Thread.Sleep(TimeSpan.FromTicks(50));
            }
        }

        private async void ReadPublisherMediumPlace()
        {
            string dataFolder = Path.Combine(AppContext.BaseDirectory, "Data");
            string publisherPath = Path.Combine(dataFolder, "Publishers.dat");
            string mediumPath = Path.Combine(dataFolder, "Mediums.dat");
            string placePath = Path.Combine(dataFolder, "Places.dat");

            using (StreamReader sr = new StreamReader(publisherPath))
            {
                while (!sr.EndOfStream)
                {
                    ListPublisher.Add(await sr.ReadLineAsync());
                }
            }

            using (StreamReader sr = new StreamReader(mediumPath))
            {
                while (!sr.EndOfStream)
                {
                    ListMedium.Add(await sr.ReadLineAsync());
                }
            }

            using (StreamReader sr = new StreamReader(placePath))
            {
                while (!sr.EndOfStream)
                {
                    ListPlace.Add(await sr.ReadLineAsync());
                }
            }
        }
    }
}
