using Ninject.Modules;
using ProductManagementSystem.Logic;
using ProductManagementSystem.Model;
using ProductManagementSystem.DataAccessLayer.EF;
using ProductManagementSystem.DataAccessLayer.Dapper;

namespace ProductManagementSystem.DataAccessLayer
{
    /// <summary>
    /// Модуль конфигурации Ninject для настройки Dependency Injection.
    /// SOLID - D: Модуль конфигурации DI-контейнера для управления зависимостями через интерфейсы.
    /// SOLID - O: Позволяет расширять функциональность без изменения существующего кода через замену реализаций.
    /// 
    /// Тут мы создаём модуль конфигурации для DI-контейнера Ninject
    /// Этот модуль определяет, какую реализацию нужно использовать
    /// для каждого интерфейса (зависимости0
    /// </summary>
    public class SimpleConfigModule : NinjectModule
    {
        /// <summary>
        /// Метод Load() вызывается Ninject при загрузке модуля.
        /// Здесь мы регистрируем привязки (bindings) между интерфейсами и их реализациями.
        ///         /// Тут мы настроим DI-контейнер:
        /// DI-контейнер в C# — это инструмент, который управляет внедрением зависимостей (Dependency Injection),
        /// автоматически создавая и настраивая объекты и их зависимости. Это позволяет писать более слабосвязанный,
        /// гибкий и тестируемый код, так как классы зависят от интерфейсов, а не от конкретных реализаций.
        /// - Bind<TInterface>().To<TImplementation>() - связывает интерфейс с конкретной реализацией
        /// - InSingletonScope() - гарантирует, что экземпляр создаётся один раз и переиспользуется
        /// </summary>
        public override void Load()
        {
            //  Использование репозиториев 

            // Тут мы настраиваем привязку для интерфейса Product.
            // Когда кто-то запросит IRepository<Product>, Ninject создаст EntityRepository<Product>.
            // 
            // InSingletonScope() означает:
            // Контейнер создаёт экземпляр репозитория 1 раз при первом запросе
            // Все последующие запросы получают тот же самый экземпляр
            // Это улучшает скорость и предотвращает дублирование объектов в течение жизни приложения

            // SOLID - D: Привязка IRepository<Product> к конкретной реализации через DI
            // SOLID - O: Можно переключиться на другую реализацию без изменения кода клиента



            // ПЕРЕКЛЮЧАТЬ ТУТ!!
            Bind<IRepository<Product>>().To<EntityRepository<Product>>().InSingletonScope();
            // Bind<IRepository<Product>>().To<DapperRepository<Product>>().InSingletonScope();
        }
    }
}
