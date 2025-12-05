using System;
using System.Windows.Forms;
using ProductManagementSystem.Logic;
using ProductManagementSystem.Logic.Presenters;
using ProductManagementSystem.Shared;

namespace ProductManagementSystem.WinFormsApp
{
    /// <summary>
    /// Шаблон MVP — Application Runner.
    /// Содержит метод для запуска приложения Windows Forms.
    /// Вызывается приложением Console Presenter.
    /// SOLID — D: Использует контейнер DI для подключения компонентов MVP.
    /// </summary>
    public static class WinFormsRunner
    {
        public static void Run(IProductModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            try
            {
                // Создали  View (MainForm)===============================================================
                var view = new MainForm();
                
                // СОЗДАЛИ Presenter, СВЯЗАЛИ View И Model =========================================================
                using var presenter = new ProductPresenter(view, model);

                // Подписались на AddProductRequested, чтобы обрабатывать диалог добавления
                view.AddProductRequested += (sender, e) =>
                {
                    var addView = new AddProductForm();
                    using var addPresenter = presenter.CreateAddProductPresenter(addView);
                    addView.ShowDialog();
                };

                // Подписались на DeleteByQuantityRequested для обработки диалога
                view.DeleteProductByQuantityRequested += (sender, args) =>
                {
                    if (args.ProductId == -1)
                    {
                        // Signal to show dialog first
                        var productsWithIndexes = presenter.GetProductsWithIndexes();
                        var result = view.ShowDeleteByQuantityDialog(productsWithIndexes);
                        if (result.HasValue)
                        {
                            // Fire the real delete event
                            OnDeleteByQuantityConfirmed(presenter, view, result.Value);
                        }
                    }
                };
                
                // Запуск
                Application.Run(view);


            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка запуска приложения: {ex.Message}\n\n{ex.StackTrace}",
                    "Критическая ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private static void OnDeleteByQuantityConfirmed(ProductPresenter presenter, IProductView view, (int ProductId, int Quantity) args)
        {
            try
            {
                var product = presenter.GetProduct(args.ProductId);
                if (product == null)
                {
                    view.ShowError("Ошибка", "Товар не найден.");
                    return;
                }

                int quantityToRemove = args.Quantity;
                if (quantityToRemove == 0)
                {
                    quantityToRemove = product.StockQuantity;
                }

                if (view.ShowConfirmation("Подтверждение удаления",
                    $"Вы хотите удалить {product.Name} в количестве {quantityToRemove}, вы уверены?"))
                {
                    if (presenter.RemoveQuantityFromProduct(args.ProductId, quantityToRemove))
                    {
                        view.ShowMessage("Успех", "Товар удален в указанном количестве.");
                    }
                    else
                    {
                        view.ShowError("Ошибка", "Не удалось удалить товар.");
                    }
                }
            }
            catch (Exception ex)
            {
                view.ShowError("Ошибка", $"Ошибка при удалении товара: {ex.Message}");
            }
        }
    }
}
