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
    public partial class EmployeesPage : Page
    {
        private readonly EmployeeService _employeeService;
        private List<Employee> _allEmployees = new List<Employee>();

        public EmployeesPage()
        {
            InitializeComponent();
            _employeeService = new EmployeeService(new ApplicationDbContext());
            LoadEmployees();
        }

        private async void LoadEmployees()
        {
            try
            {
                _allEmployees = await _employeeService.GetAllEmployeesAsync();
                EmployeesGrid.ItemsSource = _allEmployees;
            }
            catch (Exception ex)
            {
                Views.Dialogs.CustomMessageBox.Show($"Ошибка загрузки сотрудников: {ex.Message}", "Ошибка", 
                              MessageBoxImage.Error, MessageBoxButton.OK);
            }
        }

        private async void AddEmployee_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Views.Dialogs.AddEmployeeDialog();
            if (dialog.ShowDialog() == true)
            {
                var employee = new Employee
                {
                    FullName = dialog.FullName,
                    Position = dialog.Position,
                    Phone = dialog.Phone,
                    Email = dialog.Email,
                    HireDate = dialog.HireDate,
                    Salary = dialog.Salary
                };

                var success = await _employeeService.AddEmployeeAsync(employee);
                if (success)
                {
                    LoadEmployees();
                    Views.Dialogs.CustomMessageBox.Show("Сотрудник успешно добавлен!", "Успех", 
                                  MessageBoxImage.Information, MessageBoxButton.OK);
                }
                else
                {
                    Views.Dialogs.CustomMessageBox.Show("Ошибка при добавлении сотрудника", "Ошибка", 
                                  MessageBoxImage.Error, MessageBoxButton.OK);
                }
            }
        }

        private async void EditEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeesGrid.SelectedItem is Employee selectedEmployee)
            {
                var dialog = new Views.Dialogs.EditEmployeeDialog(selectedEmployee);
                if (dialog.ShowDialog() == true)
                {
                    // Обновляем данные сотрудника
                    selectedEmployee.FullName = dialog.FullName;
                    selectedEmployee.Position = dialog.Position;
                    selectedEmployee.Phone = dialog.Phone;
                    selectedEmployee.Email = dialog.Email;
                    selectedEmployee.HireDate = dialog.HireDate;
                    selectedEmployee.Salary = dialog.Salary;
                    selectedEmployee.IsActive = dialog.IsEmployeeActive;

                    var success = await _employeeService.UpdateEmployeeAsync(selectedEmployee);
                    if (success)
                    {
                        LoadEmployees();
                        Views.Dialogs.CustomMessageBox.Show("Данные сотрудника успешно обновлены!", "Успех", 
                                      MessageBoxImage.Information, MessageBoxButton.OK);
                    }
                    else
                    {
                        Views.Dialogs.CustomMessageBox.Show("Ошибка при обновлении данных сотрудника", "Ошибка", 
                                      MessageBoxImage.Error, MessageBoxButton.OK);
                    }
                }
            }
            else
            {
                Views.Dialogs.CustomMessageBox.Show("Выберите сотрудника для редактирования", "Предупреждение", 
                              MessageBoxImage.Warning, MessageBoxButton.OK);
            }
        }

        private void ShowEmployeeCard_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeesGrid.SelectedItem is Employee selectedEmployee)
            {
                Views.Dialogs.CustomMessageBox.Show($"Карточка сотрудника:\n\nФИО: {selectedEmployee.FullName}\nДолжность: {selectedEmployee.Position}\nТелефон: {selectedEmployee.Phone}\nEmail: {selectedEmployee.Email}\nДата приема: {selectedEmployee.HireDate:dd.MM.yyyy}\nЗарплата: {selectedEmployee.Salary:C}", 
                              "Карточка сотрудника", MessageBoxImage.Information, MessageBoxButton.OK);
            }
            else
            {
                Views.Dialogs.CustomMessageBox.Show("Выберите сотрудника для просмотра карточки", "Предупреждение", 
                              MessageBoxImage.Warning, MessageBoxButton.OK);
            }
        }

        private void ShowSalary_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeesGrid.SelectedItem is Employee selectedEmployee)
            {
                Views.Dialogs.CustomMessageBox.Show($"Зарплата сотрудника {selectedEmployee.FullName}: {selectedEmployee.Salary:C}", 
                              "Зарплата", MessageBoxImage.Information, MessageBoxButton.OK);
            }
            else
            {
                Views.Dialogs.CustomMessageBox.Show("Выберите сотрудника для просмотра зарплаты", "Предупреждение", 
                              MessageBoxImage.Warning, MessageBoxButton.OK);
            }
        }
    }
}