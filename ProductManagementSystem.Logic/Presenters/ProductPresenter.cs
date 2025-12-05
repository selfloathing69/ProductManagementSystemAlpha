using System;
using ProductManagementSystem.Model;
using ProductManagementSystem.Shared;

namespace ProductManagementSystem.Logic.Presenters
{
    /// <summary>
    /// MVP паттерн - главный презентер.
    /// Соединяет представление (IProductView) и модель (IProductModel).
    /// Обрабатывает действия пользователя из представления и обновляет представление на основе изменений модели.
    /// 
    /// SOLID - D: Зависит от абстракций (IProductView, IProductModel), а не от конкретных реализаций.
    /// </summary>
    public class ProductPresenter : IDisposable
    {
        private readonly IProductView _view;
        private readonly IProductModel _model;
        private bool _disposed;

        /// <summary>
        /// Инициализирует новый экземпляр ProductPresenter с указанным представлением и моделью.
        /// </summary>
        /// <param name="view">The View interface for UI interactions</param>
        /// <param name="model">The Model interface for business logic</param>
        public ProductPresenter(IProductView view, IProductModel model)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _model = model ?? throw new ArgumentNullException(nameof(model));

            // Подписка на события представления
            SubscribeToViewEvents();

            // Подписка на события модели
            SubscribeToModelEvents();

            // Начальная загрузка данных
            LoadProducts();
        }

        #region View Event Subscriptions

        private void SubscribeToViewEvents()
        {
            _view.RefreshRequested += OnRefreshRequested;
            _view.AddProductRequested += OnAddProductRequested;
            _view.DeleteProductRequested += OnDeleteProductRequested;
            _view.DeleteProductByQuantityRequested += OnDeleteProductByQuantityRequested;
        }

        private void UnsubscribeFromViewEvents()
        {
            _view.RefreshRequested -= OnRefreshRequested;
            _view.AddProductRequested -= OnAddProductRequested;
            _view.DeleteProductRequested -= OnDeleteProductRequested;
            _view.DeleteProductByQuantityRequested -= OnDeleteProductByQuantityRequested;
        }

        #endregion

        #region Model Event Subscriptions

        private void SubscribeToModelEvents()
        {
            _model.ProductsChanged += OnProductsChanged;
            _model.ErrorOccurred += OnModelErrorOccurred;
        }

        private void UnsubscribeFromModelEvents()
        {
            _model.ProductsChanged -= OnProductsChanged;
            _model.ErrorOccurred -= OnModelErrorOccurred;
        }

        #endregion

        #region View Event Handlers

        private void OnRefreshRequested(object? sender, EventArgs e)
        {
            LoadProducts();
            _view.ShowMessage("Обновление", "Данные успешно обновлены.");
        }

        private void OnAddProductRequested(object? sender, EventArgs e)
        {
            // Представление будет обрабатывать показ диалога формы добавления.
            // Когда форма добавления закрыта, она должна вызвать обратный вызов для добавления товара.
            // Это событие в основном для уведомления презентера о том, что был запрошен запрос на добавление.
        }

        private void OnDeleteProductRequested(object? sender, int productId)
        {
            try
            {
                var product = _model.GetProductById(productId);
                if (product == null)
                {
                    _view.ShowError("Ошибка", "Товар не найден.");
                    return;
                }

                if (_view.ShowConfirmation("Подтверждение удаления", 
                    $"Вы уверены, что хотите удалить товар с ID {productId}?"))
                {
                    if (_model.DeleteProduct(productId))
                    {
                        _view.ShowMessage("Успех", "Товар успешно удалён.");
                    }
                    else
                    {
                        _view.ShowError("Ошибка", "Не удалось удалить товар.");
                    }
                }
            }
            catch (Exception ex)
            {
                _view.ShowError("Ошибка", $"Ошибка при удалении товара: {ex.Message}");
            }
        }

