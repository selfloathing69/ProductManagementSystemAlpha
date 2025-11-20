# Реализация слоя доступа к данным (Data Access Layer)

## Обзор выполненного задания

Успешно реализован полный слой доступа к данным (Data Access Layer) для системы управления товарами согласно всем требованиям задания.

## Выполненные требования

### 1. ? Интерфейс IDomainObject
**Файл**: `ProductManagementSystem.Core/IDomainObject.cs`

```csharp
public interface IDomainObject
{
    int Id { get; set; }
}
```

- Определяет базовый интерфейс для всех доменных объектов
- Обязательное свойство `Id` для идентификации сущностей
- Класс `Product` реализует этот интерфейс

### 2. ? DLL библиотека DataAccessLayer
**Проект**: `ProductManagementSystem.DataAccessLayer`

- Создан отдельный проект библиотеки классов (.NET 8.0)
- Содержит всю логику доступа к данным
- Изолирован от UI слоя

### 3. ? Интерфейс IRepository<T>
**Файл**: `ProductManagementSystem.Core/IRepository.cs`

```csharp
public interface IRepository<T> where T : IDomainObject
{
    void Add(T entity);
    void Delete(int id);
    IEnumerable<T> ReadAll();
    T? ReadById(int id);
    void Update(T entity);
}
```

Обеспечивает стандартный набор CRUD операций:
- **Add** - добавление новой сущности
- **Delete** - удаление по ID
- **ReadAll** - получение всех сущностей
- **ReadById** - получение по ID
- **Update** - обновление сущности

### 4. ? NuGet пакеты

Установлены в проекте `DataAccessLayer`:
- `Microsoft.EntityFrameworkCore` (9.0.10)
- `Microsoft.EntityFrameworkCore.SqlServer` (9.0.10)
- `Dapper` (2.1.66)
- `Microsoft.Data.SqlClient` (6.1.2)

### 5. ? Два сценария работы с БД

#### a) Entity Framework сценарий

**Файл**: `ProductManagementSystem.DataAccessLayer/EF/AppDbContext.cs`
```csharp
public class AppDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    // Настройка подключения и схемы БД
}
```

**Файл**: `ProductManagementSystem.DataAccessLayer/EF/EntityRepository.cs`
```csharp
public class EntityRepository<T> : IRepository<T> 
    where T : class, IDomainObject
{
    // Реализация CRUD через Entity Framework
}
```

#### b) Dapper сценарий

**Файл**: `ProductManagementSystem.DataAccessLayer/Dapper/DapperRepository.cs`
```csharp
public class DapperRepository<T> : IRepository<T> 
    where T : class, IDomainObject
{
    // Реализация CRUD через Dapper и SQL запросы
}
```

### 6. ? Оба репозитория имплементируют IRepository

- `EntityRepository<T> : IRepository<T>`
- `DapperRepository<T> : IRepository<T>`

Полная совместимость и взаимозаменяемость реализаций.

### 7. ? ProductLogic использует IRepository

**Файл**: `ProductManagementSystem.Core/ProductLogic.cs`

```csharp
public class ProductLogic
{
    private readonly IRepository<Product>? _repository;
    
    public ProductLogic(IRepository<Product>? repository)
    {
        _repository = repository;
        // ...
    }
}
```

- Вместо локального списка используется репозиторий
- Поддерживается работа как с БД, так и с локальным списком (для обратной совместимости)

### 8. ? Уровень View не изменён

Все UI приложения работают без изменений функциональности:
- **Console App** - полностью функционален
- **WinForms App** - полностью функционален
- **WPF App** - полностью функционален

## Переключение между Dapper и Entity Framework

Во всех UI проектах реализовано простое переключение между ORM:

### Console App (`Program.cs`):
```csharp
using ProductManagementSystem.DataAccessLayer.EF;
//using ProductManagementSystem.DataAccessLayer.Dapper;

// Для переключения между Entity Framework и Dapper:
private static ProductLogic _productLogic = new ProductLogic(new EntityRepository<Product>());
//private static ProductLogic _productLogic = new ProductLogic(new DapperRepository<Product>());
```

### WinForms App (`MainForm.cs`):
```csharp
using ProductManagementSystem.DataAccessLayer.EF;
//using ProductManagementSystem.DataAccessLayer.Dapper;

// Для переключения между Entity Framework и Dapper:
private ProductLogic _logic = new ProductLogic(new EntityRepository<Product>());
//private ProductLogic _logic = new ProductLogic(new DapperRepository<Product>());
```

### WPF App (`MainWindow.xaml.cs`):
```csharp
using ProductManagementSystem.DataAccessLayer.EF;
//using ProductManagementSystem.DataAccessLayer.Dapper;

// Для переключения между Entity Framework и Dapper:
private ProductLogic _logic = new ProductLogic(new EntityRepository<Product>());
//private ProductLogic _logic = new ProductLogic(new DapperRepository<Product>());
```

**Инструкция по переключению:**
1. Закомментируйте using для текущего ORM
2. Раскомментируйте using для желаемого ORM
3. Закомментируйте строку создания текущего repository
4. Раскомментируйте строку создания желаемого repository

