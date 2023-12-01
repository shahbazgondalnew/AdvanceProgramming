using LiveCharts.Wpf;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace APproject
{
    public partial class accountingOP : UserControl, INotifyPropertyChanged
    {
        public ObservableCollection<AccountingItem> AccountingItems { get; set; }
        public Visibility DataLoadErrorVisibility { get; set; }

        private decimal totalCredit;
        public decimal TotalCredit
        {
            get { return totalCredit; }
            set
            {
                if (totalCredit != value)
                {
                    totalCredit = value;
                    OnPropertyChanged(nameof(TotalCredit));
                }
            }
        }

        private decimal totalDebit;
        public decimal TotalDebit
        {
            get { return totalDebit; }
            set
            {
                if (totalDebit != value)
                {
                    totalDebit = value;
                    OnPropertyChanged(nameof(TotalDebit));
                }
            }
        }

        public accountingOP()
        {
            InitializeComponent();
            DataContext = this;
            TryLoadTableData();  // Attempt to load table data
            GenerateRandomChartData();
        }
        private SeriesCollection barSeries;
        public SeriesCollection BarSeries
        {
            get { return barSeries; }
            set
            {
                if (barSeries != value)
                {
                    barSeries = value;
                    OnPropertyChanged(nameof(BarSeries));
                }
            }
        }

        private List<string> barLabels;
        public List<string> BarLabels
        {
            get { return barLabels; }
            set
            {
                if (barLabels != value)
                {
                    barLabels = value;
                    OnPropertyChanged(nameof(BarLabels));
                }
            }
        }
        private void GenerateRandomChartData()
        {
            Random random = new Random();
            var months = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

            var barLabels = new List<string>();
            var creditData = new List<double>();
            var debitData = new List<double>();

            foreach (var month in months)
            {
                barLabels.Add(month);

                // Check if there is a search query
                string searchText = searchTextBox.Text.ToLower();
                var filteredItems = AccountingItems
                    .Where(item => item.TypeName.ToLower().Contains(searchText) && item.Date.ToString("MMM") == month)
                    .ToList();

                // If search results are found for the specific month, generate data for search results
                if (filteredItems.Any())
                {
                    creditData.Add((double)filteredItems.Where(item => item.TypeOfTransaction == "credit").Sum(item => item.Amount));
                    debitData.Add((double)filteredItems.Where(item => item.TypeOfTransaction == "debit").Sum(item => item.Amount));
                }
                else
                {
                    // If no search results or search is empty for the specific month, generate random data
                    creditData.Add(random.Next(1, 100)); // Replace with your actual credit data logic
                    debitData.Add(random.Next(1, 100));  // Replace with your actual debit data logic
                }
            }

            // Update the chart data
            BarLabels = barLabels;
            BarSeries = new SeriesCollection
    {
        new ColumnSeries { Title = "Credit", Values = new ChartValues<double>(creditData) },
        new ColumnSeries { Title = "Debit", Values = new ChartValues<double>(debitData) }
    };
        }








        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Filter the items based on the search text
            string searchText = searchTextBox.Text.ToLower();
            var filteredItems = AccountingItems
                .Where(item => item.TypeName.ToLower().Contains(searchText))
                .ToList();

            // Update credit and debit totals based on the filtered items
            TotalCredit = filteredItems.Where(item => item.TypeOfTransaction == "credit").Sum(item => item.Amount);
            TotalDebit = filteredItems.Where(item => item.TypeOfTransaction == "debit").Sum(item => item.Amount);

            // Update the DataGrid with the filtered items
            accountingDataGrid.ItemsSource = filteredItems;
        }
       
       

        private void TryLoadTableData()
        {
            try
            {
                string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Integrated Security=True";
                string sql = "SELECT c.cid, c.amount, c.date,c.type, c.typeOfTransaction, c.id, " +
                             "CASE WHEN c.typeOfTransaction = 'farmer' THEN f.name " +
                             "WHEN c.typeOfTransaction = 'company' THEN com.name END AS TypeName " +
                             "FROM credit c " +
                             "LEFT JOIN farmer f ON c.id = f.farmer_id " +
                             "LEFT JOIN company com ON c.id = com.companyID";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            AccountingItems = new ObservableCollection<AccountingItem>();
                            TotalCredit = 0;
                            TotalDebit = 0;

                            while (reader.Read())
                            {
                                AccountingItem item = new AccountingItem
                                {
                                    TransactionID = Convert.ToInt32(reader["cid"]),
                                    Amount = Convert.ToDecimal(reader["amount"]),
                                    Date = Convert.ToDateTime(reader["date"]),
                                    TypeOfTransaction = reader["type"].ToString(),
                                    TypeName = reader["TypeName"].ToString()
                                };

                                AccountingItems.Add(item);

                                if (item.TypeOfTransaction == "credit")
                                {
                                    TotalCredit += item.Amount;
                                }
                                else if (item.TypeOfTransaction == "debit")
                                {
                                    TotalDebit += item.Amount;
                                }
                            }
                        }
                    }

                    // Set the DataContext to this instance
                    DataContext = this;
                    DataLoadErrorVisibility = Visibility.Collapsed;

                    // Bind the data to the DataGrid
                    accountingDataGrid.ItemsSource = AccountingItems;
                    
                }
            }
            catch (Exception ex)
            {
                AccountingItems = new ObservableCollection<AccountingItem>();
                DataLoadErrorVisibility = Visibility.Visible;

                MessageBox.Show($"Error loading table data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Add this method to raise property changed event
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class AccountingItem
    {
        public int TransactionID { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string TypeOfTransaction { get; set; }
        public string TypeName { get; set; }
    }
}
