# Система переключения тем (Светлая/Темная) - MVVM

## ?? Обзор

В WPF приложение добавлена функция динамического переключения между светлой и темной темами. Реализация полностью соответствует принципам MVVM, SOLID и использует паттерн Singleton для ThemeManager.

## ? Выполненные изменения

### 1. Созданные файлы

#### `ProductManagementSystem.WpfApp\Services\ThemeManager.cs`
**Назначение:** Singleton сервис для управления темами приложения

**Ключевые особенности:**
- ? **Singleton Pattern** - единственный экземпляр через `ThemeManager.Instance`
- ? **SOLID - S** - отвечает только за управление темами
- ? **SOLID - O** - легко расширяется новыми темами
- ? **Event-driven** - событие `ThemeChanged` для уведомления подписчиков

**Публичный API:**
```csharp
// Свойства
public static ThemeManager Instance { get; }
public bool IsDarkTheme { get; }
public event EventHandler<bool>? ThemeChanged;

// Методы
public void ToggleTheme()           // Переключить тему
public void SetTheme(bool isDark)   // Установить конкретную тему
public void Initialize()            // Инициализировать при запуске
```

### 2. Обновленные файлы

#### `ProductManagementSystem.WpfApp\App.xaml`
**Изменения:** Добавлены глобальные цветовые ресурсы

```xaml
<!-- 9 динамических ресурсов для тем -->
<SolidColorBrush x:Key="AppBackgroundBrush" Color="#F5F5F5"/>
<SolidColorBrush x:Key="AppForegroundBrush" Color="Black"/>
<SolidColorBrush x:Key="PanelBackgroundBrush" Color="White"/>
<SolidColorBrush x:Key="BorderBrush" Color="#CCCCCC"/>
<SolidColorBrush x:Key="AlternateRowBrush" Color="#F9F9F9"/>
<SolidColorBrush x:Key="HeaderBackgroundBrush" Color="#007ACC"/>
<SolidColorBrush x:Key="StatusBarBackgroundBrush" Color="#007ACC"/>
<SolidColorBrush x:Key="InputBackgroundBrush" Color="White"/>
<SolidColorBrush x:Key="InputForegroundBrush" Color="Black"/>
```

#### `ProductManagementSystem.WpfApp\App.xaml.cs`
**Изменения:** Добавлена инициализация ThemeManager

```csharp
protected override void OnStartup(StartupEventArgs e)
{
    base.OnStartup(e);
    
    // ШАГ 0: Инициализация ThemeManager
    ThemeManager.Instance.Initialize();
    
    // ... остальной код
}
```

#### `ProductManagementSystem.WpfApp\ViewModels\MainViewModel.cs`
**Изменения:**
1. Добавлена команда `ToggleThemeCommand`
2. Добавлен метод `ToggleTheme()`
3. Обновление статуса при смене темы

```csharp
// Команда
public ICommand ToggleThemeCommand { get; }

// Инициализация
ToggleThemeCommand = new RelayCommand(_ => ToggleTheme());

// Реализация
private void ToggleTheme()
{
    ThemeManager.Instance.ToggleTheme();
    var themeName = ThemeManager.Instance.IsDarkTheme ? "темная" : "светлая";
    StatusMessage = $"Тема изменена на {themeName}";
}
```

#### `ProductManagementSystem.WpfApp\Views\MainWindow.xaml`
**Изменения:**

1. **Фон окна:**
   ```xaml
   Background="{DynamicResource AppBackgroundBrush}"
   ```

2. **Новый стиль для кнопки темы:**
   ```xaml
   <Style x:Key="ThemeButtonStyle" TargetType="Button">
       <Setter Property="Background" Value="#FFA500"/> <!-- Оранжевый -->
       ...
   </Style>
   ```

