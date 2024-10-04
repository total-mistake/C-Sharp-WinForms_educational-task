using System.Diagnostics.Tracing;
using System.Windows.Forms;

namespace WinForms
{
    public partial class Form1 : Form
    {
        private List<Point> Points = new();
        private Bitmap trailBitmap; // Bitmap для хранения следов

        //Стили
        public int LineWidth = 1;
        public int PointRadius = 5;

        private Color PointColor { get; set; } = Color.Black;
        private Color LineColor { get; set; } = Color.Black;
        private System.Windows.Forms.Timer moveTimer = new ();
        private int moveDirectionX = 5; // Скорость движения по оси X
        private int moveDirectionY = 5; // Скорость движения по оси Y
        private Random random = new Random();
        private Color TrailColor { get; set; } = Color.LightGray; // Цвет следа

        //Флаги
        private bool IsAddingPoints = false; // Переменная для отслеживания состояния режима добавления точек
        private bool IsMoving = false;      //Переменная для отслеживания состояния режима движения точек
        private bool isDragging = false; // Флаг для отслеживания состояния перетаскивания
        private bool isTrailMode = false; // Режим «След»
        private int selectedPointIndex = -1; // Индекс выбранной точки
        private Point selectedPoint; // Точка, которую перетаскиваем
        private enum ShapeType { None, ClosedCurve, Polygone, Bezier, FilledCurve }
        private ShapeType currentShapeType = ShapeType.None;
        private Button currentLineButton; // Кнопка, соответствующая текущему типу линии

        public Form1()
        {
            InitializeComponent();

            this.Size = new System.Drawing.Size(800, 600); // Установка размера формы
            this.StartPosition = FormStartPosition.CenterScreen; // Положение при запуске
            this.MinimumSize = new System.Drawing.Size(400, 300); // Минимальный размер
            this.MaximumSize = new System.Drawing.Size(1200, 900); // Максимальный размер
            this.Text = "Практическая работа №3"; // Заголовок формы

            this.KeyPreview = true; // Позволяет форме получать события клавиатуры перед дочерними элементами
            this.KeyDown += new KeyEventHandler(Form1_KeyDown);
            this.MouseClick += Form1_MouseClick;
            this.MouseDown += Form1_MouseDown;
            this.MouseMove += Form1_MouseMove;
            this.MouseUp += Form1_MouseUp;

            points.Click += ButtonPoints_Click;
            parameters.Click += ButtonParameters_Click;
            curve.Click += ButtonClosedCurve_Click;
            polygone.Click += ButtonPolygone_Click;
            beziers.Click += ButtonDrawBezier_Click;
            fillCurve.Click += ButtonFillCurve_Click;
            clean.Click += ButtonClear_Click;
            movement.Click += ButtonMove_Click;
            track.Click += ButtonTrailMode_Click;

            // Настройка таймера
            moveTimer.Interval = 30; // Установка интервала в миллисекундах
            moveTimer.Tick += new EventHandler(TimerTickHandler);
            trailBitmap = new Bitmap(this.ClientSize.Width, this.ClientSize.Height); // Инициализация Bitmap для следов
        }

        private void SelectLineType(Button selectedButton, ShapeType shapeType)
        {
            // Если уже выбрана другая линия
            if (currentLineButton != null && currentLineButton != selectedButton)
            {
                // Возвращаем исходный цвет предыдущей кнопки
                currentLineButton.BackColor = Color.White; // Неактивный цвет
            }

            // Устанавливаем новую кнопку как текущую
            currentLineButton = selectedButton;

            // Меняем цвет новой кнопки на активный
            selectedButton.BackColor = Color.DarkSeaGreen; // Активный цвет

            // Устанавливаем текущий тип линии
            currentShapeType = shapeType;

            this.Invalidate(); // Перерисовываем форму для отображения изменений
        }

