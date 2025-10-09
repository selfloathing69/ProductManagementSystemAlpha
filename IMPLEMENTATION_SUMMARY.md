# Практическая работа №2 - Реализация Data Access Layer

## ✅ ВЫПОЛНЕННЫЕ ТРЕБОВАНИЯ

### 1. Создан интерфейс IDomainObject ✅
**Файл:** `ProductManagementSystem.Core/IDomainObject.cs`

```csharp
public interface IDomainObject
{
    int Id { get; set; }
}
```

### 2. Модель Product реализует IDomainObject ✅
**Изменено:** `ProductManagementSystem.Core/Product.cs`

```csharp
public class Product : IDomainObject
{
    public int Id { get; set; }
    // ... остальные свойства
}
```

### 3. Создан новый проект DataAccessLayer ✅
**Проект:** `ProductManagementSystem.DataAccessLayer` (Class Library .NET 8.0)

Структура:
```
DataAccessLayer/
├── IRepository.cs           # Интерфейс репозитория
├── EF/
│   ├── DBContext.cs         # Entity Framework контекст
│   └── EntityRepository.cs  # Реализация через EF
├── Dapper/
│   └── DapperRepository.cs  # Реализация через Dapper
└── RepositoryTester.cs      # Утилиты для тестирования
```

### 4. Интерфейс IRepository<T> ✅
**Файл:** `ProductManagementSystem.DataAccessLayer/IRepository.cs`

```csharp
public interface IRepository<T> where T : IDomainObject
{
    void Add(T entity);
    void Delete(int id);
    IEnumerable<T> ReadAll();
    T ReadById(int id);
    void Update(T entity);
}
```

✅ Generic интерфейс с ограничением `where T : IDomainObject`
✅ Точно 5 методов с указанными сигнатурами

### 5. Entity Framework реализация ✅

#### NuGet пакеты установлены:
- EntityFramework 6.5.1 ✅

#### DBContext класс ✅
**Файл:** `ProductManagementSystem.DataAccessLayer/EF/DBContext.cs`

```csharp
public class AppDbContext : DbContext
{
    public AppDbContext() : base("name=DefaultConnection") { }
    
    public DbSet<Product> Products { get; set; }
    
    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        // Настройки маппинга
    }
}
```

✅ Наследуется от DbContext (EF 6)
✅ Использует connection string из App.config
✅ DbSet для модели Product
✅ ID с автогенерацией (IDENTITY)

#### EntityRepository<T> ✅
**Файл:** `ProductManagementSystem.DataAccessLayer/EF/EntityRepository.cs`

✅ Реализует IRepository<T>
✅ Обработка ВСЕХ исключений
✅ Using pattern для DbContext
✅ Проверки на null
✅ Информативные сообщения об ошибках

### 6. Dapper реализация ✅

#### NuGet пакеты установлены:
- Dapper 2.1.35 ✅
- System.Data.SqlClient 4.8.6 ✅
- System.Configuration.ConfigurationManager 8.0.0 ✅

#### DapperRepository<T> ✅
**Файл:** `ProductManagementSystem.DataAccessLayer/Dapper/DapperRepository.cs`

✅ Connection string из App.config
✅ Динамическая генерация SQL на основе свойств модели
✅ Обработка ВСЕХ исключений
✅ Using pattern для соединений
✅ Информативные сообщения об ошибках

Примеры генерируемого SQL:
```sql
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

### 7. Изменения в Business Logic ✅
**Изменено:** `ProductManagementSystem.Core/ProductLogic.cs`

#### Dependency Injection через конструктор:
```csharp
// Конструктор без репозитория (обратная совместимость)
public ProductLogic() { }

// Конструктор с репозиторием (новый функционал)
public ProductLogic(dynamic repository) 
{
    _repository = repository;
    _useRepository = true;
}
```

#### Методы используют репозиторий:
```csharp
public Product AddProduct(Product product)
{
    if (_useRepository)
        _repository!.Add(product);
    else
        _products.Add(product);
    return product;
}

