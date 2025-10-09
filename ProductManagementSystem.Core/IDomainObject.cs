namespace ProductManagementSystem.Model
{
    /// <summary>
    /// Базовый интерфейс для всех доменных объектов.
    /// Обеспечивает наличие уникального идентификатора у всех сущностей.
    /// </summary>
    public interface IDomainObject
    {
        /// <summary>
        /// Уникальный идентификатор объекта.
        /// </summary>
        int Id { get; set; }
    }
}
