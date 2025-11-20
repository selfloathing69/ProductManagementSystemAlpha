namespace ProductManagementSystem.Model
{
    /// <summary>
    /// Базовый интерфейс для всех доменных объектов в системе.
    /// Определяет обязательное наличие уникального идентификатора.
    /// </summary>
    public interface IDomainObject
    {
        /// <summary>
        /// Уникальный идентификатор объекта в базе данных.
        /// </summary>
        int Id { get; set; }
    }
}
