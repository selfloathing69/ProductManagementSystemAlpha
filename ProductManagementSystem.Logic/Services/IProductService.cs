using ProductManagementSystem.Model;

namespace ProductManagementSystem.Logic.Services
{
    /// <summary>
    /// SOLID - I: Интерфейс сегрегирован для работы с товарами.
    /// SOLID - D: Высокоуровневые модули зависят от абстракции, а не от конкретной реализации.
    /// 
    /// Интерфейс сервиса для управления товарами.
    /// Определяет CRUD и бизнес-логики над товарами.
    /// </summary>
    public interface IProductService
    {
        /// <summary>
        /// Добавляет новый товар в систему.
        /// </summary>
        /// <param name="model">Модель товара для добавления</param>
        /// <returns>Добавленный товар с присвоенным ID</returns>
        ProductModel AddProduct(ProductModel model);

        /// <summary>
        /// Обновляет существующий товар.
        /// </summary>
        /// <param name="id">ID товара для обновления</param>
        /// <param name="model">Обновленные данные товара</param>
        /// <returns>true, если обновление прошло успешно; иначе false</returns>
        bool UpdateProduct(int id, ProductModel model);

        /// <summary>
        /// Удаляет товар по его ID.
        /// </summary>
        /// <param name="id">ID товара для удаления</param>
        /// <returns>true, если удаление прошло успешно; иначе false</returns>
        bool DeleteProduct(int id);

        /// <summary>
        /// Получает товар по его ID.
        /// </summary>
        /// <param name="id">ID товара</param>
        /// <returns>Модель товара или null, если товар не найден</returns>
        ProductModel? GetProduct(int id);

        /// <summary>
        /// Получает список всех товаров.
        /// </summary>
        /// <returns>Список всех товаров</returns>
        List<ProductModel> GetAllProducts();

        /// <summary>
        /// Выполняет поиск товаров по запросу.
        /// Поиск выполняется по названию, описанию и категории.
        /// </summary>
        /// <param name="query">Поисковый запрос</param>
        /// <returns>Список найденных товаров</returns>
        List<ProductModel> SearchProducts(string query);

        /// <summary>
        /// Фильтрует товары по категории.
        /// </summary>
        /// <param name="category">Название категории</param>
        /// <returns>Список товаров указанной категории</returns>
        List<ProductModel> FilterByCategory(string category);

        /// <summary>
        /// Вычисляет общую стоимость всех товаров на складе.
        /// </summary>
        /// <returns>Общая стоимость инвентаря</returns>
        decimal CalculateTotalInventoryValue();

        /// <summary>
        /// Добавляет количество товара на склад.
        /// </summary>
        /// <param name="id">ID товара</param>
        /// <param name="quantity">Количество для добавления</param>
        /// <returns>true, если операция прошла успешно</returns>
        bool AddQuantityToProduct(int id, int quantity);

        /// <summary>
        /// Удаляет указанное количество товара со склада.
        /// </summary>
        /// <param name="id">ID товара</param>
        /// <param name="quantityToRemove">Количество для удаления</param>
        /// <returns>Результат операции удаления</returns>
        DeleteProductResult DeleteProductByQuantity(int id, int quantityToRemove);

        /// <summary>
        /// Добавляет товар с проверкой дубликатов и возможностью объединения.
        /// </summary>
        /// <param name="model">Модель товара</param>
        /// <param name="allowMerge">Разрешить объединение с существующим товаром</param>
        /// <returns>Результат операции добавления</returns>
        AddProductResult AddProductWithValidation(ProductModel model, bool allowMerge);
    }
}
