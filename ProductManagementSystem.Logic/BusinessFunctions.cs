using System;
using System.Collections.Generic;
using System.Linq;
using ProductManagementSystem.Model;

namespace ProductManagementSystem.Logic
{
    /// <summary>
    /// SOLID - S: Класс отвечает ТОЛЬКО за бизнес-логику (фильтрация, расчёты, группировка).
    /// SOLID - O: Можно расширять новыми бизнес-функциями без изменения CRUD логики.
    /// 
    /// Реализация бизнес-функций для работы с товарами.
    /// Содержит методы фильтрации, поиска, расчётов и группировки.
    /// </summary>
    public class BusinessFunctions : IBusinessFunctions
    {
        /// <summary>
        /// Фильтрует товары по категории.
        /// Поиск выполняется без учёта регистра.
        /// </summary>
        public List<Product> FilterByCategory(IEnumerable<Product> products, string category)
        {
            return products
                .Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        /// <summary>
        /// Группирует товары по категориям.
        /// </summary>
        public Dictionary<string, List<Product>> GroupByCategory(IEnumerable<Product> products)
        {
            return products
                .GroupBy(p => p.Category)
                .ToDictionary(g => g.Key, g => g.ToList());
        }

        /// <summary>
        /// Рассчитывает общую стоимость всех товаров на складе.
        /// Учитывает цену товара и его количество на складе.
        /// </summary>
        public decimal CalculateTotalInventoryValue(IEnumerable<Product> products)
        {
            return products.Sum(p => p.Price * p.StockQuantity);
        }

        /// <summary>
        /// Находит товар с таким же названием и категорией.
        /// Поиск выполняется без учёта регистра.
        /// </summary>
        public Product? FindByNameAndCategory(IEnumerable<Product> products, string name, string category)
        {
            return products.FirstOrDefault(p =>
                p.Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
                p.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Выполняет поиск товаров по запросу.
        /// Поиск выполняется по названию, описанию и категории.
        /// </summary>
        public List<Product> Search(IEnumerable<Product> products, string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return products.ToList();
            }

            return products.Where(p =>
                p.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                p.Description.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                p.Category.Contains(query, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        /// <summary>
        /// Проверяет существование товара с указанным ID.
        /// </summary>
        public bool ExistsById(IEnumerable<Product> products, int id)
        {
            return products.Any(p => p.Id == id);
        }
    }
}
