# Рефакторинг в MVVM архитектуру - Практическая работа №5

## Обзор изменений

Этот коммит реализует полный переход от MVP к MVVM архитектуре в WPF приложении с использованием ViewModelFirst подхода.

## Реализованные компоненты

### 1. DTO с INotifyPropertyChanged

**Файл**: `ProductManagementSystem.Shared/ProductDto.cs`

Обновлен `ProductDto` с полной поддержкой `INotifyPropertyChanged`:
- Все свойства теперь уведомляют об изменениях
- Реализованы приватные поля для хранения значений
- Добавлен метод `SetProperty` для упрощения обновления свойств
- Поддержка двусторонней привязки данных (TwoWay binding)

```csharp
public class ProductDto : INotifyPropertyChanged
{
    private int _id;
    public int Id
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }
    // ... остальные свойства
}
```

### 2. MVVM Core - ViewModels

#### ViewModelBase (`ViewModels/ViewModelBase.cs`)
Базовый класс для всех ViewModel с реализацией `INotifyPropertyChanged`:
- Метод `OnPropertyChanged` с автоматическим получением имени свойства
- Метод `SetProperty` для упрощенного обновления свойств
- SOLID принципы: Single Responsibility, Open/Closed

#### MainViewModel (`ViewModels/MainViewModel.cs`)
Главная ViewModel с полным функционалом:
- **ObservableCollection<ProductDto>** для автоматического обновления UI
- **Команды** для всех действий пользователя:
  - `RefreshCommand` - обновление списка товаров
  - `AddProductCommand` - добавление нового товара
  - `DeleteProductCommand` - удаление выбранного товара
  - `ApplyFilterCommand` - фильтрация по категории
  - `CalculateTotalCommand` - расчет общей стоимости
  - `ClearFormCommand` - очистка формы
- **Свойства для привязки**:
  - Коллекции: `Products`, `Categories`
  - Выбранные элементы: `SelectedProduct`, `SelectedCategory`
  - Поля формы: `ProductName`, `ProductPrice`, и т.д.
  - Статус: `StatusMessage`, `TotalInventoryValue`
- **Маппинг** между доменными сущностями `Product` и DTO
- **Валидация** входных данных
- **Dependency Injection** - получает `ILogic` через конструктор

### 3. Commands - RelayCommand

**Файл**: `Commands/RelayCommand.cs`

Реализация паттерна Command для WPF:
- Инкапсуляция действий и проверок в переиспользуемые команды
- Поддержка `CanExecute` для управления доступностью UI элементов
- Автоматическая интеграция с `CommandManager` WPF
- Метод `RaiseCanExecuteChanged` для принудительного обновления

```csharp
public ICommand AddProductCommand => new RelayCommand(
    execute: _ => AddProduct(),
    canExecute: _ => CanAddProduct()
);
```

### 4. ViewManager - ViewModelFirst подход

**Файл**: `Services/ViewManager.cs`

Менеджер для управления окнами приложения:
- Реализует паттерн **ViewModelFirst**
- Централизованное создание и отображение окон
- Метод `ShowMainWindow` для главного окна
- Методы для диалоговых окон и сообщений
- Отделяет ViewModel от знания о конкретных View

### 5. Современный WPF UI

#### MainWindow.xaml (`Views/MainWindow.xaml`)

Полностью переработанный интерфейс с современным дизайном:

**Стили и ресурсы**:
- `ModernButtonStyle` - кнопки с закругленными углами и hover эффектами
- `DangerButtonStyle` - красные кнопки для опасных действий
- `ModernTextBoxStyle` - текстовые поля с подсветкой при фокусе
- `ModernComboBoxStyle` - стилизованные выпадающие списки
- `ModernDataGridStyle` - таблица с чередующимися строками

