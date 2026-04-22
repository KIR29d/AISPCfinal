using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AISPC.Models;
using AISPC.Services;

namespace AISPC.Views.Dialogs
{
    public partial class WarehouseOperationDialog : Window
    {
        public string OperationType { get; private set; } = "";
        public Component? SelectedComponent { get; private set; }
        public int Quantity { get; private set; }
        public string Reason { get; private set; } = "";
        public DateTime OperationDate { get; private set; }
        public string Responsible { get; private set; } = "";

        private readonly ComponentService _componentService;
        private List<Component> _components = new List<Component>();

        public WarehouseOperationDialog()
        {
            InitializeComponent();
            _componentService = new ComponentService(new Data.ApplicationDbContext());
            OperationDatePicker.SelectedDate = DateTime.Now;
            LoadComponents();
            UpdatePreview();
        }

        private async void LoadComponents()
        {
            try
            {
                _components = await _componentService.GetAllComponentsAsync();
                ComponentComboBox.ItemsSource = _components;
            }
            catch (Exception ex)
            {
                Views.Dialogs.CustomMessageBox.Show($"Ошибка загрузки компонентов: {ex.Message}", "Ошибка", 
                              MessageBoxImage.Error, MessageBoxButton.OK, this);
                
                // Загружаем тестовые данные
                _components = new List<Component>
                {
                    new Component { Id = 1, Name = "Intel Core i7-13700K", StockQuantity = 15, Category = "Процессоры" },
                    new Component { Id = 2, Name = "AMD Ryzen 7 7700X", StockQuantity = 12, Category = "Процессоры" },
                    new Component { Id = 3, Name = "NVIDIA RTX 4070", StockQuantity = 8, Category = "Видеокарты" }
                };
                ComponentComboBox.ItemsSource = _components;
            }
        }

        private void OperationTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateCurrentStock();
            UpdatePreview();
        }

