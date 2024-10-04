using System.Diagnostics.Tracing;
using System.Windows.Forms;

namespace WinForms
{
    public partial class Form1 : Form
    {
        private List<Point> Points = new();
        private Bitmap trailBitmap; // Bitmap ��� �������� ������

        //�����
        public int LineWidth = 1;
        public int PointRadius = 5;

        private Color PointColor { get; set; } = Color.Black;
        private Color LineColor { get; set; } = Color.Black;
        private System.Windows.Forms.Timer moveTimer = new ();
        private int moveDirectionX = 5; // �������� �������� �� ��� X
        private int moveDirectionY = 5; // �������� �������� �� ��� Y
        private Random random = new Random();
        private Color TrailColor { get; set; } = Color.LightGray; // ���� �����

        //�����
        private bool IsAddingPoints = false; // ���������� ��� ������������ ��������� ������ ���������� �����
        private bool IsMoving = false;      //���������� ��� ������������ ��������� ������ �������� �����
        private bool isDragging = false; // ���� ��� ������������ ��������� ��������������
        private bool isTrailMode = false; // ����� �����
        private int selectedPointIndex = -1; // ������ ��������� �����
        private Point selectedPoint; // �����, ������� �������������
        private enum ShapeType { None, ClosedCurve, Polygone, Bezier, FilledCurve }
        private ShapeType currentShapeType = ShapeType.None;
        private Button currentLineButton; // ������, ��������������� �������� ���� �����

        public Form1()
        {
            InitializeComponent();

            this.Size = new System.Drawing.Size(800, 600); // ��������� ������� �����
            this.StartPosition = FormStartPosition.CenterScreen; // ��������� ��� �������
            this.MinimumSize = new System.Drawing.Size(400, 300); // ����������� ������
            this.MaximumSize = new System.Drawing.Size(1200, 900); // ������������ ������
            this.Text = "������������ ������ �3"; // ��������� �����

            this.KeyPreview = true; // ��������� ����� �������� ������� ���������� ����� ��������� ����������
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

            // ��������� �������
            moveTimer.Interval = 30; // ��������� ��������� � �������������
            moveTimer.Tick += new EventHandler(TimerTickHandler);
            trailBitmap = new Bitmap(this.ClientSize.Width, this.ClientSize.Height); // ������������� Bitmap ��� ������
        }

        private void SelectLineType(Button selectedButton, ShapeType shapeType)
        {
            // ���� ��� ������� ������ �����
            if (currentLineButton != null && currentLineButton != selectedButton)
            {
                // ���������� �������� ���� ���������� ������
                currentLineButton.BackColor = Color.White; // ���������� ����
            }

            // ������������� ����� ������ ��� �������
            currentLineButton = selectedButton;

            // ������ ���� ����� ������ �� ��������
            selectedButton.BackColor = Color.DarkSeaGreen; // �������� ����

            // ������������� ������� ��� �����
            currentShapeType = shapeType;

            this.Invalidate(); // �������������� ����� ��� ����������� ���������
        }

