namespace ProductManagementSystem.Shared
{
    /// <summary>
    /// Шаблон MVP — интерфейс для диалогового окна «Добавить продукт».
    /// Определяет методы и события для добавления нового продукта.
    /// View работает с примитивными типами данных, не имеет зависимости от Model.
    /// </summary>
    public interface IAddProductView
    {
        /// <summary>
        /// Срабатывает, когда пользователь подтверждает добавление продукта.
        /// </summary>
        event EventHandler? SaveRequested;

        /// <summary>
        /// Срабатывает, когда пользователь отменяет операцию.
        /// </summary>
        event EventHandler? CancelRequested;

        /// <summary>
        /// Получает идентификатор продукта, введенный пользователем.
        /// </summary>
        int ProductId { get; }

        /// <summary>
        /// Получает название продукта, введенное пользователем.
        /// </summary>
        string ProductName { get; }

        /// <summary>
        /// Получает описание продукта, введенное пользователем.
        /// </summary>
        string ProductDescription { get; }

        /// <summary>
        /// Получает цену товара, введенную пользователем.
        /// </summary>
        decimal ProductPrice { get; }

        /// <summary>
        /// Получает категорию продукта, выбранную пользователем.
        /// </summary>
        string ProductCategory { get; }

        /// <summary>
        /// Возвращает количество товара на складе, введенное пользователем.
        /// </summary>
        int StockQuantity { get; }




        /// <summary>
        /// Показывает пользователю сообщение об ошибке.
        /// </summary>
        /// <param name="title">Заголовок ошибки</param>
        /// <param name="message">Сообщение об ошибке</param>
        void ShowError(string title, string message);

        /// <summary>
        /// Показывает информационное сообщение пользователю.
        /// </summary>
        /// <param name="title">Заголовок сообщения</param>
        /// <param name="message">Содержание сообщения</param>
        void ShowMessage(string title, string message);

        /// <summary>
        /// Показывает пользователю диалоговое окно подтверждения.
        /// </summary>
        /// <param name="title">Заголовок диалогового окна</param>
        /// <param name="message">Сообщение подтверждения</param>
        /// <returns>True, если пользователь подтверждает, false в противном случае</returns>
      
        
        bool ShowConfirmation(string title, string message);

        /// <summary>
        /// Закрывает диалоговое окно с успешным результатом.
        /// </summary>
     
        
        void CloseWithSuccess();

        /// <summary>
        /// Закрывает диалоговое окно с результатом отмены.
        /// </summary>
     
                
        
        void CloseWithCancel();

        /// <summary>
        /// Устанавливает фокус на определенное поле.
        /// </summary>
        /// <param name="fieldName">Name of the field to focus</param>
        void FocusField(string fieldName);

    }
}
