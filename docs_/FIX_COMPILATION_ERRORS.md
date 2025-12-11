# Быстрое исправление ошибок компиляции

## ? Проблема

WinForms и WPF вызывают:
```csharp
new ProductLogic(repository)  // ОШИБКА! Нужно 2 параметра: repository + businessFunctions
```

## ? Решение

Заменить на:
```csharp
RepositoryFactory.CreateProductLogic()  // Ninject автоматически создаст с нужными зависимостями
```

---

## ?? Файлы для исправления

### **1. MainForm.cs** (WinForms)

**Строка 45:**
```csharp
// БЫЛО:
_logic = new ProductLogic(RepositoryFactory.CreateRepository());

// СТАЛО:
_logic = RepositoryFactory.CreateProductLogic();
```

**Строка 55:**
```csharp
// БЫЛО:
_logic = new ProductLogic(null);

// СТАЛО:
_logic = new ProductLogic(null, new BusinessFunctions());
// ИЛИ (рекомендуется):
_logic = new ProductLogic();  // Конструктор по умолчанию
```

---

### **2. MainWindow.xaml.cs** (WPF)

**Строка 21:**
```csharp
// БЫЛО:
_logic = new ProductLogic(repository);

// СТАЛО:
_logic = RepositoryFactory.CreateProductLogic();
```

---

## ?? Автоматическое исправление (PowerShell)

Выполните в корне проекта:

```powershell
# WinForms
(Get-Content "ProductManagementSystem.WinFormsApp\MainForm.cs") `
    -replace 'new ProductLogic\(RepositoryFactory\.CreateRepository\(\)\)', 'RepositoryFactory.CreateProductLogic()' `
    -replace 'new ProductLogic\(null\)', 'new ProductLogic()' `
    | Set-Content "ProductManagementSystem.WinFormsApp\MainForm.cs"

# WPF
(Get-Content "ProductManagementSystem.WpfApp\MainWindow.xaml.cs") `
    -replace 'new ProductLogic\(repository\)', 'RepositoryFactory.CreateProductLogic()' `
    | Set-Content "ProductManagementSystem.WpfApp\MainWindow.xaml.cs"
```

---

## ? После исправления

Проект должен компилироваться без ошибок! Запустите:

```bash
dotnet build
```

---

## ?? Проверка

Убедитесь, что:
1. ? `ProductLogic` создаётся через `RepositoryFactory.CreateProductLogic()`
2. ? Ninject автоматически внедряет `IRepository` и `IBusinessFunctions`
3. ? Код компилируется без ошибок
4. ? Приложение запускается и работает корректно
