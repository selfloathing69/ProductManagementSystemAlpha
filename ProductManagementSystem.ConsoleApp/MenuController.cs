using System;
using ProductManagementSystem.Logic;
using ProductManagementSystem.Logic.Services;
using ProductManagementSystem.Logic.Exceptions;

namespace ProductManagementSystem.ConsoleApp
{
    /// <summary>
    /// SOLID - S: Класс отвечает только за управление меню консольного приложения.
    /// 
    /// Контроллер меню для консольного приложения управления товарами.
    /// Обрабатывает взаимодействие с пользователем и делегирует бизнес-логику сервису.
    /// </summary>
    public class MenuController
    {
        private readonly IProductService _productService;

        /// <summary>
        /// SOLID - D: Constructor Injection для внедрения зависимости от IProductService.
        /// </summary>
        /// <param name="productService">Сервис для работы с товарами</param>
        public MenuController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Запускает главный цикл работы приложения.
        /// </summary>
        public void Run()
        {
            bool exit = false;
            Console.WriteLine("═══ Система управления товарами в консольке :) ═══");

            while (!exit)
            {
                DisplayMenu();
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": HandleViewProducts(); break;
                    case "2": HandleShowProductDetails(); break;
                    case "3": HandleAddProduct(); break;
                    case "4": HandleUpdateProduct(); break;
                    case "5": HandleDeleteProduct(); break;
                    case "6": HandleFilterByCategory(); break;
                    case "7": HandleCalculateTotalValue(); break;
                    case "9": HandleDeleteByQuantity(); break;
                    case "0": exit = true; break;
                    default: Console.WriteLine("Неверный выбор."); break;
                }

                if (!exit)
                {
                    Console.WriteLine("\nНажмите любую клавишу...");
                    Console.ReadKey();
                    Console.Clear();
                }
            }
        }

        /// <summary>
        /// Отображает главное меню приложения.
        /// </summary>
        private void DisplayMenu()
        {
            Console.WriteLine("\n┌─────────────────────────────────────────┐");
            Console.WriteLine("│ 1. Показать все товары                  │");
            Console.WriteLine("│ 2. Показать детали товара               │");
            Console.WriteLine("│ 3. Добавить новый товар                 │");
            Console.WriteLine("│ 4. Обновить товар                       │");
            Console.WriteLine("│ 5. Удалить весь товар с определённым ID │");
            Console.WriteLine("│ 6. Фильтр по категории                  │");
            Console.WriteLine("│ 7. Рассчитать общую стоимость на складе │");
            Console.WriteLine("│ 9. Удалить указанное количество товара  │");
            Console.WriteLine("│ 0. Выход                                │");
            Console.WriteLine("└─────────────────────────────────────────┘");
            Console.Write("Выбор: ");
        }

