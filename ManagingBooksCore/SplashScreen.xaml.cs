using System;
using System.Collections.Generic;
using System.IO;
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
        public static string ResourcePath = System.IO.Path.Combine(AppContext.BaseDirectory, "Resources\\Language.config");
        Uri English = new Uri(".\\Resources\\Resources.xaml", UriKind.Relative);
        Uri German = new Uri(".\\Resources\\Resources.de.xaml", UriKind.Relative);

        public SplashScreen()
        {
            InitializeComponent();
            this.Height = SystemParameters.FullPrimaryScreenHeight * 0.5;
            this.Width = SystemParameters.PrimaryScreenWidth * 0.5;
            Dt.Tick += Dt_Tick;
            Dt.Interval = new TimeSpan(0, 0, 2);
            Dt.Start();

            using (StreamReader r = new StreamReader(ResourcePath))
            {
                int.TryParse(r.ReadLine(), out int lang);
                ResourceDictionary dict = new ResourceDictionary();
                if (lang == 0)
                {
                    dict.Source = English;
                }
                else
                {
                    dict.Source = German;
                }
                Application.Current.Resources.MergedDictionaries.Clear();
                Application.Current.Resources.MergedDictionaries.Add(dict);
            }
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
