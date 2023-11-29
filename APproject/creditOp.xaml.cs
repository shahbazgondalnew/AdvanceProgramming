using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;

namespace APproject
{
    public partial class creditOP : UserControl
    {
        public SeriesCollection BarChartValues { get; set; }

        public creditOP()
        {
            InitializeComponent();
            DataContext = this;

            // Initialize SeriesCollection for the bar chart
            BarChartValues = new SeriesCollection();

            // Load data into the DataGrid and Bar Chart
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                // Connection string for your SQL Server
                string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Integrated Security=True";

                // SQL query to retrieve data for the Bar Chart
                string queryBarChart = "SELECT MONTH(Date) as Month, SUM(CASE WHEN Type = 'credit' THEN Amount ELSE 0 END) as CreditAmount, " +
                                       "SUM(CASE WHEN Type = 'debit' THEN Amount ELSE 0 END) as DebitAmount " +
                                       "FROM Credit GROUP BY MONTH(Date)";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(queryBarChart, connection))
                    {
                        SqlDataReader reader = command.ExecuteReader();

                        // Create a list to store the series
                        var seriesList = new SeriesCollection();

                        while (reader.Read())
                        {
                            int month = reader.GetInt32(0);
                            decimal creditAmount = reader.GetDecimal(1);
                            decimal debitAmount = reader.GetDecimal(2);

                            // Add data to the Bar Chart
                            seriesList.Add(new ColumnSeries
                            {
                                Title = $"Month {month}",
                                Values = new ChartValues<decimal> { creditAmount, debitAmount }
                            });
                        }

                        // Update UI controls on the UI thread
                        Dispatcher.Invoke(() =>
                        {
                            BarChartValues = seriesList;
                            barChart.Series = BarChartValues;
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void RemoveSeriesButton_Click(object sender, RoutedEventArgs e)
        {
            // Remove the last series from BarChartValues when the button is clicked
            if (BarChartValues.Count > 0)
            {
                BarChartValues.RemoveAt(BarChartValues.Count - 1);
                barChart.Series = BarChartValues; // Update the chart
            }
        }
    }
}
