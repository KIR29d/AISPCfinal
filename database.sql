-- Создание базы данных AISPC (Автоматизированная информационная система сборки ПК)
CREATE DATABASE IF NOT EXISTS custom_pc_shop CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE custom_pc_shop;

-- Отключаем проверку внешних ключей временно
SET FOREIGN_KEY_CHECKS = 0;

-- Таблица ролей пользователей
CREATE TABLE IF NOT EXISTS Roles (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(100) NOT NULL UNIQUE,
    Description VARCHAR(500),
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- Таблица сотрудников
CREATE TABLE IF NOT EXISTS Employees (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    FullName VARCHAR(100) NOT NULL,
    Position VARCHAR(100) NOT NULL,
    Phone VARCHAR(20),
    Email VARCHAR(100),
    HireDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    Salary DECIMAL(18,2) DEFAULT 0,
    IsActive BOOLEAN DEFAULT TRUE,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- Таблица клиентов
CREATE TABLE IF NOT EXISTS Clients (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(200) NOT NULL,
    ClientType VARCHAR(50), -- Физическое лицо, ИП, ООО
    Phone VARCHAR(20),
    Email VARCHAR(100),
    Address VARCHAR(300),
    INN VARCHAR(20),
    Notes VARCHAR(500),
    IsActive BOOLEAN DEFAULT TRUE,
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- Таблица пользователей
CREATE TABLE IF NOT EXISTS Users (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Login VARCHAR(50) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    RoleId INT NOT NULL,
    EmployeeId INT NULL,
    ClientId INT NULL,
    IsActive BOOLEAN DEFAULT TRUE,
    LastLogin DATETIME NULL,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- Таблица компонентов
CREATE TABLE IF NOT EXISTS Components (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(200) NOT NULL,
    Category VARCHAR(100), -- Процессоры, Видеокарты, Память, и т.д.
    Brand VARCHAR(100),
    Model VARCHAR(100),
    Price DECIMAL(18,2) NOT NULL DEFAULT 0,
    StockQuantity INT DEFAULT 0,
    MinStockLevel INT DEFAULT 5,
    Description VARCHAR(1000),
    Specifications VARCHAR(500),
    IsActive BOOLEAN DEFAULT TRUE,
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- Таблица складских позиций
CREATE TABLE IF NOT EXISTS WarehouseItems (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    ComponentId INT NOT NULL,
    ComponentName VARCHAR(200) NOT NULL,
    Category VARCHAR(100),
    Quantity INT DEFAULT 0,
    MinLevel INT DEFAULT 5,
    Status VARCHAR(50) DEFAULT 'В наличии',
    LastMovement DATETIME DEFAULT CURRENT_TIMESTAMP,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- Таблица заказов
CREATE TABLE IF NOT EXISTS Orders (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    ClientName VARCHAR(100) NOT NULL,
    Status VARCHAR(50) DEFAULT 'Новый', -- Новый, В обработке, Сборка, Готов, Доставка, Завершен, Отменен
    TotalAmount DECIMAL(18,2) DEFAULT 0,
    OrderDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    CompletionDate DATETIME NULL,
    Notes VARCHAR(500),
    Priority VARCHAR(20) DEFAULT 'Обычный' -- Низкий, Обычный, Высокий, Критический
);

-- Таблица позиций заказа
CREATE TABLE IF NOT EXISTS OrderItems (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    OrderId INT NOT NULL,
    ComponentId INT NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(18,2) NOT NULL,
    TotalPrice DECIMAL(18,2) NOT NULL
);

-- Таблица задач сборки
CREATE TABLE IF NOT EXISTS AssemblyTasks (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    OrderId INT NOT NULL,
    AssemblerId INT NOT NULL,
    AssemblerName VARCHAR(200) NOT NULL,
    Status VARCHAR(50) DEFAULT 'Ожидает', -- Ожидает, В работе, Завершено, Приостановлено, Отменено
    StartDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    CompletionDate DATETIME NULL,
    Notes VARCHAR(500),
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- Вставка начальных данных
INSERT INTO Roles (Name, Description) VALUES 
('Администратор', 'Полный доступ к системе'),
('Менеджер', 'Управление заказами и клиентами'),
('Сборщик', 'Сборка компьютеров'),
('Кладовщик', 'Управление складом');

INSERT INTO Employees (FullName, Position, Phone, Email, HireDate, Salary) VALUES
('Петров Алексей Иванович', 'Главный сборщик', '+7 (903) 111-11-11', 'petrov@aispc.ru', '2023-01-15', 80000.00),
('Сидорова Мария Петровна', 'Менеджер по продажам', '+7 (903) 222-22-22', 'sidorova@aispc.ru', '2023-02-01', 70000.00),
('Козлов Дмитрий Сергеевич', 'Кладовщик', '+7 (903) 333-33-33', 'kozlov@aispc.ru', '2023-03-10', 50000.00),
('Иванова Елена Александровна', 'Бухгалтер', '+7 (903) 444-44-44', 'ivanova@aispc.ru', '2023-04-05', 60000.00);

INSERT INTO Users (Login, PasswordHash, RoleId, EmployeeId) VALUES 
('admin', 'admin', 1, NULL),
('manager', 'manager', 2, 2),
('assembler', 'assembler', 3, 1),
('warehouse', 'warehouse', 4, 3);

INSERT INTO Components (Name, Category, Brand, Model, Price, StockQuantity, MinStockLevel, Description) VALUES
('Intel Core i7-13700K', 'Процессоры', 'Intel', 'i7-13700K', 35000.00, 15, 5, '16-ядерный процессор'),
('AMD Ryzen 7 7700X', 'Процессоры', 'AMD', '7700X', 32000.00, 12, 5, '8-ядерный процессор'),
('NVIDIA RTX 4070', 'Видеокарты', 'NVIDIA', 'RTX 4070', 65000.00, 8, 3, 'Игровая видеокарта'),
('AMD RX 7800 XT', 'Видеокарты', 'AMD', 'RX 7800 XT', 58000.00, 6, 3, 'Игровая видеокарта'),
('Corsair Vengeance 32GB', 'Память', 'Corsair', 'Vengeance DDR5-5600', 12000.00, 12, 4, 'Оперативная память DDR5'),
('Samsung 980 PRO 1TB', 'Накопители', 'Samsung', '980 PRO', 8500.00, 2, 5, 'NVMe SSD накопитель'),
('ASUS ROG Strix B650', 'Материнские платы', 'ASUS', 'ROG Strix B650E', 18000.00, 6, 3, 'ATX материнская плата'),
('Corsair RM850x', 'Блоки питания', 'Corsair', 'RM850x', 12500.00, 15, 5, '850W 80+ Gold модульный БП');

-- Создаем складские позиции на основе компонентов
INSERT INTO WarehouseItems (ComponentId, ComponentName, Category, Quantity, MinLevel, Status, LastMovement) VALUES
(1, 'Intel Core i7-13700K', 'Процессоры', 15, 5, 'В наличии', DATE_SUB(NOW(), INTERVAL 2 DAY)),
(2, 'AMD Ryzen 7 7700X', 'Процессоры', 12, 5, 'В наличии', DATE_SUB(NOW(), INTERVAL 1 DAY)),
(3, 'NVIDIA RTX 4070', 'Видеокарты', 8, 3, 'В наличии', DATE_SUB(NOW(), INTERVAL 1 DAY)),
(4, 'AMD RX 7800 XT', 'Видеокарты', 6, 3, 'В наличии', DATE_SUB(NOW(), INTERVAL 3 DAY)),
(5, 'Corsair Vengeance 32GB', 'Память', 12, 4, 'В наличии', DATE_SUB(NOW(), INTERVAL 1 DAY)),
(6, 'Samsung 980 PRO 1TB', 'Накопители', 2, 5, 'Требует пополнения', DATE_SUB(NOW(), INTERVAL 3 DAY)),
(7, 'ASUS ROG Strix B650', 'Материнские платы', 6, 3, 'В наличии', DATE_SUB(NOW(), INTERVAL 4 DAY)),
(8, 'Corsair RM850x', 'Блоки питания', 15, 5, 'В наличии', DATE_SUB(NOW(), INTERVAL 2 DAY));

INSERT INTO Clients (Name, ClientType, Phone, Email, Address, INN, Notes) VALUES
('ООО Техносфера', 'ООО', '+7 (495) 123-45-67', 'info@technosfera.ru', 'г. Москва, ул. Ленина, д. 10', '7701234567', 'Постоянный клиент'),
('Иванов Иван Иванович', 'Физическое лицо', '+7 (903) 123-45-67', 'ivanov@mail.ru', 'г. Москва, ул. Пушкина, д. 5, кв. 10', NULL, 'Игровые конфигурации'),
('ИП Петров Петр Петрович', 'ИП', '+7 (916) 987-65-43', 'petrov.ip@gmail.com', 'г. Москва, пр-т Мира, д. 15', '123456789012', 'Офисная техника'),
('ООО Компьютеры+', 'ООО', '+7 (495) 555-66-77', 'orders@computers-plus.ru', 'г. Москва, ул. Тверская, д. 20', '7702345678', 'Корпоративные заказы'),
('Сидоров Сидор Сидорович', 'Физическое лицо', '+7 (926) 111-22-33', 'sidorov@yandex.ru', 'г. Москва, ул. Арбат, д. 25, кв. 5', NULL, 'Домашние ПК');

INSERT INTO Orders (ClientName, Status, TotalAmount, OrderDate, Notes, Priority) VALUES
('ООО Техносфера', 'В обработке', 125000.00, DATE_SUB(NOW(), INTERVAL 1 DAY), 'Срочный заказ', 'Высокий'),
('Иванов Иван Иванович', 'Сборка', 85000.00, DATE_SUB(NOW(), INTERVAL 2 DAY), 'Игровая конфигурация', 'Обычный'),
('ИП Петров Петр Петрович', 'Готов', 95000.00, DATE_SUB(NOW(), INTERVAL 3 DAY), 'Офисный ПК', 'Обычный'),
('ООО Компьютеры+', 'Доставка', 150000.00, DATE_SUB(NOW(), INTERVAL 4 DAY), 'Рабочая станция', 'Высокий'),
('Сидоров Сидор Сидорович', 'Завершен', 75000.00, DATE_SUB(NOW(), INTERVAL 5 DAY), 'Домашний ПК', 'Обычный');

-- Добавляем позиции к заказам
INSERT INTO OrderItems (OrderId, ComponentId, Quantity, UnitPrice, TotalPrice) VALUES
(1, 1, 1, 35000.00, 35000.00),
(1, 3, 1, 65000.00, 65000.00),
(1, 5, 1, 12000.00, 12000.00),
(2, 2, 1, 32000.00, 32000.00),
(2, 4, 1, 58000.00, 58000.00),
(3, 1, 1, 35000.00, 35000.00),
(3, 6, 1, 8500.00, 8500.00),
(4, 1, 2, 35000.00, 70000.00),
(4, 3, 1, 65000.00, 65000.00),
(5, 2, 1, 32000.00, 32000.00),
(5, 6, 1, 8500.00, 8500.00);

-- Создаем задачи сборки
INSERT INTO AssemblyTasks (OrderId, AssemblerId, AssemblerName, Status, StartDate, Notes) VALUES
(1, 1, 'Петров Алексей Иванович', 'В работе', DATE_SUB(NOW(), INTERVAL 2 DAY), 'Игровой ПК высокого класса'),
(2, 1, 'Петров Алексей Иванович', 'Ожидает', DATE_SUB(NOW(), INTERVAL 1 DAY), 'Офисный компьютер'),
(3, 1, 'Петров Алексей Иванович', 'Завершено', DATE_SUB(NOW(), INTERVAL 5 DAY), 'Рабочая станция для дизайна'),
(4, 1, 'Петров Алексей Иванович', 'Ожидает', NOW(), 'Сервер для малого бизнеса');

-- Включаем проверку внешних ключей обратно
SET FOREIGN_KEY_CHECKS = 1;

-- Добавляем внешние ключи после создания всех таблиц и данных
ALTER TABLE Users ADD CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleId) REFERENCES Roles(Id) ON DELETE RESTRICT;
ALTER TABLE Users ADD CONSTRAINT FK_Users_Employees FOREIGN KEY (EmployeeId) REFERENCES Employees(Id) ON DELETE SET NULL;
ALTER TABLE Users ADD CONSTRAINT FK_Users_Clients FOREIGN KEY (ClientId) REFERENCES Clients(Id) ON DELETE SET NULL;
ALTER TABLE WarehouseItems ADD CONSTRAINT FK_WarehouseItems_Components FOREIGN KEY (ComponentId) REFERENCES Components(Id) ON DELETE RESTRICT;
ALTER TABLE OrderItems ADD CONSTRAINT FK_OrderItems_Orders FOREIGN KEY (OrderId) REFERENCES Orders(Id) ON DELETE CASCADE;
ALTER TABLE OrderItems ADD CONSTRAINT FK_OrderItems_Components FOREIGN KEY (ComponentId) REFERENCES Components(Id) ON DELETE RESTRICT;
ALTER TABLE AssemblyTasks ADD CONSTRAINT FK_AssemblyTasks_Orders FOREIGN KEY (OrderId) REFERENCES Orders(Id) ON DELETE RESTRICT;
ALTER TABLE AssemblyTasks ADD CONSTRAINT FK_AssemblyTasks_Employees FOREIGN KEY (AssemblerId) REFERENCES Employees(Id) ON DELETE RESTRICT;

-- Создание индексов для оптимизации
CREATE INDEX idx_users_login ON Users(Login);
CREATE INDEX idx_components_category ON Components(Category);
CREATE INDEX idx_components_brand ON Components(Brand);
CREATE INDEX idx_orders_status ON Orders(Status);
CREATE INDEX idx_orders_date ON Orders(OrderDate);
CREATE INDEX idx_clients_name ON Clients(Name);
CREATE INDEX idx_clients_type ON Clients(ClientType);
CREATE INDEX idx_warehouse_component ON WarehouseItems(ComponentId);
CREATE INDEX idx_assembly_order ON AssemblyTasks(OrderId);
CREATE INDEX idx_assembly_assembler ON AssemblyTasks(AssemblerId);