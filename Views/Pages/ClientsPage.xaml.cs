using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AISPC.Models;

namespace AISPC.Views.Pages
{
    public partial class ClientsPage : Page
    {
        private List<Client> _allClients;

        public ClientsPage()
        {
            InitializeComponent();
            LoadClients();
        }

        private void LoadClients()
        {
            // Тестовые данные клиентов
            _allClients = new List<Client>
            {
                new Client { Id = 1, Name = "ООО Техносфера", ClientType = "ООО", Phone = "+7 (495) 123-45-67", Email = "info@technosfera.ru", Address = "г. Москва, ул. Ленина, д. 10", INN = "7701234567", Notes = "Постоянный клиент", CreatedDate = DateTime.Now.AddMonths(-6) },
                new Client { Id = 2, Name = "Иванов Иван Иванович", ClientType = "Физическое лицо", Phone = "+7 (903) 123-45-67", Email = "ivanov@mail.ru", Address = "г. Москва, ул. Пушкина, д. 5, кв. 10", Notes = "Игровые конфигурации", CreatedDate = DateTime.Now.AddMonths(-3) },
                new Client { Id = 3, Name = "ИП Петров Петр Петрович", ClientType = "ИП", Phone = "+7 (916) 987-65-43", Email = "petrov.ip@gmail.com", Address = "г. Москва, пр-т Мира, д. 15", INN = "123456789012", Notes = "Офисная техника", CreatedDate = DateTime.Now.AddMonths(-2) },
                new Client { Id = 4, Name = "ООО Компьютеры Плюс", ClientType = "ООО", Phone = "+7 (495) 555-66-77", Email = "orders@computers-plus.ru", Address = "г. Москва, ул. Тверская, д. 20", INN = "7702345678", Notes = "Корпоративные заказы", CreatedDate = DateTime.Now.AddMonths(-4) },
                new Client { Id = 5, Name = "Сидоров Сидор Сидорович", ClientType = "Физическое лицо", Phone = "+7 (926) 111-22-33", Email = "sidorov@yandex.ru", Address = "г. Москва, ул. Арбат, д. 25, кв. 5", Notes = "Домашние ПК", CreatedDate = DateTime.Now.AddMonths(-1) }
            };

            ClientsGrid.ItemsSource = _allClients;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            var searchText = SearchBox.Text.ToLower();
            if (string.IsNullOrWhiteSpace(searchText) || searchText == "поиск клиентов...")
            {
                ClientsGrid.ItemsSource = _allClients;
            }
            else
            {
                var filtered = _allClients.Where(c => 
                    c.Name.ToLower().Contains(searchText) ||
                    c.Phone.ToLower().Contains(searchText) ||
                    c.Email.ToLower().Contains(searchText) ||
                    c.ClientType.ToLower().Contains(searchText)).ToList();
                ClientsGrid.ItemsSource = filtered;
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new Views.Dialogs.ClientManagementDialog();
                if (dialog.ShowDialog() == true)
                {
                    var client = dialog.GetClient();
                    client.Id = _allClients.Count + 1;
                    
                    // Добавляем в список (в реальном приложении сохранялось бы в БД)
                    _allClients.Add(client);
                    
                    Views.Dialogs.CustomMessageBox.Show("Клиент успешно добавлен!", "Успех", 
                                  MessageBoxImage.Information, MessageBoxButton.OK);
                    
                    LoadClients();
                }
            }
            catch (Exception ex)
            {
                Views.Dialogs.CustomMessageBox.Show($"Ошибка при добавлении клиента: {ex.Message}", "Ошибка", 
                              MessageBoxImage.Error, MessageBoxButton.OK);
            }
        }

        private void ShowAddClientPanel()
        {
            // Показываем панель добавления
            AddClientPanel.Visibility = Visibility.Visible;
            
            // Очищаем поля
            ClearAddClientFields();
            
            // Загружаем типы клиентов в ComboBox
            LoadClientTypes();
            
            // Устанавливаем фокус на первое поле
            NameTextBox.Focus();
        }

        private void LoadClientTypes()
        {
            var clientTypes = new List<string>
            {
                "Физическое лицо",
                "ИП",
                "ООО",
                "АО",
                "ПАО",
                "ЗАО"
            };
            
            ClientTypeComboBox.ItemsSource = clientTypes;
        }

        private void ClearAddClientFields()
        {
            NameTextBox.Text = "";
            PhoneTextBox.Text = "";
            EmailTextBox.Text = "";
            INNTextBox.Text = "";
            AddressTextBox.Text = "";
            ClientTypeComboBox.SelectedIndex = -1;
            
            // Обновляем placeholder'ы
            UpdateClientPlaceholders();
        }

