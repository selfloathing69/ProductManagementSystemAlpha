# ??? MVVM с ViewModelFirst - Система управления товарами

## ?? О проекте

Это WPF приложение, реализующее **паттерн MVVM с подходом ViewModelFirst**.

Проект демонстрирует:
- ? Правильную архитектуру MVVM
- ? ViewModelFirst подход к инициализации
- ? Dependency Injection (Ninject)
- ? SOLID принципы
- ? Современный дизайн WPF интерфейса

---

## ?? Быстрый старт

### Запуск приложения:
1. Откройте `ProductManagementSystem.sln` в Visual Studio
2. Установите проект `ProductManagementSystem.WpfApp` как стартовый
3. Нажмите `F5` для запуска

### Первый запуск:
- База данных создается автоматически (SQLite)
- Тестовые данные загружаются при первом запуске
- Окно приложения откроется с таблицей товаров

---

## ?? Документация

### Для быстрого понимания:
- **[ШПАРГАЛКА.md](ШПАРГАЛКА.md)** - Краткие ответы на типовые вопросы
- **[SUMMARY.md](SUMMARY.md)** - Резюме реализации

### Для защиты работы:
- **[ЗАЩИТА_MVVM.md](ЗАЩИТА_MVVM.md)** - Структурированный текст защиты
- **[АРХИТЕКТУРА_ДИАГРАММЫ.md](АРХИТЕКТУРА_ДИАГРАММЫ.md)** - Визуальные схемы

### Для глубокого изучения:
- **[MVVM_ViewModelFirst_Documentation.md](MVVM_ViewModelFirst_Documentation.md)** - Полная документация

---

## ??? Архитектура

### Общая структура:

```
???????????????????????????????????????????????
?  VIEW (MainWindow.xaml)                     ?
?  - XAML разметка                            ?
?  - Минимальный CodeBehind                   ?
???????????????????????????????????????????????
                    ? Binding
???????????????????????????????????????????????
?  VIEWMODEL (MainViewModel.cs)               ?
?  - Команды (ICommand)                       ?
?  - ObservableCollection<ProductDto>         ?
?  - Логика представления                     ?
???????????????????????????????????????????????
                    ? ILogic
???????????????????????????????????????????????
?  MODEL (ProductLogic.cs)                    ?
?  - Бизнес-логика                            ?
?  - Работа с Repository                      ?
???????????????????????????????????????????????
                    ? IRepository
???????????????????????????????????????????????
?  DATA ACCESS (EntityRepository.cs)          ?
?  - Entity Framework                         ?
?  - База данных                              ?
???????????????????????????????????????????????
```

### ViewModelFirst поток:

```
App.OnStartup()
  ?
ConfigureDependencyInjection()
  ?
ViewManager.Create()
  ?
MainViewModel.Create() ??? ViewModelFirst!
  ?
ViewManager.ShowMainWindow(viewModel)
  ?
MainWindow открывается
```

---

## ?? Структура проекта

```
ProductManagementSystem.WpfApp/
?
??? ?? App.xaml.cs                    # Composition Root (ViewModelFirst)
?
??? ?? Views/
?   ??? BaseView.cs                   # Базовый класс окон
?   ??? MainWindow.xaml               # UI разметка
?   ??? MainWindow.xaml.cs            # Минимальный CodeBehind
?
??? ?? ViewModels/
?   ??? ViewModelBase.cs              # Базовый класс ViewModel
?   ??? MainViewModel.cs              # ViewModel главного окна
?
??? ?? Commands/
?   ??? RelayCommand.cs               # Реализация ICommand
?
??? ?? Services/
?   ??? ViewManager.cs                # Управление окнами (ViewModelFirst)
?
??? ?? Converters/
?   ??? RubleCurrencyConverter.cs     # Конвертер валюты
?
??? ?? Documentation/
    ??? MVVM_ViewModelFirst_Documentation.md
    ??? ЗАЩИТА_MVVM.md
    ??? ШПАРГАЛКА.md
    ??? АРХИТЕКТУРА_ДИАГРАММЫ.md
    ??? SUMMARY.md
    ??? README.md                     # Этот файл
```

---

## ?? Ключевые компоненты

### 1. BaseView (Базовый класс окон)
**Файл:** `Views/BaseView.cs`

Базовый класс для всех окон приложения:
- Управление DataContext
- Управление жизненным циклом окна
- Единообразие всех View

```csharp
public abstract class BaseView : Window
{
    protected ViewModelBase? ViewModel => DataContext as ViewModelBase;
    
    protected BaseView(ViewModelBase viewModel)
    {
        DataContext = viewModel;
    }
}
```

---

### 2. ViewManager (Управление окнами)
**Файл:** `Services/ViewManager.cs`

Реализует ViewModelFirst подход:
- Создает окна на основе ViewModel
- Устанавливает DataContext
- Показывает окна пользователю