        private void ButtonPoints_Click(object sender, EventArgs e)
        {
            IsAddingPoints = !IsAddingPoints; // Переключаем состояние
            if (IsAddingPoints) points.BackColor = Color.DarkSeaGreen;
            else points.BackColor = Color.White;
        }

        private void ButtonParameters_Click(object sender, EventArgs e)
        {
            ParametersForm parametersForm = new ParametersForm(PointColor, LineColor, LineWidth, PointRadius);

            if (parametersForm.ShowDialog() == DialogResult.OK)
            {
                PointColor = parametersForm.PointColor;
                LineColor = parametersForm.LineColor;
                LineWidth = parametersForm.LineWidth;
                PointRadius = parametersForm.PointRadius;

                Refresh();
            }
        }

        private void ButtonClosedCurve_Click(object sender, EventArgs e)
        {
            if (Points.Count < 3) // Для замкнутой кривой нужно минимум 3 точки
            {
                MessageBox.Show("Для построения замкнутой кривой необходимо как минимум 3 точки.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SelectLineType((Button)sender, ShapeType.ClosedCurve); // Вызываем выбор типа линии

            this.Invalidate();  // Очищаем форму для перерисовки
        }

        private void ButtonPolygone_Click(object sender, EventArgs e)
        {
            if (Points.Count < 2) // Для многоугольника нужно минимум 2 точки
            {
                MessageBox.Show("Для построения ломаной линии необходимо как минимум 2 точки.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SelectLineType((Button)sender, ShapeType.Polygone); // Вызываем выбор типа линии
            this.Invalidate(); // Очищаем форму для перерисовки
        }

        private void ButtonDrawBezier_Click(object sender, EventArgs e)
        {
            if (Points.Count < 4 && Points.Count % 3 == 1) // Для кривых Безье нужно минимум 4 точки
            {
                MessageBox.Show("Для построения кривых Безье необходимо как минимум 4 точки и их количество должно быть кратно 3 плюс 1 (например 4, 7 или 10).", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SelectLineType((Button)sender, ShapeType.Bezier); // Вызываем выбор типа линии
            this.Invalidate(); // Очищаем форму для перерисовки
        }

        private void ButtonFillCurve_Click(object sender, EventArgs e)
        {
            if (Points.Count < 3) // Для заполненной кривой нужно минимум 3 точки
            {
                MessageBox.Show("Для построения заполненной кривой необходимо как минимум 3 точки.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SelectLineType((Button)sender, ShapeType.FilledCurve); // Вызываем выбор типа линии
            this.Invalidate(); // Очищаем форму для перерисовки
        }

        private void ButtonClear_Click(object sender, EventArgs e)
        {
            Points.Clear(); // Очищаем список точек
            currentShapeType = ShapeType.None; // Сбрасываем тип фигуры
            this.Invalidate(); // Перерисовываем форму, чтобы отобразить изменения
            moveTimer.Stop(); // Останавливаем движение
            IsAddingPoints = false;
            IsMoving = false;
            isDragging = false;
            isTrailMode = false;
            moveDirectionX = 5;
            moveDirectionY = 5;
            trailBitmap.Dispose(); // Освобождаем ресурсы Bitmap перед созданием нового 
            trailBitmap = new Bitmap(this.ClientSize.Width, this.ClientSize.Height); // Создаем новый Bitmap для следов 
            currentLineButton = null;

            // Меняем цвет всех кнопок на белый
            foreach (Control control in this.Controls)
            {
                if (control is Button button) // Проверяем, является ли элемент кнопкой
                {
                    button.BackColor = Color.White; // Устанавливаем цвет кнопки в белый
                }
            }
        }

        private void ButtonTrailMode_Click(object sender, EventArgs e)
        {
            isTrailMode = !isTrailMode; // Переключаем режим следа
            if (isTrailMode) track.BackColor = Color.DarkSeaGreen;
            else track.BackColor = Color.White;
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            if (IsAddingPoints && e.Button == MouseButtons.Left) // Проверяем состояние и кнопку мыши
            {
                // Сохраняем координаты курсора мышки
                Point p = e.Location;

                // Добавляем в коллекцию точек
                Points.Add(p);

                // Генерируем событие Paint для перерисовки точек
                Refresh(); // Вызываем обновление формы для перерисовки
            }
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (!IsAddingPoints && e.Button == MouseButtons.Left && !IsMoving) // Проверяем состояние добавления точек
            {
                // Проверяем, находится ли курсор над одной из точек
                for (int i = 0; i < Points.Count; i++)
                {
                    if (IsPointUnderCursor(Points[i], e.Location))
                    {
                        isDragging = !isDragging; // Включаем режим перетаскивания
                        selectedPointIndex = i; // Запоминаем индекс выбранной точки
                        selectedPoint = Points[i]; // Сохраняем выбранную точку
                        break;
                    }
                }
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging) // Если мы перетаскиваем точку
            {
                // Обновляем позицию выбранной точки с учетом положения курсора
                selectedPoint = e.Location;
                Points[selectedPointIndex] = selectedPoint; // Обновляем позицию в коллекции
                Refresh(); // Перерисовываем форму для отображения изменений
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (isDragging) // Если мы отпускаем кнопку мыши и перетаскиваем точку
            {
                isDragging = false; // Выключаем режим перетаскивания
                selectedPointIndex = -1; // Сбрасываем индекс выбранной точки
            }
        }

        private bool IsPointUnderCursor(Point point, Point cursorLocation)
        {
            return Math.Abs(point.X - cursorLocation.X) <= PointRadius && Math.Abs(point.Y - cursorLocation.Y) <= PointRadius;
        }

        private void ButtonMove_Click(object sender, EventArgs e)
        {
            moveDirectionX = 5;
            moveDirectionY = 5;
            if (IsMoving) moveTimer.Stop(); // Останавливаем движение
            else moveTimer.Start(); // Запускаем движение

            IsMoving = !IsMoving;

            if (IsMoving) movement.BackColor = Color.DarkSeaGreen;
            else movement.BackColor = Color.White;
        }

        private void TimerTickHandler(object sender, EventArgs e)
        {
            // Обновляем координаты точек
            for (int i = 0; i < Points.Count; i++)
            {
                Points[i] = new Point(Points[i].X + moveDirectionX, Points[i].Y + moveDirectionY);

                if (isTrailMode)
                {
                    using (Graphics g = Graphics.FromImage(trailBitmap))
                    {
                        g.FillEllipse(new SolidBrush(TrailColor), Points[i].X - PointRadius, Points[i].Y - PointRadius, PointRadius * 2, PointRadius * 2);
                    }
                }
            }

            // Проверяем столкновение с краями формы
            foreach (var point in Points)
            {
                // Проверка на столкновение с левым краем
                if (point.X <= 0)
                {
                    moveDirectionX = Math.Abs(moveDirectionX); // Направление вправо
                    break;
                }
                // Проверка на столкновение с правым краем
                else if (point.X >= this.ClientSize.Width)
                {
                    moveDirectionX = -Math.Abs(moveDirectionX); // Направление влево
                    break;
                }
                // Проверка на столкновение с верхним краем
                else if (point.Y <= 0)
                {
                    moveDirectionY = Math.Abs(moveDirectionY); // Направление вниз
                    break;
                }
                // Проверка на столкновение с нижним краем
                else if (point.Y >= this.ClientSize.Height)
                {
                    moveDirectionY = -Math.Abs(moveDirectionY); // Направление вверх
                    break;
                }
            }

                this.Invalidate(); // Перерисовываем форму для отображения изменений
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e); // Вызов базового метода

            using (Graphics g = e.Graphics)
            {
                //Рисуем след
                g.DrawImage(trailBitmap, Point.Empty); // Рисуем изображение следов на форме

                // Рисуем все точки
                foreach (var point in Points)
                {
                    g.FillEllipse(new SolidBrush(PointColor), point.X - PointRadius, point.Y - PointRadius, PointRadius * 2, PointRadius * 2);
                }

                switch (currentShapeType)
                {
                    case ShapeType.ClosedCurve:
                        if (Points.Count >= 3)
                            g.DrawClosedCurve(new Pen(LineColor, LineWidth), Points.ToArray());
                        break;

                    case ShapeType.Polygone:
                        if (Points.Count >= 2)
                            g.DrawPolygon(new Pen(LineColor, LineWidth), Points.ToArray());
                        break;

                    case ShapeType.Bezier:
                        if (Points.Count >= 4 && Points.Count % 3 == 1)
                            g.DrawBeziers(new Pen(LineColor, LineWidth), Points.ToArray());
                        break;

                    case ShapeType.FilledCurve:
                        if (Points.Count >= 3)
                        {
                            g.FillClosedCurve(new SolidBrush(PointColor), Points.ToArray());
                            g.DrawClosedCurve(new Pen(LineColor, LineWidth), Points.ToArray());
                        }
                        break;
                }
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Space:
                    if (moveTimer.Enabled)
                    {
                        moveTimer.Stop(); // Останавливаем движение
                    }
                    else
                    {
                        moveTimer.Start(); // Запускаем движение
                    }
                    e.Handled = true; // Устанавливаем флаг обработки
                    break;

                case Keys.Add: // Клавиша "+" на клавиатуре
                case Keys.Oemplus: // Для "+" на цифровой клавиатуре
                    moveDirectionX += 1; // Увеличиваем скорость по оси X
                    moveDirectionY += 1; // Увеличиваем скорость по оси Y
                    e.Handled = true; // Устанавливаем флаг обработки
                    break;

                case Keys.Subtract: // Клавиша "-" на клавиатуре
                case Keys.OemMinus: // Для "-" на цифровой клавиатуре
                    if (moveDirectionX > 1 && moveDirectionY > 1) // Не позволяем скорость быть отрицательной или нулевой
                    {
                        moveDirectionX -= 1; // Уменьшаем скорость по оси X
                        moveDirectionY -= 1; // Уменьшаем скорость по оси Y
                    }
                    e.Handled = true; // Устанавливаем флаг обработки
                    break;

                case Keys.Escape:
                    ButtonClear_Click(sender, e); // Вызываем функцию очистки
                    e.Handled = true; // Устанавливаем флаг обработки
                    break;

                default:
                    break;
            }
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Проверяем, не находится ли таймер в состоянии "движение"
            if (IsMoving)
            {
                return base.ProcessCmdKey(ref msg, keyData); // Если таймер включен, передаем обработку дальше
            }

            // Обработка нажатий клавиш стрелок
            switch (keyData)
            {
                case Keys.Up:
                    for (int i = 0; i < Points.Count; i++)
                    {
                        Points[i] = new Point(Points[i].X, Points[i].Y - 5); // Сместить фигуру вверх на 5 пикселей
                    }
                    this.Invalidate(); // Перерисовываем форму для отображения изменений
                    return true; // Указываем, что клавиша обработана

                case Keys.Down:
                    for (int i = 0; i < Points.Count; i++)
                    {
                        Points[i] = new Point(Points[i].X, Points[i].Y + 5); // Сместить фигуру вниз на 5 пикселей
                    }
                    this.Invalidate();
                    return true;

                case Keys.Left:
                    for (int i = 0; i < Points.Count; i++)
                    {
                        Points[i] = new Point(Points[i].X - 5, Points[i].Y); // Сместить фигуру влево на 5 пикселей
                    }
                    this.Invalidate();
                    return true;

                case Keys.Right:
                    for (int i = 0; i < Points.Count; i++)
                    {
                        Points[i] = new Point(Points[i].X + 5, Points[i].Y); // Сместить фигуру вправо на 5 пикселей
                    }
                    this.Invalidate();
                    return true;

                default:
                    return base.ProcessCmdKey(ref msg, keyData); // Для остальных клавиш передаем обработку дальше
            }
        }
    }
}
