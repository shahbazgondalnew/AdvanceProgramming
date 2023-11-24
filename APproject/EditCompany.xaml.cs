using System.Windows;

namespace APproject
{
    public partial class EditCompany : Window
    {
        private Company companyToEdit;

        public EditCompany(Company selectedCompany)
        {
            InitializeComponent();
            companyToEdit = selectedCompany;

            // Populate the TextBox with the current company name
            companyNameTextBox.Text = companyToEdit.Name;
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            // Update the company name with the value from the TextBox
            companyToEdit.Name = companyNameTextBox.Text;

            // Perform any other necessary update operations

            // Close the window
            Close();
        }
    }
}
