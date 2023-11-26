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

           
            companyNameTextBox.Text = companyToEdit.Name;
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
           
            companyToEdit.Name = companyNameTextBox.Text;

           
            Close();
        }
    }
}
