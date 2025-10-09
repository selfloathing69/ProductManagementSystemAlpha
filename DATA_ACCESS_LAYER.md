# Data Access Layer Documentation

## Обзор

Данный проект реализует слой доступа к данным (Data Access Layer) для системы управления товарами. Поддерживаются два подхода к работе с базой данных:
- **Entity Framework 6.x** - полнофункциональный ORM с поддержкой Code First
- **Dapper** - легковесный микро-ORM для высокой производительности

## Архитектура

```
ProductManagementSystemAlpha/
├── ProductManagementSystem.Core/           (Model & Business Logic Layer)
│   ├── IDomainObject.cs                   (базовый интерфейс для всех сущностей)
│   ├── Product.cs                         (модель товара, реализует IDomainObject)
│   └── ProductLogic.cs                    (бизнес-логика с поддержкой репозиториев)
│
├── ProductManagementSystem.DataAccessLayer/ (новый слой доступа к данным)
│   ├── IRepository.cs                     (интерфейс репозитория)
│   ├── EF/
│   │   ├── DBContext.cs                   (контекст Entity Framework)
│   │   └── EntityRepository.cs            (реализация через EF)
│   ├── Dapper/
│   │   └── DapperRepository.cs            (реализация через Dapper)
│   └── RepositoryTester.cs                (тесты и примеры использования)
│
└── ProductManagementSystem.WpfApp/         (View Layer)
    └── App.config                          (строка подключения к БД)
```

## Интерфейс IDomainObject

Все доменные объекты должны реализовывать интерфейс `IDomainObject`:

```csharp
public interface IDomainObject
{
    int Id { get; set; }
}
```

Модель `Product` теперь реализует этот интерфейс:

```csharp
public class Product : IDomainObject
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string Category { get; set; }
    public int StockQuantity { get; set; }
}
```

## Интерфейс IRepository<T>

Базовый интерфейс репозитория определяет стандартные CRUD операции:

```csharp
public interface IRepository<T> where T : IDomainObject
{
    void Add(T entity);              // Создание
    void Delete(int id);             // Удаление по ID
    IEnumerable<T> ReadAll();        // Чтение всех записей
    T ReadById(int id);              // Чтение по ID
    void Update(T entity);           // Обновление
}
```

## Использование Entity Framework

### 1. Создание репозитория

```csharp
using ProductManagementSystem.DataAccessLayer.EF;
using ProductManagementSystem.Model;

var repository = new EntityRepository<Product>();
```

### 2. CRUD операции

```csharp
// CREATE - Добавление товара
var product = new Product 
{ 
    Name = "Ноутбук", 
    Price = 75000, 
    Category = "Электроника",
    StockQuantity = 10 
};
repository.Add(product);

// READ - Чтение всех товаров
var allProducts = repository.ReadAll();

// READ - Чтение по ID
var product = repository.ReadById(1);

// UPDATE - Обновление
product.Price = 80000;
repository.Update(product);

// DELETE - Удаление
repository.Delete(1);
```

### 3. Настройка DBContext

`AppDbContext` настроен для автоматического создания базы данных:

```csharp
public class AppDbContext : DbContext
{
    public AppDbContext() : base("name=DefaultConnection") { }
    
    public DbSet<Product> Products { get; set; }
}
```

## Использование Dapper

### 1. Создание репозитория

```csharp
using ProductManagementSystem.DataAccessLayer.Dapper;
using ProductManagementSystem.Model;

var repository = new DapperRepository<Product>();
```

### 2. CRUD операции

```csharp
// CREATE
var product = new Product { Name = "Монитор", Price = 25000 };
repository.Add(product);

// READ
var allProducts = repository.ReadAll();
var product = repository.ReadById(1);

// UPDATE
product.Price = 27000;
repository.Update(product);

// DELETE
repository.Delete(1);
```

### 3. Динамическая генерация SQL

`DapperRepository` автоматически генерирует SQL запросы на основе свойств модели:

```csharp
// Для Product генерируется:
INSERT INTO Products (Name, Description, Price, Category, StockQuantity) 
VALUES (@Name, @Description, @Price, @Category, @StockQuantity)

UPDATE Products 
SET Name = @Name, Description = @Description, Price = @Price, 
    Category = @Category, StockQuantity = @StockQuantity 
WHERE Id = @Id

DELETE FROM Products WHERE Id = @Id

SELECT * FROM Products
SELECT * FROM Products WHERE Id = @Id
```

## Интеграция с Business Logic

### Dependency Injection через конструктор

```csharp
using ProductManagementSystem.Logic;
using ProductManagementSystem.DataAccessLayer.EF;

// Создаём репозиторий
var repository = new EntityRepository<Product>();

// Передаём репозиторий в бизнес-логику
var logic = new ProductLogic(repository);

// Используем как обычно
var products = logic.GetAllProducts();
logic.AddProduct(new Product { Name = "Товар", Price = 1000 });
```

