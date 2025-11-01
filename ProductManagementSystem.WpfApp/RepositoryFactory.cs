using ProductManagementSystem.Logic;
using ProductManagementSystem.Model;
using ProductManagementSystem.DataAccessLayer.EF;
using ProductManagementSystem.DataAccessLayer.Dapper;

namespace ProductManagementSystem.WpfApp
{
    /// <summary>
    /// Фабрика для создания репозиториев.
    /// Для переключения между Entity Framework и Dapper измените константу ниже.
    /// </summary>
    internal static class RepositoryFactory
    {
        /// <summary>
        /// Тип используемого репозитория.
        /// Закомментируйте/раскомментируйте нужную строку для переключения.
        /// </summary>
        private enum RepositoryType
        {
            EntityFramework,
            Dapper
        }

        // === ПЕРЕКЛЮЧЕНИЕ РЕПОЗИТОРИЯ ===
        // Раскомментируйте нужную строку:
        
        private const RepositoryType CurrentType = RepositoryType.EntityFramework;
        //private const RepositoryType CurrentType = RepositoryType.Dapper;

        /// <summary>
        /// Создаёт экземпляр репозитория в соответствии с выбранным типом.
        /// </summary>
        /// <returns>Экземпляр IRepository для работы с товарами</returns>
        public static IRepository<Product> CreateRepository()
        {
            return CurrentType switch
            {
                RepositoryType.EntityFramework => new EntityRepository<Product>(),
                RepositoryType.Dapper => new DapperRepository<Product>(),
                _ => throw new System.NotSupportedException($"Тип репозитория {CurrentType} не поддерживается")
            };
        }
    }
}
