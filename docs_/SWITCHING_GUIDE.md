# Руководство по переключению между Entity Framework и Dapper

## Быстрое переключение

### Шаг 1: Откройте нужный UI проект

Выберите один из файлов:
- **Console App**: `ProductManagementSystem.ConsoleApp/Program.cs`
- **WinForms App**: `ProductManagementSystem.WinFormsApp/MainForm.cs`
- **WPF App**: `ProductManagementSystem.WpfApp/MainWindow.xaml.cs`

### Шаг 2: Найдите блок подключения репозитория

В начале файла вы увидите:

```csharp
using ProductManagementSystem.DataAccessLayer.EF;
//using ProductManagementSystem.DataAccessLayer.Dapper;
```

И в классе:

```csharp
private ProductLogic _logic = new ProductLogic(new EntityRepository<Product>());
//private ProductLogic _logic = new ProductLogic(new DapperRepository<Product>());
```

### Шаг 3: Переключение на Dapper

**Закомментируйте Entity Framework:**
```csharp
//using ProductManagementSystem.DataAccessLayer.EF;
using ProductManagementSystem.DataAccessLayer.Dapper;
```

```csharp
//private ProductLogic _logic = new ProductLogic(new EntityRepository<Product>());
private ProductLogic _logic = new ProductLogic(new DapperRepository<Product>());
```

### Шаг 4: Переключение обратно на Entity Framework

**Закомментируйте Dapper:**
```csharp
using ProductManagementSystem.DataAccessLayer.EF;
//using ProductManagementSystem.DataAccessLayer.Dapper;
```

```csharp
private ProductLogic _logic = new ProductLogic(new EntityRepository<Product>());
//private ProductLogic _logic = new ProductLogic(new DapperRepository<Product>());
```

## Проверка работы

1. Пересоберите проект (Ctrl+Shift+B)
2. Запустите приложение
3. Проверьте, что данные загружаются из базы данных

## Отличия между EF и Dapper

| Аспект | Entity Framework | Dapper |
|--------|------------------|--------|
| Производительность | Медленнее | Быстрее |
| Простота использования | Проще (авто-маппинг) | Требует SQL запросов |
| Отслеживание изменений | Есть | Нет |
| Lazy Loading | Поддерживается | Не поддерживается |
| Миграции | Встроенные | Ручные |

## Примечания

- Оба репозитория работают с одной и той же базой данных
- Переключение не требует изменений в базе данных
- Функциональность приложения остается одинаковой
- Можно использовать разные репозитории в разных UI одновременно

## База данных

**Строка подключения:**
```
Server=AspireNotebook\SQLEXPRESS;Database=ProductManagementDB;Integrated Security=True;TrustServerCertificate=True;
```

**Создание базы данных:**
- Entity Framework создаст базу автоматически при первом запуске
- Dapper требует существующую базу данных (создается EF)

## Рекомендации

- **Для разработки**: Используйте Entity Framework (проще отладка)
- **Для production**: Рассмотрите Dapper (выше производительность)
- **Для прототипов**: Entity Framework (быстрее разработка)
- **Для сложных запросов**: Dapper (больше контроля)
