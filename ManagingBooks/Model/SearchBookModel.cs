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

        public string SearchText {
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

        public ObservableCollection<SearchBook> DisplayBooks { get; set; }

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
        public int BookId { get; set; }
        public int Number { get; set; }
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
