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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ManagingBooks.Model;
using ManagingBooks.Windows;

namespace ManagingBooks
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new SearchBookModel();
            SearchList.ItemsSource = (DataContext as SearchBookModel).DisplayBooks;
        }

        private void AddWindow_Click(object sender, RoutedEventArgs e)
        {
            new AddBook() { Owner = this }.ShowDialog();
        }
        private void ExitCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ExitCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void SearchCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !string.IsNullOrWhiteSpace(BoxSearchText.Text);
        }

        private void SearchCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SearchBookModel context = (this.DataContext as SearchBookModel);

            MessageBox.Show($"Search is executed\nSearch for: {context.SearchText}\nSearch By: {context.SearchBy}", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            (this.DataContext as SearchBookModel).SearchText = string.Empty;
        }
    }

    public static class CustomCommands
    {
        public static readonly RoutedUICommand Exit = new RoutedUICommand("Exit", "Exit", typeof(CustomCommands), new InputGestureCollection()
        {
            new KeyGesture(Key.Q, ModifierKeys.Control)
        });

        public static readonly RoutedUICommand Search = new RoutedUICommand("Search", "Search", typeof(CustomCommands));
    }
}
