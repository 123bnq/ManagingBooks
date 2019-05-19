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
    class TransferBookModel : INotifyPropertyChanged
    {
        private string m_BookNumber;
        public string BookNumber
        {
            get => m_BookNumber;
            set
            {
                if (value != m_BookNumber)
                {
                    m_BookNumber = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ObservableCollection<TransferingBook> TransferList { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
    class TransferingBook
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
        public string Title { get; set; }
    }
}
