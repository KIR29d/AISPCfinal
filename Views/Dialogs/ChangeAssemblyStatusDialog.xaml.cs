using System;
using System.Windows;
using System.Windows.Controls;
using AISPC.Models;

namespace AISPC.Views.Dialogs
{
    public partial class ChangeAssemblyStatusDialog : Window
    {
        public AssemblyTask Task { get; private set; }
        public string NewStatus { get; private set; }
        public string Comment { get; private set; }
        public DateTime? CompletionDate { get; private set; }

        public ChangeAssemblyStatusDialog(AssemblyTask task)
        {
            InitializeComponent();
            Task = task;
            LoadTaskInfo();
        }

        private void LoadTaskInfo()
        {
            TaskInfoTextBlock.Text = $"Задача #{Task.Id} - {Task.AssemblerName}\nТекущий статус: {Task.Status}";
            
            // Устанавливаем текущий статус
            for (int i = 0; i < StatusComboBox.Items.Count; i++)
            {
                if (((ComboBoxItem)StatusComboBox.Items[i]).Content.ToString() == Task.Status)
                {
                    StatusComboBox.SelectedIndex = i;
                    break;
                }
            }
            
            // Устанавливаем текущую дату для завершения
            CompletionDatePicker.SelectedDate = DateTime.Now;
        }

        private void StatusComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StatusComboBox.SelectedItem != null)
            {
                var selectedStatus = ((ComboBoxItem)StatusComboBox.SelectedItem).Content.ToString();
                
                // Показываем поле даты завершения только для статуса "Завершено"
                if (selectedStatus == "Завершено")
                {
                    CompletionDateLabel.Visibility = Visibility.Visible;
                    CompletionDatePicker.Visibility = Visibility.Visible;
                }
                else
                {
                    CompletionDateLabel.Visibility = Visibility.Collapsed;
                    CompletionDatePicker.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (StatusComboBox.SelectedItem == null)
            {
                CustomMessageBox.Show("Пожалуйста, выберите новый статус.", "Ошибка", MessageBoxImage.Warning, MessageBoxButton.OK, this);
                return;
            }

            NewStatus = ((ComboBoxItem)StatusComboBox.SelectedItem).Content.ToString();
            Comment = string.IsNullOrWhiteSpace(CommentTextBox.Text) || CommentTextBox.Text == "Введите комментарий (необязательно)" 
                ? null : CommentTextBox.Text.Trim();
            
            if (NewStatus == "Завершено")
            {
                CompletionDate = CompletionDatePicker.SelectedDate ?? DateTime.Now;
            }

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}