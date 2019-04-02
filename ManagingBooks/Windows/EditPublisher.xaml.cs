using ManagingBooks.Model;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ManagingBooks.Windows
{
    /// <summary>
    /// Interaction logic for EditPublisher.xaml
    /// </summary>
    public partial class EditPublisher : Window
    {
        public EditPublisher()
        {
            EditPublisherModel context = new EditPublisherModel();
            context.ListPublisher = new ObservableCollection<Publisher>();
            DataContext = context;
            GetPublisher();
            InitializeComponent();
        }

        private void GetPublisher()
        {
            EditPublisherModel context = this.DataContext as EditPublisherModel;
            context.ListPublisher.Clear();
            SqlMethods.SqlConnect(out SqliteConnection con);
            var selectCommand = con.CreateCommand();
            selectCommand.CommandText = "SELECT Name, City, Country FROM Publishers ORDER BY Name";
            SqliteDataReader r = selectCommand.ExecuteReader();
            while (r.Read())
            {
                Publisher pub = new Publisher();
                pub.Name = Convert.ToString(r["Name"]);
                pub.City = Convert.ToString(r["City"]);
                pub.Country = Convert.ToString(r["Country"]);
                context.ListPublisher.Add(pub);
            }
            r.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            EditPublisherModel context = this.DataContext as EditPublisherModel;
            if (!string.IsNullOrWhiteSpace(context.Name) || !string.IsNullOrWhiteSpace(context.City) || !string.IsNullOrWhiteSpace(context.Country))
            {
                MessageBoxResult result = MessageBox.Show("Do you want to discard?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
                e.Cancel = result == MessageBoxResult.No;
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearEntries();
        }

        private void ClearEntries()
        {
            foreach (var ctrl in MainContent.Children)
            {
                if (ctrl.GetType() == typeof(TextBox))
                {
                    (ctrl as TextBox).Clear();
                }
            }
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
                SqlMethods.SqlConnect(out SqliteConnection con);
                var tr = con.BeginTransaction();
                SqliteCommand removeCommand = con.CreateCommand();
                removeCommand.Transaction = tr;
                await Task.Run(() =>
                {
                    for (int i = removingItems.Count - 1; i >= 0; i--)
                    {
                        Publisher temp = removingItems[i] as Publisher;
                        removeCommand.CommandText = $"DELETE FROM Publishers WHERE Name='{temp.Name}'";
                        removeCommand.ExecuteNonQueryAsync();
                        Thread.Sleep(TimeSpan.FromTicks(500));
                    }
                    tr.Commit();
                    con.Close();
                });
                GetPublisher();
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
            SqlMethods.SqlConnect(out SqliteConnection con);
            SqliteCommand selectCommand = con.CreateCommand();
            selectCommand.CommandText = $"SELECT Name FROM Publishers WHERE Name='{context.Name}'";
            SqliteDataReader r = selectCommand.ExecuteReader();
            if (!r.Read())
            {
                SqliteCommand insertCommand = con.CreateCommand();
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
                con.Close();
                ClearEntries();
                GetPublisher();
            }
            else
            {
                MessageBox.Show("Publisher exists!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            
        }
    }
}
