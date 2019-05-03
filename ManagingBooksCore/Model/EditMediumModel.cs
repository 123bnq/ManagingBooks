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
    class EditMediumModel : INotifyPropertyChanged
    {
        private int m_Id;
        private string m_Name;
        private string m_LabelName;

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

        public ObservableCollection<Medium> ListMedium { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    class Medium
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
