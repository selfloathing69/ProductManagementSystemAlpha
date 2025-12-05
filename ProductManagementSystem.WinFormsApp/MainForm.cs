using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ProductManagementSystem.Logic;
using ProductManagementSystem.Logic.Presenters;
using ProductManagementSystem.Model;
using ProductManagementSystem.Shared;

namespace ProductManagementSystem.WinFormsApp
{
    /// <summary>
    /// MVP паттерн - реализация главного представления.
    /// Реализует интерфейс IProductView для MVP паттерна.
    /// SOLID - S: Класс отвечает только за UI главного окна.
    /// SOLID - D: Зависит от абстракции IProductView, взаимодействует с презентером через события.
    /// </summary>
    public partial class MainForm : Form, IProductView
    {
        #region Fields

        private ProductPresenter? _presenter;
        private IProductModel? _model;

        // Основные элементы UI
        private DataGridView dataGridView = null!;
        private Panel buttonPanel = null!;
        private Button btnRefresh = null!;
        private Button btnAdd = null!;
        private Button btnDelete = null!;
        private Button btnDeleteByQuantity = null!;

        #endregion

        #region IProductView Events

        /// <inheritdoc/>
        public event EventHandler? RefreshRequested;

        /// <inheritdoc/>
        public event EventHandler? AddProductRequested;

        /// <inheritdoc/>
        public event EventHandler<int>? DeleteProductRequested;

        /// <inheritdoc/>
        public event EventHandler<(int ProductId, int Quantity)>? DeleteProductByQuantityRequested;

        #endregion

        #region Constructor

        /// <summary>
        /// Конструктор по умолчанию для MainForm.
        /// Инициализирует компоненты UI. Презентер будет установлен позже через метод SetPresenter.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Устанавливает презентер и модель для этого представления.
        /// Этот метод должен быть вызван после создания для завершения связывания MVP.
        /// </summary>
        /// <param name="presenter">The presenter to handle view events</param>
        /// <param name="model">The model for business operations</param>
        public void SetPresenter(ProductPresenter presenter, IProductModel model)
        {
            _presenter = presenter ?? throw new ArgumentNullException(nameof(presenter));
            _model = model ?? throw new ArgumentNullException(nameof(model));
        }

        #endregion

        #region IProductView Implementation

        /// <inheritdoc/>
        public void ShowProducts(IEnumerable<Product> products)
        {
            try
            {
                dataGridView.DataSource = null;
                dataGridView.DataSource = products.ToList();

                // Настройка заголовков столбцов
                if (dataGridView.Columns.Count > 0)
                {
                    if (dataGridView.Columns.Contains("Id"))
                        dataGridView.Columns["Id"].HeaderText = "ID";
                    
                    if (dataGridView.Columns.Contains("Name"))
                        dataGridView.Columns["Name"].HeaderText = "Название";
                    
                    if (dataGridView.Columns.Contains("Description"))
                        dataGridView.Columns["Description"].HeaderText = "Описание";
                    
                    if (dataGridView.Columns.Contains("Price"))
                    {
                        dataGridView.Columns["Price"].HeaderText = "Цена";
                        dataGridView.Columns["Price"].DefaultCellStyle.Format = "N2";
                    }
                    
                    if (dataGridView.Columns.Contains("Category"))
                        dataGridView.Columns["Category"].HeaderText = "Категория";
                    
                    if (dataGridView.Columns.Contains("StockQuantity"))
                        dataGridView.Columns["StockQuantity"].HeaderText = "Количество";
                }
            }
            catch (Exception ex)
            {
                ShowError("Ошибка", $"Ошибка при обновлении таблицы: {ex.Message}");
            }
        }

        /// <inheritdoc/>
        public void ShowError(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <inheritdoc/>
        public void ShowMessage(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public bool ShowConfirmation(string title, string message)
        {
            return MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }
        public int? GetSelectedProductId()
        {
            if (dataGridView.CurrentRow == null)
                return null;

            if (!dataGridView.Columns.Contains("Id") || dataGridView.CurrentRow.Cells["Id"].Value == null)
                return null;

            return (int)dataGridView.CurrentRow.Cells["Id"].Value;
        }

        #endregion

        #region Initialize Components

        /// <summary>
        /// Инициализирует все компоненты формы.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Настройки формы
            this.Text = "Продакт манагемент систем";
            this.ClientSize = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimumSize = new Size(800, 600);
            this.MaximumSize = new Size(800, 600);

            // Панель кнопок
            buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                Padding = new Padding(5)
            };

            // Кнопка обновления
            btnRefresh = new Button
            {
                Text = "Обновить",
                Location = new Point(10, 10),
                Size = new Size(120, 30),
                UseVisualStyleBackColor = true
            };
            btnRefresh.Click += BtnRefresh_Click;

            // Кнопка добавления
            btnAdd = new Button
            {
                Text = "Добавить",
                Location = new Point(140, 10),
                Size = new Size(120, 30),
                UseVisualStyleBackColor = true
            };
            btnAdd.Click += BtnAdd_Click;

            // Кнопка удаления
            btnDelete = new Button
            {
                Text = "Удалить",
                Location = new Point(270, 10),
                Size = new Size(120, 30),
                UseVisualStyleBackColor = true
            };
            btnDelete.Click += BtnDelete_Click;

            // Кнопка удаления по количеству
            btnDeleteByQuantity = new Button
            {
                Text = "Удалить количество",
                Location = new Point(400, 10),
                Size = new Size(160, 30),
                UseVisualStyleBackColor = true
            };
            btnDeleteByQuantity.Click += BtnDeleteByQuantity_Click;

            // Добавление кнопок на панель
            buttonPanel.Controls.Add(btnRefresh);
            buttonPanel.Controls.Add(btnAdd);
            buttonPanel.Controls.Add(btnDelete);
            buttonPanel.Controls.Add(btnDeleteByQuantity);

            // Создание DataGridView
            dataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoGenerateColumns = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                RowHeadersVisible = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = SystemColors.Window,
                BorderStyle = BorderStyle.Fixed3D
            };

