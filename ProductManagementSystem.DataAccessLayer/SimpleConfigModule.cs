using Ninject.Modules;
using ProductManagementSystem.Logic;
using ProductManagementSystem.Logic.Services;
using ProductManagementSystem.Logic.Validators;
using ProductManagementSystem.Logic.Mappers;
using ProductManagementSystem.Model;
using ProductManagementSystem.DataAccessLayer.EF;
using ProductManagementSystem.DataAccessLayer.Dapper;
using ProductManagementSystem.Shared;

namespace ProductManagementSystem.DataAccessLayer
{
    /// <summary>
    /// Модуль конфигурации Ninject для настройки Dependency Injection.
    /// SOLID - D: Модуль использует DI-контейнер для управления зависимостями между компонентами.
    /// SOLID - O: Позволяет изменять конфигурацию без изменения клиентского кода через замену конфигурации.
    /// 
    /// MVP Pattern: This module also registers MVP components (IProductModel).
    /// 
    /// Это же самый простой конфигуратор для DI-контейнера Ninject
    /// Наша задача говорить, какие классы используем
    /// для каждого интерфейса (зависимости)
    /// </summary>
    public class SimpleConfigModule : NinjectModule
    {
        /// <summary>
        /// Метод Load() вызывается Ninject при загрузке модуля.
        /// Здесь мы регистрируем привязки (bindings) между интерфейсами и их реализациями.
        /// 
        /// Что же такое DI-контейнер:
        /// DI-контейнер в C# — это компонент, который управляет внедрением зависимостей (Dependency Injection),
        /// автоматически создавая и предоставляя объекты и их зависимости. Это позволяет коду быть тестируемым,
        /// гибким и слабосвязанным тем, что он классы зависят от интерфейсов, а не от конкретных реализаций.
        /// - Bind<TInterface>().To<TImplementation>() - связываем интерфейс с конкретной реализацией
        /// - InSingletonScope() - гарантирует, что экземпляр создается один раз и переиспользуется
        /// </summary>
        public override void Load()
        {
            // CRUD операции - Repository
            
            // SOLID - D: Привязка IRepository<Product> к конкретной реализации через DI
            // SOLID - O: Можно переключаться на другую реализацию без изменения кода клиента

            // ПЕРЕКЛЮЧАЛКА!! Раскомментируйте нужный репозиторий
            Bind<IRepository<Product>>().To<EntityRepository<Product>>().InSingletonScope();
            // Bind<IRepository<Product>>().To<DapperRepository<Product>>().InSingletonScope();

            //Бизнес-функции
            
            // SOLID - D: Привязка IBusinessFunctions к BusinessFunctions
            // BusinessFunctions отвечает за фильтрацию, группировку, расчёты
            Bind<IBusinessFunctions>().To<BusinessFunctions>().InSingletonScope();

            // ProductLogic (объединяет CRUD и бизнес-функции)
            
            // SOLID - D: Привязка ILogic к ProductLogic
            // ProductLogic использует IRepository + IBusinessFunctions
            Bind<ILogic>().To<ProductLogic>().InSingletonScope();

            // MVP Pattern - Model Layer
            
            // Привязка IProductModel к ProductModelMvp
            // ProductModelMvp обёртывает ProductLogic и добавляет события для MVP
            Bind<IProductModel>().To<ProductModelMvp>().InSingletonScope();

            // Сервисы для юай
            
            // Регистрируем сервисы согласно SOLID принципам
            Bind<ProductMapper>().ToSelf().InSingletonScope();
            Bind<ProductValidator>().ToSelf().InSingletonScope();
            Bind<IProductService>().To<ProductService>().InSingletonScope();
        }
    }
}



