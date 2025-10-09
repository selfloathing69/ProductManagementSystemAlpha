using System;
using ProductManagementSystem.Logic;
using ProductManagementSystem.Model;
using ProductManagementSystem.DataAccessLayer.EF;
using ProductManagementSystem.DataAccessLayer.Dapper;

namespace ProductManagementSystem.ConsoleApp
{
    /// <summary>
    /// Демонстрация работы с Data Access Layer.
    /// Показывает использование Entity Framework и Dapper репозиториев.
    /// </summary>
    internal class DataAccessLayerDemo
    {
        /// <summary>
        /// Демонстрирует использование Entity Framework репозитория.
        /// </summary>
        public static void DemoEntityFramework()
        {
            Console.WriteLine("\n" + new string('=', 60));
            Console.WriteLine("ДЕМОНСТРАЦИЯ ENTITY FRAMEWORK РЕПОЗИТОРИЯ");
            Console.WriteLine(new string('=', 60));

            try
            {
                // Создаём репозиторий Entity Framework
                var repository = new EntityRepository<Product>();
                Console.WriteLine("✓ Entity Framework репозиторий создан");

                // Создаём бизнес-логику с репозиторием
                var logic = new ProductLogic(repository);
                Console.WriteLine("✓ Бизнес-логика инициализирована с EF репозиторием");

                // Добавляем тестовые товары
                Console.WriteLine("\n--- Добавление товаров ---");
                
                var product1 = new Product
                {
                    Name = "Ноутбук Dell XPS 15",
                    Description = "Мощный ноутбук для профессионалов",
                    Price = 120000,
                    Category = "Электроника",
                    StockQuantity = 5
                };
                logic.AddProduct(product1);
                Console.WriteLine($"+ {product1.Name} - {product1.Price:C}");

                var product2 = new Product
                {
                    Name = "Беспроводная мышь Logitech",
                    Description = "Эргономичная беспроводная мышь",
                    Price = 2500,
                    Category = "Периферия",
                    StockQuantity = 50
                };
                logic.AddProduct(product2);
                Console.WriteLine($"+ {product2.Name} - {product2.Price:C}");

                var product3 = new Product
                {
                    Name = "Механическая клавиатура",
                    Description = "RGB подсветка, Cherry MX switches",
                    Price = 8500,
                    Category = "Периферия",
                    StockQuantity = 20
                };
                logic.AddProduct(product3);
                Console.WriteLine($"+ {product3.Name} - {product3.Price:C}");

                // Получаем все товары
                Console.WriteLine("\n--- Список всех товаров ---");
                var allProducts = logic.GetAllProducts();
                Console.WriteLine($"Всего товаров: {allProducts.Count}");
                foreach (var p in allProducts)
                {
                    Console.WriteLine($"  ID: {p.Id}, {p.Name}, {p.Price:C}, Остаток: {p.StockQuantity} шт.");
                }

                // Фильтрация по категории
                Console.WriteLine("\n--- Фильтрация по категории 'Периферия' ---");
                var peripherals = logic.FilterByCategory("Периферия");
                Console.WriteLine($"Найдено товаров: {peripherals.Count}");
                foreach (var p in peripherals)
                {
                    Console.WriteLine($"  - {p.Name}");
                }

                // Расчёт общей стоимости
                Console.WriteLine("\n--- Общая стоимость склада ---");
                var totalValue = logic.CalculateTotalInventoryValue();
                Console.WriteLine($"Общая стоимость: {totalValue:C}");

                // Обновление товара
                if (allProducts.Count > 0)
                {
                    Console.WriteLine("\n--- Обновление товара ---");
                    var productToUpdate = allProducts[0];
                    Console.WriteLine($"Старая цена: {productToUpdate.Price:C}");
                    productToUpdate.Price += 5000;
                    logic.UpdateProduct(productToUpdate);
                    Console.WriteLine($"Новая цена: {productToUpdate.Price:C}");
                }

                Console.WriteLine("\n✅ Демонстрация Entity Framework завершена успешно!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Ошибка: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"   Детали: {ex.InnerException.Message}");
                }
            }
        }

