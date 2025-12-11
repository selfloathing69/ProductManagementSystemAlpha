using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ProductManagementSystem.Logic;
using ProductManagementSystem.Model;
using ProductManagementSystem.Shared;
using ProductManagementSystem.WpfApp.Commands;
using ProductManagementSystem.WpfApp.Services;
using ProductManagementSystem.WpfApp.Views;

namespace ProductManagementSystem.WpfApp.ViewModels
{
    /// <summary>
    /// MVVM Pattern - Главная ViewModel для управления товарами.
    /// 
    /// SOLID - S: Класс отвечает только за представление главного окна.
    /// SOLID - D: Зависит от абстракций ILogic, а не от конкретной реализации.
    /// 
    /// MainViewModel обеспечивает взаимодействие между View (MainWindow) и Model (ProductLogic).
    /// Содержит состояние данных, команды для UI элементов и бизнес-логику представления.
    /// </summary>
    public class MainViewModel : ViewModelBase // === наследует
    {

        private readonly ILogic _logic;
        private readonly ViewManager _viewManager;
        private ObservableCollection<ProductDto> _products; // ========================================
        private ObservableCollection<string> _categories;
        private ProductDto? _selectedProduct;
        private string _selectedCategory = "Все категории";
        private decimal _totalInventoryValue;
        private decimal _selectedProductValue;
        private string _statusMessage = "Готово";

        // Поля для добавления/редактирования товара
        private int? _productId;
        private string _productName = string.Empty;
        private string _productDescription = string.Empty;
        private decimal? _productPrice;
        private string _productCategory = string.Empty;
        private int? _productStockQuantity;

        // Поля для хранения оригинальных значений (для сравнения при Update)
        private ProductDto? _originalProduct;

        /// <summary>
        /// Коллекция товаров для отображения
        /// ObservableCollection автоматически обновляет UI при изменениях.
        /// </summary>
        public ObservableCollection<ProductDto> Products
        {
            get => _products;
            set => SetProperty(ref _products, value);
        }

        /// <summary>
        /// Коллекция доступных категорий для фильтрации.
        /// </summary>
        public ObservableCollection<string> Categories
        {
            get => _categories;
            set => SetProperty(ref _categories, value);
        }

