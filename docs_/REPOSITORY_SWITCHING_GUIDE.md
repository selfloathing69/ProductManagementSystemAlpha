# Инструкция по переключению между Entity Framework и Dapper

## ?? С использованием Ninject DI-контейнера (Рекомендуется)

После реализации лабораторной работы №3 переключение между репозиториями стало **максимально простым** благодаря Ninject!

### ? НОВЫЙ СПОСОБ (с Ninject):

**Для переключения измените ТОЛЬКО ОДНУ строку в файле:**  
`ProductManagementSystem.DataAccessLayer/SimpleConfigModule.cs`

```csharp
public override void Load()
{
    //  Entity Framework 
    Bind<IRepository<Product>>().To<EntityRepository<Product>>().InSingletonScope();
    
    //  Dapper 
    // Закомментируйте строку выше и раскомментируйте эту:
    // Bind<IRepository<Product>>().To<DapperRepository<Product>>().InSingletonScope();
}
```

**Преимущества нового подхода:**
- ? Изменение в **одном месте** вместо трёх
- ? Централизованная настройка DI
- ? Следование принципам SOLID (DIP)
- ? Автоматическое применение ко всем UI-проектам

---

## Архитектура решения

Система управления товарами поддерживает два способа доступа к данным:
- **Entity Framework (EF)** - ORM с автоматическим отслеживанием изменений
- **Dapper** - легковесный микро-ORM с прямыми SQL-запросами

## ?? Старый способ (до Ninject) - для справки

### Способ 1: Через RepositoryFactory ? УСТАРЕЛО

Для каждого UI-проекта нужно было изменять файл `RepositoryFactory.cs`:

#### WinForms приложение
Файл: `ProductManagementSystem.WinFormsApp/RepositoryFactory.cs`

#### Console приложение
Файл: `ProductManagementSystem.ConsoleApp/RepositoryFactory.cs`

#### WPF приложение
Файл: `ProductManagementSystem.WpfApp/RepositoryFactory.cs`

**Старый код (больше не используется):**
```csharp
//  ПЕРЕКЛЮЧЕНИЕ РЕПОЗИТОРИЯ 
// Раскомментируйте нужную строку:

private const RepositoryType CurrentType = RepositoryType.EntityFramework;
//private const RepositoryType CurrentType = RepositoryType.Dapper;
```

---

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

---

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

---

## Строка подключения

Строка подключения по умолчанию (определена в репозиториях):
```
Server=AspireNotebook\SQLEXPRESS;Database=ProductManagementDB;Integrated Security=True;TrustServerCertificate=True;
```

Для изменения строки подключения:
- **Entity Framework**: `ProductManagementSystem.DataAccessLayer/EF/AppDbContext.cs`
- **Dapper**: `ProductManagementSystem.DataAccessLayer/Dapper/DapperRepository.cs`

---

## Проверка работоспособности

После переключения репозитория:
1. Пересоберите решение (Ctrl+Shift+B)
2. Запустите приложение
3. Проверьте, что данные отображаются корректно
4. Попробуйте добавить новый товар
5. Убедитесь, что изменения сохраняются в базе данных

---

## ?? Новые возможности с Ninject

### Dependency Injection
Теперь система использует **Ninject DI-контейнер** для управления зависимостями:

```csharp
// Класс ProductLogic получает репозиторий через конструктор
public ProductLogic(IRepository<Product> repository)
{
    _repository = repository;
}

// Ninject автоматически создаёт правильную реализацию
private ProductLogic _logic = new ProductLogic(RepositoryFactory.CreateRepository());
```

### Преимущества DI:
- ? Слабая связанность между модулями
- ? Легкое тестирование (можно подставлять mock-объекты)
- ? Соответствие принципу Dependency Inversion (SOLID)
- ? Централизованная настройка зависимостей

---

## ?? Дополнительная документация

Подробное руководство по реализации Ninject DI:
- **NINJECT_DI_IMPLEMENTATION.md** - полное описание архитектуры DI

Общая документация по Data Access Layer:
- **DATA_ACCESS_LAYER_SUMMARY.md** - обзор репозиториев EF и Dapper
- **ProductManagementSystem.DataAccessLayer/README.md** - использование репозиториев

---

## Поддержка

При возникновении проблем проверьте:
- Правильность строки подключения
- Доступность SQL Server
- Наличие необходимых таблиц в базе данных
- Правильность настройки привязки в `SimpleConfigModule.cs`
- Установлен ли пакет Ninject во всех проектах

---

**Версия:** 3.0 (с Ninject DI)  
**Дата обновления:** 2025  
**Изменения:** Добавлена поддержка Ninject DI-контейнера для централизованного управления зависимостями
