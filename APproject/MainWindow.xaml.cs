using System;
using System.Data.SqlClient;
using System.Windows;


using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace APproject
{
    public partial class MainWindow : Window
    {
        private string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Integrated Security=True";


        public MainWindow()
        {
            InitializeComponent();
        }
        private void txtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            // Your logic for password changed event
        }

        private void txtEmail_TextChanged(object sender, RoutedEventArgs e)
        {
            // Your logic for text changed event
        }

        private void textEmail_MouseDown(object sender, RoutedEventArgs e)
        {
            // Your logic for mouse down event
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
                connection.Close();
            }
        }
    }
}
