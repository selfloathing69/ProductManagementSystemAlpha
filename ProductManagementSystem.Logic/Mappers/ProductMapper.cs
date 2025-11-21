using ProductManagementSystem.Model;

namespace ProductManagementSystem.Logic.Mappers
{
    /// <summary>
    /// SOLID - S: Класс отвечает только за преобразование между Product и ProductModel. 
    /// 
    /// Класс для маппинга между доменной сущностью Product и моделью ProductModel.
    /// Обеспечивает преобразование данных между слоями приложения.
    /// </summary>
    public class ProductMapper
    {
        /// <summary>
        /// Преобразует ProductModel в доменную сущность Product.
        /// </summary>
        /// <param name="model">Модель для преобразования</param>
        /// <returns>Доменная сущность Product</returns>
        public Product ToEntity(ProductModel model)
        {
            return new Product
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                Category = model.Category,
                StockQuantity = model.StockQuantity
            };
        }

        /// <summary>
        /// Преобразует доменную сущность Product в ProductModel.
        /// </summary>
        /// <param name="entity">Доменная сущность для преобразования</param>
        /// <returns>Модель ProductModel</returns>
        public ProductModel ToModel(Product entity)
        {
            return new ProductModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                Price = entity.Price,
                Category = entity.Category,
                StockQuantity = entity.StockQuantity
            };
        }

        /// <summary>
        /// Преобразует список доменных сущностей Product в список моделей ProductModel.
        /// </summary>
        /// <param name="entities">Список доменных сущностей</param>
        /// <returns>Список моделей ProductModel</returns>
        public List<ProductModel> ToModelList(IEnumerable<Product> entities)
        {
            return entities.Select(ToModel).ToList();
        }

        /// <summary>
        /// Преобразует список моделей ProductModel в список доменных сущностей Product.
        /// </summary>
        /// <param name="models">Список моделей</param>
        /// <returns>Список доменных сущностей Product</returns>
        public List<Product> ToEntityList(IEnumerable<ProductModel> models)
        {
            return models.Select(ToEntity).ToList();
        }
    }
}
