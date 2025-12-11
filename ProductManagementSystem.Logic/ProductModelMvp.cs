using System;
using System.Collections.Generic;
using System.Linq;
using ProductManagementSystem.Model;

namespace ProductManagementSystem.Logic
{
    /// <summary>
    /// Шаблон MVP — Реализация модели.
    /// Реализует интерфейс IProductModel и оборачивает существующий ProductLogic.
    /// Вызывает события при изменении данных для уведомления презентера.
    /// </summary>
    public class ProductModelMvp : IProductModel
    {
        private readonly ProductLogic _logic;
        public event EventHandler? ProductsChanged;
        public event EventHandler<string>? ErrorOccurred;


 

        /// <summary>
        /// Инициализирует новый экземпляр ProductModelMvp с указанным ProductLogic.
        /// </summary>
        public ProductModelMvp(ProductLogic logic)
        {
            _logic = logic ?? throw new ArgumentNullException(nameof(logic));
        }


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
        protected virtual void OnErrorOccurred(string errorMessage)
        {
            ErrorOccurred?.Invoke(this, errorMessage);
        }


        public List<Product> GetAllProducts()
        {
            try
            {
                return _logic.GetAll();
            }
            catch (Exception ex)
            {
                OnErrorOccurred($"ошибка: {ex.Message}");
                return new List<Product>();
            }
        }

        public Product? GetProductById(int id)
        {
            try
            {
                return _logic.GetById(id);
            }
            catch (Exception ex)
            {
                OnErrorOccurred($"ошибка получения айди: {ex.Message}");
                return null;
            }
        }


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
                OnErrorOccurred($"ошибка добавления: {ex.Message}");
                throw;
            }
        }

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
                OnErrorOccurred($"ошибка обновы: {ex.Message}");
                return false;
            }
        }


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
                OnErrorOccurred($"ошибка удадения: {ex.Message}");
                return false;
            }
        }


        public List<Product> FilterByCategory(string category)
        {
            try
            {
                return _logic.FilterByCategory(category);
            }
            catch (Exception ex)
            {
                OnErrorOccurred($"ошибка: {ex.Message}");
                return new List<Product>();
            }
        }

        public Dictionary<string, List<Product>> GroupByCategory()
        {
            try
            {
                return _logic.GroupByCategory();
            }
            catch (Exception ex)
            {
                OnErrorOccurred($"ошибка: {ex.Message}");
                return new Dictionary<string, List<Product>>();
            }
        }


        public decimal CalculateTotalInventoryValue()
        {
            try
            {
                return _logic.CalculateTotalInventoryValue();
            }
            catch (Exception ex)
            {
                OnErrorOccurred($"ошибка: {ex.Message}");
                return 0;
            }
        }


        public Product? FindProductByNameAndCategory(string name, string category)
        {
            try
            {
                return _logic.FindProductByNameAndCategory(name, category);
            }
            catch (Exception ex)
            {
                OnErrorOccurred($"ошибка: {ex.Message}");
                return null;
            }
        }


        public List<Product> SearchProducts(string query)
        {
            try
            {
                return _logic.SearchProducts(query);
            }
            catch (Exception ex)
            {
                OnErrorOccurred($"ошибка: {ex.Message}");
                return new List<Product>();
            }
        }

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
                OnErrorOccurred($"ошибка: {ex.Message}");
                return false;
            }
        }


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
                OnErrorOccurred($"ошибка: {ex.Message}");
                return false;
            }
        }


        public bool ProductExists(int id)
        {
            try
            {
                return _logic.GetById(id) != null;
            }
            catch (Exception ex)
            {
                OnErrorOccurred($"ошибка: {ex.Message}");
                return false;
            }
        }


        public List<(int Index, Product Product)> GetProductsWithIndexes()
        {
            try
            {
                return _logic.GetAll().Select((p, index) => (index + 1, p)).ToList();
            }
            catch (Exception ex)
            {
                OnErrorOccurred($"ошибка: {ex.Message}");
                return new List<(int, Product)>();
            }
        }

    }
}
