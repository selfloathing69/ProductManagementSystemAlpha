# Текст защиты: Реализация паттерна MVVM с ViewModelFirst

## Что такое MVVM?

**MVVM (Model-View-ViewModel)** — архитектурный паттерн для приложений с богатым пользовательским интерфейсом (WPF, UWP). Разделяет приложение на три компонента:

- **Model** — бизнес-логика и данные (`Product`, `ProductLogic`, `IRepository`)
- **View** — визуальный интерфейс (XAML-разметка без бизнес-логики)
- **ViewModel** — промежуточный слой между View и Model, содержит логику представления

**Главное преимущество:** Полное разделение UI и бизнес-логики. View не знает о Model, Model не зависит от View.

---

## Что такое ViewModelFirst?

**ViewModelFirst** — подход к инициализации MVVM-приложения, где **ViewModel создается РАНЬШЕ View**.

**Алгоритм:**
1. Создаем ViewModel (через DI контейнер)
2. ViewManager создает соответствующую View
3. View.DataContext = ViewModel (автоматическая связь)
4. Показываем окно пользователю

**Преимущества:**
- ViewModel не зависит от View ? легко тестировать
- Можно подменить View без изменения ViewModel
- Централизованное управление окнами

---

## Что мы реализовали

### 1. Базовый класс View (`BaseView.cs`)

Создали абстрактный базовый класс для всех окон WPF:

```csharp
public abstract class BaseView : Window
{
    // Доступ к ViewModel из наследников
    protected ViewModelBase? ViewModel => DataContext as ViewModelBase;
    
    // Конструктор для ViewModelFirst
    protected BaseView(ViewModelBase viewModel)
    {
        DataContext = viewModel;
    }
}
```

**Назначение:**
- Единообразие всех окон приложения
- Управление жизненным циклом View
- Автоматическая установка DataContext

**SOLID-S:** Отвечает только за базовую функциональность окон

---

### 2. ViewManager (`ViewManager.cs`)

Создали менеджер для управления окнами по паттерну ViewModelFirst:

```csharp
public void ShowMainWindow(MainViewModel viewModel)
{
    // 1. ViewModel уже создана (передана извне)
    // 2. Создаем View и передаем ViewModel
    var mainWindow = new MainWindow(viewModel);
    
    // 3. View автоматически устанавливает DataContext
    // 4. Показываем окно
    mainWindow.Show();
}
```

**Назначение:**
- Создание окон на основе ViewModel
- Реализация ViewModelFirst подхода
- Централизованное управление UI

**SOLID-S:** Отвечает только за создание окон  
**SOLID-D:** Зависит от абстракции `ViewModelBase`

---

### 3. Composition Root (`App.xaml.cs`)

Настроили точку входа приложения с ViewModelFirst:

```csharp
protected override void OnStartup(StartupEventArgs e)
{
    // ШАГ 1: Настройка DI контейнера
    ConfigureDependencyInjection();
    
    // ШАГ 2: Создание ViewManager
    var viewManager = new ViewManager();
    
    // ШАГ 3: Создание ViewModel (ViewModelFirst!)
    var mainViewModel = _kernel!.Get<MainViewModel>();
    
    // ШАГ 4: ViewManager показывает окно
    viewManager.ShowMainWindow(mainViewModel);
}
```

**Ключевой момент:** ViewModel создается **ДО** View через DI контейнер.

**Граф зависимостей:**
```
MainViewModel
  ? требует ILogic
ProductLogic
  ? требует IRepository + IBusinessFunctions
EntityRepository + BusinessFunctions
```

---

### 4. Минимальный CodeBehind

`MainWindow.xaml.cs` содержит только конструкторы:

```csharp
public partial class MainWindow : BaseView
{
    // Для дизайнера XAML
    public MainWindow()
    {
        InitializeComponent();
    }
    
    // ViewModelFirst (используется ViewManager)
    public MainWindow(MainViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
    }
}
```

**Вся бизнес-логика находится в ViewModel, а не в CodeBehind!**

---

### 5. DTO с INotifyPropertyChanged

