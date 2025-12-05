using ProductManagementSystem.Model;

namespace ProductManagementSystem.Logic
{
    /// <summary>
    /// SOLID - I: Интерфейс для бизнес-функций, отделённых от CRUD операций.
    /// SOLID - S: Отвечает только за бизнес-логику (фильтрация, расчёты, группировка).
    /// 
    /// Интерфейс для дополнительных бизнес-операций над товарами.
    /// Содержит методы, не относящиеся к базовым CRUD операциям.
    /// </summary>
    public interface IBusinessFunctions
    {
        /// <summary>
        /// Фильтрует товары по категории.
        /// </summary>
        /// <param name="products">Коллекция товаров для фильтрации</param>
        /// <param name="category">Категория для фильтра</param>
        /// <returns>Отфильтрованный список товаров</returns>
        List<Product> FilterByCategory(IEnumerable<Product> products, string category);

        /// <summary>
        /// Группирует товары по категориям.
        /// </summary>
        /// <param name="products">Коллекция товаров для группировки</param>
        /// <returns>Словарь: категория -> список товаров</returns>
        Dictionary<string, List<Product>> GroupByCategory(IEnumerable<Product> products);

        /// <summary>
        /// Рассчитывает общую стоимость товаров на складе.
        /// </summary>
        /// <param name="products">Коллекция товаров</param>
        /// <returns>Общая стоимость</returns>
        decimal CalculateTotalInventoryValue(IEnumerable<Product> products);

        /// <summary>
        /// Находит товар по названию и категории.
        /// </summary>
        /// <param name="products">Коллекция товаров</param>
        /// <param name="name">Название товара</param>
        /// <param name="category">Категория товара</param>
        /// <returns>Найденный товар или null</returns>
        Product? FindByNameAndCategory(IEnumerable<Product> products, string name, string category);

        /// <summary>
        /// Выполняет поиск товаров по запросу.
        /// </summary>
        /// <param name="products">Коллекция товаров</param>
        /// <param name="query">Поисковый запрос</param>
        /// <returns>Список найденных товаров</returns>
        List<Product> Search(IEnumerable<Product> products, string query);

        /// <summary>
        /// Проверяет существование товара с указанным ID.
        /// </summary>
        /// <param name="products">Коллекция товаров</param>
        /// <param name="id">ID для проверки</param>
        /// <returns>true, если товар существует</returns>
        bool ExistsById(IEnumerable<Product> products, int id);
    }
}
