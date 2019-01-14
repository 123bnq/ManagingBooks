namespace ManagingBooks
{
    class Book
    {
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

        public int NoSignature { get; }

        public int NoAuthor { get; }

        public Book()
        {
        }

        public Book(int noAuthor, int noSignature, int number, string title, string publisher, int version, int year, string medium, string date, string place, int pages, double price)
        {
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
    }
}