public List<Product> GetAllProducts()
{
    if (_useRepository)
        return _repository!.ReadAll().ToList();
    else
        return _products.ToList();
}
```

✅ BL не знает, какая реализация репозитория используется
✅ Поддержка работы как с репозиторием, так и без него

### 8. App.config файлы ✅

#### WPF приложение:
**Файл:** `ProductManagementSystem.WpfApp/App.config`

#### Console приложение:
**Файл:** `ProductManagementSystem.ConsoleApp/App.config`

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

✅ Connection string для LocalDB
✅ Конфигурация Entity Framework

### 9. Решение Visual Studio обновлено ✅
**Файл:** `ProductManagementSystem.sln`

✅ Добавлен проект DataAccessLayer в решение
✅ Настроены конфигурации Debug и Release

### 10. Дополнительные файлы ✅

#### Документация:
- **DATA_ACCESS_LAYER.md** - подробная документация по использованию DAL
- **README.md** - обновлён с информацией о DAL

#### SQL скрипт:
- **Database_Setup.sql** - скрипт для ручного создания БД

#### Демонстрационные программы:
- **DataAccessLayerDemo.cs** - демонстрация работы с EF и Dapper
- **RepositoryTester.cs** - утилиты для тестирования репозиториев

## 🎯 КЛЮЧЕВЫЕ ОСОБЕННОСТИ РЕАЛИЗАЦИИ

### 1. Обработка исключений
Все методы репозиториев обрабатывают исключения:
```csharp
try
{
    using (var context = new AppDbContext())
    {
        return context.Set<T>().Find(id);
    }
}
catch (Exception ex)
{
    throw new Exception($"Ошибка при чтении записи: {ex.Message}", ex);
}
```

### 2. Using Pattern
Все подключения к БД используют `using`:
```csharp
using (var context = new AppDbContext())
{
    // операции с БД
}

using (var connection = CreateConnection())
{
    // операции через Dapper
}
```

### 3. Dependency Injection
Бизнес-логика получает репозиторий через конструктор:
```csharp
var repository = new EntityRepository<Product>();
var logic = new ProductLogic(repository);
```

### 4. Обратная совместимость
Старый код продолжает работать:
```csharp
// Старый способ (без БД)
var logic = new ProductLogic();

// Новый способ (с БД)
var repository = new EntityRepository<Product>();
var logic = new ProductLogic(repository);
```

### 5. Динамическая генерация SQL (Dapper)
SQL запросы генерируются автоматически на основе свойств модели:
```csharp
var properties = typeof(T).GetProperties()
    .Where(p => p.Name != "Id")
    .ToList();

var columns = string.Join(", ", properties.Select(p => p.Name));
var values = string.Join(", ", properties.Select(p => "@" + p.Name));

var sql = $"INSERT INTO {_tableName} ({columns}) VALUES ({values})";
```

## 📊 ПРИМЕРЫ ИСПОЛЬЗОВАНИЯ

### Entity Framework:
```csharp
var repository = new EntityRepository<Product>();
var logic = new ProductLogic(repository);

logic.AddProduct(new Product { Name = "Товар", Price = 1000 });
var products = logic.GetAllProducts();
```

### Dapper:
```csharp
var repository = new DapperRepository<Product>();
var logic = new ProductLogic(repository);

var product = logic.GetProduct(1);
logic.UpdateProduct(product);
```

### Без репозитория (In-Memory):
```csharp
var logic = new ProductLogic(); // Старый способ

