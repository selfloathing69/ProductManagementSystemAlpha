using ProductManagementSystem.Model;

namespace ProductManagementSystem.Shared
{
    /// <summary>
    /// MVP паттерн - интерфейс для представления диалога добавления товара.
    /// Определяет методы и события для добавления нового товара.
    /// </summary>
    public interface IAddProductView
    {

        /// <summary>
        /// Возникает, когда пользователь подтверждает добавление товара.
        /// </summary>
        event EventHandler? SaveRequested;

        /// <summary>
        /// Возникает, когда пользователь отменяет операцию.
        /// </summary>
        event EventHandler? CancelRequested;


        /// <summary>
        /// Получает ID товара, введённый пользователем.
        /// </summary>
        int ProductId { get; }

        /// <summary>
        /// Получает название товара, введённое пользователем.
        /// </summary>
        string ProductName { get; }

        /// <summary>
        /// Получает описание товара, введённое пользователем.
        /// </summary>
        string ProductDescription { get; }

        /// <summary>
        /// Получает цену товара, введённую пользователем.
        /// </summary>
        decimal ProductPrice { get; }

        /// <summary>
        /// Получает категорию товара, выбранную пользователем.
        /// </summary>
        string ProductCategory { get; }

        /// <summary>
        /// Получает количество на складе, введённое пользователем.
        /// </summary>
        int StockQuantity { get; }


        /// <summary>
        /// Показывает пользователю сообщение об ошибке.
        /// </summary>
        /// <param name="title">Error title</param>
        /// <param name="message">Error message</param>
        void ShowError(string title, string message);

        /// <summary>
        /// Показывает пользователю информационное сообщение.
        /// </summary>
        /// <param name="title">Message title</param>
        /// <param name="message">Message content</param>
        void ShowMessage(string title, string message);

        /// <summary>
        /// Показывает пользователю диалог подтверждения.
        /// </summary>
        /// <param name="title">Dialog title</param>
        /// <param name="message">Confirmation message</param>
        /// <returns>True if user confirms, false otherwise</returns>
        bool ShowConfirmation(string title, string message);

        /// <summary>
        /// Закрывает диалог с результатом успеха.
        /// </summary>
        void CloseWithSuccess();

        /// <summary>
        /// Закрывает диалог с результатом отмены.
        /// </summary>
        void CloseWithCancel();

        /// <summary>
        /// Устанавливает фокус на конкретное поле.
        /// </summary>
        /// <param name="fieldName">Имя поля для установки фокуса</param>
        void FocusField(string fieldName);

    }
}
