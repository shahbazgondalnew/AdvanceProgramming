using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
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
