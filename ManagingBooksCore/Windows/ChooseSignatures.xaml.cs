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
using System.Data.SQLite;
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
        private Signature MainSig = new Signature();
        private Signature SubSig = new Signature();

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
            context.Title = Application.Current.FindResource("ChooseSignature.Title").ToString();
            InitializeComponent();
            EditSig.Visibility = Visibility.Collapsed;
            EditSubSig.Visibility = Visibility.Collapsed;
            context.IsEdit = IsEdit;
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
            context.IsEdit = isEdit;
            EditSig.Visibility = Visibility.Visible;
            EditSubSig.Visibility = Visibility.Visible;
            context.Title = Application.Current.FindResource("ChooseSignature.EditTitle").ToString();
        }

        private void ClearEntries(ChooseSignaturesModel context, string name = null, string info = null, string parrentId = null, bool main = true)
        {
            context = this.DataContext as ChooseSignaturesModel;
            PropertyInfo[] properties = context.GetType().GetProperties();

            foreach (var propertyInfo in properties)
            {
                if (propertyInfo.PropertyType == typeof(string) && !propertyInfo.Name.Equals("Sort") && !propertyInfo.Name.Equals(name) && !propertyInfo.Name.Equals(info)
                    && !propertyInfo.Name.Equals("LabelName") && !propertyInfo.Name.Equals("LabelInfo") && !propertyInfo.Name.Equals("LabelSubName") && !propertyInfo.Name.Equals("LabelSubInfo"))
                {
                    propertyInfo.SetValue(context, string.Empty, null);
                }
                if (propertyInfo.PropertyType == typeof(int) && !propertyInfo.Name.Equals(parrentId))
                {
                    propertyInfo.SetValue(context, 0, null);
                }
            }
            ResetLabel(context, main: main);
            //CommandManager.InvalidateRequerySuggested();
        }

        private void ReadSignature()
        {
            ChooseSignaturesModel context = this.DataContext as ChooseSignaturesModel;
            context.Signatures.Clear();
            SqlMethods.SqlConnect(out SQLiteConnection con);
            SQLiteCommand selectCommand = con.CreateCommand();
            selectCommand.CommandText = "SELECT SignatureId, Signature, Info FROM Signatures WHERE ParentId IS NULL ORDER BY Signature";
            SQLiteDataReader r = selectCommand.ExecuteReader();
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
            r.Close();
            con.Close();
        }

        private void ReadSubSignature(int SignatureId)
        {
            ChooseSignaturesModel context = this.DataContext as ChooseSignaturesModel;
            context.SubSignatures.Clear();
            SqlMethods.SqlConnect(out SQLiteConnection con);
            SQLiteCommand selectCommand = con.CreateCommand();
            selectCommand.CommandText = $"SELECT SignatureId, Signature, Info, Sort FROM Signatures Where ParentId=@SignatureId ORDER BY Sort,Signature";
            selectCommand.Parameters.AddWithValue("SignatureId", SignatureId);
            SQLiteDataReader r = selectCommand.ExecuteReader();
            selectCommand.Parameters.Clear();
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
            r.Close();
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
                    ClearEntries(context, "Name", "Info");
                    context.IsSubSig = false;
                    context.CurrentId = context.MainSig.Id;
                    context.ParentId = context.MainSig.Id;
                    context.Name = context.MainSig.Name;
                    context.Info = context.MainSig.Info;
                    // change to edit label for main signature
                    EditLabel(context, sub: false);
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
                    //ClearEntries(context, "Name", "Info", "ParrentId");
                    context.IsSubSig = true;
                    context.CurrentSubId = context.SubSig.Id;
                    context.SubName = context.SubSig.Name;
                    context.SubInfo = context.SubSig.Info;
                    context.ParentId = context.MainSig.Id;
                    context.Sort = context.SubSig.Sort;
                    // change to edit label for sub signature
                    EditLabel(context, sub: true);
                }
            }
        }

        private void EditLabel(ChooseSignaturesModel context, bool sub = false)
        {
            context.LabelName = Application.Current.FindResource("ChooseSignature.Label.EditSignature").ToString();
            context.LabelInfo = Application.Current.FindResource("ChooseSignature.Label.EditInfo").ToString();
            if (sub)
            {
                context.LabelSubName = Application.Current.FindResource("ChooseSignature.Label.EditSignature").ToString();
                context.LabelSubInfo = Application.Current.FindResource("ChooseSignature.Label.EditInfo").ToString();
            }
            //context.LabelParent = Application.Current.FindResource("ChooseSignature.Label.EditParent").ToString();
            //context.LabelSort = Application.Current.FindResource("ChooseSignature.Label.EditSort").ToString();
        }
        private void ResetLabel(ChooseSignaturesModel context, bool main = false)
        {
            if (main)
            {
                context.LabelName = Application.Current.FindResource("ChooseSignature.Label.Signature").ToString();
                context.LabelInfo = Application.Current.FindResource("ChooseSignature.Label.Info").ToString();
            }
            context.LabelSubName = Application.Current.FindResource("ChooseSignature.Label.Signature").ToString();
            context.LabelSubInfo = Application.Current.FindResource("ChooseSignature.Label.Info").ToString();
            //context.LabelParent = Application.Current.FindResource("ChooseSignature.Label.Parent").ToString();
            //context.LabelSort = Application.Current.FindResource("ChooseSignature.Label.Sort").ToString();
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ChooseSignaturesModel context = this.DataContext as ChooseSignaturesModel;
            context.MainSig = null;
            //context.SubSig = null;
            context.SubSignatures.Clear();
            context.ParentId = 0;
            ClearEntries(context, "SubName", "SubInfo");
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ChooseSignaturesModel context = this.DataContext as ChooseSignaturesModel;
            if (IsEdit)
            {
                if (!string.IsNullOrWhiteSpace(context.Name) || !string.IsNullOrWhiteSpace(context.Info) || !string.IsNullOrWhiteSpace(context.SubName) || !string.IsNullOrWhiteSpace(context.SubInfo))
                {
                    string message = Application.Current.FindResource("ChooseSignature.CodeBehind.WarningClose.Message").ToString();
                    string caption = Application.Current.FindResource("ChooseSignature.CodeBehind.WarningClose.Caption").ToString();
                    var result = CustomMessageBox.ShowYesNo(message, caption, CustomMessageBoxButton.Yes, CustomMessageBoxButton.No, MessageBoxImage.Warning);
                    e.Cancel = result == MessageBoxResult.No;
                }
            }
        }

        private void RemoveMainSigCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            ChooseSignaturesModel context = this.DataContext as ChooseSignaturesModel;
            e.CanExecute = context.MainSig != null || context.SubSig != null;
        }

        private void RemoveMainSigCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ChooseSignaturesModel context = this.DataContext as ChooseSignaturesModel;
            SqlMethods.SqlConnect(out SQLiteConnection con);
            SQLiteCommand removeCommand = con.CreateCommand();

            // remove main Signature
            SQLiteCommand selectCommand = con.CreateCommand();
            selectCommand.CommandText = $"SELECT * FROM Signatures WHERE ParentId=@ParentId";
            selectCommand.Parameters.AddWithValue("ParentId", context.CurrentId);
            SQLiteDataReader r = selectCommand.ExecuteReader();
            selectCommand.Parameters.Clear();
            if (!r.Read())
            {
                removeCommand.CommandText = $"DELETE FROM Signatures WHERE SignatureId=@SignatureId";
                removeCommand.Parameters.AddWithValue("SignatureId", context.CurrentId);
                removeCommand.ExecuteNonQuery();
                removeCommand.Parameters.Clear();
                ReadSignature();
                ReadParentList();
                context.SubSignatures.Clear();
                BtnClearSub_Click(null, null);
                BtnClear_Click(null, null);
            }
            else
            {
                string message = Application.Current.FindResource("ChooseSignature.CodeBehind.ErrorRemove.Message").ToString();
                string caption = Application.Current.FindResource("ChooseSignature.CodeBehind.ErrorRemove.Caption").ToString();
                MessageBoxResult result = CustomMessageBox.ShowOK(message, caption, CustomMessageBoxButton.OK, MessageBoxImage.Error);
            }
            r.Close();
            con.Close();
            //context.MainSig = null;
            //context.SubSig = null;
        }

        private void RemoveSubSigCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            ChooseSignaturesModel context = this.DataContext as ChooseSignaturesModel;
            e.CanExecute = context.MainSig != null || context.SubSig != null;
        }

        private async void RemoveSubSigCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ChooseSignaturesModel context = this.DataContext as ChooseSignaturesModel;
            SqlMethods.SqlConnect(out SQLiteConnection con);
            SQLiteCommand removeCommand = con.CreateCommand();

            var removeItems = SubList.SelectedItems;
            SQLiteTransaction tr = con.BeginTransaction();
            removeCommand.Transaction = tr;
            await Task.Run(async () =>
            {
                foreach (var item in removeItems)
                {
                    Signature temp = item as Signature;
                    removeCommand.CommandText = $"DELETE FROM Signatures WHERE SignatureId=@SignatureId";
                    removeCommand.Parameters.AddWithValue("SignatureId", temp.Id);
                    await removeCommand.ExecuteNonQueryAsync();
                    removeCommand.Parameters.Clear();
                }
                tr.Commit();
            });
            ReadSubSignature(context.MainSig.Id);
            con.Close();
            BtnClearSub_Click(null, null);
        }

        private void SaveMainSigCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            ChooseSignaturesModel context = this.DataContext as ChooseSignaturesModel;
            e.CanExecute = !string.IsNullOrWhiteSpace(context.Name) && !string.IsNullOrWhiteSpace(context.Info);
        }

        private void SaveMainSigCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ChooseSignaturesModel context = this.DataContext as ChooseSignaturesModel;
            SqlMethods.SqlConnect(out SQLiteConnection con);
            SQLiteCommand selectCommand = con.CreateCommand();
            SQLiteCommand updateCommand = con.CreateCommand();
            selectCommand.CommandText = $"SELECT SignatureId, Signature, Info FROM Signatures WHERE ParentId IS NULL AND Signature = @Name";
            selectCommand.Parameters.AddWithValue("Name", context.Name);
            SQLiteDataReader r = selectCommand.ExecuteReader();
            selectCommand.Parameters.Clear();
            if (!r.Read())
            {
                if (context.CurrentId == 0)
                {
                    SQLiteCommand insertCommand = con.CreateCommand();
                    insertCommand.CommandText = $"INSERT INTO Signatures (Signature,Info) VALUES (@Name,@Info)";
                    insertCommand.Parameters.AddWithValue("Name", context.Name);
                    insertCommand.Parameters.AddWithValue("Info", context.Info);
                    insertCommand.ExecuteNonQuery();
                    insertCommand.Parameters.Clear();
                }
                else
                {

                    updateCommand.CommandText = $"UPDATE Signatures SET Signature=@Name, Info=@Info WHERE SignatureId=@SignatureId";
                    updateCommand.Parameters.AddWithValue("Name", context.Name);
                    updateCommand.Parameters.AddWithValue("Info", context.Info);
                    updateCommand.Parameters.AddWithValue("SignatureId", context.CurrentId);
                    updateCommand.ExecuteNonQuery();
                    updateCommand.Parameters.Clear();
                }
                BtnClear_Click(null, null);
                // reset label of main signature
                ResetLabel(context, main: true);
                ReadSignature();
                ReadParentList();
                context.SubSignatures.Clear();
            }
            else
            {
                if (context.CurrentId != 0)
                {
                    updateCommand.CommandText = $"UPDATE Signatures SET Signature=@Name, Info=@Info WHERE SignatureId=@SignatureId";
                    updateCommand.Parameters.AddWithValue("Name", context.Name);
                    updateCommand.Parameters.AddWithValue("Info", context.Info);
                    updateCommand.Parameters.AddWithValue("SignatureId", context.CurrentId);
                    updateCommand.ExecuteNonQuery();
                    updateCommand.Parameters.Clear();
                    BtnClear_Click(null, null);
                    // reset label of main signature
                    ResetLabel(context, main: true);
                    ReadSignature();
                    ReadParentList();
                    context.SubSignatures.Clear();
                }
                else
                {
                    string message = Application.Current.FindResource("ChooseSignature.CodeBehind.ErrorSave.Message").ToString();
                    string caption = Application.Current.FindResource("ChooseSignature.CodeBehind.ErrorSave.Caption").ToString();
                    MessageBoxResult result = CustomMessageBox.ShowOK(message, caption, CustomMessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            r.Close();
            con.Close();

            //if (context.ParentId == 0)
            //{
            //    selectCommand.CommandText = $"SELECT SignatureId, Signature, Info FROM Signatures WHERE ParentId IS NULL AND Signature = @Name";
            //    selectCommand.Parameters.AddWithValue("Name", context.Name);
            //}
            //else
            //{
            //    selectCommand.CommandText = $"SELECT SignatureId, Signature, Info FROM Signatures WHERE ParentId = @ParentId AND Signature = @Name";
            //    selectCommand.Parameters.AddWithValue("ParentId", context.ParentId);
            //    selectCommand.Parameters.AddWithValue("Name", context.Name);
            //}
            //SQLiteDataReader r = selectCommand.ExecuteReader();
            //selectCommand.Parameters.Clear();
            //if (!r.Read())
            //{
            //    // to add new signature
            //    if (context.CurrentId == 0)
            //    {
            //        SQLiteCommand insertCommand = con.CreateCommand();
            //        if (context.ParentId == 0)
            //        {
            //            insertCommand.CommandText = $"INSERT INTO Signatures (Signature,Info) VALUES (@Name,@Info)";
            //            insertCommand.Parameters.AddWithValue("Name", context.Name);
            //            insertCommand.Parameters.AddWithValue("Info", context.Info);
            //        }
            //        else
            //        {
            //            insertCommand.CommandText = $"INSERT INTO Signatures (Signature,Info,ParentId,Sort) VALUES (@Name,@Info,@ParentId,@Sort)";
            //            insertCommand.Parameters.AddWithValue("Name", context.Name);
            //            insertCommand.Parameters.AddWithValue("Info", context.Info);
            //            insertCommand.Parameters.AddWithValue("ParentId", context.ParentId);
            //            insertCommand.Parameters.AddWithValue("Sort", context.Sort);
            //        }
            //        insertCommand.ExecuteNonQuery();
            //        insertCommand.Parameters.Clear();
            //    }

            //    // to update place's State and country
            //    else
            //    {
            //        SQLiteCommand updateCommand = con.CreateCommand();
            //        if (context.ParentId == 0)
            //        {
            //            updateCommand.CommandText = $"UPDATE Signatures SET Signature=@Name, Info=@Info WHERE SignatureId=@SignatureId";
            //            updateCommand.Parameters.AddWithValue("Name", context.Name);
            //            updateCommand.Parameters.AddWithValue("Info", context.Info);
            //            updateCommand.Parameters.AddWithValue("SignatureId", context.CurrentId);
            //        }
            //        else
            //        {
            //            updateCommand.CommandText = $"UPDATE Signatures SET Signature=@Name, Info=@Info,Sort=@Sort WHERE SignatureId=@SignatureId";
            //            updateCommand.Parameters.AddWithValue("Name", context.Name);
            //            updateCommand.Parameters.AddWithValue("Info", context.Info);
            //            updateCommand.Parameters.AddWithValue("Sort", context.Sort);
            //            updateCommand.Parameters.AddWithValue("SignatureId", context.CurrentId);
            //        }
            //        updateCommand.ExecuteNonQuery();
            //        updateCommand.Parameters.Clear();
            //    }
            //}
            //else
            //{
            //    // to update signature
            //    if (context.CurrentId != 0)
            //    {
            //        SQLiteCommand updateCommand = con.CreateCommand();
            //        if (context.ParentId == 0)
            //        {
            //            updateCommand.CommandText = $"UPDATE Signatures SET Signature=@Name, Info=@Info WHERE SignatureId=@SignatureId";
            //            updateCommand.Parameters.AddWithValue("Name", context.Name);
            //            updateCommand.Parameters.AddWithValue("Info", context.Info);
            //            updateCommand.Parameters.AddWithValue("SignatureId", context.CurrentId);
            //        }
            //        else
            //        {
            //            updateCommand.CommandText = $"UPDATE Signatures SET Signature=@Name, Info=@Info,Sort=@Sort WHERE SignatureId=@SignatureId";
            //            updateCommand.Parameters.AddWithValue("Name", context.Name);
            //            updateCommand.Parameters.AddWithValue("Info", context.Info);
            //            updateCommand.Parameters.AddWithValue("Sort", context.Sort);
            //            updateCommand.Parameters.AddWithValue("SignatureId", context.CurrentId);
            //        }
            //        updateCommand.ExecuteNonQuery();
            //        updateCommand.Parameters.Clear();
            //    }
            //    else
            //    {
            //        string message = Application.Current.FindResource("ChooseSignature.CodeBehind.ErrorSave.Message").ToString();
            //        string caption = Application.Current.FindResource("ChooseSignature.CodeBehind.ErrorSave.Caption").ToString();
            //        MessageBoxResult result = CustomMessageBox.ShowOK(message, caption, CustomMessageBoxButton.OK, MessageBoxImage.Error);
            //    }
            //}
            //con.Close();
            //context.SubSig = null;
            //context.SubSignatures.Clear();
            //ClearEntries(context);
            //ReadSignature();
            //ReadParentList();
        }

        private void BtnClearSub_Click(object sender, RoutedEventArgs e)
        {
            ChooseSignaturesModel context = this.DataContext as ChooseSignaturesModel;
            context.SubSig = null;
            ClearEntries(context, "Name", "Info", "ParentId", main: false);
            //if (!string.IsNullOrWhiteSpace(context.Name) || !string.IsNullOrWhiteSpace(context.Info))
            //{
            //    EditLabel(context, sub: false); 
            //}
            //else
            //{
            //    ResetLabel(context, main: true);
            //}
            context.CurrentId = context.ParentId;
        }

        private void SaveSubSigCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            ChooseSignaturesModel context = this.DataContext as ChooseSignaturesModel;
            e.CanExecute = !string.IsNullOrWhiteSpace(context.SubName) && !string.IsNullOrWhiteSpace(context.SubInfo) && context.ParentId != 0;
        }
        private void SaveSubSigCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ChooseSignaturesModel context = this.DataContext as ChooseSignaturesModel;
            Signature subSig = new Signature { Name = context.SubName, Info = context.SubInfo };
            SqlMethods.SqlConnect(out SQLiteConnection con);
            SQLiteCommand selectCommand = con.CreateCommand();
            SQLiteCommand updateCommand = con.CreateCommand();
            selectCommand.CommandText = $"SELECT SignatureId, Signature, Info FROM Signatures WHERE ParentId = @ParentId AND Signature = @Name";
            selectCommand.Parameters.AddWithValue("ParentId", context.ParentId);
            selectCommand.Parameters.AddWithValue("Name", context.SubName);
            SQLiteDataReader r = selectCommand.ExecuteReader();
            selectCommand.Parameters.Clear();
            if (!r.Read())
            {
                if (context.CurrentSubId == 0)
                {
                    var insertCommand = con.CreateCommand();
                    insertCommand.CommandText = $"INSERT INTO Signatures (Signature,Info,ParentId,Sort) VALUES (@Name,@Info,@ParentId,@Sort)";
                    insertCommand.Parameters.AddWithValue("Name", context.SubName);
                    insertCommand.Parameters.AddWithValue("Info", context.SubInfo);
                    insertCommand.Parameters.AddWithValue("ParentId", context.ParentId);
                    insertCommand.Parameters.AddWithValue("Sort", context.Sort);
                    try
                    {
                        insertCommand.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                    insertCommand.Parameters.Clear();
                }
                else
                {
                    updateCommand.CommandText = $"UPDATE Signatures SET Signature=@Name, Info=@Info,Sort=@Sort WHERE SignatureId=@SignatureId";
                    updateCommand.Parameters.AddWithValue("Name", context.SubName);
                    updateCommand.Parameters.AddWithValue("Info", context.SubInfo);
                    updateCommand.Parameters.AddWithValue("Sort", context.Sort);
                    updateCommand.Parameters.AddWithValue("SignatureId", context.CurrentSubId);
                    try
                    {
                        updateCommand.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                    updateCommand.Parameters.Clear();
                }
                BtnClearSub_Click(null, null);
                // reset label of sub signature
                ResetLabel(context, main: false);
                ReadSubSignature(context.ParentId);
            }
            else
            {
                if (context.CurrentSubId != 0)
                {
                    updateCommand.CommandText = $"UPDATE Signatures SET Signature=@Name, Info=@Info,Sort=@Sort WHERE SignatureId=@SignatureId";
                    updateCommand.Parameters.AddWithValue("Name", context.SubName);
                    updateCommand.Parameters.AddWithValue("Info", context.SubInfo);
                    updateCommand.Parameters.AddWithValue("Sort", context.Sort);
                    updateCommand.Parameters.AddWithValue("SignatureId", context.CurrentSubId);
                    try
                    {
                        updateCommand.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                    updateCommand.Parameters.Clear();
                    BtnClearSub_Click(null, null);
                    ResetLabel(context, main: false);
                    ReadSubSignature(context.ParentId);
                }
                else
                {
                    string message = Application.Current.FindResource("ChooseSignature.CodeBehind.ErrorSave.Message").ToString();
                    string caption = Application.Current.FindResource("ChooseSignature.CodeBehind.ErrorSave.Caption").ToString();
                    MessageBoxResult result = CustomMessageBox.ShowOK(message, caption, CustomMessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            r.Close();
            con.Close();

        }
    }
}
