using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ManagingBooks.Model
{
    class AddBookModel : INotifyPropertyChanged
    {
        private int m_Number;
        private string m_Signature1;
        private string m_Signature2;
        private string m_Signature3;
        private string m_Author1;
        private string m_Author2;
        private string m_Author3;
        private string m_Title;
        private string m_Publisher;
        private int m_Version;
        private int m_Year;
        private string m_Medium;
        private string m_Place;
        private string m_Date;
        private int m_Pages;
        private double m_Price;

        public int Number {
            get => m_Number;
            set
            {
                if (value != m_Number)
                {
                    m_Number = value;
                    NotifyPropertyChanged();
                }   
            }
        }
        public string Signature1 {
            get => m_Signature1;
            set
            {
                if(value != m_Signature1)
                {
                    m_Signature1 = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string Signature2
        {
            get => m_Signature2;
            set
            {
                if (value != m_Signature2)
                {
                    m_Signature2 = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string Signature3
        {
            get => m_Signature3;
            set
            {
                if (value != m_Signature3)
                {
                    m_Signature3 = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string Author1
        {
            get => m_Author1;
            set
            {
                if (value != m_Author1)
                {
                    m_Author1 = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string Author2
        {
            get => m_Author2;
            set
            {
                if (value != m_Author2)
                {
                    m_Author2 = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string Author3
        {
            get => m_Author3;
            set
            {
                if (value != m_Author3)
                {
                    m_Author3 = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string Title
        {
            get => m_Title;
            set
            {
                if (value != m_Title)
                {
                    m_Title = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string Publisher
        {
            get => m_Publisher;
            set
            {
                if (value != m_Publisher)
                {
                    m_Publisher = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public int Version
        {
            get => m_Version;
            set
            {
                if (value != m_Version)
                {
                    m_Version = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public int Year
        {
            get => m_Year;
            set
            {
                if (value != m_Year)
                {
                    m_Year = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string Medium
        {
            get => m_Medium;
            set
            {
                if (value != m_Medium)
                {
                    m_Medium = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string Place
        {
            get => m_Place;
            set
            {
                if (value != m_Place)
                {
                    m_Place = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string Date
        {
            get => m_Date;
            set
            {
                if (value != m_Date)
                {
                    m_Date = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public double Price
        {
            get => m_Price;
            set
            {
                if (value != m_Price)
                {
                    m_Price = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public int Pages
        {
            get => m_Pages;
            set
            {
                if (value != m_Pages)
                {
                    m_Pages = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ObservableCollection<int> ListBook { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
