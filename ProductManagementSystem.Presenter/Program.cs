using System;
using Ninject;
using ProductManagementSystem.Logic;
using ProductManagementSystem.WinFormsApp;

namespace ProductManagementSystem.Presenter
{
    /// <summary>
    /// MVP Pattern - Presenter Application (Автозапускаемый элемент).
    /// 
    /// ВАЖНО: Это главная точка входа для MVP-архитектуры.
    /// Presenter связывает пассивную View (WinFormsApp) и активную Model (IProductModel),
    /// обеспечивая 100% выполнение паттерна MVP.
    /// 
    /// SOLID - S: Класс отвечает только за инициализацию и запуск приложения.
    /// SOLID - D: Используем DI-контейнер для управления зависимостями.
    /// 
    /// Согласно техническому заданию:
    /// - Точка сборки (CompositionRoot) находится в Presenter
    /// - Presenter запускает View (WinFormsApp)
    /// - Presenter передает Model во View
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Главная точка входа в приложение MVP.
        /// Инициализирует DI-контейнер, создает Model и запускает View.
        /// </summary>
        /// <param name="args">Аргументы командной строки</param>
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("Включаем");
            
            try
            {
                // SOLID - D: Используем DI-контейнер для управления зависимостями
                // MVP: Точка сборки (CompositionRoot) находится в Presenter
                using var kernel = new StandardKernel(new CompositionRoot());
                
                Console.WriteLine("Получение что там в Model (IProductModel)...");
                
                // Получаем Model через DI (все зависимости разрешаются автоматически)
                var model = kernel.Get<IProductModel>();
                
                Console.WriteLine("Model успешно создан и настроен");
                Console.WriteLine("Запуск View (WinFormsApp)");
                Console.WriteLine();
                
                // MVP: Presenter запускает View с Model
                // View (WinFormsApp) - пассивная, только отображает данные
                // Model (IProductModel) - активная, содержит бизнес-логику
                // Presenter (этот проект) - связывает их через DI
                WinFormsRunner.Run(model);
                
                Console.WriteLine();
                Console.WriteLine("WinFormsApp успешно завершено))");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка!");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine("Нажмите любую клавишу для выхода...");
                Console.ReadKey();
            }
        }
    }
}
