using System;
using System.Windows;

namespace AISPC.Views.Dialogs
{
    public partial class AddEmployeeDialog : Window
    {
        public string FullName => FullNameTextBox.Text.Trim();
        public string Position => PositionTextBox.Text.Trim();
        public string Phone => PhoneTextBox.Text.Trim();
        public string Email => EmailTextBox.Text.Trim();
        public DateTime HireDate => HireDatePicker.SelectedDate ?? DateTime.Now;
        public decimal Salary
        {
            get
            {
                if (decimal.TryParse(SalaryTextBox.Text.Trim(), out decimal salary))
                    return salary;
                return 0;
            }
        }

        public AddEmployeeDialog()
        {
            InitializeComponent();
            HireDatePicker.SelectedDate = DateTime.Now;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInput())
            {
                DialogResult = true;
                Close();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(FullName))
            {
                Views.Dialogs.CustomMessageBox.Show("Введите ФИО сотрудника", "Ошибка валидации", 
                              MessageBoxImage.Warning, MessageBoxButton.OK, this);
                FullNameTextBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(Position))
            {
                Views.Dialogs.CustomMessageBox.Show("Введите должность", "Ошибка валидации", 
                              MessageBoxImage.Warning, MessageBoxButton.OK, this);
                PositionTextBox.Focus();
                return false;
            }

            if (Salary <= 0)
            {
                Views.Dialogs.CustomMessageBox.Show("Введите корректную зарплату", "Ошибка валидации", 
                              MessageBoxImage.Warning, MessageBoxButton.OK, this);
                SalaryTextBox.Focus();
                return false;
            }

            return true;
        }
    }
}