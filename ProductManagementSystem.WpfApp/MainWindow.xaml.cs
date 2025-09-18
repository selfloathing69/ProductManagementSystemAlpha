using System.Windows;
using System.Linq;
using ProductManagementSystem.Logic;
using ProductManagementSystem.Model;
using System.Collections.Generic;

namespace ProductManagementSystem.WpfApp
{
    public partial class MainWindow : Window
    {
        private ProductLogic _logic = new ProductLogic();

        public MainWindow()
        {
            InitializeComponent();
            Refresh();
        }

        private void Refresh()
        {
            DataGridProducts.ItemsSource = null;
            DataGridProducts.ItemsSource = _logic.GetAllProducts();
            LoadCategories();
            StatusText.Text = "Данные обновлены";
        }

        private void LoadCategories()
        {
            ComboBoxCategory.Items.Clear();
            ComboBoxCategory.Items.Add("Все категории");
            var cats = new HashSet<string>(_logic.GetAllProducts().Where(p => !string.IsNullOrWhiteSpace(p.Category)).Select(p => p.Category));
            foreach (var c in cats) ComboBoxCategory.Items.Add(c);
            ComboBoxCategory.SelectedIndex = 0;
        }

        private void ButtonRefresh_Click(object sender, RoutedEventArgs e) => Refresh();

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!decimal.TryParse(TextBoxPrice.Text, out var price)) { StatusText.Text = "Некорректная цена"; return; }
            if (!int.TryParse(TextBoxQuantity.Text, out var q)) { StatusText.Text = "Некорректное количество"; return; }
            var p = new Product { Name = TextBoxName.Text, Description = TextBoxDescription.Text, Category = TextBoxCategory.Text, Price = price, StockQuantity = q };
            _logic.AddProduct(p);
            StatusText.Text = "Товар добавлен";
            TextBoxName.Clear(); TextBoxDescription.Clear(); TextBoxCategory.Clear(); TextBoxPrice.Clear(); TextBoxQuantity.Clear();
            Refresh();
        }

        private void ButtonFind_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(TextBoxFindId.Text, out var id)) { StatusText.Text = "Некорректный ID"; return; }
            var p = _logic.GetProduct(id);
            if (p == null) { StatusText.Text = "Не найден"; return; }
            TextBlockEditId.Text = "ID: " + p.Id;
            TextBoxEditName.Text = p.Name; TextBoxEditDescription.Text = p.Description; TextBoxEditCategory.Text = p.Category;
            TextBoxEditPrice.Text = p.Price.ToString(); TextBoxEditQuantity.Text = p.StockQuantity.ToString();
            StatusText.Text = "Товар загружен для редактирования";
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            var idText = TextBlockEditId.Text.Replace("ID: ", "");
            if (!int.TryParse(idText, out var id)) { StatusText.Text = "Нет выбранного товара"; return; }
            if (!decimal.TryParse(TextBoxEditPrice.Text, out var price)) { StatusText.Text = "Некорректная цена"; return; }
            if (!int.TryParse(TextBoxEditQuantity.Text, out var q)) { StatusText.Text = "Некорректное количество"; return; }
            var p = new Product { Id = id, Name = TextBoxEditName.Text, Description = TextBoxEditDescription.Text, Category = TextBoxEditCategory.Text, Price = price, StockQuantity = q };
            if (_logic.UpdateProduct(p)) { StatusText.Text = "Сохранено"; Refresh(); }
            else StatusText.Text = "Ошибка при сохранении";
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridProducts.SelectedItem is Product p)
            {
                if (_logic.DeleteProduct(p.Id)) { StatusText.Text = "Удалено"; Refresh(); }
                else StatusText.Text = "Ошибка при удалении";
            }
            else StatusText.Text = "Выберите строку для удаления";
        }

        private void ButtonFilter_Click(object sender, RoutedEventArgs e)
        {
            var sel = ComboBoxCategory.SelectedItem as string ?? "Все категории";
            if (sel == "Все категории") DataGridProducts.ItemsSource = _logic.GetAllProducts();
            else DataGridProducts.ItemsSource = _logic.FilterByCategory(sel);
            StatusText.Text = "Фильтр применён";
        }

        private void ButtonTotal_Click(object sender, RoutedEventArgs e)
        {
            var val = _logic.CalculateTotalInventoryValue();
            TextBlockTotal.Text = "Общая стоимость: " + val.ToString("C");
        }
    }
}
