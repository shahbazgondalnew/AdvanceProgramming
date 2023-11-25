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
    public partial class FarmerOp : UserControl
    {
        private ICollectionView farmersView;
        private List<Farmer> allFarmers;
        private int itemsPerPage = 10;
        private int currentPage = 1;
        private string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Integrated Security=True";

        public FarmerOp()
        {
            InitializeComponent();
            ComboBoxItem defaultItem = sortComboBox.Items.OfType<ComboBoxItem>().FirstOrDefault(item => item.Tag?.ToString() == "Balance");

            if (defaultItem != null)
            {
                sortComboBox.SelectedItem = defaultItem;
            }

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

                    string sql = "SELECT * FROM Farmer"; // Assuming your table is named 'Farmer'

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            allFarmers = new List<Farmer>();

                            while (reader.Read())
                            {
                                Farmer farmer = new Farmer
                                {
                                    FarmerID = Convert.ToInt32(reader["farmer_id"]),
                                    Balance = Convert.ToDecimal(reader["balance"]),
                                    Contact = Convert.ToString(reader["contact"]),
                                    Address = Convert.ToString(reader["address"]),
                                    Name = Convert.ToString(reader["name"])
                                    // Add other properties based on your Farmer class
                                };


                                allFarmers.Add(farmer);
                            }

                            // Create a ListCollectionView for sorting and filtering
                            farmersView = new ListCollectionView(allFarmers);

                            // Initialize dataGrid with all farmers
                            dataGrid.ItemsSource = farmersView;
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
            farmersView = CollectionViewSource.GetDefaultView(dataGrid.ItemsSource);
            farmersView.Filter = item =>
            {
                int index = allFarmers.IndexOf((Farmer)item);
                return index >= (currentPage - 1) * itemsPerPage && index < currentPage * itemsPerPage;
            };
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (farmersView != null)
            {
                string searchText = searchTextBox.Text.ToLower();

                // Apply filter to the ListCollectionView
                farmersView.Filter = item =>
                {
                    if (item is Farmer farmer)
                    {
                        // Adjust the properties based on your Farmer class
                        return farmer.Name.ToString().Contains(searchText) ||
                               farmer.Address
                               .ToLower().Contains(searchText);
                    }
                    return false;
                };
            }
        }

        private void DataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            // Prevent automatic sorting to handle it manually
            e.Handled = true;

            // Get the column being sorted
            DataGridColumn column = e.Column;

            // Determine the sort direction
            ListSortDirection direction = (column.SortDirection != ListSortDirection.Ascending)
                ? ListSortDirection.Ascending
                : ListSortDirection.Descending;

            // Clear previous sorting
            dataGrid.Items.SortDescriptions.Clear();

            // Apply the new sorting
            dataGrid.Items.SortDescriptions.Add(new SortDescription(column.SortMemberPath, direction));

            // Set the sort direction on the column header
            column.SortDirection = direction;
        }

        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            currentPage++;
            farmersView.Refresh();
        }

        private void PreviousPage_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                farmersView.Refresh();
            }
        }

        private void SortButton_Click(object sender, RoutedEventArgs e)
        {
            if (sortComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string sortMemberPath = selectedItem.Tag.ToString();
                SortDataGrid(sortMemberPath);
            }
            else
            {
                MessageBox.Show("No item selected in the ComboBox.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FilterByButton_Click(object sender, RoutedEventArgs e)
        {
            // Add code for filtering based on specific criteria
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            // Add code for adding a new farmer
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            // Assuming you have a way to get the selected farmer (replace this with your logic)
            Farmer selectedFarmer = GetSelectedFarmer();

            // Check if a farmer is selected
            if (selectedFarmer != null)
            {
                // Open the EditFarmerWindow with the selected farmer
                EditFarmerWindow editF = new EditFarmerWindow(selectedFarmer);
                editF.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please select a farmer to edit.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Replace this method with your logic to get the selected farmer
        private Farmer GetSelectedFarmer()
        {
            // Implement your logic to get the selected farmer, for example, from the DataGrid
            // Return the selected farmer or null if none is selected
            return dataGrid.SelectedItem as Farmer;
        }


        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Check if an item is selected in the DataGrid
            if (dataGrid.SelectedItem != null)
            {
                // Confirm the deletion (optional)
                MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this farmer?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Get the selected farmer from the DataGrid
                    Farmer selectedFarmer = (Farmer)dataGrid.SelectedItem;

                    // Assuming you have a database connection (replace connectionString with your actual connection string)
                   
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        // SQL DELETE statement
                        string deleteQuery = "DELETE FROM farmer WHERE farmer_id = @FarmerID";

                        using (SqlCommand command = new SqlCommand(deleteQuery, connection))
                        {
                            // Replace FarmerID with the actual ID property of your Farmer class
                            command.Parameters.AddWithValue("@FarmerID", selectedFarmer.FarmerID);

                            // Execute the DELETE statement
                            command.ExecuteNonQuery();
                        }
                    }

                    // Refresh the DataGrid
                    LoadData();// Assuming LoadFarmers is a method to reload the farmers from the database
                }
            }
            else
            {
                // Inform the user that no farmer is selected
                MessageBox.Show("Please select a farmer to delete.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


        private void SortDataGrid(string sortMemberPath)
        {
            if (farmersView != null)
            {
                farmersView.SortDescriptions.Clear();
                farmersView.SortDescriptions.Add(new SortDescription(sortMemberPath, ListSortDirection.Ascending));
                farmersView.Refresh();
            }
        }
    }

    public class Farmer
    {
        public int FarmerID { get; set; }
        public decimal Balance { get; set; }
        public string Contact { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }

        
    }

}
