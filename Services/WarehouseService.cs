using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AISPC.Data;
using AISPC.Models;

namespace AISPC.Services
{
    public class WarehouseService
    {
        private readonly ApplicationDbContext _context;

        public WarehouseService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<WarehouseItem>> GetAllWarehouseItemsAsync()
        {
            return await _context.WarehouseItems
                .Include(w => w.Component)
                .OrderBy(w => w.ComponentName)
                .ToListAsync();
        }

        public async Task<WarehouseItem> GetWarehouseItemByIdAsync(int id)
        {
            return await _context.WarehouseItems
                .Include(w => w.Component)
                .FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task<WarehouseItem> GetWarehouseItemByComponentIdAsync(int componentId)
        {
            return await _context.WarehouseItems
                .Include(w => w.Component)
                .FirstOrDefaultAsync(w => w.ComponentId == componentId);
        }

        public async Task<bool> UpdateStockAsync(int componentId, int quantity, string operationType)
        {
            try
            {
                var warehouseItem = await GetWarehouseItemByComponentIdAsync(componentId);
                if (warehouseItem == null)
                {
                    // Создаем новую запись склада если её нет
                    var componentData = await _context.Components.FindAsync(componentId);
                    if (componentData == null) return false;

                    warehouseItem = new WarehouseItem
                    {
                        ComponentId = componentId,
                        ComponentName = componentData.Name,
                        Category = componentData.Category ?? "Без категории",
                        Quantity = 0,
                        MinLevel = componentData.MinStockLevel,
                        Status = "В наличии"
                    };
                    _context.WarehouseItems.Add(warehouseItem);
                }

                // Обновляем количество в зависимости от типа операции
                switch (operationType.ToLower())
                {
                    case "поступление":
                        warehouseItem.Quantity += quantity;
                        break;
                    case "списание":
                        warehouseItem.Quantity -= quantity;
                        if (warehouseItem.Quantity < 0) warehouseItem.Quantity = 0;
                        break;
                    case "инвентаризация":
                        warehouseItem.Quantity = quantity;
                        break;
                }

                // Обновляем статус
                warehouseItem.Status = warehouseItem.Quantity <= warehouseItem.MinLevel ? "Мало" : "В наличии";

                // Также обновляем количество в таблице компонентов
                var component = await _context.Components.FindAsync(componentId);
                if (component != null)
                {
                    component.StockQuantity = warehouseItem.Quantity;
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CreateWarehouseItemAsync(WarehouseItem item)
        {
            try
            {
                _context.WarehouseItems.Add(item);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateWarehouseItemAsync(WarehouseItem item)
        {
            try
            {
                _context.WarehouseItems.Update(item);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteWarehouseItemAsync(int id)
        {
            try
            {
                var item = await _context.WarehouseItems.FindAsync(id);
                if (item == null) return false;

                _context.WarehouseItems.Remove(item);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<WarehouseItem>> GetLowStockItemsAsync()
        {
            return await _context.WarehouseItems
                .Where(w => w.Quantity <= w.MinLevel)
                .OrderBy(w => w.Quantity)
                .ToListAsync();
        }

        public async Task<List<WarehouseItem>> SearchWarehouseItemsAsync(string searchTerm)
        {
            return await _context.WarehouseItems
                .Where(w => w.ComponentName.Contains(searchTerm) || 
                           (w.Category != null && w.Category.Contains(searchTerm)))
                .OrderBy(w => w.ComponentName)
                .ToListAsync();
        }
    }
}