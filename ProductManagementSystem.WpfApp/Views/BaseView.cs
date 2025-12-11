using System.Windows;
using ProductManagementSystem.WpfApp.ViewModels;

namespace ProductManagementSystem.WpfApp.Views
{
    /// <summary>
    /// MVVM Pattern - Базовый класс для всех окон View.
    /// 
    /// SOLID - S: Класс отвечает только за базовую функциональность окон.
    /// SOLID - O: Открыт для расширения через наследование.
    /// 
    /// ViewModelFirst Pattern:
    /// - Окно создается после создания ViewModel
    /// - DataContext устанавливается через конструктор или ViewManager
    /// - CodeBehind минимален (только InitializeComponent)
    /// 
    /// Преимущества базового класса:
    /// - Единообразие всех окон
    /// - Централизованная логика работы с ViewModel
    /// - Упрощение тестирования
    /// </summary>
    
    
    public abstract class BaseView : Window
    {
        /// <summary>
        /// Получает ViewModel, связанную с этим View.
        /// </summary>
        
        
        protected ViewModelBase? ViewModel => DataContext as ViewModelBase;

        /// <summary>
        /// Инициализирует новый экземпляр BaseView.
        /// </summary>
        
        
        protected BaseView()
        {
            // Устанавливаем обработчики событий для жизненного цикла окна
            Loaded += OnViewLoaded;
            Unloaded += OnViewUnloaded;
        }

        /// <summary>
        /// Инициализирует новый экземпляр BaseView с указанной ViewModel.
        /// ViewModelFirst: ViewModel передается в конструктор View.
        /// </summary>

        protected BaseView(ViewModelBase viewModel) : this()
        {
            DataContext = viewModel;
        }

        /// <summary>
        /// Вызывается при загрузке View.
        /// Переопределите для инициализации специфичной логики View.
        /// </summary>
        
        protected virtual void OnViewLoaded(object sender, RoutedEventArgs e)
        {
            // Базовая реализация пуста
            // Наследники могут переопределить для своей логики
        }

        /// <summary>
        /// Вызывается при выгрузке View.
        /// Переопределите для освобождения ресурсов.
        /// </summary>
        
        protected virtual void OnViewUnloaded(object sender, RoutedEventArgs e)
        {
            // Очистка привязок для предотвращения утечек памяти
            DataContext = null;
        }

        /// <summary>
        /// Устанавливает ViewModel для View.
        /// Используется ViewManager для динамического связывания.
        /// </summary>

        public void SetViewModel(ViewModelBase viewModel)
        {
            DataContext = viewModel;
        }
    }
}
