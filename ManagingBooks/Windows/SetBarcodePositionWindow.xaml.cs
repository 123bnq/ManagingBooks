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
        public bool IsClosed { get; private set; }
        public SetBarcodePositionWindow()
        {
            IsClosed = false;
            this.DataContext = new SetBarcodePositionModel();
            InitializeComponent();
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            IsClosed = true;
        }
    }
}
