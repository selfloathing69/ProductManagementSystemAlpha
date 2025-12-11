namespace ProductManagementSystem.Shared
{
    /// <summary>
    /// Шаблон MVP — Интерфейс просмотра. 
    /// Интерфейс для представлений управления продуктом, позволяющий избежать циклических зависимостей. 
    /// Определяет методы обновления пользовательского интерфейса и события для действий пользователя. 
    /// Просмотр работает с объектами DTO, не зависит от модели.
    /// </summary>
    public interface IProductView
    {
        #region Events Пользв дейст
        /// <summary>
        /// Срабатывает, когда пользователь запрашивает обновление списка товаров.
        /// </summary>
        event EventHandler? RefreshRequested;

        /// <summary>
        /// Срабатывает, когда пользователь запрашивает добавление нового продукта.
        /// </summary>
        event EventHandler? AddProductRequested;

        /// <summary>
        /// Срабатывает, когда пользователь запрашивает удаление продукта.
        /// </summary>
        event EventHandler<int>? DeleteProductRequested;

        /// <summary>
        /// Срабатывает, когда пользователь запрашивает удаление определенного количества товара.
        /// </summary>
        event EventHandler<(int ProductId, int Quantity)>? DeleteProductByQuantityRequested;

        #endregion


        /// <summary>
        /// Отображает список товаров в представлении.
        /// </summary>
        /// <param name="products">Список DTO товаров для отображения</param>
        void ShowProducts(IEnumerable<ProductDto> products);

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
        /// Получает идентификатор текущего выбранного товара.
        /// </summary>
        /// <returns>Идентификатор выбранного товара или null, если ничего не выбрано</returns>
        int? GetSelectedProductId();

        /// <summary>
        /// Отображает диалоговое окно удаления по количеству.
        /// </summary>
        /// <param name="products">Список товаров с индексами для выбора</param>
        /// <returns>Кортеж с идентификатором выбранного товара и количеством, или null в случае отмены</returns>
        (int ProductId, int Quantity)? ShowDeleteByQuantityDialog(IEnumerable<(int Index, ProductDto Product)> products);

    }
}
