using ProductManagementSystem.Model;

namespace ProductManagementSystem.Shared
{
    /// <summary>
    /// MVP паттерн - интерфейс представлени€.
    /// »нтерфейс дл€ представлений управлени€ товарами дл€ избежани€ циклических зависимостей.
    /// ќпредел€ет методы дл€ обновлени€ UI и событи€ дл€ действий пользовател€.
    /// </summary>
    public interface IProductView
    {

        /// <summary>
        /// ¬озникает, когда пользователь запрашивает обновление списка товаров.
        /// </summary>
        event EventHandler? RefreshRequested;

        /// <summary>
        /// ¬озникает, когда пользователь запрашивает добавление нового товара.
        /// </summary>
        event EventHandler? AddProductRequested;

        /// <summary>
        /// ¬озникает, когда пользователь запрашивает удаление товара.
        /// </summary>
        event EventHandler<int>? DeleteProductRequested;

        /// <summary>
        /// ¬озникает, когда пользователь запрашивает удаление определЄнного количества товара.
        /// </summary>
        event EventHandler<(int ProductId, int Quantity)>? DeleteProductByQuantityRequested;

        /// <summary>
        /// ќтображает список товаров в представлении.
        /// </summary>
        /// <param name="products">—писок товаров дл€ отображени€</param>
        void ShowProducts(IEnumerable<Product> products);

        /// <summary>
        /// ѕоказывает пользователю сообщение об ошибке.
        /// </summary>
        /// <param name="title">«аголовок ошибки</param>
        /// <param name="message">—ообщение об ошибке</param>
        void ShowError(string title, string message);

        /// <summary>
        /// ѕоказывает пользователю информационное сообщение.
        /// </summary>
        /// <param name="title">«аголовок сообщени€</param>
        /// <param name="message">—одержимое сообщени€</param>
        void ShowMessage(string title, string message);

        /// <summary>
        /// ѕоказывает пользователю диалог подтверждени€.
        /// </summary>
        /// <param name="title">«аголовок диалога</param>
        /// <param name="message">—ообщение подтверждени€</param>
        /// <returns>True, если пользователь подтверждает, иначе false</returns>
        bool ShowConfirmation(string title, string message);

        /// <summary>
        /// ѕолучает ID текущего выбранного товара.
        /// </summary>
        /// <returns>ID выбранного товара или null, если нет выбора</returns>
        int? GetSelectedProductId();
    }
}
