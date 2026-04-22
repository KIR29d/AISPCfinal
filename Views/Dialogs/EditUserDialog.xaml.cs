using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AISPC.Models;
using AISPC.Services;

namespace AISPC.Views.Dialogs
{
    public partial class EditUserDialog : Window
    {
        public User User { get; private set; }
        public string NewPassword { get; private set; }
        private readonly UserService _userService;

        public EditUserDialog(User user)
        {
            InitializeComponent();
            User = user;
            _userService = new UserService(new Data.ApplicationDbContext());
            LoadUserData();
            LoadRoles();
        }

        private void LoadUserData()
        {
            LoginTextBox.Text = User.Login;
            IsActiveCheckBox.IsChecked = User.IsActive;
            UpdatePreview();
        }

        private async void LoadRoles()
        {
            try
            {
                var roles = await _userService.GetAllRolesAsync();
                RoleComboBox.ItemsSource = roles;
                
                // Устанавливаем текущую роль
                var currentRole = roles.FirstOrDefault(r => r.Id == User.RoleId);
                if (currentRole != null)
                {
                    RoleComboBox.SelectedItem = currentRole;
                }
            }
            catch
            {
                // Если не удалось загрузить роли, создаем тестовые
                var testRoles = new List<Role>
                {
                    new Role { Id = 1, Name = "Администратор" },
                    new Role { Id = 2, Name = "Менеджер" },
                    new Role { Id = 3, Name = "Сборщик" }
                };
                RoleComboBox.ItemsSource = testRoles;
                RoleComboBox.SelectedIndex = 0;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LoginTextBox.Text))
            {
                ShowCustomMessage("Ошибка", "Пожалуйста, введите логин пользователя.", MessageBoxImage.Warning);
                return;
            }

            if (RoleComboBox.SelectedItem == null)
            {
                ShowCustomMessage("Ошибка", "Пожалуйста, выберите роль пользователя.", MessageBoxImage.Warning);
                return;
            }

            // Обновляем данные пользователя
            User.Login = LoginTextBox.Text.Trim();
            User.RoleId = ((Role)RoleComboBox.SelectedItem).Id;
            User.IsActive = IsActiveCheckBox.IsChecked ?? true;
            
            // Сохраняем новый пароль если он был введен
            NewPassword = string.IsNullOrWhiteSpace(PasswordBox.Password) ? null : PasswordBox.Password;

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
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

        private void UpdatePreview()
        {
            var status = IsActiveCheckBox.IsChecked == true ? "активен" : "неактивен";
            PreviewText.Text = $"Пользователь '{LoginTextBox.Text}' будет {status}";
        }

        private void ShowCustomMessage(string title, string message, MessageBoxImage icon)
        {
            var messageWindow = new CustomMessageBox(title, message, icon);
            messageWindow.Owner = this;
            messageWindow.ShowDialog();
        }
    }
}