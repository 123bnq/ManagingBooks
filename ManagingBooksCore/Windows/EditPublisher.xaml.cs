using ManagingBooks.Model;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data.SQLite;
using WPFCustomMessageBox;

namespace ManagingBooks.Windows
{
    /// <summary>
    /// Interaction logic for EditPublisher.xaml
    /// </summary>
    public partial class EditPublisher : Window
    {
        private GridViewColumnHeader listViewSortCol = null;
        private SortAdorner listViewSortAdorner = null;

        public EditPublisher()
        {
            EditPublisherModel context = new EditPublisherModel();
            ClearEntries(context);
            context.ListPublisher = new ObservableCollection<Publisher>();
            DataContext = context;
            GetPublisher();
            InitializeComponent();
        }

        private void GetPublisher()
        {
            EditPublisherModel context = this.DataContext as EditPublisherModel;
            context.ListPublisher.Clear();
            SqlMethods.SqlConnect(out SQLiteConnection con);
            var selectCommand = con.CreateCommand();
            selectCommand.CommandText = "SELECT Id, Name, City, Country FROM Publishers ORDER BY Name";
            SQLiteDataReader r = selectCommand.ExecuteReader();
            while (r.Read())
            {
                Publisher pub = new Publisher();
                int tempNum;
                int.TryParse(Convert.ToString(r["Id"]), out tempNum);
                pub.Id = tempNum;
                pub.Name = Convert.ToString(r["Name"]);
                pub.City = Convert.ToString(r["City"]);
                pub.Country = Convert.ToString(r["Country"]);
                context.ListPublisher.Add(pub);
            }
            r.Close();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            EditPublisherModel context = this.DataContext as EditPublisherModel;
            if (!string.IsNullOrWhiteSpace(context.Name) || !string.IsNullOrWhiteSpace(context.City) || !string.IsNullOrWhiteSpace(context.Country))
            {
                string message = Application.Current.FindResource("EditPublisher.CodeBehind.WarningClose.Message").ToString();
                string caption = Application.Current.FindResource("EditPublisher.CodeBehind.WarningClose.Caption").ToString();
                MessageBoxResult result = CustomMessageBox.ShowYesNo(message, caption, CustomMessageBoxButton.Yes, CustomMessageBoxButton.No, MessageBoxImage.Warning);
                //MessageBoxResult result = MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
                e.Cancel = result == MessageBoxResult.No;
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            EditPublisherModel context = this.DataContext as EditPublisherModel;
            ClearEntries(context);
            PubList.SelectedItem = null;
        }

        private void ClearEntries(EditPublisherModel context)
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
            context.LabelName = Application.Current.FindResource("EditPublisher.Label.Name").ToString();
            context.LabelCity = Application.Current.FindResource("EditPublisher.Label.City").ToString();
            context.LabelCountry = Application.Current.FindResource("EditPublisher.Label.Country").ToString();
            CommandManager.InvalidateRequerySuggested();
        }

        private void RemovePubCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (PubList.SelectedItem != null);
        }

        private async void RemovePubCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var removingItems = PubList.SelectedItems;
            if (PubList.SelectedIndex != -1)
            {
                SqlMethods.SqlConnect(out SQLiteConnection con);
                var tr = con.BeginTransaction();
                SQLiteCommand removeCommand = con.CreateCommand();
                removeCommand.Transaction = tr;
                await Task.Run(() =>
                {
                    for (int i = removingItems.Count - 1; i >= 0; i--)
                    {
                        Publisher temp = removingItems[i] as Publisher;
                        removeCommand.CommandText = $"DELETE FROM Publishers WHERE Name='{temp.Name}'";
                        removeCommand.ExecuteNonQueryAsync();
                    }
                    tr.Commit();
                    con.Close();
                });
                GetPublisher();
                ClearEntries(this.DataContext as EditPublisherModel);
            }
        }

        private void SavePubCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            EditPublisherModel context = this.DataContext as EditPublisherModel;
            e.CanExecute = !string.IsNullOrWhiteSpace(context.Name) && !string.IsNullOrWhiteSpace(context.City);
        }

        private void SavePubCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            EditPublisherModel context = this.DataContext as EditPublisherModel;
            SqlMethods.SqlConnect(out SQLiteConnection con);
            SQLiteCommand selectCommand = con.CreateCommand();
            selectCommand.CommandText = $"SELECT Name FROM Publishers WHERE Name='{context.Name}'";
            SQLiteDataReader r = selectCommand.ExecuteReader();
            if (!r.Read())
            {
                // to add new Publisher
                if (context.Id == 0)
                {
                    SQLiteCommand insertCommand = con.CreateCommand();
                    if (!string.IsNullOrWhiteSpace(context.City) && !string.IsNullOrWhiteSpace(context.Country))
                    {
                        insertCommand.CommandText = "INSERT INTO Publishers (Name,City,Country) VALUES (@Name,@City,@Country)";
                        insertCommand.Parameters.AddWithValue("Name", context.Name);
                        insertCommand.Parameters.AddWithValue("City", context.City);
                        insertCommand.Parameters.AddWithValue("Country", context.Country);
                    }
                    else
                    {
                        insertCommand.CommandText = "INSERT INTO Publishers (Name,City) VALUES (@Name,@City)";
                        insertCommand.Parameters.AddWithValue("Name", context.Name);
                        insertCommand.Parameters.AddWithValue("City", context.City);
                    }
                    insertCommand.ExecuteNonQuery();
                }

                // to update publisher's name
                else
                {
                    SQLiteCommand updateCommand = con.CreateCommand();
                    updateCommand.CommandText = $"UPDATE Publishers SET Name='{context.Name}', City='{context.City}', Country='{context.Country}' WHERE Id={context.Id}";
                    updateCommand.ExecuteNonQuery();
                }
            }
            else
            {
                // to update old publisher's place
                if (context.Id != 0)
                {
                    SQLiteCommand updateCommand = con.CreateCommand();
                    updateCommand.CommandText = $"UPDATE Publishers SET Name='{context.Name}', City='{context.City}', Country='{context.Country}' WHERE Id={context.Id}";
                    updateCommand.ExecuteNonQuery();
                }
                else
                {
                    string message = Application.Current.FindResource("EditPublisher.CodeBehind.ErrorSave.Message").ToString();
                    string caption = Application.Current.FindResource("EditPublisher.CodeBehind.ErrorSave.Caption").ToString();
                    MessageBoxResult result = CustomMessageBox.ShowOK(message, caption, CustomMessageBoxButton.OK, MessageBoxImage.Error);
                    //MessageBoxResult result = MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            con.Close();
            ClearEntries(context);
            GetPublisher();
        }

        /// <summary>
        /// Provide edit function for the selected Publisher
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PubList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EditPublisherModel context = this.DataContext as EditPublisherModel;
            Publisher pub = PubList.SelectedItem as Publisher;
            if (pub != null)
            {
                context.LabelName = Application.Current.FindResource("EditPublisher.Label.EditName").ToString();
                context.LabelCity = Application.Current.FindResource("EditPublisher.Label.EditCity").ToString();
                context.LabelCountry = Application.Current.FindResource("EditPublisher.Label.EditCountry").ToString();
                context.Id = pub.Id;
                context.Name = pub.Name;
                context.City = pub.City;
                context.Country = pub.Country;
            }
        }

        /// <summary>
        /// Sorting function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            SortAdorner.SortClick(sender, e, ref listViewSortCol, ref listViewSortAdorner, ref PubList);
        }
    }
}