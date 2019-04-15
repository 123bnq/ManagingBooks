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
    class ChooseSignaturesModel : INotifyPropertyChanged
    {
        private string m_test;
        private Signature m_MainSig;
        private Signature m_SubSig;

        public string Test
        {
            get => m_test;
            set
            {
                if (value != m_test)
                {
                    m_test = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public ObservableCollection<Signature> Signatures { get; set; }
        public ObservableCollection<Signature> SubSignatures { get; set; }

        public Signature MainSig
        {
            get => m_MainSig;
            set
            {
                if (value != m_MainSig)
                {
                    m_MainSig = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public Signature SubSig
        {
            get => m_SubSig;
            set
            {
                if (value != m_SubSig)
                {
                    m_SubSig = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    class Signature
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Info { get; set; }
        public string Sort { get; set; }
    }
}
