using System;
using System.Data.SqlClient;
using System.Windows;

namespace APproject
{
    public partial class AddProductWindow : Window
    {
        private string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Integrated Security=True";

        public AddProductWindow()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (AreFieldsValid())
            {
                // Get values from the text boxes and date picker
                string productName = productNameTextBox.Text;
                decimal price = Convert.ToDecimal(priceTextBox.Text);
                string policy = policyTextBox.Text;
                int batch = Convert.ToInt32(batchTextBox.Text);
                DateTime expireDate = expireDatePicker.SelectedDate ?? DateTime.Now; // Use DateTime.Now if SelectedDate is null
                int quality = Convert.ToInt32(qualityTextBox.Text);

                // Perform the insert operation
                InsertProduct(productName, price, policy, batch, expireDate, quality);

                // Close the window after saving
                Close();
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

        private void InsertProduct(string productName, decimal price, string policy, int batch, DateTime expireDate, int quality)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Insert query
                    string insertSql = @"
                        INSERT INTO Product (Product_Name, Price, Policy, Batch, ExpireDate, Quality)
                        VALUES (@ProductName, @Price, @Policy, @Batch, @ExpireDate, @Quality);
                    ";

                    using (SqlCommand insertCommand = new SqlCommand(insertSql, connection))
                    {
                        // Set parameters
                        insertCommand.Parameters.AddWithValue("@ProductName", productName);
                        insertCommand.Parameters.AddWithValue("@Price", price);
                        insertCommand.Parameters.AddWithValue("@Policy", policy);
                        insertCommand.Parameters.AddWithValue("@Batch", batch);
                        insertCommand.Parameters.AddWithValue("@ExpireDate", expireDate);
                        insertCommand.Parameters.AddWithValue("@Quality", quality);

                        // Execute the insert command
                        insertCommand.ExecuteNonQuery();
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inserting product: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
