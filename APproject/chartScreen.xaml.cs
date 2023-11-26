﻿using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Windows.Controls;
using System.Windows.Media;

namespace APproject
{
    public partial class chartScreen : UserControl
    {
        public chartScreen()
        {
            InitializeComponent();
            DataContext = new ChartViewModel();
        }
    }
    public class ChartViewModel
    {
        public SeriesCollection BarSeries { get; set; }
        public SeriesCollection PieSeries { get; set; }
        public ObservableCollection<string> Labels { get; set; }
        public Func<double, string> Formatter { get; set; }

        public ChartViewModel()
        {
           
            InitializeData();
        }

        private void InitializeData()
        {
            try
            {
                
                string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Integrated Security=True";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    
                    string sql = "SELECT ExpireDate, Quality FROM Product";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            Labels = new ObservableCollection<string>();
                            ChartValues<ObservableValue> barValues = new ChartValues<ObservableValue>();
                            ChartValues<ObservableValue> pieValues = new ChartValues<ObservableValue>();

                            
                            Dictionary<string, int> quantityByMonth = new Dictionary<string, int>();

                            while (reader.Read())
                            {
                                DateTime expireDate = Convert.ToDateTime(reader["ExpireDate"]);
                                int quantity = Convert.ToInt32(reader["Quality"]);

                              
                                string monthLabel = expireDate.ToString("MMMM");

                                if (quantityByMonth.ContainsKey(monthLabel))
                                {
                                    quantityByMonth[monthLabel] += quantity;
                                }
                                else
                                {
                                    quantityByMonth[monthLabel] = quantity;
                                }
                            }

                           
                            List<string> sliceColors = new List<string>
                            {
                                "#FF4500", // OrangeRed
                                "#32CD32", // LimeGreen
                                "#1E90FF", // DodgerBlue
                                "#FFD700", // Gold
                                "#8A2BE2", // BlueViolet
                                "#FF6347", // Tomato
                                "#008080", // Teal
                                "#FFA500", // Orange
                                "#00CED1", // DarkTurquoise
                                "#9370DB", // MediumPurple
                                "#40E0D0", // Turquoise
                                "#FF69B4"  // HotPink
                            };

                            
                            foreach (var kvp in quantityByMonth)
                            {
                                Labels.Add(kvp.Key);
                                barValues.Add(new ObservableValue(kvp.Value));
                                pieValues.Add(new ObservableValue(kvp.Value));
                            }

                            BarSeries = new SeriesCollection
                            {
                                new ColumnSeries
                                {
                                    Title = "Expiry",
                                    Values = barValues
                                }
                            };

                            PieSeries = new SeriesCollection();

                          
                            for (int i = 0; i < Labels.Count; i++)
                            {
                                PieSeries.Add(new PieSeries
                                {
                                    Title = Labels[i], 
                                    Values = new ChartValues<ObservableValue> { new ObservableValue(pieValues[i].Value) },
                                    Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(sliceColors[i])),
                                    DataLabels = true,
                                });
                            }
                        }
                    }

                    connection.Close();
                }

                Formatter = value => value.ToString("N");
            }
            catch (Exception ex)
            {
               
                Console.WriteLine($"Error fetching data: {ex.Message}");
            }
        }
    }
}