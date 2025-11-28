using System;
using ProductManagementSystem.Model;
using ProductManagementSystem.Shared;

namespace ProductManagementSystem.Logic.Presenters
{
    /// <summary>
    /// MVP Pattern - Add Product Dialog Presenter.
    /// Connects the AddProduct View (IAddProductView) and Model (IProductModel).
    /// Handles validation and product addition logic.
    /// 
    /// SOLID - D: Depends on abstractions (IAddProductView, IProductModel), not concrete implementations.
    /// </summary>
    public class AddProductPresenter : IDisposable
    {
        private readonly IAddProductView _view;
        private readonly IProductModel _model;
        private bool _disposed;

        /// <summary>
        /// Gets the created product after successful save.
        /// </summary>
        public Product? CreatedProduct { get; private set; }

        /// <summary>
        /// Initializes a new instance of AddProductPresenter with the specified View and Model.
        /// </summary>
        /// <param name="view">The View interface for UI interactions</param>
        /// <param name="model">The Model interface for business logic</param>
        public AddProductPresenter(IAddProductView view, IProductModel model)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _model = model ?? throw new ArgumentNullException(nameof(model));

            // Subscribe to View events
            SubscribeToViewEvents();
        }

        #region View Event Subscriptions

        private void SubscribeToViewEvents()
        {
            _view.SaveRequested += OnSaveRequested;
            _view.CancelRequested += OnCancelRequested;
        }

        private void UnsubscribeFromViewEvents()
        {
            _view.SaveRequested -= OnSaveRequested;
            _view.CancelRequested -= OnCancelRequested;
        }

        #endregion

        #region View Event Handlers

        private void OnSaveRequested(object? sender, EventArgs e)
        {
            try
            {
                // Validate form data
                if (string.IsNullOrWhiteSpace(_view.ProductName))
                {
                    _view.ShowError("Ошибка", "Поле 'Название' обязательно для заполнения!");
                    _view.FocusField("Name");
                    return;
                }

                if (string.IsNullOrWhiteSpace(_view.ProductCategory))
                {
                    _view.ShowError("Ошибка", "Поле 'Категория' обязательно для заполнения!");
                    _view.FocusField("Category");
                    return;
                }

                int id = _view.ProductId;

                // Check if ID already exists
                if (_model.ProductExists(id))
                {
                    if (_view.ShowConfirmation("ID уже существует",
                        $"ID {id} уже существует. Хотите использовать другой свободный ID?"))
                    {
                        _view.FocusField("Id");
                        return;
                    }
                    else
                    {
                        // Check for duplicate product by name and category
                        string name = _view.ProductName.Trim();
                        string category = _view.ProductCategory.Trim();

                        var existingProduct = _model.FindProductByNameAndCategory(name, category);

                        if (existingProduct != null &&
                            !category.Equals("Другое", StringComparison.OrdinalIgnoreCase))
                        {
                            if (_view.ShowConfirmation("Товар уже существует",
                                $"Такой товар '{name}' уже существует с количеством: {existingProduct.StockQuantity}.\n" +
                                $"Желаете просуммировать значения двух количеств в уже имеющемся?"))
                            {
                                if (_model.AddQuantityToProduct(existingProduct.Id, _view.StockQuantity))
                                {
                                    CreatedProduct = _model.GetProductById(existingProduct.Id);
                                    _view.ShowMessage("Успех",
                                        $"Количество товара обновлено. Новое количество: {CreatedProduct?.StockQuantity ?? 0}");
                                    _view.CloseWithSuccess();
                                }
                                return;
                            }
                        }

                        _view.ShowMessage("Отмена", "Добавление отменено.");
                        return;
                    }
                }

                // Create and add the new product
                var product = new Product
                {
                    Id = id,
                    Name = _view.ProductName.Trim(),
                    Description = _view.ProductDescription.Trim(),
                    Price = _view.ProductPrice,
                    Category = _view.ProductCategory.Trim(),
                    StockQuantity = _view.StockQuantity
                };

                CreatedProduct = _model.AddProduct(product);
                _view.ShowMessage("Успех", "Товар успешно добавлен.");
                _view.CloseWithSuccess();
            }
            catch (Exception ex)
            {
                _view.ShowError("Ошибка", $"Не удалось добавить товар: {ex.Message}");
            }
        }

        private void OnCancelRequested(object? sender, EventArgs e)
        {
            _view.CloseWithCancel();
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// Disposes the presenter and unsubscribes from events.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes managed resources.
        /// </summary>
        /// <param name="disposing">True if disposing managed resources</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    UnsubscribeFromViewEvents();
                }
                _disposed = true;
            }
        }

        #endregion
    }
}
