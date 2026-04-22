using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using AISPC.Data;
using AISPC.Models;
using AISPC.Services;
using AISPC.Views.Dialogs;

namespace AISPC.Views.Pages
{
    public partial class UsersPage : Page
    {
        private readonly ApplicationDbContext? _context;
        private readonly UserService? _userService;
        private List<User> _users = new List<User>();
        private List<object> _allUsers = new List<object>(); // Для хранения всех пользователей (включая тестовые)

        public UsersPage()
        {
            InitializeComponent();
            
            try
            {
                _context = new ApplicationDbContext();
                _userService = new UserService(_context);
                LoadUsers();
            }
            catch (Exception ex)
            {
                // Если не удается подключиться к БД, работаем без сервисов
                _context = null;
                _userService = null;
                LoadTestUsers();
                Views.Dialogs.CustomMessageBox.Show($"База данных недоступна: {ex.Message}\nПоказаны тестовые данные.", "Информация", 
                              MessageBoxImage.Information, MessageBoxButton.OK);
            }
        }

        private async void LoadUsers()
        {
            try
            {
                if (_userService != null)
                {
                    _users = await _userService.GetAllUsersAsync();
                    _allUsers = _users.Cast<object>().ToList();
                    UsersGrid.ItemsSource = _users;
                }
                else
                {
                    // Показываем тестовые данные, если БД недоступна
                    LoadTestUsers();
                }
                UpdateStatusText();
            }
            catch (Exception ex)
            {
                // Если не удается загрузить из БД, показываем тестовые данные
                LoadTestUsers();
                Views.Dialogs.CustomMessageBox.Show($"Ошибка загрузки пользователей из БД: {ex.Message}\nПоказаны тестовые данные.", "Предупреждение", 
                              MessageBoxImage.Warning, MessageBoxButton.OK);
            }
        }

        private void LoadTestUsers()
        {
            // Создаем простые объекты для тестирования
            var users = new List<dynamic>
            {
                new { Id = 1, Login = "admin", Role = new { Name = "Администратор" }, IsActive = true, PasswordHash = "***" },
                new { Id = 2, Login = "manager", Role = new { Name = "Менеджер" }, IsActive = true, PasswordHash = "***" },
                new { Id = 3, Login = "assembler", Role = new { Name = "Сборщик" }, IsActive = true, PasswordHash = "***" },
                new { Id = 4, Login = "warehouse", Role = new { Name = "Кладовщик" }, IsActive = false, PasswordHash = "***" }
            };

            _allUsers = users.Cast<object>().ToList();
            
            // Принудительно обновляем DataGrid
            UsersGrid.ItemsSource = null;
            UsersGrid.ItemsSource = _allUsers;
            UsersGrid.Items.Refresh();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = SearchTextBox.Text.ToLower();
            
            // Управляем видимостью placeholder'а
            PlaceholderText.Visibility = string.IsNullOrEmpty(SearchTextBox.Text) ? Visibility.Visible : Visibility.Hidden;
            
            if (string.IsNullOrWhiteSpace(searchText))
            {
                // Показываем всех пользователей
                UsersGrid.ItemsSource = _allUsers;
            }
            else
            {
                // Фильтруем пользователей
                if (_userService != null && _users.Any())
                {
                    // Работаем с реальными данными из БД
                    var filteredUsers = _users.Where(u => 
                        u.Login.ToLower().Contains(searchText) ||
                        (u.Role?.Name?.ToLower().Contains(searchText) ?? false)
                    ).ToList();
                    
                    UsersGrid.ItemsSource = filteredUsers;
                }
                else
                {
                    // Работаем с тестовыми данными
                    var filteredUsers = _allUsers.Where(u => 
                    {
                        var userType = u.GetType();
                        var login = userType.GetProperty("Login")?.GetValue(u)?.ToString()?.ToLower() ?? "";
                        var role = userType.GetProperty("Role")?.GetValue(u);
                        var roleName = role?.GetType().GetProperty("Name")?.GetValue(role)?.ToString()?.ToLower() ?? "";
                        
                        return login.Contains(searchText) || roleName.Contains(searchText);
                    }).ToList();
                    
                    UsersGrid.ItemsSource = filteredUsers;
                }
            }
        }

        private void UpdateStatusText()
        {
            var activeUsers = _users.Count(u => u.IsActive);
            var totalUsers = _users.Count;
            // Обновляем статус, если элемент существует
            // StatusText.Text = $"Всего пользователей: {totalUsers} | Активных: {activeUsers}";
        }

