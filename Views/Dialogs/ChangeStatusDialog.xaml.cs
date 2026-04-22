using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AISPC.Views.Dialogs
{
    public partial class ChangeStatusDialog : Window
    {
        public string NewStatus { get; private set; } = "";
        public string Comment { get; private set; } = "";
        public DateTime? CompletionDate { get; private set; }

        public ChangeStatusDialog(string currentStatus)
        {
            InitializeComponent();
            
            CurrentStatusText.Text = currentStatus;
            CompletionDatePicker.SelectedDate = DateTime.Now;
            
            // Устанавливаем цвет текущего статуса
            SetStatusColor(CurrentStatusText, currentStatus);
            
            UpdatePreview();
        }

        private void SetStatusColor(TextBlock textBlock, string status)
        {
            var color = status switch
            {
                "Ожидает" => "#FFA657",
                "В работе" => "#58A6FF", 
                "Приостановлено" => "#F85149",
                "Завершено" => "#3FB950",
                "Отменено" => "#8B949E",
                _ => "#C9D1D9"
            };
            
            textBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
        }

        private void StatusComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdatePreview();
            
            // Автоматически включаем дату завершения для статуса "Завершено"
            if (StatusComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                var status = selectedItem.Content.ToString();
                if (status == "Завершено")
                {
                    SetCompletionDateCheckBox.IsChecked = true;
                    CompletionDatePicker.IsEnabled = true;
                }
            }
        }

        private void SetCompletionDateCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CompletionDatePicker.IsEnabled = true;
            UpdatePreview();
        }

        private void SetCompletionDateCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CompletionDatePicker.IsEnabled = false;
            UpdatePreview();
        }

        private void CommentTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (CommentTextBox.Text == "Укажите причину изменения статуса (необязательно)")
            {
                CommentTextBox.Text = "";
                CommentTextBox.Foreground = Brushes.White;
            }
        }

        private void CommentTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CommentTextBox.Text))
            {
                CommentTextBox.Text = "Укажите причину изменения статуса (необязательно)";
                CommentTextBox.Foreground = new SolidColorBrush(Color.FromRgb(201, 209, 217));
            }
        }

        private void UpdatePreview()
        {
            try
            {
                var selectedStatus = (StatusComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Не выбран";
                var comment = GetCommentValue();
                var completionDate = SetCompletionDateCheckBox.IsChecked == true && CompletionDatePicker.SelectedDate.HasValue 
                    ? CompletionDatePicker.SelectedDate.Value.ToString("dd.MM.yyyy") 
                    : "Не установлена";
                
                PreviewText.Text = $"Новый статус: {selectedStatus}\n" +
                                 $"Комментарий: {(string.IsNullOrEmpty(comment) ? "Не указан" : comment)}\n" +
                                 $"Дата завершения: {completionDate}";
            }
            catch
            {
                PreviewText.Text = "Ошибка в данных изменения статуса";
            }
        }

        private string GetCommentValue()
        {
            return string.IsNullOrWhiteSpace(CommentTextBox.Text) || 
                   CommentTextBox.Text == "Укажите причину изменения статуса (необязательно)" 
                   ? "" : CommentTextBox.Text.Trim();
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Валидация
                if (StatusComboBox.SelectedItem == null)
                {
                    Views.Dialogs.CustomMessageBox.Show("Выберите новый статус для задачи", "Ошибка валидации", 
                                  MessageBoxImage.Warning, MessageBoxButton.OK, this);
                    StatusComboBox.Focus();
                    return;
                }

                var selectedStatus = (StatusComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
                
                // Проверяем, что статус действительно изменился
                if (selectedStatus == CurrentStatusText.Text)
                {
                    Views.Dialogs.CustomMessageBox.Show("Выберите статус, отличный от текущего", "Предупреждение", 
                                  MessageBoxImage.Warning, MessageBoxButton.OK, this);
                    StatusComboBox.Focus();
                    return;
                }

                NewStatus = selectedStatus ?? "";
                Comment = GetCommentValue();
                CompletionDate = SetCompletionDateCheckBox.IsChecked == true ? CompletionDatePicker.SelectedDate : null;

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                Views.Dialogs.CustomMessageBox.Show($"Ошибка при применении изменений: {ex.Message}", "Ошибка", 
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