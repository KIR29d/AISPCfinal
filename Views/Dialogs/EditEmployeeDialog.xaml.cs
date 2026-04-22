using System;
using System.Windows;
using AISPC.Models;

namespace AISPC.Views.Dialogs
{
    public partial class EditEmployeeDialog : Window
    {
        private readonly Employee _employee;

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
        public bool IsEmployeeActive => IsActiveCheckBox.IsChecked ?? true;

        public EditEmployeeDialog(Employee employee)
        {
            InitializeComponent();
            _employee = employee;
            LoadEmployeeData();
        }

        private void LoadEmployeeData()
        {
            FullNameTextBox.Text = _employee.FullName;
            PositionTextBox.Text = _employee.Position;
            PhoneTextBox.Text = _employee.Phone;
            EmailTextBox.Text = _employee.Email;
            HireDatePicker.SelectedDate = _employee.HireDate;
            SalaryTextBox.Text = _employee.Salary.ToString();
            IsActiveCheckBox.IsChecked = _employee.IsActive;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
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
                MessageBox.Show("Введите ФИО сотрудника", "Ошибка валидации", 
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                FullNameTextBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(Position))
            {
                MessageBox.Show("Введите должность", "Ошибка валидации", 
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                PositionTextBox.Focus();
                return false;
            }

            if (Salary <= 0)
            {
                MessageBox.Show("Введите корректную зарплату", "Ошибка валидации", 
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                SalaryTextBox.Focus();
                return false;
            }

            return true;
        }
    }
}