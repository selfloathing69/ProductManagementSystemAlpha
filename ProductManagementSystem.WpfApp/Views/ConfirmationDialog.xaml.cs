using System.Windows;
using ProductManagementSystem.WpfApp.ViewModels;

namespace ProductManagementSystem.WpfApp.Views
{
    /// <summary>
    /// MVVM Pattern - View для диалога подтверждения изменений.
    /// ViewModelFirst: DataContext устанавливается перед отображением.
    /// </summary>
    public partial class ConfirmationDialog : BaseView
    {
        /// <summary>
        /// Инициализирует новый экземпляр ConfirmationDialog.
        /// </summary>
        public ConfirmationDialog(ConfirmationViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();

            // Подписываемся на изменение DialogResult для автоматического закрытия окна
            if (DataContext is ConfirmationViewModel vm)
            {
                vm.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(ConfirmationViewModel.DialogResult))
                    {
                        if (vm.DialogResult.HasValue)
                        {
                            DialogResult = vm.DialogResult;
                            Close();
                        }
                    }
                };
            }
        }
    }
}
