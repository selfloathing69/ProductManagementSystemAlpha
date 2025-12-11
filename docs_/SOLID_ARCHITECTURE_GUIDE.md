# Реализация разделения CRUD и бизнес-функций согласно SOLID

## ?? Описание изменений

Согласно требованиям преподавателя, логика была разделена на **два независимых слоя**:

1. **CRUD операции** ? выполняются через `IRepository<T>`
2. **Бизнес-функции** ? выполняются через `IBusinessFunctions`

---

## ??? Новая архитектура

```
???????????????????????????????????????????????????
?              ProductLogic : ILogic              ?
?  (Координирует CRUD и бизнес-функции)          ?
???????????????????????????????????????????????????
         ?                       ?
         ?                       ?
????????????????????    ????????????????????????
?  IRepository<T>  ?    ? IBusinessFunctions   ?
?  (CRUD операции) ?    ? (Бизнес-логика)     ?
????????????????????    ????????????????????????
         ?                          ?
    ???????????                    ?
    ?         ?                    ?
???????? ??????????     ????????????????????
?  EF  ? ? Dapper ?     ?BusinessFunctions ?
???????? ??????????     ????????????????????
```

---

## ?? Созданные файлы

### **1. IBusinessFunctions.cs**
**Путь:** `ProductManagementSystem.Logic/IBusinessFunctions.cs`

**Назначение:** Интерфейс для бизнес-функций (фильтрация, поиск, расчёты, группировка)

**Методы:**
- `FilterByCategory()` — фильтрация по категории
- `GroupByCategory()` — группировка по категориям
- `CalculateTotalInventoryValue()` — расчёт общей стоимости склада
- `FindByNameAndCategory()` — поиск товара
- `Search()` — поиск по запросу
- `ExistsById()` — проверка существования

---

### **2. BusinessFunctions.cs**
**Путь:** `ProductManagementSystem.Logic/BusinessFunctions.cs`

**Назначение:** Реализация бизнес-функций

**Принципы SOLID:**
- **S (Single Responsibility):** Отвечает ТОЛЬКО за бизнес-логику
- **O (Open/Closed):** Можно расширять новыми функциями без изменения CRUD

---

### **3. ILogic.cs**
**Путь:** `ProductManagementSystem.Logic/ILogic.cs`

**Назначение:** Интерфейс для `ProductLogic`

**Методы:**
- **CRUD:** `Add()`, `GetById()`, `GetAll()`, `Update()`, `Delete()`
- **Бизнес-функции:** `FilterByCategory()`, `GroupByCategory()`, `CalculateTotalInventoryValue()`
- **Дополнительные:** `AddQuantityToProduct()`, `RemoveQuantityFromProduct()`, `AddProductWithValidation()`, `DeleteProductByQuantity()`

---

## ?? Изменённые файлы

### **ProductLogic.cs**

**Ключевые изменения:**

1. **Теперь реализует `ILogic`:**
```csharp
public class ProductLogic : ILogic
```

2. **Использует две зависимости:**
```csharp
private readonly IRepository<Product>? _repository;        // CRUD
private readonly IBusinessFunctions _businessFunctions;    // Бизнес-логика
```

3. **Constructor Injection (Ninject):**
```csharp
public ProductLogic(IRepository<Product>? repository, IBusinessFunctions businessFunctions)
{
    _repository = repository;
    _businessFunctions = businessFunctions;
}
```

4. **CRUD методы делегируются в IRepository:**
```csharp
public Product Add(Product product)
{
    _repository.Add(product);  // Вызов репозитория
    return product;
}

public List<Product> GetAll()
{
    return _repository.ReadAll().ToList();  // Вызов репозитория
}
```

5. **Бизнес-функции делегируются в IBusinessFunctions:**
```csharp
public List<Product> FilterByCategory(string category)
{
    var allProducts = GetAll();
    return _businessFunctions.FilterByCategory(allProducts, category);  // Вызов бизнес-функций
}

public decimal CalculateTotalInventoryValue()
{
    var allProducts = GetAll();
    return _businessFunctions.CalculateTotalInventoryValue(allProducts);  // Вызов бизнес-функций
}
```

