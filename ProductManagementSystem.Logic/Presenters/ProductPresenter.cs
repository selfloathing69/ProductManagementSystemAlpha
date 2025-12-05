using System;
using ProductManagementSystem.Model;
using ProductManagementSystem.Shared;

namespace ProductManagementSystem.Logic.Presenters
{
    /// <summary>
    /// MVP Pattern - Main Presenter.
    /// Connects the View (IProductView) and Model (IProductModel).
    /// Handles user actions from the View and updates the View based on Model changes.
    /// Преобразует доменные объекты Product в DTO для View.
    /// 
    /// SOLID - D: Зависит от абстракций (IProductView, IProductModel), а не конкретных реализаций.
    /// </summary>
    public class ProductPresenter : IDisposable
    {
        private readonly IProductView _view;
        private readonly IProductModel _model;
        private bool _disposed;

        /// <summary>
        /// Инициализирует новый экземпляр ProductPresenter с указанными View и Model.
        /// </summary>
        public ProductPresenter(IProductView view, IProductModel model)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _model = model ?? throw new ArgumentNullException(nameof(model));

            // Подписаться на просмотр событий
            SubscribeToViewEvents();

            // Подпишитесь на события Model
            SubscribeToModelEvents();

            // загрузка начальных данных
            LoadProducts();
        }

        #region Product <-> ProductDto Mapping

        /// <summary>
        /// Преобразует объект домена Product в ProductDto для View.
        /// </summary>
        private static ProductDto ToDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Category = product.Category,
                StockQuantity = product.StockQuantity
            };
        }

        /// <summary>
        /// Преобразует ProductDto в объект домена Product для Model.
        /// </summary>
        private static Product ToProduct(ProductDto dto)
        {
            return new Product
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Category = dto.Category,
                StockQuantity = dto.StockQuantity
            };
        }

        /// <summary>
        /// Преобразовать список продуктов в список продуктов.
        /// </summary>
        private static IEnumerable<ProductDto> ToDtoList(IEnumerable<Product> products)
        {
            return products.Select(ToDto);
        }

        #endregion

        #region View Event Subscriptions

        private void SubscribeToViewEvents()
        {
            _view.RefreshRequested += OnRefreshRequested;
            _view.DeleteProductRequested += OnDeleteProductRequested;
            _view.DeleteProductByQuantityRequested += OnDeleteProductByQuantityRequested;
        }

        private void UnsubscribeFromViewEvents()
        {
            _view.RefreshRequested -= OnRefreshRequested;
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

        /// <summary>
        /// Создает AddProductPresenter для заданного представления.
        /// </summary>
        public AddProductPresenter CreateAddProductPresenter(IAddProductView addView)
        {
            return new AddProductPresenter(addView, _model);
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
        /// Загружает все продукты из модели и отображает их в представлении.
        /// </summary>
        public void LoadProducts()
        {
            try
            {
                var products = _model.GetAllProducts();
                _view.ShowProducts(ToDtoList(products));
            }
            catch (Exception ex)
            {
                _view.ShowError("Ошибка", $"Ошибка при загрузке товаров: {ex.Message}");
            }
        }

        /// <summary>
        /// Добавляет продукт в модель.
        /// </summary>

        public bool AddProduct(ProductDto productDto)
        {
            try
            {
                _model.AddProduct(ToProduct(productDto));
                return true;
            }
            catch (Exception ex)
            {
                _view.ShowError("Ошибка", $"Не удалось добавить товар: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Получает продукт по id
        /// </summary>

        public ProductDto? GetProduct(int id)
        {
            var product = _model.GetProductById(id);
            return product != null ? ToDto(product) : null;
        }

        /// <summary>
        /// Находит продукт по названию и категории.
        /// </summary>

        public bool ProductExists(int id)
        {
            return _model.ProductExists(id);
        }

        /// <summary>
        /// Находит продукт по названию и категории.       
        /// </summary>

        public ProductDto? FindProductByNameAndCategory(string name, string category)
        {
            var product = _model.FindProductByNameAndCategory(name, category);
            return product != null ? ToDto(product) : null;
        }

        /// <summary>
        /// Увеличивает количество существующего продукта.
        /// </summary>

        public bool AddQuantityToProduct(int id, int quantity)
        {
            return _model.AddQuantityToProduct(id, quantity);
        }

        /// <summary>
        /// Получает продукты с их индексами отображения в виде DTO.
        /// </summary>

        public List<(int Index, ProductDto Product)> GetProductsWithIndexes()
        {
            var products = _model.GetProductsWithIndexes();
            return products.Select(p => (p.Index, ToDto(p.Product))).ToList();
        }

        /// <summary>
        /// Удаляет количество из продукта.
        /// </summary>
        public bool RemoveQuantityFromProduct(int id, int quantity)
        {
            return _model.RemoveQuantityFromProduct(id, quantity);
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// Удаляет ведущего и отменяет подписку на события.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Распоряжается управляемыми ресурсами.
        /// </summary>

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
