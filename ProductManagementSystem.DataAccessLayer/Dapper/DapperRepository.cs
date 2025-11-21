using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.SqlClient;
using Dapper;
using ProductManagementSystem.Model;
using ProductManagementSystem.Logic;

namespace ProductManagementSystem.DataAccessLayer.Dapper
{
    /// <summary>
    /// Реализация репозитория с использованием Dapper.
    /// Предоставляет CRUD операции для работы с сущностями через Dapper и SQL-запросы.
    /// </summary>
    /// <typeparam name="T">Тип доменного объекта</typeparam>
    public class DapperRepository<T> : IRepository<T> where T : class, IDomainObject
    {
        private readonly string _connectionString;
        private readonly string _tableName;

        /// <summary>
        /// Инициализирует новый экземпляр DapperRepository с настройками по умолчанию.
        /// </summary>
        public DapperRepository()
        {
            // Строка подключения по умолчанию
            _connectionString = "Server=AspireNotebook\\SQLEXPRESS;Database=ProductManagementDB;Integrated Security=True;TrustServerCertificate=True;";
            
            // Определяем имя таблицы на основе типа
            _tableName = typeof(T).Name + "s";
        }

        /// <summary>
        /// Инициализирует новый экземпляр DapperRepository с пользовательской строкой подключения.
        /// </summary>
        /// <param name="connectionString">Строка подключения к базе данных</param>
        public DapperRepository(string connectionString)
        {
            _connectionString = connectionString;
            _tableName = typeof(T).Name + "s";
        }

        /// <summary>
        /// Добавляет новую сущность в базу данных.
        /// </summary>
        /// <param name="entity">Сущность для добавления</param>
        /// <exception cref="Exception">Выбрасывается при ошибке добавления в БД</exception>
        public void Add(T entity)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    
                    // Получаем все свойства, кроме Id (для автоинкремента)
                    // Важно: всегда исключаем Id, даже если он задан
                    var properties = typeof(T).GetProperties()
                        .Where(p => p.Name != "Id" && p.CanWrite && p.CanRead)
                        .ToArray();
                    
                    if (properties.Length == 0)
                    {
                        throw new Exception("Не найдено свойств для вставки");
                    }
                    
                    var propertyNames = string.Join(", ", properties.Select(p => p.Name));
                    var propertyParams = string.Join(", ", properties.Select(p => "@" + p.Name));
                    
                    // Используем более надежный способ получения последнего ID
                    var query = $"INSERT INTO {_tableName} ({propertyNames}) VALUES ({propertyParams}); SELECT CAST(SCOPE_IDENTITY() as int);";
                    
                    // Создаем параметры только для нужных свойств
                    var parameters = new DynamicParameters();
                    foreach (var prop in properties)
                    {
                        parameters.Add("@" + prop.Name, prop.GetValue(entity));
                    }
                    
                    var id = connection.ExecuteScalar<int>(query, parameters);
                    
                    // Устанавливаем полученный ID
                    entity.Id = id;
                }
            }
            catch (SqlException sqlEx)
            {
                // Более детальная информация об ошибке SQL
                throw new Exception($"Ошибка SQL при добавлении записи в таблицу {_tableName}: {sqlEx.Message}", sqlEx);
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
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    
                    var query = $"DELETE FROM {_tableName} WHERE Id = @Id";
                    var rowsAffected = connection.Execute(query, new { Id = id });
                    
                    if (rowsAffected == 0)
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
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    
                    var query = $"SELECT * FROM {_tableName}";
                    return connection.Query<T>(query).ToList();
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
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    
                    var query = $"SELECT * FROM {_tableName} WHERE Id = @Id";
                    return connection.QueryFirstOrDefault<T>(query, new { Id = id });
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
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    
                    // Получаем все свойства, кроме Id
                    var properties = typeof(T).GetProperties()
                        .Where(p => p.Name != "Id" && p.CanWrite && p.CanRead)
                        .ToArray();
                    
                    if (properties.Length == 0)
                    {
                        throw new Exception("Не найдено свойств для обновления");
                    }
                    
                    var setClause = string.Join(", ", properties.Select(p => $"{p.Name} = @{p.Name}"));
                    
                    var query = $"UPDATE {_tableName} SET {setClause} WHERE Id = @Id";
                    
                    // Создаем параметры для всех свойств включая Id
                    var parameters = new DynamicParameters();
                    parameters.Add("@Id", entity.Id);
                    foreach (var prop in properties)
                    {
                        parameters.Add("@" + prop.Name, prop.GetValue(entity));
                    }
                    
                    var rowsAffected = connection.Execute(query, parameters);
                    
                    if (rowsAffected == 0)
                    {
                        throw new Exception($"Запись с ID={entity.Id} не найдена");
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception($"Ошибка SQL при обновлении записи с ID={entity.Id} в таблице {_tableName}: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при обновлении записи с ID={entity.Id}: {ex.Message}", ex);
            }
        }
    }
}
