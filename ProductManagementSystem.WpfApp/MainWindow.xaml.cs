using System.Windows;
using System.Linq;
using ProductManagementSystem.Logic;
using System.Collections.Generic;
using System.Windows.Controls;
using System;

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
        /// Для переключения между Entity Framework и Dapper, измените константу в RepositoryFactory.cs
        /// </summary>
        private ProductLogic _logic = new ProductLogic(RepositoryFactory.CreateRepository());

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
            // Валидация ID
            if (!TryGetIntFromComboBox(ComboBoxId, out var id) || id <= 0)
            {
                StatusText.Text = "Введите корректный положительный ID";
                return;
            }

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

            string name = TextBoxName.Text.Trim();
            string category = ComboBoxAddCategory.Text?.Trim() ?? "Разное";

            // Проверяем существование ID
            if (_logic.IdExists(id))
            {
                var result = MessageBox.Show(
                    $"ID {id} уже существует, хотите присвоить новому товару другой id?",
                    "ID уже существует",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    StatusText.Text = "Измените ID и попробуйте снова";
                    return;
                }
                else
                {
                    // Проверяем возможность суммирования количества
                    var existingProduct = _logic.FindProductByNameAndCategory(name, category);
                    if (existingProduct != null && !category.Equals("Разное", StringComparison.OrdinalIgnoreCase))
                    {
                        var sumResult = MessageBox.Show(
                            $"Такой товар {name} уже существует в количестве: {existingProduct.StockQuantity}, Желаете суммировать введеное вами количество с уже имеющимся?",
                            "Товар уже существует",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Question);

                        if (sumResult == MessageBoxResult.Yes)
                        {
                            _logic.AddQuantityToProduct(existingProduct.Id, quantity);
                            StatusText.Text = $"Количество товара увеличено. Новое количество: {existingProduct.StockQuantity}";
                            ClearAddFields();
                            Refresh();
                            return;
                        }
                    }

                    StatusText.Text = "Операция отменена";
                    return;
                }
            }

            // Создание нового товара
            var product = new ProductManagementSystem.Model.Product
            {
                Id = id,
                Name = name,
                Description = TextBoxDescription.Text?.Trim() ?? "",
                Category = category,
                Price = price,
                StockQuantity = quantity
            };

            _logic.AddProduct(product);
            StatusText.Text = "Товар успешно добавлен";

            ClearAddFields();
            Refresh();
        }

        /// <summary>
        /// Очищает поля добавления товара
        /// </summary>
        private void ClearAddFields()
        {
            ComboBoxId.SelectedIndex = -1;
            ComboBoxId.Text = "";
            TextBoxName.Clear();
            TextBoxDescription.Clear();
            ComboBoxAddCategory.SelectedIndex = -1;
            ComboBoxPrice.SelectedIndex = -1;
            ComboBoxQuantity.SelectedIndex = -1;
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
            var product = new ProductManagementSystem.Model.Product
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
            if (DataGridProducts.SelectedItem is ProductManagementSystem.Model.Product product)
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

        /// <summary>
        /// Обработчик нажатия кнопки "Удалить товар".
        /// Позволяет выбрать товар и количество для удаления.
        /// </summary>
        private void ButtonDeleteByQuantity_Click(object sender, RoutedEventArgs e)
        {
            var productsWithIndexes = _logic.GetProductsWithIndexes();
            if (productsWithIndexes.Count == 0)
            {
                MessageBox.Show("Товары не найдены.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Создаем окно для выбора товара и количества
            var selectWindow = new Window
            {
                Title = "Выберите товар для удаления",
                Width = 500,
                Height = 400,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this,
                ResizeMode = ResizeMode.NoResize
            };

            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // Список товаров
            var listBox = new ListBox { Margin = new Thickness(10) };
            Grid.SetRow(listBox, 0);

            foreach (var (index, product) in productsWithIndexes)
            {
                listBox.Items.Add($"{index}. {product.Name}, {product.StockQuantity} шт");
            }

            // Поле для ввода количества
            var quantityPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(10, 5, 10, 5) };
            quantityPanel.Children.Add(new TextBlock { Text = "Количество для удаления (0 - всё): ", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, 5, 0) });

            var quantityComboBox = new ComboBox { Width = 120, IsEditable = true };
            quantityComboBox.Items.Add("0");
            for (int i = 1; i <= 100; i++)
            {
                quantityComboBox.Items.Add(i.ToString());
            }
            quantityPanel.Children.Add(quantityComboBox);

            Grid.SetRow(quantityPanel, 1);

            // Кнопки
            var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(10) };
            
            var okButton = new Button { Content = "ОК", Width = 80, Height = 30, Margin = new Thickness(0, 0, 10, 0) };
            var cancelButton = new Button { Content = "Отмена", Width = 80, Height = 30 };

            buttonPanel.Children.Add(okButton);
            buttonPanel.Children.Add(cancelButton);
            Grid.SetRow(buttonPanel, 2);

            // Обновление максимального количества при выборе товара
            listBox.SelectionChanged += (s, args) =>
            {
                if (listBox.SelectedIndex >= 0)
                {
                    quantityComboBox.Text = "0";
                }
            };

            okButton.Click += (s, args) =>
            {
                if (listBox.SelectedIndex < 0)
                {
                    MessageBox.Show("Выберите товар для удаления.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(quantityComboBox.Text?.Trim(), out int quantityToRemove) || quantityToRemove < 0)
                {
                    MessageBox.Show("Введите корректное количество.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var selectedProduct = productsWithIndexes[listBox.SelectedIndex].Product;

                if (quantityToRemove == 0)
                    quantityToRemove = selectedProduct.StockQuantity;

                if (quantityToRemove > selectedProduct.StockQuantity)
                {
                    MessageBox.Show("Количество слишком большое.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var confirmResult = MessageBox.Show(
                    $"Вы хотите удалить {selectedProduct.Name} в количестве {quantityToRemove}, вы уверены?",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (confirmResult == MessageBoxResult.Yes)
                {
                    _logic.RemoveQuantityFromProduct(selectedProduct.Id, quantityToRemove);
                    Refresh();
                    StatusText.Text = "Товар удален в указанном количестве";
                }

                selectWindow.DialogResult = true;
            };

            cancelButton.Click += (s, args) =>
            {
                selectWindow.DialogResult = false;
            };

            grid.Children.Add(listBox);
            grid.Children.Add(quantityPanel);
            grid.Children.Add(buttonPanel);

            selectWindow.Content = grid;
            selectWindow.ShowDialog();
        }
    }
}
