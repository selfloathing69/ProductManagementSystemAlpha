using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using ProductManagementSystem.WpfApp.Commands;

namespace ProductManagementSystem.WpfApp.ViewModels
{
    /// <summary>
    /// MVVM Pattern - ViewModel для окна подтверждения изменений товара.
    /// 
    /// SOLID - S: Класс отвечает только за логику окна подтверждения.
    /// </summary>
    public class ConfirmationViewModel : ViewModelBase
    {
        private bool? _dialogResult;

        /// <summary>
        /// Результат диалогового окна (true = Да, false = Нет, null = закрыто)
        /// </summary>
        public bool? DialogResult
        {
            get => _dialogResult;
            set => SetProperty(ref _dialogResult, value);
        }

        /// <summary>
        /// Заголовок окна
        /// </summary>
        public string Title { get; set; } = "Подтвердите изменения";

        /// <summary>
        /// ID товара
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Название товара
        /// </summary>
        public string ProductName { get; set; } = string.Empty;

        /// <summary>
        /// Текст с описанием изменений
        /// </summary>
        public string ChangesText { get; set; } = string.Empty;

        /// <summary>
        /// Команда подтверждения (Да)
        /// </summary>
        public ICommand ConfirmCommand { get; }

        /// <summary>
        /// Команда отмены (Нет)
        /// </summary>
        public ICommand CancelCommand { get; }

        /// <summary>
        /// Инициализирует новый экземпляр ConfirmationViewModel.
        /// </summary>
        public ConfirmationViewModel()
        {
            ConfirmCommand = new RelayCommand(_ => OnConfirm());
            CancelCommand = new RelayCommand(_ => OnCancel());
        }

        /// <summary>
        /// Обработчик команды подтверждения
        /// </summary>
        private void OnConfirm()
        {
            DialogResult = true;
        }

        /// <summary>
        /// Обработчик команды отмены
        /// </summary>
        private void OnCancel()
        {
            DialogResult = false;
        }

        /// <summary>
        /// Создает текст с описанием изменений
        /// </summary>
        public static string BuildChangesText(
            string? oldName, string? newName,
            string? oldDescription, string? newDescription,
            decimal? oldPrice, decimal? newPrice,
            string? oldCategory, string? newCategory,
            int? oldQuantity, int? newQuantity)
        {
            var changes = new List<string>();

            if (!string.IsNullOrEmpty(newName) && oldName != newName)
            {
                changes.Add($"Название \"{oldName}\" изменится на \"{newName}\"");
            }

            if (!string.IsNullOrEmpty(newDescription) && oldDescription != newDescription)
            {
                changes.Add($"Описание \"{oldDescription}\" изменится на \"{newDescription}\"");
            }

            if (newPrice.HasValue && oldPrice != newPrice)
            {
                changes.Add($"Цена {oldPrice:N2} руб. изменится на {newPrice:N2} руб.");
            }

            if (!string.IsNullOrEmpty(newCategory) && oldCategory != newCategory)
            {
                changes.Add($"Категория \"{oldCategory}\" изменится на \"{newCategory}\"");
            }

            if (newQuantity.HasValue && oldQuantity != newQuantity)
            {
                changes.Add($"Количество {oldQuantity} шт. изменится на {newQuantity} шт.");
            }

            return changes.Any() 
                ? string.Join(Environment.NewLine, changes) 
                : "Нет изменений";
        }
    }
}