## Архитектура проекта

```
ProductManagementSystemAlpha/
?
??? ProductManagementSystem.Core/          # Модели и бизнес-логика
?   ??? IDomainObject.cs                   # Базовый интерфейс для доменных объектов
?   ??? IRepository.cs                     # Интерфейс репозитория с CRUD операциями
?   ??? Product.cs                         # Модель товара (реализует IDomainObject)
?   ??? ProductModel.cs                    # Дополнительная модель в Logic namespace
?   ??? ProductLogic.cs                    # Бизнес-логика (использует IRepository)
?
??? ProductManagementSystem.DataAccessLayer/ # Слой доступа к данным
?   ??? EF/                                # Entity Framework реализация
?   ?   ??? AppDbContext.cs                # DbContext для EF
?   ?   ??? EntityRepository.cs            # Репозиторий через EF
?   ??? Dapper/                            # Dapper реализация
?   ?   ??? DapperRepository.cs            # Репозиторий через Dapper
?   ??? Examples/                          # Примеры использования
?       ??? RepositoryUsageExample.cs
?
??? ProductManagementSystem.ConsoleApp/    # Console UI (View слой)
??? ProductManagementSystem.WinFormsApp/   # WinForms UI (View слой)
??? ProductManagementSystem.WpfApp/        # WPF UI (View слой)
```

## Разрешение циклических зависимостей

### Проблема
Первоначально возникала проблема с циклическими ссылками между пространствами имен:
- UI проекты ссылались на `ProductManagementSystem.Model`
- `ProductLogic` использовал `IRepository<Product>`
- Repositories ссылались обратно на Model

### Решение
1. **Модель остается в Model namespace**: `ProductManagementSystem.Model.Product`
2. **Logic использует Model**: `ProductManagementSystem.Logic` импортирует `ProductManagementSystem.Model`
3. **UI использует оба namespace**: UI проекты импортируют оба namespace по необходимости
4. **Repositories работают с Model.Product**: Все репозитории работают с `ProductManagementSystem.Model.Product`

Это позволяет:
- ? Избежать циклических зависимостей
- ? Сохранить четкое разделение слоев
- ? Обеспечить работу всех UI без изменений
- ? Поддерживать взаимозаменяемость репозиториев

## Строка подключения к базе данных

```
Server=AspireNotebook\SQLEXPRESS;Database=ProductManagementDB;Integrated Security=True;TrustServerCertificate=True;
```

- Используется SQL Server Express
- Integrated Security для автоматической аутентификации
- TrustServerCertificate=True для упрощения разработки

## Автоматическая инициализация данных

При первом запуске с репозиторием:
- Проверяется наличие данных в БД
- Если БД пуста, автоматически добавляются 10 примеров товаров
- Это обеспечивает немедленную работоспособность приложения

## Тестирование

? **Сборка**: Все проекты успешно компилируются без ошибок
? **Совместимость**: Оба репозитория (EF и Dapper) реализуют один интерфейс
? **Функциональность**: Все CRUD операции работают корректно
? **UI**: Все три UI приложения функционируют без изменений

## Преимущества реализованного решения

1. **Модульность**: Четкое разделение слоев приложения
2. **Гибкость**: Легкое переключение между EF и Dapper
3. **Расширяемость**: Простое добавление новых реализаций репозитория
4. **Тестируемость**: Возможность использования mock-объектов через IRepository
5. **Обратная совместимость**: Поддержка работы без БД (локальный список)
6. **Безопасность**: Параметризованные запросы предотвращают SQL-инъекции

## Соответствие требованиям задания

| Требование | Статус | Файл/Проект |
|------------|--------|-------------|
| IDomainObject интерфейс | ? | ProductManagementSystem.Core/IDomainObject.cs |
| Product реализует IDomainObject | ? | ProductManagementSystem.Core/Product.cs |
| DLL библиотека DataAccessLayer | ? | ProductManagementSystem.DataAccessLayer |
| IRepository интерфейс | ? | ProductManagementSystem.Core/IRepository.cs |
| Entity Framework | ? | DataAccessLayer/EF/ |
| Dapper | ? | DataAccessLayer/Dapper/ |
| AppDbContext | ? | DataAccessLayer/EF/AppDbContext.cs |
| EntityRepository<T> | ? | DataAccessLayer/EF/EntityRepository.cs |
| DapperRepository<T> | ? | DataAccessLayer/Dapper/DapperRepository.cs |
| IRepository в ProductLogic | ? | ProductManagementSystem.Core/ProductLogic.cs |
| View не изменен | ? | Все UI проекты |
| Переключение EF/Dapper | ? | Все UI проекты |

## Заключение

Реализация полностью соответствует всем требованиям задания:
- ? Создан слой Data Access Layer
- ? Реализованы оба сценария (EF и Dapper)
- ? Обеспечена архитектурная чистота
- ? Сохранена функциональность всех UI
- ? Добавлена возможность простого переключения между ORM
- ? Проект успешно компилируется и готов к использованию

Архитектура обеспечивает гибкость, расширяемость и соответствие принципам SOLID.
