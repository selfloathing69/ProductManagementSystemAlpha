using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using ProductManagementSystem.Model;

namespace ProductManagementSystem.DataAccessLayer.Dapper
{
    /// <summary>
    /// Реализация репозитория для работы с Dapper.
    /// Предоставляет CRUD операции через Dapper ORM.
    /// </summary>
    /// <typeparam name="T">Тип сущности, которая реализует IDomainObject</typeparam>
    public class DapperRepository<T> : IRepository<T> where T : class, IDomainObject
    {
        private readonly string _connectionString;
        private readonly string _tableName;

        /// <summary>
        /// Инициализирует новый экземпляр репозитория Dapper.
        /// </summary>
        public DapperRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            _tableName = typeof(T).Name + "s"; // Например: Product -> Products
        }

        /// <summary>
        /// Создает подключение к базе данных.
        /// </summary>
        /// <returns>Объект подключения к БД</returns>
        private IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        /// <summary>
        /// Добавляет новую сущность в базу данных.
        /// </summary>
        /// <param name="entity">Сущность для добавления</param>
        public void Add(T entity)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    // SQL генерируется на основе свойств модели
                    var properties = typeof(T).GetProperties()
                        .Where(p => p.Name != "Id")
                        .ToList();
                    
                    var columns = string.Join(", ", properties.Select(p => p.Name));
                    var values = string.Join(", ", properties.Select(p => "@" + p.Name));
                    
                    var sql = $"INSERT INTO {_tableName} ({columns}) VALUES ({values})";
                    connection.Execute(sql, entity);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при добавлении записи через Dapper: {ex.Message}", ex);
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
                using (var connection = CreateConnection())
                {
                    var sql = $"DELETE FROM {_tableName} WHERE Id = @Id";
                    var affectedRows = connection.Execute(sql, new { Id = id });
                    
                    if (affectedRows == 0)
                    {
                        throw new Exception($"Запись с ID={id} не найдена");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при удалении записи через Dapper: {ex.Message}", ex);
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
                using (var connection = CreateConnection())
                {
                    var sql = $"SELECT * FROM {_tableName}";
                    return connection.Query<T>(sql).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при чтении всех записей через Dapper: {ex.Message}", ex);
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
                using (var connection = CreateConnection())
                {
                    var sql = $"SELECT * FROM {_tableName} WHERE Id = @Id";
                    var entity = connection.QueryFirstOrDefault<T>(sql, new { Id = id });
                    
                    if (entity == null)
                    {
                        throw new Exception($"Запись с ID={id} не найдена");
                    }
                    
                    return entity;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при чтении записи через Dapper: {ex.Message}", ex);
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
                using (var connection = CreateConnection())
                {
                    var properties = typeof(T).GetProperties()
                        .Where(p => p.Name != "Id")
                        .ToList();
                    
                    var setClause = string.Join(", ", properties.Select(p => $"{p.Name} = @{p.Name}"));
                    var sql = $"UPDATE {_tableName} SET {setClause} WHERE Id = @Id";
                    
                    var affectedRows = connection.Execute(sql, entity);
                    
                    if (affectedRows == 0)
                    {
                        throw new Exception($"Запись с ID={entity.Id} не найдена");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при обновлении записи через Dapper: {ex.Message}", ex);
            }
        }
    }
}
