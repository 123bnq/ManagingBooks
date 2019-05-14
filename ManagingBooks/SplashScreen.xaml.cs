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
using System.Windows.Threading;

namespace ManagingBooks
{
    /// <summary>
    /// Interaction logic for SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen : Window
    {
        DispatcherTimer Dt = new DispatcherTimer();

        public SplashScreen()
        {
            InitializeComponent();
            Dt.Tick += Dt_Tick;
            Dt.Interval = new TimeSpan(0, 0, 2);
            Dt.Start();
        }

        private void Dt_Tick(object sender, EventArgs e)
        {
            MainWindow main = new MainWindow();
            Dt.Stop();
            this.Close();
            main.Show();
        }
    }
}
