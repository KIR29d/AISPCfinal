using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AISPC.Data;
using AISPC.Models;

namespace AISPC.Services
{
    public class ClientService
    {
        private readonly ApplicationDbContext _context;

        public ClientService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Client>> GetAllClientsAsync()
        {
            return await _context.Clients
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Client?> GetClientByIdAsync(int id)
        {
            return await _context.Clients
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Client> CreateClientAsync(Client client)
        {
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();
            return client;
        }

        public async Task<Client> UpdateClientAsync(Client client)
        {
            var existingClient = await _context.Clients.FindAsync(client.Id);
            if (existingClient == null)
            {
                throw new ArgumentException($"Клиент с ID {client.Id} не найден");
            }

            existingClient.Name = client.Name;
            existingClient.ClientType = client.ClientType;
            existingClient.Phone = client.Phone;
            existingClient.Email = client.Email;
            existingClient.Address = client.Address;
            existingClient.INN = client.INN;
            existingClient.Notes = client.Notes;
            existingClient.IsActive = client.IsActive;

            await _context.SaveChangesAsync();
            return existingClient;
        }

        public async Task<bool> DeleteClientAsync(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return false;
            }

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeactivateClientAsync(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return false;
            }

            client.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ActivateClientAsync(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return false;
            }

            client.IsActive = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Client>> GetActiveClientsAsync()
        {
            return await _context.Clients
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<List<Client>> SearchClientsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetAllClientsAsync();
            }

            var term = searchTerm.ToLower();
            return await _context.Clients
                .Where(c => c.Name.ToLower().Contains(term) ||
                           c.Phone.ToLower().Contains(term) ||
                           c.Email.ToLower().Contains(term) ||
                           c.INN.ToLower().Contains(term))
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<List<Client>> GetClientsByTypeAsync(string clientType)
        {
            return await _context.Clients
                .Where(c => c.ClientType == clientType)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }
    }
}