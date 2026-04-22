using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AISPC.Models;
using AISPC.Data;
using AISPC.Services;

namespace AISPC.Views.Dialogs
{
    public partial class AddComponentDialog : Window
    {
        private readonly ComponentService? _componentService;

        public AddComponentDialog()
        {
            InitializeComponent();
            
            try
            {
                var context = new ApplicationDbContext();
                _componentService = new ComponentService(context);
            }
            catch
            {
                // Если не удается подключиться к БД, работаем без сервиса
            }
            
            UpdatePreview();
            
            // Подписываемся на изменения для обновления превью
            NameTextBox.TextChanged += (s, e) => UpdatePreview();
            CategoryComboBox.SelectionChanged += (s, e) => UpdatePreview();
            BrandTextBox.TextChanged += (s, e) => UpdatePreview();
            PriceTextBox.TextChanged += (s, e) => UpdatePreview();
            StockQuantityTextBox.TextChanged += (s, e) => UpdatePreview();
        }

        private void UpdatePreview()
        {
            try
            {
                var name = string.IsNullOrWhiteSpace(NameTextBox.Text) ? "Не указано" : NameTextBox.Text;
                var category = CategoryComboBox.Text ?? "Не указана";
                var brand = string.IsNullOrWhiteSpace(BrandTextBox.Text) ? "Не указан" : BrandTextBox.Text;
                var price = decimal.TryParse(PriceTextBox.Text, out var p) ? p.ToString("C") : "0 ₽";
                var quantity = int.TryParse(StockQuantityTextBox.Text, out var q) ? q.ToString() : "0";
                
                PreviewText.Text = $"Название: {name}\n" +
                                 $"Категория: {category}\n" +
                                 $"Бренд: {brand}\n" +
                                 $"Цена: {price}\n" +
                                 $"Количество: {quantity} шт.";
            }
            catch
            {
                PreviewText.Text = "Ошибка в данных компонента";
            }
        }

        private async void AddComponent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Валидация данных
                if (string.IsNullOrWhiteSpace(NameTextBox.Text) || NameTextBox.Text == "Введите название компонента")
                {
                    MessageBox.Show("Введите название компонента", "Ошибка валидации", 
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    NameTextBox.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(CategoryComboBox.Text) || CategoryComboBox.Text == "Выберите или введите категорию")
                {
                    MessageBox.Show("Выберите или введите категорию", "Ошибка валидации", 
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    CategoryComboBox.Focus();
                    return;
                }

                var priceText = GetTextBoxValue(PriceTextBox, "Введите цену в рублях");
                if (!decimal.TryParse(priceText, out var price) || price <= 0)
                {
                    MessageBox.Show("Введите корректную цену", "Ошибка валидации", 
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    PriceTextBox.Focus();
                    return;
                }

                var stockText = GetTextBoxValue(StockQuantityTextBox, "Введите количество");
                if (!int.TryParse(stockText, out var stockQuantity) || stockQuantity < 0)
                {
                    MessageBox.Show("Введите корректное количество", "Ошибка валидации", 
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    StockQuantityTextBox.Focus();
                    return;
                }

                var minStockText = GetTextBoxValue(MinStockLevelTextBox, "Минимальный остаток");
                if (!int.TryParse(minStockText, out var minStockLevel) || minStockLevel < 0)
                {
                    MessageBox.Show("Введите корректный минимальный остаток", "Ошибка валидации", 
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    MinStockLevelTextBox.Focus();
                    return;
                }

                // Создаем новый компонент
                var component = new Component
                {
                    Name = NameTextBox.Text.Trim(),
                    Category = CategoryComboBox.Text.Trim(),
                    Brand = GetTextBoxValue(BrandTextBox, "Введите бренд производителя"),
                    Model = GetTextBoxValue(ModelTextBox, "Введите модель компонента"),
                    Price = price,
                    StockQuantity = stockQuantity,
                    MinStockLevel = minStockLevel,
                    Description = GetTextBoxValue(DescriptionTextBox, "Введите описание компонента (необязательно)"),
                    Specifications = "",
                    IsActive = true,
                    CreatedDate = DateTime.Now
                };

                // Сохраняем в базу данных
                if (_componentService != null)
                {
                    await _componentService.CreateComponentAsync(component);
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении компонента: {ex.Message}", "Ошибка", 
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GetTextBoxValue(TextBox textBox, string placeholder)
        {
            return string.IsNullOrWhiteSpace(textBox.Text) || textBox.Text == placeholder ? "" : textBox.Text.Trim();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        // Обработчики для placeholder'ов
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                var placeholders = new[]
                {
                    "Введите название компонента",
                    "Введите бренд производителя",
                    "Введите модель компонента",
                    "Введите цену в рублях",
                    "Введите количество",
                    "Минимальный остаток",
                    "Введите описание компонента (необязательно)"
                };

                if (placeholders.Contains(textBox.Text))
                {
                    textBox.Text = "";
                    textBox.Foreground = System.Windows.Media.Brushes.White;
                }
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null && string.IsNullOrWhiteSpace(textBox.Text))
            {
                var placeholder = "";
                if (textBox == NameTextBox) placeholder = "Введите название компонента";
                else if (textBox == BrandTextBox) placeholder = "Введите бренд производителя";
                else if (textBox == ModelTextBox) placeholder = "Введите модель компонента";
                else if (textBox == PriceTextBox) placeholder = "Введите цену в рублях";
                else if (textBox == StockQuantityTextBox) placeholder = "Введите количество";
                else if (textBox == MinStockLevelTextBox) placeholder = "Минимальный остаток";
                else if (textBox == DescriptionTextBox) placeholder = "Введите описание компонента (необязательно)";

                textBox.Text = placeholder;
                textBox.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(139, 148, 158));
            }
        }
    }
}