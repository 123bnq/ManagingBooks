using ManagingBooks.Model;
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
    /// Interaction logic for EditPlace.xaml
    /// </summary>
    public partial class EditPlace : Window
    {
        private GridViewColumnHeader listViewSortCol = null;
        private SortAdorner listViewSortAdorner = null;

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
            SqlMethods.SqlConnect(out SQLiteConnection con);
            var selectCommand = con.CreateCommand();
            selectCommand.CommandText = "SELECT Id, City, State, Country FROM Places ORDER BY City";
            SQLiteDataReader r = selectCommand.ExecuteReader();
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
                SqlMethods.SqlConnect(out SQLiteConnection con);
                var tr = con.BeginTransaction();
                SQLiteCommand removeCommand = con.CreateCommand();
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
            SqlMethods.SqlConnect(out SQLiteConnection con);
            SQLiteCommand selectCommand = con.CreateCommand();
            selectCommand.CommandText = $"SELECT City FROM Places WHERE City='{context.City}'";
            SQLiteDataReader r = selectCommand.ExecuteReader();
            if (!r.Read())
            {
                // to add new place
                if (context.Id == 0)
                {
                    SQLiteCommand insertCommand = con.CreateCommand();
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

                // to update place's State and country
                else
                {
                    SQLiteCommand updateCommand = con.CreateCommand();
                    updateCommand.CommandText = $"UPDATE Places SET City='{context.City}', State='{context.State}', Country='{context.Country}' WHERE Id={context.Id}";
                    updateCommand.ExecuteNonQuery();
                }
            }
            else
            {
                // to update place's city
                if (context.Id != 0)
                {
                    SQLiteCommand updateCommand = con.CreateCommand();
                    updateCommand.CommandText = $"UPDATE Places SET City='{context.City}', State='{context.State}', Country='{context.Country}' WHERE Id={context.Id}";
                    updateCommand.ExecuteNonQuery();
                }
                else
                {
                    string message = Application.Current.FindResource("EditPlace.CodeBehind.ErrorSave.Message").ToString();
                    string caption = Application.Current.FindResource("EditPlace.CodeBehind.ErrorSave.Caption").ToString();
                    MessageBoxResult result = CustomMessageBox.ShowOK(message, caption, CustomMessageBoxButton.OK, MessageBoxImage.Error);
                    //MessageBoxResult result = MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            con.Close();
            ClearEntries(context);
            GetPlace();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            EditPlaceModel context = this.DataContext as EditPlaceModel;
            if (!string.IsNullOrWhiteSpace(context.City) || !string.IsNullOrWhiteSpace(context.State) || !string.IsNullOrWhiteSpace(context.Country))
            {
                string message = Application.Current.FindResource("EditPlace.CodeBehind.WarningClose.Message").ToString();
                string caption = Application.Current.FindResource("EditPlace.CodeBehind.WarningClose.Caption").ToString();
                MessageBoxResult result = CustomMessageBox.ShowYesNo(message, caption, CustomMessageBoxButton.Yes, CustomMessageBoxButton.No, MessageBoxImage.Warning);
                //MessageBoxResult result = MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
                e.Cancel = result == MessageBoxResult.No;
            }
        }

        /// <summary>
        /// Provide edit function for the selected place
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlaceList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EditPlaceModel context = this.DataContext as EditPlaceModel;
            Place place = PlaceList.SelectedItem as Place;
            if (place != null)
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

        /// <summary>
        /// Sorting function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            SortAdorner.SortClick(sender, e, ref listViewSortCol, ref listViewSortAdorner, ref PlaceList);
        }
    }
}
