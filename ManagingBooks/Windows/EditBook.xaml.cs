using ManagingBooks.Model;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Linq;

namespace ManagingBooks.Windows
{
    /// <summary>
    /// Interaction logic for EditBook.xaml
    /// </summary>
    public partial class EditBook : Window
    {
        private Book book;

        public EditBook(int bookId)
        {
            InitializeComponent();
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
            selectCommmand.CommandText = $"SELECT b.Number,b.Title,b.Publisher,b.Version,b.Year,b.Medium,b.Place,b.DayBought,b.Pages,b.Price,a.AuthorId,a.Name,s.Signature FROM Books b LEFT JOIN Books_Authors ba ON (ba.BookId = b.BookId) LEFT JOIN Authors a ON (a.AuthorId = ba.AuthorId) LEFT JOIN Signatures s ON (s.BookId = b.BookId) WHERE b.BookId = '{book.BookId}' ORDER BY ba.Id";
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
            book.Authors = new Author[authors.Count];

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

            book.Signatures = new string[signatures.Count];

            foreach (var s in signatures)
            {
                book.Signatures[i++] = s;
            }
            CopyBookToView(this.DataContext as AddBookModel);
            con.Close();
        }

        private void CopyBookToView(AddBookModel context)
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

        private void BtnUndo_Click(object sender, RoutedEventArgs e)
        {
            CopyBookToView(this.DataContext as AddBookModel);
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // update book if the change condition is sufficient

            AddBookModel context = this.DataContext as AddBookModel;
            Book tempBook;
            bool isChanged = false;

            if (IsDataToAdd(context))
            {
                int noAuthor = NumberOfAuthor(context);
                int noSignature = NumberOfSignature(context);

                if (noAuthor != 0 && noSignature != 0)
                {
                    tempBook = new Book();
                    tempBook.BookId = book.BookId;
                    tempBook.Number = context.Number;
                    tempBook.Title = context.Title;
                    tempBook.Publisher = context.Publisher;
                    tempBook.Version = context.Version;
                    tempBook.Medium = context.Medium;
                    tempBook.Place = context.Place;
                    tempBook.Year = context.Year;
                    tempBook.DayBought = context.Date;
                    tempBook.Pages = context.Pages;
                    tempBook.Price = context.Price;

                    tempBook.Authors = new Author[noAuthor];
                    for (int i = 0; i < noAuthor; i++)
                    {
                        tempBook.Authors[i] = new Author();
                    }
                    tempBook.Signatures = new string[noSignature];
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

                if (!isChanged)
                {
                    var result = MessageBox.Show("Nothing is changed. Do you want to modify?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Information);
                    if(result == MessageBoxResult.Yes)
                    {
                        e.Cancel = true;
                    }
                }
                else
                {
                    var result = MessageBox.Show("Content is changed. Do you want to discard?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.No)
                    {
                        e.Cancel = true;
                    }
                }


            }


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
    }
}
