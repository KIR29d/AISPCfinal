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
    public partial class ComponentsPage : Page
    {
        private readonly ComponentService _componentService;
        private List<Component> _allComponents = new List<Component>();

        public ComponentsPage()
        {
            InitializeComponent();
            _componentService = new ComponentService(new ApplicationDbContext());
            LoadComponents();
        }

        private async void LoadComponents()
        {
            try
            {
                _allComponents = await _componentService.GetAllComponentsAsync();
                ComponentsGrid.ItemsSource = _allComponents;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки компонентов: {ex.Message}", "Ошибка", 
                              MessageBoxButton.OK, MessageBoxImage.Error);
                LoadTestData();
            }
        }

        private void LoadTestData()
        {
            _allComponents = new List<Component>
            {
                new Component { Id = 1, Name = "Intel Core i7-13700K", Category = "Процессоры", Brand = "Intel", Model = "i7-13700K", Price = 35000m, StockQuantity = 15, MinStockLevel = 5, Description = "16-ядерный процессор" },
                new Component { Id = 2, Name = "AMD Ryzen 7 7700X", Category = "Процессоры", Brand = "AMD", Model = "7700X", Price = 32000m, StockQuantity = 12, MinStockLevel = 5, Description = "8-ядерный процессор" },
                new Component { Id = 3, Name = "NVIDIA RTX 4070", Category = "Видеокарты", Brand = "NVIDIA", Model = "RTX 4070", Price = 65000m, StockQuantity = 8, MinStockLevel = 3, Description = "Игровая видеокарта" },
                new Component { Id = 4, Name = "Kingston Fury 32GB DDR5", Category = "Память", Brand = "Kingston", Model = "Fury DDR5-5600", Price = 12000m, StockQuantity = 25, MinStockLevel = 10, Description = "Оперативная память" },
                new Component { Id = 5, Name = "Samsung 980 PRO 1TB", Category = "Накопители", Brand = "Samsung", Model = "980 PRO", Price = 8500m, StockQuantity = 20, MinStockLevel = 8, Description = "NVMe SSD накопитель" }
            };
            ComponentsGrid.ItemsSource = _allComponents;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            var searchText = SearchBox.Text.ToLower();
            if (string.IsNullOrWhiteSpace(searchText) || searchText == "поиск компонентов...")
            {
                ComponentsGrid.ItemsSource = _allComponents;
            }
            else
            {
                var filtered = _allComponents.Where(c => 
                    c.Name.ToLower().Contains(searchText) ||
                    (c.Category?.ToLower().Contains(searchText) ?? false) ||
                    (c.Brand?.ToLower().Contains(searchText) ?? false) ||
                    (c.Model?.ToLower().Contains(searchText) ?? false)).ToList();
                ComponentsGrid.ItemsSource = filtered;
            }
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Views.Dialogs.ComponentManagementDialog();
            if (dialog.ShowDialog() == true)
            {
                var component = new Component
                {
                    Name = dialog.ComponentName,
                    Category = dialog.Category,
                    Brand = dialog.Brand,
                    Model = dialog.Model,
                    Price = dialog.Price,
                    StockQuantity = dialog.StockQuantity,
                    MinStockLevel = dialog.MinStockLevel,
                    Description = dialog.Description,
                    Specifications = dialog.Specifications,
                    IsActive = dialog.IsComponentActive
                };

                var success = await _componentService.AddComponentAsync(component);
                if (success)
                {
                    LoadComponents();
                    MessageBox.Show("Компонент успешно добавлен!", "Успех", 
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Ошибка при добавлении компонента", "Ошибка", 
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (ComponentsGrid.SelectedItem is Component selectedComponent)
            {
                var dialog = new Views.Dialogs.ComponentManagementDialog(selectedComponent);
                if (dialog.ShowDialog() == true)
                {
                    // Обновляем данные компонента
                    selectedComponent.Name = dialog.ComponentName;
                    selectedComponent.Category = dialog.Category;
                    selectedComponent.Brand = dialog.Brand;
                    selectedComponent.Model = dialog.Model;
                    selectedComponent.Price = dialog.Price;
                    selectedComponent.StockQuantity = dialog.StockQuantity;
                    selectedComponent.MinStockLevel = dialog.MinStockLevel;
                    selectedComponent.Description = dialog.Description;
                    selectedComponent.Specifications = dialog.Specifications;
                    selectedComponent.IsActive = dialog.IsComponentActive;

                    var success = await _componentService.UpdateComponentAsync(selectedComponent);
                    if (success)
                    {
                        LoadComponents();
                        MessageBox.Show("Компонент успешно обновлен!", "Успех", 
                                      MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при обновлении компонента", "Ошибка", 
                                      MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите компонент для редактирования", "Предупреждение", 
                              MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (ComponentsGrid.SelectedItem is Component selectedComponent)
            {
                var result = MessageBox.Show($"Вы уверены, что хотите удалить компонент '{selectedComponent.Name}'?", 
                                           "Подтверждение удаления", 
                                           MessageBoxButton.YesNo, 
                                           MessageBoxImage.Question);
                
                if (result == MessageBoxResult.Yes)
                {
                    var success = await _componentService.DeleteComponentAsync(selectedComponent.Id);
                    if (success)
                    {
                        LoadComponents();
                        MessageBox.Show("Компонент успешно удален!", "Успех", 
                                      MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при удалении компонента", "Ошибка", 
                                      MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите компонент для удаления", "Предупреждение", 
                              MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SearchBox.Text == "Поиск компонентов...")
            {
                SearchBox.Text = "";
                SearchBox.Foreground = System.Windows.Media.Brushes.White;
            }
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchBox.Text))
            {
                SearchBox.Text = "Поиск компонентов...";
                SearchBox.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Автоматический поиск при изменении текста (если это не placeholder)
            if (SearchBox.Text != "Поиск компонентов...")
            {
                SearchButton_Click(sender, e);
            }
        }
    }
}