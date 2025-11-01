using ProductManagementSystem.Logic;
using ProductManagementSystem.Model;
using ProductManagementSystem.DataAccessLayer.EF;
using ProductManagementSystem.DataAccessLayer.Dapper;

namespace ProductManagementSystem.WinFormsApp
{
    /// <summary>
    /// Для переключения между Entity Framework и Dapper измените константу ниже.
    /// </summary>
    internal static class RepositoryFactory
    {
        /// <summary>
        /// Тип используемого репозитория даппер или еф.
        /// </summary>
        private enum RepositoryType
        {
            EntityFramework,
            Dapper
        }

        // Переключалка тут!!!
        
        private const RepositoryType CurrentType = RepositoryType.EntityFramework;
        //private const RepositoryType CurrentType = RepositoryType.Dapper;

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
