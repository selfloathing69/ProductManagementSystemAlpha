using ProductManagementSystem.Model;

namespace ProductManagementSystem.Logic
{
    /// <summary>
    /// Интерфейс репозитория для работы с доменными объектами.
    /// Определяет стандартный набор CRUD операций для доступа к данным.
    /// </summary>
    /// <typeparam name="T">Тип доменного объекта, должен реализовывать IDomainObject</typeparam>
    public interface IRepository<T> where T : IDomainObject
    {
        /// <summary>
        /// Добавляет новую сущность в хранилище данных.
        /// </summary>
        /// <param name="entity">Сущность для добавления</param>
        void Add(T entity);

        /// <summary>
        /// Удаляет сущность из хранилища данных по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор сущности для удаления</param>
        void Delete(int id);

        /// <summary>
        /// Получает все сущности из хранилища данных.
        /// </summary>
        /// <returns>Коллекция всех сущностей</returns>
        IEnumerable<T> ReadAll();

        /// <summary>
        /// Получает сущность по её идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор сущности</param>
        /// <returns>Сущность с указанным идентификатором или null, если не найдена</returns>
        T? ReadById(int id);

        /// <summary>
        /// Обновляет существующую сущность в хранилище данных.
        /// </summary>
        /// <param name="entity">Сущность с обновлёнными данными</param>
        void Update(T entity);
    }
}
