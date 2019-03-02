namespace ManagingBooks
{
    class Book
    {
        public int BookId { get; set; }
        public int Number { get; set; }
        public string[] Signatures { get; set; }
        public string Title { get; set; }
        public Author[] Authors { get; set; }
        public string Publisher { get; set; }
        public int Version { get; set; }
        public int Year { get; set; }
        public string Medium { get; set; }
        public string DayBought { get; set; }
        public string Place { get; set; }
        public int Pages { get; set; }
        public double Price { get; set; }

        public int NoSignature { get; set; }

        public int NoAuthor { get; set; }

        public Book()
        {
            BookId = -1;
        }

        public Book(int noAuthor, int noSignature, int number, string title, string publisher, int version, int year, string medium, string date, string place, int pages, double price)
        {
            BookId = -1;
            NoAuthor = noAuthor;
            NoSignature = noSignature;
            Number = number;
            Title = title;
            Publisher = publisher;
            Version = version;
            Year = year;
            Medium = medium;
            DayBought = date;
            Place = place;
            Pages = pages;
            Price = price;
        }

        public static bool Compare(Book b1, Book b2)
        {
            if(b1.Authors.Length != b2.Authors.Length || b1.Signatures.Length != b2.Signatures.Length)
            {
                return false;
            }

            for (int i = 0; i < b1.Authors.Length; i++)
            {
                if (!b1.Authors[i].Name.Equals(b2.Authors[i].Name))
                {
                    return false;
                }
            }

            for (int i = 0; i < b1.Signatures.Length; i++)
            {
                if (!b1.Signatures[i].Equals(b2.Signatures[i]))
                {
                    return false;
                }
            }

            return (b1.Number == b2.Number) && 
                b1.Title.Equals(b2.Title) &&
                b1.Publisher.Equals(b2.Publisher) &&
                (b1.Version == b2.Version) &&
                (b1.Year == b2.Year) &&
                b1.Medium.Equals(b2.Medium) &&
                b1.Place.Equals(b2.Place) &&
                b1.DayBought.Equals(b2.DayBought) &&
                (b1.Pages == b2.Pages) &&
                (b1.Price == b2.Price);
        }
    }
}
