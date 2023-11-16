using System;
using System.Data.SqlClient;
using System.Windows;

namespace APproject
{
    public partial class MainWindow : Window
    {
        private string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Integrated Security=True";


        public MainWindow()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Password;

            // Check user credentials against the database
            if (AuthenticateUser(username, password))
            {
                MessageBox.Show("Login successful!");
                mainScreen mainMenu = new mainScreen();
                mainMenu.Show();
                this.Close();

            }
            else
            {
                MessageBox.Show("Login failed. Please check your username and password.");
            }
        }

        private bool AuthenticateUser(string username, string password)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT COUNT(*) FROM userauth WHERE email = @username AND password = @password";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@password", password);

                    int count = (int)command.ExecuteScalar();

                    return count > 0;
                }
            }
        }
    }
}
