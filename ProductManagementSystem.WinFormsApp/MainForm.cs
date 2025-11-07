using System;
using System.Drawing;
using System.Windows.Forms;
using ProductManagementSystem.Logic;

namespace ProductManagementSystem.WinFormsApp
{
    /// <summary>
    /// Главное окно приложения для управления товарами
    /// </summary>
    public partial class MainForm : Form
    {
        #region Поля

        // Бизнес-логика: все операции с товарами
        private ProductLogic _logic;

        // Основные элементы управления
        private DataGridView dataGridView;
        private Panel buttonPanel;
        private Button btnRefresh;
        private Button btnAdd;
        private Button btnDelete;
        private Button btnDeleteByQuantity;

        // Панель добавления нового товара
        private Panel addPanel;
        private NumericUpDown numId;
        private TextBox txtName;
        private TextBox txtDescription;
        private ComboBox cmbCategory;
        private NumericUpDown numPrice;
        private NumericUpDown numStock;
        private Button btnAddOk;
        private Button btnAddCancel;

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
                _logic = new ProductLogic(null); // Fallback к in-memory репозиторию
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

            // === Настройка формы ===
            this.Text = "Система управления товарами - Windows Forms";
            this.ClientSize = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimumSize = new Size(800, 600);
            this.MaximumSize = new Size(800, 600);

            // === Создание панели с кнопками (внизу) ===
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

            // === Создание DataGridView (заполняет оставшееся пространство) ===
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

            // === Добавление элементов на форму ===
            this.Controls.Add(dataGridView);
            this.Controls.Add(buttonPanel);

            // === Создание панели добавления товара ===
            CreateAddPanel();
            this.Controls.Add(addPanel);
            addPanel.Visible = false;
            addPanel.BringToFront();

            this.ResumeLayout(false);
        }

