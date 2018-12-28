using System;
using System.Collections.Generic;
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
            }
        }
    }
}
