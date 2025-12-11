# DataAccessLayer - слой доступа к данным

Этот проект предоставляет абстракцию для работы с базой данных через два ORM фреймворка: Entity Framework Core и Dapper.

## Архитектура

```
DataAccessLayer/
├── IRepository<T>           - Интерфейс репозитория (находится в Core/Logic)
├── EF/
│   ├── AppDbContext.cs      - Контекст Entity Framework
│   └── EntityRepository<T>  - Реализация через Entity Framework
└── Dapper/
    └── DapperRepository<T>  - Реализация через Dapper
```

## Использование Entity Framework Repository

```csharp
using ProductManagementSystem.DataAccessLayer.EF;
using ProductManagementSystem.Logic;
using ProductManagementSystem.Model;

// Создание репозитория Entity Framework
IRepository<Product> efRepository = new EntityRepository<Product>();

// Использование в ProductLogic
var productLogic = new ProductLogic(efRepository);

// CRUD операции
var product = new Product 
{ 
    Id = 0, // ID будет назначен автоматически
    Name = "Новый товар", 
    Price = 1000, 
    Category = "Электроника",
    StockQuantity = 10
};

productLogic.AddProduct(product);
var allProducts = productLogic.GetAllProducts();
```

## Использование Dapper Repository

```csharp
using ProductManagementSystem.DataAccessLayer.Dapper;
using ProductManagementSystem.Logic;
using ProductManagementSystem.Model;

// Создание репозитория Dapper
IRepository<Product> dapperRepository = new DapperRepository<Product>();

// Использование в ProductLogic
var productLogic = new ProductLogic(dapperRepository);

// CRUD операции работают аналогично
var product = new Product 
{ 
    Id = 0,
    Name = "Товар через Dapper", 
    Price = 2000, 
    Category = "Аксессуары",
    StockQuantity = 5
};

productLogic.AddProduct(product);
```

## Использование с пользовательской строкой подключения

```csharp
// Dapper с пользовательской строкой подключения
var connectionString = "Server=MyServer;Database=MyDB;Integrated Security=True;";
var dapperRepository = new DapperRepository<Product>(connectionString);
var productLogic = new ProductLogic(dapperRepository);
```

## Создание базы данных

Для создания базы данных с помощью Entity Framework:

1. Откройте Package Manager Console в Visual Studio
2. Выполните команды:

```powershell
# Создание миграции
Add-Migration InitialCreate -Project ProductManagementSystem.DataAccessLayer

# Применение миграции к базе данных
Update-Database -Project ProductManagementSystem.DataAccessLayer
```

Или используйте .NET CLI:

```bash
# Установите EF Core tools (если ещё не установлены)
dotnet tool install --global dotnet-ef

# Создание миграции
dotnet ef migrations add InitialCreate --project ProductManagementSystem.DataAccessLayer

# Применение миграции
dotnet ef database update --project ProductManagementSystem.DataAccessLayer
```

## IRepository<T> интерфейс

Интерфейс определяет стандартный набор CRUD операций:

```csharp
public interface IRepository<T> where T : IDomainObject
{
    void Add(T entity);              // Добавить сущность
    void Delete(int id);             // Удалить сущность по ID
    IEnumerable<T> ReadAll();        // Получить все сущности
    T? ReadById(int id);             // Получить сущность по ID
    void Update(T entity);           // Обновить сущность
}
```

## Обработка ошибок

Все методы репозитория могут выбрасывать исключения при ошибках работы с базой данных:

```csharp
try
{
    var product = new Product { /* ... */ };
    efRepository.Add(product);
}
catch (Exception ex)
{
    Console.WriteLine($"Ошибка при добавлении товара: {ex.Message}");
}
```

## Различия между Entity Framework и Dapper

### Entity Framework
- **Преимущества**: 
  - Автоматическое отслеживание изменений
  - Более высокий уровень абстракции
  - Code-First миграции
- **Недостатки**: 
  - Может быть медленнее для простых операций
  - Больший overhead

### Dapper
- **Преимущества**: 
  - Высокая производительность
  - Прямой контроль над SQL запросами
  - Минимальный overhead
- **Недостатки**: 
  - Требует создания таблиц вручную или через EF
  - Нет автоматического отслеживания изменений

## Совместимость

Текущая реализация использует .NET 8.0 и следующие пакеты:
- Microsoft.EntityFrameworkCore 9.0.10
- Microsoft.EntityFrameworkCore.SqlServer 9.0.10
- Dapper 2.1.66
- Microsoft.Data.SqlClient 6.1.2
