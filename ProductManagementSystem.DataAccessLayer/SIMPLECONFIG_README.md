# SimpleConfigModule - Ninject Configuration Module

## Назначение

`SimpleConfigModule` - это модуль конфигурации для Ninject DI-контейнера, который определяет правила привязки интерфейсов к их конкретным реализациям.

## Расположение

```
ProductManagementSystem.DataAccessLayer/SimpleConfigModule.cs
```

## Структура

```csharp
public class SimpleConfigModule : NinjectModule
{
    public override void Load()
    {
        // Здесь определяются привязки (bindings)
        Bind<IRepository<Product>>()
            .To<EntityRepository<Product>>()
            .InSingletonScope();
    }
}
```

## Компоненты привязки

### 1. Bind<TInterface>()
Указывает, какой интерфейс мы хотим привязать.

### 2. To<TImplementation>()
Указывает, какую конкретную реализацию использовать.

### 3. InSingletonScope()
Указывает, что экземпляр создаётся один раз на всё приложение.

## Жизненные циклы (Scopes)

### InSingletonScope()
```csharp
Bind<IRepository<Product>>().To<EntityRepository<Product>>().InSingletonScope();
```
- Экземпляр создаётся **один раз**
- Все запросы получают **тот же экземпляр**
- **Рекомендуется** для репозиториев

### InTransientScope() (по умолчанию)
```csharp
Bind<IRepository<Product>>().To<EntityRepository<Product>>().InTransientScope();
```
- Создаётся **новый экземпляр** при каждом запросе
- Используется для stateless объектов

### InThreadScope()
```csharp
Bind<IRepository<Product>>().To<EntityRepository<Product>>().InThreadScope();
```
- Один экземпляр **на поток**
- Редко используется

## Примеры использования

### Переключение между Entity Framework и Dapper

```csharp
public override void Load()
{
    // Entity Framework
    Bind<IRepository<Product>>().To<EntityRepository<Product>>().InSingletonScope();
    
    // Для переключения на Dapper закомментируйте EF и раскомментируйте это:
    // Bind<IRepository<Product>>().To<DapperRepository<Product>>().InSingletonScope();
}
```

### Добавление новой сущности

```csharp
public override void Load()
{
    // Product
    Bind<IRepository<Product>>().To<EntityRepository<Product>>().InSingletonScope();
    
    // Order (новая сущность)
    Bind<IRepository<Order>>().To<EntityRepository<Order>>().InSingletonScope();
    
    // Customer (новая сущность)
    Bind<IRepository<Customer>>().To<EntityRepository<Customer>>().InSingletonScope();
}
```

### Условная привязка

```csharp
public override void Load()
{
    // В Development используем Entity Framework
    Bind<IRepository<Product>>()
        .To<EntityRepository<Product>>()
        .When(x => IsDebug())
        .InSingletonScope();
    
    // В Production используем Dapper
    Bind<IRepository<Product>>()
        .To<DapperRepository<Product>>()
        .When(x => !IsDebug())
        .InSingletonScope();
}

private bool IsDebug()
{
    #if DEBUG
        return true;
    #else
        return false;
    #endif
}
```

### Named Bindings

```csharp
public override void Load()
{
    // Именованные привязки
    Bind<IRepository<Product>>()
        .To<EntityRepository<Product>>()
        .Named("EF")
        .InSingletonScope();
    
    Bind<IRepository<Product>>()
        .To<DapperRepository<Product>>()
        .Named("Dapper")
        .InSingletonScope();
}

// Использование:
var efRepo = kernel.Get<IRepository<Product>>("EF");
var dapperRepo = kernel.Get<IRepository<Product>>("Dapper");
```

### Привязка с параметрами конструктора

```csharp
public override void Load()
{
    // С пользовательской строкой подключения
    Bind<IRepository<Product>>()
        .To<DapperRepository<Product>>()
        .WithConstructorArgument("connectionString", 
            "Server=MyServer;Database=MyDB;...")
        .InSingletonScope();
}
```

## Использование в приложении

### Загрузка модуля

```csharp
// В RepositoryFactory
private static IKernel Kernel
{
    get
    {
        if (_kernel == null)
        {
            // Загружаем SimpleConfigModule
            _kernel = new StandardKernel(new SimpleConfigModule());
        }
        return _kernel;
    }
}
```

### Получение зависимостей

```csharp
// Ninject автоматически создаст правильную реализацию
var repository = Kernel.Get<IRepository<Product>>();

// Использование в бизнес-логике
var productLogic = new ProductLogic(repository);
```

## Расширение модуля

### Множественные модули

```csharp
// Создайте дополнительный модуль
public class DatabaseConfigModule : NinjectModule
{
    public override void Load()
    {
        Bind<IRepository<Product>>().To<EntityRepository<Product>>().InSingletonScope();
    }
}

public class ServiceConfigModule : NinjectModule
{
    public override void Load()
    {
        Bind<IEmailService>().To<SmtpEmailService>().InSingletonScope();
        Bind<ILoggingService>().To<FileLoggingService>().InSingletonScope();
    }
}

// Загрузка всех модулей
_kernel = new StandardKernel(
    new DatabaseConfigModule(),
    new ServiceConfigModule()
);
```

