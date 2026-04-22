using System;
using System.Windows;
using System.Windows.Controls;

namespace AISPC.Views.Dialogs
{
    public partial class WarehouseMovementDialog : Window
    {
        public string FromLocation
        {
            get
            {
                if (FromLocationComboBox.SelectedItem is ComboBoxItem selectedItem)
                    return selectedItem.Content.ToString() ?? "";
                return "";
            }
        }

        public string ToLocation
        {
            get
            {
                if (ToLocationComboBox.SelectedItem is ComboBoxItem selectedItem)
                    return selectedItem.Content.ToString() ?? "";
                return "";
            }
        }

        public int Quantity
        {
            get
            {
                if (int.TryParse(QuantityTextBox.Text.Trim(), out int quantity))
                    return quantity;
                return 0;
            }
        }

        public WarehouseMovementDialog(string componentName)
        {
            InitializeComponent();
            ComponentNameTextBlock.Text = componentName;
            
            // Устанавливаем значения по умолчанию
            FromLocationComboBox.SelectedIndex = 0; // Основной склад
            ToLocationComboBox.SelectedIndex = 2;   // Зона сборки
        }

        private void Move_Click(object sender, RoutedEventArgs e)
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
            if (FromLocationComboBox.SelectedItem == null)
            {
                Views.Dialogs.CustomMessageBox.Show("Выберите исходное местоположение", "Ошибка валидации", 
                              MessageBoxImage.Warning, MessageBoxButton.OK, this);
                FromLocationComboBox.Focus();
                return false;
            }

            if (ToLocationComboBox.SelectedItem == null)
            {
                Views.Dialogs.CustomMessageBox.Show("Выберите целевое местоположение", "Ошибка валидации", 
                              MessageBoxImage.Warning, MessageBoxButton.OK, this);
                ToLocationComboBox.Focus();
                return false;
            }

            if (FromLocation == ToLocation)
            {
                Views.Dialogs.CustomMessageBox.Show("Исходное и целевое местоположение не могут быть одинаковыми", "Ошибка валидации", 
                              MessageBoxImage.Warning, MessageBoxButton.OK, this);
                ToLocationComboBox.Focus();
                return false;
            }

            if (Quantity <= 0)
            {
                Views.Dialogs.CustomMessageBox.Show("Введите корректное количество (больше 0)", "Ошибка валидации", 
                              MessageBoxImage.Warning, MessageBoxButton.OK, this);
                QuantityTextBox.Focus();
                return false;
            }

            return true;
        }
    }
}