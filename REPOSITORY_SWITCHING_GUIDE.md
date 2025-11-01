# Инструкция по переключению между Entity Framework и Dapper

## Архитектура решения

Система управления товарами поддерживает два способа доступа к данным:
- **Entity Framework (EF)** - ORM с автоматическим отслеживанием изменений
- **Dapper** - легковесный микро-ORM с прямыми SQL-запросами

## Как переключиться между репозиториями

### Способ 1: Через RepositoryFactory (Рекомендуется)

Для каждого UI-проекта создан класс `RepositoryFactory.cs`, который централизованно управляет выбором репозитория.

#### WinForms приложение
Файл: `ProductManagementSystem.WinFormsApp/RepositoryFactory.cs`

```csharp
// === ПЕРЕКЛЮЧЕНИЕ РЕПОЗИТОРИЯ ===
// Раскомментируйте нужную строку:

private const RepositoryType CurrentType = RepositoryType.EntityFramework;
//private const RepositoryType CurrentType = RepositoryType.Dapper;
```

#### Console приложение
Файл: `ProductManagementSystem.ConsoleApp/RepositoryFactory.cs`

```csharp
// === ПЕРЕКЛЮЧЕНИЕ РЕПОЗИТОРИЯ ===
// Раскомментируйте нужную строку:

private const RepositoryType CurrentType = RepositoryType.EntityFramework;
//private const RepositoryType CurrentType = RepositoryType.Dapper;
```

#### WPF приложение
Файл: `ProductManagementSystem.WpfApp/RepositoryFactory.cs`

```csharp
// === ПЕРЕКЛЮЧЕНИЕ РЕПОЗИТОРИЯ ===
// Раскомментируйте нужную строку:

private const RepositoryType CurrentType = RepositoryType.EntityFramework;
//private const RepositoryType CurrentType = RepositoryType.Dapper;
```

### Способ 2: Прямое указание в MainForm/Program

Если хотите больше контроля, можете изменить код напрямую:

#### WinForms:
```csharp
// В MainForm.cs измените строку:
// private ProductLogic _logic = new ProductLogic(RepositoryFactory.CreateRepository());

// На одну из:
private ProductLogic _logic = new ProductLogic(new EntityRepository<Product>());
// или
private ProductLogic _logic = new ProductLogic(new DapperRepository<Product>());
```

#### Console:
```csharp
// В Program.cs измените строку:
// private static ProductLogic _productLogic = new ProductLogic(RepositoryFactory.CreateRepository());

// На одну из:
private static ProductLogic _productLogic = new ProductLogic(new EntityRepository<Product>());
// или
private static ProductLogic _productLogic = new ProductLogic(new DapperRepository<Product>());
```

## Различия между Entity Framework и Dapper

### Entity Framework
**Преимущества:**
- Автоматическое отслеживание изменений (Change Tracking)
- Кэширование на уровне контекста
- Поддержка миграций базы данных
- LINQ запросы компилируются в SQL

**Недостатки:**
- Больше overhead памяти
- Медленнее при работе с большими объёмами данных

### Dapper
**Преимущества:**
- Высокая производительность (близко к ADO.NET)
- Минимальный overhead
- Полный контроль над SQL-запросами

**Недостатки:**
- Нужно писать SQL вручную
- Нет автоматического отслеживания изменений
- Нет встроенной поддержки миграций

## Требования к базе данных

Убедитесь, что:
1. SQL Server запущен
2. База данных `ProductManagementDB` создана
3. Таблица `Products` существует со следующей структурой:

```sql
CREATE TABLE Products (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(200) NOT NULL,
    Description NVARCHAR(500),
    Price DECIMAL(18,2) NOT NULL,
    Category NVARCHAR(100) NOT NULL,
    StockQuantity INT NOT NULL
);
```

## Строка подключения

Строка подключения по умолчанию (определена в репозиториях):
```
Server=AspireNotebook\SQLEXPRESS;Database=ProductManagementDB;Integrated Security=True;TrustServerCertificate=True;
```

Для изменения строки подключения:
- **Entity Framework**: `ProductManagementSystem.DataAccessLayer/EF/AppDbContext.cs`
- **Dapper**: `ProductManagementSystem.DataAccessLayer/Dapper/DapperRepository.cs`

## Проверка работоспособности

После переключения репозитория:
1. Пересоберите решение (Ctrl+Shift+B)
2. Запустите приложение
3. Проверьте, что данные отображаются корректно
4. Попробуйте добавить новый товар
5. Убедитесь, что изменения сохраняются в базе данных

## Поддержка

При возникновении проблем проверьте:
- Правильность строки подключения
- Доступность SQL Server
- Наличие необходимых таблиц в базе данных
- Правильность выбора типа репозитория в RepositoryFactory
