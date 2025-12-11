using System;
using System.Windows;
using Ninject;
using ProductManagementSystem.DataAccessLayer;
using ProductManagementSystem.DataAccessLayer.EF;
using ProductManagementSystem.Logic;
using ProductManagementSystem.Model;
using ProductManagementSystem.WpfApp.Services;
using ProductManagementSystem.WpfApp.ViewModels;

namespace ProductManagementSystem.WpfApp
{
    /// <summary>
    /// MVVM Pattern - Composition Root.
    /// 
    /// SOLID - S: Класс отвечает только за инициализацию приложения и настройку DI.
    /// SOLID - D: Использует Dependency Injection для управления зависимостями.
    /// 
    /// App является точкой входа WPF приложения.
    /// Реализует ViewModelFirst подход: сначала создается ViewModel, затем View.
    /// Настраивает DI контейнер (Ninject) для автоматического разрешения зависимостей.
    /// </summary>
    public partial class App : Application
    {
        private IKernel? _kernel;

        /// <summary>
        /// Переопределяет метод запуска приложения.
        /// Вызывается при старте приложения вместо использования StartupUri в XAML.
        /// 
        /// ViewModelFirst подход:
        /// 1. Настраиваем DI контейнер (Ninject)
        /// 2. Создаем MainViewModel через DI
        /// 3. Создаем MainWindow и устанавливаем DataContext
        /// 4. Показываем окно
        /// </summary>
        /// <param name="e">Аргументы события запуска</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                // Шаг 1: Настройка Dependency Injection контейнера
                ConfigureDependencyInjection();

                // Шаг 2: Создание ViewManager для управления окнами
                var viewManager = new ViewManager();

                // Шаг 3: Создание MainViewModel через DI
                // Ninject автоматически разрешит все зависимости (ILogic, IRepository и т.д.)
                var mainViewModel = _kernel!.Get<MainViewModel>();

                // Шаг 4: ViewModelFirst - показываем главное окно с ViewModel
                viewManager.ShowMainWindow(mainViewModel);
            }
            catch (Exception ex)
            {
                // Обработка критических ошибок при запуске
                MessageBox.Show(
                    $"Критическая ошибка при запуске приложения:\n{ex.Message}\n\nПриложение будет закрыто.",
                    "Ошибка запуска",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                Shutdown(1);
            }
        }

        /// <summary>
        /// Настраивает Dependency Injection контейнер (Ninject).
        /// 
        /// SOLID - D: Принцип инверсии зависимостей.
        /// Регистрируем привязки между интерфейсами и реализациями.
        /// 
        /// Ninject автоматически создаст и внедрит зависимости при запросе типа.
        /// Например, при создании MainViewModel он автоматически создаст:
        /// - ILogic -> ProductLogic
        /// - IRepository<Product> -> EntityRepository<Product> (или DapperRepository)
        /// - IBusinessFunctions -> BusinessFunctions
        /// </summary>
        private void ConfigureDependencyInjection()
        {
            _kernel = new StandardKernel();

            // Регистрация репозитория данных
            // ПЕРЕКЛЮЧАТЕЛЬ: Раскомментируйте нужный репозиторий
            _kernel.Bind<IRepository<Product>>().To<EntityRepository<Product>>().InSingletonScope();
            // _kernel.Bind<IRepository<Product>>().To<DapperRepository<Product>>().InSingletonScope();

            // Регистрация бизнес-функций
            _kernel.Bind<IBusinessFunctions>().To<BusinessFunctions>().InSingletonScope();

            // Регистрация основной бизнес-логики
            _kernel.Bind<ILogic>().To<ProductLogic>().InSingletonScope();

            // Регистрация ViewModel
            _kernel.Bind<MainViewModel>().ToSelf().InTransientScope();
        }

        /// <summary>
        /// Вызывается при завершении работы приложения.
        /// Освобождает ресурсы DI контейнера.
        /// </summary>
        /// <param name="e">Аргументы события завершения</param>
        protected override void OnExit(ExitEventArgs e)
        {
            _kernel?.Dispose();
            base.OnExit(e);
        }
    }
}