        /// <summary>
        /// Отображает список всех товаров.
        /// </summary>
        private void HandleViewProducts()
        {
            try
            {
                var products = _productService.GetAllProducts();
                Console.WriteLine("\n═══ Все товары ═══");

                if (products.Count == 0)
                {
                    Console.WriteLine("Товары не найдены.");
                    return;
                }

                foreach (var p in products)
                    Console.WriteLine(p);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        /// <summary>
        /// Показывает детальную информацию о товаре по ID.
        /// </summary>
        private void HandleShowProductDetails()
        {
            try
            {
                Console.Write("Введите ID товара: ");
                if (!int.TryParse(Console.ReadLine(), out int id) || id <= 0)
                {
                    Console.WriteLine("Некорректный ID.");
                    return;
                }

                var product = _productService.GetProduct(id);
                if (product == null)
                {
                    Console.WriteLine("Товар не найден.");
                    return;
                }

                Console.WriteLine("\n═══ Информация о товаре ═══");
                Console.WriteLine($"ID: {product.Id}");
                Console.WriteLine($"Название: {product.Name}");
                Console.WriteLine($"Описание: {product.Description}");
                Console.WriteLine($"Цена: {product.Price} руб.");
                Console.WriteLine($"Категория: {product.Category}");
                Console.WriteLine($"Количество на складе: {product.StockQuantity}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        /// <summary>
        /// Обрабатывает добавление нового товара.
        /// </summary>
        private void HandleAddProduct()
        {
            try
            {
                Console.WriteLine("\n═══ Добавление нового товара ═══");

                var model = new ProductModel { Id = 0 };

                Console.Write("Название: ");
                model.Name = Console.ReadLine() ?? "";

                Console.Write("Описание: ");
                model.Description = Console.ReadLine() ?? "";

                Console.Write("Цена: ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal price))
                {
                    Console.WriteLine("Некорректная цена.");
                    return;
                }
                model.Price = price;

                Console.Write("Категория: ");
                model.Category = Console.ReadLine() ?? "";

                Console.Write("Количество: ");
                if (!int.TryParse(Console.ReadLine(), out int quantity))
                {
                    Console.WriteLine("Некорректное количество.");
                    return;
                }
                model.StockQuantity = quantity;

                var added = _productService.AddProduct(model);
                Console.WriteLine($"Товар успешно добавлен с ID: {added.Id}");
            }
            catch (ProductValidationException ex)
            {
                Console.WriteLine($"Ошибка валидации: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        /// <summary>
        /// Обрабатывает обновление существующего товара.
        /// </summary>
        private void HandleUpdateProduct()
        {
            try
            {
                Console.Write("ID для обновления: ");
                if (!int.TryParse(Console.ReadLine(), out int id) || id <= 0)
                {
                    Console.WriteLine("Некорректный ID.");
                    return;
                }

                var existing = _productService.GetProduct(id);
                if (existing == null)
                {
                    Console.WriteLine("Товар не найден.");
                    return;
                }

                Console.WriteLine("\n═══ Обновление товара ═══");
                Console.WriteLine("Оставьте поле пустым, чтобы сохранить текущее значение.");

                Console.Write($"Название [{existing.Name}]: ");
                var name = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(name)) existing.Name = name;

                Console.Write($"Описание [{existing.Description}]: ");
                var desc = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(desc)) existing.Description = desc;

                Console.Write($"Цена [{existing.Price}]: ");
                var priceS = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(priceS))
                {
                    if (decimal.TryParse(priceS, out var pr))
                        existing.Price = pr;
                    else
                    {
                        Console.WriteLine("Некорректная цена. Обновление отменено.");
                        return;
                    }
                }

                Console.Write($"Категория [{existing.Category}]: ");
                var cat = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(cat)) existing.Category = cat;

                Console.Write($"Количество [{existing.StockQuantity}]: ");
                var qS = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(qS))
                {
                    if (int.TryParse(qS, out var q))
                        existing.StockQuantity = q;
                    else
                    {
                        Console.WriteLine("Некорректное количество. Обновление отменено.");
                        return;
                    }
                }

                if (_productService.UpdateProduct(id, existing))
                    Console.WriteLine("Товар успешно обновлён.");
                else
                    Console.WriteLine("Ошибка при обновлении товара.");
            }
            catch (ProductValidationException ex)
            {
                Console.WriteLine($"Ошибка валидации: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        /// <summary>
        /// Обрабатывает удаление товара по ID.
        /// </summary>
        private void HandleDeleteProduct()
        {
            try
            {
                Console.Write("ID для удаления: ");
                if (!int.TryParse(Console.ReadLine(), out int id) || id <= 0)
                {
                    Console.WriteLine("Некорректный ID.");
                    return;
                }

                var product = _productService.GetProduct(id);
                if (product == null)
                {
                    Console.WriteLine("Товар не найден.");
                    return;
                }

                Console.WriteLine($"\nВы уверены, что хотите удалить товар '{product.Name}'? (y/n)");
                var confirm = Console.ReadLine()?.ToLower();

                if (confirm == "y" || confirm == "yes" || confirm == "д" || confirm == "да")
                {
                    if (_productService.DeleteProduct(id))
                        Console.WriteLine("Товар успешно удалён.");
                    else
                        Console.WriteLine("Ошибка при удалении товара.");
                }
                else
                {
                    Console.WriteLine("Удаление отменено.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        /// <summary>
        /// Обрабатывает фильтрацию товаров по категории.
        /// </summary>
        private void HandleFilterByCategory()
        {
            try
            {
                Console.Write("Введите категорию: ");
                var category = Console.ReadLine() ?? "";

                var products = _productService.FilterByCategory(category);

                Console.WriteLine($"\n═══ Товары в категории '{category}' ═══");
                if (products.Count == 0)
                {
                    Console.WriteLine("Товары не найдены.");
                    return;
                }

                foreach (var p in products)
                    Console.WriteLine(p);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        /// <summary>
        /// Вычисляет и отображает общую стоимость товаров на складе.
        /// </summary>
        private void HandleCalculateTotalValue()
        {
            try
            {
                var total = _productService.CalculateTotalInventoryValue();
                Console.WriteLine($"\n═══ Общая стоимость товаров на складе: {total:N2} руб. ═══");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        /// <summary>
        /// Обрабатывает удаление указанного количества товара.
        /// </summary>
        private void HandleDeleteByQuantity()
        {
            try
            {
                Console.Write("ID товара: ");
                if (!int.TryParse(Console.ReadLine(), out int id) || id <= 0)
                {
                    Console.WriteLine("Некорректный ID.");
                    return;
                }

                var product = _productService.GetProduct(id);
                if (product == null)
                {
                    Console.WriteLine("Товар не найден.");
                    return;
                }

                Console.WriteLine($"Текущее количество: {product.StockQuantity}");
                Console.Write("Количество для удаления (0 = удалить весь товар): ");
                if (!int.TryParse(Console.ReadLine(), out int quantity))
                {
                    Console.WriteLine("Некорректное количество.");
                    return;
                }

                var result = _productService.DeleteProductByQuantity(id, quantity);
                Console.WriteLine(result.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }
}
