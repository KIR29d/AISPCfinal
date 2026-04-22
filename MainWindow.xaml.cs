using System;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using AISPC.Data;
using AISPC.Services;
using AISPC.Views;
using AISPC.Models;

namespace AISPC
{
    public partial class MainWindow : Window
    {
        private readonly AuthenticationService _authService;

        public MainWindow()
        {
            InitializeComponent();
            
            try
            {
                // Настройка подключения к базе данных
                var configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: true)
                    .Build();

                var connectionString = configuration.GetConnectionString("DefaultConnection");
                
                if (!string.IsNullOrEmpty(connectionString))
                {
                    var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                        .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
                        .Options;

                    var context = new ApplicationDbContext(options);
                    _authService = new AuthenticationService(context);
                }
                else
                {
                    // Если нет настроек подключения, используем контекст по умолчанию
                    var context = new ApplicationDbContext();
                    _authService = new AuthenticationService(context);
                }
            }
            catch (Exception)
            {
                // Если не удается подключиться к базе, используем контекст по умолчанию
                var context = new ApplicationDbContext();
                _authService = new AuthenticationService(context);
                
                StatusText.Text = "Предупреждение: Работа в автономном режиме";
                StatusText.Foreground = System.Windows.Media.Brushes.Orange;
            }

            // Устанавливаем фокус на поле пароля, если логин уже заполнен
            if (!string.IsNullOrEmpty(LoginBox.Text))
            {
                PasswordBox.Focus();
            }
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginBox.Text.Trim();
            string password = PasswordBox.Password;

            // Валидация входных данных
            if (string.IsNullOrWhiteSpace(login) || login == "Введите логин")
            {
                ShowStatus("Введите логин", System.Windows.Media.Brushes.Red);
                LoginBox.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                ShowStatus("Введите пароль", System.Windows.Media.Brushes.Red);
                PasswordBox.Focus();
                return;
            }

            // Блокируем интерфейс во время проверки
            SetLoginState(false);
            ShowStatus("🔄 Проверка учетных данных...", System.Windows.Media.Brushes.Blue);

            try
            {
                var result = await _authService.AuthenticateAsync(login, password);

                if (result.IsSuccess && _authService.CurrentUser != null)
                {
                    ShowStatus($"✅ Добро пожаловать, {_authService.CurrentUser.Role?.Name}!", 
                              System.Windows.Media.Brushes.Green);
                    
                    // Небольшая задержка для показа сообщения об успехе
                    await System.Threading.Tasks.Task.Delay(1000);
                    
                    // Открываем главное окно приложения
                    OpenDashboard();
                }
                else
                {
                    ShowStatus($"❌ {result.Message}", System.Windows.Media.Brushes.Red);
                    PasswordBox.Clear();
                    PasswordBox.Focus();
                }
            }
            catch (Exception ex)
            {
                ShowStatus($"🔌 Ошибка подключения: {ex.Message}", System.Windows.Media.Brushes.Red);
                
                // В случае ошибки подключения к БД, пробуем простую проверку
                if (login == "admin" && password == "admin")
                {
                    ShowStatus("✅ Вход выполнен (автономный режим)", System.Windows.Media.Brushes.Orange);
                    await System.Threading.Tasks.Task.Delay(1000);
                    OpenDashboard();
                }
            }
            finally
            {
                SetLoginState(true);
            }
        }

        private void OpenDashboard()
        {
            try
            {
                // Создаем и открываем окно дашборда
                var currentUser = _authService.CurrentUser ?? new User 
                { 
                    Login = "admin", 
                    Role = new Role { Name = "Администратор" } 
                };
                
                var dashboardWindow = new DashboardWindow(currentUser);
                dashboardWindow.Show();
                
                // Закрываем окно входа
                this.Close();
            }
            catch (Exception ex)
            {
                Views.Dialogs.CustomMessageBox.Show($"Ошибка при открытии главного окна: {ex.Message}", 
                              "Ошибка", MessageBoxImage.Error, MessageBoxButton.OK);
            }
        }

        private void ShowStatus(string message, System.Windows.Media.Brush color)
        {
            StatusText.Text = message;
            StatusText.Foreground = color;
        }

        private void SetLoginState(bool enabled)
        {
            LoginButton.IsEnabled = enabled;
            LoginBox.IsEnabled = enabled;
            PasswordBox.IsEnabled = enabled;
            
            if (enabled)
            {
                LoginButton.Content = "🚀 Войти в систему";
            }
            else
            {
                LoginButton.Content = "⏳ Проверка...";
            }
        }

        // Обработка нажатия Enter в полях ввода
        private void LoginBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                PasswordBox.Focus();
            }
        }

        private void PasswordBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                LoginButton_Click(sender, new RoutedEventArgs());
            }
        }

        // Обработка загрузки окна
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Устанавливаем фокус на соответствующее поле
            LoginBox.Focus();
        }

        // Обработчики для placeholder'ов в поле логина
        private void LoginBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (LoginBox.Text == "Введите логин")
            {
                LoginBox.Text = "";
                LoginBox.Foreground = System.Windows.Media.Brushes.White;
            }
        }

        private void LoginBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LoginBox.Text))
            {
                LoginBox.Text = "Введите логин";
                LoginBox.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(139, 148, 158));
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
    }
}