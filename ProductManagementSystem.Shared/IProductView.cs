namespace ProductManagementSystem.Shared
{
    /// <summary>
    /// MVP Pattern - View Interface.
    /// Interface for product management views to avoid circular dependencies.
    /// Defines methods for updating the UI and events for user actions.
    /// View работает с DTO объектами, не имеет зависимости от Model.
    /// </summary>
    public interface IProductView
    {
        #region Events - User Actions

        /// <summary>
        /// Fired when user requests to refresh the product list.
        /// </summary>
        event EventHandler? RefreshRequested;

        /// <summary>
        /// Fired when user requests to add a new product.
        /// </summary>
        event EventHandler? AddProductRequested;

        /// <summary>
        /// Fired when user requests to delete a product.
        /// </summary>
        event EventHandler<int>? DeleteProductRequested;

        /// <summary>
        /// Fired when user requests to delete a specific quantity of a product.
        /// </summary>
        event EventHandler<(int ProductId, int Quantity)>? DeleteProductByQuantityRequested;

        #endregion

        #region Methods - UI Updates

        /// <summary>
        /// Displays the list of products in the view.
        /// </summary>
        /// <param name="products">List of product DTOs to display</param>
        void ShowProducts(IEnumerable<ProductDto> products);

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
        /// Gets the ID of the currently selected product.
        /// </summary>
        /// <returns>Selected product ID or null if no selection</returns>
        int? GetSelectedProductId();

        /// <summary>
        /// Shows the delete by quantity dialog.
        /// </summary>
        /// <param name="products">List of products with indexes for selection</param>
        /// <returns>Tuple with selected product ID and quantity, or null if cancelled</returns>
        (int ProductId, int Quantity)? ShowDeleteByQuantityDialog(IEnumerable<(int Index, ProductDto Product)> products);

        #endregion
    }
}
