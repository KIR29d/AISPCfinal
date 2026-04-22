using System;
using System.Windows;
using System.Windows.Controls;
using AISPC.Models;
using AISPC.Views.Pages;

namespace AISPC.Views
{
    public partial class DashboardWindow : Window
    {
        private readonly User _currentUser;

        public DashboardWindow(User currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
            
            UserInfoText.Text = $"Пользователь: {_currentUser.Login} ({_currentUser.Role.Name})";
            
            // Загружаем панель управления по умолчанию
            LoadPage("Dashboard");
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string pageTag)
            {
                LoadPage(pageTag);
            }
        }

        private void LoadPage(string pageTag)
        {
            Page? page = pageTag switch
            {
                "Dashboard" => new DashboardPage(),
                "Components" => new ComponentsPage(),
                "Orders" => new OrdersPage(),
                "Clients" => new ClientsPage(),
                "Assembly" => new AssemblyPage(),
                "Warehouse" => new WarehousePage(),
                "Users" => new UsersPage(),
                "Employees" => new EmployeesPage(),
                "Reports" => new ReportsPage(),
                _ => new DashboardPage()
            };

            if (page != null)
            {
                MainFrame.Navigate(page);
                StatusText.Text = $"Загружен модуль: {GetPageTitle(pageTag)}";
            }
        }

        private string GetPageTitle(string pageTag)
        {
            return pageTag switch
            {
                "Dashboard" => "Панель управления",
                "Components" => "Каталог компонентов",
                "Orders" => "Управление заказами",
                "Clients" => "Клиенты",
                "Assembly" => "Сборочные задания",
                "Warehouse" => "Управление складом",
                "Users" => "Пользователи",
                "Employees" => "Сотрудники",
                "Reports" => "Отчеты",
                _ => "Неизвестный модуль"
            };
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var result = Views.Dialogs.CustomMessageBox.Show("Вы действительно хотите выйти из системы?", 
                                       "Подтверждение", 
                                       MessageBoxImage.Question,
                                       MessageBoxButton.YesNo);
            
            if (result == MessageBoxResult.Yes)
            {
                // Открываем окно входа
                var loginWindow = new MainWindow();
                loginWindow.Show();
                
                // Закрываем текущее окно
                this.Close();
            }
        }
    }
}