using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ProductManagementSystem.WpfApp.ViewModels
{
    /// <summary>
    /// MVVM Pattern - Базовый класс для всех ViewModel.
    /// 
    /// SOLID - S: Класс отвечает только за реализацию механизма уведомления об изменении свойств.
    /// SOLID - O: Класс открыт для расширения (наследование), закрыт для модификации.
    /// 
    /// Реализует интерфейс INotifyPropertyChanged для обеспечения двусторонней привязки данных
    /// между View и ViewModel. Когда свойство изменяется в ViewModel, UI автоматически обновляется.
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Событие, возникающее при изменении значения свойства.
        /// Подписчики (обычно WPF Binding) получают уведомление и обновляют UI.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Вызывает событие PropertyChanged для указанного свойства.
        /// 
        /// Использование CallerMemberName позволяет автоматически получить имя вызывающего свойства,
        /// что упрощает код и предотвращает ошибки при переименовании свойств.
        /// </summary>
        /// <param name="propertyName">Имя свойства, которое изменилось (заполняется автоматически)</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Устанавливает новое значение свойства и вызывает уведомление об изменении.
        /// 
        /// Типичное использование в свойстве:
        /// <code>
        /// private string _name;
        /// public string Name
        /// {
        ///     get => _name;
        ///     set => SetProperty(ref _name, value);
        /// }
        /// </code>
        /// </summary>
        /// <typeparam name="T">Тип свойства</typeparam>
        /// <param name="field">Ссылка на поле, хранящее значение</param>
        /// <param name="value">Новое значение</param>
        /// <param name="propertyName">Имя свойства (заполняется автоматически)</param>
        /// <returns>true, если значение изменилось; иначе false</returns>
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            // Проверяем, действительно ли значение изменилось
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }

            // Устанавливаем новое значение
            field = value;

            // Уведомляем подписчиков об изменении
            OnPropertyChanged(propertyName);

            return true;
        }
    }
}
