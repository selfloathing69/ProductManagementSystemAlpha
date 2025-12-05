namespace ProductManagementSystem.Shared
{
    /// <summary>
    /// Data Transfer Object для товара.
    /// Используется для передачи данных между Presenter и View без зависимости от Model.
    /// MVP Pattern: View работает с DTO, а не с доменными объектами.
    /// </summary>
    public class ProductDto
    {
        /// <summary>
        /// Уникальный идентификатор товара.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Название товара.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Описание товара.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Цена товара за единицу.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Категория товара.
        /// </summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// Количество товара на складе.
        /// </summary>
        public int StockQuantity { get; set; }

        /// <summary>
        /// Инициализирует новый экземпляр класса ProductDto с пустыми значениями.
        /// </summary>
        public ProductDto() { }

        /// <summary>
        /// Инициализирует новый экземпляр класса ProductDto с указанными параметрами.
        /// </summary>
        public ProductDto(int id, string name, string description, decimal price, string category, int stockQuantity)
        {
            Id = id;
            Name = name;
            Description = description;
            Price = price;
            Category = category;
            StockQuantity = stockQuantity;
        }

        /// <summary>
        /// Возвращает строковое представление товара.
        /// </summary>
        public override string ToString()
        {
            return $"ID: {Id}, Название: {Name}, Цена: {Price} руб., Категория: {Category}, Количество: {StockQuantity}";
        }
    }
}