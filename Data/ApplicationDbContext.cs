using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using AISPC.Models;

namespace AISPC.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public ApplicationDbContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                try
                {
                    // Пробуем подключиться к MySQL
                    optionsBuilder.UseMySql("Server=100.123.222.38;Port=3306;Database=custom_pc_shop;User Id=Amon;Password=80012799;SslMode=None;", 
                        new MySqlServerVersion(new Version(8, 0, 21)));
                }
                catch
                {
                    // Если MySQL недоступен, используем SQLite как fallback
                    optionsBuilder.UseSqlite("Data Source=aispc_local.db");
                }
            }
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Component> Components { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<WarehouseItem> WarehouseItems { get; set; }
        public DbSet<AssemblyTask> AssemblyTasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Настройка User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Login).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.HasIndex(e => e.Login).IsUnique();
                
                entity.HasOne(e => e.Role)
                    .WithMany(r => r.Users)
                    .HasForeignKey(e => e.RoleId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Настройка Role
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            });

            // Настройка Component
            modelBuilder.Entity<Component>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Category).HasMaxLength(100).IsRequired(false);
                entity.Property(e => e.Brand).HasMaxLength(100).IsRequired(false);
                entity.Property(e => e.Model).HasMaxLength(100).IsRequired(false);
                entity.Property(e => e.Description).HasMaxLength(1000).IsRequired(false);
                entity.Property(e => e.Specifications).HasMaxLength(500).IsRequired(false);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
            });

            // Настройка Order
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ClientName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Status).HasMaxLength(50).HasDefaultValue("Новый");
                entity.Property(e => e.Priority).HasMaxLength(20).HasDefaultValue("Обычный");
            });

            // Настройка OrderItem
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TotalPrice).HasColumnType("decimal(18,2)");
                
                entity.HasOne(e => e.Order)
                    .WithMany(o => o.OrderItems)
                    .HasForeignKey(e => e.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
                    
                entity.HasOne(e => e.Component)
                    .WithMany(c => c.OrderItems)
                    .HasForeignKey(e => e.ComponentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Настройка Client
            modelBuilder.Entity<Client>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.ClientType).HasMaxLength(50);
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.Address).HasMaxLength(300);
                entity.Property(e => e.INN).HasMaxLength(20);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
            });

            // Настройка Employee
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Position).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.Salary).HasColumnType("decimal(18,2)");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
            });

            // Настройка WarehouseItem
            modelBuilder.Entity<WarehouseItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ComponentName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Category).HasMaxLength(100);
                entity.Property(e => e.Status).HasMaxLength(50).HasDefaultValue("В наличии");
                
                entity.HasOne(e => e.Component)
                    .WithMany()
                    .HasForeignKey(e => e.ComponentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Настройка AssemblyTask
            modelBuilder.Entity<AssemblyTask>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.AssemblerName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(50).HasDefaultValue("Ожидает");
                entity.Property(e => e.Notes).HasMaxLength(500);
                
                entity.HasOne(e => e.Order)
                    .WithMany()
                    .HasForeignKey(e => e.OrderId)
                    .OnDelete(DeleteBehavior.Restrict);
                    
                entity.HasOne(e => e.Assembler)
                    .WithMany()
                    .HasForeignKey(e => e.AssemblerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}