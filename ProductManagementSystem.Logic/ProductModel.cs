using ProductManagementSystem.Model;

namespace ProductManagementSystem.Logic
{
    /// <summary>
    /// Представляет товар в системе управления товарами.
    /// Данный класс является частью бизнес-логики и предоставляет доступ к модели Product.
    /// Используется для обеспечения слабой связанности между слоями приложения.
    /// </summary>
    public class ProductModel : IDomainObject
    {
        /// <summary>
        /// Уникальный идентификатор товара
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Название товара
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Описание товара
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Цена товара за единицу
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Категория товара
        /// </summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// Количество товара на складе
        /// </summary>
        public int StockQuantity { get; set; }

        /// <summary>
        /// Инициализирует новый экземпляр класса ProductModel с пустыми значениями.
        /// </summary>
        public ProductModel() { }

        /// <summary>
        /// Инициализирует новый экземпляр класса ProductModel с указанными параметрами.
        /// </summary>
        /// <param name="id">Уникальный идентификатор товара</param>
        /// <param name="name">Название товара</param>
        /// <param name="description">Описание товара</param>
        /// <param name="price">Цена товара</param>
        /// <param name="category">Категория товара</param>
        /// <param name="stockQuantity">Количество на складе</param>
        public ProductModel(int id, string name, string description, decimal price, string category, int stockQuantity)
        {
            Id = id;
            Name = name;
            Description = description;
            Price = price;
            Category = category;
            StockQuantity = stockQuantity;
        }

        /// <summary>
        /// Создает ProductModel из Product
        /// </summary>
        public static ProductModel FromProduct(Product product)
        {
            return new ProductModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Category = product.Category,
                StockQuantity = product.StockQuantity
            };
        }

        /// <summary>
        /// Преобразует ProductModel в Product
        /// </summary>
        public Product ToProduct()
        {
            return new Product
            {
                Id = this.Id,
                Name = this.Name,
                Description = this.Description,
                Price = this.Price,
                Category = this.Category,
                StockQuantity = this.StockQuantity
            };
        }

        /// <summary>
        /// Возвращает строковое представление товара с основной информацией.
        /// </summary>
        /// <returns>Строка с информацией о товаре</returns>
        public override string ToString()
        {
            return $"ID: {Id}, Название: {Name}, Цена: {Price} RUB, Категория: {Category}, Количество: {StockQuantity}";
        }
    }
}
