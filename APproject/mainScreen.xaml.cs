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
                        break;

                    case "CreditsScreen":
                        contentArea.Content = new ProductsOp();
                        break;

                    case "CompanyScreen":
                        contentArea.Content = new compOP();
                        break;

                    case "ReportScreen":
                        contentArea.Content = new ProductsOp();
                        break;
                }
            }
        }
    }


}