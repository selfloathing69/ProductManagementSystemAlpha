using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ProductManagementSystem.Shared
{
    /// <summary>
    /// Data Transfer Object для товара с поддержкой уведомлений об изменениях.
    /// Используется для передачи данных между слоями приложения без зависимости от Model.
    /// 
    /// MVVM Pattern: DTO реализует INotifyPropertyChanged для двусторонней привязки данных.
    /// View автоматически обновляется при изменении свойств DTO в ViewModel.
    /// </summary>
    public class ProductDto : INotifyPropertyChanged // =============== DTO дата трасфер обджект
    {
        private int _id;
        private string _name = string.Empty;
        private string _description = string.Empty;
        private decimal _price;
        private string _category = string.Empty;
        private int _stockQuantity;

        /// <summary>
        /// Уникальный идентификатор товара.
        /// </summary>
        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        /// <summary>
        /// Название товара.
        /// </summary>
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        /// <summary>
        /// Описание товара.
        /// </summary>
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        /// <summary>
        /// Цена товара за единицу.
        /// </summary>
        public decimal Price
        {
            get => _price;
            set => SetProperty(ref _price, value);
        }

        /// <summary>
        /// Категория товара.
        /// </summary>
        public string Category
        {
            get => _category;
            set => SetProperty(ref _category, value);
        }

        /// <summary>
        /// Количество товара на складе.
        /// </summary>
        public int StockQuantity
        {
            get => _stockQuantity;
            set => SetProperty(ref _stockQuantity, value);
        }

        /// <summary>
        /// Событие, возникающее при изменении значения свойства.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Инициализирует новый экземпляр класса ProductDto с пустыми значениями.
        /// </summary>
        public ProductDto() { }

        /// <summary>
        /// Инициализирует новый экземпляр класса ProductDto с указанными параметрами.
        /// </summary>
        public ProductDto(int id, string name, string description, decimal price, string category, int stockQuantity)
        {
            _id = id;
            _name = name;
            _description = description;
            _price = price;
            _category = category;
            _stockQuantity = stockQuantity;
        }

        /// <summary>
        /// Возвращает строковое представление товара.
        /// </summary>
        public override string ToString()
        {
            return $"ID: {Id}, Название: {Name}, Цена: {Price} руб., Категория: {Category}, Количество: {StockQuantity}";
        }

        /// <summary>
        /// Вызывает событие PropertyChanged для указанного свойства.
        /// </summary>
        /// <param name="propertyName">Имя свойства, которое изменилось</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Устанавливает новое значение свойства и вызывает уведомление об изменении.
        /// </summary>
        /// <typeparam name="T">Тип свойства</typeparam>
        /// <param name="field">Ссылка на поле, хранищее значение</param>
        /// <param name="value">Новое значение</param>
        /// <param name="propertyName">Имя свойства</param>
        /// <returns>true, если значение изменилось; иначе false</returns>
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}