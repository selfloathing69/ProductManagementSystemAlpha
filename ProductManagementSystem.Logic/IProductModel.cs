using ProductManagementSystem.Model;

namespace ProductManagementSystem.Logic
{
    /// <summary>
    /// MVP Pattern - Model Interface (Business Logic Facade).
    /// Defines the contract for the Model layer to avoid circular dependencies.
    /// The Model raises events when data changes, which the Presenter can listen to.
    /// Model работает с доменными объектами Product, Presenter преобразует их в DTO для View.
    /// </summary>
    public interface IProductModel
    {
        #region Events - Data Change Notifications

        /// <summary>
        /// Raised when the products list has been modified (add/update/delete).
        /// </summary>
        event EventHandler? ProductsChanged;

        /// <summary>
        /// Raised when an error occurs in the Model.
        /// </summary>
        event EventHandler<string>? ErrorOccurred;

        #endregion

        #region CRUD Operations

        /// <summary>
        /// Gets all products.
        /// </summary>
        /// <returns>List of all products</returns>
        List<Product> GetAllProducts();

        /// <summary>
        /// Gets a product by its ID.
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>Product or null if not found</returns>
        Product? GetProductById(int id);

        /// <summary>
        /// Adds a new product.
        /// </summary>
        /// <param name="product">Product to add</param>
        /// <returns>The added product with assigned ID</returns>
        Product AddProduct(Product product);

        /// <summary>
        /// Updates an existing product.
        /// </summary>
        /// <param name="product">Product with updated data</param>
        /// <returns>True if update succeeded, false otherwise</returns>
        bool UpdateProduct(Product product);

        /// <summary>
        /// Deletes a product by ID.
        /// </summary>
        /// <param name="id">Product ID to delete</param>
        /// <returns>True if deletion succeeded, false otherwise</returns>
        bool DeleteProduct(int id);

        #endregion

        #region Business Functions

        /// <summary>
        /// Filters products by category.
        /// </summary>
        /// <param name="category">Category to filter by</param>
        /// <returns>List of products in the specified category</returns>
        List<Product> FilterByCategory(string category);

        /// <summary>
        /// Groups products by category.
        /// </summary>
        /// <returns>Dictionary of categories to product lists</returns>
        Dictionary<string, List<Product>> GroupByCategory();

        /// <summary>
        /// Calculates the total inventory value.
        /// </summary>
        /// <returns>Total value of all products (price * quantity)</returns>
        decimal CalculateTotalInventoryValue();

        /// <summary>
        /// Finds a product by name and category.
        /// </summary>
        /// <param name="name">Product name</param>
        /// <param name="category">Product category</param>
        /// <returns>Product if found, null otherwise</returns>
        Product? FindProductByNameAndCategory(string name, string category);

        /// <summary>
        /// Searches products by query string.
        /// </summary>
        /// <param name="query">Search query</param>
        /// <returns>List of matching products</returns>
        List<Product> SearchProducts(string query);

        /// <summary>
        /// Adds quantity to an existing product.
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="quantity">Quantity to add</param>
        /// <returns>True if operation succeeded</returns>
        bool AddQuantityToProduct(int id, int quantity);

        /// <summary>
        /// Removes quantity from a product. If quantity to remove >= stock, deletes the product.
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="quantityToRemove">Quantity to remove</param>
        /// <returns>True if operation succeeded</returns>
        bool RemoveQuantityFromProduct(int id, int quantityToRemove);

        /// <summary>
        /// Checks if a product with the specified ID exists.
        /// </summary>
        /// <param name="id">Product ID to check</param>
        /// <returns>True if product exists</returns>
        bool ProductExists(int id);

        /// <summary>
        /// Gets products with their display indexes.
        /// </summary>
        /// <returns>List of tuples with index and product</returns>
        List<(int Index, Product Product)> GetProductsWithIndexes();

        #endregion
    }
}
