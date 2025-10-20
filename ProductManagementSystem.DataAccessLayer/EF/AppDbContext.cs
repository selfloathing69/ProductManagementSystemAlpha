using Microsoft.EntityFrameworkCore;
using ProductManagementSystem.Model;

namespace ProductManagementSystem.DataAccessLayer.EF
{
    /// <summary>
    /// Контекст базы данных для Entity Framework.
    /// Управляет подключением к базе данных и определяет наборы сущностей (DbSet) для работы с таблицами.
    /// </summary>
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// Набор сущностей Product для работы с таблицей Products.
        /// </summary>
        public DbSet<Product> Products { get; set; }

        /// <summary>
        /// Инициализирует новый экземпляр AppDbContext с настройками по умолчанию.
        /// </summary>
        public AppDbContext() : base()
        {
        }

        /// <summary>
        /// Инициализирует новый экземпляр AppDbContext с пользовательскими настройками.
        /// </summary>
        /// <param name="options">Опции конфигурации контекста базы данных</param>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// Настройка подключения к базе данных.
        /// Использует строку подключения из конфигурации или значение по умолчанию.
        /// </summary>
        /// <param name="optionsBuilder">Построитель настроек контекста</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Строка подключения по умолчанию для SQL Server LocalDB
                // Server=AspireNotebook\\SQLEXPRESS;Database=ProductManagementDB;Integrated Security=True;TrustServerCertificate=True
                var connectionString = "Server=AspireNotebook\\SQLEXPRESS;Database=ProductManagementDB;Integrated Security=True;TrustServerCertificate=True;";
                
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        /// <summary>
        /// Настройка маппинга сущностей на таблицы базы данных.
        /// </summary>
        /// <param name="modelBuilder">Построитель модели базы данных</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Настройка таблицы Products
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Products");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Category).IsRequired().HasMaxLength(100);
                entity.Property(e => e.StockQuantity).IsRequired();

            });
        }
    }
}
