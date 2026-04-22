using System;
using System.Windows;
using AISPC.Models;
using AISPC.Data;
using AISPC.Services;

namespace AISPC.Views.Dialogs
{
    public partial class AddOrderDialog : Window
    {
        private readonly OrderService? _orderService;

        public string ClientName => ClientNameTextBox.Text.Trim();
        public decimal TotalAmount
        {
            get
            {
                if (decimal.TryParse(TotalAmountTextBox.Text.Trim(), out decimal amount))
                    return amount;
                return 0;
            }
        }
        public DateTime OrderDate => OrderDatePicker.SelectedDate ?? DateTime.Now;
        public string Notes => string.IsNullOrWhiteSpace(NotesTextBox.Text) ? "" : NotesTextBox.Text.Trim();
        public string Priority => (PriorityComboBox.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString() ?? "Обычный";

        public AddOrderDialog()
        {
            InitializeComponent();
            
            try
            {
                var context = new ApplicationDbContext();
                _orderService = new OrderService(context);
            }
            catch
            {
                // Если не удается подключиться к БД, работаем без сервиса
            }
            
            UpdatePreview();
            
            // Подписываемся на изменения для обновления превью
            ClientNameTextBox.TextChanged += (s, e) => UpdatePreview();
            TotalAmountTextBox.TextChanged += (s, e) => UpdatePreview();
            PriorityComboBox.SelectionChanged += (s, e) => UpdatePreview();
        }

        private void UpdatePreview()
        {
            try
            {
                var clientName = string.IsNullOrWhiteSpace(ClientNameTextBox.Text) ? "Не указан" : ClientNameTextBox.Text;
                var priority = (PriorityComboBox.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString() ?? "Обычный";
                var amount = decimal.TryParse(TotalAmountTextBox.Text, out var amt) ? amt.ToString("C") : "0 ₽";
                
                PreviewText.Text = $"Клиент: {clientName}\n" +
                                 $"Приоритет: {priority}\n" +
                                 $"Сумма: {amount}\n" +
                                 $"Статус: Новый";
            }
            catch
            {
                PreviewText.Text = "Ошибка в данных заказа";
            }
        }

        private async void CreateOrder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Валидация данных
                if (string.IsNullOrWhiteSpace(ClientNameTextBox.Text))
                {
                    Views.Dialogs.CustomMessageBox.Show("Введите название клиента", "Ошибка валидации", 
                                  MessageBoxImage.Warning, MessageBoxButton.OK, this);
                    ClientNameTextBox.Focus();
                    return;
                }

                if (!decimal.TryParse(TotalAmountTextBox.Text, out var totalAmount) || totalAmount <= 0)
                {
                    Views.Dialogs.CustomMessageBox.Show("Введите корректную сумму заказа", "Ошибка валидации", 
                                  MessageBoxImage.Warning, MessageBoxButton.OK, this);
                    TotalAmountTextBox.Focus();
                    return;
                }

                // Создаем новый заказ
                var order = new Order
                {
                    ClientName = ClientNameTextBox.Text.Trim(),
                    Status = "Новый",
                    TotalAmount = totalAmount,
                    OrderDate = OrderDatePicker.SelectedDate ?? DateTime.Now,
                    Priority = (PriorityComboBox.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString() ?? "Обычный",
                    Notes = string.IsNullOrWhiteSpace(NotesTextBox.Text) ? null : NotesTextBox.Text.Trim()
                };

                // Сохраняем в базу данных
                if (_orderService != null)
                {
                    await _orderService.CreateOrderAsync(order);
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                Views.Dialogs.CustomMessageBox.Show($"Ошибка при создании заказа: {ex.Message}", "Ошибка", 
                              MessageBoxImage.Error, MessageBoxButton.OK, this);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}