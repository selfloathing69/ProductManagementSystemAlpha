using System.Data.Entity;
using ProductManagementSystem.Model;

namespace ProductManagementSystem.DataAccessLayer.EF
{
    /// <summary>
    /// Контекст базы данных Entity Framework для управления товарами.
    /// </summary>
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// Инициализирует новый экземпляр контекста базы данных.
        /// Использует строку подключения из конфигурационного файла.
        /// </summary>
        public AppDbContext() : base("name=DefaultConnection")
        {
        }

        /// <summary>
        /// Набор данных для работы с товарами.
        /// </summary>
        public DbSet<Product> Products { get; set; } = null!;

        /// <summary>
        /// Настройка модели данных при создании базы данных.
        /// </summary>
        /// <param name="modelBuilder">Построитель модели</param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Настройка таблицы Products
            modelBuilder.Entity<Product>().ToTable("Products");
            modelBuilder.Entity<Product>().HasKey(p => p.Id);
            modelBuilder.Entity<Product>().Property(p => p.Id).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<Product>().Property(p => p.Name).IsRequired().HasMaxLength(200);
            modelBuilder.Entity<Product>().Property(p => p.Description).HasMaxLength(1000);
            modelBuilder.Entity<Product>().Property(p => p.Category).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Product>().Property(p => p.Price).HasPrecision(18, 2);
        }
    }
}
