using System;
using ProductManagementSystem.Model;
using ProductManagementSystem.Shared;

namespace ProductManagementSystem.Logic.Presenters
{
    /// <summary>
    /// Шаблон MVP — добавление презентатора диалогового окна продукта. 
    /// Соединяет представление AddProduct (IADProductView) и модель (IProductModel). 
    /// Обрабатывает логику проверки и добавления продуктов. 
    /// Вид не имеет зависимости от модели - работает только с примитивными типами. 
    /// 
    /// SOLID - D: Зависит от абстракций (IAAddProductView, IProductModel), а не от конкретных реализаций.
    /// </summary>
    public class AddProductPresenter : IDisposable
    {
        private readonly IAddProductView _view;
        private readonly IProductModel _model;
        private bool _disposed;

        /// <summary>
        /// Получает созданный DTO продукта после успешного сохранения.
        /// </summary>
        public ProductDto? CreatedProduct { get; private set; }

        /// <summary>
        /// Инициализирует новый экземпляр AddProductPresenter с указанными View и Model.
        /// </summary>

        public AddProductPresenter(IAddProductView view, IProductModel model)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _model = model ?? throw new ArgumentNullException(nameof(model));

            // =================================================================================
            SubscribeToViewEvents();
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

        #endregion

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
                // Валидируем вводимые данные
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

                // проверяем если ID уже существует
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
                        // проверяем если продукт с таким именем и категорией уже существует
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
                                    var updatedProduct = _model.GetProductById(existingProduct.Id);
                                    CreatedProduct = updatedProduct != null ? ToDto(updatedProduct) : null;
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

                // создаем новый продукт
                var product = new Product
                {
                    Id = id,
                    Name = _view.ProductName.Trim(),
                    Description = _view.ProductDescription.Trim(),
                    Price = _view.ProductPrice,
                    Category = _view.ProductCategory.Trim(),
                    StockQuantity = _view.StockQuantity
                };

                var addedProduct = _model.AddProduct(product);
                CreatedProduct = ToDto(addedProduct);
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
        /// Удаляет презентатор и отменяет подписку на события.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Удаляет управляемые ресурсы.
        /// </summary
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
