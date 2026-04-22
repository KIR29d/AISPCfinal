using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using AISPC.Data;
using AISPC.Models;
using AISPC.Services;

namespace AISPC.Views.Dialogs
{
    public partial class UserManagementDialog : Window
    {
        private readonly ApplicationDbContext? _context;
        private readonly UserService? _userService;
        private readonly User? _user;
        private readonly bool _isEditMode;

        public string Login => LoginTextBox.Text.Trim();
        public string Password => PasswordBox.Password;
        public int RoleId => RoleComboBox.SelectedValue != null ? (int)RoleComboBox.SelectedValue : 0;
        public bool IsUserActive => IsActiveCheckBox.IsChecked ?? true;

        // Конструктор для добавления нового пользователя
        public UserManagementDialog()
        {
            InitializeComponent();
            
            try
            {
                _context = new ApplicationDbContext();
                _userService = new UserService(_context);
            }
            catch
            {
                // Если не удается подключиться к БД, работаем без сервисов
            }
            
            _isEditMode = false;
            TitleTextBlock.Text = "👤 Добавление пользователя";
            SaveButton.Content = "💾 Добавить пользователя";
            
            LoadRoles();
            UpdatePreview();
            
            // Подписываемся на изменения для обновления превью
            LoginTextBox.TextChanged += (s, e) => UpdatePreview();
            RoleComboBox.SelectionChanged += (s, e) => UpdatePreview();
            IsActiveCheckBox.Checked += (s, e) => UpdatePreview();
            IsActiveCheckBox.Unchecked += (s, e) => UpdatePreview();
        }

