using System.Windows;
using ProductManagementSystem.WpfApp.ViewModels;

namespace ProductManagementSystem.WpfApp.Views
{
    /// <summary>
    /// MVVM Pattern - Main View (Представление).
    /// 
    /// SOLID - S: Класс отвечает только за инициализацию UI компонентов.
    /// SOLID - O: Наследует BaseView для расширения базовой функциональности.
    /// 
    /// ViewModelFirst Pattern:
    /// - Окно создается ПОСЛЕ создания MainViewModel
    /// - DataContext устанавливается через ViewManager
    /// - CodeBehind почти нет (только InitializeComponent)
    /// 
    /// CodeBehind в MVVM должен быть минимальным.
    /// Вся бизнес-логика и обработка событий находятся в ViewModel.
    /// View только отображает интерфейс и устанавливает DataContext.
    /// </summary>
    public partial class MainWindow : BaseView // ==== code behind
    {
        /// <summary>
        /// Инициализирует новый экземпляр главного окна.
        /// DataContext устанавливается извне через ViewManager (ViewModelFirst подход).
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Инициализирует новый экземпляр главного окна с указанной ViewModel.
        /// ViewModelFirst: ViewModel передается в конструктор.
        /// </summary>
        /// <param name="viewModel">MainViewModel для привязки</param>
        public MainWindow(MainViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }
    }
}
