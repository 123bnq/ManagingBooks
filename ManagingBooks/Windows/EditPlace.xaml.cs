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
    /// Interaction logic for EditPlace.xaml
    /// </summary>
    public partial class EditPlace : Window
    {
        public EditPlace()
        {
            EditPlaceModel context = new EditPlaceModel();
            ClearEntries(context);
            context.ListPlace = new ObservableCollection<Place>();
            this.DataContext = context;
            GetPlace();
            InitializeComponent();
        }

        private void ClearEntries(EditPlaceModel context)
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
            context.LabelCity = Application.Current.FindResource("EditPlace.Label.City").ToString();
            context.LabelState = Application.Current.FindResource("EditPlace.Label.State").ToString();
            context.LabelCountry = Application.Current.FindResource("EditPlace.Label.Country").ToString();
            CommandManager.InvalidateRequerySuggested();
        }

        private void GetPlace()
        {
            EditPlaceModel context = this.DataContext as EditPlaceModel;
            context.ListPlace.Clear();
            SqlMethods.SqlConnect(out SqliteConnection con);
            var selectCommand = con.CreateCommand();
            selectCommand.CommandText = "SELECT Id, City, State, Country FROM Places ORDER BY City";
            SqliteDataReader r = selectCommand.ExecuteReader();
            while (r.Read())
            {
                Place place = new Place();
                int tempNum;
                int.TryParse(Convert.ToString(r["Id"]), out tempNum);
                place.Id = tempNum;
                place.City = Convert.ToString(r["City"]);
                place.State = Convert.ToString(r["State"]);
                place.Country = Convert.ToString(r["Country"]);
                context.ListPlace.Add(place);
            }
            r.Close();
        }

        private void RemovePlaceCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (PlaceList.SelectedItem != null);
        }

        private async void RemovePlaceCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var removingItems = PlaceList.SelectedItems;
            if (PlaceList.SelectedIndex != -1)
            {
                SqlMethods.SqlConnect(out SqliteConnection con);
                var tr = con.BeginTransaction();
                SqliteCommand removeCommand = con.CreateCommand();
                removeCommand.Transaction = tr;
                await Task.Run(() =>
                {
                    for (int i = removingItems.Count - 1; i >= 0; i--)
                    {
                        Place temp = removingItems[i] as Place;
                        removeCommand.CommandText = $"DELETE FROM Places WHERE City='{temp.City}'";
                        removeCommand.ExecuteNonQueryAsync();
                    }
                    tr.Commit();
                    con.Close();
                });
                GetPlace();
            }
            ClearEntries(this.DataContext as EditPlaceModel);
        }

        private void SavePlaceCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            EditPlaceModel context = this.DataContext as EditPlaceModel;
            e.CanExecute = !string.IsNullOrWhiteSpace(context.City);
        }

        private void SavePlaceCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            EditPlaceModel context = this.DataContext as EditPlaceModel;
            SqlMethods.SqlConnect(out SqliteConnection con);
            SqliteCommand selectCommand = con.CreateCommand();
            selectCommand.CommandText = $"SELECT City FROM Places WHERE City='{context.City}'";
            SqliteDataReader r = selectCommand.ExecuteReader();
            if (!r.Read())
            {
                if (context.Id == 0)
                {
                    SqliteCommand insertCommand = con.CreateCommand();
                    if (!string.IsNullOrWhiteSpace(context.State) && !string.IsNullOrWhiteSpace(context.Country))
                    {
                        insertCommand.CommandText = "INSERT INTO Places (City,State,Country) VALUES (@City,@State,@Country)";
                        insertCommand.Parameters.AddWithValue("City", context.City);
                        insertCommand.Parameters.AddWithValue("State", context.State);
                        insertCommand.Parameters.AddWithValue("Country", context.Country);
                    }
                    else if (!string.IsNullOrWhiteSpace(context.State) && string.IsNullOrWhiteSpace(context.Country))
                    {
                        insertCommand.CommandText = "INSERT INTO Places (City,State) VALUES (@City,@State)";
                        insertCommand.Parameters.AddWithValue("City", context.City);
                        insertCommand.Parameters.AddWithValue("State", context.State);
                    }
                    else if (string.IsNullOrWhiteSpace(context.State) && !string.IsNullOrWhiteSpace(context.Country))
                    {
                        insertCommand.CommandText = "INSERT INTO Places (City,Country) VALUES (@City,@Country)";
                        insertCommand.Parameters.AddWithValue("City", context.City);
                        insertCommand.Parameters.AddWithValue("Country", context.Country);
                    }
                    else
                    {
                        insertCommand.CommandText = "INSERT INTO Places (City) VALUES (@City)";
                        insertCommand.Parameters.AddWithValue("City", context.City);
                    }
                    insertCommand.ExecuteNonQuery();
                }
                else
                {
                    SqliteCommand updateCommand = con.CreateCommand();
                    updateCommand.CommandText = $"UPDATE Places SET City='{context.City}', State='{context.State}', Country='{context.Country}' WHERE Id={context.Id}";
                    updateCommand.ExecuteNonQuery();
                }
                con.Close();
                ClearEntries(context);
                GetPlace();
            }
            else
            {
                string message = Application.Current.FindResource("EditPlace.CodeBehind.ErrorSave.Message").ToString();
                string caption = Application.Current.FindResource("EditPlace.CodeBehind.ErrorSave.Caption").ToString();
                MessageBoxResult result = CustomMessageBox.ShowOK(message, caption, CustomMessageBoxButton.OK, MessageBoxImage.Error);
                //MessageBoxResult result = MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            EditPlaceModel context = this.DataContext as EditPlaceModel;
            if(!string.IsNullOrWhiteSpace(context.City) || !string.IsNullOrWhiteSpace(context.State) || !string.IsNullOrWhiteSpace(context.Country))
            {
                string message = Application.Current.FindResource("EditPlace.CodeBehind.WarningClose.Message").ToString();
                string caption = Application.Current.FindResource("EditPlace.CodeBehind.WarningClose.Caption").ToString();
                MessageBoxResult result = CustomMessageBox.ShowYesNo(message, caption, CustomMessageBoxButton.Yes, CustomMessageBoxButton.No, MessageBoxImage.Warning);
                //MessageBoxResult result = MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
                e.Cancel = result == MessageBoxResult.No;
            }
        }

        private void PlaceList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EditPlaceModel context = this.DataContext as EditPlaceModel;
            Place place = PlaceList.SelectedItem as Place;
            if(place != null)
            {
                context.LabelCity = Application.Current.FindResource("EditPlace.Label.EditCity").ToString();
                context.LabelState = Application.Current.FindResource("EditPlace.Label.EditState").ToString();
                context.LabelCountry = Application.Current.FindResource("EditPlace.Label.EditCountry").ToString();
                context.Id = place.Id;
                context.City = place.City;
                context.State = place.State;
                context.Country = place.Country;
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearEntries(this.DataContext as EditPlaceModel);
            PlaceList.SelectedItem = null;
        }
    }
}
