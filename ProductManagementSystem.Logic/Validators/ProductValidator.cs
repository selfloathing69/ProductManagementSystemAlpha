using System;
using System.Text.RegularExpressions;
using ProductManagementSystem.Logic.Exceptions;

namespace ProductManagementSystem.Logic.Validators
{
    /// <summary>
    /// SOLID - S: Класс отвечает только за валидацию данных товара.
    /// 
    /// Валидатор для проверки корректности данных товара.
    /// Выполняет проверки на соответствие бизнес-правилам и ограничениям.
    /// </summary>
    public class ProductValidator
    {
        private const int MaxNameLength = 200;
        private const int MinNameLength = 1;
        private const decimal MinPrice = 0.01m;
        private const decimal MaxPrice = 99999999.99m;
        private const int MinQuantity = 0;
        private const int MaxQuantity = 1000000;

        /// <summary>
        /// Проверяет название товара на корректность.
        /// Название не должно содержать недопустимые специальные символы и должно иметь допустимую длину.
        /// </summary>
        /// <param name="name">Название для проверки</param>
        /// <exception cref="ProductValidationException">Выбрасывается при некорректном названии</exception>
        public void ValidateProductName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ProductValidationException("Название товара не может быть пустым.");
            }

            if (name.Length < MinNameLength || name.Length > MaxNameLength)
            {
                throw new ProductValidationException($"Длина названия должна быть от {MinNameLength} до {MaxNameLength} символов.");
            }

            // Проверка на недопустимые специальные символы (разрешены буквы, цифры, пробелы и базовая пунктуация)
            if (Regex.IsMatch(name, @"[<>{}[\]\\|`~]"))
            {
                throw new ProductValidationException("Название содержит недопустимые специальные символы.");
            }
        }

        /// <summary>
        /// Проверяет текст на использование только латиницы или только кириллицы.
        /// Не допускается смешивание алфавитов в одном тексте.
        /// </summary>
        /// <param name="text">Текст для проверки</param>
        /// <exception cref="ProductValidationException">Выбрасывается при смешивании алфавитов</exception>
        public void ValidateLanguage(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return; // Пустой текст допускается
            }

            bool hasCyrillic = Regex.IsMatch(text, @"[А-Яа-яЁё]");
            bool hasLatin = Regex.IsMatch(text, @"[A-Za-z]");

            if (hasCyrillic && hasLatin)
            {
                throw new ProductValidationException("Текст не должен содержать одновременно латиницу и кириллицу.");
            }
        }

        /// <summary>
        /// Проверяет корректность цены товара.
        /// Цена должна быть положительной и не превышать максимальное значение.
        /// </summary>
        /// <param name="price">Цена для проверки</param>
        /// <exception cref="ProductValidationException">Выбрасывается при некорректной цене</exception>
        public void ValidatePrice(decimal price)
        {
            if (price < MinPrice)
            {
                throw new ProductValidationException($"Цена должна быть не меньше {MinPrice}.");
            }

            if (price > MaxPrice)
            {
                throw new ProductValidationException($"Цена не должна превышать {MaxPrice}.");
            }
        }

        /// <summary>
        /// Проверяет корректность количества товара на складе.
        /// Количество должно быть неотрицательным и не превышать максимальное значение.
        /// </summary>
        /// <param name="quantity">Количество для проверки</param>
        /// <exception cref="ProductValidationException">Выбрасывается при некорректном количестве</exception>
        public void ValidateQuantity(int quantity)
        {
            if (quantity < MinQuantity)
            {
                throw new ProductValidationException($"Количество не может быть меньше {MinQuantity}.");
            }

            if (quantity > MaxQuantity)
            {
                throw new ProductValidationException($"Количество не должно превышать {MaxQuantity}.");
            }
        }

        /// <summary>
        /// Выполняет комплексную валидацию модели товара.
        /// Проверяет все поля на соответствие бизнес-правилам.
        /// </summary>
        /// <param name="model">Модель товара для проверки</param>
        /// <exception cref="ProductValidationException">Выбрасывается при любой ошибке валидации</exception>
        public void ValidateProductModel(ProductModel model)
        {
            if (model == null)
            {
                throw new ProductValidationException("Модель товара не может быть null.");
            }

            // Валидация названия
            ValidateProductName(model.Name);
            ValidateLanguage(model.Name);

            // Валидация описания
            if (!string.IsNullOrWhiteSpace(model.Description))
            {
                ValidateLanguage(model.Description);
            }

            // Валидация категории
            if (!string.IsNullOrWhiteSpace(model.Category))
            {
                ValidateLanguage(model.Category);
            }

            // Валидация цены
            ValidatePrice(model.Price);

            // Валидация количества
            ValidateQuantity(model.StockQuantity);
        }
    }
}
