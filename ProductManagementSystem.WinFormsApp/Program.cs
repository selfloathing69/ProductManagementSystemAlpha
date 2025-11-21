using System;
using System.Windows.Forms;

namespace ProductManagementSystem.WinFormsApp
{
    /// <summary>
    /// Главный класс Windows Forms приложения для управления товарами.
    /// Содержит точку входа для запуска приложения.
    /// </summary>
    static class Program
    {
        /// <summary>
        /// Главная точка входа для Windows Forms приложения.
        /// Инициализирует приложение и запускает главную форму.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Инициализация конфигурации приложения для современных версий .NET
            ApplicationConfiguration.Initialize();
            
            // Запуск главной формы приложения
            Application.Run(new MainForm());
        }
    }
}
