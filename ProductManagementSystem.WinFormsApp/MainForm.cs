using System;
using System.Windows.Forms;
using ProductManagementSystem.Logic;
using ProductManagementSystem.Model;

namespace ProductManagementSystem.WinFormsApp
{
    /// <summary>
    /// главное окно приложения: тут таблица с товарами и панель для быстрого добавления новых позиций
    /// </summary>
    public partial class MainForm : Form
    {
        // бизнес-логика: все операции с товарами держим в отдельном классе, чтобы форма не пухла
        private ProductLogic _logic = new ProductLogic();

        // таблица со списком товаров (автогенерит колонки из свойств модели)
        private DataGridView dataGridView;

        // кнопки управления: обновить, добавить, удалить
        private Button btnRefresh; // обновить список
        private Button btnAdd;     // показать форму добавления
        private Button btnDelete;  // удалить выбранный товар

        // панель добавления нового товара (появляется поверх формы)
        private Panel addPanel;
        // поля ввода: имя и описание — обычные TextBox, но с ограничением на русские буквы
        private TextBox txtName, txtDescription;
        // категория как выпадающий список — чтобы не было мусора и опечаток
        private ComboBox cmbCategory;
        // цена и количество — числовые поля, без лишних заморочек
        private NumericUpDown numPrice, numStock;
        // кнопки подтверждения и отмены добавления
        private Button btnAddOk, btnAddCancel;

        /// <summary>
        /// конструктор формы: поднимаем контролы и сразу запрашиваем данные
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            RefreshGrid(); // при запуске сразу загрузим список товаров, чтобы не смотреть на пустой экран
        }

        /// <summary>
        /// создаём и настраиваем все контролы формы, подписываемся на события
        /// </summary>
        private void InitializeComponent()
        {
            // базовые настройки окна: заголовок, размеры и позиционирование
            this.Text = "Система управления товарами - Windows Forms";
            this.Width = 1280;
            this.Height = 720;
            this.MinimumSize = new System.Drawing.Size(1280, 720);
            this.StartPosition = FormStartPosition.CenterScreen;

            // таблица для отображения данных
            dataGridView = new DataGridView
            {
                Dock = DockStyle.Top,                 // прибиваем к верхнему краю, чтобы занимала «шапку»
                Height = 400,                         // фиксируем высоту, остальное — под кнопки/панели
                ReadOnly = true,                      // редактирование прямо в гриде не используем
                AutoGenerateColumns = true,           // колонки берутся автоматически из свойств Product
                AllowUserToAddRows = false,           // без пустой последней строки
                AllowUserToDeleteRows = false,        // удаляем только через кнопку
                SelectionMode = DataGridViewSelectionMode.FullRowSelect // выделяем целую строку, так удобнее
            };

            // кнопка обновления — просто перезатираем источник и подгружаем актуальные данные
            btnRefresh = new Button { Text = "Обновить список", Left = 20, Top = 420, Width = 150, Height = 35 };
            btnRefresh.Click += (s, e) => RefreshGrid();

            // кнопка добавления — показывает панель с полями
            btnAdd = new Button { Text = "Добавить товар", Left = 180, Top = 420, Width = 150, Height = 35 };
            btnAdd.Click += (s, e) => ShowAddPanel();

            // кнопка удаления — требует выделенной строки, спрашивает подтверждение
            btnDelete = new Button { Text = "Удалить выбранный", Left = 340, Top = 420, Width = 150, Height = 35 };
            btnDelete.Click += BtnDelete_Click;

            // добавляем контролы на форму (сначала таблицу, потом кнопки — сверху вниз)
            this.Controls.Add(dataGridView);
            this.Controls.Add(btnRefresh);
            this.Controls.Add(btnAdd);
            this.Controls.Add(btnDelete);

            // готовим панель для добавления товара, но пока прячем
            CreateAddPanel();
            this.Controls.Add(addPanel);
            addPanel.Visible = false;
        }

