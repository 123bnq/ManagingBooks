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
using WPFCustomMessageBox;

namespace ManagingBooks.Windows
{
    /// <summary>
    /// Interaction logic for ChooseSignatures.xaml
    /// </summary>
    public partial class ChooseSignatures : Window
    {
        private AddBookModel preContext;
        private bool IsEdit = false;
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
            EditSig.Visibility = Visibility.Collapsed;
            //ClearEntries(context);
        }

        public ChooseSignatures(bool isEdit)
        {
            ChooseSignaturesModel context = new ChooseSignaturesModel();
            context.Signatures = new ObservableCollection<Signature>();
            context.SubSignatures = new ObservableCollection<Signature>();
            this.DataContext = context;
            this.IsEdit = isEdit;
            ReadSignature();
            ClearEntries(context);
            context.ParentList = new ObservableCollection<Signature>();
            ReadParentList();
            InitializeComponent();
            EditSig.Visibility = Visibility.Visible;

        }

        private void ClearEntries(ChooseSignaturesModel context)
        {
            context = this.DataContext as ChooseSignaturesModel;
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
            ResetLabel(context);
            CommandManager.InvalidateRequerySuggested();
        }

        private void ReadSignature()
        {
            ChooseSignaturesModel context = this.DataContext as ChooseSignaturesModel;
            context.Signatures.Clear();
            SqlMethods.SqlConnect(out SqliteConnection con);
            SqliteCommand selectCommand = con.CreateCommand();
            selectCommand.CommandText = "SELECT SignatureId, Signature, Info FROM Signatures WHERE ParentId IS NULL";
            SqliteDataReader r = selectCommand.ExecuteReader();
            while (r.Read())
            {
                Signature sig = new Signature();
                int resultNum;
                int.TryParse(Convert.ToString(r["SignatureId"]), out resultNum);
                sig.Id = resultNum;
                sig.Name = Convert.ToString(r["Signature"]);
                sig.Info = Convert.ToString(r["Info"]);
                if (!sig.Name.Equals(sig.Info))
                {
                    context.Signatures.Add(sig);
                }
            }
            con.Close();
        }

        private void ReadSubSignature(int SignatureId)
        {
            ChooseSignaturesModel context = this.DataContext as ChooseSignaturesModel;
            context.SubSignatures.Clear();
            SqlMethods.SqlConnect(out SqliteConnection con);
            SqliteCommand selectCommand = con.CreateCommand();
            selectCommand.CommandText = $"SELECT SignatureId, Signature, Info, Sort FROM Signatures Where ParentId={SignatureId} ORDER BY Sort,Signature";
            SqliteDataReader r = selectCommand.ExecuteReader();
            while (r.Read())
            {
                Signature sig = new Signature();
                int resultNum;
                int.TryParse(Convert.ToString(r["SignatureId"]), out resultNum);
                sig.Id = resultNum;
                sig.Name = Convert.ToString(r["Signature"]);
                sig.Info = Convert.ToString(r["Info"]);
                sig.Sort = Convert.ToString(r["Sort"]);
                context.SubSignatures.Add(sig);
            }
            con.Close();
        }

        private void ReadParentList()
        {
            ChooseSignaturesModel context = this.DataContext as ChooseSignaturesModel;
            if (context.Signatures != null)
            {
                context.ParentList.Clear();
                foreach (var sig in context.Signatures)
                {
                    context.ParentList.Add(sig);
                }
            }
        }

