using ProductManagementSystem.Model;

namespace ProductManagementSystem.Logic
{
    /// <summary>
    /// SOLID - I: Интерфейс для бизнес-логики управления товарами.
    /// SOLID - D: Высокоуровневые модули зависят от этой абстракции.
    /// 
    /// Интерфейс логики управления товарами.
    /// Объединяет CRUD операции и бизнес-функции.
    /// </summary>
    public interface ILogic
    {
        // CRUD операции делегируются в IRepository
        
        /// <summary>
        /// Добавляет новый товар в систему.
        /// </summary>
        Product Add(Product product);

        /// <summary>
        /// Получает товар по его ID.
        /// </summary>
        Product? GetById(int id);

        /// <summary>
        /// Получает все товары.
        /// </summary>
        List<Product> GetAll();

        /// <summary>
        /// Обновляет существующий товар.
        /// </summary>
        bool Update(Product product);

        /// <summary>
        /// Удаляет товар по ID.
        /// </summary>
        bool Delete(int id);

        // бизнес-функции делегируются в IBusinessFunctions

        /// <summary>
        /// Фильтрует товары по категории.
        /// </summary>
        List<Product> FilterByCategory(string category);

        /// <summary>
        /// Группирует товары по категориям.
        /// </summary>
        Dictionary<string, List<Product>> GroupByCategory();

        /// <summary>
        /// Рассчитывает общую стоимость склада.
        /// </summary>
        decimal CalculateTotalInventoryValue();

        /// <summary>
        /// Находит товар по названию и категории.
        /// </summary>
        Product? FindProductByNameAndCategory(string name, string category);

        /// <summary>
        /// Выполняет поиск товаров.
        /// </summary>
        List<Product> SearchProducts(string query);

        // Дополнительные операции

        /// <summary>
        /// Увеличивает количество товара.
        /// </summary>
        bool AddQuantityToProduct(int id, int quantity);

        /// <summary>
        /// Уменьшает количество товара.
        /// </summary>
        bool RemoveQuantityFromProduct(int id, int quantityToRemove);

        /// <summary>
        /// Добавляет товар с валидацией и возможностью слияния.
        /// </summary>
        AddProductResult AddProductWithValidation(Product product, bool allowMerge);

        /// <summary>
        /// Удаляет указанное количество товара.
        /// </summary>
        DeleteProductResult DeleteProductByQuantity(int productId, int quantityToDelete);
    }
}
