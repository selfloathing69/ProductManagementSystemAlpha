# Ninject Dependency Injection - Руководство по реализации

## Обзор изменений (Лабораторная работа №3)

Этот документ описывает реализацию принципа **Dependency Inversion Principle (DIP)** из SOLID с использованием **Dependency Injection (DI)** контейнера **Ninject**.

---

## 🎯 Цель рефакторинга

**До рефакторинга:**
```csharp
// ❌ Жёсткая связь - класс создаёт зависимость напрямую
public class ProductLogic
{
    private IRepository<Product> _repository = new EntityRepository<Product>();
}
```

**После рефакторинга:**
```csharp
// ✅ Слабая связь - зависимость внедряется через конструктор
public class ProductLogic
{
    private readonly IRepository<Product> _repository;
    
    public ProductLogic(IRepository<Product> repository)
    {
        _repository = repository;
    }
}
```

---

## 📦 Установленные пакеты

Для всех проектов установлен пакет **Ninject 3.3.6**:

```bash
# Core
dotnet add ProductManagementSystem.Core package Ninject

# DataAccessLayer
dotnet add ProductManagementSystem.DataAccessLayer package Ninject

# UI Projects
dotnet add ProductManagementSystem.WinFormsApp package Ninject
dotnet add ProductManagementSystem.ConsoleApp package Ninject
dotnet add ProductManagementSystem.WpfApp package Ninject
```

---

## 🏗️ Архитектура решения

### 1. Интерфейс (Абстракция)

**Файл:** `ProductManagementSystem.Core/IRepository.cs`

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

**Принцип DIP:** Высокоуровневый модуль (ProductLogic) зависит от абстракции (IRepository), а не от деталей реализации.

---

### 2. Класс бизнес-логики с Constructor Injection

**Файл:** `ProductManagementSystem.Core/ProductLogic.cs`

```csharp
public class ProductLogic
{
    // Зависимость от интерфейса, а не от конкретной реализации
    private readonly IRepository<Product>? _repository;

    // Constructor Injection - зависимость внедряется через конструктор
    public ProductLogic(IRepository<Product>? repository)
    {
        _repository = repository;
        
        if (_repository != null)
        {
            InitializeDataIfEmpty();
        }
        else
        {
            InitializeLocalData();
        }
    }
    
    // Остальные методы используют _repository
}
```

**Что было изменено:**
- ✅ Удалено прямое создание репозитория (`new EntityRepository()`)
- ✅ Добавлен конструктор, принимающий `IRepository<Product>`
- ✅ Зависимость помечена как `readonly` для неизменности
- ✅ Добавлены подробные комментарии о DIP и Constructor Injection

---

### 3. Модуль конфигурации Ninject

**Файл:** `ProductManagementSystem.DataAccessLayer/SimpleConfigModule.cs`

```csharp
using Ninject.Modules;
using ProductManagementSystem.Logic;
using ProductManagementSystem.Model;
using ProductManagementSystem.DataAccessLayer.EF;
using ProductManagementSystem.DataAccessLayer.Dapper;

public class SimpleConfigModule : NinjectModule
{
    public override void Load()
    {
        // Привязка интерфейса к конкретной реализации
        Bind<IRepository<Product>>()
            .To<EntityRepository<Product>>()
            .InSingletonScope();
        
        // Для переключения на Dapper раскомментируйте:
        // Bind<IRepository<Product>>()
        //     .To<DapperRepository<Product>>()
        //     .InSingletonScope();
    }
}
```

**Что делает этот модуль:**
- 📌 **Bind<TInterface>().To<TImplementation>()** - связывает интерфейс с реализацией
- 📌 **InSingletonScope()** - создаёт экземпляр один раз и переиспользует его
- 📌 Централизованная настройка всех зависимостей приложения

---

### 4. RepositoryFactory с Ninject

**Файлы:** 
- `ProductManagementSystem.WinFormsApp/RepositoryFactory.cs`
- `ProductManagementSystem.ConsoleApp/RepositoryFactory.cs`
- `ProductManagementSystem.WpfApp/RepositoryFactory.cs`

```csharp
using Ninject;
using ProductManagementSystem.DataAccessLayer;

internal static class RepositoryFactory
{
    private static IKernel? _kernel;

    private static IKernel Kernel
    {
        get
        {
            if (_kernel == null)
            {
                // Создание ядра Ninject с модулем конфигурации
                _kernel = new StandardKernel(new SimpleConfigModule());
            }
            return _kernel;
        }
    }

    public static IRepository<Product> CreateRepository()
    {
        // Ninject автоматически создаст нужную реализацию
        return Kernel.Get<IRepository<Product>>();
    }
}
```

