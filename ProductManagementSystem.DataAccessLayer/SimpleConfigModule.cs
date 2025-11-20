using Ninject.Modules;
using ProductManagementSystem.Logic;
using ProductManagementSystem.Logic.Services;
using ProductManagementSystem.Logic.Validators;
using ProductManagementSystem.Logic.Mappers;
using ProductManagementSystem.Model;
using ProductManagementSystem.DataAccessLayer.EF;
using ProductManagementSystem.DataAccessLayer.Dapper;

namespace ProductManagementSystem.DataAccessLayer
{
    /// <summary>
    /// Модуль конфигурации Ninject для настройки Dependency Injection.
    /// SOLID - D: Модуль использует DI-контейнер для управления зависимостями между компонентами.
    /// SOLID - O: Позволяет изменять конфигурацию без изменения клиентского кода через замену конфигурации.
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
        ///         /// Что же такое DI-контейнер:
        /// DI-контейнер в C# — это компонент, который управляет внедрением зависимостей (Dependency Injection),
        /// автоматически создавая и предоставляя объекты и их зависимости. Это позволяет коду быть тестируемым,
        /// гибким и слабосвязанным тем, что он классы зависят от интерфейсов, а не от конкретных реализаций.
        /// - Bind<TInterface>().To<TImplementation>() - связываем интерфейс с конкретной реализацией
        /// - InSingletonScope() - гарантирует, что экземпляр создается один раз и переиспользуется
        /// </summary>
        public override void Load()
        {
            //  Регистрируем зависимости 

            // Тут мы регистрируем репозиторий для доменного Product.
            // Когда кто-то попросит IRepository<Product>, Ninject вернёт EntityRepository<Product>.
            // 
            // InSingletonScope() означает:
            // Контейнер создаст экземпляр репозитория 1 раз при первом запросе
            // Все последующие запросы получат тот же самый экземпляр
            // Это повышает эфф и предотвращает множественные подключения к базе через разных экземпляров

            // SOLID - D: Привязка IRepository<Product> к конкретной реализации через DI
            // SOLID - O: Можно переключаться на другую реализацию без изменения кода клиента

            // Раскомментируй нужный репозиторий!!
            Bind<IRepository<Product>>().To<EntityRepository<Product>>().InSingletonScope();
            // Bind<IRepository<Product>>().To<DapperRepository<Product>>().InSingletonScope();

            // Регистрируем новые сервисы согласно SOLID принципам
            Bind<ProductMapper>().ToSelf().InSingletonScope();
            Bind<ProductValidator>().ToSelf().InSingletonScope();
            Bind<IProductService>().To<ProductService>().InSingletonScope();
        }
    }
}

