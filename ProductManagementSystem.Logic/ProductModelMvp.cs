using System;
using System.Collections.Generic;
using System.Linq;
using ProductManagementSystem.Model;
using ProductManagementSystem.Shared;

namespace ProductManagementSystem.Logic
{
    /// <summary>
    /// MVP Pattern - Model Implementation.
    /// Implements IProductModel interface and wraps existing ProductLogic.
    /// Raises events when data changes to notify the Presenter.
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
        /// Initializes a new instance of ProductModelMvp with the specified ProductLogic.
        /// </summary>
        /// <param name="logic">The ProductLogic instance to wrap</param>
        public ProductModelMvp(ProductLogic logic)
        {
            _logic = logic ?? throw new ArgumentNullException(nameof(logic));
        }

        #endregion

        #region Event Raising Methods

        /// <summary>
        /// Raises the ProductsChanged event.
        /// </summary>
        protected virtual void OnProductsChanged()
        {
            ProductsChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Raises the ErrorOccurred event.
        /// </summary>
        /// <param name="errorMessage">Error message to include</param>
        protected virtual void OnErrorOccurred(string errorMessage)
        {
            ErrorOccurred?.Invoke(this, errorMessage);
        }

        #endregion

        #region CRUD Operations

        /// <inheritdoc/>
        public List<Product> GetAllProducts()
        {
            try
            {
                return _logic.GetAll();
            }
            catch (Exception ex)
            {
                OnErrorOccurred($"Error getting products: {ex.Message}");
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
                OnErrorOccurred($"Error getting product by ID: {ex.Message}");
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
                OnErrorOccurred($"Error adding product: {ex.Message}");
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
                OnErrorOccurred($"Error updating product: {ex.Message}");
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
                OnErrorOccurred($"Error deleting product: {ex.Message}");
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
                OnErrorOccurred($"Error filtering by category: {ex.Message}");
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
                OnErrorOccurred($"Error grouping by category: {ex.Message}");
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
                OnErrorOccurred($"Error calculating total inventory value: {ex.Message}");
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
                OnErrorOccurred($"Error finding product: {ex.Message}");
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
                OnErrorOccurred($"Error searching products: {ex.Message}");
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
                OnErrorOccurred($"Error adding quantity to product: {ex.Message}");
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
                OnErrorOccurred($"Error removing quantity from product: {ex.Message}");
                return false;
            }
        }

        /// <inheritdoc/>
        public bool ProductExists(int id)
        {
            try
            {
#pragma warning disable CS0618 // Suppress obsolete warning for IdExists
                return _logic.IdExists(id);
#pragma warning restore CS0618
            }
            catch (Exception ex)
            {
                OnErrorOccurred($"Error checking product existence: {ex.Message}");
                return false;
            }
        }

        /// <inheritdoc/>
        public List<(int Index, Product Product)> GetProductsWithIndexes()
        {
            try
            {
#pragma warning disable CS0618 // Suppress obsolete warning for GetProductsWithIndexes
                return _logic.GetProductsWithIndexes();
#pragma warning restore CS0618
            }
            catch (Exception ex)
            {
                OnErrorOccurred($"Error getting products with indexes: {ex.Message}");
                return new List<(int, Product)>();
            }
        }

        #endregion
    }
}
