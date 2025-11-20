using System;
using System.Drawing;
using System.Windows.Forms;
using ProductManagementSystem.Logic;

namespace ProductManagementSystem.WinFormsApp
{
    /// <summary>
    /// SOLID - S: Класс отвечает только за UI главного окна WinForms приложения
    /// 
    /// Главное окно приложения для управления товарами
    /// </summary>
    public partial class MainForm : Form
    {
        #region Поля

        // Бизнес-логика: все операции с товарами
        /// <summary>
        /// SOLID - D: Зависимость от ProductLogic через RepositoryFactory для переключения между EF и Dapper
        /// </summary>
        private ProductLogic _logic;

        // Основные элементы управления
        private DataGridView dataGridView;
        private Panel buttonPanel;
        private Button btnRefresh;
        private Button btnAdd;
        private Button btnDelete;
        private Button btnDeleteByQuantity;

        #endregion

        #region Конструктор

        /// <summary>
        /// Конструктор главной формы
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            
            // Инициализация бизнес-логики после InitializeComponent
            try
            {
                _logic = new ProductLogic(RepositoryFactory.CreateRepository());
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка инициализации репозитория: {ex.Message}", 
                    "Ошибка", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
                // SOLID - D: Fallback к in-memory репозиторию при ошибке БД
                _logic = new ProductLogic(null);
            }
            
            // Загрузка данных при запуске
            RefreshGrid();
        }

        #endregion

        #region Инициализация компонентов

        /// <summary>
        /// Инициализация всех компонентов формы
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Настройка формы 
            this.Text = "Продакт манагемент систем";
            this.ClientSize = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimumSize = new Size(800, 600);
            this.MaximumSize = new Size(800, 600);

