using System;
using Ninject;
using ProductManagementSystem.DataAccessLayer;

namespace ProductManagementSystem.ConsoleApp
{
    /// <summary>
    /// SOLID - S: Класс отвечает только за инициализацию и запуск приложения.
    /// 
    /// Точка входа консольного приложения для управления товарами.
    /// Использует Dependency Injection для конфигурации зависимостей.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Главная точка входа в консольное приложение.
        /// Инициализирует DI-контейнер и запускает MenuController.
        /// </summary>
        /// <param name="args">Аргументы командной строки</param>
        static void Main(string[] args)
        {
            // SOLID - D: Используем DI-контейнер для управления зависимостями
            // Создаём Ninject kernel с конфигурацией из SimpleConfigModule
            var kernel = new StandardKernel(new SimpleConfigModule());

            // Регистрируем MenuController (специфично для ConsoleApp)
            kernel.Bind<MenuController>().ToSelf();

            // Получаем MenuController через DI (все зависимости разрешаются автоматически)
            var menuController = kernel.Get<MenuController>();

            // Запускаем приложение
            menuController.Run();
        }
    }
}
