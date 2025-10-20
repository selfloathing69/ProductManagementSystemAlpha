using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ProductManagementSystem.Model;

namespace ProductManagementSystem.DataAccessLayer.EF
{
    /// <summary>
    /// Реализация репозитория с использованием Entity Framework.
    /// Предоставляет CRUD операции для работы с сущностями через EF.
    /// </summary>
    /// <typeparam name="T">Тип доменного объекта</typeparam>
    public class EntityRepository<T> : IRepository<T> where T : class, IDomainObject
    {
        /// <summary>
        /// Добавляет новую сущность в базу данных.
        /// </summary>
        /// <param name="entity">Сущность для добавления</param>
        /// <exception cref="Exception">Выбрасывается при ошибке добавления в БД</exception>
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
        /// Удаляет сущность из базы данных по её идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор сущности для удаления</param>
        /// <exception cref="Exception">Выбрасывается если запись не найдена или при ошибке удаления</exception>
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
                throw new Exception($"Ошибка при удалении записи с ID={id}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Получает все сущности из базы данных.
        /// </summary>
        /// <returns>Коллекция всех сущностей</returns>
        /// <exception cref="Exception">Выбрасывается при ошибке чтения из БД</exception>
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
        /// Получает сущность из базы данных по её идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор сущности</param>
        /// <returns>Сущность с указанным идентификатором или null, если не найдена</returns>
        /// <exception cref="Exception">Выбрасывается при ошибке чтения из БД</exception>
        public T? ReadById(int id)
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    return context.Set<T>().Find(id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при чтении записи с ID={id}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Обновляет существующую сущность в базе данных.
        /// </summary>
        /// <param name="entity">Сущность с обновлёнными данными</param>
        /// <exception cref="Exception">Выбрасывается если запись не найдена или при ошибке обновления</exception>
        public void Update(T entity)
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    var existing = context.Set<T>().Find(entity.Id);
                    if (existing != null)
                    {
                        context.Entry(existing).CurrentValues.SetValues(entity);
                        context.SaveChanges();
                    }
                    else
                    {
                        throw new Exception($"Запись с ID={entity.Id} не найдена");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при обновлении записи с ID={entity.Id}: {ex.Message}", ex);
            }
        }
    }
}