            // Добавление элементов на форму
            this.Controls.Add(dataGridView);
            this.Controls.Add(buttonPanel);

            this.ResumeLayout(false);
        }

        #endregion

        #region Button Event Handlers - Fire View Events

        /// <summary>
        /// Обрабатывает нажатие кнопки обновления - генерирует событие RefreshRequested.
        /// </summary>
        private void BtnRefresh_Click(object? sender, EventArgs e)
        {
            RefreshRequested?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Обрабатывает нажатие кнопки добавления - показывает диалог AddProductForm.
        /// MVP: представление генерирует событие AddProductRequested и обрабатывает диалог.
        /// </summary>
        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            AddProductRequested?.Invoke(this, EventArgs.Empty);

            if (_model == null)
            {
                ShowError("Ошибка", "Модель не инициализирована.");
                return;
            }

            // Показ диалога добавления товара (представление отвечает за показ диалога)
            using (var addForm = new AddProductForm(_model))
            {
                if (addForm.ShowDialog(this) == DialogResult.OK)
                {
                    // Обновление обрабатывается событием ProductsChanged модели
                }
            }
        }

        /// <summary>
        /// Обрабатывает нажатие кнопки удаления - генерирует событие DeleteProductRequested.
        /// </summary>
        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            var productId = GetSelectedProductId();
            if (productId == null)
            {
                ShowError("Предупреждение", "Пожалуйста, выберите строку для удаления.");
                return;
            }

            DeleteProductRequested?.Invoke(this, productId.Value);
        }

        /// <summary>
        /// Обрабатывает нажатие кнопки удаления по количеству.
        /// MVP: представление обрабатывает показ диалога выбора и генерирует событие DeleteProductByQuantityRequested.
        /// </summary>
        private void BtnDeleteByQuantity_Click(object? sender, EventArgs e)
        {
            if (_presenter == null)
            {
                ShowError("Ошибка", "Презентер не инициализирован.");
                return;
            }

            try
            {
                var productsWithIndexes = _presenter.GetProductsWithIndexes();
                
                if (productsWithIndexes.Count == 0)
                {
                    ShowMessage("Информация", "Товары не найдены.");
                    return;
                }

                // Создание формы выбора товара
                using (var selectForm = new Form
                {
                    Text = "Выберите товар для удаления",
                    Size = new Size(500, 400),
                    StartPosition = FormStartPosition.CenterParent,
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    MaximizeBox = false,
                    MinimizeBox = false
                })
                {
                    // Список товаров
                    var listBox = new ListBox
                    {
                        Dock = DockStyle.Top,
                        Height = 200
                    };

                    foreach (var (index, product) in productsWithIndexes)
                    {
                        listBox.Items.Add($"{index}. {product.Name}, {product.StockQuantity} шт");
                    }

                    // Поле ввода количества
                    var lblQuantity = new Label
                    {
                        Text = "Количество для удаления (0 - всё):",
                        Location = new Point(10, 220),
                        Size = new Size(250, 20)
                    };

                    var numQuantity = new NumericUpDown
                    {
                        Location = new Point(10, 245),
                        Size = new Size(120, 25),
                        Minimum = 0,
                        Maximum = 999999
                    };

                    var btnOk = new Button
                    {
                        Text = "ОК",
                        Location = new Point(10, 280),
                        Size = new Size(80, 30)
                    };

                    var btnCancel = new Button
                    {
                        Text = "Отмена",
                        Location = new Point(100, 280),
                        Size = new Size(80, 30)
                    };

                    // Обработчик выбора товара
                    listBox.SelectedIndexChanged += (s, args) =>
                    {
                        if (listBox.SelectedIndex >= 0)
                        {
                            var selectedProduct = productsWithIndexes[listBox.SelectedIndex].Product;
                            numQuantity.Maximum = selectedProduct.StockQuantity;
                            numQuantity.Value = 0;
                        }
                    };

                    // Обработчик кнопки OK - генерирует событие для презентера
                    btnOk.Click += (s, args) =>
                    {
                        if (listBox.SelectedIndex < 0)
                        {
                            ShowError("Предупреждение", "Выберите товар для удаления.");
                            return;
                        }

                        var selectedProduct = productsWithIndexes[listBox.SelectedIndex].Product;
                        int quantityToRemove = (int)numQuantity.Value;

                        // Генерация события для презентера
                        DeleteProductByQuantityRequested?.Invoke(this, (selectedProduct.Id, quantityToRemove));
                        selectForm.DialogResult = DialogResult.OK;
                    };

                    // Обработчик кнопки отмены
                    btnCancel.Click += (s, args) =>
                    {
                        selectForm.DialogResult = DialogResult.Cancel;
                    };

                    selectForm.Controls.Add(listBox);
                    selectForm.Controls.Add(lblQuantity);
                    selectForm.Controls.Add(numQuantity);
                    selectForm.Controls.Add(btnOk);
                    selectForm.Controls.Add(btnCancel);

                    selectForm.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                ShowError("Ошибка", $"Ошибка при удалении товара: {ex.Message}");
            }
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Очистка используемых ресурсов.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _presenter?.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}
