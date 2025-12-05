using System;
using Ninject;
using ProductManagementSystem.DataAccessLayer;
using ProductManagementSystem.Logic;
using ProductManagementSystem.WinFormsApp;

namespace ProductManagementSystem.ConsoleApp
{
    /// <summary>
    /// MVP Pattern - Presenter Application (Composition Root).
    /// SOLID - S: Класс отвечает только за инициализацию и запуск приложения.
    /// SOLID - D: Используем DI-контейнер для управления зависимостями.
    /// 
    /// По требованиям паттерна MVP - консольное приложение является Presenter,
    /// которое при запуске создает и запускает WinForm с ProductView.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Главная точка входа в приложение.
        /// Инициализирует DI-контейнер и запускает WinForms UI.
        /// </summary>
        /// <param name="args">Аргументы командной строки</param>
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("Запускаем Продакт манагемент систем.");
            Console.WriteLine("Запускаем ДИ контейнер");
            
            // SOLID - D: Используем DI-контейнер для управления зависимостями
            // Создаём Ninject kernel с конфигурацией из SimpleConfigModule
            using var kernel = new StandardKernel(new SimpleConfigModule());

            Console.WriteLine("Смотрим Модел");
            
            // Получаем Model через DI (все зависимости разрешаются автоматически)
            var model = kernel.Get<IProductModel>();

            Console.WriteLine("Запущено");
            
            // Запускаем WinForms UI
            // MVP: ConsoleApp (Presenter) запускает WinForms (View) с Model
            WinFormsRunner.Run(model);
            
            Console.WriteLine("Закрыто");
        }
    }
}