**Что было изменено:**
- ❌ **Удалено:** Ручное создание репозиториев (`new EntityRepository()`, `new DapperRepository()`)
- ❌ **Удалено:** Enum `RepositoryType` и switch для выбора реализации
- ✅ **Добавлено:** Использование Ninject для автоматического создания зависимостей
- ✅ **Добавлено:** Singleton паттерн для DI-контейнера

---

## 🔄 Как работает Dependency Injection

### Жизненный цикл создания объекта:

```
1. UI-слой запрашивает:
   RepositoryFactory.CreateRepository()
   
2. RepositoryFactory обращается к Ninject:
   Kernel.Get<IRepository<Product>>()
   
3. Ninject смотрит в SimpleConfigModule:
   "IRepository<Product> → EntityRepository<Product>"
   
4. Ninject создаёт EntityRepository<Product>
   (один раз, т.к. InSingletonScope)
   
5. UI-слой создаёт ProductLogic:
   new ProductLogic(repository)
   
6. ProductLogic получает готовый репозиторий через конструктор
```

---

## 🎨 Преимущества использования Ninject

### ✅ До (без DI):
```csharp
// В каждом UI-проекте нужно было выбирать реализацию
private const RepositoryType CurrentType = RepositoryType.EntityFramework;

public static IRepository<Product> CreateRepository()
{
    return CurrentType switch
    {
        RepositoryType.EntityFramework => new EntityRepository<Product>(),
        RepositoryType.Dapper => new DapperRepository<Product>(),
        _ => throw new NotSupportedException()
    };
}
```

### ✅ После (с Ninject):
```csharp
// Одна настройка для всего приложения в SimpleConfigModule
Bind<IRepository<Product>>().To<EntityRepository<Product>>().InSingletonScope();

// В UI-проектах просто:
return Kernel.Get<IRepository<Product>>();
```

**Преимущества:**
1. **Централизованная настройка** - все зависимости в одном месте
2. **Упрощение кода** - UI-слой не знает о конкретных реализациях
3. **Легкое тестирование** - можно подставлять mock-объекты
4. **SOLID-принципы** - следование DIP и ISP
5. **Singleton управление** - контейнер сам управляет жизненным циклом

---

## 🔀 Переключение между Entity Framework и Dapper

### Старый способ (без Ninject):
```csharp
// ❌ Нужно менять в 3 файлах (каждый RepositoryFactory)
private const RepositoryType CurrentType = RepositoryType.EntityFramework;
// меняем на
private const RepositoryType CurrentType = RepositoryType.Dapper;
```

### Новый способ (с Ninject):
```csharp
// ✅ Меняем ТОЛЬКО в SimpleConfigModule.cs
// Было:
Bind<IRepository<Product>>().To<EntityRepository<Product>>().InSingletonScope();

// Становится:
Bind<IRepository<Product>>().To<DapperRepository<Product>>().InSingletonScope();
```

**Результат:** Одна строка кода вместо трёх!

---

## 📝 Использование в UI-слое

### WinForms (MainForm.cs):
```csharp
using ProductManagementSystem.Logic;

public partial class MainForm : Form
{
    // Ninject автоматически создаст нужный репозиторий
    private ProductLogic _logic = new ProductLogic(RepositoryFactory.CreateRepository());
    
    // Остальной код остаётся без изменений
}
```

### Console (Program.cs):
```csharp
using ProductManagementSystem.Logic;

internal class Program
{
    // Ninject автоматически создаст нужный репозиторий
    private static ProductLogic _productLogic = 
        new ProductLogic(RepositoryFactory.CreateRepository());
    
    // Остальной код остаётся без изменений
}
```

---

## 🧪 Тестирование с Ninject

Пример unit-теста с mock-репозиторием:

```csharp
[TestMethod]
public void AddProduct_ShouldCallRepositoryAdd()
{
    // Arrange
    var mockRepository = new Mock<IRepository<Product>>();
    var productLogic = new ProductLogic(mockRepository.Object);
    var product = new Product { Name = "Test", Price = 100 };
    
    // Act
    productLogic.AddProduct(product);
    
    // Assert
    mockRepository.Verify(r => r.Add(It.IsAny<Product>()), Times.Once);
}
```

**Преимущество:** Можно легко подставить mock без изменения кода ProductLogic.

---

## 📊 Сравнение подходов

| Аспект | Без DI | С Ninject DI |
|--------|--------|--------------|
| **Связанность** | Высокая (жёсткая связь) | Низкая (слабая связь) |
| **Гибкость** | Низкая | Высокая |
| **Тестируемость** | Сложная | Лёгкая |
| **Настройка** | В каждом проекте | Централизованная |
| **SOLID** | Нарушает DIP | Соответствует DIP |
| **Управление зависимостями** | Ручное | Автоматическое |

---