### Динамическая конфигурация

```csharp
public class SimpleConfigModule : NinjectModule
{
    private readonly string _environment;
    
    public SimpleConfigModule(string environment)
    {
        _environment = environment;
    }
    
    public override void Load()
    {
        if (_environment == "Development")
        {
            Bind<IRepository<Product>>().To<EntityRepository<Product>>().InSingletonScope();
        }
        else
        {
            Bind<IRepository<Product>>().To<DapperRepository<Product>>().InSingletonScope();
        }
    }
}

// Использование
_kernel = new StandardKernel(new SimpleConfigModule("Development"));
```

## Лучшие практики

### ? Рекомендуется:
- Использовать `InSingletonScope()` для репозиториев
- Группировать связанные привязки вместе
- Добавлять комментарии к каждой привязке
- Один модуль на логическую группу зависимостей

### ? Избегайте:
- Слишком сложной логики в методе `Load()`
- Прямого создания объектов (`new`) внутри модуля
- Привязок, зависящих от порядка загрузки
- Циклических зависимостей

## Отладка

### Проверка привязок

```csharp
// Получить все привязки для типа
var bindings = kernel.GetBindings(typeof(IRepository<Product>));
foreach (var binding in bindings)
{
    Console.WriteLine($"Binding: {binding.Target}");
}
```

### Логирование

```csharp
public override void Load()
{
    Bind<IRepository<Product>>()
        .To<EntityRepository<Product>>()
        .InSingletonScope()
        .OnActivation((context, instance) => 
        {
            Console.WriteLine($"Created: {instance.GetType().Name}");
        });
}
```

## Тестирование

### Unit-тесты с mock-объектами

```csharp
[TestClass]
public class ProductLogicTests
{
    [TestMethod]
    public void TestWithMockRepository()
    {
        // Создаём mock-репозиторий
        var mockRepo = new Mock<IRepository<Product>>();
        mockRepo.Setup(r => r.ReadAll()).Returns(new List<Product>());
        
        // Внедряем mock через конструктор (без Ninject)
        var logic = new ProductLogic(mockRepo.Object);
        
        // Тестируем
        var products = logic.GetAllProducts();
        Assert.AreEqual(0, products.Count);
    }
}
```

### Интеграционные тесты с Ninject

```csharp
[TestClass]
public class IntegrationTests
{
    [TestMethod]
    public void TestWithRealRepository()
    {
        // Создаём kernel с тестовым модулем
        var kernel = new StandardKernel(new TestConfigModule());
        
        // Получаем реальную реализацию
        var repo = kernel.Get<IRepository<Product>>();
        var logic = new ProductLogic(repo);
        
        // Тестируем
        var products = logic.GetAllProducts();
        Assert.IsNotNull(products);
    }
}

public class TestConfigModule : NinjectModule
{
    public override void Load()
    {
        // Используем in-memory репозиторий для тестов
        Bind<IRepository<Product>>().To<InMemoryRepository<Product>>().InSingletonScope();
    }
}
```

## FAQ

### Q: Почему используется InSingletonScope()?
**A:** Для репозиториев это оптимально, так как они stateless и не хранят состояние между вызовами. Создание одного экземпляра экономит память.

### Q: Как добавить новую сущность?
**A:** Добавьте новую привязку в метод `Load()`:
```csharp
Bind<IRepository<NewEntity>>().To<EntityRepository<NewEntity>>().InSingletonScope();
```

### Q: Можно ли использовать разные репозитории для разных сущностей?
**A:** Да:
```csharp
Bind<IRepository<Product>>().To<EntityRepository<Product>>().InSingletonScope();
Bind<IRepository<Order>>().To<DapperRepository<Order>>().InSingletonScope();
```

### Q: Как переключиться на другую реализацию?
**A:** Измените `To<>()` на нужную реализацию и перезапустите приложение.

## Связанные файлы

- `ProductManagementSystem.Core/IRepository.cs` - Интерфейс репозитория
- `ProductManagementSystem.DataAccessLayer/EF/EntityRepository.cs` - Реализация EF
- `ProductManagementSystem.DataAccessLayer/Dapper/DapperRepository.cs` - Реализация Dapper
- `ProductManagementSystem.*/RepositoryFactory.cs` - Фабрики, использующие модуль

## Документация

Подробнее о Ninject:
- [Ninject Documentation](http://www.ninject.org/)
- [GitHub Repository](https://github.com/ninject/ninject)
- [Dependency Injection in .NET](https://docs.microsoft.com/en-us/dotnet/core/extensions/dependency-injection)

## Версия

- **Ninject:** 3.3.6
- **Модуль:** SimpleConfigModule
- **Проект:** ProductManagementSystem
- **Лабораторная работа:** №3 (SOLID: DIP + DI)

---

**Последнее обновление:** 2025  
**Автор:** ProductManagementSystem Team
