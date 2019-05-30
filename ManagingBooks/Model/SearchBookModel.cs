using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

namespace ManagingBooks.Model
{
    class SearchBookModel : INotifyPropertyChanged
    {
        private string m_SearchText;
        private string m_SearchBy;
        private string m_Status;
        private int m_Progress;

        private string m_ViewNumber;
        private string m_ViewSignatures;
        private string m_ViewTitle;
        private string m_ViewAuthors;
        private string m_ViewPublisher;
        private string m_ViewYear;
        private string m_ViewVersion;
        private string m_ViewMedium;
        private string m_ViewPlace;
        private string m_ViewDate;
        private string m_ViewPages;
        private string m_ViewPrice;

        public string SearchText
        {
            get => m_SearchText;
            set
            {
                if (value != m_SearchText)
                {
                    m_SearchText = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string SearchBy
        {
            get => m_SearchBy;
            set
            {
                if (value != m_SearchBy)
                {
                    m_SearchBy = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string Status
        {
            get => m_Status;
            set
            {
                if (value != m_Status)
                {
                    m_Status = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public int Progress
        {
            get => m_Progress;
            set
            {
                if (value != m_Progress)
                {
                    m_Progress = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //public string ViewNumber
        //{
        //    get => m_ViewNumber;
        //    set
        //    {
        //        if (value != m_ViewNumber)
        //        {
        //            if (int.TryParse(value, out int n))
        //            {
        //                string temp = value;
        //                int length = temp.Length;
        //                for (int i = 6; i > length; i--)
        //                {
        //                    temp = String.Concat("0", temp);
        //                }
        //                m_ViewNumber = temp;
        //            }
        //            else
        //            {
        //                m_ViewNumber = value;
        //            }
        //            NotifyPropertyChanged();
        //        }
        //    }
        //}
        public string ViewNumber
        {
            get => m_ViewNumber;
            set
            {
                if (value != m_ViewNumber)
                {
                    m_ViewNumber = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string ViewSignatures
        {
            get => m_ViewSignatures;
            set
            {
                if (value != m_ViewSignatures)
                {
                    m_ViewSignatures = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string ViewTitle
        {
            get => m_ViewTitle;
            set
            {
                if (value != m_ViewTitle)
                {
                    m_ViewTitle = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string ViewAuthors
        {
            get => m_ViewAuthors;
            set
            {
                if (value != m_ViewAuthors)
                {
                    m_ViewAuthors = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string ViewPublisher
        {
            get => m_ViewPublisher;
            set
            {
                if (value != m_ViewPublisher)
                {
                    m_ViewPublisher = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string ViewYear
        {
            get => m_ViewYear;
            set
            {
                if (value != m_ViewYear)
                {
                    m_ViewYear = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string ViewVersion
        {
            get => m_ViewVersion;
            set
            {
                if (value != m_ViewVersion)
                {
                    m_ViewVersion = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string ViewMedium
        {
            get => m_ViewMedium;
            set
            {
                if (value != m_ViewMedium)
                {
                    m_ViewMedium = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string ViewPlace
        {
            get => m_ViewPlace;
            set
            {
                if (value != m_ViewPlace)
                {
                    m_ViewPlace = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string ViewDate
        {
            get => m_ViewDate;
            set
            {
                if (value != m_ViewDate)
                {
                    m_ViewDate = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string ViewPages
        {
            get => m_ViewPages;
            set
            {
                if (value != m_ViewPages)
                {
                    m_ViewPages = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string ViewPrice
        {
            get => m_ViewPrice;
            set
            {
                if (value != m_ViewPrice)
                {
                    if(double.TryParse(value, out double d))
                    {
                        m_ViewPrice = d.ToString("0.00");
                    }
                    else
                    {
                        m_ViewPrice = value;
                    }
                    NotifyPropertyChanged();
                }
            }
        }

        public ObservableCollection<SearchBook> DisplayBooks { get; set; }
        public ObservableCollection<SearchBook> ListBookPrint { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    class SearchBook
    {
        private string m_Nr;

        public int BookId { get; set; }
        public string Number
        {
            get => m_Nr;
            set
            {
                if (m_Nr != value)
                {
                    if (int.TryParse(value, out int n))
                    {
                        string temp = value;
                        int length = temp.Length;
                        for (int i = 6; i > length; i--)
                        {
                            temp = String.Concat("0", temp);
                        }
                        m_Nr = temp;
                    }
                    else
                    {
                        m_Nr = value;
                    } 
                }
            }
        }
        public string Signatures { get; set; }
        public string Title { get; set; }
        public string Publishers { get; set; }
        public string Authors { get; set; }
        public int Version { get; set; }
        public int Year { get; set; }
        public string Medium { get; set; }
        public string Place { get; set; }
        public string Date { get; set; }
        public int Pages { get; set; }
        public double Price { get; set; }
    }
}
