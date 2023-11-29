using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Helpers;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace APproject
{
    public partial class accountingOP : UserControl
    {
        public SeriesCollection BarChartValues { get; set; }
        public ObservableCollection<string> Labels { get; set; }
        public ObservableCollection<AccountingTransaction> Transactions { get; set; }

        public accountingOP()
        {
            InitializeComponent();
            DataContext = new AccountingViewModel();
        }
    }

    public class AccountingTransaction
    {
        public int TransactionID { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        // Add more properties as needed
    }

    public class AccountingViewModel
    {
        public SeriesCollection BarChartValues { get; set; }
        public ObservableCollection<string> Labels { get; set; }
        public ObservableCollection<AccountingTransaction> Transactions { get; set; }

        public AccountingViewModel()
        {
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Integrated Security=True";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string queryBarChart = "SELECT cid, Amount, Date,typeOfTransaction FROM credit";

                    using (SqlCommand command = new SqlCommand(queryBarChart, connection))
                    {
                        SqlDataReader reader = command.ExecuteReader();

                        ObservableCollection<ObservableValue> creditValues = new ObservableCollection<ObservableValue>();
                        ObservableCollection<ObservableValue> debitValues = new ObservableCollection<ObservableValue>();
                        ObservableCollection<string> labels = new ObservableCollection<string>();
                        ObservableCollection<AccountingTransaction> transactions = new ObservableCollection<AccountingTransaction>();

                        while (reader.Read())
                        {
                            int transactionID = reader.GetInt32(0);
                            decimal amount = reader.GetDecimal(1);
                            DateTime date = reader.GetDateTime(2);

                            transactions.Add(new AccountingTransaction
                            {
                                TransactionID = transactionID,
                                Amount = amount,
                                Date = date
                            });

                            // Assuming 'Type' is a column in your database indicating Credit or Debit
                            string type = reader["typeOfTransaction"].ToString();
                            Console.WriteLine(type);

                            if (type == "credit")

                            {

                                creditValues.Add(new ObservableValue((double)amount));
                            }
                            else if (type == "debit")
                            {
                                debitValues.Add(new ObservableValue((double)amount));

                            }
                        }

                        SeriesCollection barChartValues = new SeriesCollection
                        {
                            new ColumnSeries
                            {
                                Title = "Credit",
                                Values = creditValues.AsChartValues()  // Explicit conversion here
                            },
                            new ColumnSeries
                            {
                                Title = "Debit",
                                Values = debitValues.AsChartValues()   // Explicit conversion here
                            }
                        };

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            BarChartValues = barChartValues;
                            Labels = labels;
                            Transactions = transactions;
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