        // Конструктор для редактирования существующего пользователя
        public UserManagementDialog(User user)
        {
            InitializeComponent();
            
            try
            {
                _context = new ApplicationDbContext();
                _userService = new UserService(_context);
            }
            catch
            {
                // Если не удается подключиться к БД, работаем без сервисов
            }
            
            _user = user;
            _isEditMode = true;
            TitleTextBlock.Text = "✏️ Редактирование пользователя";
            SaveButton.Content = "💾 Сохранить изменения";
            
            LoadRoles();
            LoadUserData();
            UpdatePreview();
            
            // Подписываемся на изменения для обновления превью
            LoginTextBox.TextChanged += (s, e) => UpdatePreview();
            RoleComboBox.SelectionChanged += (s, e) => UpdatePreview();
            IsActiveCheckBox.Checked += (s, e) => UpdatePreview();
            IsActiveCheckBox.Unchecked += (s, e) => UpdatePreview();
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
                    var roles = new List<Role>
                    {
                        new Role { Id = 1, Name = "Администратор" },
                        new Role { Id = 2, Name = "Менеджер" },
                        new Role { Id = 3, Name = "Сборщик" },
                        new Role { Id = 4, Name = "Кладовщик" }
                    };
                    
                    RoleComboBox.ItemsSource = roles;
                }
            }
            catch (Exception ex)
            {
                Views.Dialogs.CustomMessageBox.Show($"Ошибка загрузки ролей: {ex.Message}", "Ошибка", 
                              MessageBoxImage.Error, MessageBoxButton.OK, this);
                
                // Fallback на тестовые роли
                var roles = new List<Role>
                {
                    new Role { Id = 1, Name = "Администратор" },
                    new Role { Id = 2, Name = "Менеджер" },
                    new Role { Id = 3, Name = "Сборщик" },
                    new Role { Id = 4, Name = "Кладовщик" }
                };
                
                RoleComboBox.ItemsSource = roles;
            }
        }

        private void LoadUserData()
        {
            if (_user != null)
            {
                LoginTextBox.Text = _user.Login;
                LoginTextBox.Foreground = System.Windows.Media.Brushes.White;
                
                // Пароль не показываем в режиме редактирования
                PasswordPlaceholder.Text = "Оставьте пустым, чтобы не менять пароль";
                
                RoleComboBox.SelectedValue = _user.RoleId;
                IsActiveCheckBox.IsChecked = _user.IsActive;
            }
        }

        private void UpdatePreview()
        {
            try
            {
                var login = string.IsNullOrWhiteSpace(LoginTextBox.Text) || LoginTextBox.Text == "Введите логин пользователя" ? "Не указан" : LoginTextBox.Text;
                var role = (RoleComboBox.SelectedItem as Role)?.Name ?? "Не выбрана";
                var status = IsUserActive ? "Активный" : "Неактивный";
                
                PreviewText.Text = $"Логин: {login}\n" +
                                 $"Роль: {role}\n" +
                                 $"Статус: {status}";
            }
            catch
            {
                PreviewText.Text = "Ошибка в данных пользователя";
            }
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Валидация полей
                if (string.IsNullOrWhiteSpace(LoginTextBox.Text) || LoginTextBox.Text == "Введите логин пользователя")
                {
                    Views.Dialogs.CustomMessageBox.Show("Введите логин пользователя", "Ошибка валидации", 
                                  MessageBoxImage.Warning, MessageBoxButton.OK, this);
                    LoginTextBox.Focus();
                    return;
                }

                if (!_isEditMode && string.IsNullOrWhiteSpace(PasswordBox.Password))
                {
                    Views.Dialogs.CustomMessageBox.Show("Введите пароль пользователя", "Ошибка валидации", 
                                  MessageBoxImage.Warning, MessageBoxButton.OK, this);
                    PasswordBox.Focus();
                    return;
                }

                if (RoleComboBox.SelectedItem == null)
                {
                    Views.Dialogs.CustomMessageBox.Show("Выберите роль пользователя", "Ошибка валидации", 
                                  MessageBoxImage.Warning, MessageBoxButton.OK, this);
                    RoleComboBox.Focus();
                    return;
                }

                if (_isEditMode && _user != null)
                {
                    // Обновляем существующего пользователя
                    _user.Login = Login;
                    if (!string.IsNullOrWhiteSpace(Password))
                    {
                        _user.PasswordHash = Password; // В реальном приложении должно быть хеширование
                    }
                    _user.RoleId = RoleId;
                    _user.IsActive = IsUserActive;
                    
                    if (_userService != null)
                    {
                        await _userService.UpdateUserAsync(_user);
                    }
                }
                else
                {
                    // Создаем нового пользователя
                    var newUser = new User
                    {
                        Login = Login,
                        PasswordHash = Password, // В реальном приложении должно быть хеширование
                        RoleId = RoleId,
                        IsActive = IsUserActive
                    };

                    // Сохраняем в БД
                    if (_userService != null)
                    {
                        await _userService.CreateUserAsync(newUser);
                    }
                }

                DialogResult = true;
                Close();
            }
            catch (InvalidOperationException ex)
            {
                // Обрабатываем ошибку уникальности логина
                Views.Dialogs.CustomMessageBox.Show(ex.Message, "Ошибка", 
                              MessageBoxImage.Warning, MessageBoxButton.OK, this);
                LoginTextBox.Focus();
            }
            catch (Exception ex)
            {
                Views.Dialogs.CustomMessageBox.Show($"Ошибка при сохранении пользователя: {ex.Message}", "Ошибка", 
                              MessageBoxImage.Error, MessageBoxButton.OK, this);
            }
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

        // Обработчики для placeholder'ов в поле логина
        private void LoginTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (LoginTextBox.Text == "Введите логин пользователя")
            {
                LoginTextBox.Text = "";
                LoginTextBox.Foreground = System.Windows.Media.Brushes.White;
            }
        }

        private void LoginTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LoginTextBox.Text))
            {
                LoginTextBox.Text = "Введите логин пользователя";
                LoginTextBox.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(201, 209, 217));
            }
        }

        // Обработчики для placeholder'ов в поле пароля
        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            PasswordPlaceholder.Visibility = Visibility.Collapsed;
        }

        private void PasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(PasswordBox.Password))
            {
                PasswordPlaceholder.Visibility = Visibility.Visible;
            }
        }

        public User GetUser()
        {
            if (_isEditMode && _user != null)
            {
                return _user;
            }
            else
            {
                return new User
                {
                    Login = Login,
                    PasswordHash = Password,
                    RoleId = RoleId,
                    IsActive = IsUserActive
                };
            }
        }
    }
}