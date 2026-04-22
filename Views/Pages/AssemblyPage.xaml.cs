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
    public partial class AssemblyPage : Page
    {
        private readonly AssemblyService _assemblyService;
        private List<AssemblyTask> _allAssemblyTasks = new List<AssemblyTask>();

        public AssemblyPage()
        {
            InitializeComponent();
            _assemblyService = new AssemblyService(new ApplicationDbContext());
            LoadAssemblyTasks();
        }

        private async void LoadAssemblyTasks()
        {
            try
            {
                _allAssemblyTasks = await _assemblyService.GetAllAssemblyTasksAsync();
                AssemblyGrid.ItemsSource = _allAssemblyTasks;
            }
            catch (Exception ex)
            {
                Views.Dialogs.CustomMessageBox.Show($"Ошибка загрузки задач сборки: {ex.Message}", "Ошибка", 
                              MessageBoxImage.Error, MessageBoxButton.OK);
            }
        }

        private async void AddTask_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Views.Dialogs.AddAssemblyTaskDialog();
            if (dialog.ShowDialog() == true)
            {
                var task = new AssemblyTask
                {
                    OrderId = dialog.OrderId,
                    AssemblerId = dialog.AssemblerId,
                    AssemblerName = dialog.AssemblerName,
                    Status = "Ожидает",
                    StartDate = dialog.StartDate,
                    Notes = dialog.Notes
                };

                var success = await _assemblyService.AddAssemblyTaskAsync(task);
                if (success)
                {
                    LoadAssemblyTasks();
                    Views.Dialogs.CustomMessageBox.Show("Задача сборки успешно добавлена!", "Успех", 
                                  MessageBoxImage.Information, MessageBoxButton.OK);
                }
                else
                {
                    Views.Dialogs.CustomMessageBox.Show("Ошибка при добавлении задачи", "Ошибка", 
                                  MessageBoxImage.Error, MessageBoxButton.OK);
                }
            }
        }

        private async void ChangeStatus_Click(object sender, RoutedEventArgs e)
        {
            if (AssemblyGrid.SelectedItem is AssemblyTask selectedTask)
            {
                var dialog = new Views.Dialogs.ChangeStatusDialog(selectedTask.Status);
                if (dialog.ShowDialog() == true)
                {
                    var success = await _assemblyService.UpdateTaskStatusAsync(selectedTask.Id, dialog.NewStatus);
                    if (success)
                    {
                        LoadAssemblyTasks();
                        Views.Dialogs.CustomMessageBox.Show("Статус задачи обновлен!", "Успех", 
                                      MessageBoxImage.Information, MessageBoxButton.OK);
                    }
                    else
                    {
                        Views.Dialogs.CustomMessageBox.Show("Ошибка при обновлении статуса", "Ошибка", 
                                      MessageBoxImage.Error, MessageBoxButton.OK);
                    }
                }
            }
            else
            {
                Views.Dialogs.CustomMessageBox.Show("Выберите задачу для изменения статуса", "Предупреждение", 
                              MessageBoxImage.Warning, MessageBoxButton.OK);
            }
        }

        private void ShowDetails_Click(object sender, RoutedEventArgs e)
        {
            if (AssemblyGrid.SelectedItem is AssemblyTask selectedTask)
            {
                var details = $"Детали задачи сборки:\n\n" +
                             $"№ Задачи: {selectedTask.Id}\n" +
                             $"Заказ: {selectedTask.OrderId}\n" +
                             $"Сборщик: {selectedTask.AssemblerName}\n" +
                             $"Статус: {selectedTask.Status}\n" +
                             $"Дата начала: {selectedTask.StartDate:dd.MM.yyyy}\n" +
                             $"Дата завершения: {(selectedTask.CompletionDate?.ToString("dd.MM.yyyy") ?? "Не завершено")}\n" +
                             $"Примечания: {selectedTask.Notes}";

                Views.Dialogs.CustomMessageBox.Show(details, "Детали задачи", MessageBoxImage.Information, MessageBoxButton.OK);
            }
            else
            {
                Views.Dialogs.CustomMessageBox.Show("Выберите задачу для просмотра деталей", "Предупреждение", 
                              MessageBoxImage.Warning, MessageBoxButton.OK);
            }
        }
    }
}