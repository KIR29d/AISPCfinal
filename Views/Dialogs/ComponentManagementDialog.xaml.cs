using System;
using System.Windows;
using System.Windows.Controls;
using AISPC.Models;

namespace AISPC.Views.Dialogs
{
    public partial class ComponentManagementDialog : Window
    {
        private readonly Component? _component;
        private readonly bool _isEditMode;

        public string ComponentName => NameTextBox.Text.Trim();
        public string Category => CategoryComboBox.Text.Trim();
        public string Brand => BrandTextBox.Text.Trim();
        public string Model => ModelTextBox.Text.Trim();
        public decimal Price
        {
            get
            {
                if (decimal.TryParse(PriceTextBox.Text.Trim(), out decimal price))
                    return price;
                return 0;
            }
        }
        public int StockQuantity
        {
            get
            {
                if (int.TryParse(StockTextBox.Text.Trim(), out int stock))
                    return stock;
                return 0;
            }
        }
        public int MinStockLevel
        {
            get
            {
                if (int.TryParse(MinStockTextBox.Text.Trim(), out int minStock))
                    return minStock;
                return 5;
            }
        }
        public string Description => DescriptionTextBox.Text.Trim();
        public string Specifications => SpecificationsTextBox.Text.Trim();
        public bool IsComponentActive => IsActiveCheckBox.IsChecked ?? true;

        // Конструктор для добавления нового компонента
        public ComponentManagementDialog()
        {
            InitializeComponent();
            _isEditMode = false;
            TitleTextBlock.Text = "🛠️ Добавить новый компонент";
            SaveButton.Content = "Добавить";
        }

        // Конструктор для редактирования существующего компонента
        public ComponentManagementDialog(Component component)
        {
            InitializeComponent();
            _component = component;
            _isEditMode = true;
            TitleTextBlock.Text = "✏️ Редактировать компонент";
            SaveButton.Content = "Сохранить";
            LoadComponentData();
        }

        private void LoadComponentData()
        {
            if (_component != null)
            {
                NameTextBox.Text = _component.Name;
                CategoryComboBox.Text = _component.Category;
                BrandTextBox.Text = _component.Brand;
                ModelTextBox.Text = _component.Model;
                PriceTextBox.Text = _component.Price.ToString();
                StockTextBox.Text = _component.StockQuantity.ToString();
                MinStockTextBox.Text = _component.MinStockLevel.ToString();
                DescriptionTextBox.Text = _component.Description;
                SpecificationsTextBox.Text = _component.Specifications;
                IsActiveCheckBox.IsChecked = _component.IsActive;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInput())
            {
                DialogResult = true;
                Close();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(ComponentName))
            {
                Views.Dialogs.CustomMessageBox.Show("Введите название компонента", "Ошибка валидации", 
                              MessageBoxImage.Warning, MessageBoxButton.OK, this);
                NameTextBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(Category))
            {
                Views.Dialogs.CustomMessageBox.Show("Выберите или введите категорию", "Ошибка валидации", 
                              MessageBoxImage.Warning, MessageBoxButton.OK, this);
                CategoryComboBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(Brand))
            {
                Views.Dialogs.CustomMessageBox.Show("Введите бренд", "Ошибка валидации", 
                              MessageBoxImage.Warning, MessageBoxButton.OK, this);
                BrandTextBox.Focus();
                return false;
            }

            if (Price <= 0)
            {
                Views.Dialogs.CustomMessageBox.Show("Введите корректную цену (больше 0)", "Ошибка валидации", 
                              MessageBoxImage.Warning, MessageBoxButton.OK, this);
                PriceTextBox.Focus();
                return false;
            }

            if (StockQuantity < 0)
            {
                Views.Dialogs.CustomMessageBox.Show("Количество не может быть отрицательным", "Ошибка валидации", 
                              MessageBoxImage.Warning, MessageBoxButton.OK, this);
                StockTextBox.Focus();
                return false;
            }

            if (MinStockLevel < 0)
            {
                Views.Dialogs.CustomMessageBox.Show("Минимальный остаток не может быть отрицательным", "Ошибка валидации", 
                              MessageBoxImage.Warning, MessageBoxButton.OK, this);
                MinStockTextBox.Focus();
                return false;
            }

            return true;
        }

        public Component GetComponent()
        {
            if (_isEditMode && _component != null)
            {
                // Обновляем существующий компонент
                _component.Name = ComponentName;
                _component.Category = Category;
                _component.Brand = Brand;
                _component.Model = Model;
                _component.Price = Price;
                _component.StockQuantity = StockQuantity;
                _component.MinStockLevel = MinStockLevel;
                _component.Description = Description;
                _component.Specifications = Specifications;
                _component.IsActive = IsComponentActive;
                return _component;
            }
            else
            {
                // Создаем новый компонент
                return new Component
                {
                    Name = ComponentName,
                    Category = Category,
                    Brand = Brand,
                    Model = Model,
                    Price = Price,
                    StockQuantity = StockQuantity,
                    MinStockLevel = MinStockLevel,
                    Description = Description,
                    Specifications = Specifications,
                    IsActive = IsComponentActive
                };
            }
        }
    }
}