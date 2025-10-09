# Архитектура Data Access Layer

## Диаграмма классов

```
┌─────────────────────────────────────────────────────────────────┐
│                         View Layer                              │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐          │
│  │  WPF App     │  │ WinForms App │  │ Console App  │          │
│  │              │  │              │  │              │          │
│  └──────┬───────┘  └──────┬───────┘  └──────┬───────┘          │
│         │                  │                  │                  │
│         └──────────────────┴──────────────────┘                  │
│                            │                                     │
└────────────────────────────┼─────────────────────────────────────┘
                             │
┌────────────────────────────┼─────────────────────────────────────┐
│                  Business Logic Layer                            │
│                            ▼                                     │
│                   ┌──────────────────┐                           │
│                   │  ProductLogic    │                           │
│                   │                  │                           │
│                   │ - _repository    │◄──────┐                  │
│                   │ - _products      │       │                  │
│                   │                  │       │                  │
│                   │ + AddProduct()   │       │                  │
│                   │ + GetProduct()   │       │ Dependency       │
│                   │ + UpdateProduct()│       │ Injection        │
│                   │ + DeleteProduct()│       │                  │
│                   │ + GetAll()       │       │                  │
│                   │ + FilterBy...()  │       │                  │
│                   │ + Calculate...() │       │                  │
│                   └──────────────────┘       │                  │
└────────────────────────────┬─────────────────┼───────────────────┘
                             │                 │
┌────────────────────────────┼─────────────────┼───────────────────┐
│                  Data Access Layer           │                   │
│                            ▼                 │                   │
│              ┌──────────────────────────┐    │                   │
│              │   IRepository<T>         │◄───┘                   │
│              │  where T : IDomainObject │                        │
│              │                          │                        │
│              │ + Add(T entity)          │                        │
│              │ + Delete(int id)         │                        │
│              │ + ReadAll()              │                        │
│              │ + ReadById(int id)       │                        │
│              │ + Update(T entity)       │                        │
│              └─────────┬────────────────┘                        │
│                        │                                         │
│         ┌──────────────┴──────────────┐                          │
│         │                              │                          │
│         ▼                              ▼                          │
│  ┌─────────────────┐         ┌─────────────────┐                │
│  │EntityRepository │         │DapperRepository │                │
│  │<T>              │         │<T>              │                │
│  │                 │         │                 │                │
│  │ Uses:           │         │ Uses:           │                │
│  │ - AppDbContext  │         │ - SqlConnection │                │
│  │ - DbContext     │         │ - Dapper        │                │
│  │ - LINQ          │         │ - Raw SQL       │                │
│  │                 │         │                 │                │
│  │ + Add()         │         │ + Add()         │                │
│  │ + Delete()      │         │ + Delete()      │                │
│  │ + ReadAll()     │         │ + ReadAll()     │                │
│  │ + ReadById()    │         │ + ReadById()    │                │
│  │ + Update()      │         │ + Update()      │                │
│  └────────┬────────┘         └────────┬────────┘                │
│           │                           │                          │
└───────────┼───────────────────────────┼──────────────────────────┘
            │                           │
┌───────────┼───────────────────────────┼──────────────────────────┐
│           │        Model Layer        │                          │
│           │                           │                          │
│           ▼                           ▼                          │
│  ┌─────────────────┐         ┌─────────────────┐                │
│  │ IDomainObject   │         │  Product        │                │
│  │                 │◄────────│ : IDomainObject │                │
│  │ + int Id        │         │                 │                │
│  └─────────────────┘         │ + int Id        │                │
│                               │ + string Name   │                │
│                               │ + string Desc   │                │
│                               │ + decimal Price │                │
│                               │ + string Category│               │
│                               │ + int StockQty  │                │
│                               └─────────┬────────┘                │
└─────────────────────────────────────────┼──────────────────────────┘
                                          │
┌─────────────────────────────────────────┼──────────────────────────┐
│                    Database Layer       ▼                          │
│                                                                    │
│                    ┌────────────────────┐                          │
│                    │ SQL Server LocalDB │                          │
│                    │                    │                          │
│                    │  ProductDatabase   │                          │
│                    │  ┌──────────────┐  │                          │
│                    │  │   Products   │  │                          │
│                    │  │ ──────────── │  │                          │
│                    │  │ Id (PK)      │  │                          │
│                    │  │ Name         │  │                          │
│                    │  │ Description  │  │                          │
│                    │  │ Price        │  │                          │
│                    │  │ Category     │  │                          │
│                    │  │ StockQuantity│  │                          │
│                    │  └──────────────┘  │                          │
│                    └────────────────────┘                          │
└─────────────────────────────────────────────────────────────────────┘
```

## Поток данных

### 1. Создание товара (Create)
```
User Input (View)
    ↓
ProductLogic.AddProduct(product)
    ↓
IRepository<Product>.Add(product)
    ↓
┌─────────────────┬─────────────────┐
│  Entity Framework   │     Dapper      │
│                 │                 │
│ context.Set<Product>() │ SQL: INSERT INTO  │
│   .Add(product) │   Products...   │
│ context.SaveChanges() │ connection.Execute() │
└─────────────────┴─────────────────┘
    ↓
SQL Server LocalDB (Products table)
```

