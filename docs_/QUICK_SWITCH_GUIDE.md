# ?? Быстрая инструкция: Переключение репозитория

## Для переключения между Entity Framework и Dapper

### ?? Откройте ОДИН файл:
```
ProductManagementSystem.DataAccessLayer/SimpleConfigModule.cs
```

### ?? Измените ОДНУ строку в методе `Load()`:

#### Вариант 1: Entity Framework (по умолчанию)
```csharp
public override void Load()
{
    // ? Используется Entity Framework
    Bind<IRepository<Product>>().To<EntityRepository<Product>>().InSingletonScope();
    
    // ? Dapper закомментирован
    // Bind<IRepository<Product>>().To<DapperRepository<Product>>().InSingletonScope();
}
```

#### Вариант 2: Dapper
```csharp
public override void Load()
{
    // ? Entity Framework закомментирован
    // Bind<IRepository<Product>>().To<EntityRepository<Product>>().InSingletonScope();
    
    // ? Используется Dapper
    Bind<IRepository<Product>>().To<DapperRepository<Product>>().InSingletonScope();
}
```

### ?? Сохраните файл

### ?? Перезапустите приложение

**Готово!** Все UI-приложения (WinForms, Console, WPF) автоматически используют новый репозиторий.

---

## ? Преимущества нового подхода

| До (без Ninject) | После (с Ninject) |
|------------------|-------------------|
| Изменение в **3 файлах** | Изменение в **1 файле** |
| Ручное создание объектов | Автоматическое внедрение |
| Жёсткая связь | Слабая связь |
| Сложное тестирование | Лёгкое тестирование |

---

## ?? Подробная документация

Для детального изучения смотрите:
- `NINJECT_DI_IMPLEMENTATION.md` - полное руководство по DI
- `LAB3_SUMMARY.md` - резюме лабораторной работы №3
- `REPOSITORY_SWITCHING_GUIDE.md` - расширенная инструкция

---

**Время переключения:** < 1 минуты  
**Файлов для изменения:** 1  
**Перезапуск приложения:** Требуется  

? Быстро | ? Просто | ? Централизованно
