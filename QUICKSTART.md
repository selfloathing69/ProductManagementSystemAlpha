# ?? Быстрый старт - Настройка автозапускаемого проекта

## Вариант 1: Visual Studio 2022

### Шаг 1: Откройте Solution
Откройте файл `ProductManagementSystem.sln` в Visual Studio 2022

### Шаг 2: Установите автозапускаемый проект
1. В **Solution Explorer** найдите проект **ProductManagementSystem.Presenter**
2. Щелкните **правой кнопкой мыши** по проекту
3. Выберите **"Назначить запускаемым проектом"** (Set as Startup Project)
4. Проект станет **жирным шрифтом** - это означает, что он автозапускаемый

### Шаг 3: Запуск
Нажмите **F5** или кнопку **"Запуск"** (??)

---

## Вариант 2: Visual Studio Code + .NET CLI

### Запуск Presenter (рекомендуется)
```bash
dotnet run --project ProductManagementSystem.Presenter
```

### Запуск ConsoleApp (текстовое меню)
```bash
dotnet run --project ProductManagementSystem.ConsoleApp
```

### Запуск WinFormsApp (standalone, не рекомендуется)
```bash
dotnet run --project ProductManagementSystem.WinFormsApp
```

### Запуск WpfApp (альтернативный UI)
```bash
dotnet run --project ProductManagementSystem.WpfApp
```

---

## Вариант 3: JetBrains Rider

### Шаг 1: Откройте Solution
Откройте файл `ProductManagementSystem.sln` в Rider

### Шаг 2: Настройте Run Configuration
1. Откройте меню **Run** ? **Edit Configurations...**
2. Нажмите **+** ? **".NET Project"**
3. Выберите проект **ProductManagementSystem.Presenter**
4. Нажмите **OK**

### Шаг 3: Запуск
Нажмите **Shift+F10** или кнопку **Run** (??)

---

## ?? Проверка корректности установки

После запуска **Presenter** вы должны увидеть в консоли:

```
??????????????????????????????????????????????????????????
?  Product Management System - MVP Presenter (v4.0)     ?
??????????????????????????????????????????????????????????

Инициализация MVP архитектуры...
?? Запуск Dependency Injection контейнера...
?? DI контейнер успешно инициализирован
?? Получение Model (IProductModel)...
?? Model успешно создан и настроен
?? Запуск View (WinFormsApp)...
```

И откроется окно **WinForms** с интерфейсом управления товарами.

---

## ? Частые проблемы

### Проблема: "Не найден проект ProductManagementSystem.Presenter"
**Решение**: Убедитесь, что проект добавлен в solution:
```bash
dotnet sln ProductManagementSystem.sln add ProductManagementSystem.Presenter\ProductManagementSystem.Presenter.csproj
```

### Проблема: "Ошибка компиляции в Presenter"
**Решение**: Восстановите NuGet пакеты:
```bash
dotnet restore
dotnet build
```

### Проблема: WinFormsApp открывается напрямую
**Решение**: Вы запустили WinFormsApp вместо Presenter. Запустите именно **ProductManagementSystem.Presenter**

---

## ?? Итог

Теперь у вас есть **4 автозапускаемых проекта**:

| Проект | Описание | Когда использовать |
|--------|----------|-------------------|
| **Presenter** ? | MVP архитектура | **Основной режим работы** |
| **ConsoleApp** | Текстовое меню | Когда нужен консольный интерфейс |
| **WinFormsApp** | Standalone GUI | Только для тестирования View |
| **WpfApp** | WPF интерфейс | Альтернативный UI |

**Рекомендация**: Всегда используйте **Presenter** для запуска приложения в режиме MVP! ?
