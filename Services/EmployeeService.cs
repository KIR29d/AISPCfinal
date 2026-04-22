using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AISPC.Data;
using AISPC.Models;
using Microsoft.EntityFrameworkCore;

namespace AISPC.Services
{
    public class EmployeeService
    {
        private readonly ApplicationDbContext _context;

        public EmployeeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Employee>> GetAllEmployeesAsync()
        {
            try
            {
                return await _context.Employees
                    .Where(e => e.IsActive)
                    .OrderBy(e => e.FullName)
                    .ToListAsync();
            }
            catch
            {
                // Возвращаем тестовые данные при ошибке подключения к БД
                return GetTestEmployees();
            }
        }

        public async Task<Employee?> GetEmployeeByIdAsync(int id)
        {
            try
            {
                return await _context.Employees.FindAsync(id);
            }
            catch
            {
                return GetTestEmployees().FirstOrDefault(e => e.Id == id);
            }
        }

        public async Task<bool> AddEmployeeAsync(Employee employee)
        {
            try
            {
                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateEmployeeAsync(Employee employee)
        {
            try
            {
                _context.Employees.Update(employee);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            try
            {
                var employee = await _context.Employees.FindAsync(id);
                if (employee != null)
                {
                    employee.IsActive = false;
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        private List<Employee> GetTestEmployees()
        {
            return new List<Employee>
            {
                new Employee { Id = 1, FullName = "Петров Алексей Иванович", Position = "Главный сборщик", Phone = "+7 (903) 111-11-11", Email = "petrov@aispc.ru", HireDate = new DateTime(2023, 1, 15), Salary = 80000m, IsActive = true },
                new Employee { Id = 2, FullName = "Сидорова Мария Петровна", Position = "Менеджер по продажам", Phone = "+7 (903) 222-22-22", Email = "sidorova@aispc.ru", HireDate = new DateTime(2023, 2, 1), Salary = 70000m, IsActive = true },
                new Employee { Id = 3, FullName = "Козлов Дмитрий Сергеевич", Position = "Кладовщик", Phone = "+7 (903) 333-33-33", Email = "kozlov@aispc.ru", HireDate = new DateTime(2023, 3, 10), Salary = 50000m, IsActive = true },
                new Employee { Id = 4, FullName = "Иванова Елена Александровна", Position = "Бухгалтер", Phone = "+7 (903) 444-44-44", Email = "ivanova@aispc.ru", HireDate = new DateTime(2023, 4, 5), Salary = 60000m, IsActive = true }
            };
        }
    }
}