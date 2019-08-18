using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using iText.Layout.Element;

namespace ManagingBooks.Model
{
    public class SetBarcodePositionModel : INotifyPropertyChanged
    {
        private int m_RowNumber;
        private int m_ColNumber;

        public SetBarcodePositionModel()
        {
        }

        public SetBarcodePositionModel(List<int> rows, List<int> cols)
        {
            Rows = new ObservableCollection<int>(rows);
            Cols = new ObservableCollection<int>(cols);
        }

        public int RowNumber
        {
            get => m_RowNumber;
            set
            {
                if (value != m_RowNumber)
                {
                    m_RowNumber = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public int ColNumber
        {
            get => m_ColNumber;
            set
            {
                if (value != m_ColNumber)
                {
                    m_ColNumber = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public ObservableCollection<int> Rows { get; set; }
        public ObservableCollection<int> Cols { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class BarcodeWithPosiition
    {
        public int RowNumber { get; set; }
        public int ColNumber { get; set; }
        public string BarcodeNumber { get; set; }
        public string Signatures { get; set; }
        public Image BarcodeImage { get; set; }
        public Paragraph Text { get; set; }
    }
}
