using System;
using System.Windows.Forms;
using Ninject;
using ProductManagementSystem.DataAccessLayer;
using ProductManagementSystem.Logic;
using ProductManagementSystem.Logic.Presenters;
using ProductManagementSystem.Shared;

namespace ProductManagementSystem.WinFormsApp
{
    /// <summary>
    /// MVP паттерн - корень композиции / точка входа.
    /// Содержит главную точку входа и настройку внедрения зависимостей.
    /// SOLID - D: Использует DI-контейнер для связывания MVP компонентов.
    /// </summary>
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения Windows Forms.
        /// Настраивает Dependency Injection и создаёт триаду MVP.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Инициализация конфигурации приложения для современных версий .NET
            ApplicationConfiguration.Initialize();
            
            // Создание ядра Ninject DI с модулем конфигурации
            // SimpleConfigModule уже регистрирует IProductModel -> ProductModelMvp
            using (var kernel = new StandardKernel(new SimpleConfigModule()))
            {
                try
                {
                    // Создаём представление (MainForm)
                    var view = new MainForm();
                    
                    // Получаем модель из DI-контейнера
                    var model = kernel.Get<IProductModel>();
                    
                    // Создаём презентер, связывая представление и модель
                    var presenter = new ProductPresenter(view, model);
                    
                    // Устанавливаем презентер на представление для обработки диалогов
                    view.SetPresenter(presenter, model);
                    
                    // Запускаем приложение
                    Application.Run(view);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Ошибка запуска приложения: {ex.Message}\n\n{ex.StackTrace}",
                        "Критическая ошибка",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }
    }
}
