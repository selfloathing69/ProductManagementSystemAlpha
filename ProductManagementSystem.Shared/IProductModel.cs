using ProductManagementSystem.Model;

namespace ProductManagementSystem.Shared
{
    /// <summary>
    /// Шаблон MVP — Интерфейс модели (фасад бизнес-логики).
    /// Определяет контракт для слоя модели, чтобы избежать циклических зависимостей.
    /// Модель генерирует события при изменении данных, которые может отслеживать презентер.
    /// </summary>
    public interface IProductModel
    {

        /// <summary>
        /// Возникает при изменении списка товаров (добавлении/обновлении/удалении).        /// </summary>
        event EventHandler? ProductsChanged;

        /// <summary>
        /// Возникает при возникновении ошибки в модели.
        /// /// </summary>
        event EventHandler<string>? ErrorOccurred;

        /// <returns>List of all products</returns>
        List<Product> GetAllProducts();

        Product? GetProductById(int id);

        Product AddProduct(Product product);


        bool UpdateProduct(Product product);

        /// <summary>
        /// Удаляет продукт по идентификатору.
        /// </summary>

        bool DeleteProduct(int id);


        /// <summary>
        /// Фильтрует продукты по категориям.
        /// </summary>
        /// <param name="category">Category to filter by</param>
        /// <returns>List of products in the specified category</returns>
        List<Product> FilterByCategory(string category);

        /// <summary>
        /// Группирует продукты по категориям.
        /// </summary>
        /// <returns>Dictionary of categories to product lists</returns>
        Dictionary<string, List<Product>> GroupByCategory();

        /// <summary>
        /// Рассчитывает общую стоимость инвентаря.
        /// </summary>
        /// <returns>Total value of all products (price * quantity)</returns>
        decimal CalculateTotalInventoryValue();

        /// <summary>
        ///Находит продукт по названию и категории.
        /// </summary>
        /// <param name="name">Product name</param>
        /// <param name="category">Product category</param>
        /// <returns>Product if found, null otherwise</returns>
        Product? FindProductByNameAndCategory(string name, string category);

        /// <summary>
        /// Поиск продуктов по строке запроса
        /// </summary>
        /// <param name="query">Search query</param>
        /// <returns>List of matching products</returns>
        List<Product> SearchProducts(string query);

        /// <summary>
        /// Увеличивает количество существующего продукта.
        /// /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="quantity">Quantity to add</param>
        /// <returns>True if operation succeeded</returns>
        bool AddQuantityToProduct(int id, int quantity);

        /// <summary>
        /// Удаляет указанное количество товара. Если количество для удаления >= остатка на складе, товар удаляется.
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="quantityToRemove">Quantity to remove</param>
        /// <returns>True if operation succeeded</returns>
        bool RemoveQuantityFromProduct(int id, int quantityToRemove);

        /// <summary>
        /// Проверяет, существует ли продукт с указанным идентификатором.
        /// </summary>
        /// <param name="id">Product ID to check</param>
        /// <returns>True if product exists</returns>
        bool ProductExists(int id);

        /// <summary>
        /// Получает продукты с их индексами отображения.
        /// /// </summary>
        /// <returns>List of tuples with index and product</returns>
        List<(int Index, Product Product)> GetProductsWithIndexes();

    }
}