        /// <summary>
        /// Демонстрирует использование Dapper репозитория.
        /// </summary>
        public static void DemoDapper()
        {
            Console.WriteLine("\n" + new string('=', 60));
            Console.WriteLine("ДЕМОНСТРАЦИЯ DAPPER РЕПОЗИТОРИЯ");
            Console.WriteLine(new string('=', 60));

            try
            {
                // Создаём репозиторий Dapper
                var repository = new DapperRepository<Product>();
                Console.WriteLine("✓ Dapper репозиторий создан");

                // Создаём бизнес-логику с репозиторием
                var logic = new ProductLogic(repository);
                Console.WriteLine("✓ Бизнес-логика инициализирована с Dapper репозиторием");

                // Получаем все товары из БД
                Console.WriteLine("\n--- Чтение товаров из БД (Dapper) ---");
                var allProducts = logic.GetAllProducts();
                Console.WriteLine($"Найдено товаров: {allProducts.Count}");
                foreach (var p in allProducts)
                {
                    Console.WriteLine($"  ID: {p.Id}, {p.Name}, {p.Price:C}");
                }

                // Добавляем новый товар через Dapper
                Console.WriteLine("\n--- Добавление товара через Dapper ---");
                var newProduct = new Product
                {
                    Name = "Монитор Samsung 27\"",
                    Description = "4K монитор с IPS матрицей",
                    Price = 35000,
                    Category = "Электроника",
                    StockQuantity = 10
                };
                logic.AddProduct(newProduct);
                Console.WriteLine($"+ {newProduct.Name} добавлен");

                // Проверяем, что товар добавлен
                allProducts = logic.GetAllProducts();
                Console.WriteLine($"Теперь товаров: {allProducts.Count}");

                // Расчёт общей стоимости через Dapper
                Console.WriteLine("\n--- Общая стоимость склада (Dapper) ---");
                var totalValue = logic.CalculateTotalInventoryValue();
                Console.WriteLine($"Общая стоимость: {totalValue:C}");

                Console.WriteLine("\n✅ Демонстрация Dapper завершена успешно!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Ошибка: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"   Детали: {ex.InnerException.Message}");
                }
            }
        }

        /// <summary>
        /// Демонстрирует работу без репозитория (старый режим с List в памяти).
        /// </summary>
        public static void DemoWithoutRepository()
        {
            Console.WriteLine("\n" + new string('=', 60));
            Console.WriteLine("ДЕМОНСТРАЦИЯ РЕЖИМА БЕЗ РЕПОЗИТОРИЯ (IN-MEMORY)");
            Console.WriteLine(new string('=', 60));

            try
            {
                // Создаём бизнес-логику без репозитория (старый способ)
                var logic = new ProductLogic();
                Console.WriteLine("✓ Бизнес-логика создана в режиме in-memory");

                // Получаем начальные тестовые данные
                Console.WriteLine("\n--- Начальные тестовые данные ---");
                var products = logic.GetAllProducts();
                Console.WriteLine($"Товаров в системе: {products.Count}");
                foreach (var p in products)
                {
                    Console.WriteLine($"  - {p.Name} ({p.Category})");
                }

                // Добавляем новый товар
                Console.WriteLine("\n--- Добавление нового товара ---");
                var newProduct = new Product
                {
                    Name = "Флешка Kingston 64GB",
                    Description = "USB 3.0 флеш-накопитель",
                    Price = 800,
                    Category = "Аксессуары",
                    StockQuantity = 100
                };
                logic.AddProduct(newProduct);
                Console.WriteLine($"+ {newProduct.Name} добавлен с ID: {newProduct.Id}");

                // Фильтрация
                Console.WriteLine("\n--- Фильтрация по категории 'Электроника' ---");
                var electronics = logic.FilterByCategory("Электроника");
                Console.WriteLine($"Найдено: {electronics.Count} товара(ов)");

                // Общая стоимость
                var totalValue = logic.CalculateTotalInventoryValue();
                Console.WriteLine($"\nОбщая стоимость склада: {totalValue:C}");

                Console.WriteLine("\n✅ Режим in-memory работает корректно!");
                Console.WriteLine("ℹ️  Данные хранятся в памяти и не сохраняются после завершения программы");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Ошибка: {ex.Message}");
            }
        }

        /// <summary>
        /// Главное меню демонстрации.
        /// </summary>
        public static void ShowMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("\n" + new string('═', 60));
                Console.WriteLine("    DATA ACCESS LAYER - ДЕМОНСТРАЦИЯ");
                Console.WriteLine(new string('═', 60));
                Console.WriteLine("\n  1. Демонстрация Entity Framework");
                Console.WriteLine("  2. Демонстрация Dapper");
                Console.WriteLine("  3. Демонстрация режима без репозитория (In-Memory)");
                Console.WriteLine("  4. Запустить все демонстрации");
                Console.WriteLine("  0. Выход");
                Console.WriteLine(new string('═', 60));
                Console.Write("\nВыберите опцию: ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        DemoEntityFramework();
                        break;
                    case "2":
                        DemoDapper();
                        break;
                    case "3":
                        DemoWithoutRepository();
                        break;
                    case "4":
                        DemoWithoutRepository();
                        Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                        Console.ReadKey();
                        DemoEntityFramework();
                        Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                        Console.ReadKey();
                        DemoDapper();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("\n❌ Неверный выбор. Попробуйте ещё раз.");
                        break;
                }

                if (choice != "0")
                {
                    Console.WriteLine("\n\nНажмите любую клавишу для возврата в меню...");
                    Console.ReadKey();
                }
            }
        }
    }
}
