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
            // Add code for editing a farmer
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Add code for deleting a farmer
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
