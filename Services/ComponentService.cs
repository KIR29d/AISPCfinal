using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AISPC.Data;
using AISPC.Models;

namespace AISPC.Services
{
    public class ComponentService
    {
        private readonly ApplicationDbContext _context;

        public ComponentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Component>> GetAllComponentsAsync()
        {
            return await _context.Components
                .Where(c => c.IsActive)
                .OrderBy(c => c.Category ?? "")
                .ThenBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Component> GetComponentByIdAsync(int id)
        {
            return await _context.Components.FindAsync(id);
        }

        public async Task<Component> CreateComponentAsync(Component component)
        {
            _context.Components.Add(component);
            await _context.SaveChangesAsync();
            return component;
        }

        public async Task<bool> AddComponentAsync(Component component)
        {
            try
            {
                _context.Components.Add(component);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateComponentAsync(Component component)
        {
            try
            {
                _context.Components.Update(component);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteComponentAsync(int id)
        {
            var component = await _context.Components.FindAsync(id);
            if (component == null) return false;

            component.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Component>> SearchComponentsAsync(string searchTerm)
        {
            return await _context.Components
                .Where(c => c.IsActive && 
                           (c.Name.Contains(searchTerm) || 
                            (c.Brand != null && c.Brand.Contains(searchTerm)) || 
                            (c.Model != null && c.Model.Contains(searchTerm)) ||
                            (c.Category != null && c.Category.Contains(searchTerm))))
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<List<Component>> GetLowStockComponentsAsync()
        {
            return await _context.Components
                .Where(c => c.IsActive && c.StockQuantity <= c.MinStockLevel)
                .OrderBy(c => c.StockQuantity)
                .ToListAsync();
        }

        public async Task<bool> UpdateStockAsync(int componentId, int newQuantity)
        {
            var component = await _context.Components.FindAsync(componentId);
            if (component == null) return false;

            component.StockQuantity = newQuantity;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<string>> GetCategoriesAsync()
        {
            return await _context.Components
                .Where(c => c.IsActive && !string.IsNullOrEmpty(c.Category))
                .Select(c => c.Category)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();
        }

        public async Task<List<string>> GetBrandsAsync()
        {
            return await _context.Components
                .Where(c => c.IsActive && !string.IsNullOrEmpty(c.Brand))
                .Select(c => c.Brand)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();
        }
    }
}