---

### **SimpleConfigModule.cs**

**Добавлены привязки:**

```csharp
public override void Load()
{
    // ===== CRUD операции =====
    Bind<IRepository<Product>>().To<EntityRepository<Product>>().InSingletonScope();
    // Bind<IRepository<Product>>().To<DapperRepository<Product>>().InSingletonScope();

    // ===== Бизнес-функции =====
    Bind<IBusinessFunctions>().To<BusinessFunctions>().InSingletonScope();

    // ===== ProductLogic (координатор) =====
    Bind<ILogic>().To<ProductLogic>().InSingletonScope();

    // ===== Сервисы =====
    Bind<ProductMapper>().ToSelf().InSingletonScope();
    Bind<ProductValidator>().ToSelf().InSingletonScope();
    Bind<IProductService>().To<ProductService>().InSingletonScope();
}
```

**Как Ninject создаёт ProductLogic:**
1. Видит запрос на `ProductLogic`
2. Смотрит на конструктор: `ProductLogic(IRepository<Product>?, IBusinessFunctions)`
3. Создаёт `EntityRepository<Product>` (или `DapperRepository`)
4. Создаёт `BusinessFunctions`
5. Передаёт их в конструктор `ProductLogic`
6. Возвращает готовый объект

---

### **RepositoryFactory.cs** (WPF и WinForms)

**Добавлены методы:**

```csharp
/// <summary>
/// Создаёт ProductLogic через Ninject с автоматическим внедрением зависимостей.
/// </summary>
public static ProductLogic CreateProductLogic()
{
    return Kernel.Get<ProductLogic>();
}

/// <summary>
/// Создаёт ILogic через Ninject (рекомендуемый способ).
/// </summary>
public static ILogic CreateLogic()
{
    return Kernel.Get<ILogic>();
}
```

---

## ?? Как использовать новую архитектуру

### **Вариант 1: Через Ninject (рекомендуется)**

```csharp
// В WPF/WinForms/ConsoleApp
var productLogic = RepositoryFactory.CreateProductLogic();

// Ninject автоматически создаст:
// - IRepository<Product> ? EntityRepository<Product>
// - IBusinessFunctions ? BusinessFunctions
// - ProductLogic с этими зависимостями
```

### **Вариант 2: Вручную (для тестов)**

```csharp
IRepository<Product> repository = new EntityRepository<Product>();
IBusinessFunctions businessFunctions = new BusinessFunctions();
ProductLogic logic = new ProductLogic(repository, businessFunctions);
```

---

## ?? Примеры использования

### **CRUD операции (через IRepository)**

```csharp
var logic = RepositoryFactory.CreateProductLogic();

// CREATE
var product = new Product { Name = "Мышь", Price = 1500 };
logic.Add(product);  // ? вызывает _repository.Add()

// READ
var allProducts = logic.GetAll();  // ? вызывает _repository.ReadAll()
var oneProduct = logic.GetById(5);  // ? вызывает _repository.ReadById()

// UPDATE
product.Price = 2000;
logic.Update(product);  // ? вызывает _repository.Update()

// DELETE
logic.Delete(5);  // ? вызывает _repository.Delete()
```

### **Бизнес-функции (через IBusinessFunctions)**

```csharp
var logic = RepositoryFactory.CreateProductLogic();

// Фильтрация по категории
var electronics = logic.FilterByCategory("Электроника");
// ? Вызывает _businessFunctions.FilterByCategory()

// Группировка по категориям
var grouped = logic.GroupByCategory();
// ? Вызывает _businessFunctions.GroupByCategory()

// Расчёт общей стоимости
var totalValue = logic.CalculateTotalInventoryValue();
// ? Вызывает _businessFunctions.CalculateTotalInventoryValue()

// Поиск
var found = logic.SearchProducts("Samsung");
// ? Вызывает _businessFunctions.Search()
```

---

## ? Соответствие требованиям преподавателя

### **1. Logic разделена на CRUD и BusinessFunctions ?**
- **CRUD** ? `IRepository<T>` (методы: Add, Delete, Update, ReadAll, ReadById)
- **BusinessFunctions** ? `IBusinessFunctions` (методы: FilterByCategory, GroupByCategory, Calculate, Search)

