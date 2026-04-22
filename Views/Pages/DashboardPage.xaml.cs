using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AISPC.Models;
using AISPC.Data;
using AISPC.Services;
using AISPC.Views.Dialogs;

namespace AISPC.Views.Pages
{
    public partial class DashboardPage : Page
    {
        private readonly ApplicationDbContext _context;
        private readonly OrderService _orderService;
        private readonly ComponentService _componentService;

        public DashboardPage()
        {
            InitializeComponent();
            
            try
            {
                _context = new ApplicationDbContext();
                _orderService = new OrderService(_context);
                _componentService = new ComponentService(_context);
            }
            catch
            {
                // Если не удается подключиться к БД, работаем с тестовыми данными
            }
            
            LoadDashboardData();
        }

        private async void LoadDashboardData()
        {
            try
            {
                if (_orderService != null && _componentService != null)
                {
                    // Загружаем реальные данные из БД
                    var orders = await _orderService.GetAllOrdersAsync();
                    var components = await _componentService.GetAllComponentsAsync();
                    
                    OrdersCountText.Text = orders.Count(o => o.Status != "Завершен" && o.Status != "Отменен").ToString();
                    ComponentsCountText.Text = components.Sum(c => c.StockQuantity).ToString();
                    AssemblyCountText.Text = orders.Count(o => o.Status == "Сборка").ToString();
                    ClientsCountText.Text = orders.Select(o => o.ClientName).Distinct().Count().ToString();
                    
                    // Загружаем последние заказы
                    var recentOrders = orders.OrderByDescending(o => o.OrderDate).Take(10).ToList();
                    RecentOrdersGrid.ItemsSource = recentOrders;
                }
                else
                {
                    LoadTestData();
                }
            }
            catch (Exception)
            {
                LoadTestData();
            }
        }

        private void LoadTestData()
        {
            // Загружаем статистику (тестовые данные)
            OrdersCountText.Text = "12";
            ComponentsCountText.Text = "247";
            AssemblyCountText.Text = "5";
            ClientsCountText.Text = "89";

            // Загружаем последние заказы (тестовые данные)
            var recentOrders = new List<Order>
            {
                new Order { Id = 1001, ClientName = "ООО Техносфера", Status = "В обработке", TotalAmount = 125000m, OrderDate = DateTime.Now.AddDays(-1), Priority = "Высокий" },
                new Order { Id = 1002, ClientName = "Иванов И.И.", Status = "Сборка", TotalAmount = 85000m, OrderDate = DateTime.Now.AddDays(-2), Priority = "Обычный" },
                new Order { Id = 1003, ClientName = "ИП Петров", Status = "Готов", TotalAmount = 95000m, OrderDate = DateTime.Now.AddDays(-3), Priority = "Обычный" },
                new Order { Id = 1004, ClientName = "ООО Компьютеры+", Status = "Доставка", TotalAmount = 150000m, OrderDate = DateTime.Now.AddDays(-4), Priority = "Высокий" },
                new Order { Id = 1005, ClientName = "Сидоров С.С.", Status = "Завершен", TotalAmount = 75000m, OrderDate = DateTime.Now.AddDays(-5), Priority = "Обычный" }
            };

            RecentOrdersGrid.ItemsSource = recentOrders;
        }

        private void NewOrder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new AddOrderDialog();
                if (dialog.ShowDialog() == true)
                {
                    // Обновляем данные после добавления заказа
                    LoadDashboardData();
                    MessageBox.Show("Заказ успешно создан!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании заказа: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddComponent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new AddComponentDialog();
                if (dialog.ShowDialog() == true)
                {
                    // Обновляем данные после добавления компонента
                    LoadDashboardData();
                    MessageBox.Show("Компонент успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении компонента: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NewClient_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new Views.Dialogs.AddClientDialog();
                if (dialog.ShowDialog() == true)
                {
                    // Обновляем данные после добавления клиента
                    LoadDashboardData();
                    MessageBox.Show("Клиент успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении клиента: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void NewAssemblyTask_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new Views.Dialogs.AddAssemblyTaskDialog();
                if (dialog.ShowDialog() == true)
                {
                    var assemblyService = new AssemblyService(_context ?? new ApplicationDbContext());
                    var task = new AssemblyTask
                    {
                        OrderId = dialog.OrderId,
                        AssemblerId = dialog.AssemblerId,
                        AssemblerName = dialog.AssemblerName,
                        Status = "Ожидает",
                        StartDate = dialog.StartDate,
                        Notes = dialog.Notes
                    };

                    var success = await assemblyService.AddAssemblyTaskAsync(task);
                    if (success)
                    {
                        LoadDashboardData();
                        MessageBox.Show("Задача сборки успешно создана!", "Успех", 
                                      MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при создании задачи сборки", "Ошибка", 
                                      MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании задачи сборки: {ex.Message}", "Ошибка", 
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshData_Click(object sender, RoutedEventArgs e)
        {
            LoadDashboardData();
            MessageBox.Show("Данные обновлены", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void RecentOrdersGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (RecentOrdersGrid.SelectedItem is Order selectedOrder)
            {
                MessageBox.Show($"Детали заказа №{selectedOrder.Id}\n" +
                              $"Клиент: {selectedOrder.ClientName}\n" +
                              $"Статус: {selectedOrder.Status}\n" +
                              $"Сумма: {selectedOrder.TotalAmount:C}\n" +
                              $"Приоритет: {selectedOrder.Priority}\n" +
                              $"Дата: {selectedOrder.OrderDate:dd.MM.yyyy}",
                              "Детали заказа", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}