        private void ButtonPoints_Click(object sender, EventArgs e)
        {
            IsAddingPoints = !IsAddingPoints; // ����������� ���������
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
            if (Points.Count < 3) // ��� ��������� ������ ����� ������� 3 �����
            {
                MessageBox.Show("��� ���������� ��������� ������ ���������� ��� ������� 3 �����.", "������", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SelectLineType((Button)sender, ShapeType.ClosedCurve); // �������� ����� ���� �����

            this.Invalidate();  // ������� ����� ��� �����������
        }

        private void ButtonPolygone_Click(object sender, EventArgs e)
        {
            if (Points.Count < 2) // ��� �������������� ����� ������� 2 �����
            {
                MessageBox.Show("��� ���������� ������� ����� ���������� ��� ������� 2 �����.", "������", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SelectLineType((Button)sender, ShapeType.Polygone); // �������� ����� ���� �����
            this.Invalidate(); // ������� ����� ��� �����������
        }

        private void ButtonDrawBezier_Click(object sender, EventArgs e)
        {
            if (Points.Count < 4 && Points.Count % 3 == 1) // ��� ������ ����� ����� ������� 4 �����
            {
                MessageBox.Show("��� ���������� ������ ����� ���������� ��� ������� 4 ����� � �� ���������� ������ ���� ������ 3 ���� 1 (�������� 4, 7 ��� 10).", "������", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SelectLineType((Button)sender, ShapeType.Bezier); // �������� ����� ���� �����
            this.Invalidate(); // ������� ����� ��� �����������
        }

        private void ButtonFillCurve_Click(object sender, EventArgs e)
        {
            if (Points.Count < 3) // ��� ����������� ������ ����� ������� 3 �����
            {
                MessageBox.Show("��� ���������� ����������� ������ ���������� ��� ������� 3 �����.", "������", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SelectLineType((Button)sender, ShapeType.FilledCurve); // �������� ����� ���� �����
            this.Invalidate(); // ������� ����� ��� �����������
        }

        private void ButtonClear_Click(object sender, EventArgs e)
        {
            Points.Clear(); // ������� ������ �����
            currentShapeType = ShapeType.None; // ���������� ��� ������
            this.Invalidate(); // �������������� �����, ����� ���������� ���������
            moveTimer.Stop(); // ������������� ��������
            IsAddingPoints = false;
            IsMoving = false;
            isDragging = false;
            isTrailMode = false;
            moveDirectionX = 5;
            moveDirectionY = 5;
            trailBitmap.Dispose(); // ����������� ������� Bitmap ����� ��������� ������ 
            trailBitmap = new Bitmap(this.ClientSize.Width, this.ClientSize.Height); // ������� ����� Bitmap ��� ������ 
            currentLineButton = null;

            // ������ ���� ���� ������ �� �����
            foreach (Control control in this.Controls)
            {
                if (control is Button button) // ���������, �������� �� ������� �������
                {
                    button.BackColor = Color.White; // ������������� ���� ������ � �����
                }
            }
        }

        private void ButtonTrailMode_Click(object sender, EventArgs e)
        {
            isTrailMode = !isTrailMode; // ����������� ����� �����
            if (isTrailMode) track.BackColor = Color.DarkSeaGreen;
            else track.BackColor = Color.White;
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            if (IsAddingPoints && e.Button == MouseButtons.Left) // ��������� ��������� � ������ ����
            {
                // ��������� ���������� ������� �����
                Point p = e.Location;

                // ��������� � ��������� �����
                Points.Add(p);

                // ���������� ������� Paint ��� ����������� �����
                Refresh(); // �������� ���������� ����� ��� �����������
            }
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (!IsAddingPoints && e.Button == MouseButtons.Left && !IsMoving) // ��������� ��������� ���������� �����
            {
                // ���������, ��������� �� ������ ��� ����� �� �����
                for (int i = 0; i < Points.Count; i++)
                {
                    if (IsPointUnderCursor(Points[i], e.Location))
                    {
                        isDragging = !isDragging; // �������� ����� ��������������
                        selectedPointIndex = i; // ���������� ������ ��������� �����
                        selectedPoint = Points[i]; // ��������� ��������� �����
                        break;
                    }
                }
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging) // ���� �� ������������� �����
            {
                // ��������� ������� ��������� ����� � ������ ��������� �������
                selectedPoint = e.Location;
                Points[selectedPointIndex] = selectedPoint; // ��������� ������� � ���������
                Refresh(); // �������������� ����� ��� ����������� ���������
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (isDragging) // ���� �� ��������� ������ ���� � ������������� �����
            {
                isDragging = false; // ��������� ����� ��������������
                selectedPointIndex = -1; // ���������� ������ ��������� �����
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
            if (IsMoving) moveTimer.Stop(); // ������������� ��������
            else moveTimer.Start(); // ��������� ��������

            IsMoving = !IsMoving;

            if (IsMoving) movement.BackColor = Color.DarkSeaGreen;
            else movement.BackColor = Color.White;
        }

        private void TimerTickHandler(object sender, EventArgs e)
        {
            // ��������� ���������� �����
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

            // ��������� ������������ � ������ �����
            foreach (var point in Points)
            {
                // �������� �� ������������ � ����� �����
                if (point.X <= 0)
                {
                    moveDirectionX = Math.Abs(moveDirectionX); // ����������� ������
                    break;
                }
                // �������� �� ������������ � ������ �����
                else if (point.X >= this.ClientSize.Width)
                {
                    moveDirectionX = -Math.Abs(moveDirectionX); // ����������� �����
                    break;
                }
                // �������� �� ������������ � ������� �����
                else if (point.Y <= 0)
                {
                    moveDirectionY = Math.Abs(moveDirectionY); // ����������� ����
                    break;
                }
                // �������� �� ������������ � ������ �����
                else if (point.Y >= this.ClientSize.Height)
                {
                    moveDirectionY = -Math.Abs(moveDirectionY); // ����������� �����
                    break;
                }
            }

                this.Invalidate(); // �������������� ����� ��� ����������� ���������
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e); // ����� �������� ������

            using (Graphics g = e.Graphics)
            {
                //������ ����
                g.DrawImage(trailBitmap, Point.Empty); // ������ ����������� ������ �� �����

                // ������ ��� �����
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
                        moveTimer.Stop(); // ������������� ��������
                    }
                    else
                    {
                        moveTimer.Start(); // ��������� ��������
                    }
                    e.Handled = true; // ������������� ���� ���������
                    break;

                case Keys.Add: // ������� "+" �� ����������
                case Keys.Oemplus: // ��� "+" �� �������� ����������
                    moveDirectionX += 1; // ����������� �������� �� ��� X
                    moveDirectionY += 1; // ����������� �������� �� ��� Y
                    e.Handled = true; // ������������� ���� ���������
                    break;

                case Keys.Subtract: // ������� "-" �� ����������
                case Keys.OemMinus: // ��� "-" �� �������� ����������
                    if (moveDirectionX > 1 && moveDirectionY > 1) // �� ��������� �������� ���� ������������� ��� �������
                    {
                        moveDirectionX -= 1; // ��������� �������� �� ��� X
                        moveDirectionY -= 1; // ��������� �������� �� ��� Y
                    }
                    e.Handled = true; // ������������� ���� ���������
                    break;

                case Keys.Escape:
                    ButtonClear_Click(sender, e); // �������� ������� �������
                    e.Handled = true; // ������������� ���� ���������
                    break;

                default:
                    break;
            }
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // ���������, �� ��������� �� ������ � ��������� "��������"
            if (IsMoving)
            {
                return base.ProcessCmdKey(ref msg, keyData); // ���� ������ �������, �������� ��������� ������
            }

            // ��������� ������� ������ �������
            switch (keyData)
            {
                case Keys.Up:
                    for (int i = 0; i < Points.Count; i++)
                    {
                        Points[i] = new Point(Points[i].X, Points[i].Y - 5); // �������� ������ ����� �� 5 ��������
                    }
                    this.Invalidate(); // �������������� ����� ��� ����������� ���������
                    return true; // ���������, ��� ������� ����������

                case Keys.Down:
                    for (int i = 0; i < Points.Count; i++)
                    {
                        Points[i] = new Point(Points[i].X, Points[i].Y + 5); // �������� ������ ���� �� 5 ��������
                    }
                    this.Invalidate();
                    return true;

                case Keys.Left:
                    for (int i = 0; i < Points.Count; i++)
                    {
                        Points[i] = new Point(Points[i].X - 5, Points[i].Y); // �������� ������ ����� �� 5 ��������
                    }
                    this.Invalidate();
                    return true;

                case Keys.Right:
                    for (int i = 0; i < Points.Count; i++)
                    {
                        Points[i] = new Point(Points[i].X + 5, Points[i].Y); // �������� ������ ������ �� 5 ��������
                    }
                    this.Invalidate();
                    return true;

                default:
                    return base.ProcessCmdKey(ref msg, keyData); // ��� ��������� ������ �������� ��������� ������
            }
        }
    }
}
