using System.Windows;
using System.Linq;
using ProductManagementSystem.Logic;
using ProductManagementSystem.Model;
using System.Collections.Generic;
using System.Windows.Controls;

namespace ProductManagementSystem.WpfApp
{
    /// <summary>
    /// Главное окно WPF приложения для управления товарами.
    /// Предоставляет графический интерфейс для выполнения CRUD операций
    /// и дополнительных бизнес-функций.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Экземпляр класса с бизнес-логикой для работы с товарами.
        /// </summary>
        private ProductLogic _logic = new ProductLogic();

        /// <summary>
        /// Инициализирует новый экземпляр главного окна.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Refresh();
        }

        /// <summary>
        /// Обновляет данные в таблице и загружает категории в выпадающие списки.
        /// </summary>
        private void Refresh()
        {
            DataGridProducts.ItemsSource = null;
            DataGridProducts.ItemsSource = _logic.GetAllProducts();
            LoadCategories();
            StatusText.Text = "Данные обновлены";
        }

        /// <summary>
        /// Загружает список категорий в выпадающий список для фильтрации.
        /// </summary>
        private void LoadCategories()
        {
            ComboBoxCategory.Items.Clear();
            ComboBoxCategory.Items.Add("Все категории");
            
            var cats = new HashSet<string>(_logic.GetAllProducts()
                .Where(p => !string.IsNullOrWhiteSpace(p.Category))
                .Select(p => p.Category));
                
            foreach (var c in cats) 
                ComboBoxCategory.Items.Add(c);
                
            ComboBoxCategory.SelectedIndex = 0;
        }

        /// <summary>
        /// Получает числовое значение из ComboBox с валидацией.
        /// </summary>
        /// <param name="comboBox">ComboBox для проверки</param>
        /// <param name="value">Выходное значение</param>
        /// <returns>true, если значение корректно; false в противном случае</returns>
        private bool TryGetDecimalFromComboBox(ComboBox comboBox, out decimal value)
        {
            value = 0;
            string text = comboBox.Text?.Trim() ?? "";
            
            if (string.IsNullOrEmpty(text))
                return false;
                
            if (!decimal.TryParse(text, out value) || value < 0)
            {
                StatusText.Text = "Введите корректное положительное число";
                return false;
            }
            
            return true;
        }

        /// <summary>
        /// Получает целое числовое значение из ComboBox с валидацией.
        /// </summary>
        /// <param name="comboBox">ComboBox для проверки</param>
        /// <param name="value">Выходное значение</param>
        /// <returns>true, если значение корректно; false в противном случае</returns>
        private bool TryGetIntFromComboBox(ComboBox comboBox, out int value)
        {
            value = 0;
            string text = comboBox.Text?.Trim() ?? "";
            
            if (string.IsNullOrEmpty(text))
                return false;
                
            if (!int.TryParse(text, out value) || value < 0)
            {
                StatusText.Text = "Введите корректное неотрицательное целое число";
                return false;
            }
            
            return true;
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Обновить".
        /// </summary>
        private void ButtonRefresh_Click(object sender, RoutedEventArgs e) => Refresh();

        /// <summary>
        /// Обработчик нажатия кнопки "Добавить товар".
        /// Включает валидацию всех полей и использует ComboBox для числовых значений.
        /// </summary>
        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем заполненность обязательных полей
            if (string.IsNullOrWhiteSpace(TextBoxName.Text))
            {
                StatusText.Text = "Введите название товара";
                return;
            }

            // Валидация цены
            if (!TryGetDecimalFromComboBox(ComboBoxPrice, out var price))
                return;

            // Валидация количества
            if (!TryGetIntFromComboBox(ComboBoxQuantity, out var quantity))
                return;

            // Создание нового товара
            var product = new Product
            {
                Name = TextBoxName.Text.Trim(),
                Description = TextBoxDescription.Text?.Trim() ?? "",
                Category = ComboBoxAddCategory.Text?.Trim() ?? "Разное",
                Price = price,
                StockQuantity = quantity
            };

            _logic.AddProduct(product);
            StatusText.Text = "Товар успешно добавлен";

            // Очистка полей ввода
            TextBoxName.Clear();
            TextBoxDescription.Clear();
            ComboBoxAddCategory.SelectedIndex = -1;
            ComboBoxPrice.SelectedIndex = -1;
            ComboBoxQuantity.SelectedIndex = -1;

            Refresh();
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Найти".
        /// Ищет товар по ID с улучшенной валидацией.
        /// </summary>
        private void ButtonFind_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(TextBoxFindId.Text?.Trim(), out var id) || id <= 0)
            {
                StatusText.Text = "Введите корректный положительный ID";
                return;
            }

            var product = _logic.GetProduct(id);
            if (product == null)
            {
                StatusText.Text = "Товар не найден";
                return;
            }

            // Заполняем поля для редактирования
            TextBlockEditId.Text = "ID: " + product.Id;
            TextBoxEditName.Text = product.Name;
            TextBoxEditDescription.Text = product.Description;
            ComboBoxEditCategory.Text = product.Category;
            ComboBoxEditPrice.Text = product.Price.ToString();
            ComboBoxEditQuantity.Text = product.StockQuantity.ToString();

            StatusText.Text = "Товар загружен для редактирования";
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Сохранить изменения".
        /// Обновляет информацию о товаре с валидацией.
        /// </summary>
        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            var idText = TextBlockEditId.Text.Replace("ID: ", "").Trim();
            if (!int.TryParse(idText, out var id) || id <= 0)
            {
                StatusText.Text = "Товар не выбран для редактирования";
                return;
            }

            // Проверяем заполненность названия
            if (string.IsNullOrWhiteSpace(TextBoxEditName.Text))
            {
                StatusText.Text = "Название товара не может быть пустым";
                return;
            }

            // Валидация цены
            if (!TryGetDecimalFromComboBox(ComboBoxEditPrice, out var price))
                return;

            // Валидация количества
            if (!TryGetIntFromComboBox(ComboBoxEditQuantity, out var quantity))
                return;

            // Создание обновлённого товара
            var product = new Product
            {
                Id = id,
                Name = TextBoxEditName.Text.Trim(),
                Description = TextBoxEditDescription.Text?.Trim() ?? "",
                Category = ComboBoxEditCategory.Text?.Trim() ?? "Разное",
                Price = price,
                StockQuantity = quantity
            };

            if (_logic.UpdateProduct(product))
            {
                StatusText.Text = "Товар успешно сохранён";
                Refresh();
            }
            else
            {
                StatusText.Text = "Ошибка при сохранении товара";
            }
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Удалить".
        /// Удаляет выбранный товар с подтверждением.
        /// </summary>
        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridProducts.SelectedItem is Product product)
            {
                // Запрашиваем подтверждение удаления
                var result = MessageBox.Show(
                    $"Вы уверены, что хотите удалить товар '{product.Name}' (ID: {product.Id})?",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    if (_logic.DeleteProduct(product.Id))
                    {
                        StatusText.Text = "Товар успешно удалён";
                        Refresh();
                    }
                    else
                    {
                        StatusText.Text = "Ошибка при удалении товара";
                    }
                }
            }
            else
            {
                StatusText.Text = "Выберите строку в таблице для удаления";
            }
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Применить фильтр".
        /// Фильтрует товары по выбранной категории.
        /// </summary>
        private void ButtonFilter_Click(object sender, RoutedEventArgs e)
        {
            var selectedCategory = ComboBoxCategory.SelectedItem as string ?? "Все категории";
            
            if (selectedCategory == "Все категории")
            {
                DataGridProducts.ItemsSource = _logic.GetAllProducts();
                StatusText.Text = "Показаны все товары";
            }
            else
            {
                var filteredProducts = _logic.FilterByCategory(selectedCategory);
                DataGridProducts.ItemsSource = filteredProducts;
                StatusText.Text = $"Показаны товары категории '{selectedCategory}' ({filteredProducts.Count} шт.)";
            }
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Общая стоимость".
        /// Рассчитывает и отображает общую стоимость товаров на складе.
        /// </summary>
        private void ButtonTotal_Click(object sender, RoutedEventArgs e)
        {
            var totalValue = _logic.CalculateTotalInventoryValue();
            TextBlockTotal.Text = $"Общая стоимость: {totalValue:C}";
            StatusText.Text = "Общая стоимость склада рассчитана";
        }
    }
}