        private void OnDeleteProductByQuantityRequested(object? sender, (int ProductId, int Quantity) args)
        {
            try
            {
                var product = _model.GetProductById(args.ProductId);
                if (product == null)
                {
                    _view.ShowError("Ошибка", "Товар не найден.");
                    return;
                }

                int quantityToRemove = args.Quantity;
                if (quantityToRemove == 0)
                {
                    quantityToRemove = product.StockQuantity;
                }

                if (_view.ShowConfirmation("Подтверждение удаления",
                    $"Вы хотите удалить {product.Name} в количестве {quantityToRemove}, вы уверены?"))
                {
                    if (_model.RemoveQuantityFromProduct(args.ProductId, quantityToRemove))
                    {
                        _view.ShowMessage("Успех", "Товар удален в указанном количестве.");
                    }
                    else
                    {
                        _view.ShowError("Ошибка", "Не удалось удалить товар.");
                    }
                }
            }
            catch (Exception ex)
            {
                _view.ShowError("Ошибка", $"Ошибка при удалении товара: {ex.Message}");
            }
        }

        #endregion

        #region Model Event Handlers

        private void OnProductsChanged(object? sender, EventArgs e)
        {
            LoadProducts();
        }

        private void OnModelErrorOccurred(object? sender, string errorMessage)
        {
            _view.ShowError("Ошибка модели", errorMessage);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Загружает все товары из модели и отображает их в представлении.
        /// </summary>
        public void LoadProducts()
        {
            try
            {
                var products = _model.GetAllProducts();
                _view.ShowProducts(products);
            }
            catch (Exception ex)
            {
                _view.ShowError("Ошибка", $"Ошибка при загрузке товаров: {ex.Message}");
            }
        }

        /// <summary>
        /// Добавляет товар в модель.
        /// </summary>
        /// <param name="product">Product to add</param>
        /// <returns>True if product was added successfully</returns>
        public bool AddProduct(Product product)
        {
            try
            {
                _model.AddProduct(product);
                return true;
            }
            catch (Exception ex)
            {
                _view.ShowError("Ошибка", $"Не удалось добавить товар: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Получает товар по ID.
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>Product or null</returns>
        public Product? GetProduct(int id)
        {
            return _model.GetProductById(id);
        }

        /// <summary>
        /// Проверяет, существует ли товар с указанным ID.
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>True if product exists</returns>
        public bool ProductExists(int id)
        {
            return _model.ProductExists(id);
        }

        /// <summary>
        /// Находит товар по названию и категории.
        /// </summary>
        /// <param name="name">Product name</param>
        /// <param name="category">Product category</param>
        /// <returns>Product or null</returns>
        public Product? FindProductByNameAndCategory(string name, string category)
        {
            return _model.FindProductByNameAndCategory(name, category);
        }

        /// <summary>
        /// Добавляет количество к существующему товару.
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="quantity">Quantity to add</param>
        /// <returns>True if successful</returns>
        public bool AddQuantityToProduct(int id, int quantity)
        {
            return _model.AddQuantityToProduct(id, quantity);
        }

        /// <summary>
        /// Получает товары с их индексами отображения.
        /// </summary>
        /// <returns>List of tuples with index and product</returns>
        public List<(int Index, Product Product)> GetProductsWithIndexes()
        {
            return _model.GetProductsWithIndexes();
        }

        /// <summary>
        /// Удаляет количество из товара.
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="quantity">Quantity to remove</param>
        /// <returns>True if successful</returns>
        public bool RemoveQuantityFromProduct(int id, int quantity)
        {
            return _model.RemoveQuantityFromProduct(id, quantity);
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// Освобождает ресурсы презентера и отменяет подписки на события.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Освобождает управляемые ресурсы.
        /// </summary>
        /// <param name="disposing">True if disposing managed resources</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    UnsubscribeFromViewEvents();
                    UnsubscribeFromModelEvents();
                }
                _disposed = true;
            }
        }

        #endregion
    }
}
