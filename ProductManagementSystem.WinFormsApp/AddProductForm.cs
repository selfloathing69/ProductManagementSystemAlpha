using System;
using System.Drawing;
using System.Windows.Forms;
using ProductManagementSystem.Logic;
using ProductManagementSystem.Model;

namespace ProductManagementSystem.WinFormsApp
{
    /// <summary>
    /// SOLID - S: Класс отвечает только за UI формы добавления товара.
    /// 
    /// Форма для добавления нового товара
    /// </summary>
    public partial class AddProductForm : Form
    {
        #region Поля

        /// <summary>
        /// SOLID - D: Зависимость от ProductLogic через конструктор (Constructor Injection).
        /// </summary>
        private readonly ProductLogic _logic;
        
        // Элементы управления
        private NumericUpDown numId;
        private TextBox txtName;
        private TextBox txtDescription;
        private ComboBox cmbCategory;
        private NumericUpDown numPrice;
        private NumericUpDown numStock;
        private Button btnOk;
        private Button btnCancel;

        #endregion

        #region Свойства

        /// <summary>
        /// Созданный товар (доступен после успешного добавления)
        /// </summary>
        public Product? CreatedProduct { get; private set; }

        #endregion

        #region Конструктор

        /// <summary>
        /// SOLID - D: Constructor Injection - внедрение зависимости ProductLogic через конструктор.
        /// 
        /// Конструктор формы добавления товара
        /// </summary>
        /// <param name="logic">Экземпляр бизнес-логики</param>
        public AddProductForm(ProductLogic logic)
        {
            _logic = logic ?? throw new ArgumentNullException(nameof(logic));
            InitializeComponent();
        }

        #endregion

        #region Инициализация компонентов

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text = "Новый товар";
            this.ClientSize = new Size(460, 340);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = SystemColors.Window;
            this.ShowInTaskbar = false;

            Label lblTitle = new Label
            {
                Text = "Новый товар",
                Location = new Point(10, 10),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };

            Label lblId = new Label
            {
                Text = "ID:",
                Location = new Point(10, 45),
                Size = new Size(100, 20)
            };
            numId = new NumericUpDown
            {
                Location = new Point(120, 40),
                Size = new Size(150, 25),
                Minimum = 1M,
                Maximum = 999999M,
                Value = 1M
            };

            Label lblName = new Label
            {
                Text = "Название:",
                Location = new Point(10, 80),
                Size = new Size(100, 20)
            };
            txtName = new TextBox
            {
                Location = new Point(120, 75),
                Size = new Size(300, 25)
            };


            Label lblDesc = new Label
            {
                Text = "Описание:",
                Location = new Point(10, 115),
                Size = new Size(100, 20)
            };
            txtDescription = new TextBox
            {
                Location = new Point(120, 110),
                Size = new Size(300, 25)
            };


            Label lblPrice = new Label
            {
                Text = "Цена:",
                Location = new Point(10, 150),
                Size = new Size(100, 20)
            };
            numPrice = new NumericUpDown
            {
                Location = new Point(120, 145),
                Size = new Size(150, 25),
                Maximum = 1000000M,
                DecimalPlaces = 2,
                Minimum = 0M
            };

            Label lblCategory = new Label
            {
                Text = "Категория:",
                Location = new Point(10, 185),
                Size = new Size(100, 20)
            };
            cmbCategory = new ComboBox
            {
                Location = new Point(120, 180),
                Size = new Size(300, 25),
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

            Label lblStock = new Label
            {
                Text = "Количество:",
                Location = new Point(10, 220),
                Size = new Size(100, 20)
            };
            numStock = new NumericUpDown
            {
                Location = new Point(120, 215),
                Size = new Size(150, 25),
                Maximum = 100000M,
                Minimum = 0M
            };

            // Кнопки
            btnOk = new Button
            {
                Text = "ОК",
                Location = new Point(120, 260),
                Size = new Size(90, 30),
                UseVisualStyleBackColor = true
            };
            btnOk.Click += BtnOk_Click;

            btnCancel = new Button
            {
                Text = "Отмена",
                Location = new Point(240, 260),
                Size = new Size(90, 30),
                UseVisualStyleBackColor = true
            };
            btnCancel.Click += BtnCancel_Click;

            // Добавление всех элементов на форму
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblId);
            this.Controls.Add(numId);
            this.Controls.Add(lblName);
            this.Controls.Add(txtName);
            this.Controls.Add(lblDesc);
            this.Controls.Add(txtDescription);
            this.Controls.Add(lblPrice);
            this.Controls.Add(numPrice);
            this.Controls.Add(lblCategory);
            this.Controls.Add(cmbCategory);
            this.Controls.Add(lblStock);
            this.Controls.Add(numStock);
            this.Controls.Add(btnOk);
            this.Controls.Add(btnCancel);

            // Установка Accept и Cancel кнопок для удобства
            this.AcceptButton = btnOk;
            this.CancelButton = btnCancel;

            this.ResumeLayout(false);
        }

        #endregion

        #region Обработчики событий

        /// <summary>
        /// Подтверждение добавления товара
        /// </summary>
        private void BtnOk_Click(object? sender, EventArgs e)
        {
            try
            {
                // Валидация полей
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
                        string category = cmbCategory.SelectedItem.ToString()!.Trim();

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
                                    var updatedProduct = _logic.GetProduct(existingProduct.Id);
                                    MessageBox.Show(
                                        $"Количество товара увеличено. Новое количество: {updatedProduct?.StockQuantity ?? 0}",
                                        "Готово",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                                    
                                    CreatedProduct = updatedProduct;
                                    this.DialogResult = DialogResult.OK;
                                    this.Close();
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
                var product = new Product
                {
                    Id = id,
                    Name = txtName.Text.Trim(),
                    Description = txtDescription.Text.Trim(),
                    Price = numPrice.Value,
                    Category = cmbCategory.SelectedItem.ToString()!.Trim(),
                    StockQuantity = (int)numStock.Value
                };

                _logic.AddProduct(product);
                CreatedProduct = product;

                MessageBox.Show(
                    "Товар успешно добавлен.",
                    "Готово",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
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
        private void BtnCancel_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }



        #endregion
    }
}
