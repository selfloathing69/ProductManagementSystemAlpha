using System;
using System.Globalization;
using System.Windows.Data;

namespace ProductManagementSystem.WpfApp.Converters
{
    /// <summary>
    /// MVVM Pattern - Value Converter для форматирования валюты в российские рубли.
    /// 
    /// SOLID - S: Класс отвечает только за преобразование числовых значений в строку с форматированием рублей.
    /// 
    /// Конвертер преобразует decimal значения в строку формата "100 000.00 ₽":
    /// - Разделение тысяч пробелами
    /// - Точка для отделения копеек
    /// - Знак рубля после суммы
    /// </summary>
    public class RubleCurrencyConverter : IValueConverter
    {
        /// <summary>
        /// Преобразует decimal значение в строку с форматированием рублей.
        /// </summary>
        /// <param name="value">Числовое значение (decimal)</param>
        /// <param name="targetType">Целевой тип (String)</param>
        /// <param name="parameter">Дополнительный параметр (не используется)</param>
        /// <param name="culture">Культура (не используется, используем кастомное форматирование)</param>
        /// <returns>Строка формата "100 000.00 ₽"</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "0.00 ₽";

            decimal amount = 0;

            if (value is decimal decimalValue)
            {
                amount = decimalValue;
            }
            else if (value is double doubleValue)
            {
                amount = (decimal)doubleValue;
            }
            else if (value is int intValue)
            {
                amount = intValue;
            }
            else if (value is float floatValue)
            {
                amount = (decimal)floatValue;
            }
            else
            {
                // Попытка преобразовать строку в decimal
                if (decimal.TryParse(value.ToString(), out decimal parsedValue))
                {
                    amount = parsedValue;
                }
                else
                {
                    return "0.00 ₽";
                }
            }

            // Форматирование: разделение тысяч пробелами, точка для копеек
            // Используем кастомный формат
            var cultureInfo = new CultureInfo("ru-RU");
            var numberFormat = (NumberFormatInfo)cultureInfo.NumberFormat.Clone();
            
            // Настраиваем формат:
            // - Разделитель групп (тысяч) - пробел
            // - Разделитель дробной части - точка
            numberFormat.NumberGroupSeparator = " ";
            numberFormat.NumberDecimalSeparator = ".";
            numberFormat.NumberDecimalDigits = 2;

            // Форматируем число и добавляем знак рубля после суммы
            string formattedAmount = amount.ToString("N", numberFormat);
            return $"{formattedAmount} руб.";
        }

        /// <summary>
        /// Обратное преобразование (не поддерживается для этого конвертера).
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("Обратное преобразование не поддерживается для RubleCurrencyConverter");
        }
    }
}