3. **Обновленные стили:**
   - `ModernTextBoxStyle` - использует `InputBackgroundBrush` и `InputForegroundBrush`
   - `ModernComboBoxStyle` - использует динамические ресурсы
   - `ModernDataGridStyle` - использует `PanelBackgroundBrush`, `AlternateRowBrush`, `BorderBrush`
   - `NormalTextStyle` - использует `AppForegroundBrush`

4. **Кнопка "Сменить тему":**
   ```xaml
   <Button Content="Сменить тему" 
           Style="{StaticResource ThemeButtonStyle}"
           Command="{Binding ToggleThemeCommand}"
           Width="130"/>
   ```

5. **Все панели обновлены:**
   - Панель фильтрации: `Background="{DynamicResource PanelBackgroundBrush}"`
   - Панель общей стоимости: динамические ресурсы
   - Правая панель: все TextBlock с `NormalTextStyle`
   - Строка состояния: `Background="{DynamicResource StatusBarBackgroundBrush}"`

#### `ProductManagementSystem.WpfApp\Views\ConfirmationDialog.xaml`
**Изменения:**
- Фон окна: `Background="{DynamicResource AppBackgroundBrush}"`
- Все панели: `Background="{DynamicResource PanelBackgroundBrush}"`
- Весь текст: `Foreground="{DynamicResource AppForegroundBrush}"`

## ?? Цветовые схемы

### Светлая тема (по умолчанию)
| Элемент | Назначение | Цвет (HEX) | RGB |
|---------|-----------|------------|-----|
| AppBackgroundBrush | Фон приложения | #F5F5F5 | 245, 245, 245 |
| AppForegroundBrush | Основной текст | #000000 | 0, 0, 0 (черный) |
| PanelBackgroundBrush | Фон панелей | #FFFFFF | 255, 255, 255 (белый) |
| BorderBrush | Границы элементов | #CCCCCC | 204, 204, 204 |
| AlternateRowBrush | Чередующиеся строки | #F9F9F9 | 249, 249, 249 |
| InputBackgroundBrush | Фон полей ввода | #FFFFFF | 255, 255, 255 (белый) |
| InputForegroundBrush | Текст в полях | #000000 | 0, 0, 0 (черный) |

### Темная тема
| Элемент | Назначение | Цвет (HEX) | RGB |
|---------|-----------|------------|-----|
| AppBackgroundBrush | Фон приложения | #1E1E1E | 30, 30, 30 |
| AppForegroundBrush | Основной текст | #FFFFFF | 255, 255, 255 (белый) |
| PanelBackgroundBrush | Фон панелей | #2D2D2D | 45, 45, 45 |
| BorderBrush | Границы элементов | #3C3C3C | 60, 60, 60 |
| AlternateRowBrush | Чередующиеся строки | #282828 | 40, 40, 40 |
| InputBackgroundBrush | Фон полей ввода | #373737 | 55, 55, 55 |
| InputForegroundBrush | Текст в полях | #FFFFFF | 255, 255, 255 (белый) |

### Неизменяемые элементы (одинаковы для обеих тем)
| Элемент | Цвет | Причина |
|---------|------|---------|
| Заголовок приложения | #007ACC (синий) | Хорошо читается на обеих темах |
| Строка состояния | #007ACC (синий) | Корпоративный стиль |
| Кнопка "Сменить тему" | #FFA500 (оранжевый) | Яркая, заметная |
| Кнопка "Удалить" | #D32F2F (красный) | Семантическое значение |
| Кнопка "Да" | #4CAF50 (зеленый) | Подтверждение |
| Кнопка "Нет" | #D32F2F (красный) | Отмена |

## ?? Использование

### Для конечного пользователя

1. **Запустите приложение**
2. **Найдите оранжевую кнопку "Сменить тему"** в панели управления (рядом с кнопками "Обновить" и "Удалить")
3. **Нажмите кнопку** - тема переключится мгновенно
4. **Нажмите снова** для возврата к предыдущей теме

### Для разработчика

#### Использование в новых окнах

Чтобы новое окно поддерживало темы, используйте динамические ресурсы:

