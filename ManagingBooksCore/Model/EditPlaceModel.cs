using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ManagingBooks.Model
{
    class EditPlaceModel : INotifyPropertyChanged
    {
        private int m_Id;
        private string m_City;
        private string m_State;
        private string m_Country;
        private string m_LabelCity;
        private string m_LabelState;
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
        public string City
        {
            get => m_City;
            set
            {
                if (value != m_City)
                {
                    m_City = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string State
        {
            get => m_State;
            set
            {
                if (value != m_State)
                {
                    m_State = value;
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
        public string LabelState
        {
            get => m_LabelState;
            set
            {
                if (value != m_LabelState)
                {
                    m_LabelState = value;
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

        public ObservableCollection<Place> ListPlace { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    class Place
    {
        public int Id { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
    }
}
