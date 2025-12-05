namespace ProductManagementSystem.Model
{
    /// <summary>
    /// Базовый интерфейс для всех объектов в БД.
    /// Определяет обязательное наличие уникального идентификатора.
    /// </summary>
    public interface IDomainObject
    {
        /// <summary>
        /// Уникальный идентификатор объекта в Базе Данных.
        /// </summary>
        int Id { get; set; }
    }
}