        private async void AddUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new Views.Dialogs.UserManagementDialog();
                if (dialog.ShowDialog() == true)
                {
                    var user = dialog.GetUser();
                    
                    Views.Dialogs.CustomMessageBox.Show("Пользователь успешно добавлен!", "Успех", 
                                  MessageBoxImage.Information, MessageBoxButton.OK);
                    
                    LoadUsers();
                }
            }
            catch (Exception ex)
            {
                Views.Dialogs.CustomMessageBox.Show($"Ошибка при добавлении пользователя: {ex.Message}", "Ошибка", 
                              MessageBoxImage.Error, MessageBoxButton.OK);
            }
        }

        private void ShowAddUserPanel()
        {
            // Показываем панель добавления
            AddUserPanel.Visibility = Visibility.Visible;
            
            // Очищаем поля
            LoginTextBox.Text = "";
            PasswordBox.Password = "";
            RoleComboBox.SelectedIndex = -1;
            
            // Загружаем роли в ComboBox
            LoadRoles();
            
            // Устанавливаем фокус на первое поле
            LoginTextBox.Focus();
        }

        private async void LoadRoles()
        {
            try
            {
                if (_userService != null)
                {
                    // Загружаем роли из БД через UserService
                    var roles = await _userService.GetAllRolesAsync();
                    RoleComboBox.ItemsSource = roles;
                }
                else if (_context != null)
                {
                    // Загружаем роли из БД напрямую
                    var roles = await _context.Roles.OrderBy(r => r.Name).ToListAsync();
                    RoleComboBox.ItemsSource = roles;
                }
                else
                {
                    // Загружаем тестовые роли
                    var roles = new List<object>
                    {
                        new { Id = 1, Name = "Администратор" },
                        new { Id = 2, Name = "Менеджер" },
                        new { Id = 3, Name = "Сборщик" },
                        new { Id = 4, Name = "Кладовщик" }
                    };
                    
                    RoleComboBox.ItemsSource = roles;
                }
            }
            catch (Exception ex)
            {
                Views.Dialogs.CustomMessageBox.Show($"Ошибка загрузки ролей: {ex.Message}", "Ошибка", 
                              MessageBoxImage.Error, MessageBoxButton.OK);
                
                // Fallback на тестовые роли
                var roles = new List<object>
                {
                    new { Id = 1, Name = "Администратор" },
                    new { Id = 2, Name = "Менеджер" },
                    new { Id = 3, Name = "Сборщик" },
                    new { Id = 4, Name = "Кладовщик" }
                };
                
                RoleComboBox.ItemsSource = roles;
            }
        }

        private async void SaveUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Валидация полей
                if (string.IsNullOrWhiteSpace(LoginTextBox.Text))
                {
                    Views.Dialogs.CustomMessageBox.Show("Введите логин пользователя", "Ошибка валидации", 
                                  MessageBoxImage.Warning, MessageBoxButton.OK);
                    LoginTextBox.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(PasswordBox.Password))
                {
                    Views.Dialogs.CustomMessageBox.Show("Введите пароль пользователя", "Ошибка валидации", 
                                  MessageBoxImage.Warning, MessageBoxButton.OK);
                    PasswordBox.Focus();
                    return;
                }

                if (RoleComboBox.SelectedItem == null)
                {
                    Views.Dialogs.CustomMessageBox.Show("Выберите роль пользователя", "Ошибка валидации", 
                                  MessageBoxImage.Warning, MessageBoxButton.OK);
                    RoleComboBox.Focus();
                    return;
                }

                if (_userService != null)
                {
                    // Создаем нового пользователя
                    var newUser = new User
                    {
                        Login = LoginTextBox.Text.Trim(),
                        PasswordHash = PasswordBox.Password, // В реальном приложении должно быть хеширование
                        RoleId = ((dynamic)RoleComboBox.SelectedItem).Id,
                        IsActive = true
                    };

                    // Сохраняем в БД
                    await _userService.CreateUserAsync(newUser);
                    
                    Views.Dialogs.CustomMessageBox.Show("Пользователь успешно добавлен!", "Успех", 
                                  MessageBoxImage.Information, MessageBoxButton.OK);
                    
                    // Скрываем панель и обновляем список
                    HideAddUserPanel();
                    LoadUsers();
                }
                else
                {
                    Views.Dialogs.CustomMessageBox.Show("База данных недоступна. Пользователь не может быть добавлен.", "Ошибка", 
                                  MessageBoxImage.Error, MessageBoxButton.OK);
                }
            }
            catch (InvalidOperationException ex)
            {
                // Обрабатываем ошибку уникальности логина
                Views.Dialogs.CustomMessageBox.Show(ex.Message, "Ошибка", 
                              MessageBoxImage.Warning, MessageBoxButton.OK);
                LoginTextBox.Focus();
            }
            catch (Exception ex)
            {
                Views.Dialogs.CustomMessageBox.Show($"Ошибка при сохранении пользователя: {ex.Message}", "Ошибка", 
                              MessageBoxImage.Error, MessageBoxButton.OK);
            }
        }

        private void CancelAddUser_Click(object sender, RoutedEventArgs e)
        {
            HideAddUserPanel();
        }

        private void HideAddUserPanel()
        {
            AddUserPanel.Visibility = Visibility.Collapsed;
            
            // Очищаем поля
            LoginTextBox.Text = "";
            PasswordBox.Password = "";
            RoleComboBox.SelectedIndex = -1;
            
            // Обновляем placeholder'ы
            UpdatePlaceholders();
        }

        private void LoginTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            LoginPlaceholder.Visibility = Visibility.Hidden;
        }

        private void LoginTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(LoginTextBox.Text))
            {
                LoginPlaceholder.Visibility = Visibility.Visible;
            }
        }

        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            PasswordPlaceholder.Visibility = Visibility.Hidden;
        }

        private void PasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(PasswordBox.Password))
            {
                PasswordPlaceholder.Visibility = Visibility.Visible;
            }
        }

        private void UpdatePlaceholders()
        {
            LoginPlaceholder.Visibility = string.IsNullOrEmpty(LoginTextBox.Text) ? Visibility.Visible : Visibility.Hidden;
            PasswordPlaceholder.Visibility = string.IsNullOrEmpty(PasswordBox.Password) ? Visibility.Visible : Visibility.Hidden;
        }

        private async void DeactivateUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_userService == null)
                {
                    Views.Dialogs.CustomMessageBox.Show("База данных недоступна. Функция управления пользователями временно отключена.", "Предупреждение", 
                                  MessageBoxImage.Warning, MessageBoxButton.OK);
                    return;
                }

                if (UsersGrid.SelectedItem is User selectedUser)
                {
                    var result = Views.Dialogs.CustomMessageBox.Show($"Деактивировать пользователя '{selectedUser.Login}'?", 
                                               "Подтверждение", MessageBoxImage.Question, MessageBoxButton.YesNo);
                    
                    if (result == MessageBoxResult.Yes)
                    {
                        await _userService.DeactivateUserAsync(selectedUser.Id);
                        LoadUsers();
                        Views.Dialogs.CustomMessageBox.Show("Пользователь деактивирован!", "Успех", 
                                      MessageBoxImage.Information, MessageBoxButton.OK);
                    }
                }
                else
                {
                    Views.Dialogs.CustomMessageBox.Show("Выберите пользователя для деактивации", "Предупреждение", 
                                  MessageBoxImage.Warning, MessageBoxButton.OK);
                }
            }
            catch (Exception ex)
            {
                Views.Dialogs.CustomMessageBox.Show($"Ошибка при деактивации пользователя: {ex.Message}", "Ошибка", 
                              MessageBoxImage.Error, MessageBoxButton.OK);
            }
        }

        private async void ActivateUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_userService == null)
                {
                    Views.Dialogs.CustomMessageBox.Show("База данных недоступна. Функция управления пользователями временно отключена.", "Предупреждение", 
                                  MessageBoxImage.Warning, MessageBoxButton.OK);
                    return;
                }

                if (UsersGrid.SelectedItem is User selectedUser)
                {
                    await _userService.ActivateUserAsync(selectedUser.Id);
                    LoadUsers();
                    Views.Dialogs.CustomMessageBox.Show("Пользователь активирован!", "Успех", 
                                  MessageBoxImage.Information, MessageBoxButton.OK);
                }
                else
                {
                    Views.Dialogs.CustomMessageBox.Show("Выберите пользователя для активации", "Предупреждение", 
                                  MessageBoxImage.Warning, MessageBoxButton.OK);
                }
            }
            catch (Exception ex)
            {
                Views.Dialogs.CustomMessageBox.Show($"Ошибка при активации пользователя: {ex.Message}", "Ошибка", 
                              MessageBoxImage.Error, MessageBoxButton.OK);
            }
        }

        private void RefreshUsers_Click(object sender, RoutedEventArgs e)
        {
            LoadUsers();
        }

        private async void EditUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (UsersGrid.SelectedItem is User selectedUser)
                {
                    var dialog = new Views.Dialogs.UserManagementDialog(selectedUser);
                    if (dialog.ShowDialog() == true)
                    {
                        var updatedUser = dialog.GetUser();
                        
                        Views.Dialogs.CustomMessageBox.Show("Данные пользователя успешно обновлены!", "Успех", 
                                      MessageBoxImage.Information, MessageBoxButton.OK);
                        
                        LoadUsers();
                    }
                }
                else
                {
                    Views.Dialogs.CustomMessageBox.Show("Выберите пользователя для редактирования", "Предупреждение", 
                                  MessageBoxImage.Warning, MessageBoxButton.OK);
                }
            }
            catch (Exception ex)
            {
                Views.Dialogs.CustomMessageBox.Show($"Ошибка при редактировании пользователя: {ex.Message}", "Ошибка", 
                              MessageBoxImage.Error, MessageBoxButton.OK);
            }
        }
    }
}