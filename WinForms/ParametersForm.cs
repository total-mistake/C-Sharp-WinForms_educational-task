using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace WinForms
{
    public partial class ParametersForm : Form
    {
        public Color PointColor { get; private set; }
        public Color LineColor { get; private set; }
        public int LineWidth { get; private set; }
        public int PointRadius { get; private set; }
        public ParametersForm(Color currentPointColor, Color currentLineColor, int currentLineWidth, int currentPointRadius)
        {
            InitializeComponent();
            // Установка начальной позиции формы
            this.StartPosition = FormStartPosition.CenterParent;

            PointColor = currentPointColor;
            LineColor = currentLineColor;
            LineWidth = currentLineWidth;
            PointRadius = currentPointRadius;

            // Настройка элементов управления
            SetupColorComboBoxes();
            numericUpDown1.Value = currentLineWidth; // Толщина линий
            numericUpDown2.Value = currentPointRadius; // Размер точек

            button1.Click += button1_Click;
        }

        private void SetupColorComboBoxes()
        {

            object[] colors = new object[] 
            {
                Color.Red,
                Color.Green,
                Color.Blue,
                Color.Yellow,
                Color.Black,
                Color.Purple,
                Color.Orange,
                Color.Brown,
                Color.Magenta
            };
            // Добавляем цвета в comboBox1 (для точек)
            comboBox1.Items.AddRange(colors);

            // Добавляем цвета в comboBox2 (для линий)
            comboBox2.Items.AddRange(colors);

            // Устанавливаем текущие цвета
            comboBox1.SelectedItem = PointColor;
            comboBox2.SelectedItem = LineColor;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Получаем выбранные значения
            PointColor = (Color)comboBox1.SelectedItem;
            LineColor = (Color)comboBox2.SelectedItem;
            LineWidth = (int)numericUpDown1.Value;
            PointRadius = (int)numericUpDown2.Value;

            this.DialogResult = DialogResult.OK; // Устанавливаем результат диалога
            this.Close(); // Закрываем форму
        }
    }
}
