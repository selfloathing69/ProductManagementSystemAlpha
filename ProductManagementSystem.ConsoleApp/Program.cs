using System;
using ProductManagementSystem.Logic;

namespace ProductManagementSystem.ConsoleApp
{
    /// <summary>
    /// Консольное приложение для управления товарами.
    /// Предоставляет пользовательский интерфейс командной строки для выполнения CRUD операций
    /// и дополнительных бизнес-функций над товарами.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Экземпляр класса с бизнес-логикой для работы с товарами.
        /// Для переключения между Entity Framework и Dapper, измените константу в RepositoryFactory.cs
        /// </summary>
        private static ProductLogic _productLogic = new ProductLogic(RepositoryFactory.CreateRepository());
        
        /// <summary>
        /// Главная точка входа в консольное приложение.
        /// Запускает основной цикл работы с пользователем.
        /// </summary>
        /// <param name="args">Аргументы командной строки</param>
        static void Main(string[] args)
        {
            bool exit = false;
            Console.WriteLine("===== Система управления товарами (Console) =====");
            
            // Основной цикл работы приложения
            while (!exit)
            {
                ShowMenu();  // Отображаем меню опций
                var choice = Console.ReadLine();
                
                // Обрабатываем выбор пользователя
                switch (choice)
                {
                    case "1": ShowAllProducts(); break;
                    case "2": ShowProductDetails(); break;
                    case "3": AddProduct(); break;
                    case "4": UpdateProduct(); break;
                    case "5": DeleteProduct(); break;
                    case "6": FilterByCategory(); break;
                    case "7": CalculateTotalInventoryValue(); break;
                    case "9": DeleteProductByQuantity(); break;
                    case "0": exit = true; break;
                    default: Console.WriteLine("Неверный выбор."); break;
                }
                
                // Ожидаем нажатия клавиши перед следующей итерацией (кроме выхода)
                if (!exit) 
                { 
                    Console.WriteLine("Нажмите любую клавишу..."); 
                    Console.ReadKey(); 
                    Console.Clear(); 
                }
            }
        }

        /// <summary>
        /// Отображает главное меню приложения с доступными опциями.
        /// </summary>
        static void ShowMenu()
        {
            Console.WriteLine("1. Показать все товары");
            Console.WriteLine("2. Показать детали товара");
            Console.WriteLine("3. Добавить новый товар");
            Console.WriteLine("4. Обновить товар");
            Console.WriteLine("5. Удалить весь товар с определённым ID");
            Console.WriteLine("6. Фильтр по категории");
            Console.WriteLine("7. Рассчитать общую стоимость на складе");
            Console.WriteLine("9. Удалить указанное количество товара");
            Console.WriteLine("0. Выход");
            Console.Write("Выбор: ");
        }

        /// <summary>
        /// Отображает список всех товаров в системе.
        /// </summary>
        static void ShowAllProducts()
        {
            var products = _productLogic.GetAllProducts();
            Console.WriteLine("===== Все товары =====");
            
            if (products.Count == 0)
            {
                Console.WriteLine("Товары не найдены.");
                return;
            }
            
            foreach (var p in products) 
                Console.WriteLine(p);
        }

        /// <summary>
        /// Показывает детальную информацию о товаре по его ID.
        /// Включает проверку корректности ввода идентификатора.
        /// </summary>
        static void ShowProductDetails()
        {
            Console.Write("Введите ID: ");
            string idInput = Console.ReadLine() ?? "";
            
            // Проверка корректности ввода ID (только числа)
            if (!int.TryParse(idInput, out int id) || id <= 0) 
            { 
                Console.WriteLine("Некорректный ID. Введите положительное число."); 
                return; 
            }
            
            var p = _productLogic.GetProduct(id);
            if (p == null) 
            { 
                Console.WriteLine("Товар не найден."); 
                return; 
            }
            
            Console.WriteLine("===== Детали товара =====");
            Console.WriteLine(p);
        }

