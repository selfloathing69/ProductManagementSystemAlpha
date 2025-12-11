using System.Windows;

namespace ProductManagementSystem.WpfApp.Views
{
    /// <summary>
    /// MVVM Pattern - View (Представление).
    /// 
    /// SOLID - S: Класс отвечает только за инициализацию UI компонентов.
    /// 
    /// CodeBehind в MVVM должен быть минимальным.
    /// Вся бизнес-логика и логика представления находится в ViewModel.
    /// View только инициализирует компоненты и устанавливает DataContext.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Инициализирует новый экземпляр главного окна.
        /// DataContext устанавливается извне через ViewManager (ViewModelFirst подход).
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }
    }
}
