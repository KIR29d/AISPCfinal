using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AISPC.Models;
using AISPC.Data;

namespace AISPC.Views.Dialogs
{
    public partial class AddClientDialog : Window
    {
        private readonly ApplicationDbContext? _context;

        public AddClientDialog()
        {
            InitializeComponent();
            
            try
            {
                _context = new ApplicationDbContext();
            }
            catch
            {
                // Если не удается подключиться к БД, работаем без контекста
            }
            
            UpdatePreview();
            
            // Подписываемся на изменения для обновления превью
            NameTextBox.TextChanged += (s, e) => UpdatePreview();
            ClientTypeComboBox.SelectionChanged += (s, e) => UpdatePreview();
            PhoneTextBox.TextChanged += (s, e) => UpdatePreview();
            EmailTextBox.TextChanged += (s, e) => UpdatePreview();
        }

        private void UpdatePreview()
        {
            try
            {
                var name = string.IsNullOrWhiteSpace(NameTextBox.Text) ? "Не указано" : NameTextBox.Text;
                var clientType = (ClientTypeComboBox.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString() ?? "Не указан";
                var phone = string.IsNullOrWhiteSpace(PhoneTextBox.Text) ? "Не указан" : PhoneTextBox.Text;
                var email = string.IsNullOrWhiteSpace(EmailTextBox.Text) ? "Не указан" : EmailTextBox.Text;
                
                PreviewText.Text = $"Название/ФИО: {name}\n" +
                                 $"Тип: {clientType}\n" +
                                 $"Телефон: {phone}\n" +
                                 $"Email: {email}";
            }
            catch
            {
                PreviewText.Text = "Ошибка в данных клиента";
            }
        }

        private async void AddClient_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Валидация данных
                if (string.IsNullOrWhiteSpace(NameTextBox.Text) || NameTextBox.Text == "Введите название компании или ФИО")
                {
                    Views.Dialogs.CustomMessageBox.Show("Введите название или ФИО клиента", "Ошибка валидации", 
                                  MessageBoxImage.Warning, MessageBoxButton.OK, this);
                    NameTextBox.Focus();
                    return;
                }

                if (ClientTypeComboBox.SelectedItem == null)
                {
                    Views.Dialogs.CustomMessageBox.Show("Выберите тип клиента", "Ошибка валидации", 
                                  MessageBoxImage.Warning, MessageBoxButton.OK, this);
                    ClientTypeComboBox.Focus();
                    return;
                }

                // Создаем нового клиента
                var client = new Client
                {
                    Name = NameTextBox.Text.Trim(),
                    ClientType = (ClientTypeComboBox.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString() ?? "ООО",
                    Phone = GetTextBoxValue(PhoneTextBox, "Введите номер телефона"),
                    Email = GetTextBoxValue(EmailTextBox, "Введите email адрес"),
                    Address = GetTextBoxValue(AddressTextBox, "Введите адрес клиента"),
                    INN = GetTextBoxValue(INNTextBox, "Введите ИНН (необязательно)"),
                    Notes = GetTextBoxValue(NotesTextBox, "Дополнительная информация о клиенте (необязательно)"),
                    IsActive = true,
                    CreatedDate = DateTime.Now
                };

                // Сохраняем в базу данных
                if (_context != null)
                {
                    _context.Clients.Add(client);
                    await _context.SaveChangesAsync();
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                Views.Dialogs.CustomMessageBox.Show($"Ошибка при добавлении клиента: {ex.Message}", "Ошибка", 
                              MessageBoxImage.Error, MessageBoxButton.OK, this);
            }
        }

        private string GetTextBoxValue(TextBox textBox, string placeholder)
        {
            return string.IsNullOrWhiteSpace(textBox.Text) || textBox.Text == placeholder ? "" : textBox.Text.Trim();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            _context?.Dispose();
            base.OnClosed(e);
        }

        // Обработчики для placeholder'ов
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                var placeholders = new[]
                {
                    "Введите название компании или ФИО",
                    "Введите ИНН (необязательно)",
                    "Введите номер телефона",
                    "Введите email адрес",
                    "Введите адрес клиента",
                    "Дополнительная информация о клиенте (необязательно)"
                };

                if (placeholders.Contains(textBox.Text))
                {
                    textBox.Text = "";
                    textBox.Foreground = System.Windows.Media.Brushes.White;
                }
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null && string.IsNullOrWhiteSpace(textBox.Text))
            {
                var placeholder = "";
                if (textBox == NameTextBox) placeholder = "Введите название компании или ФИО";
                else if (textBox == INNTextBox) placeholder = "Введите ИНН (необязательно)";
                else if (textBox == PhoneTextBox) placeholder = "Введите номер телефона";
                else if (textBox == EmailTextBox) placeholder = "Введите email адрес";
                else if (textBox == AddressTextBox) placeholder = "Введите адрес клиента";
                else if (textBox == NotesTextBox) placeholder = "Дополнительная информация о клиенте (необязательно)";

                textBox.Text = placeholder;
                textBox.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(139, 148, 158));
            }
        }
    }
}