        /// <summary>
        /// Выбранный товарб
        /// </summary>
        public ProductDto? SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                if (SetProperty(ref _selectedProduct, value))
                {
                    if (value != null)
                    {
                        // При выборе товара загружаем его для редактирования
                        LoadProductForEdit(value);
                        // Автоматически рассчитываем стоимость выбранного товара
                        CalculateSelectedProductValue();
                    }
                    else
                    {
                        SelectedProductValue = 0;
                    }
                }
            }
        }

        /// <summary>
        /// Выбранная категория для фильтрации.
        /// </summary>
        public string SelectedCategory
        {
            get => _selectedCategory;
            set => SetProperty(ref _selectedCategory, value);
        }

        /// <summary>
        /// Общая стоимость всех товаров на складе.
        /// </summary>
        public decimal TotalInventoryValue
        {
            get => _totalInventoryValue;
            set => SetProperty(ref _totalInventoryValue, value);
        }

        /// <summary>
        /// Стоимость выбранного товара (Price * StockQuantity).
        /// </summary>
        public decimal SelectedProductValue
        {
            get => _selectedProductValue;
            set => SetProperty(ref _selectedProductValue, value);
        }

        /// <summary>
        /// Сообщение статуса для отображения в строке состояния.
        /// </summary>
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }


        public int? ProductId
        {
            get => _productId;
            set => SetProperty(ref _productId, value);
        }

        public string ProductName
        {
            get => _productName;
            set => SetProperty(ref _productName, value);
        }

        public string ProductDescription
        {
            get => _productDescription;
            set => SetProperty(ref _productDescription, value);
        }

        public decimal? ProductPrice
        {
            get => _productPrice;
            set => SetProperty(ref _productPrice, value);
        }

        public string ProductCategory
        {
            get => _productCategory;
            set => SetProperty(ref _productCategory, value);
        }

        public int? ProductStockQuantity
        {
            get => _productStockQuantity;
            set => SetProperty(ref _productStockQuantity, value);
        }

        /// <summary>
        /// Команды главного окна =======================
        /// </summary>
        public ICommand RefreshCommand { get; }

        /// <summary>
        /// Команда добавления нового товара.
        /// </summary>
        public ICommand AddProductCommand { get; }

        /// <summary>
        /// Команда удаления выбранного товара.
        /// </summary>
        public ICommand DeleteProductCommand { get; }

        /// <summary>
        /// Команда применения фильтра по категории.
        /// </summary>
        public ICommand ApplyFilterCommand { get; }

        /// <summary>
        /// Команда расчета общей стоимости товаров.
        /// </summary>
        public ICommand CalculateTotalCommand { get; }

        /// <summary>
        /// Команда очистки формы добавления товара.
        /// </summary>
        public ICommand ClearFormCommand { get; }

        /// <summary>
        /// Команда сохранения изменений товара
        /// </summary>
        public ICommand UpdateProductCommand { get; }

        /// <summary>
        /// Команда переключения темы приложения
        /// </summary>
        public ICommand ToggleThemeCommand { get; }



        /// <summary>
        /// Инициализирует новый экземпляр MainViewModel.
        /// SOLID - D: Dependency Injection - получаем ILogic через конструктор.
        /// </summary>
        /// <param name="logic">Сервис бизнес-логики</param>
        /// <param name="viewManager">Менеджер для управления окнами</param>
        public MainViewModel(ILogic logic, ViewManager viewManager)
        {
            _logic = logic ?? throw new ArgumentNullException(nameof(logic));
            _viewManager = viewManager ?? throw new ArgumentNullException(nameof(viewManager));
            _products = new ObservableCollection<ProductDto>();
            _categories = new ObservableCollection<string>();

            // Инициализация команд
            RefreshCommand = new RelayCommand(_ => RefreshProducts());
            AddProductCommand = new RelayCommand(_ => AddProduct(), _ => CanAddProduct());
            DeleteProductCommand = new RelayCommand(_ => DeleteProduct(), _ => CanDeleteProduct());
            ApplyFilterCommand = new RelayCommand(_ => ApplyFilter());
            CalculateTotalCommand = new RelayCommand(_ => CalculateTotal());
            ClearFormCommand = new RelayCommand(_ => ClearForm());
            UpdateProductCommand = new RelayCommand(_ => UpdateProduct(), _ => CanUpdateProduct());
            ToggleThemeCommand = new RelayCommand(_ => ToggleTheme());

            // Загрузка начальных данных
            LoadInitialData();
        }



        /// <summary>
        /// Загружает начальные данные при запуске приложения.
        /// </summary>
        private void LoadInitialData()
        {
            RefreshProducts();
            LoadCategories();
            CalculateTotal();
            StatusMessage = "Данные загружены";
        }

        /// <summary>
        /// Обновляет список товаров из базы.
        /// </summary>
        private void RefreshProducts()
        {
            try
            {
                var products = _logic.GetAll();
                Products.Clear();
                
                foreach (var product in products)
                {
                    Products.Add(MapToDto(product));
                }

                LoadCategories();
                CalculateTotal();
                StatusMessage = $"Обновлено товаров: {Products.Count}";
            }
            catch (Exception ex)
            {
                ShowError("Ошибка обновления", $"Не удалось обновить данные: {ex.Message}");
            }
        }

        /// <summary>
        /// Загружает список категорий для фильтра.
        /// </summary>
        private void LoadCategories()
        {
            Categories.Clear();
            Categories.Add("Все категории");

            var products = _logic.GetAll();
            var categories = products
                .Select(p => p.Category)
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Distinct()
                .OrderBy(c => c);

            foreach (var category in categories)
            {
                Categories.Add(category);
            }
        }

        /// <summary>
        /// Добавляет новый товар в систему.
        /// </summary>
        private void AddProduct()
        {
            try
            {
                // Валидация
                if (string.IsNullOrWhiteSpace(ProductName))
                {
                    ShowError("Ошибка валидации", "Название товара обязательно для заполнения!");
                    return;
                }

                if (string.IsNullOrWhiteSpace(ProductCategory))
                {
                    ShowError("Ошибка валидации", "Категория товара обязательна для заполнения!");
                    return;
                }

                if (!ProductPrice.HasValue || ProductPrice.Value <= 0)
                {
                    ShowError("Ошибка валидации", "Цена должна быть больше нуля!");
                    return;
                }

                // Создание товара
                var product = new Product
                {
                    Id = ProductId ?? 0,
                    Name = ProductName.Trim(),
                    Description = ProductDescription.Trim(),
                    Price = ProductPrice.Value,
                    Category = ProductCategory.Trim(),
                    StockQuantity = ProductStockQuantity ?? 0
                };

                // Проверка существования ID
                if (ProductId.HasValue && ProductId.Value > 0)
                {
                    var existing = _logic.GetById(ProductId.Value);
                    if (existing != null)
                    {
                        var result = MessageBox.Show(
                            $"ID {ProductId} уже существует. Хотите использовать другой ID?",
                            "ID уже существует",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Question);

                        if (result == MessageBoxResult.Yes)
                        {
                            return;
                        }
                    }
                }

                _logic.Add(product);
                ClearForm();
                RefreshProducts();
                StatusMessage = "Товар успешно добавлен";
            }
            catch (Exception ex)
            {
                ShowError("Ошибка", $"Не удалось добавить товар: {ex.Message}");
            }
        }

        /// <summary>
        /// Проверяет возможность добавления товара.
        /// </summary>
        private bool CanAddProduct()
        {
            return !string.IsNullOrWhiteSpace(ProductName) &&
                   !string.IsNullOrWhiteSpace(ProductCategory) &&
                   ProductPrice.HasValue &&
                   ProductPrice.Value > 0;
        }

        /// <summary>
        /// Обновляет существующий товар
        /// </summary>
        /// 
        #region ne interesno - oshibki
        private void UpdateProduct()
        {
            try
            {
                if (!ProductId.HasValue || ProductId.Value <= 0)
                {
                    ShowError("Ошибка", "Не выбран товар для редактирования");
                    return;
                }

                // Валидация
                if (string.IsNullOrWhiteSpace(ProductName))
                {
                    ShowError("Ошибка валидации", "Название товара обязательно для заполнения!");
                    return;
                }

                if (string.IsNullOrWhiteSpace(ProductCategory))
                {
                    ShowError("Ошибка валидации", "Категория товара обязательна для заполнения!");
                    return;
                }

                if (!ProductPrice.HasValue || ProductPrice.Value <= 0)
                {
                    ShowError("Ошибка валидации", "Цена должна быть больше нуля!");
                    return;
                }

                // Проверяем, есть ли изменения
                if (_originalProduct == null)
                {
                    ShowError("Ошибка", "Не найдены оригинальные данные товара");
                    return;
                }
                #endregion
                // Формируем текст изменений
                var changesText = ConfirmationViewModel.BuildChangesText(
                    _originalProduct.Name, ProductName.Trim(),
                    _originalProduct.Description, ProductDescription.Trim(),
                    _originalProduct.Price, ProductPrice.Value,
                    _originalProduct.Category, ProductCategory.Trim(),
                    _originalProduct.StockQuantity, ProductStockQuantity ?? 0
                );

                if (changesText == "Нет изменений")
                {
                    ShowError("Информация", "Вы не внесли никаких изменений");
                    return;
                }

                // Создаем ViewModel для диалога подтверждения
                var confirmationViewModel = new ConfirmationViewModel
                {
                    ProductId = ProductId.Value,
                    ProductName = _originalProduct.Name,
                    ChangesText = changesText
                };

                // Создаем и показываем диалог
                var dialog = new ConfirmationDialog(confirmationViewModel);
                dialog.Owner = Application.Current.MainWindow;
                var result = dialog.ShowDialog();

                // Если пользователь подтвердил изменения
                if (result == true)
                {
                    var product = new Product
                    {
                        Id = ProductId.Value,
                        Name = ProductName.Trim(),
                        Description = ProductDescription.Trim(),
                        Price = ProductPrice.Value,
                        Category = ProductCategory.Trim(),
                        StockQuantity = ProductStockQuantity ?? 0
                    };

                    if (_logic.Update(product))
                    {
                        RefreshProducts();
                        ClearForm();
                        StatusMessage = "Товар успешно обновлен";
                    }
                    else
                    {
                        ShowError("Ошибка", "Не удалось обновить товар");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError("Ошибка", $"Не удалось обновить товар: {ex.Message}");
            }
        }

        /// <summary>
        /// Проверяет возможность обновления товара
        /// </summary>
        private bool CanUpdateProduct()
        {
            return ProductId.HasValue && 
                   ProductId.Value > 0 &&
                   !string.IsNullOrWhiteSpace(ProductName) &&
                   !string.IsNullOrWhiteSpace(ProductCategory) &&
                   ProductPrice.HasValue &&
                   ProductPrice.Value > 0;
        }

        /// <summary>
        /// Удаляет выбранный товар.
        /// </summary>
        private void DeleteProduct()
        {
            if (SelectedProduct == null)
            {
                ShowError("Предупреждение", "Выберите товар для удаления");
                return;
            }

            var result = MessageBox.Show(
                $"Вы уверены, что хотите удалить товар '{SelectedProduct.Name}'?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _logic.Delete(SelectedProduct.Id);
                    RefreshProducts();
                    ClearForm();
                    StatusMessage = "Товар успешно удален";
                }
                catch (Exception ex)
                {
                    ShowError("Ошибка", $"Не удалось удалить товар: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Проверяет возможность удаления товара.
        /// </summary>
        private bool CanDeleteProduct()
        {
            return SelectedProduct != null;
        }

        /// <summary>
        /// Применяет фильтр по выбранной категории.
        /// </summary>
        private void ApplyFilter()
        {
            try
            {
                if (SelectedCategory == "Все категории")
                {
                    RefreshProducts();
                    StatusMessage = "Отображены все товары";
                }
                else
                {
                    var filtered = _logic.FilterByCategory(SelectedCategory);
                    Products.Clear();

                    foreach (var product in filtered)
                    {
                        Products.Add(MapToDto(product));
                    }

                    StatusMessage = $"Товаров категории '{SelectedCategory}' ({Products.Count} шт.)";
                }
            }
            catch (Exception ex)
            {
                ShowError("Ошибка", $"Не удалось применить фильтр: {ex.Message}");
            }
        }

        /// <summary>
        /// Рассчитывает общую стоимость товаров на складе.
        /// </summary>
        private void CalculateTotal()
        {
            try
            {
                TotalInventoryValue = _logic.CalculateTotalInventoryValue();
                StatusMessage = $"Общая стоимость склада рассчитана";
            }
            catch (Exception ex)
            {
                ShowError("Ошибка", $"Не удалось рассчитать общую стоимость: {ex.Message}");
            }
        }

        /// <summary>
        /// Автоматически рассчитывает стоимость выбранного товара.
        /// </summary>
        private void CalculateSelectedProductValue()
        {
            if (SelectedProduct != null)
            {
                SelectedProductValue = SelectedProduct.Price * SelectedProduct.StockQuantity;
                StatusMessage = $"Стоимость товара ID {SelectedProduct.Id} рассчитана";
            }
        }

        /// <summary>
        /// Переключает тему приложения между светлой и темной
        /// </summary>
        private void ToggleTheme()
        {
            ThemeManager.Instance.ToggleTheme();
            var themeName = ThemeManager.Instance.IsDarkTheme ? "тёмная" : "светлая";
            StatusMessage = $"Тема изменена на {themeName}";
        }

        /// <summary>
        /// Очищает форму добавления товара.
        /// </summary>
        private void ClearForm()
        {
            ProductId = null;
            ProductName = string.Empty;
            ProductDescription = string.Empty;
            ProductPrice = null;
            ProductCategory = string.Empty;
            ProductStockQuantity = null;
            SelectedProduct = null;
            _originalProduct = null;
        }



        /// <summary>
        /// Загружает данные выбранного товара в форму для редактирования.
        /// </summary>
        private void LoadProductForEdit(ProductDto product)
        {
            // Сохраняем оригинальные данные для сравнения
            _originalProduct = new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Category = product.Category,
                StockQuantity = product.StockQuantity
            };

            ProductId = product.Id;
            ProductName = product.Name;
            ProductDescription = product.Description;
            ProductPrice = product.Price;
            ProductCategory = product.Category;
            ProductStockQuantity = product.StockQuantity;
        }

        /// <summary>
        /// Преобразует доменную сущность Product в DTO.
        /// </summary>
        private ProductDto MapToDto(Product product)
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
        /// Отображает сообщение об ошибке.
        /// </summary>
        private void ShowError(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
            StatusMessage = $"Ошибка: {message}";
        }
    }
}