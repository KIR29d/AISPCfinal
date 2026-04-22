using System;
using System.Windows;

namespace AISPC.Views.Dialogs
{
    public partial class StockMovementDialog : Window
    {
        public int Quantity
        {
            get
            {
                if (int.TryParse(QuantityTextBox.Text.Trim(), out int quantity))
                    return quantity;
                return 0;
            }
        }

        public StockMovementDialog(string operationType, string componentName)
        {
            InitializeComponent();
            
            TitleTextBlock.Text = $"📦 {operationType} товара";
            ComponentNameTextBlock.Text = componentName;
            ConfirmButton.Content = operationType;
            
            Title = operationType;
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
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
            if (Quantity <= 0)
            {
                MessageBox.Show("Введите корректное количество (больше 0)", "Ошибка валидации", 
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                QuantityTextBox.Focus();
                return false;
            }

            return true;
        }
    }
}