using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace APproject
{
    public partial class accountingOP : UserControl
    {
        public ObservableCollection<AccountingItem> AccountingItems { get; set; }
        public Visibility DataLoadErrorVisibility { get; set; }
        public decimal TotalCredit { get; set; }
        public decimal TotalDebit { get; set; }

        public accountingOP()
        {
            InitializeComponent();
            TryLoadTableData();  // Attempt to load table data
        }

        private void TryLoadTableData()
        {
            try
            {
                // Your connection string
                string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Integrated Security=True";

                // SQL query to fetch data from the database
                string sql = "SELECT cid, amount, date, type FROM credit";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Initialize ObservableCollection for DataGrid
                            AccountingItems = new ObservableCollection<AccountingItem>();
                            TotalCredit = 0;
                            TotalDebit = 0;

                            while (reader.Read())
                            {
                                // Populate AccountingItem objects from the database
                                AccountingItem item = new AccountingItem
                                {
                                    TransactionID = Convert.ToInt32(reader["cid"]),
                                    Amount = Convert.ToDecimal(reader["amount"]),
                                    Date = Convert.ToDateTime(reader["date"]),
                                    TypeOfTransaction = reader["type"].ToString()
                                };

                                AccountingItems.Add(item);

                                // Update total credit and debit
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

                    // If data loading is successful, show the table
                    DataLoadErrorVisibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                // If there's an issue, show error messages and log the exception
                AccountingItems = new ObservableCollection<AccountingItem>();
                DataLoadErrorVisibility = Visibility.Visible;

                // Log or display the exception, for example:
                MessageBox.Show($"Error loading table data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public class AccountingItem
    {
        public int TransactionID { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string TypeOfTransaction { get; set; }
    }
}
