using System;
using System.Collections.Generic;
using System.Configuration; // Add reference to System.Configuration
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace APproject
{
  
        public partial class ProductsOp : UserControl
        {
        private string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Integrated Security=True";

        public ProductsOp()
            {
                InitializeComponent();

                // Load data when the control is initialized
                LoadData();
            }

            private void LoadData()
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        string sql = "SELECT * FROM Product"; // Assuming your table is named 'Product'

                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                List<Product> products = new List<Product>();

                                while (reader.Read())
                                {
                                    Product product = new Product
                                    {
                                        ProductID = Convert.ToInt32(reader["ProductID"]),
                                        Product_Name = Convert.ToString(reader["Product_Name"]),
                                        Price = Convert.ToDecimal(reader["Price"]),
                                        Policy = Convert.ToString(reader["Policy"]),
                                        Batch = Convert.ToInt32(reader["Batch"]),
                                        ExpireDate = Convert.ToDateTime(reader["ExpireDate"]),
                                        Quality = Convert.ToInt32(reader["Quality"])
                                    };

                                    products.Add(product);
                                }

                                // Assuming dataGrid is the name of your DataGrid control in XAML
                                dataGrid.ItemsSource = products;
                            }
                        }
                        connection.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            // Get the clicked button
            Button btn = (Button)sender;

            // Access the data item (Product) associated with the clicked row
            Product selectedProduct = (Product)btn.Tag;

            if (selectedProduct != null)
            {
                // Open the EditProductWindow for editing the selected product
                EditProductWindow editWindow = new EditProductWindow(selectedProduct);
                editWindow.ShowDialog();

                // Refresh the DataGrid after editing (you may need to implement a more efficient way to refresh)
                LoadData();
            }
            else
            {
                MessageBox.Show("Please select a product to edit.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {

        }

        // Add methods for other CRUD operations (Insert, Update, Delete) as needed
    }




    public class Product
    {
        public int ProductID { get; set; }
        public string Product_Name { get; set; }
        public decimal Price { get; set; }
        public string Policy { get; set; }
        public int Batch { get; set; }
        public DateTime ExpireDate { get; set; }
        public int Quality { get; set; }
    }

}
