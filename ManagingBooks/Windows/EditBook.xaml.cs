﻿using ManagingBooks.Model;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.IO;
using System.Collections.ObjectModel;

namespace ManagingBooks.Windows
{
    /// <summary>
    /// Interaction logic for EditBook.xaml
    /// </summary>
    public partial class EditBook : Window
    {
        private Book book;
        private bool updateRequest = false;
        private bool isCancelled = false;
        private bool copied = false;
        public bool IsNew = false;

        ObservableCollection<string> ListPublisher = new ObservableCollection<string>();
        ObservableCollection<string> ListMedium = new ObservableCollection<string>();
        ObservableCollection<string> ListPlace = new ObservableCollection<string>();

        public EditBook(int bookId)
        {
            InitializeComponent();
            BoxPublisher.ItemsSource = ListPublisher;
            BoxMedium.ItemsSource = ListMedium;
            BoxPlace.ItemsSource = ListPlace;
            AddBook.ReadPublisherMediumPlace(ListPublisher, ListMedium, ListPlace);
            //ReadPublisherMediumPlace();
            this.DataContext = new AddBookModel();
            book = new Book();
            book.BookId = bookId;
            RetriveBook(this.DataContext as AddBookModel);
        }

        private void BoxDates_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTimeFormatInfo dtfi = CultureInfo.CreateSpecificCulture("fr-FR").DateTimeFormat;
            if (BoxDates.SelectedDate != null)
            {
                (this.DataContext as AddBookModel).Date = BoxDates.SelectedDate.Value.ToString("d", dtfi);
            }
        }

        private void RetriveBook(AddBookModel context)
        {
            SqlMethods.SqlConnect(out SqliteConnection con);
            var selectCommmand = con.CreateCommand();
            selectCommmand.CommandText = $"SELECT b.Number,b.Title,b.Publisher,b.Version,b.Year,b.Medium,b.Place,b.DayBought,b.Pages,b.Price,a.AuthorId,a.Name,s.Signature " +
                $"FROM Books b " +
                $"LEFT JOIN Books_Authors ba ON (ba.BookId = b.BookId) " +
                $"LEFT JOIN Authors a ON (a.AuthorId = ba.AuthorId) " +
                $"LEFT JOIN Books_Signatures bs ON (bs.BookId = b.BookId) " +
                $"LEFT JOIN Signatures s ON (s.SignatureId = bs.SignatureId) " +
                $"WHERE b.BookId = '{book.BookId}' ORDER BY ba.Priority,bs.Priority";
            SqliteDataReader r = selectCommmand.ExecuteReader();
            int result;
            List<string> authors = new List<string>();
            List<string> signatures = new List<string>();
            while (r.Read())
            {
                int.TryParse(Convert.ToString(r["Number"]), out result);

                if (book.Number != result)
                {
                    book.Number = result;
                    book.Title = Convert.ToString(r["Title"]);
                    book.Publisher = Convert.ToString(r["Publisher"]);
                    int.TryParse(Convert.ToString(r["Version"]), out result);
                    book.Version = result;
                    int.TryParse(Convert.ToString(r["Year"]), out result);
                    book.Year = result;
                    book.Medium = Convert.ToString(r["Medium"]);
                    book.Place = Convert.ToString(r["Place"]);
                    book.DayBought = Convert.ToString(r["DayBought"]);
                    int.TryParse(Convert.ToString(r["Pages"]), out result);
                    book.Pages = result;
                    double.TryParse(Convert.ToString(r["Price"]), out double result_d);
                    book.Price = result_d;
                }
                authors.Add(Convert.ToString(r["Name"]));
                signatures.Add(Convert.ToString(r["Signature"]));
            }
            authors = authors.Distinct().ToList();
            signatures = signatures.Distinct().ToList();
            book.NoAuthor = authors.Count;
            book.Authors = new Author[book.NoAuthor];

            for (int j = 0; j < book.Authors.Length; j++)
            {
                book.Authors[j] = new Author();
            }
            int i = 0;
            foreach (var a in authors)
            {
                book.Authors[i++].Name = a;
            }
            i = 0;

            book.NoSignature = signatures.Count;
            book.Signatures = new string[book.NoSignature];

            foreach (var s in signatures)
            {
                book.Signatures[i++] = s;
            }
            CopyBookToView(this.DataContext as AddBookModel, book);
            con.Close();
        }

