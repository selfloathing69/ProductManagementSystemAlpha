using System;
using ProductManagementSystem.Logic;
using ProductManagementSystem.Model;

namespace ProductManagementSystem.ConsoleApp
{
    internal class Program
    {
        private static ProductLogic _productLogic = new ProductLogic();
        static void Main(string[] args)
        {
            bool exit = false;
            Console.WriteLine("===== Система управления товарами (Console) =====");
            while (!exit)
            {
                ShowMenu();
                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1": ShowAllProducts(); break;
                    case "2": ShowProductDetails(); break;
                    case "3": AddProduct(); break;
                    case "4": UpdateProduct(); break;
                    case "5": DeleteProduct(); break;
                    case "6": FilterByCategory(); break;
                    case "7": CalculateTotalInventoryValue(); break;
                    case "0": exit = true; break;
                    default: Console.WriteLine("Неверный выбор."); break;
                }
                if (!exit) { Console.WriteLine("Нажмите любую клавишу..."); Console.ReadKey(); Console.Clear(); }
            }
        }

        static void ShowMenu()
        {
            Console.WriteLine("1. Показать все товары");
            Console.WriteLine("2. Показать детали товара");
            Console.WriteLine("3. Добавить новый товар");
            Console.WriteLine("4. Обновить товар");
            Console.WriteLine("5. Удалить товар");
            Console.WriteLine("6. Фильтр по категории");
            Console.WriteLine("7. Рассчитать общую стоимость на складе");
            Console.WriteLine("0. Выход");
            Console.Write("Выбор: ");
        }

        static void ShowAllProducts()
        {
            var products = _productLogic.GetAllProducts();
            Console.WriteLine("===== Все товары =====");
            foreach (var p in products) Console.WriteLine(p);
        }

        static void ShowProductDetails()
        {
            Console.Write("Введите ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id)) { Console.WriteLine("Некорректный ID"); return; }
            var p = _productLogic.GetProduct(id);
            if (p==null) { Console.WriteLine("Не найден"); return; }
            Console.WriteLine(p);
        }

        static void AddProduct()
        {
            var prod = new Product();
            Console.Write("Название: "); prod.Name = Console.ReadLine() ?? "";
            Console.Write("Описание: "); prod.Description = Console.ReadLine() ?? "";
            Console.Write("Цена: "); if (!decimal.TryParse(Console.ReadLine(), out var price)) { Console.WriteLine("Некорректно"); return; } prod.Price = price;
            Console.Write("Категория: "); prod.Category = Console.ReadLine() ?? "";
            Console.Write("Количество: "); if (!int.TryParse(Console.ReadLine(), out var q)) { Console.WriteLine("Некорректно"); return; } prod.StockQuantity = q;
            _productLogic.AddProduct(prod);
            Console.WriteLine("Добавлено.");
        }

        static void UpdateProduct()
        {
            Console.Write("ID для обновления: ");
            if (!int.TryParse(Console.ReadLine(), out int id)) { Console.WriteLine("Некорректно"); return; }
            var existing = _productLogic.GetProduct(id);
            if (existing==null) { Console.WriteLine("Не найден"); return; }
            Console.Write($"Название [{existing.Name}]: "); var name = Console.ReadLine(); if (!string.IsNullOrWhiteSpace(name)) existing.Name = name;
            Console.Write($"Описание [{existing.Description}]: "); var desc = Console.ReadLine(); if (!string.IsNullOrWhiteSpace(desc)) existing.Description = desc;
            Console.Write($"Цена [{existing.Price}]: "); var priceS = Console.ReadLine(); if (decimal.TryParse(priceS, out var pr)) existing.Price = pr;
            Console.Write($"Категория [{existing.Category}]: "); var cat = Console.ReadLine(); if (!string.IsNullOrWhiteSpace(cat)) existing.Category = cat;
            Console.Write($"Количество [{existing.StockQuantity}]: "); var qS = Console.ReadLine(); if (int.TryParse(qS, out var q)) existing.StockQuantity = q;
            if (_productLogic.UpdateProduct(existing)) Console.WriteLine("Обновлено"); else Console.WriteLine("Ошибка");
        }

        static void DeleteProduct()
        {
            Console.Write("ID для удаления: ");
            if (!int.TryParse(Console.ReadLine(), out int id)) { Console.WriteLine("Некорректно"); return; }
            Console.Write("Подтвердите (да/нет): ");
            var c = Console.ReadLine();
            if (c?.ToLower() != "да") { Console.WriteLine("Отмена"); return; }
            if (_productLogic.DeleteProduct(id)) Console.WriteLine("Удалено"); else Console.WriteLine("Не найдено");
        }

        static void FilterByCategory()
        {
            Console.Write("Категория: ");
            var cat = Console.ReadLine() ?? "";
            var list = _productLogic.FilterByCategory(cat);
            foreach (var p in list) Console.WriteLine(p);
        }

        static void CalculateTotalInventoryValue()
        {
            var total = _productLogic.CalculateTotalInventoryValue();
            Console.WriteLine($"Общая стоимость: {total:C}");
        }
    }
}