        private void MainList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ChooseSignaturesModel context = this.DataContext as ChooseSignaturesModel;
            if (context.MainSig != null)
            {
                ReadSubSignature(context.MainSig.Id);
                if (!IsEdit)
                {
                    preContext.Signature1 = context.MainSig.Name;
                    preContext.Signature2 = string.Empty;
                }
                else
                {
                    ClearEntries(context);
                    EditLabel(context);
                    context.IsSubSig = false;
                    context.CurrentId = context.MainSig.Id;
                    context.Name = context.MainSig.Name;
                    context.Info = context.MainSig.Info;
                }
            }
        }

        private void SubList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ChooseSignaturesModel context = this.DataContext as ChooseSignaturesModel;
            if (context.SubSig != null)
            {
                if (!IsEdit)
                {
                    preContext.Signature2 = context.SubSig.Name;
                }
                else
                {
                    ClearEntries(context);
                    EditLabel(context);
                    context.IsSubSig = false;
                    context.CurrentId = context.SubSig.Id;
                    context.Name = context.SubSig.Name;
                    context.Info = context.SubSig.Info;
                    context.ParentId = context.MainSig.Id;
                    context.Sort = context.SubSig.Sort;
                }
            }
        }

        private void EditLabel(ChooseSignaturesModel context)
        {
            context.LabelName = Application.Current.FindResource("ChooseSignature.Label.EditSignature").ToString();
            context.LabelInfo = Application.Current.FindResource("ChooseSignature.Label.EditInfo").ToString();
            context.LabelParent = Application.Current.FindResource("ChooseSignature.Label.EditParent").ToString();
            context.LabelSort = Application.Current.FindResource("ChooseSignature.Label.EditSort").ToString();
        }
        private void ResetLabel(ChooseSignaturesModel context)
        {
            context.LabelName = Application.Current.FindResource("ChooseSignature.Label.Signature").ToString();
            context.LabelInfo = Application.Current.FindResource("ChooseSignature.Label.Info").ToString();
            context.LabelParent = Application.Current.FindResource("ChooseSignature.Label.Parent").ToString();
            context.LabelSort = Application.Current.FindResource("ChooseSignature.Label.Sort").ToString();
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ChooseSignaturesModel context = this.DataContext as ChooseSignaturesModel;
            context.MainSig = null;
            context.SubSig = null;
            context.IsSubSig = true;
            context.SubSignatures.Clear();
            ClearEntries(context);
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ChooseSignaturesModel context = this.DataContext as ChooseSignaturesModel;
            if (!string.IsNullOrWhiteSpace(context.Name) || !string.IsNullOrWhiteSpace(context.Info) || context.ParentId != 0 || !string.IsNullOrWhiteSpace(context.Sort))
            {
                string message = Application.Current.FindResource("ChooseSignature.CodeBehind.WarningClose.Message").ToString();
                string caption = Application.Current.FindResource("ChooseSignature.CodeBehind.WarningClose.Caption").ToString();
                var result = CustomMessageBox.ShowYesNo(message, caption, CustomMessageBoxButton.Yes, CustomMessageBoxButton.No, MessageBoxImage.Warning);
                e.Cancel = result == MessageBoxResult.No;
            }
        }

        private void RemoveSigCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            ChooseSignaturesModel context = this.DataContext as ChooseSignaturesModel;
            e.CanExecute = context.MainSig != null || context.SubSig != null;
        }

        private async void RemoveSigCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ChooseSignaturesModel context = this.DataContext as ChooseSignaturesModel;
            SqlMethods.SqlConnect(out SqliteConnection con);

            SqliteCommand removeCommand = con.CreateCommand();


            // remove main Signature
            if (context.ParentId == 0)
            {
                SqliteCommand selectCommand = con.CreateCommand();
                selectCommand.CommandText = $"SELECT * FROM Signatures WHERE ParentId={context.CurrentId}";
                SqliteDataReader r = selectCommand.ExecuteReader();
                if (!r.Read())
                {
                    removeCommand.CommandText = $"DELETE FROM Signatures WHERE SignatureId={context.CurrentId}";
                    removeCommand.ExecuteNonQuery();
                    ReadSignature();
                    ReadParentList();
                }
                else
                {
                    string message = Application.Current.FindResource("ChooseSignature.CodeBehind.ErrorRemove.Message").ToString();
                    string caption = Application.Current.FindResource("ChooseSignature.CodeBehind.ErrorRemove.Caption").ToString();
                    MessageBoxResult result = CustomMessageBox.ShowOK(message, caption, CustomMessageBoxButton.OK, MessageBoxImage.Error);

                }
            }
            // remove sub-Signature
            else
            {
                var removeItems = SubList.SelectedItems;
                SqliteTransaction tr = con.BeginTransaction();
                removeCommand.Transaction = tr;
                await Task.Run(() =>
                {
                    foreach (var item in removeItems)
                    {
                        Signature temp = item as Signature;
                        removeCommand.CommandText = $"DELETE FROM Signatures WHERE SignatureId={temp.Id}";
                        removeCommand.ExecuteNonQueryAsync();
                    }
                    tr.Commit();
                });
                ReadSubSignature(context.MainSig.Id);
            }
            con.Close();
            context.MainSig = null;
            context.SubSig = null;
            context.IsSubSig = true;
            context.SubSignatures.Clear();
            ClearEntries(context);
        }

        private void SaveSigCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            ChooseSignaturesModel context = this.DataContext as ChooseSignaturesModel;
            e.CanExecute = !string.IsNullOrWhiteSpace(context.Name) && !string.IsNullOrWhiteSpace(context.Info);
        }

        private void SaveSigCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ChooseSignaturesModel context = this.DataContext as ChooseSignaturesModel;
            SqlMethods.SqlConnect(out SqliteConnection con);
            SqliteCommand selectCommand = con.CreateCommand();
            if (context.ParentId == 0)
            {
                selectCommand.CommandText = $"SELECT SignatureId, Signature, Info FROM Signatures WHERE ParentId IS NULL AND Signature = '{context.Name}'";
            }
            else
            {
                selectCommand.CommandText = $"SELECT SignatureId, Signature, Info FROM Signatures WHERE ParentId = {context.ParentId} AND Signature = '{context.Name}'";
            }
            SqliteDataReader r = selectCommand.ExecuteReader();
            if (!r.Read())
            {
                // to add new place
                if (context.CurrentId == 0)
                {
                    SqliteCommand insertCommand = con.CreateCommand();
                    if (context.ParentId == 0)
                    {
                        insertCommand.CommandText = $"INSERT INTO Signatures (Signature,Info) VALUES ('{context.Name}','{context.Info}')";
                    }
                    else
                    {
                        insertCommand.CommandText = $"INSERT INTO Signatures (Signature,Info,ParentId,Sort) VALUES ('{context.Name}','{context.Info}','{context.ParentId}','{context.Sort}')";
                    }
                    insertCommand.ExecuteNonQuery();
                }

                // to update place's State and country
                else
                {
                    SqliteCommand updateCommand = con.CreateCommand();
                    if (context.ParentId == 0)
                    {
                        updateCommand.CommandText = $"UPDATE Signatures SET Signature='{context.Name}', Info='{context.Info}' WHERE SignatureId={context.CurrentId}";
                    }
                    else
                    {
                        updateCommand.CommandText = $"UPDATE Signatures SET Signature='{context.Name}', Info='{context.Info}',Sort='{context.Sort}' WHERE SignatureId={context.CurrentId}";
                    }
                    updateCommand.ExecuteNonQuery();
                }
            }
            else
            {
                // to update place's city
                if (context.CurrentId != 0)
                {
                    SqliteCommand updateCommand = con.CreateCommand();
                    if (context.ParentId == 0)
                    {
                        updateCommand.CommandText = $"UPDATE Signatures SET Signature='{context.Name}', Info='{context.Info}' WHERE SignatureId={context.CurrentId}";
                    }
                    else
                    {
                        updateCommand.CommandText = $"UPDATE Signatures SET Signature='{context.Name}', Info='{context.Info}',Sort='{context.Sort}' WHERE SignatureId={context.CurrentId}";
                    }
                    updateCommand.ExecuteNonQuery();

                }
                else
                {
                    string message = Application.Current.FindResource("ChooseSignature.CodeBehind.ErrorSave.Message").ToString();
                    string caption = Application.Current.FindResource("ChooseSignature.CodeBehind.ErrorSave.Caption").ToString();
                    MessageBoxResult result = CustomMessageBox.ShowOK(message, caption, CustomMessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            con.Close();
            context.SubSig = null;
            context.SubSignatures.Clear();
            ClearEntries(context);
            ReadSignature();
            ReadParentList();
        }
    }
}
