using ProductManagementSystem.Logic;
using ProductManagementSystem.Model;
using Ninject;
using ProductManagementSystem.DataAccessLayer;

namespace ProductManagementSystem.WinFormsApp
{
    /// <summary>
    /// Фабрика для создания репозиториев с использованием Ninject DI-контейнера.
    /// 
    /// Тут мы перешли от ручного создания репозиториев к использованию DI-контейнера.
    /// Это реализация паттерна Dependency Injection с помощью Ninject.
    /// </summary>
    internal static class RepositoryFactory
    {
        // Тут мы создали статическое поле для хранения ядра Ninject.
        // Оно инициализируется один раз и переиспользуется во всем приложении.
        private static IKernel? _kernel;

        /// <summary>
        /// Получает или создаёт экземпляр ядра Ninject.
        /// 
        /// Тут мы реализовали паттерн Singleton для DI-контейнера:
        /// - Ядро создаётся один раз при первом обращении
        /// - Все последующие вызовы возвращают тот же экземпляр
        /// - Мы загружаем SimpleConfigModule, который содержит все настройки привязок
        /// </summary>
        private static IKernel Kernel
        {
            get
            {
                if (_kernel == null)
                {
                    // Тут мы создали ядро Ninject и загрузили модуль конфигурации.
                    // StandardKernel - это стандартная реализация DI-контейнера Ninject.
                    // new SimpleConfigModule() - загружаем наши правила привязки интерфейсов к реализациям.
                    _kernel = new StandardKernel(new SimpleConfigModule());
                }
                return _kernel;
            }
        }

        /// <summary>
        /// Создаёт экземпляр репозитория через Ninject DI-контейнер.
        /// 
        /// Тут мы используем DI-контейнер для получения репозитория:
        /// - Kernel.Get<T>() запрашивает у контейнера экземпляр указанного типа
        /// - Ninject автоматически найдёт привязку IRepository<Product> в SimpleConfigModule
        /// - Создаст нужную реализацию (EntityRepository или DapperRepository)
        /// - Вернёт готовый к использованию объект
        /// 
        /// Преимущества этого подхода:
        /// - Для переключения между EF и Dapper достаточно изменить одну строку в SimpleConfigModule
        /// - Не нужно менять код в UI-слое
        /// - Все зависимости управляются централизованно
        /// </summary>
        /// <returns>Экземпляр IRepository для работы с товарами</returns>
        public static IRepository<Product> CreateRepository()
        {
            // Тут мы запрашиваем у Ninject экземпляр IRepository<Product>.
            // Ninject автоматически создаст нужную реализацию на основе настроек в SimpleConfigModule.
            return Kernel.Get<IRepository<Product>>();
        }
    }
}
