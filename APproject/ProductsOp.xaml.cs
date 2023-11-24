using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace APproject
{

    public partial class ProductsOp : UserControl
    {
        private ICollectionView productsView;
        private List<Product> allProducts;
        private int itemsPerPage = 10;
        private int currentPage = 1;
        private string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Integrated Security=True";

        public ProductsOp()
        {
            InitializeComponent();

            // Load data when the control is initialized
            LoadData();

            // Initialize pagination
            InitializePagination();
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
                            allProducts = new List<Product>();

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

                                allProducts.Add(product);
                            }

                            // Initialize dataGrid with all products
                            dataGrid.ItemsSource = allProducts;

                            // Set up the CollectionView for filtering
                            productsView = CollectionViewSource.GetDefaultView(allProducts);
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

        private void InitializePagination()
        {
            productsView = CollectionViewSource.GetDefaultView(dataGrid.ItemsSource);
            productsView.Filter = item =>
            {
                int index = allProducts.IndexOf((Product)item);
                return index >= (currentPage - 1) * itemsPerPage && index < currentPage * itemsPerPage;
            };
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (productsView != null)
            {
                string searchText = searchTextBox.Text.ToLower();

                productsView.Filter = item =>
                {
                    if (item is Product product)
                    {
                        return product.Product_Name.ToLower().Contains(searchText) ||
                               product.Policy.ToLower().Contains(searchText);
                    }
                    return false;
                };
            }
        }

        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            currentPage++;
            productsView.Refresh();
        }

        private void PreviousPage_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                productsView.Refresh();
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            // Open a new window for adding a new product
            AddProductWindow addWindow = new AddProductWindow();
            addWindow.ShowDialog();

            // Refresh the DataGrid after adding a new product
            LoadData();
            InitializePagination(); // Reset pagination after adding a new item
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

                // Refresh the DataGrid after editing
                LoadData();
                InitializePagination(); // Reset pagination after editing
            }
            else
            {
                MessageBox.Show("Please select a product to edit.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Get the selected product from the DataGrid
            Product selectedProduct = (Product)dataGrid.SelectedItem;

            if (selectedProduct != null)
            {
                // Show a confirmation dialog before proceeding with deletion
                MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete product with ID {selectedProduct.ProductID}?",
                                                          "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    // Perform the delete operation
                    DeleteProduct(selectedProduct.ProductID);

                    // Refresh the DataGrid after deletion
                    LoadData();
                    InitializePagination(); // Reset pagination after deletion
                }
            }
            else
            {
                MessageBox.Show("Please select a product to delete.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteProduct(int productId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string deleteSql = $"DELETE FROM Product WHERE ProductID = @ProductID";

                    using (SqlCommand deleteCommand = new SqlCommand(deleteSql, connection))
                    {
                        deleteCommand.Parameters.AddWithValue("@ProductID", productId);
                        deleteCommand.ExecuteNonQuery();
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting product: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
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
