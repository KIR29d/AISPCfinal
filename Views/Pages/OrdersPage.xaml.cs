using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AISPC.Models;
using AISPC.Services;
using AISPC.Data;

namespace AISPC.Views.Pages
{
    public partial class OrdersPage : Page
    {
        private readonly OrderService _orderService;
        private List<Order> _allOrders;

        public OrdersPage()
        {
            InitializeComponent();
            try
            {
                var context = new ApplicationDbContext();
                _orderService = new OrderService(context);
            }
            catch
            {
                // Если не удается создать контекст, работаем с тестовыми данными
            }
            LoadOrders();
        }

        private async void LoadOrders()
        {
            try
            {
                if (_orderService != null)
                {
                    _allOrders = await _orderService.GetAllOrdersAsync();
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

            OrdersGrid.ItemsSource = _allOrders;
        }

        private void LoadTestData()
        {
            _allOrders = new List<Order>
            {
                new Order { Id = 1001, ClientName = "ООО Техносфера", Status = "В обработке", TotalAmount = 125000m, OrderDate = DateTime.Now.AddDays(-1), Notes = "Срочный заказ", Priority = "Высокий" },
                new Order { Id = 1002, ClientName = "Иванов И.И.", Status = "Сборка", TotalAmount = 85000m, OrderDate = DateTime.Now.AddDays(-2), Notes = "Игровая конфигурация", Priority = "Обычный" },
                new Order { Id = 1003, ClientName = "ИП Петров", Status = "Готов", TotalAmount = 95000m, OrderDate = DateTime.Now.AddDays(-3), Notes = "Офисный ПК", Priority = "Обычный" },
                new Order { Id = 1004, ClientName = "ООО Компьютеры+", Status = "Доставка", TotalAmount = 150000m, OrderDate = DateTime.Now.AddDays(-4), Notes = "Рабочая станция", Priority = "Высокий" },
                new Order { Id = 1005, ClientName = "Сидоров С.С.", Status = "Завершен", TotalAmount = 75000m, OrderDate = DateTime.Now.AddDays(-5), Notes = "Домашний ПК", Priority = "Обычный", CompletionDate = DateTime.Now.AddDays(-1) }
            };
        }

        private async void NewOrder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new Views.Dialogs.AddOrderDialog();
                if (dialog.ShowDialog() == true)
                {
                    var order = new Order
                    {
                        ClientName = dialog.ClientName,
                        Status = "Новый",
                        TotalAmount = dialog.TotalAmount,
                        OrderDate = dialog.OrderDate,
                        Notes = dialog.Notes,
                        Priority = dialog.Priority
                    };

                    if (_orderService != null)
                    {
                        var success = await _orderService.AddOrderAsync(order);
                        if (success)
                        {
                            LoadOrders();
                            MessageBox.Show("Заказ успешно создан!", "Успех", 
                                          MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Ошибка при создании заказа", "Ошибка", 
                                          MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Заказ создан (тестовый режим)", "Информация", 
                                      MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании заказа: {ex.Message}", "Ошибка", 
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ChangeStatus_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersGrid.SelectedItem is Order selectedOrder)
            {
                var statusWindow = new OrderStatusWindow(selectedOrder);
                if (statusWindow.ShowDialog() == true)
                {
                    LoadOrders();
                }
            }
            else
            {
                MessageBox.Show("Выберите заказ для изменения статуса", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void OrderDetails_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersGrid.SelectedItem is Order selectedOrder)
            {
                MessageBox.Show($"Детали заказа №{selectedOrder.Id}\nКлиент: {selectedOrder.ClientName}\nСумма: {selectedOrder.TotalAmount:C}\nСтатус: {selectedOrder.Status}\nПриоритет: {selectedOrder.Priority}", "Детали заказа", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Выберите заказ для просмотра деталей", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}