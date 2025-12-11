using System;
using System.Windows.Input;

namespace ProductManagementSystem.WpfApp.Commands
{
    /// <summary>
    /// MVVM Pattern - Реализация команды для привязки действий к элементам UI.
    /// 
    /// SOLID - S: Класс отвечает только за инкапсуляцию выполняемого действия и проверку возможности выполнения.
    /// SOLID - I: Реализует интерфейс ICommand для обеспечения совместимости с WPF Binding.
    /// 
    /// RelayCommand позволяет привязывать методы ViewModel к кнопкам и другим элементам UI
    /// без необходимости писать обработчики событий в CodeBehind.
    /// 
    /// Пример использования в ViewModel:
    /// <code>
    /// public ICommand AddProductCommand => new RelayCommand(AddProduct, CanAddProduct);
    /// 
    /// private void AddProduct(object parameter)
    /// {
    ///     // Логика добавления продукта
    /// }
    /// 
    /// private bool CanAddProduct(object parameter)
    /// {
    ///     return !string.IsNullOrEmpty(ProductName);
    /// }
    /// </code>
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Predicate<object?>? _canExecute;

        /// <summary>
        /// Инициализирует новый экземпляр команды с действием и необязательным предикатом для проверки.
        /// </summary>
        /// <param name="execute">Действие, которое будет выполнено при вызове команды</param>
        /// <param name="canExecute">Предикат, определяющий, может ли команда быть выполнена (необязательно)</param>
        /// <exception cref="ArgumentNullException">Выбрасывается, если execute равен null</exception>
        public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <summary>
        /// Событие, возникающее при изменении условий, влияющих на возможность выполнения команды.
        /// WPF автоматически подписывается на это событие и обновляет состояние элементов UI (например, кнопок).
        /// </summary>
        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        /// <summary>
        /// Определяет, может ли команда быть выполнена в текущем состоянии.
        /// </summary>
        /// <param name="parameter">Параметр команды (может быть null)</param>
        /// <returns>true, если команда может быть выполнена; иначе false</returns>
        public bool CanExecute(object? parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        /// <summary>
        /// Выполняет логику команды.
        /// </summary>
        /// <param name="parameter">Параметр команды (может быть null)</param>
        public void Execute(object? parameter)
        {
            _execute(parameter);
        }

        /// <summary>
        /// Принудительно вызывает перепроверку возможности выполнения команды.
        /// Полезно вызывать этот метод после изменения состояния ViewModel,
        /// влияющего на доступность команды.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