        private void SaveClient_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Валидация полей
                if (string.IsNullOrWhiteSpace(NameTextBox.Text))
                {
                    MessageBox.Show("Введите название или ФИО клиента", "Ошибка валидации", 
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    NameTextBox.Focus();
                    return;
                }

                if (ClientTypeComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Выберите тип клиента", "Ошибка валидации", 
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    ClientTypeComboBox.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(PhoneTextBox.Text))
                {
                    MessageBox.Show("Введите телефон клиента", "Ошибка валидации", 
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    PhoneTextBox.Focus();
                    return;
                }

                // Создаем нового клиента
                var newClient = new Client
                {
                    Id = _allClients.Count + 1,
                    Name = NameTextBox.Text.Trim(),
                    ClientType = ClientTypeComboBox.SelectedItem.ToString(),
                    Phone = PhoneTextBox.Text.Trim(),
                    Email = EmailTextBox.Text.Trim(),
                    INN = INNTextBox.Text.Trim(),
                    Address = AddressTextBox.Text.Trim(),
                    Notes = "",
                    CreatedDate = DateTime.Now,
                    IsActive = true
                };

                // Добавляем в список (в реальном приложении сохранялось бы в БД)
                _allClients.Add(newClient);
                
                MessageBox.Show("Клиент успешно добавлен!", "Успех", 
                              MessageBoxButton.OK, MessageBoxImage.Information);
                
                // Скрываем панель и обновляем список
                HideAddClientPanel();
                LoadClients();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении клиента: {ex.Message}", "Ошибка", 
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelAddClient_Click(object sender, RoutedEventArgs e)
        {
            HideAddClientPanel();
        }

        private void HideAddClientPanel()
        {
            AddClientPanel.Visibility = Visibility.Collapsed;
            ClearAddClientFields();
        }

        // Обработчики для placeholder'ов в полях добавления
        private void NameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            NamePlaceholder.Visibility = Visibility.Hidden;
        }

        private void NameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(NameTextBox.Text))
                NamePlaceholder.Visibility = Visibility.Visible;
        }

        private void PhoneTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            PhonePlaceholder.Visibility = Visibility.Hidden;
        }

        private void PhoneTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(PhoneTextBox.Text))
                PhonePlaceholder.Visibility = Visibility.Visible;
        }

        private void EmailTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            EmailPlaceholder.Visibility = Visibility.Hidden;
        }

        private void EmailTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(EmailTextBox.Text))
                EmailPlaceholder.Visibility = Visibility.Visible;
        }

        private void INNTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            INNPlaceholder.Visibility = Visibility.Hidden;
        }

        private void INNTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(INNTextBox.Text))
                INNPlaceholder.Visibility = Visibility.Visible;
        }

        private void AddressTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            AddressPlaceholder.Visibility = Visibility.Hidden;
        }

        private void AddressTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(AddressTextBox.Text))
                AddressPlaceholder.Visibility = Visibility.Visible;
        }

        private void UpdateClientPlaceholders()
        {
            NamePlaceholder.Visibility = string.IsNullOrEmpty(NameTextBox.Text) ? Visibility.Visible : Visibility.Hidden;
            PhonePlaceholder.Visibility = string.IsNullOrEmpty(PhoneTextBox.Text) ? Visibility.Visible : Visibility.Hidden;
            EmailPlaceholder.Visibility = string.IsNullOrEmpty(EmailTextBox.Text) ? Visibility.Visible : Visibility.Hidden;
            INNPlaceholder.Visibility = string.IsNullOrEmpty(INNTextBox.Text) ? Visibility.Visible : Visibility.Hidden;
            AddressPlaceholder.Visibility = string.IsNullOrEmpty(AddressTextBox.Text) ? Visibility.Visible : Visibility.Hidden;
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (ClientsGrid.SelectedItem is Client selectedClient)
            {
                try
                {
                    var dialog = new Views.Dialogs.ClientManagementDialog(selectedClient);
                    if (dialog.ShowDialog() == true)
                    {
                        // Обновляем данные клиента
                        var updatedClient = dialog.GetClient();
                        
                        Views.Dialogs.CustomMessageBox.Show("Данные клиента успешно обновлены!", "Успех", 
                                      MessageBoxImage.Information, MessageBoxButton.OK);
                        
                        LoadClients();
                    }
                }
                catch (Exception ex)
                {
                    Views.Dialogs.CustomMessageBox.Show($"Ошибка при редактировании клиента: {ex.Message}", "Ошибка", 
                                  MessageBoxImage.Error, MessageBoxButton.OK);
                }
            }
            else
            {
                Views.Dialogs.CustomMessageBox.Show("Выберите клиента для редактирования", "Предупреждение", 
                              MessageBoxImage.Warning, MessageBoxButton.OK);
            }
        }

        private void ViewOrdersButton_Click(object sender, RoutedEventArgs e)
        {
            if (ClientsGrid.SelectedItem is Client selectedClient)
            {
                Views.Dialogs.CustomMessageBox.Show($"Заказы клиента: {selectedClient.Name}\n\nФункция просмотра заказов будет реализована в следующей версии", "Информация", 
                              MessageBoxImage.Information, MessageBoxButton.OK);
            }
            else
            {
                Views.Dialogs.CustomMessageBox.Show("Выберите клиента для просмотра заказов", "Предупреждение", 
                              MessageBoxImage.Warning, MessageBoxButton.OK);
            }
        }

        // Обработчики для placeholder'а в поле поиска
        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SearchBox.Text == "Поиск клиентов...")
            {
                SearchBox.Text = "";
                SearchBox.Foreground = System.Windows.Media.Brushes.White;
            }
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchBox.Text))
            {
                SearchBox.Text = "Поиск клиентов...";
                SearchBox.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(139, 148, 158));
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Автоматический поиск при изменении текста (если это не placeholder)
            if (SearchBox.Text != "Поиск клиентов...")
            {
                SearchButton_Click(sender, new RoutedEventArgs());
            }
        }
    }
}