        private void ComponentComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateCurrentStock();
            UpdatePreview();
        }

        private void QuantityTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdatePreview();
        }

        private void UpdateCurrentStock()
        {
            if (ComponentComboBox.SelectedItem is Component component)
            {
                CurrentStockTextBlock.Text = $"Название: {component.Name}\n" +
                                           $"Категория: {component.Category ?? "Не указана"}\n" +
                                           $"Текущий остаток: {component.StockQuantity} шт.\n" +
                                           $"Минимальный остаток: {component.MinStockLevel} шт.";
                SelectedComponent = component;
            }
            else
            {
                CurrentStockTextBlock.Text = "Выберите компонент для просмотра информации";
                SelectedComponent = null;
            }
        }

        private void UpdatePreview()
        {
            try
            {
                var operationType = GetOperationType();
                var componentName = SelectedComponent?.Name ?? "Не выбран";
                var quantity = GetQuantityValue();
                var responsible = GetResponsibleValue();
                var reason = GetReasonValue();
                var date = OperationDatePicker.SelectedDate?.ToString("dd.MM.yyyy") ?? DateTime.Now.ToString("dd.MM.yyyy");

                PreviewText.Text = $"Операция: {operationType}\n" +
                                 $"Компонент: {componentName}\n" +
                                 $"Количество: {(quantity > 0 ? quantity.ToString() : "Не указано")} шт.\n" +
                                 $"Дата: {date}\n" +
                                 $"Ответственный: {(string.IsNullOrEmpty(responsible) ? "Не указан" : responsible)}\n" +
                                 $"Комментарий: {(string.IsNullOrEmpty(reason) ? "Не указан" : reason)}";
            }
            catch
            {
                PreviewText.Text = "Ошибка в данных операции";
            }
        }

        private string GetOperationType()
        {
            if (OperationTypeComboBox.SelectedItem is ComboBoxItem item)
            {
                return item.Content.ToString()?.Replace("📥 ", "").Replace("📤 ", "").Replace("🔄 ", "").Replace("📊 ", "") ?? "Не выбрана";
            }
            return "Не выбрана";
        }

        private int GetQuantityValue()
        {
            return int.TryParse(QuantityTextBox.Text, out int quantity) ? quantity : 0;
        }

        private string GetResponsibleValue()
        {
            return string.IsNullOrWhiteSpace(ResponsibleTextBox.Text) || ResponsibleTextBox.Text == "Введите ФИО ответственного" 
                   ? "" : ResponsibleTextBox.Text.Trim();
        }

        private string GetReasonValue()
        {
            return string.IsNullOrWhiteSpace(ReasonTextBox.Text) || ReasonTextBox.Text == "Укажите причину или комментарий к операции" 
                   ? "" : ReasonTextBox.Text.Trim();
        }

        // Обработчики для placeholder'ов
        private void ResponsibleTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (ResponsibleTextBox.Text == "Введите ФИО ответственного")
            {
                ResponsibleTextBox.Text = "";
                ResponsibleTextBox.Foreground = System.Windows.Media.Brushes.White;
            }
        }

        private void ResponsibleTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ResponsibleTextBox.Text))
            {
                ResponsibleTextBox.Text = "Введите ФИО ответственного";
                ResponsibleTextBox.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(201, 209, 217));
            }
            UpdatePreview();
        }

        private void ReasonTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (ReasonTextBox.Text == "Укажите причину или комментарий к операции")
            {
                ReasonTextBox.Text = "";
                ReasonTextBox.Foreground = System.Windows.Media.Brushes.White;
            }
        }

        private void ReasonTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ReasonTextBox.Text))
            {
                ReasonTextBox.Text = "Укажите причину или комментарий к операции";
                ReasonTextBox.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(201, 209, 217));
            }
            UpdatePreview();
        }

        private void Execute_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Валидация
                if (OperationTypeComboBox.SelectedItem == null)
                {
                    Views.Dialogs.CustomMessageBox.Show("Пожалуйста, выберите тип операции.", "Ошибка валидации", 
                                  MessageBoxImage.Warning, MessageBoxButton.OK, this);
                    OperationTypeComboBox.Focus();
                    return;
                }

                if (ComponentComboBox.SelectedItem == null)
                {
                    Views.Dialogs.CustomMessageBox.Show("Пожалуйста, выберите компонент.", "Ошибка валидации", 
                                  MessageBoxImage.Warning, MessageBoxButton.OK, this);
                    ComponentComboBox.Focus();
                    return;
                }

                if (!int.TryParse(QuantityTextBox.Text, out int quantity) || quantity <= 0)
                {
                    Views.Dialogs.CustomMessageBox.Show("Пожалуйста, введите корректное количество (больше 0).", "Ошибка валидации", 
                                  MessageBoxImage.Warning, MessageBoxButton.OK, this);
                    QuantityTextBox.Focus();
                    return;
                }

                var responsibleValue = GetResponsibleValue();
                if (string.IsNullOrEmpty(responsibleValue))
                {
                    Views.Dialogs.CustomMessageBox.Show("Пожалуйста, укажите ответственного за операцию.", "Ошибка валидации", 
                                  MessageBoxImage.Warning, MessageBoxButton.OK, this);
                    ResponsibleTextBox.Focus();
                    return;
                }

                OperationType = GetOperationType();
                SelectedComponent = (Component)ComponentComboBox.SelectedItem;
                Quantity = quantity;
                Reason = GetReasonValue();
                OperationDate = OperationDatePicker.SelectedDate ?? DateTime.Now;
                Responsible = responsibleValue;

                // Проверяем достаточность остатков для списания
                if (OperationType == "Списание" && SelectedComponent.StockQuantity < Quantity)
                {
                    var result = Views.Dialogs.CustomMessageBox.Show(
                        $"Недостаточно товара на складе!\n\nДоступно: {SelectedComponent.StockQuantity} шт.\nЗапрошено: {Quantity} шт.\n\nВсе равно продолжить операцию?",
                        "Предупреждение о недостатке товара", MessageBoxImage.Warning, MessageBoxButton.YesNo, this);
                    
                    if (result != MessageBoxResult.Yes)
                        return;
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                Views.Dialogs.CustomMessageBox.Show($"Ошибка при выполнении операции: {ex.Message}", "Ошибка", 
                              MessageBoxImage.Error, MessageBoxButton.OK, this);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}