        /// <summary>
        /// Добавляет новый товар в систему.
        /// Включает валидацию всех числовых полей и ручной ввод ID.
        /// </summary>
        static void AddProduct()
        {
            Console.WriteLine("===== Добавление нового товара =====");
            var prod = new ProductManagementSystem.Model.Product();
            
            // Ввод названия товара
            Console.Write("Название: "); 
            prod.Name = Console.ReadLine() ?? "";
            
            // Ввод описания товара
            Console.Write("Описание: "); 
            prod.Description = Console.ReadLine() ?? "";
            
            // Ввод цены с валидацией
            Console.Write("Цена: "); 
            string priceInput = Console.ReadLine() ?? "";
            if (!decimal.TryParse(priceInput, out var price) || price < 0) 
            { 
                Console.WriteLine("Некорректная цена. Введите положительное число."); 
                return; 
            } 
            prod.Price = price;
            
            // Ввод категории товара
            Console.Write("Категория: "); 
            prod.Category = Console.ReadLine() ?? "";
            
            // Ввод количества с валидацией
            Console.Write("Количество: "); 
            string quantityInput = Console.ReadLine() ?? "";
            if (!int.TryParse(quantityInput, out var q) || q < 0) 
            { 
                Console.WriteLine("Некорректное количество. Введите неотрицательное целое число."); 
                return; 
            } 
            prod.StockQuantity = q;
            
            // Ввод ID с проверкой на существование
            while (true)
            {
                Console.Write("ID: ");
                string idInput = Console.ReadLine() ?? "";
                if (!int.TryParse(idInput, out var id) || id <= 0)
                {
                    Console.WriteLine("Некорректный ID. Введите положительное число.");
                    continue;
                }
                
                if (_productLogic.IdExists(id))
                {
                    Console.Write($"ID {id} уже существует, хотите присвоить новому товару другой id? (да/нет): ");
                    string response = Console.ReadLine()?.ToLower().Trim() ?? "";
                    if (response == "да" || response == "yes" || response == "y")
                    {
                        continue; // Спрашиваем ID заново
                    }
                    else
                    {
                        // Пользователь не хочет менять ID, проверяем возможность суммирования
                        var existingProduct = _productLogic.FindProductByNameAndCategory(prod.Name, prod.Category);
                        if (existingProduct != null && !prod.Category.Equals("Разное", StringComparison.OrdinalIgnoreCase))
                        {
                            Console.Write($"Такой товар {prod.Name} уже существует в количестве: {existingProduct.StockQuantity}, Желаете суммировать введеное вами количество с уже имеющимся? (да/нет): ");
                            string sumResponse = Console.ReadLine()?.ToLower().Trim() ?? "";
                            if (sumResponse == "да" || sumResponse == "yes" || sumResponse == "y")
                            {
                                _productLogic.AddQuantityToProduct(existingProduct.Id, prod.StockQuantity);
                                Console.WriteLine($"Количество товара увеличено. Новое количество: {existingProduct.StockQuantity}");
                                return;
                            }
                        }
                        Console.WriteLine("Операция отменена.");
                        return;
                    }
                }
                else
                {
                    prod.Id = id;
                    break; // ID уникален, продолжаем
                }
            }
            
            _productLogic.AddProduct(prod);
            Console.WriteLine("Товар успешно добавлен.");
        }

        /// <summary>
        /// Обновляет информацию о существующем товаре.
        /// Позволяет изменить отдельные поля, сохраняя предыдущие значения при пустом вводе.
        /// </summary>
        static void UpdateProduct()
        {
            Console.Write("ID для обновления: ");
            string idInput = Console.ReadLine() ?? "";
            
            // Проверка корректности ввода ID
            if (!int.TryParse(idInput, out int id) || id <= 0) 
            { 
                Console.WriteLine("Некорректный ID. Введите положительное число."); 
                return; 
            }
            
            var existing = _productLogic.GetProduct(id);
            if (existing == null) 
            { 
                Console.WriteLine("Товар не найден."); 
                return; 
            }
            
            Console.WriteLine("===== Обновление товара =====");
            Console.WriteLine("Оставьте поле пустым, чтобы сохранить текущее значение.");
            
            // Обновление названия
            Console.Write($"Название [{existing.Name}]: "); 
            var name = Console.ReadLine(); 
            if (!string.IsNullOrWhiteSpace(name)) existing.Name = name;
            
            // Обновление описания
            Console.Write($"Описание [{existing.Description}]: "); 
            var desc = Console.ReadLine(); 
            if (!string.IsNullOrWhiteSpace(desc)) existing.Description = desc;
            
            // Обновление цены с валидацией
            Console.Write($"Цена [{existing.Price}]: "); 
            var priceS = Console.ReadLine(); 
            if (!string.IsNullOrWhiteSpace(priceS))
            {
                if (decimal.TryParse(priceS, out var pr) && pr >= 0) 
                    existing.Price = pr;
                else
                {
                    Console.WriteLine("Некорректная цена. Обновление отменено.");
                    return;
                }
            }
            
            // Обновление категории
            Console.Write($"Категория [{existing.Category}]: "); 
            var cat = Console.ReadLine(); 
            if (!string.IsNullOrWhiteSpace(cat)) existing.Category = cat;
            
            // Обновление количества с валидацией
            Console.Write($"Количество [{existing.StockQuantity}]: "); 
            var qS = Console.ReadLine(); 
            if (!string.IsNullOrWhiteSpace(qS))
            {
                if (int.TryParse(qS, out var q) && q >= 0) 
                    existing.StockQuantity = q;
                else
                {
                    Console.WriteLine("Некорректное количество. Обновление отменено.");
                    return;
                }
            }
            
            if (_productLogic.UpdateProduct(existing)) 
                Console.WriteLine("Товар успешно обновлён."); 
            else 
                Console.WriteLine("Ошибка при обновлении товара.");
        }

