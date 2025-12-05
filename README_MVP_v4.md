# Product Management System - MVP Architecture v4.0

## ?? Описание архитектуры

Система спроектирована по паттерну **MVP (Model-View-Presenter)** согласно техническому заданию лабораторной работы №4.

```
???????????????????????
?                     ?
?     Presenter       ?  ???? Автозапускаемый элемент
?                     ?       Точка сборки (CompositionRoot)
???????????????????????
           ?
     ?????????????
     ?           ?
??????????? ??????????
?  IView  ? ? IModel ?
?  View   ? ? Model  ?
??????????? ??????????
  Passive     Active
```

## ?? Структура проектов

### 1?? **ProductManagementSystem.Presenter** ? (Автозапускаемый элемент)
**Назначение**: Точка входа MVP-архитектуры. Связывает View и Model.

**Компоненты**:
- `Program.cs` - главная точка входа приложения
- `CompositionRoot.cs` - точка сборки (DI configuration)

**Зависимости**:
- ? Logic (для IProductModel)
- ? WinFormsApp (для запуска View)
- ? DataAccessLayer (для конфигурации репозиториев)
- ? Model (для доменных классов)
- ? Shared (для интерфейсов)

**Запуск**:
```bash
dotnet run --project ProductManagementSystem.Presenter
```

---

### 2?? **ProductManagementSystem.ConsoleApp** (Консольное приложение с текстовым меню)
**Назначение**: Независимое консольное приложение с текстовым интерфейсом (MenuController).

**Компоненты**:
- `Program.cs` - точка входа консольного приложения
- `MenuController.cs` - контроллер текстового меню
- `CompositionRoot.cs` - DI конфигурация для консоли

**Особенности**:
- ? НЕ использует WinForms
- ? Использует IProductService для работы с бизнес-логикой
- ? Полностью независимое приложение

**Запуск**:
```bash
dotnet run --project ProductManagementSystem.ConsoleApp
```

---

### 3?? **ProductManagementSystem.WinFormsApp** (View - Пассивная)
**Назначение**: Графический интерфейс (View в MVP).

**Компоненты**:
- `MainForm.cs` - главная форма (реализует IProductView)
- `AddProductForm.cs` - форма добавления товара (реализует IAddProductView)
- `WinFormsRunner.cs` - вспомогательный класс запуска
- `EntryPoint.cs` - standalone entry point (показывает сообщение о запуске через Presenter)

**Особенности**:
- ? Пассивная View - только отображает данные
- ? Реализует интерфейсы из Shared
- ? Запускается через Presenter

**Самостоятельный запуск** (не рекомендуется):
```bash
dotnet run --project ProductManagementSystem.WinFormsApp
```
*Покажет сообщение: "Запускайте через Presenter"*

---

### 4?? **ProductManagementSystem.WpfApp** (Alternative View)
**Назначение**: Альтернативный графический интерфейс на WPF.

**Запуск**:
```bash
dotnet run --project ProductManagementSystem.WpfApp
```

---

### 5?? **ProductManagementSystem.Logic** (Presenter Logic + Business Logic)
**Назначение**: Бизнес-логика и презентеры.

**Компоненты**:
- `Presenters/ProductPresenter.cs` - презентер для главного окна
- `Presenters/AddProductPresenter.cs` - презентер для добавления товара
- `IProductModel.cs` - интерфейс модели
- `ProductModelMvp.cs` - реализация модели для MVP
- `ProductLogic.cs` - бизнес-логика
- `Services/ProductService.cs` - сервисный слой

---

### 6?? **ProductManagementSystem.Model** (Domain Model)
**Назначение**: Доменные классы.

**Компоненты**:
- `Product.cs` - доменная сущность товара
- `ProductModel.cs` - DTO для товара

---

### 7?? **ProductManagementSystem.DataAccessLayer** (Data Access)
**Назначение**: Слой доступа к данным.

**Компоненты**:
- `EF/EntityRepository.cs` - репозиторий на Entity Framework
- `Dapper/DapperRepository.cs` - репозиторий на Dapper
- `SimpleConfigModule.cs` - ?? УСТАРЕВШИЙ (помечен [Obsolete])

---