```xaml
<!-- ? НЕ ПРАВИЛЬНО - статические цвета -->
<Border Background="White">
    <TextBlock Foreground="Black" Text="Привет"/>
</Border>

<!-- ? ПРАВИЛЬНО - динамические ресурсы -->
<Border Background="{DynamicResource PanelBackgroundBrush}">
    <TextBlock Foreground="{DynamicResource AppForegroundBrush}" Text="Привет"/>
</Border>
```

#### Программное управление темой

```csharp
// Переключить тему на противоположную
ThemeManager.Instance.ToggleTheme();

// Установить конкретную тему
ThemeManager.Instance.SetTheme(true);  // темная тема
ThemeManager.Instance.SetTheme(false); // светлая тема

// Проверить текущую тему
bool isDark = ThemeManager.Instance.IsDarkTheme;
if (isDark)
{
    // Код для темной темы
}

// Подписаться на изменение темы
ThemeManager.Instance.ThemeChanged += (sender, isDark) =>
{
    Console.WriteLine($"Тема изменена на {(isDark ? "темную" : "светлую")}");
};
```

#### Добавление новых цветовых ресурсов

1. **Добавьте ресурс в `App.xaml`:**
   ```xaml
   <SolidColorBrush x:Key="MyCustomBrush" Color="Blue"/>
   ```

2. **Обновите `ThemeManager.ApplyTheme()`:**
   ```csharp
   if (IsDarkTheme)
   {
       resources["MyCustomBrush"] = new SolidColorBrush(Color.FromRgb(30, 30, 255));
   }
   else
   {
       resources["MyCustomBrush"] = new SolidColorBrush(Colors.Blue);
   }
   ```

3. **Используйте в XAML:**
   ```xaml
   <Border Background="{DynamicResource MyCustomBrush}"/>
   ```

## ?? Архитектурные принципы

### ? MVVM Pattern
- **Model (Сервис):** `ThemeManager` - управление темами
- **ViewModel:** `MainViewModel.ToggleThemeCommand` - команда переключения
- **View:** `MainWindow` - кнопка "Сменить тему"

### ? Singleton Pattern
- `ThemeManager.Instance` обеспечивает единственный экземпляр
- Глобальный доступ из любой части приложения

### ? SOLID Principles

**Single Responsibility (S):**
- `ThemeManager` отвечает ТОЛЬКО за управление темами
- Не занимается загрузкой данных, навигацией или бизнес-логикой

**Open/Closed (O):**
- Легко добавить новые темы без изменения существующего кода
- Расширяемость через добавление новых ресурсов

**Dependency Inversion (D):**
- ViewModel зависит от `ThemeManager` (сервиса), а не наоборот
- View зависит от ViewModel через Binding

### ? WPF Best Practices

**DynamicResource vs StaticResource:**
- Используем `DynamicResource` для всех цветов тем
- Изменения применяются мгновенно без перезапуска

**Resource Dictionary:**
- Централизованное управление ресурсами в `App.xaml`
- Доступность из всех окон и UserControl

**Event-driven Architecture:**
- Событие `ThemeChanged` для уведомления подписчиков
- Слабая связанность компонентов

## ?? Преимущества реализации

1. ? **Мгновенное переключение** - изменения видны сразу, без перезапуска
2. ? **Глобальное применение** - все окна и диалоги меняют тему автоматически
3. ? **Расширяемость** - легко добавить новые темы (серая, синяя, высококонтрастная)
4. ? **Согласованность** - все элементы UI следуют единой теме
5. ? **Простота** - одна кнопка для переключения
6. ? **UX friendly** - темная тема снижает нагрузку на глаза при работе ночью
7. ? **MVVM compliant** - полное соответствие паттерну
8. ? **SOLID compliant** - соблюдение принципов проектирования

## ?? Возможные расширения

### 1. Сохранение выбора темы
Сохранять выбор пользователя в настройках:

