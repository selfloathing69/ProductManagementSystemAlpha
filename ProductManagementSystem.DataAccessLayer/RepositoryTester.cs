using System;
using System.Linq;
using ProductManagementSystem.Model;
using ProductManagementSystem.DataAccessLayer.EF;
using ProductManagementSystem.DataAccessLayer.Dapper;

namespace ProductManagementSystem.DataAccessLayer
{
    /// <summary>
    /// Класс для тестирования репозиториев Entity Framework и Dapper.
    /// Демонстрирует использование обоих подходов к работе с данными.
    /// </summary>
    public class RepositoryTester
    {
        /// <summary>
        /// Тестирует работу Entity Framework репозитория.
        /// </summary>
        public static void TestEntityFramework()
        {
            Console.WriteLine("=== Тестирование Entity Framework ===");
            
            try
            {
                var repository = new EntityRepository<Product>();
                
                // Тест 1: Добавление товаров
                Console.WriteLine("\n1. Добавление товаров...");
                var product1 = new Product 
                { 
                    Name = "Ноутбук Dell", 
                    Description = "Мощный ноутбук для работы", 
                    Price = 85000, 
                    Category = "Электроника", 
                    StockQuantity = 10 
                };
                repository.Add(product1);
                Console.WriteLine("✓ Товар 1 добавлен");

                var product2 = new Product 
                { 
                    Name = "Мышь Logitech", 
                    Description = "Беспроводная мышь", 
                    Price = 1500, 
                    Category = "Электроника", 
                    StockQuantity = 50 
                };
                repository.Add(product2);
                Console.WriteLine("✓ Товар 2 добавлен");

                // Тест 2: Чтение всех товаров
                Console.WriteLine("\n2. Чтение всех товаров...");
                var allProducts = repository.ReadAll();
                Console.WriteLine($"✓ Найдено {allProducts.Count()} товаров:");
                foreach (var p in allProducts)
                {
                    Console.WriteLine($"   - ID: {p.Id}, Название: {p.Name}, Цена: {p.Price}");
                }

                // Тест 3: Чтение товара по ID
                if (allProducts.Any())
                {
                    var firstProduct = allProducts.First();
                    Console.WriteLine($"\n3. Чтение товара по ID ({firstProduct.Id})...");
                    var productById = repository.ReadById(firstProduct.Id);
                    Console.WriteLine($"✓ Товар найден: {productById.Name}");

                    // Тест 4: Обновление товара
                    Console.WriteLine($"\n4. Обновление товара ID {firstProduct.Id}...");
                    productById.Price = productById.Price + 1000;
                    repository.Update(productById);
                    Console.WriteLine($"✓ Цена обновлена на {productById.Price}");

                    // Тест 5: Удаление товара
                    Console.WriteLine($"\n5. Удаление товара ID {firstProduct.Id}...");
                    repository.Delete(firstProduct.Id);
                    Console.WriteLine("✓ Товар удалён");

                    // Проверка удаления
                    var remainingProducts = repository.ReadAll();
                    Console.WriteLine($"✓ Осталось товаров: {remainingProducts.Count()}");
                }

                Console.WriteLine("\n✅ Все тесты Entity Framework пройдены успешно!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Ошибка при тестировании Entity Framework: {ex.Message}");
                Console.WriteLine($"Детали: {ex.InnerException?.Message}");
            }
        }

        /// <summary>
        /// Тестирует работу Dapper репозитория.
        /// </summary>
        public static void TestDapper()
        {
            Console.WriteLine("\n\n=== Тестирование Dapper ===");
            
            try
            {
                var repository = new DapperRepository<Product>();
                
                // Тест 1: Чтение всех товаров
                Console.WriteLine("\n1. Чтение всех товаров...");
                var allProducts = repository.ReadAll();
                Console.WriteLine($"✓ Найдено {allProducts.Count()} товаров:");
                foreach (var p in allProducts)
                {
                    Console.WriteLine($"   - ID: {p.Id}, Название: {p.Name}, Цена: {p.Price}");
                }

                // Тест 2: Добавление товара
                Console.WriteLine("\n2. Добавление товара через Dapper...");
                var product = new Product 
                { 
                    Name = "Клавиатура Razer", 
                    Description = "Механическая клавиатура", 
                    Price = 8500, 
                    Category = "Электроника", 
                    StockQuantity = 20 
                };
                repository.Add(product);
                Console.WriteLine("✓ Товар добавлен");

                // Тест 3: Чтение всех товаров после добавления
                Console.WriteLine("\n3. Чтение всех товаров после добавления...");
                allProducts = repository.ReadAll();
                Console.WriteLine($"✓ Теперь найдено {allProducts.Count()} товаров");

                // Тест 4: Чтение товара по ID
                if (allProducts.Any())
                {
                    var firstProduct = allProducts.First();
                    Console.WriteLine($"\n4. Чтение товара по ID ({firstProduct.Id})...");
                    var productById = repository.ReadById(firstProduct.Id);
                    Console.WriteLine($"✓ Товар найден: {productById.Name}");

                    // Тест 5: Обновление товара
                    Console.WriteLine($"\n5. Обновление товара ID {firstProduct.Id}...");
                    productById.Price = productById.Price + 500;
                    repository.Update(productById);
                    Console.WriteLine($"✓ Цена обновлена на {productById.Price}");

                    // Тест 6: Удаление товара
                    Console.WriteLine($"\n6. Удаление товара ID {firstProduct.Id}...");
                    repository.Delete(firstProduct.Id);
                    Console.WriteLine("✓ Товар удалён");
                }

                Console.WriteLine("\n✅ Все тесты Dapper пройдены успешно!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Ошибка при тестировании Dapper: {ex.Message}");
                Console.WriteLine($"Детали: {ex.InnerException?.Message}");
            }
        }

        /// <summary>
        /// Демонстрирует использование репозитория в бизнес-логике.
        /// </summary>
        public static void TestBusinessLogicIntegration()
        {
            Console.WriteLine("\n\n=== Тестирование интеграции с Business Logic ===");
            
            try
            {
                // Создаём репозиторий Entity Framework
                var efRepository = new EntityRepository<Product>();
                
                // Создаём бизнес-логику с репозиторием
                var logic = new Logic.ProductLogic(efRepository);
                
                Console.WriteLine("\n1. Получение всех товаров через Business Logic...");
                var products = logic.GetAllProducts();
                Console.WriteLine($"✓ Найдено {products.Count} товаров");

                Console.WriteLine("\n2. Добавление товара через Business Logic...");
                var newProduct = new Product 
                { 
                    Name = "Монитор Samsung", 
                    Description = "4K монитор 27 дюймов", 
                    Price = 35000, 
                    Category = "Электроника", 
                    StockQuantity = 15 
                };
                logic.AddProduct(newProduct);
                Console.WriteLine("✓ Товар добавлен");

                Console.WriteLine("\n3. Фильтрация по категории 'Электроника'...");
                var electronics = logic.FilterByCategory("Электроника");
                Console.WriteLine($"✓ Найдено {electronics.Count} товаров в категории Электроника");

                Console.WriteLine("\n4. Расчёт общей стоимости склада...");
                var totalValue = logic.CalculateTotalInventoryValue();
                Console.WriteLine($"✓ Общая стоимость: {totalValue:C}");

                Console.WriteLine("\n✅ Интеграция с Business Logic работает корректно!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Ошибка при тестировании интеграции: {ex.Message}");
                Console.WriteLine($"Детали: {ex.InnerException?.Message}");
            }
        }
    }
}
