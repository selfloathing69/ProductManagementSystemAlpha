using Ninject.Modules;
using ProductManagementSystem.Logic;
using ProductManagementSystem.Model;
using ProductManagementSystem.DataAccessLayer.EF;
using ProductManagementSystem.DataAccessLayer.Dapper;

namespace ProductManagementSystem.DataAccessLayer
{
    /// <summary>
    /// Модуль конфигурации Ninject для настройки Dependency Injection.
    /// 
    /// Тут мы создали модуль конфигурации для DI-контейнера Ninject.
    /// Этот модуль определяет, какие конкретные классы должны использоваться
    /// при запросе интерфейсов (абстракций).
    /// </summary>
    public class SimpleConfigModule : NinjectModule
    {
        /// <summary>
        /// Метод Load() вызывается Ninject при загрузке модуля.
        /// Здесь мы настраиваем привязки (bindings) между интерфейсами и их реализациями.
        /// 
        /// Тут мы сделали настройку DI-контейнера:
        /// - Bind<TInterface>().To<TImplementation>() - связываем интерфейс с конкретной реализацией
        /// - InSingletonScope() - указываем, что экземпляр должен создаваться один раз и переиспользоваться
        /// </summary>
        public override void Load()
        {
            // === КОНФИГУРАЦИЯ РЕПОЗИТОРИЕВ ===
            
            // Тут мы настроили привязку для репозитория Product.
            // Когда кто-то запросит IRepository<Product>, Ninject создаст EntityRepository<Product>.
            // 
            // InSingletonScope() означает:
            // - Экземпляр репозитория создаётся один раз при первом запросе
            // - Все последующие запросы получают тот же самый экземпляр
            // - Это экономит ресурсы и обеспечивает согласованность данных в рамках приложения
            Bind<IRepository<Product>>().To<EntityRepository<Product>>().InSingletonScope();
            
            // === ДЛЯ ПЕРЕКЛЮЧЕНИЯ НА DAPPER ===
            // Тут мы можем переключиться на Dapper, просто изменив одну строку:
            // Закомментируйте строку выше и раскомментируйте эту:
            // Bind<IRepository<Product>>().To<DapperRepository<Product>>().InSingletonScope();
            
            // === ДОБАВЛЕНИЕ ДРУГИХ СУЩНОСТЕЙ ===
            // Если у вас есть другие сущности (Order, User и т.д.), добавьте их привязки здесь:
            // Bind<IRepository<Order>>().To<EntityRepository<Order>>().InSingletonScope();
            // Bind<IRepository<User>>().To<EntityRepository<User>>().InSingletonScope();
        }
    }
}
