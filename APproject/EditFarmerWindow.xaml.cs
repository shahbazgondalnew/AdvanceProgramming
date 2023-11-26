// EditFarmerWindow.xaml.cs
using System.Data.SqlClient;
using System;
using System.Windows;

namespace APproject
{
    public partial class EditFarmerWindow : Window
    {
        public int FarmerID { get; set; }
        public string Name { get; set; }
        public string Contact { get; set; }
        public string Address { get; set; }
        public decimal Balance { get; set; }

        private Farmer editedFarmer;

        public EditFarmerWindow(Farmer farmer)
        {
            InitializeComponent();
            editedFarmer = farmer;

            
            txtName.Text = editedFarmer.Name;
            txtBalance.Text = editedFarmer.Balance.ToString();
            txtContact.Text = editedFarmer.Contact;
            txtAddress.Text = editedFarmer.Address;
            FarmerID = editedFarmer.FarmerID;
        }

        private bool ValidateInput()
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Please enter a name.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtContact.Text))
            {
                MessageBox.Show("Please enter a contact number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtAddress.Text))
            {
                MessageBox.Show("Please enter an address.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Validate Balance is a valid decimal
            if (!decimal.TryParse(txtBalance.Text, out _))
            {
                MessageBox.Show("Please enter a valid balance.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            
            if (!ValidateInput())
            {
                return;
            }

           
            if (SaveFarmerDetails())
            {
                MessageBox.Show("Farmer details saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
              
                this.Close();

            }
            else
            {
                MessageBox.Show("Failed to save farmer details. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

       
        private bool SaveFarmerDetails()
            {
                try
                {
                    string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Integrated Security=True"; 
                using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                       
                        string updateQuery = "UPDATE Farmer SET Name = @Name, Contact = @Contact, Address = @Address, Balance = @Balance WHERE farmer_id = @FarmerID";

                        using (SqlCommand command = new SqlCommand(updateQuery, connection))
                        {
                           
                            command.Parameters.AddWithValue("@Name", txtName.Text);
                            command.Parameters.AddWithValue("@Contact", txtContact.Text);
                            command.Parameters.AddWithValue("@Address", txtAddress.Text);
                            command.Parameters.AddWithValue("@Balance", Convert.ToDecimal(txtBalance.Text));
                            command.Parameters.AddWithValue("@FarmerID", FarmerID);

                            int rowsAffected = command.ExecuteNonQuery();

                           
                            return rowsAffected > 0;
                        }
                    }
                }
                catch (Exception ex)
                {
                    
                    MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
        }
    }
