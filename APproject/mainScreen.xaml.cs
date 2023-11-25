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

namespace APproject
{
    /// <summary>
    /// Interaction logic for mainScreen.xaml
    /// </summary>









    public partial class mainScreen : Window
    {
        public mainScreen()
        {
            InitializeComponent();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;

            if (menuItem != null && menuItem.Tag != null)
            {
                string screenName = menuItem.Tag.ToString();

                // Dynamically set the content based on the selected menu item
                switch (screenName)
                {
                    case "FarmersScreen":
                        contentArea.Content = new FarmerOp();
                        break;

                    case "ProductsScreen":
                        contentArea.Content = new ProductsOp();
                        addProductMenuItem.Visibility = Visibility.Visible;
                        break;

                    case "CreditsScreen":
                        contentArea.Content = new ProductsOp();
                        addProductMenuItem.Visibility = Visibility.Collapsed;
                        break;

                    case "CompanyScreen":
                        contentArea.Content = new compOP();
                        addProductMenuItem.Visibility = Visibility.Visible;
                        break;

                    case "ReportScreen":
                        contentArea.Content = new chartScreen();
                        addProductMenuItem.Visibility = Visibility.Collapsed;
                        break;
                }
            }
        }
        private void AddProductMenuItem_Click(object sender, RoutedEventArgs e)
        {
            AddProductWindow addpro = new AddProductWindow();
            addpro.Show();
        }

        private void LogoutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to log out?", "Logout Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Assuming MainWindow is your login or main window
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();

                // Close the current window
                this.Close();
            }
        }

    }


}