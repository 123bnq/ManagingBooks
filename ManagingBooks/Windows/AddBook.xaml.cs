using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
using System.Data.SqlClient;
using Microsoft.Data.Sqlite;

namespace ManagingBooks.Windows
{
    /// <summary>
    /// Interaction logic for AddBook.xaml
    /// </summary>
    public partial class AddBook : Window
    {
        public AddBook()
        {
            InitializeComponent();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            foreach (var ctrl in WindowToBeClear.Children)
            {
                if(ctrl.GetType() == typeof(TextBox))
                {
                    (ctrl as TextBox).Clear();
                }
                if(ctrl.GetType() == typeof(ComboBox))
                {
                    (ctrl as ComboBox).SelectedItem = null;
                }
                if(ctrl.GetType().IsSubclassOf(typeof(Panel)))
                {
                    foreach (var childCtrl in (ctrl as Panel).Children)
                    {
                        if (childCtrl.GetType() == typeof(TextBox))
                        {
                            (childCtrl as TextBox).Clear();
                        }
                        if (childCtrl.GetType() == typeof(ComboBox))
                        {
                            (childCtrl as ComboBox).SelectedItem = null;
                        }
                    }
                }
            }
        }

        private void AddBook_Closing(object sender, CancelEventArgs e)
        {
            bool containData = false;
            foreach (var ctrl in WindowToBeClear.Children)
            {
                if (ctrl.GetType() == typeof(TextBox))
                {
                    if(!string.IsNullOrWhiteSpace((ctrl as TextBox).Text))
                    {
                        containData |= true;
                        break;
                    }
                }
                if (ctrl.GetType() == typeof(ComboBox))
                {
                    if((ctrl as ComboBox).SelectedItem != null)
                    {
                        containData |= true;
                        break;
                    }
                }
                if (ctrl.GetType().IsSubclassOf(typeof(Panel)))
                {
                    foreach (var childCtrl in (ctrl as Panel).Children)
                    {
                        if (childCtrl.GetType() == typeof(TextBox))
                        {
                            if(!string.IsNullOrWhiteSpace((childCtrl as TextBox).Text))
                            {
                                containData |= true;
                                break;
                            }
                        }
                        if (childCtrl.GetType() == typeof(ComboBox))
                        {
                            if((childCtrl as ComboBox).SelectedItem != null)
                            {
                                containData |= true;
                                break;
                            }
                        }
                    }
                    if (containData)
                    {
                        break;
                    }
                }
            }

            if (containData)
            {
                string msg = "Book is not completely added. Discard?";
                MessageBoxResult result = MessageBox.Show(msg, "Cancel?", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if(result == MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            SqliteConnection con;
            SqlConnect(out con);
            
        }

        public void SqlConnect(out SqliteConnection con)
        {
            string path = System.IO.Path.Combine(AppContext.BaseDirectory, "Database.db");
            con = new SqliteConnection("" + new SqliteConnectionStringBuilder
            {
                DataSource = "Database.db"
            });
            con.Open();
            MessageBox.Show("Connection opened");
            var transaction = con.BeginTransaction();
            var insertCommand = con.CreateCommand();
            insertCommand.Transaction = transaction;
            insertCommand.CommandText = "INSERT INTO [Authors]([FirstName],[LastName]) VALUES ('James','Quin');";
            insertCommand.ExecuteNonQuery();
            transaction.Commit();
            con.Close();
            MessageBox.Show("Connection closed");
        }

        private bool IsDataToAdd()
        {

            return false;
        }
    }
}