```csharp
public void ShowMainWindow(MainViewModel viewModel)
{
    var mainWindow = new MainWindow(viewModel);
    mainWindow.Show();
}
```

---

### 3. MainViewModel (Логика представления)
**Файл:** `ViewModels/MainViewModel.cs`

Содержит:
- Команды для кнопок (`ICommand`)
- Коллекцию данных (`ObservableCollection<ProductDto>`)
- Логику работы с бизнес-слоем

```csharp
public class MainViewModel : ViewModelBase
{
    public ObservableCollection<ProductDto> Products { get; }
    public ICommand RefreshCommand { get; }
    
    private void LoadProducts()
    {
        var products = _logic.GetAllProducts();
        // Маппинг Product ? ProductDto
    }
}
```

---

### 4. ProductDto (Data Transfer Object)
**Файл:** `ProductManagementSystem.Shared/ProductDto.cs`

DTO с уведомлениями для автоматического обновления UI:
- Реализует `INotifyPropertyChanged`
- Изолирует View от доменной модели

```csharp
public class ProductDto : INotifyPropertyChanged
{
    private string _name = string.Empty;
    
    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            OnPropertyChanged(); // UI обновляется автоматически
        }
    }
}
```

---

## ?? Технологии

| Технология | Версия | Назначение |
|------------|--------|-----------|
| .NET | 8.0 | Платформа разработки |
| WPF | 8.0 | UI фреймворк |
| MVVM | - | Архитектурный паттерн |
| Ninject | 3.3.6 | Dependency Injection |
| Entity Framework Core | 8.0 | ORM для БД |
| SQLite | - | База данных |

---

## ?? Основные возможности

### Управление товарами:
- ? Просмотр списка товаров
- ? Добавление новых товаров
- ? Удаление товаров
- ? Фильтрация по категориям
- ? Расчет общей стоимости склада

### Интерфейс:
- ? Современный дизайн
- ? Адаптивная разметка
- ? Стилизованные элементы управления
- ? Валидация ввода

---

## ?? Тестирование

### Проверка ViewModelFirst:

1. Установите точку останова в `App.xaml.cs`:
```csharp
var mainViewModel = _kernel.Get<MainViewModel>(); // ??? ЗДЕСЬ
```

2. Запустите приложение (F5)

3. Проверьте:
   - ? ViewModel создана
   - ? Окно еще не существует

4. Продолжите выполнение - окно откроется

**Вывод:** ViewModel создается РАНЬШЕ View (ViewModelFirst!)

---

## ?? SOLID принципы

### Single Responsibility:
- `BaseView` - только базовая функциональность окон
- `ViewManager` - только создание окон
- `MainViewModel` - только логика представления

### Open-Closed:
- `BaseView` - открыт для расширения через наследование
- `MainWindow` наследует `BaseView`

### Dependency Inversion:
- Все зависимости через интерфейсы (`ILogic`, `IRepository`)
- DI контейнер (Ninject) управляет зависимостями

---

## ?? Для студентов

### Задание выполнено:
- [x] П.1: WPF проект без кода в CodeBehind
- [x] П.2: ViewManager и базовый класс View
- [x] П.3: Базовый класс ViewModel и наследник
- [x] П.4: DAL, Model, Logic не тронуты
- [x] П.5: INotifyPropertyChanged в DTO
- [x] П.6: Синхронизация с Model
- [x] П.7: ViewModelFirst подход

### Для защиты:
1. Прочитайте **[ЗАЩИТА_MVVM.md](ЗАЩИТА_MVVM.md)**
2. Изучите **[ШПАРГАЛКА.md](ШПАРГАЛКА.md)**
3. Просмотрите **[АРХИТЕКТУРА_ДИАГРАММЫ.md](АРХИТЕКТУРА_ДИАГРАММЫ.md)**

### Ключевые вопросы:
1. **Что такое MVVM?** - Паттерн разделения UI и бизнес-логики
2. **Что такое ViewModelFirst?** - ViewModel создается раньше View
3. **Зачем ViewManager?** - Управление созданием окон
4. **Почему CodeBehind пустой?** - Вся логика в ViewModel
5. **Как работает Binding?** - Автоматическая синхронизация через INotifyPropertyChanged

---

## ?? Вклад

Проект разработан для демонстрации паттерна MVVM с ViewModelFirst подходом.

### Автор:
Студенческий проект - Лабораторная работа №5

### Преподаватель:
Требования согласно заданию на русском языке

---

## ?? Лицензия

Учебный проект для демонстрации архитектурных паттернов.

---

## ?? Контакты

Вопросы по реализации MVVM и ViewModelFirst:
- Изучите документацию в папке проекта
- Проверьте комментарии в коде
- Используйте шпаргалку для быстрых ответов

---

**Проект готов к защите! ??**

*Последнее обновление: 2024*