## 🚀 Расширение системы

### Добавление новой сущности (например, Order):

**1. Создайте репозиторий:**
```csharp
public class Order : IDomainObject { ... }
```

**2. Добавьте привязку в SimpleConfigModule:**
```csharp
public override void Load()
{
    Bind<IRepository<Product>>().To<EntityRepository<Product>>().InSingletonScope();
    
    // Добавляем новую привязку
    Bind<IRepository<Order>>().To<EntityRepository<Order>>().InSingletonScope();
}
```

**3. Используйте в бизнес-логике:**
```csharp
public class OrderLogic
{
    private readonly IRepository<Order> _repository;
    
    public OrderLogic(IRepository<Order> repository)
    {
        _repository = repository;
    }
}
```

**Всё!** Ninject автоматически создаст правильную реализацию.

---

## 🎓 Принципы SOLID в реализации

### ✅ **S** - Single Responsibility Principle
- `ProductLogic` отвечает только за бизнес-логику
- `IRepository` отвечает только за доступ к данным
- `SimpleConfigModule` отвечает только за настройку DI

### ✅ **O** - Open/Closed Principle
- Код открыт для расширения (можно добавлять новые репозитории)
- Код закрыт для модификации (не нужно менять ProductLogic)

### ✅ **L** - Liskov Substitution Principle
- Можно подставить любую реализацию IRepository
- ProductLogic работает с любой реализацией одинаково

### ✅ **I** - Interface Segregation Principle
- IRepository содержит только необходимые методы CRUD
- Клиенты не зависят от методов, которые не используют

### ✅ **D** - Dependency Inversion Principle ⭐
- **Высокоуровневый модуль** (ProductLogic) зависит от **абстракции** (IRepository)
- **Низкоуровневые модули** (EntityRepository, DapperRepository) зависят от той же **абстракции**
- Детали реализации не влияют на бизнес-логику

---

## 📚 Дополнительные возможности Ninject

### 1. Условная привязка:
```csharp
Bind<IRepository<Product>>()
    .To<EntityRepository<Product>>()
    .When(request => request.Target.Name == "Development");

Bind<IRepository<Product>>()
    .To<DapperRepository<Product>>()
    .When(request => request.Target.Name == "Production");
```

### 2. Named Bindings:
```csharp
Bind<IRepository<Product>>()
    .To<EntityRepository<Product>>()
    .Named("EF");

Bind<IRepository<Product>>()
    .To<DapperRepository<Product>>()
    .Named("Dapper");

// Использование:
var repo = kernel.Get<IRepository<Product>>("EF");
```

### 3. Различные Scopes:
```csharp
// Singleton - один экземпляр на всё приложение
.InSingletonScope()

// Transient - новый экземпляр при каждом запросе (по умолчанию)
.InTransientScope()

// Thread - один экземпляр на поток
.InThreadScope()
```

---

## ✅ Результаты рефакторинга

### Что было сделано:
1. ✅ Установлен Ninject во все проекты
2. ✅ Класс ProductLogic рефакторен для использования Constructor Injection
3. ✅ Создан SimpleConfigModule для настройки DI
4. ✅ RepositoryFactory переписаны для использования Ninject
5. ✅ Добавлены подробные комментарии ко всему новому коду
6. ✅ Проект успешно компилируется
7. ✅ Сохранена обратная совместимость

### Достигнутые улучшения:
- 🎯 Полное соответствие принципу DIP
- 🎯 Централизованное управление зависимостями
- 🎯 Упрощение переключения между реализациями
- 🎯 Улучшенная тестируемость
- 🎯 Следование всем принципам SOLID

---

## 🔍 Проверка работоспособности

1. **Сборка проекта:**
   ```bash
   dotnet build
   ```
   ✅ Должна пройти без ошибок

2. **Запуск приложения:**
   - WinForms, Console, WPF приложения должны работать как раньше
   - Данные должны корректно сохраняться и загружаться

3. **Переключение репозитория:**
   - Измените привязку в `SimpleConfigModule.cs`
   - Перезапустите приложение
   - Убедитесь, что новый репозиторий работает

---

## 📖 Заключение

Реализация Dependency Injection с помощью Ninject значительно улучшила архитектуру проекта:

- **Слабая связанность** между модулями
- **Высокая гибкость** при изменениях
- **Лёгкое тестирование** с mock-объектами
- **Чистый код** следующий SOLID-принципам
- **Централизованная настройка** всех зависимостей

Теперь проект полностью соответствует принципу **Dependency Inversion Principle** и готов к дальнейшему развитию и масштабированию!

---

**Дата:** 2025  
**Версия Ninject:** 3.3.6  
**Проект:** ProductManagementSystem - Лабораторная работа №3 (SOLID: DIP + DI)
