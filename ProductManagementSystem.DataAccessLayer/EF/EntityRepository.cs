using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ProductManagementSystem.Model;

namespace ProductManagementSystem.DataAccessLayer.EF
{
    /// <summary>
    /// Реализация репозитория для работы с Entity Framework.
    /// Предоставляет CRUD операции через Entity Framework.
    /// </summary>
    /// <typeparam name="T">Тип сущности, которая реализует IDomainObject</typeparam>
    public class EntityRepository<T> : IRepository<T> where T : class, IDomainObject
    {
        /// <summary>
        /// Добавляет новую сущность в базу данных.
        /// </summary>
        /// <param name="entity">Сущность для добавления</param>
        public void Add(T entity)
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    context.Set<T>().Add(entity);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при добавлении записи: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Удаляет сущность по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор сущности для удаления</param>
        public void Delete(int id)
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    var entity = context.Set<T>().Find(id);
                    if (entity != null)
                    {
                        context.Set<T>().Remove(entity);
                        context.SaveChanges();
                    }
                    else
                    {
                        throw new Exception($"Запись с ID={id} не найдена");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при удалении записи: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Получает все сущности из базы данных.
        /// </summary>
        /// <returns>Коллекция всех сущностей</returns>
        public IEnumerable<T> ReadAll()
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    return context.Set<T>().ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при чтении всех записей: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Получает сущность по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор сущности</param>
        /// <returns>Сущность с указанным идентификатором</returns>
        public T ReadById(int id)
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    var entity = context.Set<T>().Find(id);
                    if (entity == null)
                    {
                        throw new Exception($"Запись с ID={id} не найдена");
                    }
                    return entity;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при чтении записи: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Обновляет существующую сущность в базе данных.
        /// </summary>
        /// <param name="entity">Сущность с обновленными данными</param>
        public void Update(T entity)
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    context.Entry(entity).State = EntityState.Modified;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при обновлении записи: {ex.Message}", ex);
            }
        }
    }
}
