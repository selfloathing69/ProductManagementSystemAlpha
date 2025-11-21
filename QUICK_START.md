# Quick Start Guide - Data Access Layer

## Быстрый старт

Это краткое руководство поможет быстро начать работу с новым Data Access Layer.

## Шаг 1: Создание базы данных

Есть два способа создать базу данных:

### Способ А: Автоматически (Entity Framework миграции)

```bash
# 1. Установите EF Core tools глобально (один раз)
dotnet tool install --global dotnet-ef

# 2. Перейдите в папку проекта
cd ProductManagementSystem.DataAccessLayer

# 3. Создайте миграцию
dotnet ef migrations add InitialCreate --startup-project ../ProductManagementSystem.ConsoleApp

# 4. Примените миграцию (создаст базу данных и таблицы)
dotnet ef database update --startup-project ../ProductManagementSystem.ConsoleApp
```

### Способ Б: Автоматически при первом запуске

Просто запустите приложение с Entity Framework репозиторием - база данных будет создана автоматически:

```csharp
using ProductManagementSystem.DataAccessLayer.EF;

var efRepository = new EntityRepository<Product>();
var productLogic = new ProductLogic(efRepository);

// База данных создастся автоматически и заполнится примерами
```

## Шаг 2: Выбор репозитория

### Entity Framework (рекомендуется для начала)

```csharp
using ProductManagementSystem.DataAccessLayer.EF;
using ProductManagementSystem.Logic;

var repository = new EntityRepository<Product>();
var logic = new ProductLogic(repository);
```

### Dapper (для максимальной производительности)

```csharp
using ProductManagementSystem.DataAccessLayer.Dapper;
using ProductManagementSystem.Logic;

var repository = new DapperRepository<Product>();
var logic = new ProductLogic(repository);
```

### Локальный список (как было раньше)

```csharp
using ProductManagementSystem.Logic;

var logic = new ProductLogic(); // Без параметров
```

## Шаг 3: Работа с данными

После создания `ProductLogic` с репозиторием, все операции работают одинаково:

```csharp
// Добавление
var product = new Product 
{ 
    Name = "Новый товар",
    Price = 1000,
    Category = "Электроника",
    StockQuantity = 10
};
logic.AddProduct(product);

// Чтение
var allProducts = logic.GetAllProducts();
var oneProduct = logic.GetProduct(1);

// Обновление
product.Price = 1200;
logic.UpdateProduct(product);

// Удаление
logic.DeleteProduct(product.Id);
```

## Изменение строки подключения

По умолчанию используется:
```
Server=AspireNotebook\SQLEXPRESS;Database=ProductManagementDB;Integrated Security=True;TrustServerCertificate=True;
```

Для Dapper можно указать свою строку:

```csharp
var connectionString = "Server=MyServer;Database=MyDB;Integrated Security=True;";
var repository = new DapperRepository<Product>(connectionString);
```

Для Entity Framework измените в `AppDbContext.cs`:

```csharp
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    if (!optionsBuilder.IsConfigured)
    {
        var connectionString = "Ваша строка подключения";
        optionsBuilder.UseSqlServer(connectionString);
    }
}
```

## Проверка работы

### Тест 1: Проверка подключения

```csharp
try
{
    var repository = new EntityRepository<Product>();
    var logic = new ProductLogic(repository);
    var count = logic.GetAllProducts().Count;
    Console.WriteLine($"Успешно! В базе {count} товаров.");
}
catch (Exception ex)
{
    Console.WriteLine($"Ошибка: {ex.Message}");
}
```

### Тест 2: CRUD операции

Запустите пример из `RepositoryUsageExample.cs`:

```csharp
using ProductManagementSystem.DataAccessLayer.Examples;

// Тест Entity Framework
RepositoryUsageExample.EntityFrameworkExample();

// Тест Dapper
RepositoryUsageExample.DapperExample();

// Полный CRUD
RepositoryUsageExample.CrudOperationsExample();
```

## Типичные проблемы и решения

### Проблема: "Cannot connect to SQL Server"

**Решение**: Проверьте, что SQL Server Express запущен:
```bash
# Windows
services.msc
# Найдите "SQL Server (SQLEXPRESS)" и запустите
```

Или измените строку подключения на вашу.

### Проблема: "Database does not exist"

**Решение**: 
1. Запустите EF миграции (см. Шаг 1А)
2. Или просто запустите приложение - база создастся автоматически

### Проблема: "Table 'Products' does not exist"

**Решение**: 
- Для EF: Запустите `dotnet ef database update`
- Для Dapper: Убедитесь, что таблица создана через EF или вручную

## Переключение между EF и Dapper

Можно легко переключаться между реализациями:

```csharp
// Выбор ORM через конфигурацию
bool useDapper = false; // или true

IRepository<Product> repository = useDapper 
    ? new DapperRepository<Product>() 
    : new EntityRepository<Product>();

var logic = new ProductLogic(repository);
```

## Следующие шаги

1. ✅ Создайте базу данных (Шаг 1)
2. ✅ Выберите репозиторий (Шаг 2)
3. ✅ Протестируйте CRUD операции (Шаг 3)
4. 📖 Изучите `DATA_ACCESS_LAYER_SUMMARY.md` для деталей
5. 📖 Прочитайте `DataAccessLayer/README.md` для продвинутых сценариев

## Полезные ссылки

- **Полная документация**: `DATA_ACCESS_LAYER_SUMMARY.md`
- **Документация DAL**: `ProductManagementSystem.DataAccessLayer/README.md`
- **Примеры кода**: `ProductManagementSystem.DataAccessLayer/Examples/RepositoryUsageExample.cs`

## Обратная связь

Все существующие приложения (Console, WPF, WinForms) продолжают работать без изменений!
Новый функционал добавлен как опциональный и не ломает существующий код.
