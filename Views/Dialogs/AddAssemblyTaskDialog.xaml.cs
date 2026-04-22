using System;
using System.Windows;

namespace AISPC.Views.Dialogs
{
    public partial class AddAssemblyTaskDialog : Window
    {
        public int OrderId
        {
            get
            {
                if (int.TryParse(OrderIdTextBox.Text.Trim(), out int orderId))
                    return orderId;
                return 0;
            }
        }

        public int AssemblerId
        {
            get
            {
                if (int.TryParse(AssemblerIdTextBox.Text.Trim(), out int assemblerId))
                    return assemblerId;
                return 0;
            }
        }

        public string AssemblerName => AssemblerNameTextBox.Text.Trim();
        public DateTime StartDate => StartDatePicker.SelectedDate ?? DateTime.Now;
        public string Notes => NotesTextBox.Text.Trim();

        public AddAssemblyTaskDialog()
        {
            InitializeComponent();
            StartDatePicker.SelectedDate = DateTime.Now;
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
            if (OrderId <= 0)
            {
                Views.Dialogs.CustomMessageBox.Show("Введите корректный ID заказа", "Ошибка валидации", 
                              MessageBoxImage.Warning, MessageBoxButton.OK, this);
                OrderIdTextBox.Focus();
                return false;
            }

            if (AssemblerId <= 0)
            {
                Views.Dialogs.CustomMessageBox.Show("Введите корректный ID сборщика", "Ошибка валидации", 
                              MessageBoxImage.Warning, MessageBoxButton.OK, this);
                AssemblerIdTextBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(AssemblerName))
            {
                Views.Dialogs.CustomMessageBox.Show("Введите имя сборщика", "Ошибка валидации", 
                              MessageBoxImage.Warning, MessageBoxButton.OK, this);
                AssemblerNameTextBox.Focus();
                return false;
            }

            return true;
        }
    }
}