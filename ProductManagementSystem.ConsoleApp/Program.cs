using System;
using Ninject;
using ProductManagementSystem.Logic.Services;

namespace ProductManagementSystem.ConsoleApp
{
    /// <summary>
    /// Консольное приложение для управления товарами.
    /// SOLID - S: Класс отвечает только за инициализацию и запуск консольного приложения.
    /// SOLID - D: Используем DI-контейнер для управления зависимостями.
    /// 
    /// ВАЖНО: Это независимое консольное приложение с текстовым меню (MenuController).
    /// Не путать с Presenter - Presenter запускает WinFormsApp.========================================================
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Главная точка входа в консольное приложение.
        /// Инициализирует DI-контейнер и запускает MenuController.
        /// </summary>
        /// <param name="args">Аргументы командной строки</param>
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("Включаю... хочу чаю");
            
            try
            {
                // SOLID - D: Используем DI-контейнер
                using var kernel = new StandardKernel(new CompositionRoot());
                
                // Получаем сервис для работы с товарами
                var productService = kernel.Get<IProductService>();
                
                // Запускаем консольное меню
                var menuController = new MenuController(productService);
                menuController.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                Console.WriteLine("Нажмите любую клавишу для выхода...");
                Console.ReadKey();
            }
        }
    }
}
