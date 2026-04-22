using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AISPC.Data;
using AISPC.Models;
using Microsoft.EntityFrameworkCore;

namespace AISPC.Services
{
    public class AssemblyService
    {
        private readonly ApplicationDbContext _context;

        public AssemblyService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<AssemblyTask>> GetAllAssemblyTasksAsync()
        {
            try
            {
                return await _context.AssemblyTasks
                    .Include(a => a.Order)
                    .Include(a => a.Assembler)
                    .OrderBy(a => a.StartDate)
                    .ToListAsync();
            }
            catch
            {
                return GetTestAssemblyTasks();
            }
        }

        public async Task<AssemblyTask?> GetAssemblyTaskByIdAsync(int id)
        {
            try
            {
                return await _context.AssemblyTasks
                    .Include(a => a.Order)
                    .Include(a => a.Assembler)
                    .FirstOrDefaultAsync(a => a.Id == id);
            }
            catch
            {
                return GetTestAssemblyTasks().FirstOrDefault(a => a.Id == id);
            }
        }

        public async Task<bool> AddAssemblyTaskAsync(AssemblyTask task)
        {
            try
            {
                _context.AssemblyTasks.Add(task);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateTaskStatusAsync(int taskId, string newStatus)
        {
            try
            {
                var task = await _context.AssemblyTasks.FindAsync(taskId);
                if (task != null)
                {
                    task.Status = newStatus;
                    if (newStatus == "Завершено")
                    {
                        task.CompletionDate = DateTime.Now;
                    }
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

        private List<AssemblyTask> GetTestAssemblyTasks()
        {
            return new List<AssemblyTask>
            {
                new AssemblyTask { Id = 1, OrderId = 1, AssemblerId = 1, AssemblerName = "Петров Алексей Иванович", Status = "В работе", StartDate = DateTime.Now.AddDays(-2), Notes = "Игровой ПК высокого класса" },
                new AssemblyTask { Id = 2, OrderId = 2, AssemblerId = 1, AssemblerName = "Петров Алексей Иванович", Status = "Ожидает", StartDate = DateTime.Now.AddDays(-1), Notes = "Офисный компьютер" },
                new AssemblyTask { Id = 3, OrderId = 3, AssemblerId = 2, AssemblerName = "Козлов Дмитрий Сергеевич", Status = "Завершено", StartDate = DateTime.Now.AddDays(-5), CompletionDate = DateTime.Now.AddDays(-1), Notes = "Рабочая станция для дизайна" },
                new AssemblyTask { Id = 4, OrderId = 4, AssemblerId = 1, AssemblerName = "Петров Алексей Иванович", Status = "Ожидает", StartDate = DateTime.Now, Notes = "Сервер для малого бизнеса" }
            };
        }
    }
}