﻿using ManagingBooks.Model;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data.SQLite;
using WPFCustomMessageBox;

namespace ManagingBooks.Windows
{
    /// <summary>
    /// Interaction logic for EditMedium.xaml
    /// </summary>
    public partial class EditMedium : Window
    {
        private GridViewColumnHeader listViewSortCol = null;
        private SortAdorner listViewSortAdorner = null;

        public EditMedium()
        {
            EditMediumModel context = new EditMediumModel();
            ClearEntries(context);
            context.ListMedium = new ObservableCollection<Medium>();
            this.DataContext = context;
            GetMedium();
            InitializeComponent();
        }

        private void ClearEntries(EditMediumModel context)
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
            context.LabelName = Application.Current.FindResource("EditMedium.Label.Name").ToString();
            CommandManager.InvalidateRequerySuggested();
        }

        /// <summary>
        /// Fetch data from DB
        /// </summary>
        private void GetMedium()
        {
            EditMediumModel context = this.DataContext as EditMediumModel;
            context.ListMedium.Clear();
            SqlMethods.SqlConnect(out SQLiteConnection con);
            var selectCommand = con.CreateCommand();
            selectCommand.CommandText = "SELECT MediumId, Name FROM Mediums ORDER BY Name";
            SQLiteDataReader r = selectCommand.ExecuteReader();
            while (r.Read())
            {
                Medium med = new Medium();
                int tempNum;
                int.TryParse(Convert.ToString(r["MediumId"]), out tempNum);
                med.Id = tempNum;
                med.Name = Convert.ToString(r["Name"]);
                context.ListMedium.Add(med);
            }
            r.Close();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            EditMediumModel context = this.DataContext as EditMediumModel;
            if (!string.IsNullOrWhiteSpace(context.Name))
            {
                string message = Application.Current.FindResource("EditMeidum.CodeBehind.WarningClose.Message").ToString();
                string caption = Application.Current.FindResource("EditMeidum.CodeBehind.WarningClose.Caption").ToString();
                MessageBoxResult result = CustomMessageBox.ShowYesNo(message, caption, CustomMessageBoxButton.Yes, CustomMessageBoxButton.No, MessageBoxImage.Warning);
                //MessageBoxResult result = MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
                e.Cancel = result == MessageBoxResult.No;
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            EditMediumModel context = this.DataContext as EditMediumModel;
            ClearEntries(context);
            MediumList.SelectedItem = null;
        }

        private void RemoveMediumCommand_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = MediumList.SelectedItem != null;
        }

        private async void RemoveMediumCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            var removingItems = MediumList.SelectedItems;
            if (MediumList.SelectedIndex != -1)
            {
                SqlMethods.SqlConnect(out SQLiteConnection con);
                var tr = con.BeginTransaction();
                SQLiteCommand removeCommand = con.CreateCommand();
                removeCommand.Transaction = tr;
                await Task.Run( async () =>
                {
                    for (int i = removingItems.Count - 1; i >= 0; i--)
                    {
                        Medium temp = removingItems[i] as Medium;
                        removeCommand.CommandText = $"DELETE FROM Mediums WHERE Name=@Name";
                        removeCommand.Parameters.AddWithValue("Name", temp.Name);
                        await removeCommand.ExecuteNonQueryAsync();
                        removeCommand.Parameters.Clear();
                    }
                    tr.Commit();
                    con.Close();
                });
                GetMedium();
            }
            ClearEntries(this.DataContext as EditMediumModel);
        }

        private void SaveMediumCommand_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            EditMediumModel context = this.DataContext as EditMediumModel;
            e.CanExecute = !string.IsNullOrEmpty(context.Name);
        }

        private void SaveMediumCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            EditMediumModel context = this.DataContext as EditMediumModel;
            SqlMethods.SqlConnect(out SQLiteConnection con);
            SQLiteCommand selectCommand = con.CreateCommand();
            selectCommand.CommandText = $"SELECT MediumId, Name FROM Mediums WHERE Name=@Name";
            selectCommand.Parameters.AddWithValue("Name", context.Name);
            SQLiteDataReader r = selectCommand.ExecuteReader();
            selectCommand.Parameters.Clear();
            if (!r.Read())
            {
                // to add new medium
                if (context.Id == 0)
                {
                    SQLiteCommand insertCommand = con.CreateCommand();
                    insertCommand.CommandText = $"INSERT INTO Mediums (Name) VALUES (@Name)";
                    insertCommand.Parameters.AddWithValue("Name", context.Name);
                    insertCommand.ExecuteNonQuery();
                    insertCommand.Parameters.Clear();
                }

                // to update medium's name
                else
                {
                    SQLiteCommand updateCommand = con.CreateCommand();
                    updateCommand.CommandText = $"UPDATE Mediums SET Name=@Name WHERE MediumId=@Id";
                    updateCommand.Parameters.AddWithValue("Name", context.Name);
                    updateCommand.Parameters.AddWithValue("Id", context.Id);
                    updateCommand.ExecuteNonQuery();
                    updateCommand.Parameters.Clear();
                }
            }
            else
            {
                // to update medium's name
                if (context.Id != 0)
                {
                    SQLiteCommand updateCommand = con.CreateCommand();
                    updateCommand.CommandText = $"UPDATE Mediums SET Name=@Name WHERE MediumId=@Id";
                    updateCommand.Parameters.AddWithValue("Name", context.Name);
                    updateCommand.Parameters.AddWithValue("Id", context.Id);
                    updateCommand.ExecuteNonQuery();
                    updateCommand.Parameters.Clear();
                }
                else
                {
                    string message = Application.Current.FindResource("EditMeidum.CodeBehind.ErrorSave.Message").ToString();
                    string caption = Application.Current.FindResource("EditMeidum.CodeBehind.ErrorSave.Caption").ToString();
                    CustomMessageBox.ShowOK(message, caption, CustomMessageBoxButton.OK, MessageBoxImage.Error);
                    //MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            r.Close();
            con.Close();
            ClearEntries(context);
            GetMedium();
        }

        /// <summary>
        /// Provide option to edit the selected medium
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MediumList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            EditMediumModel context = this.DataContext as EditMediumModel;
            Medium med = MediumList.SelectedItem as Medium;
            if (med != null)
            {
                context.LabelName = Application.Current.FindResource("EditMedium.Label.EditName").ToString();
                context.Id = med.Id;
                context.Name = med.Name;
            }
        }

        /// <summary>
        /// Sorting function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            SortAdorner.SortClick(sender, e, ref listViewSortCol, ref listViewSortAdorner, ref MediumList);
        }
    }
}
