-- Быстрое добавление пользователя в базу данных
USE custom_pc_shop;

-- Проверяем существующие роли
SELECT 'Доступные роли:' as info;
SELECT Id, Name, Description FROM Roles;

-- Добавляем пользователя admin/admin с ролью Администратор
INSERT INTO Users (Login, PasswordHash, RoleId, IsActive) 
VALUES ('admin', 'admin', 1, TRUE)
ON DUPLICATE KEY UPDATE 
    PasswordHash = VALUES(PasswordHash),
    IsActive = VALUES(IsActive);

-- Проверяем добавленного пользователя
SELECT 'Добавленный пользователь:' as info;
SELECT u.Id, u.Login, u.PasswordHash, r.Name as RoleName, u.IsActive 
FROM Users u 
JOIN Roles r ON u.RoleId = r.Id 
WHERE u.Login = 'admin';

-- Учетные данные для входа: admin / admin