`ProductDto` реализует уведомления об изменениях:

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
            OnPropertyChanged(); // Уведомление View
        }
    }
}
```

**Зачем:** Автоматическое обновление UI при изменении данных.

**Почему не на сущностях:** Доменные модели (`Product`) должны оставаться чистыми (POCO).

---

### 6. Data Binding и Commands

**Data Binding** (привязка данных):
```xaml
<!-- Автоматическая синхронизация View ? ViewModel -->
<TextBox Text="{Binding ProductName, UpdateSourceTrigger=PropertyChanged}"/>
<DataGrid ItemsSource="{Binding Products}"/>
```

**Command Binding** (привязка команд):
```xaml
<!-- Вместо обработчиков Click используем команды -->
<Button Command="{Binding RefreshCommand}"/>
<Button Command="{Binding AddProductCommand}"/>
```

**Преимущество:** View не содержит обработчиков событий — вся логика в ViewModel.

---

## Соответствие заданию

| Требование | Реализация | Статус |
|------------|------------|--------|
| **П.1:** WPF проект без кода в CodeBehind | `MainWindow.xaml.cs` — только конструкторы | ? |
| **П.2:** ViewManager и базовый класс View | `ViewManager.cs` + `BaseView.cs` | ? |
| **П.3:** Базовый класс ViewModel | `ViewModelBase.cs` + `MainViewModel.cs` | ? |
| **П.4:** DAL, Model, Logic не тронуты | Изменения только в WPF проекте | ? |
| **П.5:** INotifyPropertyChanged в DTO | `ProductDto` реализует интерфейс | ? |
| **П.6:** Синхронизация с Model | `ToDto()` / `ToProduct()` маппинг | ? |
| **П.7:** ViewModelFirst подход | ViewModel создается ДО View | ? |

---

## Поток работы ViewModelFirst

```
???????????????????????????????????????????????????????
? 1. ЗАПУСК: App.OnStartup()                         ?
???????????????????????????????????????????????????????
                      ?
???????????????????????????????????????????????????????
? 2. НАСТРОЙКА DI: ConfigureDependencyInjection()    ?
?    Регистрация: Repository ? Logic ? ViewModel     ?
???????????????????????????????????????????????????????
                      ?
???????????????????????????????????????????????????????
? 3. СОЗДАНИЕ VIEWMANAGER                            ?
???????????????????????????????????????????????????????
                      ?
???????????????????????????????????????????????????????
? 4. СОЗДАНИЕ VIEWMODEL (ViewModelFirst!)            ?
?    mainViewModel = _kernel.Get<MainViewModel>()    ?
?    - Ninject внедряет ILogic                       ?
?    - ViewModel загружает данные                    ?
???????????????????????????????????????????????????????
                      ?
???????????????????????????????????????????????????????
? 5. VIEWMANAGER СОЗДАЕТ VIEW                        ?
?    viewManager.ShowMainWindow(mainViewModel)       ?
?    - new MainWindow(viewModel)                     ?
?    - DataContext = viewModel                       ?
?    - mainWindow.Show()                             ?
???????????????????????????????????????????????????????
                      ?
???????????????????????????????????????????????????????
? 6. UI ОТОБРАЖАЕТСЯ                                 ?
?    Все Binding автоматически работают              ?
???????????????????????????????????????????????????????
```

---

## Взаимодействие с пользователем

**Пример:** Пользователь нажимает кнопку "Обновить"

```
USER: Нажатие кнопки
  ?
VIEW (XAML): Command="{Binding RefreshCommand}"
  ?
VIEWMODEL: RefreshCommand.Execute() ? LoadProducts()
  ?
LOGIC: _logic.GetAllProducts()
  ?
REPOSITORY: EntityRepository.GetAll()
  ?
DATABASE: SELECT * FROM Products
  ?
МАППИНГ: Product ? ProductDto
  ?
VIEWMODEL: Products.Add(dto) + OnPropertyChanged()
  ?
VIEW: DataGrid автоматически обновляется
```

**Ключевое отличие от MVP:** Нет явных событий и обработчиков — все через Binding!

---

## Преимущества реализации

### ? Разделение ответственности (SOLID-S)
- View — только отображение
- ViewModel — логика представления
- Model — бизнес-логика

### ? Тестируемость
- ViewModel можно тестировать без UI
- Легко подменить репозиторий для unit-тестов

### ? Независимость компонентов (SOLID-D)
- View не знает о Model
- ViewModel не знает о конкретной View
- Все зависимости через интерфейсы

### ? Реактивность
- Автоматическое обновление UI через Binding
- INotifyPropertyChanged обеспечивает синхронизацию

### ? Расширяемость (SOLID-O)
- Легко добавить новые окна
- Можно создать разные View для одной ViewModel

---

## Сравнение с MVP (WinForms)

| Аспект | MVP (WinForms) | MVVM (WPF) |
|--------|----------------|------------|
| **Связь** | События (Event) | Binding + Commands |
| **CodeBehind** | Обработчики событий | Минимален |
| **Зависимость** | View знает Presenter | View не знает ViewModel |
| **Тестируемость** | Сложнее | Проще |
| **Реактивность** | Ручная | Автоматическая |

---

## Итоги

**Реализовано:**
- ? Паттерн MVVM с ViewModelFirst подходом
- ? BaseView как базовый класс для окон
- ? ViewManager для управления окнами
- ? Dependency Injection (Ninject)
- ? DTO с INotifyPropertyChanged
- ? Data Binding и Command Binding
- ? Минимальный CodeBehind
- ? SOLID принципы

**Результат:** Современное WPF приложение с чистой архитектурой MVVM, полностью соответствующее требованиям лабораторной работы №5.
