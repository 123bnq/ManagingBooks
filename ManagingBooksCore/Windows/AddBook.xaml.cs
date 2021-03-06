﻿using System;
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
using WPFCustomMessageBox;
using System.Windows.Data;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Data.SQLite;
using iText.Kernel.Pdf;
using iText.Kernel.Geom;
using iText.Layout;
using iText.Layout.Element;
using iText.Barcodes;
using System.Diagnostics;

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

        public string Number { get; set; }

        string NotAvalable = "N/A";

        public AddBook()
        {
            this.DataContext = new AddBookModel();
            (this.DataContext as AddBookModel).ListBook = new ObservableCollection<int>();
            InitializeComponent();
            BoxPublisher.ItemsSource = ListPublisher;
            BoxMedium.ItemsSource = ListMedium;
            BoxPlace.ItemsSource = ListPlace;
            ReadPublisherMediumPlace(ListPublisher, ListMedium, ListPlace);
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
            AddBookModel context = this.DataContext as AddBookModel;
            PropertyInfo[] properties = context.GetType().GetProperties();
            DateTimeFormatInfo dtfi = CultureInfo.CreateSpecificCulture("fr-FR").DateTimeFormat;


            foreach (var propertyInfo in properties)
            {
                if (propertyInfo.PropertyType == typeof(string))
                {
                    propertyInfo.SetValue(this.DataContext as AddBookModel, String.Empty, null);
                }
                else if (propertyInfo.PropertyType == typeof(int))
                {
                    propertyInfo.SetValue(this.DataContext as AddBookModel, 0, null);
                }
                else if (propertyInfo.PropertyType == typeof(double))
                {
                    propertyInfo.SetValue(this.DataContext as AddBookModel, 0, null);
                }
            }

            foreach (var ctrl in WindowToBeClear.Children)
            {
                if (ctrl.GetType().IsSubclassOf(typeof(Panel)))
                {
                    foreach (var childCtrl in (ctrl as Panel).Children)
                    {
                        if (childCtrl.GetType() == typeof(DatePicker))
                        {
                            (childCtrl as DatePicker).SelectedDate = new DateTime(1970, 1, 1);
                        }
                    }
                }
            }
            context.Author1 = NotAvalable;
            context.Signature1 = NotAvalable;
            context.Place = NotAvalable;
            context.Medium = NotAvalable;
            context.Publisher = NotAvalable;
            context.Date = BoxDates.SelectedDate.Value.ToString("d", dtfi);

            //foreach (var ctrl in WindowToBeClear.Children)
            //{
            //    if (ctrl.GetType() == typeof(TextBox))
            //    {
            //        (ctrl as TextBox).Clear();
            //    }
            //    if (ctrl.GetType() == typeof(ComboBox))
            //    {
            //        (ctrl as ComboBox).SelectedItem = null;
            //    }
            //    if (ctrl.GetType().IsSubclassOf(typeof(Panel)))
            //    {
            //        foreach (var childCtrl in (ctrl as Panel).Children)
            //        {
            //            if (childCtrl.GetType() == typeof(TextBox))
            //            {
            //                (childCtrl as TextBox).Clear();
            //            }
            //            if (childCtrl.GetType() == typeof(ComboBox))
            //            {
            //                (childCtrl as ComboBox).SelectedItem = null;
            //            }
            //            if (childCtrl.GetType() == typeof(DatePicker))
            //            {
            //                (childCtrl as DatePicker).SelectedDate = new DateTime(1970, 1, 1);
            //            }
            //            if (childCtrl.GetType().IsSubclassOf(typeof(Panel)))
            //            {
            //                foreach (var ccCtrl in (childCtrl as Panel).Children)
            //                {
            //                    if (ccCtrl.GetType() == typeof(ComboBox))
            //                    {
            //                        (ccCtrl as ComboBox).SelectedItem = null;
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
        }

        private void AddBook_Closing(object sender, CancelEventArgs e)
        {
            bool containData = false;
            AddBookModel context = this.DataContext as AddBookModel;
            containData = context.Number != 0
                || context.Signature1 != NotAvalable
                || !string.IsNullOrWhiteSpace(context.Signature2)
                || !string.IsNullOrWhiteSpace(context.Signature3)
                || context.Author1 != NotAvalable
                || !string.IsNullOrWhiteSpace(context.Author2)
                || !string.IsNullOrWhiteSpace(context.Author3)
                || !string.IsNullOrWhiteSpace(context.Title)
                || context.Publisher != NotAvalable
                || context.Version != 0
                || context.Year != 0
                || context.Medium != NotAvalable
                || context.Place != NotAvalable
                || context.Date != "01/01/1970"
                || context.Pages != 0
                || context.Price != 0;

            if (containData)
            {
                string message = Application.Current.FindResource("AddBook.CodeBehind.WarningCancel.Message").ToString();
                string caption = Application.Current.FindResource("AddBook.CodeBehind.WarningCancel.Caption").ToString();
                MessageBoxResult result = CustomMessageBox.ShowYesNo(message, caption, CustomMessageBoxButton.Yes, CustomMessageBoxButton.No, MessageBoxImage.Warning);
                //MessageBoxResult result = MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Warning);
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

                    SQLiteConnection con;
                    SqlMethods.SqlConnect(out con);
                    var selectCommand = con.CreateCommand();
                    //selectCommand.CommandText = $"SELECT * FROM Books WHERE Number = {book.Number} OR Title = '{book.Title}' AND Version = {book.Version} AND Medium = '{book.Medium}' AND DayBought = '{book.DayBought}'";
                    selectCommand.CommandText = $"SELECT * FROM Books WHERE Number = @Number";
                    selectCommand.Parameters.AddWithValue("Number", book.Number);
                    //SqlMethods.LogSqlAddCommand(selectCommand.CommandText);
                    SQLiteDataReader r = selectCommand.ExecuteReader();

                    if (r.Read())
                    {
                        message = Application.Current.FindResource("AddBook.CodeBehind.WarningAdd.Message").ToString();
                        caption = Application.Current.FindResource("AddBook.CodeBehind.WarningAdd.Caption").ToString();
                        CustomMessageBox.ShowOK(message, caption, CustomMessageBoxButton.OK, MessageBoxImage.Warning);
                        //MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        BtnAdd.IsEnabled = false;
                        await Task.Run(() =>
                        {

                            var tr = con.BeginTransaction();
                            AddBookToDatabase(ref con, ref tr, book);
                            tr.Commit();

                        });
                        string number = book.Number.ToString();
                        string signature = string.Join(" - ", book.Signatures);

                        int length = number.Length;
                        for (int i = 6; i > length; i--)
                        {
                            number = String.Concat("0", number);
                        }
                        Number = number;
                        message = Application.Current.FindResource("AddBook.CodeBehind.InfoAdd.Message").ToString();
                        caption = Application.Current.FindResource("AddBook.CodeBehind.InfoAdd.Caption").ToString();
                        var result = CustomMessageBox.ShowOK(message, caption, CustomMessageBoxButton.OK, MessageBoxImage.Information);

                        //if (result == MessageBoxResult.Yes)
                        //{
                        //    MainWindow.CreateBarcodeImage(number, signature, isOpen: true);
                        //}
                        BtnAdd.IsEnabled = true;
                        UpdateListBook(context);
                        ClearEntries();
                    }
                    r.Close();
                    con.Close();
                }
                else
                {
                    message = Application.Current.FindResource("AddBook.CodeBehind.ErrorAdd.IncorrectOrder.Message").ToString();
                    caption = Application.Current.FindResource("AddBook.CodeBehind.ErrorAdd.IncorrectOrder.Caption").ToString();
                    CustomMessageBox.ShowOK(message, caption, CustomMessageBoxButton.OK, MessageBoxImage.Error);
                    //MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                message = Application.Current.FindResource("AddBook.CodeBehind.ErrorAdd.NotEnough.Message").ToString();
                caption = Application.Current.FindResource("AddBook.CodeBehind.ErrorAdd.NotEnough.Caption").ToString();
                CustomMessageBox.ShowOK(message, caption, CustomMessageBoxButton.OK, MessageBoxImage.Error);
                //MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Error);
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
                && !string.IsNullOrWhiteSpace(context.Title);
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
            SQLiteConnection con;
            SqlMethods.SqlConnect(out con);
            var updateCommand = con.CreateCommand();
            updateCommand.CommandText = "SELECT Number FROM Books ORDER BY BookId DESC";
            SQLiteDataReader r = updateCommand.ExecuteReader();
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
            r.Close();
            con.Close();
        }

        // *** not used *** generate random data and add to DB
        private async void BtnAddTest_Click(object sender, RoutedEventArgs e)
        {
            AddBookModel context = this.DataContext as AddBookModel;
            string[] place = { "Frankfurt", "Darmstadt", "Muenchen", "Berlin" };
            string date = "01/01/2019";
            string[] medium = { "Text book", "CD", "DVD", "E-book" };
            Random rnd = new Random();
            SqlMethods.SqlConnect(out SQLiteConnection connect);
            SQLiteCommand selectCommand = connect.CreateCommand();
            selectCommand.CommandText = "SELECT count(*) FROM Mediums";
            SQLiteDataReader reader = selectCommand.ExecuteReader();
            reader.Read();
            int temp;
            int.TryParse(Convert.ToString(reader["count(*)"]), out temp);
            int maxMedium = temp;
            reader.Close();
            selectCommand.CommandText = "SELECT count(*) FROM Places";
            reader = selectCommand.ExecuteReader();
            reader.Read();
            int.TryParse(Convert.ToString(reader["count(*)"]), out temp);
            int maxPlace = temp;
            reader.Close();
            selectCommand.CommandText = "SELECT count(*) FROM Signatures";
            reader = selectCommand.ExecuteReader();
            reader.Read();
            int.TryParse(Convert.ToString(reader["count(*)"]), out temp);
            int maxSig = temp;
            reader.Close();
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

                    SQLiteConnection con;
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
            CustomMessageBox.ShowOK(message, caption, CustomMessageBoxButton.OK, MessageBoxImage.Information);
            //MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Information);
        }
        // *** not used ***

        public static void AddBookToDatabase(ref SQLiteConnection con, ref SQLiteTransaction tr, Book book)
        {
            var insertCommand = con.CreateCommand();
            insertCommand.Transaction = tr;
            insertCommand.CommandText = "INSERT INTO Books (Number, Title, Publisher, Version, Year, Medium, Place, DayBought, Pages, Price) " +
                "VALUES (@Number,@Title,@Publisher,@Version,@Year,@Medium,@Place,@Date,@Pages,@Price);";
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
            //SqlMethods.LogSqlAddCommand(insertCommand.CommandText);
            //string textPara = string.Empty;
            //for (int index = 0; index < insertCommand.Parameters.Count; index++)
            //{
            //    textPara += insertCommand.Parameters[index].Value + ", ";
            //}
            //SqlMethods.LogSqlAddCommand(textPara);
            insertCommand.ExecuteNonQuery();
            
            insertCommand.Parameters.Clear();
            Thread.Sleep(TimeSpan.FromTicks(5));
            SQLiteCommand selectCommand;
            SQLiteDataReader r;
            long maxId = 0;
            selectCommand = con.CreateCommand();
            selectCommand.CommandText = "SELECT last_insert_rowid();";
            maxId = (long)selectCommand.ExecuteScalar();
            //selectCommand.CommandText = $"SELECT BookId FROM Books WHERE Title = @Title";
            //selectCommand.Parameters.AddWithValue("Title", book.Title);
            ////SqlMethods.LogSqlAddCommand(selectCommand.CommandText);
            //r = selectCommand.ExecuteReader();
            //selectCommand.Parameters.Clear();
            //while (r.Read())
            //{
            //    int.TryParse(Convert.ToString(r["BookId"]), out maxId);
            //}
            //r.Close();
            for (int i = 0; i < book.NoAuthor; i++)
            {
                selectCommand = con.CreateCommand();
                selectCommand.CommandText = $"SELECT * FROM Authors WHERE Name = @Author";
                selectCommand.Parameters.AddWithValue("Author", book.Authors[i].Name);
                //SqlMethods.LogSqlAddCommand(selectCommand.CommandText);
                r = selectCommand.ExecuteReader();
                selectCommand.Parameters.Clear();
                if (!r.Read())
                {
                    insertCommand.CommandText = "INSERT INTO Authors (Name) VALUES (@Name)";
                    insertCommand.Parameters.AddWithValue("Name", book.Authors[i].Name);
                    //SqlMethods.LogSqlAddCommand(insertCommand.CommandText);
                    //textPara = string.Empty;
                    //for (int index = 0; index < insertCommand.Parameters.Count; index++)
                    //{
                    //    textPara += insertCommand.Parameters[index].Value + ", ";
                    //}
                    //SqlMethods.LogSqlAddCommand(textPara);
                    insertCommand.ExecuteNonQuery();
                    insertCommand.Parameters.Clear();
                }
                r.Close();
                //insertCommand.CommandText = $"INSERT INTO Books_Authors (BookId, AuthorId, Priority) VALUES ((SELECT BookId FROM Books " +
                //    $"WHERE Title = '{book.Title}' AND Version = '{book.Version}' AND Medium = '{book.Medium}' " +
                //    $"AND Publisher = '{book.Publisher}' AND Year = '{book.Year}' AND Pages = {book.Pages})," +
                //    $"(SELECT AuthorId FROM Authors WHERE Name = '{book.Authors[i].Name}'),{i + 1})";
                insertCommand.CommandText = $"INSERT INTO Books_Authors (BookId, AuthorId, Priority) VALUES (@maxId," +
                    $"(SELECT AuthorId FROM Authors WHERE Name = @Author),@Priority)";
                insertCommand.Parameters.AddWithValue("maxId", maxId);
                insertCommand.Parameters.AddWithValue("Author", book.Authors[i].Name);
                insertCommand.Parameters.AddWithValue("Priority", i + 1);
                //SqlMethods.LogSqlAddCommand(insertCommand.CommandText);
                insertCommand.ExecuteNonQuery();
                insertCommand.Parameters.Clear();
                Thread.Sleep(TimeSpan.FromTicks(50));
            }

            for (int i = 0; i < book.NoSignature; i++)
            {
                selectCommand = con.CreateCommand();
                selectCommand.CommandText = $"SELECT * FROM Signatures WHERE Signature = @Signature";
                selectCommand.Parameters.AddWithValue("Signature", book.Signatures[i]);
                //SqlMethods.LogSqlAddCommand(selectCommand.CommandText);
                r = selectCommand.ExecuteReader();
                selectCommand.Parameters.Clear();
                if (!r.Read())
                {
                    insertCommand.CommandText = "INSERT INTO Signatures (Signature, Info) VALUES (@Signature, @Info)";
                    insertCommand.Parameters.AddWithValue("Signature", book.Signatures[i]);
                    insertCommand.Parameters.AddWithValue("Info", book.Signatures[i]);
                    //SqlMethods.LogSqlAddCommand(insertCommand.CommandText);
                    //textPara = string.Empty;
                    //for (int index = 0; index < insertCommand.Parameters.Count; index++)
                    //{
                    //    textPara += insertCommand.Parameters[index].Value + ", ";
                    //}
                    //SqlMethods.LogSqlAddCommand(textPara);
                    insertCommand.ExecuteNonQuery();
                    insertCommand.Parameters.Clear();
                }
                //insertCommand.CommandText = $"INSERT INTO Books_Signatures (BookId, SignatureId, Priority) VALUES ((SELECT BookId FROM Books " +
                //    $"WHERE Title = '{book.Title}' AND Version = '{book.Version}' AND Medium = '{book.Medium}' " +
                //    $"AND Publisher = '{book.Publisher}' AND Year = '{book.Year}' AND Pages = {book.Pages})," +
                //    $"(SELECT SignatureId FROM Signatures WHERE Signature = '{book.Signatures[i]}'), {i + 1})";
                insertCommand.CommandText = $"INSERT INTO Books_Signatures (BookId, SignatureId, Priority) VALUES (@maxId," +
                    $"(SELECT SignatureId FROM Signatures WHERE Signature = @Signature), @Priority)";
                insertCommand.Parameters.AddWithValue("maxId", maxId);
                insertCommand.Parameters.AddWithValue("Signature", book.Signatures[i]);
                insertCommand.Parameters.AddWithValue("Priority", i + 1);
                //SqlMethods.LogSqlAddCommand(insertCommand.CommandText);
                insertCommand.ExecuteNonQuery();
                insertCommand.Parameters.Clear();
                Thread.Sleep(TimeSpan.FromTicks(50));
            }
        }

        public static void ReadPublisherMediumPlace(ObservableCollection<string> listPubl, ObservableCollection<string> listMedi, ObservableCollection<string> listPlace)
        {
            SqlMethods.SqlConnect(out SQLiteConnection con);
            var selectCommand = con.CreateCommand();
            selectCommand.CommandText = "SELECT Name FROM Publishers ORDER BY Name";
            SQLiteDataReader r = selectCommand.ExecuteReader();
            while (r.Read())
            {
                listPubl.Add(Convert.ToString(r["Name"]));
            }
            r.Close();

            selectCommand.CommandText = "SELECT Name FROM Mediums ORDER BY Name";
            r = selectCommand.ExecuteReader();
            while (r.Read())
            {
                listMedi.Add(Convert.ToString(r["Name"]));
            }
            r.Close();

            selectCommand.CommandText = "SELECT City, State, Country FROM Places";
            r = selectCommand.ExecuteReader();
            while (r.Read())
            {
                string city = Convert.ToString(r["City"]);
                string state = Convert.ToString(r["State"]);
                string country = Convert.ToString(r["Country"]);
                if (string.IsNullOrEmpty(state) && string.IsNullOrEmpty(country))
                {
                    listPlace.Add(city);
                }
                else if (string.IsNullOrEmpty(state))
                {
                    listPlace.Add($"{city}, {country}");
                }
                else if (string.IsNullOrEmpty(country))
                {
                    listPlace.Add($"{city} ({state})");
                }
                else
                {
                    listPlace.Add($"{city}, {state}, {country}");
                }
            }
            r.Close();
            con.Close();
        }

        private void BtnSelectSignature_Click(object sender, RoutedEventArgs e)
        {
            AddBookModel context = this.DataContext as AddBookModel;
            context.Signature1 = string.Empty;
            context.Signature2 = string.Empty;
            context.Signature3 = string.Empty;
            new ChooseSignatures(context)
            {
                Owner = this,
                Left = this.Left + 10,
                Top = this.Top + 150
            }.ShowDialog();
        }

        /// <summary>
        /// Allowing keys to be used in textbox for inputing integer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IntNumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        /// <summary>
        /// Allowing keys to be used in textbox for inputing float
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DecimalNumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("^[.][0-9]+$|^[0-9]*[.]{0,1}[0-9]*$");
            e.Handled = !regex.IsMatch(e.Text);
        }


    }
}
