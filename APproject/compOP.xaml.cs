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

    public partial class compOP : UserControl
    {
        private ICollectionView companiesView;
        private List<Company> allCompanies;
        private int itemsPerPage = 10;
        private int currentPage = 1;
        private string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Integrated Security=True";

        public compOP()
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

                    // Change the table name to 'Company'
                    string sql = "SELECT * FROM Company";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            allCompanies = new List<Company>();

                            while (reader.Read())
                            {
                                Company company = new Company
                                {
                                    CompanyID = Convert.ToInt32(reader["CompanyID"]),
                                    Name = Convert.ToString(reader["Name"]),
                                    Balance = Convert.ToDecimal(reader["Balance"]),
                                    ContactNo = Convert.ToString(reader["ContactNo"]),
                                    Address = Convert.ToString(reader["Address"])
                                };

                                allCompanies.Add(company);
                            }

                            // Create a ListCollectionView for sorting and filtering
                            companiesView = new ListCollectionView(allCompanies);

                            // Initialize dataGrid with all companies
                            dataGrid.ItemsSource = companiesView;
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
            companiesView = CollectionViewSource.GetDefaultView(dataGrid.ItemsSource);
            companiesView.Filter = item =>
            {
                int index = allCompanies.IndexOf((Company)item);
                return index >= (currentPage - 1) * itemsPerPage && index < currentPage * itemsPerPage;
            };
        }


        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (companiesView != null)
            {
                string searchText = searchTextBox.Text.ToLower();

                // Apply filter to the ListCollectionView
                companiesView.Filter = item =>
                {
                    if (item is Company company)
                    {
                        return company.Name.ToLower().Contains(searchText) ||
                               
                               company.Address.ToLower().Contains(searchText);
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
            companiesView.Refresh();
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
    if (companiesView == null)
    {
        return;
    }

    // Filter based on address equality to "Faisalabad"
    companiesView.Filter = item =>
    {
        if (item is Company company)
        {
            return company.Address.Equals("Faisalabad", StringComparison.OrdinalIgnoreCase);
        }
        return false;
    };
}


        private void SortDataGrid(string sortMemberPath)
        {
            if (companiesView != null)
            {
                companiesView.SortDescriptions.Clear();
                companiesView.SortDescriptions.Add(new SortDescription(sortMemberPath, ListSortDirection.Ascending));
                companiesView.Refresh();
            }
        }




        private void PreviousPage_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                companiesView.Refresh();
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

            // Access the data item (Company) associated with the clicked row
            Company selectedCompany = (Company)btn.Tag;

            if (selectedCompany != null)
            {
                // Open the EditCompanyWindow for editing the selected company
                EditCompany editWindow = new EditCompany(selectedCompany);
                editWindow.ShowDialog();

                // Refresh the DataGrid after editing
                LoadData();
                InitializePagination(); // Reset pagination after editing
            }
            else
            {
                MessageBox.Show("Please select a company to edit.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Handle menu item click event here
            // You can use the sender parameter to identify which menu item was clicked
        }
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Your delete button logic goes here
        }

        private void DeleteCompany(int companyId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string deleteSql = $"DELETE FROM Company WHERE CompanyID = @CompanyID";

                    using (SqlCommand deleteCommand = new SqlCommand(deleteSql, connection))
                    {
                        deleteCommand.Parameters.AddWithValue("@CompanyID", companyId);
                        deleteCommand.ExecuteNonQuery();
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting company: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        

    }
    public class Company
    {
        public int CompanyID { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
        public string ContactNo { get; set; }
        public string Address { get; set; }
    }





}