        private void CopyBookToView(AddBookModel context, Book book)
        {
            int noAuthor = book.Authors.Length;
            int noSignature = book.Signatures.Length;

            context.Number = book.Number;
            context.Title = book.Title;
            context.Publisher = book.Publisher;
            context.Version = book.Version;
            context.Medium = book.Medium;
            context.Place = book.Place;
            context.Year = book.Year;
            context.Date = book.DayBought;
            BoxDates.SelectedDate = DateTime.Parse(context.Date, new CultureInfo("fr-FR", true));
            context.Pages = book.Pages;
            context.Price = book.Price;
            if (noAuthor > 0)
            {
                context.Author1 = book.Authors[0].Name;
            }
            else
            {
                context.Author1 = string.Empty;
            }
            if (noAuthor > 1)
            {
                context.Author2 = book.Authors[1].Name;
            }
            else
            {
                context.Author2 = string.Empty;
            }
            if (noAuthor > 2)
            {
                context.Author3 = book.Authors[2].Name;
            }
            else
            {
                context.Author3 = string.Empty;
            }
            if (noSignature > 0)
            {
                context.Signature1 = book.Signatures[0];
            }
            else
            {
                context.Signature1 = string.Empty;
            }
            if (noSignature > 1)
            {
                context.Signature2 = book.Signatures[1];
            }
            else
            {
                context.Signature2 = string.Empty;
            }
            if (noSignature > 2)
            {
                context.Signature3 = book.Signatures[2];
            }
            else
            {
                context.Signature3 = string.Empty;
            }
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            updateRequest = true;
            this.Close();
        }

