using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AISPC.Models;
using AISPC.Data;

namespace AISPC.Views.Dialogs
{
    public partial class ClientManagementDialog : Window
    {
        private readonly ApplicationDbContext? _context;
        private readonly Client? _client;
        private readonly bool _isEditMode;

        public string ClientName => NameTextBox.Text.Trim();
        public string ClientType => (ClientTypeComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "ООО";
        public string INN => GetTextBoxValue(INNTextBox, "Введите ИНН (необязательно)");
        public string Phone => GetTextBoxValue(PhoneTextBox, "Введите номер телефона");
        public string Email => GetTextBoxValue(EmailTextBox, "Введите email адрес");
        public string Address => GetTextBoxValue(AddressTextBox, "Введите адрес клиента");
        public string Notes => GetTextBoxValue(NotesTextBox, "Дополнительная информация о клиенте (необязательно)");
        public bool IsClientActive => IsActiveCheckBox.IsChecked ?? true;

        // Конструктор для добавления нового клиента
        public ClientManagementDialog()
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
            
            _isEditMode = false;
            TitleTextBlock.Text = "👤 Добавление клиента";
            SaveButton.Content = "💾 Добавить клиента";
            
            UpdatePreview();
            
            // Подписываемся на изменения для обновления превью
            NameTextBox.TextChanged += (s, e) => UpdatePreview();
            ClientTypeComboBox.SelectionChanged += (s, e) => UpdatePreview();
            PhoneTextBox.TextChanged += (s, e) => UpdatePreview();
            EmailTextBox.TextChanged += (s, e) => UpdatePreview();
        }

        // Конструктор для редактирования существующего клиента
        public ClientManagementDialog(Client client)
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
            
            _client = client;
            _isEditMode = true;
            TitleTextBlock.Text = "✏️ Редактирование клиента";
            SaveButton.Content = "💾 Сохранить изменения";
            
            LoadClientData();
            UpdatePreview();
            
            // Подписываемся на изменения для обновления превью
            NameTextBox.TextChanged += (s, e) => UpdatePreview();
            ClientTypeComboBox.SelectionChanged += (s, e) => UpdatePreview();
            PhoneTextBox.TextChanged += (s, e) => UpdatePreview();
            EmailTextBox.TextChanged += (s, e) => UpdatePreview();
        }

        private void LoadClientData()
        {
            if (_client != null)
            {
                NameTextBox.Text = _client.Name;
                
                // Устанавливаем тип клиента
                foreach (ComboBoxItem item in ClientTypeComboBox.Items)
                {
                    if (item.Content.ToString() == _client.ClientType)
                    {
                        ClientTypeComboBox.SelectedItem = item;
                        break;
                    }
                }
                
                INNTextBox.Text = _client.INN ?? "Введите ИНН (необязательно)";
                PhoneTextBox.Text = _client.Phone ?? "Введите номер телефона";
                EmailTextBox.Text = _client.Email ?? "Введите email адрес";
                AddressTextBox.Text = _client.Address ?? "Введите адрес клиента";
                NotesTextBox.Text = _client.Notes ?? "Дополнительная информация о клиенте (необязательно)";
                IsActiveCheckBox.IsChecked = _client.IsActive;
            }
        }

        private void UpdatePreview()
        {
            try
            {
                var name = string.IsNullOrWhiteSpace(NameTextBox.Text) || NameTextBox.Text == "Введите название компании или ФИО" ? "Не указано" : NameTextBox.Text;
                var clientType = (ClientTypeComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Не указан";
                var phone = GetTextBoxValue(PhoneTextBox, "Введите номер телефона");
                var email = GetTextBoxValue(EmailTextBox, "Введите email адрес");
                
                phone = string.IsNullOrEmpty(phone) ? "Не указан" : phone;
                email = string.IsNullOrEmpty(email) ? "Не указан" : email;
                
                PreviewText.Text = $"Название/ФИО: {name}\n" +
                                 $"Тип: {clientType}\n" +
                                 $"Телефон: {phone}\n" +
                                 $"Email: {email}\n" +
                                 $"Статус: {(IsClientActive ? "Активный" : "Неактивный")}";
            }
            catch
            {
                PreviewText.Text = "Ошибка в данных клиента";
            }
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
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

                if (_isEditMode && _client != null)
                {
                    // Обновляем существующего клиента
                    _client.Name = ClientName;
                    _client.ClientType = ClientType;
                    _client.INN = INN;
                    _client.Phone = Phone;
                    _client.Email = Email;
                    _client.Address = Address;
                    _client.Notes = Notes;
                    _client.IsActive = IsClientActive;
                    
                    if (_context != null)
                    {
                        await _context.SaveChangesAsync();
                    }
                }
                else
                {
                    // Создаем нового клиента
                    var client = new Client
                    {
                        Name = ClientName,
                        ClientType = ClientType,
                        INN = INN,
                        Phone = Phone,
                        Email = Email,
                        Address = Address,
                        Notes = Notes,
                        IsActive = IsClientActive,
                        CreatedDate = DateTime.Now
                    };

                    // Сохраняем в базу данных
                    if (_context != null)
                    {
                        _context.Clients.Add(client);
                        await _context.SaveChangesAsync();
                    }
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                Views.Dialogs.CustomMessageBox.Show($"Ошибка при сохранении клиента: {ex.Message}", "Ошибка", 
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
                textBox.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(201, 209, 217));
            }
        }

        public Client GetClient()
        {
            if (_isEditMode && _client != null)
            {
                return _client;
            }
            else
            {
                return new Client
                {
                    Name = ClientName,
                    ClientType = ClientType,
                    INN = INN,
                    Phone = Phone,
                    Email = Email,
                    Address = Address,
                    Notes = Notes,
                    IsActive = IsClientActive,
                    CreatedDate = DateTime.Now
                };
            }
        }
    }
}