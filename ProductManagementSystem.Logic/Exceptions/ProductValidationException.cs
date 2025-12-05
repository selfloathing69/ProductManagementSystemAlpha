using System;

namespace ProductManagementSystem.Logic.Exceptions
{
    /// <summary>
    /// Исключение, выбрасываемое при ошибках валидации товара.
    /// </summary>
    public class ProductValidationException : Exception
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса ProductValidationException.
        /// </summary>
        public ProductValidationException() : base()
        {
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса ProductValidationException с указанным сообщением.
        /// </summary>
        /// <param name="message">Сообщение об ошибке</param>
        public ProductValidationException(string message) : base(message)
        {
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса ProductValidationException с указанным сообщением и внутренним исключением.
        /// </summary>
        /// <param name="message">Сообщение об ошибке</param>
        /// <param name="innerException">Внутреннее исключение</param>
        public ProductValidationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
