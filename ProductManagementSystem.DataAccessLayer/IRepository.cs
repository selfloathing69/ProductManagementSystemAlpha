using ProductManagementSystem.Model;

namespace ProductManagementSystem.DataAccessLayer
{
    /// <summary>
    /// Базовый интерфейс репозитория для работы с сущностями.
    /// Определяет стандартные CRUD операции.
    /// </summary>
    /// <typeparam name="T">Тип сущности, которая реализует IDomainObject</typeparam>
    public interface IRepository<T> where T : IDomainObject
    {
        /// <summary>
        /// Добавляет новую сущность в хранилище.
        /// </summary>
        /// <param name="entity">Сущность для добавления</param>
        void Add(T entity);

        /// <summary>
        /// Удаляет сущность по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор сущности для удаления</param>
        void Delete(int id);

        /// <summary>
        /// Получает все сущности из хранилища.
        /// </summary>
        /// <returns>Коллекция всех сущностей</returns>
        IEnumerable<T> ReadAll();

        /// <summary>
        /// Получает сущность по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор сущности</param>
        /// <returns>Сущность с указанным идентификатором</returns>
        T ReadById(int id);

        /// <summary>
        /// Обновляет существующую сущность.
        /// </summary>
        /// <param name="entity">Сущность с обновленными данными</param>
        void Update(T entity);
    }
}
