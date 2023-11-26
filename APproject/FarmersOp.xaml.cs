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

           
            LoadData();

            
            InitializePagination();
        }

        private void LoadData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "SELECT * FROM Farmer"; 

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
                                  
                                };


                                allFarmers.Add(farmer);
                            }

                            farmersView = new ListCollectionView(allFarmers);

                            
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

               
                farmersView.Filter = item =>
                {
                    if (item is Farmer farmer)
                    {
                       
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
            
            e.Handled = true;

            
            DataGridColumn column = e.Column;

            
            ListSortDirection direction = (column.SortDirection != ListSortDirection.Ascending)
                ? ListSortDirection.Ascending
                : ListSortDirection.Descending;

           
            dataGrid.Items.SortDescriptions.Clear();

           
            dataGrid.Items.SortDescriptions.Add(new SortDescription(column.SortMemberPath, direction));

           
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
           
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            
            Farmer selectedFarmer = GetSelectedFarmer();

            
            if (selectedFarmer != null)
            {
               
                EditFarmerWindow editF = new EditFarmerWindow(selectedFarmer);
                editF.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please select a farmer to edit.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        
        private Farmer GetSelectedFarmer()
        {
           
            return dataGrid.SelectedItem as Farmer;
        }


        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            
            if (dataGrid.SelectedItem != null)
            {
               
                MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this farmer?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                   
                    Farmer selectedFarmer = (Farmer)dataGrid.SelectedItem;

                   
                   
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                       
                        string deleteQuery = "DELETE FROM farmer WHERE farmer_id = @FarmerID";

                        using (SqlCommand command = new SqlCommand(deleteQuery, connection))
                        {
                            
                            command.Parameters.AddWithValue("@FarmerID", selectedFarmer.FarmerID);

                            
                            command.ExecuteNonQuery();
                        }
                    }

                   
                    LoadData();
                }
            }
            else
            {
               
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