### 2. Чтение товара (Read)
```
User Request (View)
    ↓
ProductLogic.GetAllProducts()
    ↓
IRepository<Product>.ReadAll()
    ↓
┌─────────────────┬─────────────────┐
│  Entity Framework   │     Dapper      │
│                 │                 │
│ context.Set<Product>() │ SQL: SELECT *  │
│   .ToList()     │   FROM Products │
│                 │ connection.Query<T>() │
└─────────────────┴─────────────────┘
    ↓
List<Product>
    ↓
View displays products
```

### 3. Обновление товара (Update)
```
User Edit (View)
    ↓
ProductLogic.UpdateProduct(product)
    ↓
IRepository<Product>.Update(product)
    ↓
┌─────────────────┬─────────────────┐
│  Entity Framework   │     Dapper      │
│                 │                 │
│ context.Entry(product) │ SQL: UPDATE  │
│   .State = Modified │   Products SET... │
│ context.SaveChanges() │ connection.Execute() │
└─────────────────┴─────────────────┘
    ↓
SQL Server LocalDB (Products table updated)
```

### 4. Удаление товара (Delete)
```
User Delete (View)
    ↓
ProductLogic.DeleteProduct(id)
    ↓
IRepository<Product>.Delete(id)
    ↓
┌─────────────────┬─────────────────┐
│  Entity Framework   │     Dapper      │
│                 │                 │
│ entity = context.Set<T>() │ SQL: DELETE FROM │
│   .Find(id)     │   Products    │
│ context.Set<T>() │   WHERE Id=@Id  │
│   .Remove(entity) │ connection.Execute() │
│ context.SaveChanges() │                 │
└─────────────────┴─────────────────┘
    ↓
SQL Server LocalDB (Product removed)
```

## Паттерны проектирования

### 1. Repository Pattern
- Абстракция доступа к данным
- Разделение бизнес-логики и логики доступа к данным
- `IRepository<T>` - универсальный интерфейс

### 2. Dependency Injection
- ProductLogic получает репозиторий через конструктор
- Слабая связанность компонентов
- Легкость тестирования и замены реализаций

### 3. Generic Programming
- `IRepository<T> where T : IDomainObject`
- Переиспользование кода для разных типов
- Типобезопасность

### 4. Strategy Pattern
- Две стратегии работы с БД: EF и Dapper
- Легкое переключение между стратегиями
- Одинаковый интерфейс для разных реализаций

### 5. Unit of Work (частично)
- `using` блоки для DbContext и SqlConnection
- Автоматическое управление транзакциями
- Правильное освобождение ресурсов

## Преимущества архитектуры

### 1. Гибкость
- Легко переключиться между EF и Dapper
- Можно добавить новые реализации (например, для PostgreSQL)
- Поддержка работы без БД (in-memory режим)

### 2. Тестируемость
- Можно создать Mock репозиторий для Unit-тестов
- Бизнес-логика изолирована от источника данных
- DI упрощает создание тестов

### 3. Масштабируемость
- Легко добавлять новые модели
- Паттерн Repository можно применить к другим сущностям
- Поддержка асинхронных операций (будущее развитие)

### 4. Поддерживаемость
- Чистое разделение ответственности
- Каждый слой имеет свою четкую роль
- Легко понять и модифицировать код

### 5. Производительность
- EF для сложных запросов с оптимизацией
- Dapper для простых операций с максимальной скоростью
- Выбор инструмента под конкретную задачу

## Сравнение Entity Framework и Dapper

| Характеристика | Entity Framework | Dapper |
|----------------|------------------|--------|
| **Тип ORM** | Full ORM | Micro-ORM |
| **Производительность** | Средняя | Высокая |
| **Сложность** | Высокая | Низкая |
| **Code First** | ✅ Да | ❌ Нет |
| **LINQ Support** | ✅ Полная | ❌ Нет |
| **Change Tracking** | ✅ Да | ❌ Нет |
| **Lazy Loading** | ✅ Да | ❌ Нет |
| **Raw SQL** | ⚠️ Поддерживается | ✅ Основной способ |
| **Migrations** | ✅ Да | ❌ Нет |
| **Размер библиотеки** | ~5 MB | ~100 KB |
| **Кривая обучения** | Крутая | Пологая |
| **Использование** | Сложные запросы | Простые CRUD |
| **Контроль SQL** | Низкий | Высокий |

## Когда использовать что?

### Entity Framework:
- ✅ Сложные запросы с множеством JOIN
- ✅ Работа со связанными сущностями
- ✅ Нужны миграции и Code First
- ✅ Разработка прототипов и MVP
- ✅ Команда не знает SQL хорошо

### Dapper:
- ✅ Простые CRUD операции
- ✅ Критична производительность
- ✅ Существующая база данных
- ✅ Нужен полный контроль над SQL
- ✅ Большие объемы данных

### In-Memory (без репозитория):
- ✅ Прототипирование UI
- ✅ Демонстрация без БД
- ✅ Unit-тесты бизнес-логики
- ✅ Временные данные сессии