### Обратная совместимость

`ProductLogic` поддерживает два режима работы:

```csharp
// Режим 1: Без репозитория (список в памяти)
var logic1 = new ProductLogic();

// Режим 2: С репозиторием (база данных)
var repository = new EntityRepository<Product>();
var logic2 = new ProductLogic(repository);
```

## Настройка базы данных

### App.config / Web.config

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <connectionStrings>
    <add name="DefaultConnection" 
         connectionString="Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\ProductDatabase.mdf;Integrated Security=True;Connect Timeout=30" 
         providerName="System.Data.SqlClient" />
  </connectionStrings>
  
  <entityFramework>
    <defaultConnectionFactory type="System.Data.Entity.Infrastructure.LocalDbConnectionFactory, EntityFramework">
      <parameters>
        <parameter value="mssqllocaldb" />
      </parameters>
    </defaultConnectionFactory>
    <providers>
      <provider invariantName="System.Data.SqlClient" 
                type="System.Data.Entity.SqlServer.SqlProviderServices, EntityFramework.SqlServer" />
    </providers>
  </entityFramework>
</configuration>
```

### Создание базы данных

1. **Автоматическое создание через Entity Framework:**
   - При первом запуске с `EntityRepository` база данных создастся автоматически
   - Таблицы создаются на основе моделей (Code First подход)

2. **Ручное создание через Visual Studio:**
   - В проекте WPF: Добавить → Создать элемент
   - Выбрать "База данных, основанная на службах"
   - Назвать файл: `ProductDatabase.mdf`

## Обработка ошибок

Все методы репозиториев обрабатывают исключения:

```csharp
try
{
    var product = repository.ReadById(999);
}
catch (Exception ex)
{
    Console.WriteLine($"Ошибка: {ex.Message}");
    // Вывод: "Ошибка при чтении записи: Запись с ID=999 не найдена"
}
```

## Тестирование

Используйте класс `RepositoryTester` для проверки работы репозиториев:

```csharp
using ProductManagementSystem.DataAccessLayer;

// Тест Entity Framework
RepositoryTester.TestEntityFramework();

// Тест Dapper
RepositoryTester.TestDapper();

// Тест интеграции с Business Logic
RepositoryTester.TestBusinessLogicIntegration();
```

## NuGet пакеты

Проект использует следующие зависимости:

```xml
<PackageReference Include="EntityFramework" Version="6.5.1" />
<PackageReference Include="Dapper" Version="2.1.35" />
<PackageReference Include="System.Data.SqlClient" Version="4.8.6" />
<PackageReference Include="System.Configuration.ConfigurationManager" Version="8.0.0" />
```

## Переключение между EF и Dapper

```csharp
// В App.xaml.cs или Program.cs:

// Вариант 1: Entity Framework
var efRepository = new EntityRepository<Product>();
var logic = new ProductLogic(efRepository);

// Вариант 2: Dapper
var dapperRepository = new DapperRepository<Product>();
var logic = new ProductLogic(dapperRepository);

// Остальной код остаётся без изменений!
```

## Преимущества реализации

1. ✅ **Abstraction** - бизнес-логика не зависит от конкретной реализации репозитория
2. ✅ **Flexibility** - легко переключаться между EF и Dapper
3. ✅ **Testability** - можно создать Mock репозиторий для тестирования
4. ✅ **Error Handling** - полная обработка всех исключений БД
5. ✅ **Code First** - автоматическое создание БД через EF
6. ✅ **Performance** - Dapper для высокой производительности
7. ✅ **Backward Compatibility** - старый код продолжает работать

## Рекомендации

- **Entity Framework** - используйте для сложных запросов, связей между таблицами, миграций
- **Dapper** - используйте для простых CRUD операций, когда важна производительность
- **LocalDB** - удобно для разработки, не требует установки SQL Server
- **Connection String** - храните в App.config, не хардкодьте в коде

## Требования к системе

- .NET 8.0 или выше
- SQL Server LocalDB (входит в Visual Studio)
- Entity Framework 6.5.1
- Windows OS (для LocalDB)

## Известные ограничения

1. Dapper требует существующую таблицу в БД (сначала запустите EF версию)
2. ID генерируется автоматически (IDENTITY в SQL) 
3. При удалении записей ID не переиспользуются
4. LocalDB доступен только на Windows

## Дальнейшее развитие

- Добавить поддержку транзакций
- Реализовать async/await версии методов
- Добавить кеширование запросов
- Поддержка других СУБД (PostgreSQL, MySQL)
- Миграции базы данных через EF Migrations