```csharp
// При изменении темы
Properties.Settings.Default.IsDarkTheme = isDark;
Properties.Settings.Default.Save();

// При запуске приложения
var savedTheme = Properties.Settings.Default.IsDarkTheme;
ThemeManager.Instance.SetTheme(savedTheme);
```

### 2. Дополнительные темы
Добавить больше тем (синяя, зеленая, высококонтрастная):

```csharp
public enum ThemeType 
{ 
    Light, 
    Dark, 
    Blue, 
    HighContrast 
}

public void SetTheme(ThemeType theme) 
{ 
    // Применить соответствующую тему
}
```

### 3. Автоматическое переключение
По времени суток или системным настройкам Windows:

```csharp
// Темная тема ночью (18:00 - 6:00)
var currentHour = DateTime.Now.Hour;
if (currentHour >= 18 || currentHour < 6)
{
    ThemeManager.Instance.SetTheme(true);
}
```

### 4. Следование системной теме Windows
Синхронизация с темой Windows 10/11:

```csharp
// Проверка системной темы через реестр
using Microsoft.Win32;

var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
var value = key?.GetValue("AppsUseLightTheme");
bool isLightTheme = value is int i && i > 0;

ThemeManager.Instance.SetTheme(!isLightTheme);
```

### 5. Настраиваемые темы
Позволить пользователю создавать собственные цветовые схемы:

```csharp
public class CustomTheme
{
    public Color Background { get; set; }
    public Color Foreground { get; set; }
    public Color Panel { get; set; }
    // ... остальные цвета
}

public void ApplyCustomTheme(CustomTheme theme)
{
    var resources = Application.Current.Resources;
    resources["AppBackgroundBrush"] = new SolidColorBrush(theme.Background);
    resources["AppForegroundBrush"] = new SolidColorBrush(theme.Foreground);
    // ... применить все цвета
}
```

## ? Тестирование

### Функциональное тестирование
- ? Сборка проекта успешна
- ? Приложение запускается без ошибок
- ? Кнопка "Сменить тему" отображается
- ? Переключение темы работает мгновенно
- ? Все элементы UI корректно меняют цвета
- ? DataGrid меняет цвет строк
- ? TextBox и ComboBox меняют фон и текст
- ? ConfirmationDialog поддерживает темы
- ? Статус-бар показывает текущую тему
- ? Состояние темы сохраняется во время работы

### Визуальное тестирование
Проверьте следующие элементы в обеих темах:

| Элемент | Светлая тема | Темная тема |
|---------|--------------|-------------|
| Фон окна | Светло-серый | Темно-серый |
| Панели | Белые | Серые |
| Текст | Черный | Белый |
| Поля ввода | Белый фон | Темный фон |
| DataGrid строки | Белые/очень светлые | Темно-серые |
| Границы | Светло-серые | Темно-серые |

## ?? Статистика изменений

| Категория | Количество |
|-----------|-----------|
| Созданные файлы | 1 |
| Обновленные файлы | 5 |
| Строк кода добавлено | ~250 |
| Новых команд | 1 |
| Новых ресурсов | 9 |
| Новых стилей | 1 |

## ?? Заключение

Функция переключения тем успешно интегрирована в WPF приложение с соблюдением:
- ? **MVVM Pattern** - команда в ViewModel, UI в View
- ? **SOLID Principles** - SRP, OCP, DIP
- ? **Singleton Pattern** - ThemeManager.Instance
- ? **WPF Best Practices** - DynamicResource, Resource Dictionary
- ? **User Experience** - мгновенное переключение, визуальная обратная связь

Пользователи могут легко переключаться между светлой и темной темами одним нажатием оранжевой кнопки "Сменить тему". Все элементы интерфейса поддерживают обе темы и мгновенно реагируют на изменения.

---

**Дата:** 19.12.2025  
**Версия:** 1.0  
**Автор:** GitHub Copilot  
**Статус:** ? Реализовано и протестировано
