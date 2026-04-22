using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AISPC.Models;
using AISPC.Data;

namespace AISPC.Views.Dialogs
{
    public partial class AddUserDialog : Window
    {
        private readonly ApplicationDbContext? _context;

        public AddUserDialog()
        {
            InitializeComponent();
            
            try
            {
                _context = new ApplicationDbContext();
                LoadRoles();
            }
            catch
            {
                // Если не удается подключиться к БД, работаем без контекста
            }
            
            UpdatePreview();
            
            // Подписываемся на изменения для обновления превью
            LoginTextBox.TextChanged += (s, e) => UpdatePreview();
            PasswordBox.PasswordChanged += (s, e) => UpdatePreview();
            RoleComboBox.SelectionChanged += (s, e) => UpdatePreview();
        }

        private async void LoadRoles()
        {
            try
            {
                if (_context != null)
                {
                    var roles = await System.Threading.Tasks.Task.Run(() => _context.Roles.ToList());
                    RoleComboBox.ItemsSource = roles;
                    if (roles.Any())
                    {
                        RoleComboBox.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки ролей: {ex.Message}", "Ошибка", 
                              MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void UpdatePreview()
        {
            try
            {
                var login = string.IsNullOrWhiteSpace(LoginTextBox.Text) ? "Не указан" : LoginTextBox.Text;
                var password = string.IsNullOrWhiteSpace(PasswordBox.Password) ? "Не указан" : "••••••••";
                var role = (RoleComboBox.SelectedItem as Role)?.Name ?? "Не выбрана";
                
                PreviewText.Text = $"Логин: {login}\n" +
                                 $"Пароль: {password}\n" +
                                 $"Роль: {role}";
            }
            catch
            {
                PreviewText.Text = "Ошибка в данных пользователя";
            }
        }

        private async void AddUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Валидация данных
                if (string.IsNullOrWhiteSpace(LoginTextBox.Text) || LoginTextBox.Text == "Введите логин пользователя")
                {
                    MessageBox.Show("Введите логин пользователя", "Ошибка валидации", 
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    LoginTextBox.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(PasswordBox.Password))
                {
                    MessageBox.Show("Введите пароль", "Ошибка валидации", 
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    PasswordBox.Focus();
                    return;
                }

                if (RoleComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Выберите роль пользователя", "Ошибка валидации", 
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    RoleComboBox.Focus();
                    return;
                }

                // Проверяем уникальность логина
                if (_context != null)
                {
                    var existingUser = _context.Users.FirstOrDefault(u => u.Login == LoginTextBox.Text.Trim());
                    if (existingUser != null)
                    {
                        MessageBox.Show("Пользователь с таким логином уже существует", "Ошибка валидации", 
                                      MessageBoxButton.OK, MessageBoxImage.Warning);
                        LoginTextBox.Focus();
                        return;
                    }
                }

                // Создаем нового пользователя
                var selectedRole = RoleComboBox.SelectedItem as Role;
                var user = new User
                {
                    Login = LoginTextBox.Text.Trim(),
                    PasswordHash = PasswordBox.Password, // В реальном приложении нужно хешировать пароль
                    RoleId = selectedRole!.Id,
                    IsActive = true
                };

                // Сохраняем в базу данных
                if (_context != null)
                {
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении пользователя: {ex.Message}", "Ошибка", 
                              MessageBoxButton.OK, MessageBoxImage.Error);
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

        // Обработчики для placeholder'ов
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null && textBox.Text == "Введите логин пользователя")
            {
                textBox.Text = "";
                textBox.Foreground = System.Windows.Media.Brushes.White;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null && string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "Введите логин пользователя";
                textBox.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(139, 148, 158));
            }
        }

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
    }
}