**Макет**:
- **Заголовок** с брендингом (#007ACC синий цвет)
- **Левая панель** (2/3 ширины):
  - Фильтр по категориям с кнопкой применения
  - Кнопки управления (Обновить, Удалить, Расчет стоимости)
  - DataGrid со всеми товарами
  - Блок с общей стоимостью склада
- **Правая панель** (1/3 ширины):
  - Форма добавления/редактирования товара
  - Все поля с двусторонней привязкой
  - Кнопки "Добавить" и "Очистить"
- **Строка состояния** внизу с актуальными сообщениями

**MVVM привязки**:
```xml
<DataGrid ItemsSource="{Binding Products}"
          SelectedItem="{Binding SelectedProduct, Mode=TwoWay}">

<Button Content="Add Product"
        Command="{Binding AddProductCommand}"/>

<TextBox Text="{Binding ProductName, UpdateSourceTrigger=PropertyChanged}"/>
```

#### MainWindow.xaml.cs (`Views/MainWindow.xaml.cs`)

Минимальный CodeBehind:
- Только вызов `InitializeComponent()`
- Нет бизнес-логики
- DataContext устанавливается извне через ViewManager

### 6. Composition Root - App.xaml.cs

**Файл**: `App.xaml.cs`

Точка входа приложения с настройкой DI:

**OnStartup** метод:
1. Настройка Ninject контейнера (`ConfigureDependencyInjection`)
2. Регистрация зависимостей:
   - `IRepository<Product>` ? `EntityRepository<Product>`
   - `IBusinessFunctions` ? `BusinessFunctions`
   - `ILogic` ? `ProductLogic`
   - `MainViewModel` ? Transient scope
3. Создание `ViewManager`
4. Создание `MainViewModel` через DI
5. Отображение главного окна (ViewModelFirst)

**OnExit** метод:
- Корректное освобождение ресурсов DI контейнера

**App.xaml**:
- Убран `StartupUri` (ViewModelFirst требует программного запуска)

## Архитектурные преимущества MVVM

### 1. Разделение ответственности
- **Model** (Product, ProductLogic) - бизнес-логика и данные
- **View** (MainWindow.xaml) - только UI, нет логики
- **ViewModel** (MainViewModel) - логика представления, команды, состояние UI

### 2. Тестируемость
- ViewModel можно тестировать без UI
- Mock'ирование ILogic для unit-тестов
- Независимость от WPF инфраструктуры

### 3. Data Binding
- Двусторонняя привязка данных
- Автоматическое обновление UI при изменении данных
- `ObservableCollection` для динамических списков
- `INotifyPropertyChanged` для отдельных свойств

### 4. Commands
- Декларативная привязка действий к UI элементам
- Автоматическое управление доступностью кнопок
- Переиспользуемость логики команд

### 5. ViewModelFirst
- ViewModel не зависит от View
- Упрощение тестирования
- Централизованное управление окнами
- Возможность замены View без изменения ViewModel

## Dependency Injection (Ninject)

Все зависимости управляются через DI контейнер:

```csharp
_kernel.Bind<IRepository<Product>>().To<EntityRepository<Product>>().InSingletonScope();
_kernel.Bind<IBusinessFunctions>().To<BusinessFunctions>().InSingletonScope();
_kernel.Bind<ILogic>().To<ProductLogic>().InSingletonScope();
_kernel.Bind<MainViewModel>().ToSelf().InTransientScope();
```

**Преимущества**:
- Легкое переключение между Entity Framework и Dapper
- Все зависимости разрешаются автоматически
- Поддержка Singleton и Transient scope
- Централизованная конфигурация

## Сравнение с MVP

| Аспект | MVP (было) | MVVM (стало) |
|--------|-----------|--------------|
| Связь View-Logic | Через интерфейсы IProductView | Через Data Binding |
| Обновление UI | Вызовы методов View | Автоматически через INotifyPropertyChanged |
| Обработка событий | Event handlers в CodeBehind | Commands в ViewModel |
| Тестирование | Mock интерфейсов View | Mock ILogic, без UI |
| CodeBehind | События, логика | Только InitializeComponent() |
| Presenter | Явный класс с логикой | Нет, заменен на ViewModel |

## Современный UI

### Цветовая схема
- **Основной цвет**: #007ACC (синий Microsoft)
- **Фон**: #F5F5F5 (светло-серый)
- **Опасные действия**: #D32F2F (красный)
- **Текст**: черный / белый в зависимости от фона

### UX улучшения
- Закругленные углы на всех элементах (CornerRadius="4")
- Hover эффекты на кнопках
- Подсветка границ при фокусе в текстовых полях
- Чередующиеся цвета строк в таблице
- Крупные, читаемые шрифты (14-16px)
- Адекватные отступы и padding

### Адаптивность
- Минимальный размер окна 1000x600
- Grid layout с пропорциональными колонками
- ScrollViewer для длинных форм
- DataGrid с автоматической шириной колонок

## Требования к запуску

### Зависимости
- .NET 8.0
- WPF
- Ninject 3.3.6
- Entity Framework Core 9.0.10 (или Dapper 2.1.66)

### База данных
```
Server=AspireNotebook\SQLEXPRESS;
Database=ProductManagementDB;
Integrated Security=True;
TrustServerCertificate=True;
```

## Проверка работоспособности

1. **Запуск приложения**:
   - Приложение запускается через `App.OnStartup`
   - Главное окно создается через `ViewManager.ShowMainWindow`
   - ViewModel автоматически связывается с View через DataContext

2. **Загрузка данных**:
   - При запуске автоматически загружаются товары из БД
   - Заполняется список категорий

3. **CRUD операции**:
   - **Добавление**: заполните форму справа, нажмите "Add Product"
   - **Редактирование**: выберите товар в таблице, измените поля, нажмите "Add Product"
   - **Удаление**: выберите товар, нажмите "Delete"
   - **Чтение**: автоматически при запуске и обновлении

4. **Фильтрация**:
   - Выберите категорию из выпадающего списка
   - Нажмите "Apply Filter"
   - Для сброса выберите "Все категории"

5. **Расчет стоимости**:
   - Нажмите "Calculate Total"
   - Результат отобразится в блоке внизу слева

## Структура файлов

```
ProductManagementSystem.WpfApp/
??? Commands/
?   ??? RelayCommand.cs          # Реализация ICommand
??? Services/
?   ??? ViewManager.cs           # Управление окнами (ViewModelFirst)
??? ViewModels/
?   ??? ViewModelBase.cs         # Базовый класс с INotifyPropertyChanged
?   ??? MainViewModel.cs         # Главная ViewModel
??? Views/
?   ??? MainWindow.xaml          # Современный UI
?   ??? MainWindow.xaml.cs       # Минимальный CodeBehind
??? App.xaml                     # Глобальные ресурсы (без StartupUri)
??? App.xaml.cs                  # Composition Root с Ninject DI
```

## SOLID принципы

### Single Responsibility
- `ViewModelBase` - только уведомления об изменениях
- `RelayCommand` - только инкапсуляция команд
- `ViewManager` - только управление окнами
- `MainViewModel` - только логика представления

### Open/Closed
- `ViewModelBase` открыт для расширения (наследование)
- Закрыт для модификации (базовая функциональность неизменна)

### Liskov Substitution
- Любая ViewModel может заменить `ViewModelBase`
- `MainViewModel` полностью соответствует контракту базового класса

### Interface Segregation
- `INotifyPropertyChanged` - минимальный интерфейс
- `ICommand` - только необходимые методы

### Dependency Inversion
- `MainViewModel` зависит от `ILogic`, а не от `ProductLogic`
- Зависимости внедряются через конструктор
- DI контейнер разрешает все зависимости

## Заключение

Реализация MVVM паттерна завершена успешно:
- ? ViewModelFirst подход
- ? Data Binding вместо событий
- ? Commands вместо обработчиков
- ? INotifyPropertyChanged в DTO и ViewModel
- ? Минимальный CodeBehind
- ? Dependency Injection (Ninject)
- ? Современный UI дизайн
- ? Полная функциональность (CRUD + бизнес-функции)
- ? SOLID принципы соблюдены
- ? Тестируемость повышена
- ? Обратная совместимость с существующей бизнес-логикой

Приложение готово к использованию и дальнейшему расширению функционала.

---

**Дата реализации**: 2025
**Паттерн**: MVVM (Model-View-ViewModel)
**Подход**: ViewModelFirst
**DI Framework**: Ninject
**UI Framework**: WPF (.NET 8)