        /// <summary>
        /// Удаляет товар из системы с подтверждением операции.
        /// Включает валидацию ID и запрос подтверждения.
        /// </summary>
        static void DeleteProduct()
        {
            Console.Write("ID для удаления: ");
            string idInput = Console.ReadLine() ?? "";
            
            // Проверка корректности ввода ID
            if (!int.TryParse(idInput, out int id) || id <= 0) 
            { 
                Console.WriteLine("Некорректный ID. Введите положительное число."); 
                return; 
            }
            
            // Проверяем существование товара перед удалением
            var product = _productLogic.GetProduct(id);
            if (product == null)
            {
                Console.WriteLine("Товар с указанным ID не найден.");
                return;
            }
            
            Console.WriteLine($"Товар для удаления: {product}");
            Console.Write("Подтвердите удаление (да/нет): ");
            var confirmation = Console.ReadLine()?.ToLower().Trim();
            
            if (confirmation != "да" && confirmation != "yes" && confirmation != "y") 
            { 
                Console.WriteLine("Удаление отменено."); 
                return; 
            }
            
            if (_productLogic.DeleteProduct(id)) 
                Console.WriteLine("Товар успешно удалён."); 
            else 
                Console.WriteLine("Товар не найден.");
        }

        /// <summary>
        /// Фильтрует и отображает товары по указанной категории.
        /// </summary>
        static void FilterByCategory()
        {
            Console.Write("Категория: ");
            var cat = Console.ReadLine() ?? "";
            
            if (string.IsNullOrWhiteSpace(cat))
            {
                Console.WriteLine("Категория не может быть пустой.");
                return;
            }
            
            var list = _productLogic.FilterByCategory(cat);
            Console.WriteLine($"===== Товары категории '{cat}' =====");
            
            if (list.Count == 0)
            {
                Console.WriteLine("Товары данной категории не найдены.");
                return;
            }
            
            foreach (var p in list) 
                Console.WriteLine(p);
        }

        /// <summary>
        /// Рассчитывает и отображает общую стоимость всех товаров на складе.
        /// </summary>
        static void CalculateTotalInventoryValue()
        {
            var total = _productLogic.CalculateTotalInventoryValue();
            Console.WriteLine($"Общая стоимость товаров на складе: {total} руб");
        }

        /// <summary>
        /// Удаляет товар по количеству с возможностью выбора из списка.
        /// </summary>
        static void DeleteProductByQuantity()
        {
            var productsWithIndexes = _productLogic.GetProductsWithIndexes();
            if (productsWithIndexes.Count == 0)
            {
                Console.WriteLine("Товары не найдены.");
                return;
            }
            
            Console.WriteLine("===== Список товаров =====");
            foreach (var (index, product) in productsWithIndexes)
            {
                Console.WriteLine($"{index}. {product.Name}, {product.StockQuantity} шт");
            }
            
            // Выбор номера товара
            while (true)
            {
                Console.Write("Введите номер товара который вы хотите удалить: ");
                var input = Console.ReadLine() ?? "";
                
                if (!int.TryParse(input, out int selectedNumber) || selectedNumber < 1 || selectedNumber > productsWithIndexes.Count)
                {
                    Console.WriteLine("Номера нет, введите корректный номер товара.");
                    continue;
                }
                
                var selectedProduct = productsWithIndexes[selectedNumber - 1].Product;
                
                // Ввод количества для удаления
                while (true)
                {
                    Console.WriteLine($"Текущее количество: {selectedProduct.StockQuantity}");
                    Console.WriteLine("Введите количество для удаления или 0 для удаления всего (Enter - отмена):");
                    Console.Write("Ваше значение: ");
                    
                    var quantityInput = Console.ReadLine()?.Trim() ?? "";
                    
                    if (string.IsNullOrEmpty(quantityInput))
                    {
                        // Пустой ввод - отмена
                        return;
                    }
                    
                    if (quantityInput == "0")
                    {
                        // Удалить всё
                        Console.WriteLine($"Вы хотите удалить {selectedProduct.Name} в количестве {selectedProduct.StockQuantity}, вы уверены?");
                        Console.Write("1. Да 2. Нет: ");
                        
                        var confirmation = Console.ReadLine()?.Trim() ?? "";
                        
                        if (confirmation == "1")
                        {
                            _productLogic.DeleteProduct(selectedProduct.Id);
                            Console.WriteLine("Товар удален.");
                        }
                        return;
                    }
                    else if (int.TryParse(quantityInput, out int quantity) && quantity > 0)
                    {
                        if (quantity > selectedProduct.StockQuantity)
                        {
                            Console.WriteLine("Количество слишком большое, введите корректное значение.");
                            continue;
                        }
                        
                        Console.WriteLine($"Вы хотите удалить {selectedProduct.Name} в количестве {quantity}, вы уверены?");
                        Console.Write("1. Да 2. Нет: ");
                        
                        var confirmation = Console.ReadLine()?.Trim() ?? "";
                        
                        if (confirmation == "1")
                        {
                            _productLogic.RemoveQuantityFromProduct(selectedProduct.Id, quantity);
                            Console.WriteLine("Товар удален в указанном количестве.");
                        }
                        return;
                    }
                    else
                    {
                        Console.WriteLine("Введите корректное количество.");
                    }
                }
            }
        }
    }
}