var products = logic.GetAllProducts();
```

## ✅ СООТВЕТСТВИЕ ТРЕБОВАНИЯМ

| Требование | Статус | Примечание |
|-----------|--------|------------|
| IDomainObject интерфейс | ✅ | В ProductManagementSystem.Core |
| Product реализует IDomainObject | ✅ | Добавлено наследование |
| DataAccessLayer проект (DLL) | ✅ | Class Library .NET 8.0 |
| IRepository<T> интерфейс | ✅ | 5 методов CRUD |
| Entity Framework 6.x | ✅ | Версия 6.5.1 |
| DBContext класс | ✅ | AppDbContext с DbSet<Product> |
| EntityRepository<T> | ✅ | Реализует IRepository<T> |
| Dapper пакет | ✅ | Версия 2.1.35 |
| DapperRepository<T> | ✅ | Реализует IRepository<T> |
| Обработка исключений | ✅ | Во всех методах |
| Using pattern | ✅ | Для всех подключений |
| Dependency Injection | ✅ | Через конструктор |
| App.config с connection string | ✅ | В WPF и Console |
| LocalDB поддержка | ✅ | В строке подключения |
| Code First | ✅ | Таблицы создаются автоматически |
| Динамическая генерация SQL | ✅ | В DapperRepository |
| Проверки на null | ✅ | Во всех методах |
| Информативные ошибки | ✅ | С контекстом |
| Обратная совместимость | ✅ | Старый код работает |
| Документация | ✅ | DATA_ACCESS_LAYER.md |
| SQL скрипт | ✅ | Database_Setup.sql |
| Тесты/Демо | ✅ | RepositoryTester, DataAccessLayerDemo |

## 🏗️ АРХИТЕКТУРА

```
┌─────────────────────────────────────────────────────────────┐
│                    View Layer (WPF/WinForms/Console)         │
│                         ↓ использует                         │
│                    Business Logic Layer                      │
│                  (ProductLogic с DI)                         │
│                         ↓ использует                         │
│                    Data Access Layer                         │
│         ┌──────────────────┬──────────────────┐             │
│         │   EntityRepository │  DapperRepository │            │
│         │   (Entity Framework) │    (Dapper)     │            │
│         └──────────────────┴──────────────────┘             │
│                         ↓ работает с                         │
│                    Model Layer                               │
│              (Product : IDomainObject)                       │
│                         ↓                                    │
│              SQL Server LocalDB                              │
│          (ProductDatabase.mdf)                               │
└─────────────────────────────────────────────────────────────┘
```

## 📝 ВАЖНЫЕ ЗАМЕЧАНИЯ

1. ✅ Entity Framework создаёт таблицы автоматически при первом запуске
2. ✅ ID генерируются автоматически (IDENTITY в SQL)
3. ✅ Dapper требует существующие таблицы - запустить сначала EF версию
4. ✅ Строка подключения использует LocalDB (не требует установки SQL Server)
5. ✅ View уровень НЕ ИЗМЕНЁН - только Model, DAL и BL
6. ✅ Проект компилируется без ошибок

## 🚀 ЗАПУСК И ТЕСТИРОВАНИЕ

### Быстрый тест Entity Framework:
```bash
dotnet build ProductManagementSystem.sln
# На Windows запустить WPF или ConsoleApp с DataAccessLayerDemo
```

### Переключение между EF и Dapper:
```csharp
// В App.xaml.cs или Program.cs:

// Entity Framework:
var efRepository = new EntityRepository<Product>();
var logic = new ProductLogic(efRepository);

// Dapper:
var dapperRepository = new DapperRepository<Product>();
var logic = new ProductLogic(dapperRepository);
```

## ✨ ПРЕИМУЩЕСТВА РЕАЛИЗАЦИИ

1. ✅ **Гибкость** - легко переключаться между EF и Dapper
2. ✅ **Тестируемость** - можно создать Mock репозиторий
3. ✅ **Надёжность** - полная обработка ошибок
4. ✅ **Производительность** - Dapper для быстрых операций
5. ✅ **Удобство** - EF Code First для разработки
6. ✅ **Совместимость** - старый код продолжает работать
7. ✅ **Масштабируемость** - легко добавить новые модели

## 📚 ДОКУМЕНТАЦИЯ

- **DATA_ACCESS_LAYER.md** - полная документация по DAL
- **Database_Setup.sql** - SQL скрипт для ручного создания БД
- **README.md** - обновлённая главная документация

## ✅ ИТОГ

Все требования практической работы №2 выполнены полностью!
