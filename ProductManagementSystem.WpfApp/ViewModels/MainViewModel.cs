using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ProductManagementSystem.Logic;
using ProductManagementSystem.Model;
using ProductManagementSystem.Shared;
using ProductManagementSystem.WpfApp.Commands;

namespace ProductManagementSystem.WpfApp.ViewModels
{
    /// <summary>
    /// MVVM Pattern - Главная ViewModel для управления товарами.
    /// 
    /// SOLID - S: Класс отвечает только за логику представления главного окна.
    /// SOLID - D: Зависит от абстракции ILogic, а не от конкретной реализации.
    /// 
    /// MainViewModel координирует взаимодействие между View (MainWindow) и Model (ProductLogic).
    /// Содержит коллекции данных, команды для UI элементов и бизнес-логику представления.
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        #region Поля

        private readonly ILogic _logic;
        private ObservableCollection<ProductDto> _products;
        private ObservableCollection<string> _categories;
        private ProductDto? _selectedProduct;
        private string _selectedCategory = "Все категории";
        private decimal _totalInventoryValue;
        private string _statusMessage = "Готово";

        // Поля для добавления/редактирования товара
        private int _productId;
        private string _productName = string.Empty;
        private string _productDescription = string.Empty;
        private decimal _productPrice;
        private string _productCategory = string.Empty;
        private int _productStockQuantity;

        #endregion

        #region Свойства для привязки к UI

        /// <summary>
        /// Коллекция товаров для отображения в DataGrid.
        /// ObservableCollection автоматически уведомляет UI об изменениях.
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
        /// Выбранный товар в DataGrid.
        /// </summary>
        public ProductDto? SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                if (SetProperty(ref _selectedProduct, value) && value != null)
                {
                    // При выборе товара заполняем поля для редактирования
                    LoadProductForEdit(value);
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
        /// Сообщение статуса для отображения в строке состояния.
        /// </summary>
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        #region Свойства для формы добавления/редактирования

        public int ProductId
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

        public decimal ProductPrice
        {
            get => _productPrice;
            set => SetProperty(ref _productPrice, value);
        }

        public string ProductCategory
        {
            get => _productCategory;
            set => SetProperty(ref _productCategory, value);
        }

        public int ProductStockQuantity
        {
            get => _productStockQuantity;
            set => SetProperty(ref _productStockQuantity, value);
        }

        #endregion

        #endregion

        #region Команды

        /// <summary>
        /// Команда обновления списка товаров.
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

        #endregion

        #region Конструктор

        /// <summary>
        /// Инициализирует новый экземпляр MainViewModel.
        /// SOLID - D: Dependency Injection - получаем ILogic через конструктор.
        /// </summary>
        /// <param name="logic">Экземпляр бизнес-логики</param>
        public MainViewModel(ILogic logic)
        {
            _logic = logic ?? throw new ArgumentNullException(nameof(logic));
            _products = new ObservableCollection<ProductDto>();
            _categories = new ObservableCollection<string>();

            // Инициализация команд
            RefreshCommand = new RelayCommand(_ => RefreshProducts());
            AddProductCommand = new RelayCommand(_ => AddProduct(), _ => CanAddProduct());
            DeleteProductCommand = new RelayCommand(_ => DeleteProduct(), _ => CanDeleteProduct());
            ApplyFilterCommand = new RelayCommand(_ => ApplyFilter());
            CalculateTotalCommand = new RelayCommand(_ => CalculateTotal());
            ClearFormCommand = new RelayCommand(_ => ClearForm());

            // Загрузка начальных данных
            LoadInitialData();
        }

        #endregion

        #region Методы загрузки данных

        /// <summary>
        /// Загружает начальные данные при запуске приложения.
        /// </summary>
        private void LoadInitialData()
        {
            RefreshProducts();
            LoadCategories();
            StatusMessage = "Данные загружены";
        }

        /// <summary>
        /// Обновляет список товаров из модели.
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

                StatusMessage = $"Загружено товаров: {Products.Count}";
            }
            catch (Exception ex)
            {
                ShowError("Ошибка загрузки", $"Не удалось загрузить товары: {ex.Message}");
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

        #endregion

        #region Методы команд

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

                // Создание товара
                var product = new Product
                {
                    Id = ProductId,
                    Name = ProductName.Trim(),
                    Description = ProductDescription.Trim(),
                    Price = ProductPrice,
                    Category = ProductCategory.Trim(),
                    StockQuantity = ProductStockQuantity
                };

                // Проверка существования ID
                if (ProductId > 0)
                {
                    var existing = _logic.GetById(ProductId);
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
                LoadCategories();
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
                   ProductPrice > 0;
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
                    LoadCategories();
                    ClearForm();
                    StatusMessage = "Товар успешно удалён";
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
                    StatusMessage = "Показаны все товары";
                }
                else
                {
                    var filtered = _logic.FilterByCategory(SelectedCategory);
                    Products.Clear();

                    foreach (var product in filtered)
                    {
                        Products.Add(MapToDto(product));
                    }

                    StatusMessage = $"Показаны товары категории '{SelectedCategory}' ({Products.Count} шт.)";
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
                StatusMessage = $"Общая стоимость склада: {TotalInventoryValue:C}";
            }
            catch (Exception ex)
            {
                ShowError("Ошибка", $"Не удалось рассчитать общую стоимость: {ex.Message}");
            }
        }

        /// <summary>
        /// Очищает форму добавления товара.
        /// </summary>
        private void ClearForm()
        {
            ProductId = 0;
            ProductName = string.Empty;
            ProductDescription = string.Empty;
            ProductPrice = 0;
            ProductCategory = string.Empty;
            ProductStockQuantity = 0;
            SelectedProduct = null;
        }

        #endregion

        #region Вспомогательные методы

        /// <summary>
        /// Загружает данные выбранного товара в форму для редактирования.
        /// </summary>
        private void LoadProductForEdit(ProductDto product)
        {
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

        #endregion
    }
}
