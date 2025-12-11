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
    /// MVVM Pattern - Composition Root (Корень композиции).
    /// 
    /// SOLID - S: Класс отвечает только за инициализацию приложения и настройку DI.
    /// SOLID - D: Использует Dependency Injection для управления зависимостями.
    /// 
    /// App является точкой входа WPF приложения.
    /// Реализует ViewModelFirst вот та вот:
    /// 
    /// 1. Настраиваем DI контейнер (Ninject)
    ///    - Регистрируем Repository, BusinessLogic, ViewModel
    /// 
    /// 2. Создаем ViewManager для управления окнами
    ///    - ViewManager знает как создавать View для ViewModel
    /// 
    /// 3. Создаем MainViewModel через DI
    ///    - Ninject автоматически разрешает все зависимости
    ///    - ViewModel не знает о существовании View
    /// 
    /// 4. Передаем ViewModel в ViewManager
    ///    - ViewManager создает MainWindow
    ///    - Устанавливает DataContext = ViewModel
    ///    - Показывает окно пользователю
    /// 
    /// КЛЮЧЕВАЯ ОСОБЕННОСТЬ: ViewModel создается РАНЬШЕ View!!!!!!
    /// </summary>
    public partial class App : Application
    {
        private IKernel? _kernel;

        /// <summary>
        /// Переопределяет метод запуска приложения.
        /// Вызывается при старте приложения вместо использования StartupUri в XAML.
        /// 
        /// ViewModelFirst подход (согласно заданию):
        /// ==========================================
        /// Шаг 1: Настраиваем DI контейнер (Ninject)
        ///        - Регистрируем зависимости: Repository → Logic → ViewModel
        /// 
        /// Шаг 2: Создаем ViewManager (управление окнами)
        ///        - Отвечает за создание View по ViewModel
        /// 
        /// Шаг 3: Создаем MainViewModel через DI
        ///        - ViewModel создается ПЕРВОЙ (ViewModelFirst!)
        ///        - Ninject внедряет ILogic, IBusinessFunctions и т.д.
        /// 
        /// Шаг 4: ViewManager показывает MainWindow с ViewModel
        ///        - ViewManager создает MainWindow
        ///        - MainWindow.DataContext = MainViewModel
        ///        - Все Binding в XAML автоматически работают
        /// </summary>
        /// <param name="e">Аргументы события запуска</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                // ═══════════════════════════════════════════════════════════
                // ШАГ 1: Настройка Dependency Injection контейнера
                // ═══════════════════════════════════════════════════════════
                ConfigureDependencyInjection();

                // ═══════════════════════════════════════════════════════════
                // ШАГ 2: Создание ViewManager для управления окнами
                // ═══════════════════════════════════════════════════════════
                // ViewManager реализует ViewModelFirst подход:
                // - Получает ViewModel
                // - Создает соответствующую View
                // - Связывает их через DataContext
                var viewManager = new ViewManager();

                // ═══════════════════════════════════════════════════════════
                // ШАГ 3: Создание MainViewModel через DI (ViewModelFirst!)
                // ═══════════════════════════════════════════════════════════
                // ВАЖНО: ViewModel создается РАНЬШЕ View!
                // Ninject автоматически разрешит все зависимости:
                // - ILogic → ProductLogic
                // - IRepository<Product> → EntityRepository<Product>
                // - IBusinessFunctions → BusinessFunctions
                var mainViewModel = _kernel!.Get<MainViewModel>();

                // ═══════════════════════════════════════════════════════════
                // ШАГ 4: ViewManager создает и показывает главное окно
                // ═══════════════════════════════════════════════════════════
                // ViewManager:
                // 1. Создает MainWindow (View)
                // 2. Устанавливает MainWindow.DataContext = mainViewModel
                // 3. Показывает окно: mainWindow.Show()
                //
                // Теперь все Binding в MainWindow.xaml работают:
                // - {Binding Products} → mainViewModel.Products
                // - {Binding RefreshCommand} → mainViewModel.RefreshCommand
                viewManager.ShowMainWindow(mainViewModel);
            }
            catch (Exception ex)
            {
                // Обработка ошибок при запуске
                MessageBox.Show(
                    $"Ошибка инициализации приложения:\n{ex.Message}\n\nПриложение будет закрыто",
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
        /// - ILogic → ProductLogic
        /// - IRepository<Product> → EntityRepository<Product> (или DapperRepository)
        /// - IBusinessFunctions → BusinessFunctions
        /// 
        /// ГРАФ ЗАВИСИМОСТЕЙ:
        /// ==================
        /// MainViewModel
        ///   ↓ требует ILogic
        /// ProductLogic
        ///   ↓ требует IRepository<Product> + IBusinessFunctions
        /// EntityRepository<Product> + BusinessFunctions
        /// </summary>
        private void ConfigureDependencyInjection()
        {
            _kernel = new StandardKernel();

            // ───────────────────────────────────────────────────────────
            // Регистрация репозитория данных (Data Access Layer)
            // ───────────────────────────────────────────────────────────
            // ПЕРЕКЛЮЧАТЕЛЬ: Выберите нужную реализацию
            _kernel.Bind<IRepository<Product>>().To<EntityRepository<Product>>().InSingletonScope();
            // _kernel.Bind<IRepository<Product>>().To<DapperRepository<Product>>().InSingletonScope();

            // ───────────────────────────────────────────────────────────
            // Регистрация бизнес-функций (Business Logic)
            // ───────────────────────────────────────────────────────────
            _kernel.Bind<IBusinessFunctions>().To<BusinessFunctions>().InSingletonScope();

            // ───────────────────────────────────────────────────────────
            // Регистрация основной бизнес-логики (Facade Pattern)
            // ───────────────────────────────────────────────────────────
            _kernel.Bind<ILogic>().To<ProductLogic>().InSingletonScope();

            // ───────────────────────────────────────────────────────────
            // Регистрация ViewModel (MVVM Pattern)
            // ───────────────────────────────────────────────────────────
            // InTransientScope() - каждый раз создается новый экземпляр
            // (для окон, которые могут открываться несколько раз)
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
