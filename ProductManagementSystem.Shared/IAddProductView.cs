namespace ProductManagementSystem.Shared
{
    /// <summary>
    /// MVP Pattern - Interface for Add Product dialog view.
    /// Defines methods and events for adding a new product.
    /// View работает с примитивными типами данных, не имеет зависимости от Model.
    /// </summary>
    public interface IAddProductView
    {
        #region Events - User Actions

        /// <summary>
        /// Fired when user confirms adding a product.
        /// </summary>
        event EventHandler? SaveRequested;

        /// <summary>
        /// Fired when user cancels the operation.
        /// </summary>
        event EventHandler? CancelRequested;

        #endregion

        #region Properties - Form Data

        /// <summary>
        /// Gets the product ID entered by the user.
        /// </summary>
        int ProductId { get; }

        /// <summary>
        /// Gets the product name entered by the user.
        /// </summary>
        string ProductName { get; }

        /// <summary>
        /// Gets the product description entered by the user.
        /// </summary>
        string ProductDescription { get; }

        /// <summary>
        /// Gets the product price entered by the user.
        /// </summary>
        decimal ProductPrice { get; }

        /// <summary>
        /// Gets the product category selected by the user.
        /// </summary>
        string ProductCategory { get; }

        /// <summary>
        /// Gets the stock quantity entered by the user.
        /// </summary>
        int StockQuantity { get; }

        #endregion

        #region Methods - UI Updates

        /// <summary>
        /// Shows an error message to the user.
        /// </summary>
        /// <param name="title">Error title</param>
        /// <param name="message">Error message</param>
        void ShowError(string title, string message);

        /// <summary>
        /// Shows an information message to the user.
        /// </summary>
        /// <param name="title">Message title</param>
        /// <param name="message">Message content</param>
        void ShowMessage(string title, string message);

        /// <summary>
        /// Shows a confirmation dialog to the user.
        /// </summary>
        /// <param name="title">Dialog title</param>
        /// <param name="message">Confirmation message</param>
        /// <returns>True if user confirms, false otherwise</returns>
        bool ShowConfirmation(string title, string message);

        /// <summary>
        /// Closes the dialog with success result.
        /// </summary>
        void CloseWithSuccess();

        /// <summary>
        /// Closes the dialog with cancel result.
        /// </summary>
        void CloseWithCancel();

        /// <summary>
        /// Sets focus to a specific field.
        /// </summary>
        /// <param name="fieldName">Name of the field to focus</param>
        void FocusField(string fieldName);

        #endregion
    }
}