### **2. Logic наследуется от ILogic ?**
```csharp
public class ProductLogic : ILogic
```

### **3. Инкапсуляция зависимостей ?**
```csharp
public class ProductLogic : ILogic
{
    private readonly IRepository<Product>? _repository;        // CRUD
    private readonly IBusinessFunctions _businessFunctions;    // Бизнес-логика
}
```

### **4. Методы делегируют вызовы ?**
- `Add()` ? вызывает `_repository.Add()`
- `FilterByCategory()` ? вызывает `_businessFunctions.FilterByCategory()`
- `GroupByCategory()` ? вызывает `_businessFunctions.GroupByCategory()`

### **5. Ninject управляет зависимостями ?**
```csharp
// В SimpleConfigModule
Bind<IRepository<Product>>().To<EntityRepository<Product>>();
Bind<IBusinessFunctions>().To<BusinessFunctions>();
Bind<ILogic>().To<ProductLogic>();
```

---

## ?? Обратная совместимость

Старые методы помечены как `[Obsolete]`, но продолжают работать:

```csharp
[Obsolete("Используйте Add() вместо AddProduct()")]
public Product AddProduct(Product product) => Add(product);

[Obsolete("Используйте GetAll() вместо GetAllProducts()")]
public List<Product> GetAllProducts() => GetAll();
```

**Рекомендуется** переходить на новые методы (`Add`, `GetAll`, `Update`, `Delete`).

---

## ?? Для защиты перед преподавателем

### **Вопрос 1: Где находятся CRUD операции?**
**Ответ:** В `IRepository<T>`. Методы: `Add`, `Delete`, `Update`, `ReadAll`, `ReadById`.

### **Вопрос 2: Где находятся бизнес-функции?**
**Ответ:** В `IBusinessFunctions`. Методы: `FilterByCategory`, `GroupByCategory`, `CalculateTotalInventoryValue`, `Search`.

### **Вопрос 3: Как ProductLogic использует эти слои?**
**Ответ:** 
- Метод `Add()` ? вызывает `_repository.Add()` (CRUD)
- Метод `FilterByCategory()` ? вызывает `_businessFunctions.FilterByCategory()` (бизнес-логика)

### **Вопрос 4: Как Ninject управляет зависимостями?**
**Ответ:** В `SimpleConfigModule` зарегистрированы привязки:
```csharp
Bind<IRepository<Product>>().To<EntityRepository<Product>>();
Bind<IBusinessFunctions>().To<BusinessFunctions>();
```
Ninject автоматически создаёт и внедряет эти зависимости в конструктор `ProductLogic`.

### **Вопрос 5: Почему два объекта создаются через Ninject, а не напрямую?**
**Ответ:** Это устраняет жёсткую зависимость (`new EntityRepository()`, `new BusinessFunctions()`). Вместо этого `ProductLogic` зависит от абстракций (`IRepository`, `IBusinessFunctions`), что соответствует принципу **D** из SOLID.

---

## ?? Переключение между Entity Framework и Dapper

Достаточно изменить **одну строку** в `SimpleConfigModule.cs`:

```csharp
// Entity Framework
Bind<IRepository<Product>>().To<EntityRepository<Product>>().InSingletonScope();

// Dapper
// Bind<IRepository<Product>>().To<DapperRepository<Product>>().InSingletonScope();
```

Весь остальной код (UI, `ProductLogic`, `IBusinessFunctions`) остаётся **без изменений**!

---

## ?? Файлы для изучения

| Файл | Описание |
|------|----------|
| `IBusinessFunctions.cs` | Интерфейс бизнес-функций |
| `BusinessFunctions.cs` | Реализация бизнес-функций |
| `ILogic.cs` | Интерфейс ProductLogic |
| `ProductLogic.cs` | Координатор CRUD + бизнес-функции |
| `SimpleConfigModule.cs` | Настройка Ninject (DI) |
| `RepositoryFactory.cs` | Фабрика для создания объектов через Ninject |

---

**Готово к защите!** ?
