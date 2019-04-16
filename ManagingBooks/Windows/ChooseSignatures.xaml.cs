using ManagingBooks.Model;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ManagingBooks.Windows
{
    /// <summary>
    /// Interaction logic for ChooseSignatures.xaml
    /// </summary>
    public partial class ChooseSignatures : Window
    {
        private AddBookModel preContext;
        public ChooseSignatures()
        {
            InitializeComponent();
        }

        public ChooseSignatures(AddBookModel preContext)
        {
            this.preContext = preContext;
            ChooseSignaturesModel context = new ChooseSignaturesModel();
            context.Signatures = new ObservableCollection<Signature>();
            context.SubSignatures = new ObservableCollection<Signature>();
            this.DataContext = context;
            ReadSignature();
            InitializeComponent();
            //ClearEntries(context);

        }

        private void ClearEntries(ChooseSignaturesModel context)
        {
            PropertyInfo[] properties = context.GetType().GetProperties();

            foreach (var propertyInfo in properties)
            {
                if (propertyInfo.PropertyType == typeof(string))
                {
                    propertyInfo.SetValue(context, string.Empty, null);
                }
                if (propertyInfo.PropertyType == typeof(int))
                {
                    propertyInfo.SetValue(context, 0, null);
                }
            }
        }

        private void ReadSignature()
        {
            ChooseSignaturesModel context = this.DataContext as ChooseSignaturesModel;
            SqlMethods.SqlConnect(out SqliteConnection con);
            SqliteCommand selectCommand = con.CreateCommand();
            selectCommand.CommandText = "SELECT SignatureId, Signature, Info FROM Signatures";
            SqliteDataReader r = selectCommand.ExecuteReader();
            while (r.Read())
            {
                Signature sig = new Signature();
                int resultNum;
                int.TryParse(Convert.ToString(r["SignatureId"]), out resultNum);
                sig.Id = resultNum;
                sig.Name = Convert.ToString(r["Signature"]);
                sig.Info = Convert.ToString(r["Info"]);
                context.Signatures.Add(sig);
            }
            con.Close();
        }

        private void ReadSubSignature(int SignatureId)
        {
            ChooseSignaturesModel context = this.DataContext as ChooseSignaturesModel;
            context.SubSignatures.Clear();
            SqlMethods.SqlConnect(out SqliteConnection con);
            SqliteCommand selectCommand = con.CreateCommand();
            selectCommand.CommandText = $"SELECT Id, Signature, Info, Sort FROM SubSignatures Where ParentId={SignatureId} ORDER BY Sort,Signature";
            SqliteDataReader r = selectCommand.ExecuteReader();
            while (r.Read())
            {
                Signature sig = new Signature();
                int resultNum;
                int.TryParse(Convert.ToString(r["Id"]), out resultNum);
                sig.Id = resultNum;
                sig.Name = Convert.ToString(r["Signature"]);
                sig.Info = Convert.ToString(r["Info"]);
                sig.Sort = Convert.ToString(r["Sort"]);
                context.SubSignatures.Add(sig);
            }
            con.Close();
        }

        private void MainList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        { 
            ChooseSignaturesModel context = this.DataContext as ChooseSignaturesModel;
            if (context.MainSig != null)
            {
                preContext.Signature1 = context.MainSig.Name;
                ReadSubSignature(context.MainSig.Id);
                preContext.Signature2 = string.Empty;
            }
        }

        private void SubList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ChooseSignaturesModel context = this.DataContext as ChooseSignaturesModel;
            if (context.SubSig != null)
            {
                preContext.Signature2 = context.SubSig.Name;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ChooseSignaturesModel context = this.DataContext as ChooseSignaturesModel;
            if (context.MainSig != null)
            {
                preContext.Signature1 = context.MainSig.Name;
                if (context.SubSig != null)
                {
                    preContext.Signature2 = context.SubSig.Name;
                }
            }
        }
    }
}