            // кнопочки короче тут :)
            buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                Padding = new Padding(5)
            };

            // Кнопка "Обновить"
            btnRefresh = new Button
            {
                Text = "Обновить",
                Location = new Point(10, 10),
                Size = new Size(120, 30),
                UseVisualStyleBackColor = true
            };
            btnRefresh.Click += BtnRefresh_Click;

            // Кнопка "Добавить"
            btnAdd = new Button
            {
                Text = "Добавить",
                Location = new Point(140, 10),
                Size = new Size(120, 30),
                UseVisualStyleBackColor = true
            };
            btnAdd.Click += BtnAdd_Click;

            // Кнопка "Удалить"
            btnDelete = new Button
            {
                Text = "Удалить",
                Location = new Point(270, 10),
                Size = new Size(120, 30),
                UseVisualStyleBackColor = true
            };
            btnDelete.Click += BtnDelete_Click;

            // Кнопка "Удалить определенное количество"
            btnDeleteByQuantity = new Button
            {
                Text = "Удалить количество",
                Location = new Point(400, 10),
                Size = new Size(160, 30),
                UseVisualStyleBackColor = true
            };
            btnDeleteByQuantity.Click += BtnDeleteByQuantity_Click;

            // Добавляем кнопки на панель
            buttonPanel.Controls.Add(btnRefresh);
            buttonPanel.Controls.Add(btnAdd);
            buttonPanel.Controls.Add(btnDelete);
            buttonPanel.Controls.Add(btnDeleteByQuantity);

            //  Создание DataGridView (заполняет оставшееся пространство)
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

            //  Добавление элементов на форму 
            this.Controls.Add(dataGridView);
            this.Controls.Add(buttonPanel);

            this.ResumeLayout(false);
        }

        #endregion

        #region Обработчики событий кнопок

        /// <summary>
        /// Обновление данных в таблице
        /// </summary>
        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                RefreshGrid();
                MessageBox.Show(
                    "Данные успешно обновлены.", 
                    "Обновление", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка при обновлении данных: {ex.Message}", 
                    "Ошибка", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Показать форму добавления товара
        /// </summary>
        private void BtnAdd_Click(object sender, EventArgs e)
        {
            // SOLID - S: Отдельная форма AddProductForm отвечает за ui добавления товара
            using (var addForm = new AddProductForm(_logic))
            {
                // ShowDialog делает форму модальной автоматически:
                // - Блокирует взаимодействие с родительской формой
                // - Форму можно перемещать
                // - Возвращает DialogResult после закрытия
                // Модальное окно блокирует родительскую форму
                if (addForm.ShowDialog(this) == DialogResult.OK)
                {
                    // Обновляем таблицу только если товар был успешно добавлен
                    RefreshGrid();
                }
            }
        }

        /// <summary>
        /// Удаление выбранного товара
        /// </summary>
        private void BtnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView.CurrentRow == null)
                {
                    MessageBox.Show(
                        "Пожалуйста, выберите строку для удаления.", 
                        "Предупреждение",
                        MessageBoxButtons.OK, 
                        MessageBoxIcon.Warning);
                    return;
                }

                if (!dataGridView.Columns.Contains("Id") || 
                    dataGridView.CurrentRow.Cells["Id"].Value == null)
                {
                    MessageBox.Show(
                        "Не удалось определить ID товара.", 
                        "Ошибка",
                        MessageBoxButtons.OK, 
                        MessageBoxIcon.Error);
                    return;
                }

                int id = (int)dataGridView.CurrentRow.Cells["Id"].Value;

                DialogResult result = MessageBox.Show(
                    $"Вы уверены, что хотите удалить товар с ID {id}?",
                    "Подтверждение удаления",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    bool deleted = _logic.DeleteProduct(id);
                    if (deleted)
                    {
                        RefreshGrid();
                        MessageBox.Show(
                            "Товар успешно удалён.", 
                            "Успех",
                            MessageBoxButtons.OK, 
                            MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show(
                            "Не удалось удалить товар.", 
                            "Ошибка",
                            MessageBoxButtons.OK, 
                            MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка при удалении товара: {ex.Message}", 
                    "Ошибка",
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Удаление товара с указанием количества
        /// </summary>
        private void BtnDeleteByQuantity_Click(object sender, EventArgs e)
        {
            try
            {
                var productsWithIndexes = _logic.GetProductsWithIndexes();
                
                if (productsWithIndexes.Count == 0)
                {
                    MessageBox.Show(
                        "Товары не найдены.", 
                        "Информация",
                        MessageBoxButtons.OK, 
                        MessageBoxIcon.Information);
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

                    // Поле для ввода количества
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

                    // Обработчик кнопки ОК
                    btnOk.Click += (s, args) =>
                    {
                        if (listBox.SelectedIndex < 0)
                        {
                            MessageBox.Show(
                                "Выберите товар для удаления.", 
                                "Предупреждение",
                                MessageBoxButtons.OK, 
                                MessageBoxIcon.Warning);
                            return;
                        }

                        var selectedProduct = productsWithIndexes[listBox.SelectedIndex].Product;
                        int quantityToRemove = (int)numQuantity.Value;

                        if (quantityToRemove == 0)
                            quantityToRemove = selectedProduct.StockQuantity;

                        DialogResult confirmResult = MessageBox.Show(
                            $"Вы хотите удалить {selectedProduct.Name} в количестве {quantityToRemove}, вы уверены?",
                            "Подтверждение удаления",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);

                        if (confirmResult == DialogResult.Yes)
                        {
                            bool success = _logic.RemoveQuantityFromProduct(selectedProduct.Id, quantityToRemove);
                            if (success)
                            {
                                RefreshGrid();
                                MessageBox.Show(
                                    "Товар удален в указанном количестве.", 
                                    "Успех",
                                    MessageBoxButtons.OK, 
                                    MessageBoxIcon.Information);
                                selectForm.DialogResult = DialogResult.OK;
                            }
                            else
                            {
                                MessageBox.Show(
                                    "Не удалось удалить товар.", 
                                    "Ошибка",
                                    MessageBoxButtons.OK, 
                                    MessageBoxIcon.Error);
                            }
                        }
                    };

                    // Обработчик кнопки Отмена
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
                MessageBox.Show(
                    $"Ошибка при удалении товара: {ex.Message}", 
                    "Ошибка",
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Работа с данными

        /// <summary>
        /// Обновление данных в таблице
        /// </summary>
        private void RefreshGrid()
        {
            try
            {
                dataGridView.DataSource = null;
                var products = _logic.GetAllProducts();
                dataGridView.DataSource = products;

                // Настройка заголовков колонок
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
                MessageBox.Show(
                    $"Ошибка при обновлении таблицы: {ex.Message}", 
                    "Ошибка",
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
            }
        }

        #endregion
    }
}