        /// <summary>
        /// Создание панели для добавления нового товара
        /// </summary>
        private void CreateAddPanel()
        {
            int panelWidth = 460;
            int panelHeight = 340;

            addPanel = new Panel
            {
                Width = panelWidth,
                Height = panelHeight,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = SystemColors.Control
            };

            // Центрирование панели
            addPanel.Left = (this.ClientSize.Width - panelWidth) / 2;
            addPanel.Top = (this.ClientSize.Height - panelHeight) / 2;

            // Заголовок
            Label lblTitle = new Label
            {
                Text = "Новый товар",
                Left = 10,
                Top = 10,
                Width = 200,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };

            // ID
            Label lblId = new Label { Text = "ID:", Left = 10, Top = 45, Width = 100 };
            numId = new NumericUpDown
            {
                Left = 120,
                Top = 40,
                Width = 150,
                Minimum = 1M,
                Maximum = 999999M,
                Value = 1M
            };

            // Название
            Label lblName = new Label { Text = "Название:", Left = 10, Top = 80, Width = 100 };
            txtName = new TextBox { Left = 120, Top = 75, Width = 300 };
            txtName.KeyPress += RussianOnly_KeyPress;
            txtName.TextChanged += RussianOnly_TextChanged;

            // Описание
            Label lblDesc = new Label { Text = "Описание:", Left = 10, Top = 115, Width = 100 };
            txtDescription = new TextBox { Left = 120, Top = 110, Width = 300 };
            txtDescription.KeyPress += RussianOnly_KeyPress;
            txtDescription.TextChanged += RussianOnly_TextChanged;

            // Цена
            Label lblPrice = new Label { Text = "Цена:", Left = 10, Top = 150, Width = 100 };
            numPrice = new NumericUpDown
            {
                Left = 120,
                Top = 145,
                Width = 150,
                Maximum = 1000000M,
                DecimalPlaces = 2,
                Minimum = 0M
            };

            // Категория
            Label lblCategory = new Label { Text = "Категория:", Left = 10, Top = 185, Width = 100 };
            cmbCategory = new ComboBox
            {
                Left = 120,
                Top = 180,
                Width = 300,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbCategory.Items.AddRange(new object[] 
            { 
                "Электроника", 
                "Периферия", 
                "Аудио", 
                "Комплектующие", 
                "Аксессуары", 
                "Разное" 
            });

            // Количество
            Label lblStock = new Label { Text = "Количество:", Left = 10, Top = 220, Width = 100 };
            numStock = new NumericUpDown
            {
                Left = 120,
                Top = 215,
                Width = 150,
                Maximum = 100000M,
                Minimum = 0M
            };

            // Кнопки
            btnAddOk = new Button { Text = "ОК", Left = 120, Top = 260, Width = 90 };
            btnAddCancel = new Button { Text = "Отмена", Left = 240, Top = 260, Width = 90 };

            btnAddOk.Click += BtnAddOk_Click;
            btnAddCancel.Click += BtnAddCancel_Click;

            // Добавление всех элементов на панель
            addPanel.Controls.Add(lblTitle);
            addPanel.Controls.Add(lblId);
            addPanel.Controls.Add(numId);
            addPanel.Controls.Add(lblName);
            addPanel.Controls.Add(txtName);
            addPanel.Controls.Add(lblDesc);
            addPanel.Controls.Add(txtDescription);
            addPanel.Controls.Add(lblPrice);
            addPanel.Controls.Add(numPrice);
            addPanel.Controls.Add(lblCategory);
            addPanel.Controls.Add(cmbCategory);
            addPanel.Controls.Add(lblStock);
            addPanel.Controls.Add(numStock);
            addPanel.Controls.Add(btnAddOk);
            addPanel.Controls.Add(btnAddCancel);
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
        /// Показать панель добавления товара
        /// </summary>
        private void BtnAdd_Click(object sender, EventArgs e)
        {
            ShowAddPanel();
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

        #region Работа с панелью добавления

        /// <summary>
        /// Показать панель добавления товара
        /// </summary>
        private void ShowAddPanel()
        {
            // Очистка полей
            numId.Value = 1M;
            txtName.Text = string.Empty;
            txtDescription.Text = string.Empty;
            cmbCategory.SelectedIndex = -1;
            numPrice.Value = 0M;
            numStock.Value = 0M;

            // Блокировка других элементов
            dataGridView.Enabled = false;
            btnRefresh.Enabled = false;
            btnAdd.Enabled = false;
            btnDelete.Enabled = false;
            btnDeleteByQuantity.Enabled = false;

            // Показать панель
            addPanel.Visible = true;
            addPanel.BringToFront();
            numId.Focus();
        }

        /// <summary>
        /// Скрыть панель добавления товара
        /// </summary>
        private void HideAddPanel()
        {
            addPanel.Visible = false;
            
            // Разблокировка элементов
            dataGridView.Enabled = true;
            btnRefresh.Enabled = true;
            btnAdd.Enabled = true;
            btnDelete.Enabled = true;
            btnDeleteByQuantity.Enabled = true;
        }

        /// <summary>
        /// Подтверждение добавления товара
        /// </summary>
        private void BtnAddOk_Click(object sender, EventArgs e)
        {
            try
            {
                // Валидация
                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    MessageBox.Show(
                        "Поле 'Название' обязательно для заполнения!", 
                        "Ошибка",
                        MessageBoxButtons.OK, 
                        MessageBoxIcon.Warning);
                    txtName.Focus();
                    return;
                }

                if (cmbCategory.SelectedItem == null)
                {
                    MessageBox.Show(
                        "Поле 'Категория' обязательно для заполнения!", 
                        "Ошибка",
                        MessageBoxButtons.OK, 
                        MessageBoxIcon.Warning);
                    cmbCategory.Focus();
                    return;
                }

                int id = (int)numId.Value;

                // Проверка существования ID
                if (_logic.IdExists(id))
                {
                    DialogResult result = MessageBox.Show(
                        $"ID {id} уже существует. Хотите присвоить новому товару другой ID?",
                        "ID уже существует",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        numId.Focus();
                        return;
                    }
                    else
                    {
                        // Проверка возможности суммирования количества
                        string name = txtName.Text.Trim();
                        string category = cmbCategory.SelectedItem.ToString().Trim();

                        var existingProduct = _logic.FindProductByNameAndCategory(name, category);
                        
                        if (existingProduct != null && 
                            !category.Equals("Разное", StringComparison.OrdinalIgnoreCase))
                        {
                            DialogResult sumResult = MessageBox.Show(
                                $"Такой товар '{name}' уже существует в количестве: {existingProduct.StockQuantity}.\n" +
                                $"Желаете суммировать введённое вами количество с уже имеющимся?",
                                "Товар уже существует",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question);

                            if (sumResult == DialogResult.Yes)
                            {
                                bool added = _logic.AddQuantityToProduct(existingProduct.Id, (int)numStock.Value);
                                if (added)
                                {
                                    RefreshGrid();
                                    HideAddPanel();
                                    
                                    var updatedProduct = _logic.GetProduct(existingProduct.Id);
                                    MessageBox.Show(
                                        $"Количество товара увеличено. Новое количество: {updatedProduct?.StockQuantity ?? 0}", 
                                        "Готово",
                                        MessageBoxButtons.OK, 
                                        MessageBoxIcon.Information);
                                }
                                return;
                            }
                        }

                        MessageBox.Show(
                            "Операция отменена.", 
                            "Отмена",
                            MessageBoxButtons.OK, 
                            MessageBoxIcon.Information);
                        return;
                    }
                }

                // Создание нового товара
                var product = new ProductManagementSystem.Model.Product
                {
                    Id = id,
                    Name = txtName.Text.Trim(),
                    Description = txtDescription.Text.Trim(),
                    Price = numPrice.Value,
                    Category = cmbCategory.SelectedItem.ToString().Trim(),
                    StockQuantity = (int)numStock.Value
                };

                _logic.AddProduct(product);
                RefreshGrid();
                HideAddPanel();

                MessageBox.Show(
                    "Товар успешно добавлен.", 
                    "Готово",
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Не удалось добавить товар: {ex.Message}", 
                    "Ошибка",
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Отмена добавления товара
        /// </summary>
        private void BtnAddCancel_Click(object sender, EventArgs e)
        {
            HideAddPanel();
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

        #region Валидация ввода

        /// <summary>
        /// Фильтрация ввода при нажатии клавиш
        /// </summary>
        private void RussianOnly_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            
            // Разрешаем управляющие символы
            if (char.IsControl(c))
                return;

            // Проверяем допустимость символа
            if (!IsRussianLetterOrSeparator(c))
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Фильтрация при изменении текста (для вставки из буфера)
        /// </summary>
        private void RussianOnly_TextChanged(object sender, EventArgs e)
        {
            if (sender is not TextBox tb)
                return;

            int caretPosition = tb.SelectionStart;
            string filteredText = FilterToRussianLettersAndSeparators(tb.Text);
            
            if (!string.Equals(filteredText, tb.Text, StringComparison.Ordinal))
            {
                tb.Text = filteredText;
                tb.SelectionStart = Math.Min(caretPosition, tb.Text.Length);
            }
        }

        /// <summary>
        /// Проверка допустимости символа
        /// </summary>
        private static bool IsRussianLetterOrSeparator(char c)
        {
            // Разрешаем буквы, цифры и пробелы
            if (char.IsLetterOrDigit(c) || char.IsWhiteSpace(c))
                return true;

            // Разрешаем базовые символы пунктуации
            return c == '-' || c == '_' || c == '\'' || c == '.' || c == ',' || 
                   c == '(' || c == ')' || c == '/' || c == '\\' || c == '+' || 
                   c == ':' || c == '#';
        }

        /// <summary>
        /// Фильтрация строки от недопустимых символов
        /// </summary>
        private static string FilterToRussianLettersAndSeparators(string text)
        {
            var sb = new System.Text.StringBuilder(text.Length);
            
            foreach (char ch in text)
            {
                if (IsRussianLetterOrSeparator(ch) || char.IsControl(ch))
                {
                    sb.Append(ch);
                }
            }
            
            return sb.ToString();
        }

        #endregion
    }
}