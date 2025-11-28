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
    /// MVP Pattern - Composition Root / Entry Point.
    /// Contains the main entry point and dependency injection setup.
    /// SOLID - D: Uses DI container to wire up MVP components.
    /// </summary>
    static class Program
    {
        /// <summary>
        /// Main entry point for the Windows Forms application.
        /// Sets up Dependency Injection and creates MVP triad.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Initialize application configuration for modern .NET versions
            ApplicationConfiguration.Initialize();
            
            // Create Ninject DI kernel with configuration module
            // SimpleConfigModule already registers IProductModel -> ProductModelMvp
            using (var kernel = new StandardKernel(new SimpleConfigModule()))
            {
                try
                {
                    // Create the View (MainForm)
                    var view = new MainForm();
                    
                    // Get the Model from DI container
                    var model = kernel.Get<IProductModel>();
                    
                    // Create the Presenter, wiring View and Model
                    var presenter = new ProductPresenter(view, model);
                    
                    // Set the presenter on the view for dialog handling
                    view.SetPresenter(presenter, model);
                    
                    // Run the application
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
