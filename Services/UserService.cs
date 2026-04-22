using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AISPC.Data;
using AISPC.Models;

namespace AISPC.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users
                .Include(u => u.Role)
                .OrderBy(u => u.Login)
                .ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> GetUserByLoginAsync(string login)
        {
            return await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Login == login);
        }

        public async Task<User> CreateUserAsync(User user)
        {
            // Проверяем уникальность логина
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Login == user.Login);
            
            if (existingUser != null)
            {
                throw new InvalidOperationException($"Пользователь с логином '{user.Login}' уже существует");
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            
            // Загружаем роль для возвращаемого объекта
            await _context.Entry(user)
                .Reference(u => u.Role)
                .LoadAsync();
            
            return user;
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            var existingUser = await _context.Users.FindAsync(user.Id);
            if (existingUser == null)
            {
                throw new ArgumentException($"Пользователь с ID {user.Id} не найден");
            }

            // Проверяем уникальность логина (исключая текущего пользователя)
            var duplicateUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Login == user.Login && u.Id != user.Id);
            
            if (duplicateUser != null)
            {
                throw new InvalidOperationException($"Пользователь с логином '{user.Login}' уже существует");
            }

            existingUser.Login = user.Login;
            existingUser.PasswordHash = user.PasswordHash;
            existingUser.RoleId = user.RoleId;
            existingUser.IsActive = user.IsActive;

            await _context.SaveChangesAsync();
            
            // Загружаем роль для возвращаемого объекта
            await _context.Entry(existingUser)
                .Reference(u => u.Role)
                .LoadAsync();
            
            return existingUser;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return false;
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeactivateUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return false;
            }

            user.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ActivateUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return false;
            }

            user.IsActive = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<User>> GetActiveUsersAsync()
        {
            return await _context.Users
                .Include(u => u.Role)
                .Where(u => u.IsActive)
                .OrderBy(u => u.Login)
                .ToListAsync();
        }

        public async Task<List<User>> GetUsersByRoleAsync(int roleId)
        {
            return await _context.Users
                .Include(u => u.Role)
                .Where(u => u.RoleId == roleId)
                .OrderBy(u => u.Login)
                .ToListAsync();
        }

        public async Task<bool> ValidateUserCredentialsAsync(string login, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Login == login && u.IsActive);
            
            if (user == null)
            {
                return false;
            }

            // В реальном приложении здесь должна быть проверка хеша пароля
            // Пока что простое сравнение строк
            return user.PasswordHash == password;
        }

        public async Task<User?> AuthenticateUserAsync(string login, string password)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Login == login && u.IsActive);
            
            if (user == null)
            {
                return null;
            }

            // В реальном приложении здесь должна быть проверка хеша пароля
            if (user.PasswordHash != password)
            {
                return null;
            }

            return user;
        }

        public async Task<bool> ChangePasswordAsync(int userId, string newPassword)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return false;
            }

            // В реальном приложении здесь должно быть хеширование пароля
            user.PasswordHash = newPassword;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Role>> GetAllRolesAsync()
        {
            return await _context.Roles
                .OrderBy(r => r.Name)
                .ToListAsync();
        }
    }
}