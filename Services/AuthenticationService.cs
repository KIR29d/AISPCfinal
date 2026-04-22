using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AISPC.Data;
using AISPC.Models;

namespace AISPC.Services
{
    public class AuthenticationService
    {
        private readonly ApplicationDbContext _context;
        private User? _currentUser;

        public AuthenticationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public User? CurrentUser => _currentUser;
        public bool IsAuthenticated => _currentUser != null;

        public async Task<AuthenticationResult> AuthenticateAsync(string login, string password)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Login == login && u.PasswordHash == password && u.IsActive);

                if (user == null)
                {
                    return new AuthenticationResult(false, "Неверный логин или пароль");
                }

                // Обновляем время последнего входа
                user.LastLogin = DateTime.Now;
                await _context.SaveChangesAsync();

                _currentUser = user;
                return new AuthenticationResult(true, "Успешная авторизация");
            }
            catch (Exception ex)
            {
                return new AuthenticationResult(false, $"Ошибка: {ex.Message}");
            }
        }

        public void Logout()
        {
            _currentUser = null;
        }
    }

    public class AuthenticationResult
    {
        public bool IsSuccess { get; }
        public string Message { get; }

        public AuthenticationResult(bool isSuccess, string message)
        {
            IsSuccess = isSuccess;
            Message = message;
        }
    }
}