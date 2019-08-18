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
using ManagingBooks.Model;

namespace ManagingBooks.Windows
{
    /// <summary>
    /// Interaction logic for SetBarcodePositionWindow.xaml
    /// </summary>
    public partial class SetBarcodePositionWindow : Window
    {
        public static List<Tuple<int, int>> OccupyPositions = new List<Tuple<int, int>>();

        public static int CountWindow = 0;
        public static int MaxNoWindow = 0;
        public static string Of;

        public bool IsConfirmed { get; private set; }
        public SetBarcodePositionWindow(List<int> rows, List<int> cols)
        {
            Of = App.Current.FindResource("SetBarcodePositionWindow.Title.Of").ToString();
            IsConfirmed = false;
            this.DataContext = new SetBarcodePositionModel(rows, cols);
            CountWindow++;
            InitializeComponent();
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            IsConfirmed = true;
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SetBarcodePositionModel context = this.DataContext as SetBarcodePositionModel;
            Tuple<int, int> combination = new Tuple<int, int>(context.RowNumber, context.ColNumber);
            if (OccupyPositions.Contains(combination) && IsConfirmed)
            {
                string caption = App.Current.FindResource("SetBarcodePositionWindow.CodeBehind.ErrorPosition.Caption").ToString();
                string message = App.Current.FindResource("SetBarcodePositionWindow.CodeBehind.ErrorPosition.Message").ToString();
                MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Error);
                IsConfirmed = false;
                e.Cancel = true;
            }
        }
    }
}
