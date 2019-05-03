using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ManagingBooks.Model
{
    class EditPublisherModel : INotifyPropertyChanged
    {
        private int m_Id;
        private string m_Name;
        private string m_City;
        private string m_Country;
        private string m_LabelName;
        private string m_LabelCity;
        private string m_LabelCountry;

        public int Id
        {
            get => m_Id;
            set
            {
                if (value != m_Id)
                {
                    m_Id = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string Name
        {
            get => m_Name;
            set
            {
                if (value != m_Name)
                {
                    m_Name = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string City
        {
            get => m_City;
            set
            {
                if(value != m_City)
                {
                    m_City = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string Country
        {
            get => m_Country;
            set
            {
                if (value != m_Country)
                {
                    m_Country = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string LabelName
        {
            get => m_LabelName;
            set
            {
                if (value != m_LabelName)
                {
                    m_LabelName = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string LabelCity
        {
            get => m_LabelCity;
            set
            {
                if (value != m_LabelCity)
                {
                    m_LabelCity = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string LabelCountry
        {
            get => m_LabelCountry;
            set
            {
                if (value != m_LabelCountry)
                {
                    m_LabelCountry = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public ObservableCollection<Publisher> ListPublisher { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    class Publisher
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
    }
}