### 8?? **ProductManagementSystem.Shared** (Shared Interfaces)
**Назначение**: Общие интерфейсы для избежания циклических зависимостей.

**Компоненты**:
- `IProductView.cs` - интерфейс главного окна
- `IAddProductView.cs` - интерфейс формы добавления
- `ProductDto.cs` - DTO для передачи данных

---

## ?? Автозапускаемые элементы (Startup Projects)

В решении доступны **4 точки входа**:

| # | Проект | Описание | Рекомендация |
|---|--------|----------|--------------|
| 1 | **Presenter** ? | MVP архитектура, запускает WinForms | **Рекомендуется** |
| 2 | **ConsoleApp** | Консольное меню | Для консольной работы |
| 3 | **WinFormsApp** | Standalone WinForms (не рекомендуется) | Показывает сообщение |
| 4 | **WpfApp** | WPF приложение | Альтернативный UI |

### Установка автозапускаемого проекта в Visual Studio

1. Правой кнопкой мыши на **ProductManagementSystem.Presenter**
2. Выберите **"Назначить запускаемым проектом"** (Set as Startup Project)

### Или из командной строки:

```bash
# Запуск Presenter (рекомендуется)
dotnet run --project ProductManagementSystem.Presenter

# Запуск ConsoleApp
dotnet run --project ProductManagementSystem.ConsoleApp

# Запуск WinFormsApp (покажет сообщение)
dotnet run --project ProductManagementSystem.WinFormsApp

# Запуск WpfApp
dotnet run --project ProductManagementSystem.WpfApp
```

---

## ??? Соответствие техническому заданию

### ? Требования MVP

- [x] **Model + BL**: Доменные классы, бизнес-логика, интерфейсы
- [x] **Presenter**: Классы-презентеры подписаны на события View
- [x] **View**: Генерирует события, отображает данные (пассивная)
- [x] **Shared**: Библиотека интерфейсов (избегаем циклических ссылок)
- [x] **Точка сборки**: В Presenter (CompositionRoot)

### ? Принципы SOLID

- [x] **S** (Single Responsibility): Каждый класс отвечает за одну задачу
- [x] **O** (Open/Closed): Расширение через интерфейсы
- [x] **L** (Liskov Substitution): Переключение между EF/Dapper
- [x] **I** (Interface Segregation): Разделенные интерфейсы (IProductView, IAddProductView)
- [x] **D** (Dependency Inversion): DI контейнер (Ninject)

---

## ?? Граф зависимостей

```
Presenter
  ??? Logic (IProductModel, Presenters)
  ??? WinFormsApp (View)
  ??? DataAccessLayer (Repository)
  ??? Model (Domain)
  ??? Shared (Interfaces)

ConsoleApp
  ??? Logic (IProductService)
  ??? DataAccessLayer (Repository)
  ??? Model (Domain)

WinFormsApp
  ??? Logic (Presenters)
  ??? Shared (Interfaces)

Logic
  ??? Model (Domain)
  ??? DataAccessLayer (IRepository)
  ??? Shared (Interfaces)

DataAccessLayer
  ??? Model (Domain)
```

---

## ?? Итоги выполнения ЛР №4

1. ? Создан проект **Presenter** как автозапускаемый элемент
2. ? Точка сборки (CompositionRoot) перенесена в Presenter
3. ? ConsoleApp превращен в независимое консольное приложение
4. ? WinFormsApp - пассивная View, запускается через Presenter
5. ? Presenter связывает View и Model на 100%
6. ? Все 4 автозапускаемых элемента работают корректно
7. ? Соблюдены все принципы SOLID
8. ? Реализован паттерн MVP согласно ТЗ

---

## ?? Дополнительная информация

### Переключение между Entity Framework и Dapper

В файле `ProductManagementSystem.Presenter\CompositionRoot.cs`:

```csharp
// Entity Framework (по умолчанию)
Bind<IRepository<Product>>().To<EntityRepository<Product>>().InSingletonScope();

// Или Dapper (раскомментировать)
// Bind<IRepository<Product>>().To<DapperRepository<Product>>().InSingletonScope();
```

### База данных

Приложение использует SQLite базу данных `products.db`.
Создается автоматически при первом запуске.

---

**Автор**: MVP Architecture Team  
**Версия**: 4.0  
**Дата**: 2024  
