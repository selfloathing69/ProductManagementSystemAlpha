-- ============================================================================
-- SQL скрипт для создания базы данных Product Management System
-- СУБД: MS SQL Server 2021 / LocalDB
-- ============================================================================

USE master;
GO

-- Удаление существующей базы данных (если есть)
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'ProductManagementDB')
BEGIN
    ALTER DATABASE ProductManagementDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE ProductManagementDB;
    PRINT 'База данных ProductManagementDB удалена';
END
GO

-- Создание новой базы данных
CREATE DATABASE ProductManagementDB;
GO

PRINT 'База данных ProductManagementDB создана';
GO

USE ProductManagementDB;
GO

-- ============================================================================
-- Создание таблицы Products
-- ============================================================================

IF OBJECT_ID('dbo.Products', 'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.Products;
    PRINT 'Таблица Products удалена';
END
GO

CREATE TABLE dbo.Products
(
    Id INT IDENTITY(1,1) NOT NULL,
    Name NVARCHAR(200) NOT NULL,
    Description NVARCHAR(1000) NULL,
    Price DECIMAL(18, 2) NOT NULL,
    Category NVARCHAR(100) NOT NULL,
    StockQuantity INT NOT NULL DEFAULT 0,
    
    CONSTRAINT PK_Products PRIMARY KEY CLUSTERED (Id ASC),
    CONSTRAINT CK_Products_Price CHECK (Price >= 0),
    CONSTRAINT CK_Products_StockQuantity CHECK (StockQuantity >= 0)
);
GO

PRINT 'Таблица Products создана';
GO

-- ============================================================================
-- Заполнение тестовыми данными
-- ============================================================================

INSERT INTO dbo.Products (Name, Description, Price, Category, StockQuantity)
VALUES 
    ('Ноутбук Dell XPS 15', 'Мощный ноутбук для профессионалов', 120000.00, 'Электроника', 5),
    ('Смартфон iPhone 14', 'Флагманский смартфон Apple', 85000.00, 'Электроника', 15),
    ('Беспроводная мышь Logitech', 'Эргономичная беспроводная мышь', 2500.00, 'Периферия', 50),
    ('Механическая клавиатура', 'RGB подсветка, Cherry MX switches', 8500.00, 'Периферия', 20),
    ('Монитор Samsung 27"', '4K монитор с IPS матрицей', 35000.00, 'Электроника', 10),
    ('Наушники Sony WH-1000XM5', 'Премиум наушники с шумоподавлением', 25000.00, 'Аудио', 8),
    ('Веб-камера Logitech C920', 'Full HD веб-камера для стриминга', 7500.00, 'Периферия', 30),
    ('SSD Samsung 1TB', 'Быстрый твердотельный накопитель', 9500.00, 'Комплектующие', 40),
    ('Игровая мышь Razer', 'Высокоточная мышь для геймеров', 6500.00, 'Периферия', 25),
    ('USB Hub 7 портов', 'Активный USB 3.0 хаб', 2000.00, 'Аксессуары', 60);
GO

PRINT '10 тестовых товаров добавлено';
GO

-- ============================================================================
-- Проверка созданных данных
-- ============================================================================

SELECT 
    'Products' AS TableName,
    COUNT(*) AS RecordCount
FROM dbo.Products;
GO

-- Вывод всех товаров
SELECT 
    Id,
    Name,
    Price,
    Category,
    StockQuantity,
    (Price * StockQuantity) AS TotalValue
FROM dbo.Products
ORDER BY Category, Name;
GO

-- Общая стоимость склада
SELECT 
    COUNT(*) AS TotalProducts,
    SUM(StockQuantity) AS TotalItems,
    SUM(Price * StockQuantity) AS TotalValue
FROM dbo.Products;
GO

PRINT '============================================================================';
PRINT 'База данных успешно создана и заполнена!';
PRINT '============================================================================';
GO
