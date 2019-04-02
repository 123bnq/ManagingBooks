using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ManagingBooks.Model
{
    class EditPublisherModel : INotifyPropertyChanged
    {
        private string m_Name;
        private string m_City;
        private string m_Country;

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
        public string Name { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
    }
}
