using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;
using Microsoft.Data.Sqlite;
using ManagingBooks.Model;
using System.Globalization;
using System.Collections.ObjectModel;

namespace ManagingBooks.Windows
{
    /// <summary>
    /// Interaction logic for AddBook.xaml
    /// </summary>
    public partial class AddBook : Window
    {

        public AddBook()
        {
            this.DataContext = new AddBookModel();
            (this.DataContext as AddBookModel).ListBook = new ObservableCollection<int>();
            InitializeComponent();
            //ListNumber.ItemsSource = ListBook;
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

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            string message;
            AddBookModel context = this.DataContext as AddBookModel;
            if (IsDataToAdd(context))
            {
                int noAuthor = NumberOfAuthor(context);
                int noSignature = NumberOfSignature(context);
                if (noAuthor != 0 && noSignature != 0)
                {
                    Book book = new Book(noAuthor, noSignature, context.Number, context.Title, context.Publisher, context.Version, 
                        context.Year, context.Medium, context.Date, context.Place, context.Pages, context.Price);
                    book.Authors = new Author[noAuthor];
                    for (int i = 0; i < noAuthor; i++)
                    {
                        book.Authors[i] = new Author();
                    }
                    book.Signatures = new string[noSignature];
                    if (noAuthor > 0)
                    {
                        book.Authors[0].FirstName = context.Author1;
                        book.Authors[0].LastName = context.Author1;
                    }
                    if (noAuthor > 1)
                    {
                        book.Authors[1].FirstName = context.Author2;
                        book.Authors[1].LastName = context.Author2;
                    }
                    if (noAuthor > 2)
                    {
                        book.Authors[2].FirstName = context.Author3;
                        book.Authors[2].LastName = context.Author3;
                    }
                    if (noSignature > 0)
                    {
                        book.Signatures[0] = context.Signature1;
                    }
                    if(noSignature > 1)
                    {
                        book.Signatures[1] = context.Signature2;
                    }
                    if(noSignature > 2)
                    {
                        book.Signatures[2] = context.Signature3;
                    }
                    SqliteConnection con;
                    SqlConnect(out con);
                    //book.Authors = new Author[noAuthor];

                    var transaction = con.BeginTransaction();
                    var insertCommand = con.CreateCommand();
                    insertCommand.CommandText = "INSERT INTO Books (Number, Title, Publisher, Version, Year, Medium, DayBought, Pages, Price) " +
                        "VALUES (@Number,@Title,@Publisher,@Version,@Year,@Medium,@Date,@Pages,@Price)";
                    insertCommand.Parameters.AddWithValue("Number", book.Number);
                    insertCommand.Parameters.AddWithValue("Title", book.Title);
                    insertCommand.Parameters.AddWithValue("Publisher", book.Publisher);
                    insertCommand.Parameters.AddWithValue("Version", book.Version);
                    insertCommand.Parameters.AddWithValue("Year", book.Year);
                    insertCommand.Parameters.AddWithValue("Medium", book.Medium);
                    insertCommand.Parameters.AddWithValue("Date", book.DayBought);
                    insertCommand.Parameters.AddWithValue("Pages", book.Pages);
                    insertCommand.Parameters.AddWithValue("Price", book.Price);
                    insertCommand.ExecuteNonQuery();

                    for (int i = 0; i < noAuthor; i++)
                    {
                        insertCommand = con.CreateCommand();
                        insertCommand.CommandText = "INSERT INTO Authors (FirstName, LastName) VALUES (@FirstName,@LastName)";
                        insertCommand.Parameters.AddWithValue("FirstName", book.Authors[i].FirstName);
                        insertCommand.Parameters.AddWithValue("LastName", book.Authors[i].LastName);
                        insertCommand.ExecuteNonQuery();
                        insertCommand = con.CreateCommand();
                        insertCommand.CommandText = $"INSERT INTO Books_Authors (BookId, AuthorId) VALUES ((SELECT BookId FROM Books " +
                            $"WHERE Title = '{book.Title}' AND Version = '{book.Version}' AND Medium = '{book.Medium}')," +
                            $"(SELECT AuthorId FROM Authors WHERE FirstName = '{book.Authors[i].FirstName}' AND LastName = '{book.Authors[i].LastName}'))";
                        insertCommand.ExecuteNonQuery();
                    }

                    for (int i = 0; i < noSignature; i++)
                    {
                        insertCommand = con.CreateCommand();
                        insertCommand.CommandText = $"INSERT INTO Signatures (BookId, Signature) VALUES ((SELECT BookId FROM Books " +
                            $"WHERE Title = '{book.Title}' AND Version = '{book.Version}' AND Medium = '{book.Medium}'),@Signature)";
                        insertCommand.Parameters.AddWithValue("Signature", book.Signatures[i]);
                        insertCommand.ExecuteNonQuery();
                    }
                    
                    transaction.Commit();
                    con.Close();
                    ClearEntries();
                    UpdateListBook(context);
                    message = "Book is successfully added";
                    MessageBox.Show(message, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    message = "Next author or next signature should be put correctly :) :)";
                    MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                message = "Book does not have enough information!";
                MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void SqlConnect(out SqliteConnection con)
        {
            string path = System.IO.Path.Combine(AppContext.BaseDirectory, "Database.db");
            con = new SqliteConnection("" + new SqliteConnectionStringBuilder
            {
                DataSource = "Database.db"
            });
            con.Open();
            //MessageBox.Show("Connection opened");
            //var transaction = con.BeginTransaction();
            //var insertCommand = con.CreateCommand();
            //insertCommand.Transaction = transaction;
            //insertCommand.CommandText = "INSERT INTO [Authors]([FirstName],[LastName]) VALUES ('James','Quin');";
            //insertCommand.ExecuteNonQuery();
            //transaction.Commit();
            //con.Close();
            //MessageBox.Show("Connection closed");
        }

        /// <summary>
        /// Check if all neccessary entries are filled
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private bool IsDataToAdd(AddBookModel context)
        {
            return !string.IsNullOrWhiteSpace(BoxNumber.Text)
                && !string.IsNullOrWhiteSpace(context.Signature1)
                && !string.IsNullOrWhiteSpace(context.Author1)
                && !string.IsNullOrWhiteSpace(context.Title)
                && !string.IsNullOrWhiteSpace(context.Publisher)
                && !string.IsNullOrWhiteSpace(BoxVersion.Text)
                && !string.IsNullOrWhiteSpace(BoxYear.Text)
                && !string.IsNullOrWhiteSpace(context.Medium)
                && !string.IsNullOrWhiteSpace(context.Place)
                && !string.IsNullOrWhiteSpace(context.Date)
                && !string.IsNullOrWhiteSpace(BoxPage.Text)
                && !string.IsNullOrWhiteSpace(BoxPrice.Text);
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
            context.ListBook.Clear();
            SqliteConnection con;
            SqlConnect(out con);
            var updateCommand = con.CreateCommand();
            updateCommand.CommandText = "SELECT Number FROM Books ORDER BY BookId DESC";
            updateCommand.CommandType = System.Data.CommandType.Text;
            SqliteDataReader r = updateCommand.ExecuteReader();
            int i = 20;
            while (r.Read() && i-- > 0)
            {
                int result;
                int.TryParse(Convert.ToString(r["Number"]), out result);
                context.ListBook.Add(result);
            }
            con.Close();
        }
    }


}
