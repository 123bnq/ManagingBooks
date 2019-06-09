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
        private Signature m_MainSig;
        private Signature m_SubSig;
        private int m_CurrentId;
        private string m_Name;
        private string m_Info;

        private string m_LabelName;
        private string m_LabelInfo;
        private string m_LabelParent;
        private string m_LabelSort;
        private bool m_IsSubSig = true;

        private string m_SubName;
        private string m_SubInfo;
        private int m_ParentId;
        private string m_Sort;

        public int CurrentId
        {
            get => m_CurrentId;
            set
            {
                if (value != m_CurrentId)
                {
                    m_CurrentId = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public ObservableCollection<Signature> Signatures { get; set; }
        public ObservableCollection<Signature> SubSignatures { get; set; }
        public ObservableCollection<Signature> ParentList { get; set; }

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
        public string Info
        {
            get => m_Info;
            set
            {
                if (value != m_Info)
                {
                    m_Info = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string SubName
        {
            get => m_SubName;
            set
            {
                if (value != m_SubName)
                {
                    m_SubName = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string SubInfo
        {
            get => m_SubInfo;
            set
            {
                if (value != m_SubInfo)
                {
                    m_SubInfo = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public int ParentId
        {
            get => m_ParentId;
            set
            {
                if (value != m_ParentId)
                {
                    m_ParentId = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string Sort
        {
            get => m_Sort;
            set
            {
                if (value != m_Sort)
                {
                    m_Sort = value;
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
        public string LabelInfo
        {
            get => m_LabelInfo;
            set
            {
                if (value != m_LabelInfo)
                {
                    m_LabelInfo = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string LabelParent
        {
            get => m_LabelParent;
            set
            {
                if (value != m_LabelParent)
                {
                    m_LabelParent = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string LabelSort
        {
            get => m_LabelSort;
            set
            {
                if (value != m_LabelSort)
                {
                    m_LabelSort = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsSubSig
        {
            get => m_IsSubSig;
            set
            {
                if (value != m_IsSubSig)
                {
                    m_IsSubSig = value;
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

        public override string ToString()
        {
            string result = String.Format($"{Name} ({Info})");
            return result;
        }
    }
}