        /// <summary>
        /// удаление выбранного товара: проверяем выбор, спрашиваем подтверждение, обновляем список
        /// </summary>
        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            // аккуратно проверяем, что строка выбрана и там реально есть Id
            if (dataGridView.CurrentRow != null &&
                dataGridView.Columns.Contains("Id") &&
                dataGridView.CurrentRow.Cells["Id"].Value != null)
            {
                var id = (int)dataGridView.CurrentRow.Cells["Id"].Value;

                // спрашиваем пользователя, чтобы случайно не снести лишнее
                var result = MessageBox.Show(
                    $"Вы уверены, что хотите удалить товар с ID {id}?",
                    "Подтверждение удаления",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    _logic.DeleteProduct(id);
                    RefreshGrid();
                    MessageBox.Show("Товар успешно удалён.", "Успех",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите строку для удаления.", "Предупреждение",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// создаёт панель для добавления нового товара с полями и кнопками
        /// </summary>
        private void CreateAddPanel()
        {
            // размеры панели — чтобы смотрелось аккуратно по центру
            int panelWidth = 460;
            int panelHeight = 300;

            addPanel = new Panel
            {
                Width = panelWidth,
                Height = panelHeight,
                BorderStyle = BorderStyle.FixedSingle,                // тонкая рамка, чтобы визуально отделить
                BackColor = System.Drawing.SystemColors.Control,      // фон как у формы
                Left = 120,
                Top = 120,
            };

            // заголовок панели — просто чтобы было понятно, что это за блок
            Label lblTitle = new Label { Text = "Новый товар", Left = 10, Top = 10, Width = 200, Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold) };

            // поле «Название» — обычный TextBox, но ниже ограничим ввод русскими символами
            Label lblName = new Label { Text = "Название:", Left = 10, Top = 45, Width = 100 };
            txtName = new TextBox { Left = 120, Top = 40, Width = 300 };
            // разрешаем только русские буквы (и пробел/дефис), чтобы не было латиницы и цифр
            txtName.KeyPress += RussianOnly_KeyPress;
            txtName.TextChanged += RussianOnly_TextChanged;

            // поле «Описание» — по тем же правилам, только русские плюс пробел/дефис
            Label lblDesc = new Label { Text = "Описание:", Left = 10, Top = 80, Width = 100 };
            txtDescription = new TextBox { Left = 120, Top = 75, Width = 300 };
            txtDescription.KeyPress += RussianOnly_KeyPress;
            txtDescription.TextChanged += RussianOnly_TextChanged;

            // цена — decimal, две цифры после запятой, без экстрима
            Label lblPrice = new Label { Text = "Цена:", Left = 10, Top = 115, Width = 100 };
            numPrice = new NumericUpDown
            {
                Left = 120,
                Top = 110,
                Width = 150,
                Maximum = 1000000M,   // верхнее ограничение, чтобы случайно не уехали в космос
                DecimalPlaces = 2,    // копейки учитываем
                Minimum = 0M          // отрицательных цен не допускаем
            };

            // категория — выпадающий список, чтобы данные были консистентные
            Label lblCategory = new Label { Text = "Категория:", Left = 10, Top = 150, Width = 100 };
            cmbCategory = new ComboBox
            {
                Left = 120,
                Top = 145,
                Width = 300,
                DropDownStyle = ComboBoxStyle.DropDownList // выбор только из списка, руками не печатаем
            };
            // категории
            cmbCategory.Items.AddRange(new object[] { "Электроника", "Одежда", "Обувь", "Разное" });

            // количество — целое число, тоже без отрицательных значений
            Label lblStock = new Label { Text = "Количество:", Left = 10, Top = 185, Width = 100 };
            numStock = new NumericUpDown
            {
                Left = 120,
                Top = 180,
                Width = 150,
                Maximum = 100000M, // на всякий случай потолок
                Minimum = 0M
            };

            // кнопка «ок» — сохраняем и закрываем панель; «отмена» — просто закрываем
            btnAddOk = new Button { Text = "ОК", Left = 120, Top = 225, Width = 90 };
            btnAddCancel = new Button { Text = "Отмена", Left = 240, Top = 225, Width = 90 };

            btnAddOk.Click += BtnAddOk_Click;
            btnAddCancel.Click += (s, e) => HideAddPanel();

            // раскладываем всё по панели
            addPanel.Controls.Add(lblTitle);
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

            // простая центровка панели при ресайзе формы — чтобы не уезжала в угол
            this.Resize += (s, e) =>
            {
                addPanel.Left = Math.Max(20, (this.ClientSize.Width - addPanel.Width) / 2);
                addPanel.Top = Math.Max(60, (this.ClientSize.Height - addPanel.Height) / 2 - 40);
            };
        }

        /// <summary>
        /// показывает панель добавления и сбрасывает прошлые значения, чтобы не тянуть за собой мусор
        /// </summary>
        private void ShowAddPanel()
        {
            // чистим поля, чтобы пользователь не подправлял старые данные
            txtName.Text = string.Empty;
            txtDescription.Text = string.Empty;
            cmbCategory.SelectedIndex = -1;
            numPrice.Value = 0M;
            numStock.Value = 0M;

            // показываем панель поверх и блокируем остальную форму на время ввода
            addPanel.Visible = true;
            addPanel.BringToFront();

            btnRefresh.Enabled = false;
            btnAdd.Enabled = false;
            btnDelete.Enabled = false;
            dataGridView.Enabled = false;

            // ставим курсор в название — обычно с этого и начинают
            txtName.Focus();
        }

        /// <summary>
        /// прячет панель добавления и возвращает доступ к остальным элементам
        /// </summary>
        private void HideAddPanel()
        {
            addPanel.Visible = false;
            btnRefresh.Enabled = true;
            btnAdd.Enabled = true;
            btnDelete.Enabled = true;
            dataGridView.Enabled = true;
        }

        /// <summary>
        /// сохраняет новый товар, валидирует обязательные поля и обновляет таблицу
        /// </summary>
        private void BtnAddOk_Click(object? sender, EventArgs e)
        {
            try
            {
                // минимальная проверка: без названия и категории — вообще никуда
                if (string.IsNullOrWhiteSpace(txtName.Text) || cmbCategory.SelectedItem == null)
                {
                    MessageBox.Show("Поля 'Название' и 'Категория' обязательны!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // собираем товар из полей — ничего хитрого, всё по модели
                var product = new Product
                {
                    Name = txtName.Text.Trim(),
                    Description = txtDescription.Text.Trim(),
                    Price = numPrice.Value, // decimal напрямую, совпадает с типом в модели
                    Category = cmbCategory.SelectedItem?.ToString()!.Trim(),
                    StockQuantity = (int)numStock.Value
                };

                // пробрасываем в логику и обновляем грид
                _logic.AddProduct(product);
                RefreshGrid();

                // закрываем панель и говорим пользователю, что всё хорошо
                HideAddPanel();

                MessageBox.Show("Товар добавлен.", "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                // на всякий пожарный ловим исключение и показываем нормальное сообщение
                MessageBox.Show($"Не удалось добавить товар: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// обновляет источник данных грида и наводит красоту в заголовках
        /// </summary>
        private void RefreshGrid()
        {
            try
            {
                // сбрасываем источник, чтобы грид точно перечитал данные
                dataGridView.DataSource = null;
                dataGridView.DataSource = _logic.GetAllProducts();

                // подписываем человеко-понятные заголовки колонок
                if (dataGridView.Columns.Count > 0)
                {
                    if (dataGridView.Columns.Contains("Id")) dataGridView.Columns["Id"].HeaderText = "ID";
                    if (dataGridView.Columns.Contains("Name")) dataGridView.Columns["Name"].HeaderText = "Название";
                    if (dataGridView.Columns.Contains("Description")) dataGridView.Columns["Description"].HeaderText = "Описание";
                    if (dataGridView.Columns.Contains("Price")) dataGridView.Columns["Price"].HeaderText = "Цена";
                    if (dataGridView.Columns.Contains("Category")) dataGridView.Columns["Category"].HeaderText = "Категория";
                    if (dataGridView.Columns.Contains("StockQuantity")) dataGridView.Columns["StockQuantity"].HeaderText = "Количество";

                    dataGridView.AutoResizeColumns(); // подтягиваем ширину колонок под контент
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении таблицы: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// фильтрует ввод по клавиатуре: пропускаем только русские буквы, пробел и дефис (плюс служебные клавиши)
        /// </summary>
        private void RussianOnly_KeyPress(object? sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            if (char.IsControl(c)) return; // системные клавиши (backspace/delete/enter и т.п.) не режем
            if (!IsRussianLetterOrSeparator(c))
            {
                e.Handled = true; // всё остальное гасим на подлёте
            }
        }

        /// <summary>
        /// чистит буфер после вставки: если пользователь вставил что-то «левое», мы это аккуратно выбросим
        /// </summary>
        private void RussianOnly_TextChanged(object? sender, EventArgs e)
        {
            if (sender is not TextBox tb) return;

            int caret = tb.SelectionStart;
            string filtered = FilterToRussianLettersAndSeparators(tb.Text);
            if (!string.Equals(filtered, tb.Text, StringComparison.Ordinal))
            {
                tb.Text = filtered;
                tb.SelectionStart = Math.Min(caret, tb.Text.Length);
            }
        }

        /// <summary>
        /// проверяет, является ли символ русской буквой или допустимым разделителем (пробел/дефис)
        /// </summary>
        private static bool IsRussianLetterOrSeparator(char c)
        {
            // русские буквы: А-Я (0410-042F), а-я (0430-044F), Ё (0401), ё (0451)
            // плюс пробел и дефис — этого обычно хватает для названий
            return (c >= '\u0410' && c <= '\u044F') || c == '\u0401' || c == '\u0451' || c == ' ' || c == '-';
        }

        /// <summary>
        /// пробегается по строке и выбрасывает все символы, которые нам не подходят
        /// </summary>
        private static string FilterToRussianLettersAndSeparators(string s)
        {
            var sb = new System.Text.StringBuilder(s.Length);
            foreach (char ch in s)
            {
                if (IsRussianLetterOrSeparator(ch) || char.IsControl(ch))
                    sb.Append(ch);
            }
            return sb.ToString();
        }
    }
}