        private void BtnUndo_Click(object sender, RoutedEventArgs e)
        {
            CopyBookToView(this.DataContext as AddBookModel, book);
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            isCancelled = true;
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // update book if the change condition is sufficient

            AddBookModel context = this.DataContext as AddBookModel;
            Book tempBook = new Book();
            bool isChanged = false;
            bool sufficient = true;

            if (!isCancelled)
            {
                if (IsDataToAdd(context))
                {
                    int noAuthor = NumberOfAuthor(context);
                    int noSignature = NumberOfSignature(context);

                    if (noAuthor != 0 && noSignature != 0)
                    {
                        tempBook.BookId = book.BookId;
                        tempBook.Number = context.Number;
                        tempBook.NoAuthor = noAuthor;
                        tempBook.NoSignature = noSignature;
                        tempBook.Title = context.Title;
                        tempBook.Publisher = context.Publisher;
                        tempBook.Version = context.Version;
                        tempBook.Medium = context.Medium;
                        tempBook.Place = context.Place;
                        tempBook.Year = context.Year;
                        tempBook.DayBought = context.Date;
                        tempBook.Pages = context.Pages;
                        tempBook.Price = context.Price;

                        tempBook.Authors = new Author[tempBook.NoAuthor];
                        for (int i = 0; i < noAuthor; i++)
                        {
                            tempBook.Authors[i] = new Author();
                        }
                        tempBook.Signatures = new string[tempBook.NoSignature];
                        if (noAuthor > 0)
                        {
                            tempBook.Authors[0].Name = context.Author1;
                        }
                        if (noAuthor > 1)
                        {
                            tempBook.Authors[1].Name = context.Author2;
                        }
                        if (noAuthor > 2)
                        {
                            tempBook.Authors[2].Name = context.Author3;
                        }
                        if (noSignature > 0)
                        {
                            tempBook.Signatures[0] = context.Signature1;
                        }
                        if (noSignature > 1)
                        {
                            tempBook.Signatures[1] = context.Signature2;
                        }
                        if (noSignature > 2)
                        {
                            tempBook.Signatures[2] = context.Signature3;
                        }
                        isChanged = !Book.Compare(tempBook, book);
                    }
                    else
                    {
                        sufficient = false;
                    }

                    if (!isChanged && sufficient)
                    {
                        MessageBoxResult result;
                        if (copied)
                        {
                            copied = false;
                            result = MessageBox.Show("Nothing is changed. Book will not be added. Continue?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Information);
                        }
                        else
                        {
                            result = MessageBox.Show("Nothing is changed. Do you want to modify?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Information);
                        }
                        if (result == MessageBoxResult.Yes)
                        {
                            e.Cancel = true;
                        }
                    }
                    else if (isChanged && sufficient)
                    {
                        if (updateRequest)
                        {
                            updateRequest = false;

                            // update book

                            SqlMethods.SqlConnect(out SqliteConnection con);
                            var tr = con.BeginTransaction();
                            var updateCommand = con.CreateCommand();
                            updateCommand.Transaction = tr;
                            updateCommand.CommandText = $"UPDATE Books SET Number={tempBook.Number}, Title='{tempBook.Title}', Publisher='{tempBook.Publisher}', Version={tempBook.Version}, Year={tempBook.Year}, Medium='{tempBook.Medium}', Place='{tempBook.Place}', DayBought='{tempBook.DayBought}', Pages={tempBook.Pages}, Price={tempBook.Price} WHERE BookId={tempBook.BookId}";
                            updateCommand.ExecuteNonQuery();

                            //clear the binding from book and author and the signature

                            var deleteCommand = con.CreateCommand();
                            deleteCommand.Transaction = tr;
                            deleteCommand.CommandText = $"DELETE FROM Books_Authors WHERE BookId={tempBook.BookId}";
                            deleteCommand.ExecuteNonQuery();

                            //deleteCommand = con.CreateCommand();
                            deleteCommand.CommandText = $"DELETE FROM Books_Signatures WHERE BookId={tempBook.BookId}";
                            deleteCommand.ExecuteNonQuery();

                            var selectCommand = con.CreateCommand();
                            var insertCommand = con.CreateCommand();

                            for (int i = 0; i < noAuthor; i++)
                            {
                                // check if there is existing Author
                                selectCommand = con.CreateCommand();
                                selectCommand.CommandText = $"SELECT * FROM Authors WHERE Name = '{tempBook.Authors[i].Name}'";
                                SqliteDataReader r = selectCommand.ExecuteReader();
                                // if no, add Author
                                if (!r.Read())
                                {
                                    insertCommand.CommandText = "INSERT INTO Authors (Name) VALUES (@Name)";
                                    insertCommand.Parameters.AddWithValue("Name", tempBook.Authors[i].Name);
                                    insertCommand.ExecuteNonQuery();
                                    insertCommand.Parameters.Clear();
                                }

                                insertCommand.CommandText = $"INSERT INTO Books_Authors (BookId, AuthorId, Priority) VALUES ((SELECT BookId FROM Books " +
                                    $"WHERE Title = '{book.Title}' AND Version = '{tempBook.Version}' AND Medium = '{tempBook.Medium}')," +
                                    $"(SELECT AuthorId FROM Authors WHERE Name = '{tempBook.Authors[i].Name}'),{i + 1})";
                                insertCommand.ExecuteNonQuery();
                            }

                            for (int i = 0; i < noSignature; i++)
                            {
                                selectCommand = con.CreateCommand();
                                selectCommand.CommandText = $"SELECT * FROM Signatures WHERE Signature = '{tempBook.Signatures[i]}'";
                                SqliteDataReader r = selectCommand.ExecuteReader();
                                if (!r.Read())
                                {
                                    insertCommand.CommandText = "INSERT INTO Signatures (Signature) VALUES (@Signature)";
                                    insertCommand.Parameters.AddWithValue("Signature", tempBook.Signatures[i]);
                                    insertCommand.ExecuteNonQuery();
                                    insertCommand.Parameters.Clear();
                                }

                                insertCommand.CommandText = $"INSERT INTO Books_Signatures (BookId, SignatureId, Priority) VALUES ((SELECT BookId FROM Books " +
                                    $"WHERE Title = '{tempBook.Title}' AND Version = '{tempBook.Version}' AND Medium = '{tempBook.Medium}')," +
                                    $"(SELECT SignatureId FROM Signatures WHERE Signature = '{tempBook.Signatures[i]}'), {i + 1})";
                                insertCommand.ExecuteNonQuery();
                            }
                            tr.Commit();
                            con.Close();

                            MessageBox.Show("Book is changed successfully.", "Infomation", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else if (copied)
                        {
                            copied = false;
                            IsNew = true;
                            SqliteConnection con;
                            SqlMethods.SqlConnect(out con);
                            var tr = con.BeginTransaction();
                            AddBook.AddBookToDatabase(ref con, ref tr, tempBook);
                            tr.Commit();
                            con.Close();
                            MessageBox.Show("A copy is created successfully.", "Infomation", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            var result = MessageBox.Show("Content is changed. Do you want to discard?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
                            e.Cancel = result == MessageBoxResult.No;
                        }
                    }
                    else
                    {
                        var result = MessageBox.Show("Next author or next signature should be put correctly :) :)", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        e.Cancel = true;
                    }
                }
                else
                {
                    var result = MessageBox.Show("Not sufficient. Please provide all required fields.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    e.Cancel = true;
                }
            }
            else
            {
                var result = MessageBox.Show("Do you want to cancel the editing process?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
                isCancelled = false;
                e.Cancel = result == MessageBoxResult.No;
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

        private void BtnCopy_Click(object sender, RoutedEventArgs e)
        {
            copied = true;
            this.Close();
        }

        private void ReadPublisherMediumPlace()
        {
            string dataFolder = Path.Combine(AppContext.BaseDirectory, "Data");
            string publisherPath = Path.Combine(dataFolder, "Publishers.dat");
            string mediumPath = Path.Combine(dataFolder, "Mediums.dat");
            string placePath = Path.Combine(dataFolder, "Places.dat");

            using (StreamReader sr = new StreamReader(publisherPath))
            {
                while (!sr.EndOfStream)
                {
                    ListPublisher.Add(sr.ReadLine());
                }
            }

            using (StreamReader sr = new StreamReader(placePath))
            {
                while (!sr.EndOfStream)
                {
                    ListPlace.Add(sr.ReadLine());
                }
            }

            using (StreamReader sr = new StreamReader(mediumPath))
            {
                while (!sr.EndOfStream)
                {
                    ListMedium.Add(sr.ReadLine());
                }
            }

            
        }
    }
}
