using System;
using System.Data.SqlClient;
using System.Windows;

namespace APproject
{
    public partial class EditProductWindow : Window
    {
        // The product being edited
        private string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Integrated Security=True";

        public Product EditedProduct { get; set; }

        public EditProductWindow(Product product)
        {
            InitializeComponent();

            // Set the DataContext to the provided product for data binding
            EditedProduct = product;
            DataContext = EditedProduct;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (AreFieldsValid())
            {
                // Set up the SQL query for the update
                string updateSql = @"
            UPDATE Product
            SET
                Product_Name = @ProductName,
                Price = @Price,
                Policy = @Policy,
                Batch = @Batch,
                ExpireDate = @ExpireDate,
                Quality = @Quality
            WHERE ProductID = @ProductID;
        ";

                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        using (SqlCommand updateCommand = new SqlCommand(updateSql, connection))
                        {
                            // Set parameters
                            updateCommand.Parameters.AddWithValue("@ProductName", productNameTextBox.Text);
                            updateCommand.Parameters.AddWithValue("@Price", Convert.ToDecimal(priceTextBox.Text));
                            updateCommand.Parameters.AddWithValue("@Policy", policyTextBox.Text);
                            updateCommand.Parameters.AddWithValue("@Batch", Convert.ToInt32(batchTextBox.Text));
                            updateCommand.Parameters.AddWithValue("@ExpireDate", expireDatePicker.SelectedDate);
                            updateCommand.Parameters.AddWithValue("@Quality", Convert.ToInt32(qualityTextBox.Text));
                            updateCommand.Parameters.AddWithValue("@ProductID", EditedProduct.ProductID);

                            // Execute the update command
                            updateCommand.ExecuteNonQuery();
                        }

                        connection.Close();
                    }

                    // Close the window after saving
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating product: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please fill in all fields.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private bool AreFieldsValid()
        {
            // Check if all fields are filled
            return !string.IsNullOrEmpty(productNameTextBox.Text) &&
                   !string.IsNullOrEmpty(priceTextBox.Text) &&
                   !string.IsNullOrEmpty(policyTextBox.Text) &&
                   !string.IsNullOrEmpty(batchTextBox.Text) &&
                   expireDatePicker.SelectedDate.HasValue &&
                   !string.IsNullOrEmpty(qualityTextBox.Text);
        }

    }
}
