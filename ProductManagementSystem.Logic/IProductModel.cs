using ProductManagementSystem.Model;

namespace ProductManagementSystem.Logic
{
    /// <summary>
    /// Шаблон MVP — Интерфейс модели (фасад бизнес-логики).
    /// Определяет контракт для слоя модели, чтобы избежать циклических зависимостей.
    /// Модель генерирует события при изменении данных, которые может отслеживать презентер.
    /// Model работает с доменными объектами Product, Presenter преобразует их в DTO для View.
    /// </summary>
    public interface IProductModel
    {
        #region Events - Data Change Notifications

        /// <summary>
        /// Возникает при изменении списка товаров(add/update/delete)
        /// </summary>
        event EventHandler? ProductsChanged;

        /// <summary>
        /// Возникает при возникновении ошибки в Model.
        /// </summary>
        event EventHandler<string>? ErrorOccurred;

        #endregion

        #region CRUD Operations

        List<Product> GetAllProducts();
        Product? GetProductById(int id);
        Product AddProduct(Product product);
        bool UpdateProduct(Product product);

        bool DeleteProduct(int id);

        #endregion

        #region Business Functions

        List<Product> FilterByCategory(string category);
        Dictionary<string, List<Product>> GroupByCategory();
        decimal CalculateTotalInventoryValue();
        Product? FindProductByNameAndCategory(string name, string category);
        List<Product> SearchProducts(string query);
        bool AddQuantityToProduct(int id, int quantity);
        bool RemoveQuantityFromProduct(int id, int quantityToRemove);
        bool ProductExists(int id);
        List<(int Index, Product Product)> GetProductsWithIndexes();
        #endregion
    }
}
