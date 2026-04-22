using System;
using System.Windows;
using System.Windows.Controls;

namespace AISPC.Views.Pages
{
    public partial class ReportsPage : Page
    {
        public ReportsPage()
        {
            InitializeComponent();
            InitializeDatePickers();
        }

        private void InitializeDatePickers()
        {
            // Устанавливаем период по умолчанию - текущий месяц
            StartDatePicker.SelectedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            EndDatePicker.SelectedDate = DateTime.Now;
        }

        private void GenerateReport_Click(object sender, RoutedEventArgs e)
        {
            var startDate = StartDatePicker.SelectedDate ?? DateTime.Now.AddMonths(-1);
            var endDate = EndDatePicker.SelectedDate ?? DateTime.Now;

            if (startDate > endDate)
            {
                MessageBox.Show("Начальная дата не может быть больше конечной даты", "Ошибка", 
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Здесь будет логика генерации отчетов
            MessageBox.Show($"Отчет сформирован за период с {startDate:dd.MM.yyyy} по {endDate:dd.MM.yyyy}", 
                          "Отчет готов", MessageBoxButton.OK, MessageBoxImage.Information);
            
            // Обновляем данные в отчетах (пока статические)
            UpdateReportData(startDate, endDate);
        }

        private void UpdateReportData(DateTime startDate, DateTime endDate)
        {
            // В реальном приложении здесь будет запрос к базе данных
            // Пока показываем статические данные с учетом периода
            
            var days = (endDate - startDate).Days + 1;
            var avgOrdersPerDay = 1.5;
            var avgOrderAmount = 27778;
            
            var estimatedOrders = (int)(days * avgOrdersPerDay);
            var estimatedRevenue = estimatedOrders * avgOrderAmount;
            
            // Обновляем отчет по продажам (это упрощенная логика для демонстрации)
            SalesReportPanel.Children.Clear();
            
            AddReportLine(SalesReportPanel, "Общая сумма продаж:", $"{estimatedRevenue:N0} ₽", "#2ECC71");
            AddReportLine(SalesReportPanel, "Количество заказов:", estimatedOrders.ToString(), null);
            AddReportLine(SalesReportPanel, "Средний чек:", $"{avgOrderAmount:N0} ₽", null);
            AddReportLine(SalesReportPanel, "Период:", $"{days} дней", null);
        }

        private void AddReportLine(StackPanel panel, string label, string value, string color)
        {
            var grid = new Grid { Margin = new Thickness(0, 0, 0, 10) };
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            
            var labelBlock = new TextBlock { Text = label };
            Grid.SetColumn(labelBlock, 0);
            
            var valueBlock = new TextBlock 
            { 
                Text = value, 
                FontWeight = FontWeights.Bold 
            };
            
            if (!string.IsNullOrEmpty(color))
            {
                valueBlock.Foreground = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(color));
            }
            
            Grid.SetColumn(valueBlock, 1);
            
            grid.Children.Add(labelBlock);
            grid.Children.Add(valueBlock);
            panel.Children.Add(grid);
        }
    }
}