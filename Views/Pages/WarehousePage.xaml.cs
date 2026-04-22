using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AISPC.Data;
using AISPC.Models;
using AISPC.Services;

namespace AISPC.Views.Pages
{
    public partial class WarehousePage : Page
    {
        private readonly WarehouseService _warehouseService;
        private List<WarehouseItem> _allWarehouseItems = new List<WarehouseItem>();

        public WarehousePage()
        {
            InitializeComponent();
            _warehouseService = new WarehouseService(new ApplicationDbContext());
            LoadWarehouseItems();
        }

        private async void LoadWarehouseItems()
        {
            try
            {
                _allWarehouseItems = await _warehouseService.GetAllWarehouseItemsAsync();
                WarehouseGrid.ItemsSource = _allWarehouseItems;
            }
            catch (Exception ex)
            {
                Views.Dialogs.CustomMessageBox.Show($"Ошибка загрузки данных склада: {ex.Message}", "Ошибка", 
                              MessageBoxImage.Error, MessageBoxButton.OK);
                LoadTestData();
            }
        }

        private void LoadTestData()
        {
            _allWarehouseItems = new List<WarehouseItem>
            {
                new WarehouseItem { Id = 1, ComponentId = 1, ComponentName = "Intel Core i7-13700K", Category = "Процессоры", Quantity = 15, MinLevel = 5, Status = "В наличии" },
                new WarehouseItem { Id = 2, ComponentId = 2, ComponentName = "AMD Ryzen 7 7700X", Category = "Процессоры", Quantity = 12, MinLevel = 5, Status = "В наличии" },
                new WarehouseItem { Id = 3, ComponentId = 3, ComponentName = "NVIDIA RTX 4070", Category = "Видеокарты", Quantity = 3, MinLevel = 5, Status = "Мало" },
                new WarehouseItem { Id = 4, ComponentId = 4, ComponentName = "Kingston Fury 32GB DDR5", Category = "Память", Quantity = 25, MinLevel = 10, Status = "В наличии" },
                new WarehouseItem { Id = 5, ComponentId = 5, ComponentName = "Samsung 980 PRO 1TB", Category = "Накопители", Quantity = 20, MinLevel = 8, Status = "В наличии" }
            };
            WarehouseGrid.ItemsSource = _allWarehouseItems;
        }

        private async void Incoming_Click(object sender, RoutedEventArgs e)
        {
            if (WarehouseGrid.SelectedItem is WarehouseItem selectedItem)
            {
                var dialog = new Views.Dialogs.WarehouseOperationDialog();
                if (dialog.ShowDialog() == true)
                {
                    // Здесь будет логика обновления склада
                    LoadWarehouseItems();
                    Views.Dialogs.CustomMessageBox.Show("Операция выполнена успешно!", "Успех", 
                                  MessageBoxImage.Information, MessageBoxButton.OK);
                }
            }
            else
            {
                Views.Dialogs.CustomMessageBox.Show("Выберите компонент для операции", "Предупреждение", 
                              MessageBoxImage.Warning, MessageBoxButton.OK);
            }
        }

        private async void WriteOff_Click(object sender, RoutedEventArgs e)
        {
            if (WarehouseGrid.SelectedItem is WarehouseItem selectedItem)
            {
                var dialog = new Views.Dialogs.WarehouseOperationDialog();
                if (dialog.ShowDialog() == true)
                {
                    // Здесь будет логика обновления склада
                    LoadWarehouseItems();
                    Views.Dialogs.CustomMessageBox.Show("Операция выполнена успешно!", "Успех", 
                                  MessageBoxImage.Information, MessageBoxButton.OK);
                }
            }
            else
            {
                Views.Dialogs.CustomMessageBox.Show("Выберите компонент для операции", "Предупреждение", 
                              MessageBoxImage.Warning, MessageBoxButton.OK);
            }
        }

        private void Movement_Click(object sender, RoutedEventArgs e)
        {
            if (WarehouseGrid.SelectedItem is WarehouseItem selectedItem)
            {
                var dialog = new Views.Dialogs.WarehouseMovementDialog(selectedItem.ComponentName);
                if (dialog.ShowDialog() == true)
                {
                    Views.Dialogs.CustomMessageBox.Show($"Перемещение выполнено:\n" +
                                  $"Компонент: {selectedItem.ComponentName}\n" +
                                  $"Откуда: {dialog.FromLocation}\n" +
                                  $"Куда: {dialog.ToLocation}\n" +
                                  $"Количество: {dialog.Quantity} шт.", 
                                  "Перемещение завершено", 
                                  MessageBoxImage.Information, MessageBoxButton.OK);
                    
                    LoadWarehouseItems();
                }
            }
            else
            {
                Views.Dialogs.CustomMessageBox.Show("Выберите компонент для перемещения", "Предупреждение", 
                              MessageBoxImage.Warning, MessageBoxButton.OK);
            }
        }

        private void Inventory_Click(object sender, RoutedEventArgs e)
        {
            var lowStockItems = _allWarehouseItems.Where(w => w.Quantity <= w.MinLevel).ToList();
            
            if (lowStockItems.Any())
            {
                var message = "Компоненты, требующие пополнения:\n\n";
                foreach (var item in lowStockItems)
                {
                    message += $"• {item.ComponentName}: {item.Quantity} шт. (мин. {item.MinLevel})\n";
                }
                
                Views.Dialogs.CustomMessageBox.Show(message, "Инвентаризация", MessageBoxImage.Warning, MessageBoxButton.OK);
            }
            else
            {
                Views.Dialogs.CustomMessageBox.Show("Все компоненты в достаточном количестве", "Инвентаризация", 
                              MessageBoxImage.Information, MessageBoxButton.OK);
            }
        }
    }
}