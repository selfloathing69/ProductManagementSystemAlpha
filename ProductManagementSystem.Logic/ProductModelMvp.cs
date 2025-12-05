using System;
using System.Collections.Generic;
using System.Linq;
using ProductManagementSystem.Model;
using ProductManagementSystem.Shared;

namespace ProductManagementSystem.Logic
{
    /// <summary>
    /// MVP паттерн - реализация модели.
    /// Реализует интерфейс IProductModel и обёртывает существующий ProductLogic.
    /// Генерирует события при изменении данных для уведомления презентера.
    /// </summary>
    public class ProductModelMvp : IProductModel
    {
        private readonly ProductLogic _logic;

        #region Events

        /// <inheritdoc/>
        public event EventHandler? ProductsChanged;

        /// <inheritdoc/>
        public event EventHandler<string>? ErrorOccurred;

        #endregion

        #region Constructor

        /// <summary>
        /// Инициализирует новый экземпляр ProductModelMvp с указанным ProductLogic.
        /// </summary>
        /// <param name="logic">Экземпляр ProductLogic для обёртывания</param>
        public ProductModelMvp(ProductLogic logic)
        {
            _logic = logic ?? throw new ArgumentNullException(nameof(logic));
        }

        #endregion

        #region Event Raising Methods

        /// <summary>
        /// Вызывает событие ProductsChanged.
        /// </summary>
        protected virtual void OnProductsChanged()
        {
            ProductsChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Вызывает событие ErrorOccurred.
        /// </summary>
        /// <param name="errorMessage">Error message to include</param>
        protected virtual void OnErrorOccurred(string errorMessage)
        {
            ErrorOccurred?.Invoke(this, errorMessage);
        }

        #endregion

        #region CRUD операции

        /// <inheritdoc/>
        public List<Product> GetAllProducts()
        {
            try
            {
                return _logic.GetAll();
            }
            catch (Exception ex)
            {
                OnErrorOccurred($"Ошибка получения товаров: {ex.Message}");
                return new List<Product>();
            }
        }

        /// <inheritdoc/>
        public Product? GetProductById(int id)
        {
            try
            {
                return _logic.GetById(id);
            }
            catch (Exception ex)
            {
                OnErrorOccurred($"Ошибка получения товара по ID: {ex.Message}");
                return null;
            }
        }

        /// <inheritdoc/>
        public Product AddProduct(Product product)
        {
            try
            {
                var result = _logic.Add(product);
                OnProductsChanged();
                return result;
            }
            catch (Exception ex)
            {
                OnErrorOccurred($"Ошибка добавления товара: {ex.Message}");
                throw;
            }
        }

        /// <inheritdoc/>
        public bool UpdateProduct(Product product)
        {
            try
            {
                var result = _logic.Update(product);
                if (result)
                {
                    OnProductsChanged();
                }
                return result;
            }
            catch (Exception ex)
            {
                OnErrorOccurred($"Ошибка обновления товара: {ex.Message}");
                return false;
            }
        }

        /// <inheritdoc/>
        public bool DeleteProduct(int id)
        {
            try
            {
                var result = _logic.Delete(id);
                if (result)
                {
                    OnProductsChanged();
                }
                return result;
            }
            catch (Exception ex)
            {
                OnErrorOccurred($"Ошибка удаления товара: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region Business Functions

        /// <inheritdoc/>
        public List<Product> FilterByCategory(string category)
        {
            try
            {
                return _logic.FilterByCategory(category);
            }
            catch (Exception ex)
            {
                OnErrorOccurred($"Ошибка фильтрации по категории: {ex.Message}");
                return new List<Product>();
            }
        }

        /// <inheritdoc/>
        public Dictionary<string, List<Product>> GroupByCategory()
        {
            try
            {
                return _logic.GroupByCategory();
            }
            catch (Exception ex)
            {
                OnErrorOccurred($"Ошибка группировки по категории: {ex.Message}");
                return new Dictionary<string, List<Product>>();
            }
        }

        /// <inheritdoc/>
        public decimal CalculateTotalInventoryValue()
        {
            try
            {
                return _logic.CalculateTotalInventoryValue();
            }
            catch (Exception ex)
            {
                OnErrorOccurred($"Ошибка расчёта общей стоимости запасов: {ex.Message}");
                return 0;
            }
        }

        /// <inheritdoc/>
        public Product? FindProductByNameAndCategory(string name, string category)
        {
            try
            {
                return _logic.FindProductByNameAndCategory(name, category);
            }
            catch (Exception ex)
            {
                OnErrorOccurred($"Ошибка поиска товара: {ex.Message}");
                return null;
            }
        }

        /// <inheritdoc/>
        public List<Product> SearchProducts(string query)
        {
            try
            {
                return _logic.SearchProducts(query);
            }
            catch (Exception ex)
            {
                OnErrorOccurred($"Ошибка поиска товаров: {ex.Message}");
                return new List<Product>();
            }
        }

        /// <inheritdoc/>
        public bool AddQuantityToProduct(int id, int quantity)
        {
            try
            {
                var result = _logic.AddQuantityToProduct(id, quantity);
                if (result)
                {
                    OnProductsChanged();
                }
                return result;
            }
            catch (Exception ex)
            {
                OnErrorOccurred($"Ошибка добавления количества к товару: {ex.Message}");
                return false;
            }
        }

        /// <inheritdoc/>
        public bool RemoveQuantityFromProduct(int id, int quantityToRemove)
        {
            try
            {
                var result = _logic.RemoveQuantityFromProduct(id, quantityToRemove);
                if (result)
                {
                    OnProductsChanged();
                }
                return result;
            }
            catch (Exception ex)
            {
                OnErrorOccurred($"Ошибка удаления количества из товара: {ex.Message}");
                return false;
            }
        }

        /// <inheritdoc/>
        public bool ProductExists(int id)
        {
            try
            {
                // Используем GetProductById вместо устаревшего IdExists
                return _logic.GetById(id) != null;
            }
            catch (Exception ex)
            {
                OnErrorOccurred($"Ошибка проверки существования товара: {ex.Message}");
                return false;
            }
        }

        /// <inheritdoc/>
        public List<(int Index, Product Product)> GetProductsWithIndexes()
        {
            try
            {
                // Используем GetAll() с Select для создания индексированных кортежей вместо устаревшего метода
                return _logic.GetAll().Select((p, index) => (index + 1, p)).ToList();
            }
            catch (Exception ex)
            {
                OnErrorOccurred($"Ошибка получения товаров с индексами: {ex.Message}");
                return new List<(int, Product)>();
            }
        }

        #endregion
    }
}
